namespace HDNXUdemyData.Entities
{
    public class QuizQuestionEntities : BaseEntities
    {
        public long IdQuiz { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Point { get; set; }
    }
}
