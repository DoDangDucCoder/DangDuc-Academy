namespace HDNXUdemyData.Entities
{
    public class QuizEntities : BaseEntities
    {
        public long IdCourse { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassingScore { get; set; }
    }
}
