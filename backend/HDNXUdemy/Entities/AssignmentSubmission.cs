using System;

namespace HDNXUdemyData.Entities
{
    public class AssignmentSubmissionEntities : BaseEntities
    {
        public long IdAssignment { get; set; }
        public long IdUser { get; set; }
        public string? SubmissionText { get; set; }
        public string? FileUrl { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public int? Score { get; set; }
        public string? Feedback { get; set; }
    }
}
