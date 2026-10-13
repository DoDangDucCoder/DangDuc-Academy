using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("PayoutRequests")]
    public class PayoutRequestEntities : BaseEntities
    {
        public long IdInstructor { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int PayoutStatus { get; set; } // 0: Pending, 1: Approved, 2: Rejected

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessDate { get; set; }

        public string? AdminNote { get; set; }

        // Navigation property
        [ForeignKey("IdInstructor")]
        public virtual UserEntities? Instructor { get; set; }
    }
}
