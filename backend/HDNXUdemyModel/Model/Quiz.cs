using HDNXUdemyModel.Base;

namespace HDNXUdemyModel.Model
{
    public class QuizModel : BaseModel
    {
        public long IdCourse { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassingScore { get; set; } = 70;
        public List<QuizQuestionModel> Questions { get; set; } = new();
    }

    public class QuizQuestionModel : BaseModel
    {
        public long IdQuiz { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Point { get; set; } = 1;
        public List<QuizAnswerOptionModel> Answers { get; set; } = new();
    }

    public class QuizAnswerOptionModel : BaseModel
    {
        public long IdQuestion { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
