using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class RefundRequestEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdPurchaseCourse { get; set; }
        public string? Reason { get; set; }
        public int RefundStatus { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public string? AdminNote { get; set; }
        public LocalDateTime? RequestDate { get; set; }
        public LocalDateTime? ProcessDate { get; set; }
    }
}
