namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Learning Path — a guided sequence of courses
    /// </summary>
    public class LearningPathEntities : BaseEntities
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long IdCategory { get; set; }
        public string? PictureUrl { get; set; }
        public string LevelRequired { get; set; } = "Beginner";
        public int EstimatedHours { get; set; }
    }
}
