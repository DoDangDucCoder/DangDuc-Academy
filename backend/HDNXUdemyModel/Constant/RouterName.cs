namespace HDNXUdemyModel.Constant
{
    public static class VersionApi
    {
        public const string Version1 = "1.0";
    }

    public static class RouterControllerName
    {
        public const string MasterData = "api/v{version:apiVersion}/master-data";
        public const string Authentication = "api/v{version:apiVersion}/authentication";
        public const string Course = "api/v{version:apiVersion}/master-course";
        public const string PurchaseCourse = "api/v{version:apiVersion}/purchase-course";
        public const string Student = "api/v{version:apiVersion}/student";
        public const string UploadToCloud = "api/v{version:apiVersion}/upload-to-cloud";
        public const string UploadVideoToClound = "api/v{version:apiVersion}/upload-video-to-server";
        public const string GetVideoStreamFile = "api/v{version:apiVersion}/get-video-stream";
        public const string Home = "api/v{version:apiVersion}/home";
        public const string WebHook = "api/v{version:apiVersion}/webhook";
        public const string Promotion = "api/v{version:apiVersion}/promotion";
        public const string InstructorDashboard = "api/v{version:apiVersion}/instructor-dashboard";
        public const string Certificate = "api/v{version:apiVersion}/certificate";
        public const string Quiz = "api/v{version:apiVersion}/quiz";
        public const string LiveClass = "api/v{version:apiVersion}/live-class";
        public const string AuditLog = "api/v{version:apiVersion}/audit-log";
        public const string YoutubeImport = "api/v{version:apiVersion}/youtube-import";
        public const string Article = "api/v{version:apiVersion}/article";
    }
}
