using DNXUdemyData.Entities;
using HDNXUdemyData.Entities;
using HDNXUdemyData.SeedData;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HDNXUdemyData.EntitiesContext
{
    public class ProjectContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Static cached data source to prevent ManyServiceProvidersCreatedWarning
        private static NpgsqlDataSource? _dataSource;
        private static readonly object _lock = new();

        private static NpgsqlDataSource GetDataSource()
        {
            if (_dataSource == null)
            {
                lock (_lock)
                {
                    if (_dataSource == null)
                    {
                        var builder = new NpgsqlDataSourceBuilder(ProjectConfig.ConnectionString ?? "");
                        builder.UseNodaTime();
                        _dataSource = builder.Build();
                    }
                }
            }
            return _dataSource;
        }

        public ProjectContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(GetDataSource(), o => o.UseNodaTime());

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning));
            }
        }

        public virtual DbSet<NotificationEntities>? Notifications { get; set; }
        public virtual DbSet<BookmarkCourseEntities>? BookmarkCourses { get; set; }
        public virtual DbSet<BannerEntities>? Banners { get; set; }
        public virtual DbSet<CategoryEntities>? Categories { get; set; }
        public virtual DbSet<TheadQuestionCourseEntities>? TheadQuestionCourses { get; set; }
        public virtual DbSet<ContentCourseEntities>? ContentCourses { get; set; }
        public virtual DbSet<ContentCourseDetailEntities>? ContentCourseDetails { get; set; }
        public virtual DbSet<CourseEntities>? Courses { get; set; }
        public virtual DbSet<InformationManualBankingEntities>? InformationManualBankings { get; set; }
        public virtual DbSet<PurcharseCourseEntities>? PurcharseCourses { get; set; }
        public virtual DbSet<PurcharseCourseDetailsEntities>? PurcharseCourseDetails { get; set; }
        public virtual DbSet<UserEntities>? Users { get; set; }
        public virtual DbSet<StudentProcessEntities>? StudentProcess { get; set; }
        public virtual DbSet<StudentPromotionEntities>? StudentPromotions { get; set; }
        public virtual DbSet<SubCategoryEntities>? SubCategories { get; set; }
        public virtual DbSet<FileManagerEntities>? FileManagers { get; set; }
        public virtual DbSet<SystemConfigEntities>? SystemConfigs { get; set; }
        public virtual DbSet<PartnerEntities>? Partners { get; set; }
        public virtual DbSet<CourseEvaluationEntities>? CourseEvaluations { get; set; }
        public virtual DbSet<DetailTheadQuestionCourseEntities>? DetailTheadQuestionCourses { get; set; }

        public virtual DbSet<CouponEntities>? Coupons { get; set; }
        public virtual DbSet<PromotionCodeEntities>? PromotionCodes { get; set; }
        public virtual DbSet<SubtitleEntities>? Subtitles { get; set; }
        public virtual DbSet<RefundRequestEntities>? RefundRequests { get; set; }
        public virtual DbSet<QuizEntities>? Quizzes { get; set; }
        public virtual DbSet<QuizQuestionEntities>? QuizQuestions { get; set; }
        public virtual DbSet<QuizAnswerOptionEntities>? QuizAnswerOptions { get; set; }
        public virtual DbSet<QuizAttemptEntities>? QuizAttempts { get; set; }
        public virtual DbSet<QuizAttemptAnswerEntities>? QuizAttemptAnswers { get; set; }
        public virtual DbSet<LiveClassEntities>? LiveClasses { get; set; }
        public virtual DbSet<CertificateEntities>? Certificates { get; set; }
        public virtual DbSet<AuditLogEntities>? AuditLogs { get; set; }
        public virtual DbSet<CartEntities>? Carts { get; set; }
        public virtual DbSet<CartItemEntities>? CartItems { get; set; }
        public virtual DbSet<CourseNoteEntities>? CourseNotes { get; set; }
        public virtual DbSet<AssignmentEntities>? Assignments { get; set; }
        public virtual DbSet<AssignmentSubmissionEntities>? AssignmentSubmissions { get; set; }
        public virtual DbSet<PayoutRequestEntities>? PayoutRequests { get; set; }
        public virtual DbSet<Article>? Articles { get; set; }
        public virtual DbSet<InstructorPaymentMethodEntities>? InstructorPaymentMethods { get; set; }
        public virtual DbSet<VideoPlaybackPositionEntities>? VideoPlaybackPositions { get; set; }
        public virtual DbSet<VideoEngagementEntities>? VideoEngagements { get; set; }
        public virtual DbSet<AnnouncementEntities>? Announcements { get; set; }
        public virtual DbSet<QAVoteEntities>? QAVotes { get; set; }
        public virtual DbSet<LessonCompletionEntities>? LessonCompletions { get; set; }
        public virtual DbSet<LearningStreakEntities>? LearningStreaks { get; set; }
        public virtual DbSet<AchievementBadgeEntities>? AchievementBadges { get; set; }
        public virtual DbSet<UserBadgeEntities>? UserBadges { get; set; }
        public virtual DbSet<XPTransactionEntities>? XPTransactions { get; set; }
        public virtual DbSet<LearningPathEntities>? LearningPaths { get; set; }
        public virtual DbSet<LearningPathCourseEntities>? LearningPathCourses { get; set; }
        public virtual DbSet<CoursePrerequisiteEntities>? CoursePrerequisites { get; set; }
        public virtual DbSet<RefreshTokenEntities>? RefreshTokens { get; set; }
        public virtual DbSet<PasswordResetTokenEntities>? PasswordResetTokens { get; set; }

        // ── Language Learning ────────────────────────────────────────────────
        public virtual DbSet<LanguageExamEntities>? LanguageExams { get; set; }
        public virtual DbSet<ExamPartEntities>? ExamParts { get; set; }
        public virtual DbSet<ExamQuestionEntities>? ExamQuestions { get; set; }
        public virtual DbSet<ExamAnswerOptionEntities>? ExamAnswerOptions { get; set; }
        public virtual DbSet<UserExamAttemptEntities>? UserExamAttempts { get; set; }
        public virtual DbSet<VocabularyItemEntities>? VocabularyItems { get; set; }
        public virtual DbSet<UserVocabProgressEntities>? UserVocabProgress { get; set; }
        public virtual DbSet<UserLearningStreakEntities>? UserLearningStreaks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ProjectConfig.DefaultSchema);
            modelBuilder.Entity<NotificationEntities>().ToTable("Notifications").HasKey(x => x.Id);
            modelBuilder.Entity<BookmarkCourseEntities>().ToTable("BookmarkCourses").HasKey(x => x.Id);
            modelBuilder.Entity<BannerEntities>().ToTable("Banners").HasKey(x => x.Id);
            modelBuilder.Entity<CategoryEntities>().ToTable("Categories").HasKey(x => x.Id);
            modelBuilder.Entity<TheadQuestionCourseEntities>().ToTable("TheadQuestionCourses").HasKey(x => x.Id);
            modelBuilder.Entity<ContentCourseEntities>().ToTable("ContentCourses").HasKey(x => x.Id);
            modelBuilder.Entity<ContentCourseDetailEntities>().ToTable("ContentCourseDetails").HasKey(x => x.Id);
            modelBuilder.Entity<InformationManualBankingEntities>().ToTable("InformationManualBankings").HasKey(x => x.Id);
            modelBuilder.Entity<PurcharseCourseEntities>().ToTable("PurcharseCourses").HasKey(x => x.Id);
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().ToTable("PurcharseCourseDetails").HasKey(x => x.Id);
            modelBuilder.Entity<UserEntities>().ToTable("Users").HasKey(x => x.Id);
            modelBuilder.Entity<StudentProcessEntities>().ToTable("StudentProcess").HasKey(x => x.Id);
            modelBuilder.Entity<StudentPromotionEntities>().ToTable("StudentPromotions").HasKey(x => x.Id);
            modelBuilder.Entity<SubCategoryEntities>().ToTable("SubCategories").HasKey(x => x.Id);
            modelBuilder.Entity<FileManagerEntities>().ToTable("FileManagers").HasKey(x => x.Id);
            modelBuilder.Entity<SystemConfigEntities>().ToTable("SystemConfigs").HasKey(x => x.Id);
            modelBuilder.Entity<PartnerEntities>().ToTable("Partners").HasKey(x => x.Id);
            modelBuilder.Entity<CourseEvaluationEntities>().ToTable("CourseEvaluations").HasKey(x => x.Id);
            modelBuilder.Entity<DetailTheadQuestionCourseEntities>().ToTable("DetailTheadQuestionCourses").HasKey(x => x.Id);
            modelBuilder.Entity<CouponEntities>().ToTable("Coupons").HasKey(x => x.Id);
            modelBuilder.Entity<PromotionCodeEntities>().ToTable("PromotionCodes").HasKey(x => x.Id);
            modelBuilder.Entity<UserEntities>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<PurcharseCourseEntities>().HasIndex(x => x.PurcharseCode).IsUnique();
            modelBuilder.Entity<CouponEntities>().HasIndex(x => x.StripeCouponId).IsUnique();
            modelBuilder.Entity<PromotionCodeEntities>().HasIndex(x => x.StripePromotionCodeId).IsUnique();
            modelBuilder.Entity<BookmarkCourseEntities>().HasIndex(x => new { x.IdStudent, x.IdCourse }).IsUnique();
            modelBuilder.Entity<CourseEvaluationEntities>().HasIndex(x => new { x.IdStudent, x.IdCourse }).IsUnique();
            modelBuilder.Entity<CourseEntities>().ToTable(t => t.HasCheckConstraint("CK_Courses_Prices_NonNegative", "\"PriceOfCourse\" >= 0 AND \"PriceOfDiscount\" >= 0"));
            modelBuilder.Entity<PurcharseCourseEntities>().ToTable(t => t.HasCheckConstraint("CK_PurcharseCourses_Amounts_NonNegative", "\"TotalPrice\" >= 0 AND \"DiscountAmount\" >= 0"));
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().ToTable(t => t.HasCheckConstraint("CK_PurcharseCourseDetails_Prices_NonNegative", "\"PriceOfCourse\" >= 0 AND \"PriceOfDiscount\" >= 0"));
            modelBuilder.Entity<BookmarkCourseEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BookmarkCourseEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ContentCourseEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ContentCourseDetailEntities>().HasOne<ContentCourseEntities>().WithMany().HasForeignKey(x => x.IdContent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CourseEntities>().HasOne<CategoryEntities>().WithMany().HasForeignKey(x => x.IdCategory).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CourseEvaluationEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CourseEvaluationEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DetailTheadQuestionCourseEntities>().HasOne<TheadQuestionCourseEntities>().WithMany().HasForeignKey(x => x.IdTheadQuestionCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DetailTheadQuestionCourseEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NotificationEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NotificationEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurcharseCourseEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().HasOne<PurcharseCourseEntities>().WithMany().HasForeignKey(x => x.IdPurchaseOrder).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentProcessEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentPromotionEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentPromotionEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentPromotionEntities>().HasOne<PromotionCodeEntities>().WithMany().HasForeignKey(x => x.IdPromotion).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubCategoryEntities>().HasOne<CategoryEntities>().WithMany().HasForeignKey(x => x.IdCategory).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TheadQuestionCourseEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdStudent).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TheadQuestionCourseEntities>().HasOne<CourseEntities>().WithMany().HasForeignKey(x => x.IdCourse).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Article>().ToTable("Articles").HasKey(x => x.Id);
            modelBuilder.Entity<Article>().HasIndex(x => x.Slug).IsUnique();

            // Performance indexes for frequently queried columns
            modelBuilder.Entity<StudentProcessEntities>().HasIndex(x => new { x.IdCourse, x.CreateBy }).HasDatabaseName("IX_StudentProcess_Course_Student");
            modelBuilder.Entity<CourseNoteEntities>().HasIndex(x => new { x.IdCourse, x.IdStudent }).HasDatabaseName("IX_CourseNotes_Course_Student");
            modelBuilder.Entity<RefundRequestEntities>().HasIndex(x => x.RefundStatus).HasDatabaseName("IX_RefundRequests_Status");
            modelBuilder.Entity<RefundRequestEntities>().HasIndex(x => x.IdStudent).HasDatabaseName("IX_RefundRequests_Student");
            modelBuilder.Entity<PayoutRequestEntities>().HasIndex(x => x.PayoutStatus).HasDatabaseName("IX_PayoutRequests_Status");
            modelBuilder.Entity<PayoutRequestEntities>().HasIndex(x => x.IdInstructor).HasDatabaseName("IX_PayoutRequests_Instructor");
            modelBuilder.Entity<SubtitleEntities>().HasIndex(x => x.IdContentDetail).HasDatabaseName("IX_Subtitles_ContentDetail");
            modelBuilder.Entity<PurcharseCourseDetailsEntities>().HasIndex(x => x.IdPurchaseOrder).HasDatabaseName("IX_PurchaseDetails_PurchaseOrder");

            // InstructorPaymentMethod
            modelBuilder.Entity<InstructorPaymentMethodEntities>().ToTable("InstructorPaymentMethods").HasKey(x => x.Id);
            modelBuilder.Entity<InstructorPaymentMethodEntities>().HasIndex(x => x.IdInstructor).IsUnique().HasDatabaseName("IX_InstructorPaymentMethods_Instructor");
            modelBuilder.Entity<InstructorPaymentMethodEntities>().HasOne<UserEntities>().WithMany().HasForeignKey(x => x.IdInstructor).OnDelete(DeleteBehavior.Restrict);

            // VideoPlaybackPosition
            modelBuilder.Entity<VideoPlaybackPositionEntities>().ToTable("VideoPlaybackPositions").HasKey(x => x.Id);
            modelBuilder.Entity<VideoPlaybackPositionEntities>().HasIndex(x => new { x.IdStudent, x.IdContentDetail }).IsUnique().HasDatabaseName("IX_VideoPlayback_Student_Content");

            // VideoEngagement
            modelBuilder.Entity<VideoEngagementEntities>().ToTable("VideoEngagements").HasKey(x => x.Id);
            modelBuilder.Entity<VideoEngagementEntities>().HasIndex(x => new { x.IdContentDetail, x.IdStudent }).HasDatabaseName("IX_VideoEngagement_Content_Student");

            // Announcements
            modelBuilder.Entity<AnnouncementEntities>().ToTable("Announcements").HasKey(x => x.Id);
            modelBuilder.Entity<AnnouncementEntities>().HasIndex(x => x.IdCourse).HasDatabaseName("IX_Announcements_Course");

            // QAVotes
            modelBuilder.Entity<QAVoteEntities>().ToTable("QAVotes").HasKey(x => x.Id);
            modelBuilder.Entity<QAVoteEntities>().HasIndex(x => new { x.IdQuestion, x.IdUser, x.IsForAnswer }).IsUnique().HasDatabaseName("IX_QAVotes_Question_User");

            // LessonCompletions
            modelBuilder.Entity<LessonCompletionEntities>().ToTable("LessonCompletions").HasKey(x => x.Id);
            modelBuilder.Entity<LessonCompletionEntities>().HasIndex(x => new { x.IdStudent, x.IdCourse }).HasDatabaseName("IX_LessonCompletion_Student_Course");
            modelBuilder.Entity<LessonCompletionEntities>().HasIndex(x => new { x.IdStudent, x.IdContentDetail }).IsUnique().HasDatabaseName("IX_LessonCompletion_Student_Content");

            // Gamification: Learning Streaks
            modelBuilder.Entity<LearningStreakEntities>().ToTable("LearningStreaks").HasKey(x => x.Id);
            modelBuilder.Entity<LearningStreakEntities>().HasIndex(x => x.IdStudent).IsUnique().HasDatabaseName("IX_LearningStreak_Student");

            // Gamification: Achievement Badges
            modelBuilder.Entity<AchievementBadgeEntities>().ToTable("AchievementBadges").HasKey(x => x.Id);

            // Gamification: User Badges
            modelBuilder.Entity<UserBadgeEntities>().ToTable("UserBadges").HasKey(x => x.Id);
            modelBuilder.Entity<UserBadgeEntities>().HasIndex(x => new { x.IdStudent, x.IdBadge }).IsUnique().HasDatabaseName("IX_UserBadge_Student_Badge");

            // Gamification: XP Transactions
            modelBuilder.Entity<XPTransactionEntities>().ToTable("XPTransactions").HasKey(x => x.Id);
            modelBuilder.Entity<XPTransactionEntities>().HasIndex(x => x.IdStudent).HasDatabaseName("IX_XPTransaction_Student");

            // Learning Paths
            modelBuilder.Entity<LearningPathEntities>().ToTable("LearningPaths").HasKey(x => x.Id);
            modelBuilder.Entity<LearningPathEntities>().HasIndex(x => x.IdCategory).HasDatabaseName("IX_LearningPath_Category");

            // Learning Path Courses
            modelBuilder.Entity<LearningPathCourseEntities>().ToTable("LearningPathCourses").HasKey(x => x.Id);
            modelBuilder.Entity<LearningPathCourseEntities>().HasIndex(x => new { x.IdLearningPath, x.IdCourse }).IsUnique().HasDatabaseName("IX_LearningPathCourse_Path_Course");

            // Course Prerequisites
            modelBuilder.Entity<CoursePrerequisiteEntities>().ToTable("CoursePrerequisites").HasKey(x => x.Id);
            modelBuilder.Entity<CoursePrerequisiteEntities>().HasIndex(x => new { x.IdCourse, x.IdPrerequisiteCourse }).IsUnique().HasDatabaseName("IX_Prerequisite_Course");

            // Auth tokens
            modelBuilder.Entity<RefreshTokenEntities>().ToTable("RefreshTokens").HasKey(x => x.Id);
            modelBuilder.Entity<RefreshTokenEntities>().HasIndex(x => x.UserId).HasDatabaseName("IX_RefreshToken_User");
            modelBuilder.Entity<PasswordResetTokenEntities>().ToTable("PasswordResetTokens").HasKey(x => x.Id);
            modelBuilder.Entity<PasswordResetTokenEntities>().HasIndex(x => x.UserId).HasDatabaseName("IX_PasswordResetToken_User");

            modelBuilder.SeedDataDefault();
        }

        public override int SaveChanges()
        {
            LocalDateTime dateNow = LocalDateTime.FromDateTime(DateTime.UtcNow);
            var errorList = new List<ValidationResult>();
            int idCurrentUser = _httpContextAccessor.HttpContext == null ? 0 : int.Parse(_httpContextAccessor.HttpContext.User.Claims
                                            .Where(x => x.Type == "user-id").FirstOrDefault()?.Value ?? "0");

            var entries = ChangeTracker.Entries()
         .Where(p => p.State == EntityState.Added ||
             p.State == EntityState.Modified).ToList();

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    if (entity is BaseEntities itemBase)
                    {
                        itemBase.CreateDate = itemBase.UpdateDate = dateNow;
                        itemBase.CreateBy = idCurrentUser;
                        itemBase.UpdateBy = idCurrentUser;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entity is BaseEntities itemBase)
                    {
                        itemBase.UpdateDate = dateNow;
                        itemBase.UpdateBy = idCurrentUser;
                    }
                }

                Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
            }

            if (errorList.Any())
            {
                throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            LocalDateTime dateNow = LocalDateTime.FromDateTime(DateTime.UtcNow);
            var errorList = new List<ValidationResult>();
            int idCurrentUser = _httpContextAccessor.HttpContext == null ? 0 : int.Parse(_httpContextAccessor.HttpContext.User.Claims
                                            .Where(x => x.Type == "user-id").FirstOrDefault()?.Value ?? "0");

            var entries = ChangeTracker.Entries()
         .Where(p => p.State == EntityState.Added ||
             p.State == EntityState.Modified).ToList();

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    if (entity is BaseEntities itemBase)
                    {
                        itemBase.CreateDate = itemBase.UpdateDate = dateNow;
                        itemBase.CreateBy = idCurrentUser;
                        itemBase.UpdateBy = idCurrentUser;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entity is BaseEntities itemBase)
                    {
                        itemBase.UpdateDate = dateNow;
                        itemBase.UpdateBy = idCurrentUser;
                    }
                }

                Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
            }

            if (errorList.Any())
            {
                throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());
            }

            return base.SaveChangesAsync();
        }
    }
}
