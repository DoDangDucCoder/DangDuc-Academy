using Amazon.S3;
using HDNXUdemyAPI.ModelHelp;
using HDNXUdemyData.GenericRepository;
using HDNXUdemyData.IRepository;
using HDNXUdemyData.Repository;
using HDNXUdemyServices.IServices;
using HDNXUdemyServices.Services;

namespace HDNXUdemyAPI.ProjectExtensisons
{
    /// <summary>
    /// ApplicationServicesExtensions
    /// </summary>
    public static class ApplicationServicesExtensions
    {
        /// <summary>
        /// AddApplicationServicesExtension
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddApplicationServicesExtension(this IServiceCollection services, IConfiguration configuration)
        {
            // ─── REPOSITORIES (Scoped — share ProjectContext within a request) ────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IBookmarkCourseRepository, BookmarkCourseRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRTheadQuestionCourseRepository, RTheadQuestionCourseRepository>();
            services.AddScoped<IContentCourseRepository, ContentCourseRepository>();
            services.AddScoped<IContentCourseDetailRepository, ContentCourseDetailRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IInformationManualBankingRepository, InformationManualBankingRepository>();
            services.AddScoped<IPurcharseCourseRepository, PurcharseCourseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentProcessRepository, StudentProcessRepository>();
            services.AddScoped<IStudentPromotionRepository, StudentPromotionRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IFileManagerRepository, FileManagerRepository>();
            services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
            services.AddScoped<IRPPartnerRepository, RPPartnerRepository>();
            services.AddScoped<IRPPurcharseCourseDetailsRepository, RPurcharseCourseDetailsRepository>();
            services.AddScoped<IRCourseEvaluationRepository, RCourseEvaluationRepository>();
            services.AddScoped<IRDetailTheadQuestionCourseRepository, RDetailTheadQuestionCourseRepository>();
            services.AddScoped<IRCouponRepository, RCouponRepository>();
            services.AddScoped<IRPromotionCodeRepository, RPromotionCodeRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IAssignmentSubmissionRepository, AssignmentSubmissionRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRRefundRequest, RefundRequestRepository>();
            services.AddScoped<IPayoutRequestRepository, PayoutRequestRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IInstructorPaymentMethodRepository, InstructorPaymentMethodRepository>();
            services.AddScoped<IVideoPlaybackPositionRepository, VideoPlaybackPositionRepository>();
            services.AddScoped<IVideoEngagementRepository, VideoEngagementRepository>();
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            services.AddScoped<IQAVoteRepository, QAVoteRepository>();
            services.AddScoped<ILessonCompletionRepository, LessonCompletionRepository>();
            services.AddScoped<ILearningStreakRepository, LearningStreakRepository>();
            services.AddScoped<IAchievementBadgeRepository, AchievementBadgeRepository>();
            services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
            services.AddScoped<IXPTransactionRepository, XPTransactionRepository>();
            services.AddScoped<ILearningPathRepository, LearningPathRepository>();
            services.AddScoped<ILearningPathCourseRepository, LearningPathCourseRepository>();
            services.AddScoped<ICoursePrerequisiteRepository, CoursePrerequisiteRepository>();

            // ─── SERVICES (Scoped — inject Scoped repositories) ──────────────────────
            services.AddScoped<IMasterDataServices, MasterDataServices>();
            services.AddScoped<ICourseServices, CourseServices>();
            services.AddScoped<IStudentServices, StudentServices>();
            services.AddScoped<IHomeServices, HomeServices>();
            services.AddScoped<IPurcharseCourseServices, PurcharseCourseServices>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IRefundService, RefundService>();
            services.AddScoped<IPayoutService, PayoutService>();
            services.AddScoped<ISubtitleService, SubtitleService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICourseNoteService, CourseNoteService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<ILiveClassService, LiveClassService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IInstructorDashboardService, InstructorDashboardService>();
            // ── Language Learning ────────────────────────────────────────────
            services.AddScoped<ILanguageExamService, LanguageExamService>();
            services.AddScoped<IVocabularyService, VocabularyService>();

            // ─── STATELESS UTILITIES (Transient — no DbContext dependency) ───────────
            services.AddTransient(typeof(ILogServices<>), typeof(LogServices<>));
            services.AddTransient<ISendEmailSMSServices, SendEmailSMSServices>();
            services.AddTransient<IUploadDataToCloud, UploadDataToCloud>();
            services.AddTransient<IUploadFileVideoToServer, UploadFileVideoToServer>();
            services.AddTransient<IEmailServices, EmailServices>();
            services.AddTransient<IStripeServices, StripeServices>();

            // ─── HTTP CLIENTS (lifetime managed by HttpClientFactory) ─────────────────
            services.AddHttpClient<IAuthenticationServices, AuthenticationServices>();
            services.AddHttpClient<ICourseSearchIndexService, CourseSearchIndexService>();
            services.AddHttpClient<IYoutubeImportService, YoutubeImportService>();

            // ─── INFRASTRUCTURE ───────────────────────────────────────────────────────
            services.AddMemoryCache();
            services.AddAWSService<IAmazonS3>();
            services.ApplicationDistributedConfigulation(configuration);
        }
    }
}