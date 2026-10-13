using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class QuizAttemptEntities : BaseEntities
    {
        public long IdQuiz { get; set; }
        public long IdStudent { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public LocalDateTime SubmittedAt { get; set; }
    }
}
