namespace HDNXUdemyModel.ResponModel
{
    public class InstructorDashboardModel
    {
        public long InstructorId { get; set; }
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CompletionRate { get; set; }
        public List<InstructorCourseMetricModel> Courses { get; set; } = new();
        public List<InstructorReviewModel> RecentReviews { get; set; } = new();
    }

    public class InstructorCourseMetricModel
    {
        public long CourseId { get; set; }
        public string? Title { get; set; }
        public int Students { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class InstructorReviewModel
    {
        public long CourseId { get; set; }
        public long StudentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
