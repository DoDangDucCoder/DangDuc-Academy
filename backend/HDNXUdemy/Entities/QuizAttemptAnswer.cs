namespace HDNXUdemyData.Entities
{
    public class QuizAttemptAnswerEntities : BaseEntities
    {
        public long IdAttempt { get; set; }
        public long IdQuestion { get; set; }
        public long IdAnswerOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}
