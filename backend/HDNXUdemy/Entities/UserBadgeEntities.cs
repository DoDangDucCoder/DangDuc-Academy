using NodaTime;

namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Badges earned by students
    /// </summary>
    public class UserBadgeEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdBadge { get; set; }
        public LocalDateTime EarnedDate { get; set; }
    }
}
