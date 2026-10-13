namespace HDNXUdemyData.Entities
{
    public class QuizAnswerOptionEntities : BaseEntities
    {
        public long IdQuestion { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
