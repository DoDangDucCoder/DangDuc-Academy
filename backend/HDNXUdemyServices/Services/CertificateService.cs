using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HDNXUdemyServices.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ProjectContext _context;
        private readonly IAuditLogService _auditLogService;

        static CertificateService()
        {
            // QuestPDF Community License — free for open-source projects
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public CertificateService(IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService)
        {
            _context = new ProjectContext(httpContextAccessor);
            _auditLogService = auditLogService;
        }

        public async Task<CertificateModel> GenerateCertificate(long idCourse, long idStudent)
        {
            var existing = await _context.Certificates!.FirstOrDefaultAsync(x => x.IdCourse == idCourse && x.IdStudent == idStudent);
            if (existing != null)
            {
                return ToModel(existing);
            }

            var course = await _context.Courses!.FindAsync(idCourse) ?? throw new ProjectNotFoundException("Course not found.");
            var student = await _context.Users!.FindAsync(idStudent) ?? throw new ProjectNotFoundException("Student not found.");
            await EnsureCourseCompleted(idCourse, idStudent);
            var code = $"CERT-{idCourse}-{idStudent}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var certificate = new CertificateEntities
            {
                IdCourse = idCourse,
                IdStudent = idStudent,
                CertificateCode = code,
                IssuedDate = LocalDateTime.FromDateTime(DateTime.UtcNow),
                FileUrl = $"/api/v1/certificate/{code}/download",
                Status = (int)EStatus.Active
            };
            _context.Certificates!.Add(certificate);
            await _context.SaveChangesAsync();
            await _auditLogService.WriteAsync(idStudent, "Certificate.Generate", "Certificate", certificate.Id.ToString(), null, new { course.Title, student.Email, code });
            return ToModel(certificate);
        }

        private async Task EnsureCourseCompleted(long idCourse, long idStudent)
        {
            var contentIds = await _context.ContentCourses!.AsNoTracking()
                .Where(x => x.IdCourse == idCourse && x.Status == (int)EStatus.Active)
                .Select(x => x.Id)
                .ToListAsync();

            var totalLessons = contentIds.Count == 0
                ? 0
                : await _context.ContentCourseDetails!.AsNoTracking()
                    .CountAsync(x => contentIds.Contains(x.IdContent) && x.Status == (int)EStatus.Active);

            if (totalLessons <= 0)
            {
                throw new ProjectBadRequestException("Course has no active lessons to complete.");
            }

            var process = await _context.StudentProcess!.AsNoTracking()
                .Where(x => x.IdCourse == idCourse && x.CreateBy == idStudent && x.Status == (int)EStatus.Active)
                .OrderByDescending(x => x.NumberContentOfCourse)
                .FirstOrDefaultAsync();

            if (process == null || process.NumberContentOfCourse < totalLessons)
            {
                throw new ProjectBadRequestException("Course is not completed yet.");
            }
        }

        public async Task<CertificateModel?> GetCertificate(long idCourse, long idStudent)
        {
            var certificate = await _context.Certificates!.AsNoTracking().FirstOrDefaultAsync(x => x.IdCourse == idCourse && x.IdStudent == idStudent);
            return certificate == null ? null : ToModel(certificate);
        }

        public async Task<byte[]> DownloadCertificate(long id)
        {
            var certificate = await _context.Certificates!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ProjectNotFoundException("Certificate not found.");
            var course = await _context.Courses!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == certificate.IdCourse);
            var student = await _context.Users!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == certificate.IdStudent);

            var studentName = student?.Name ?? student?.Email ?? $"Student #{certificate.IdStudent}";
            var courseTitle = course?.Title ?? "Course";
            var issuedDate = certificate.IssuedDate.ToDateTimeUnspecified().ToString("MMMM dd, yyyy");

            return BuildQuestPdf(certificate.CertificateCode, courseTitle, studentName, issuedDate);
        }

        private static CertificateModel ToModel(CertificateEntities entity) => new()
        {
            Id = entity.Id,
            IdCourse = entity.IdCourse,
            IdStudent = entity.IdStudent,
            CertificateCode = entity.CertificateCode,
            IssuedDate = entity.IssuedDate,
            FileUrl = entity.FileUrl,
            Status = entity.Status ?? (int)EStatus.Active
        };

        private static byte[] BuildQuestPdf(string code, string courseTitle, string studentName, string issuedDate)
        {
            // Colors
            var primaryBlue = Colors.Blue.Darken3;
            var accentGold = "#C9A84C";
            var lightBg = "#F8F6F0";
            var darkText = Colors.Grey.Darken4;
            var mutedText = Colors.Grey.Darken1;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontColor(darkText));

                    page.Content().Column(col =>
                    {
                        // ── HEADER BANNER ──────────────────────────────────────────
                        col.Item().Height(120).Background(primaryBlue).Padding(20).Row(row =>
                        {
                            row.RelativeItem().Column(inner =>
                            {
                                inner.Item().Text("HDNX")
                                    .FontSize(42).Bold()
                                    .FontColor(Colors.White);
                                inner.Item().Text("Online Learning Platform")
                                    .FontSize(13).FontColor("#A8C8F0");
                            });

                            row.ConstantItem(200).AlignRight().Column(inner =>
                            {
                                inner.Item().AlignRight().Text("Certificate of Completion")
                                    .FontSize(16).Bold().FontColor(Colors.White).Italic();
                            });
                        });

                        // ── GOLD ACCENT LINE ────────────────────────────────────────
                        col.Item().Height(6).Background(accentGold);

                        // ── MAIN BODY ───────────────────────────────────────────────
                        col.Item().Background(lightBg).Padding(40).Column(body =>
                        {
                            // Subtitle
                            body.Item().AlignCenter().PaddingBottom(8)
                                .Text("This is to certify that")
                                .FontSize(15).FontColor(mutedText).Italic();

                            // Student name with decorative line
                            body.Item().AlignCenter().PaddingVertical(8).Column(nameBlock =>
                            {
                                nameBlock.Item().AlignCenter()
                                    .BorderBottom(2).BorderColor(accentGold)
                                    .PaddingHorizontal(60).PaddingBottom(8)
                                    .Text(studentName)
                                    .FontSize(38).Bold().FontColor(primaryBlue);
                            });

                            body.Item().Height(8);

                            // "has successfully completed" text
                            body.Item().AlignCenter().PaddingBottom(6)
                                .Text("has successfully completed the course")
                                .FontSize(15).FontColor(mutedText).Italic();

                            // Course title box
                            body.Item().AlignCenter().PaddingHorizontal(60).PaddingVertical(12)
                                .Background(Colors.White)
                                .Border(2).BorderColor(accentGold)
                                .Padding(16)
                                .Text(courseTitle)
                                .FontSize(24).Bold().FontColor(darkText).AlignCenter();

                            body.Item().Height(20);

                            // Date and code row
                            body.Item().Row(row =>
                            {
                                // Issued date
                                row.RelativeItem().AlignLeft().Column(c =>
                                {
                                    c.Item().Text("Date of Completion").FontSize(11).FontColor(mutedText);
                                    c.Item().Text(issuedDate).FontSize(14).Bold().FontColor(darkText);
                                });

                                // Decorative seal (text-based)
                                row.ConstantItem(120).AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter()
                                        .Width(80).Height(80)
                                        .Border(3).BorderColor(accentGold)
                                        .Background(Colors.White)
                                        .Padding(4)
                                        .AlignMiddle()
                                        .Text("✓\nVERIFIED")
                                        .FontSize(12).Bold().FontColor(accentGold).AlignCenter();
                                });

                                // Certificate code
                                row.RelativeItem().AlignRight().Column(c =>
                                {
                                    c.Item().AlignRight().Text("Certificate ID").FontSize(11).FontColor(mutedText);
                                    c.Item().AlignRight().Text(code).FontSize(11).Bold().FontColor(darkText);
                                });
                            });
                        });

                        // ── FOOTER ──────────────────────────────────────────────────
                        col.Item().Height(6).Background(accentGold);
                        col.Item().Background(primaryBlue).Height(40).Padding(10).AlignMiddle()
                            .Text($"Verify at: hdnx.edu.vn/verify/{code}  |  © {DateTime.UtcNow.Year} HDNX Online Learning Platform")
                            .FontSize(10).FontColor("#A8C8F0").AlignCenter();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}

