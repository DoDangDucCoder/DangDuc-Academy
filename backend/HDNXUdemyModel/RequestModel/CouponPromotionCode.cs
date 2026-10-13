using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace HDNXUdemyModel.RequestModel
{
    public class CouponPromotionCode
    {
        public long? Id { get; set; }

        [Required]
        [Range(0.01, 100)]
        public decimal PercentOff { get; set; }

        [Required]
        [StringLength(10)]
        public string? Currency { get; set; } = "USD";

        [Required]
        [StringLength(50)]
        public string? TypeDuration { get; set; } = "forever";

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [StringLength(100)]
        public string? NameOfCoupon { get; set; }
    }
}