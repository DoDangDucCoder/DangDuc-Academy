using NodaTime;

namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Tracks daily learning streaks for students
    /// </summary>
    public class LearningStreakEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public LocalDateTime LastActivityDate { get; set; }
        public int TotalDaysLearned { get; set; }
    }
}
