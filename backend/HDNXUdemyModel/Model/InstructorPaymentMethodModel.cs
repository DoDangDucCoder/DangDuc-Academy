namespace HDNXUdemyModel.Model
{
    public class InstructorPaymentMethodModel
    {
        public long Id { get; set; }
        public long IdInstructor { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public string? PaypalEmail { get; set; }
        public bool IsDefault { get; set; } = true;
    }
}
