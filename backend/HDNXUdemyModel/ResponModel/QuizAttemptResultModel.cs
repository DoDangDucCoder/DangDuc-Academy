namespace HDNXUdemyModel.ResponModel
{
    public class QuizAttemptResultModel
    {
        public long AttemptId { get; set; }
        public int Score { get; set; }
        public int TotalScore { get; set; }
        public int PassingScore { get; set; }
        public bool IsPassed { get; set; }
    }
}
