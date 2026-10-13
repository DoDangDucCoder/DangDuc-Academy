using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HDNXUdemyServices.Services
{
    public class InstructorDashboardService : IInstructorDashboardService
    {
        private readonly ProjectContext _context;

        public InstructorDashboardService(IHttpContextAccessor httpContextAccessor)
        {
            _context = new ProjectContext(httpContextAccessor);
        }

        public async Task<InstructorDashboardModel> GetDashboard(long instructorId)
        {
            var courses = await _context.Courses!.AsNoTracking()
                .Where(x => x.IdAuthor == instructorId && x.Status == (int)EStatus.Active)
                .ToListAsync();
            var courseIds = courses.Select(x => x.Id).ToList();
            var purchases = await _context.PurcharseCourseDetails!.AsNoTracking()
                .Where(x => courseIds.Contains(x.IdCourse))
                .ToListAsync();
            var reviews = await _context.CourseEvaluations!.AsNoTracking()
                .Where(x => courseIds.Contains(x.IdCourse))
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToListAsync();
            var processes = await _context.StudentProcess!.AsNoTracking()
                .Where(x => courseIds.Contains(x.IdCourse))
                .ToListAsync();

            var totalLessons = await _context.ContentCourseDetails!.AsNoTracking()
                .Where(x => _context.ContentCourses!.Where(c => courseIds.Contains(c.IdCourse)).Select(c => c.Id).Contains(x.IdContent))
                .CountAsync();
            var completed = processes.Count(x => totalLessons == 0 || x.LastContentOfContent >= totalLessons);

            return new InstructorDashboardModel
            {
                InstructorId = instructorId,
                TotalCourses = courses.Count,
                TotalStudents = purchases.Select(x => x.IdStudent).Distinct().Count(),
                TotalRevenue = purchases.Sum(x => x.PriceOfDiscount ?? x.PriceOfCourse ?? 0),
                CompletionRate = processes.Count == 0 ? 0 : Math.Round(completed * 100m / processes.Count, 2),
                Courses = courses.Select(course =>
                {
                    var coursePurchases = purchases.Where(x => x.IdCourse == course.Id).ToList();
                    var courseReviews = reviews.Where(x => x.IdCourse == course.Id).ToList();
                    return new InstructorCourseMetricModel
                    {
                        CourseId = course.Id,
                        Title = course.Title,
                        Students = coursePurchases.Select(x => x.IdStudent).Distinct().Count(),
                        Revenue = coursePurchases.Sum(x => x.PriceOfDiscount ?? x.PriceOfCourse ?? 0),
                        AverageRating = courseReviews.Count == 0 ? 0 : Math.Round((decimal)courseReviews.Average(x => x.VoteStartNumber), 2)
                    };
                }).ToList(),
                RecentReviews = reviews.Select(x => new InstructorReviewModel
                {
                    CourseId = x.IdCourse,
                    StudentId = x.IdStudent,
                    Rating = x.VoteStartNumber,
                    Comment = x.CommentEvaluation
                }).ToList()
            };
        }
    }
}
