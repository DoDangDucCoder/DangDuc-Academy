using HDNXUdemyModel.Base;

namespace HDNXUdemyModel.Model
{
    public class PayoutRequestModel : BaseModel
    {
        public long IdInstructor { get; set; }

        public decimal Amount { get; set; }

        public int PayoutStatus { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime? ProcessDate { get; set; }

        public string? AdminNote { get; set; }

        public UserModel? Instructor { get; set; }
    }
}
