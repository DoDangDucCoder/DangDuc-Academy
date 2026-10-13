namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Achievement badge definitions
    /// </summary>
    public class AchievementBadgeEntities : BaseEntities
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconEmoji { get; set; } = "🏆";
        public int CriteriaType { get; set; } // 1=CoursesCompleted, 2=StreakDays, 3=XPPoints, 4=ReviewsGiven, 5=FirstPurchase
        public int CriteriaValue { get; set; } // e.g. 5 courses, 30 days streak, 1000 XP
    }
}
