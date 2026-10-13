using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class CertificateEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdCourse { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public LocalDateTime IssuedDate { get; set; }
        public string? FileUrl { get; set; }
    }
}
