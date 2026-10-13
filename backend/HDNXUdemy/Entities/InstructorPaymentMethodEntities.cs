namespace HDNXUdemyData.Entities
{
    public class InstructorPaymentMethodEntities : BaseEntities
    {
        public long IdInstructor { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public string? PaypalEmail { get; set; }
        public bool IsDefault { get; set; } = true;

        public virtual UserEntities? Instructor { get; set; }
    }
}
