using System;
using HDNXUdemyModel.Base;

namespace HDNXUdemyModel.Model
{
    public class AssignmentSubmissionModel : BaseModel
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
