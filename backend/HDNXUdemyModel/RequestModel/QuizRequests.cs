namespace HDNXUdemyModel.RequestModel
{
    public class SubmitQuizRequest
    {
        public long IdQuiz { get; set; }
        public List<SubmitQuizAnswerRequest> Answers { get; set; } = new();
    }

    public class SubmitQuizAnswerRequest
    {
        public long IdQuestion { get; set; }
        public long IdAnswerOption { get; set; }
    }
}
