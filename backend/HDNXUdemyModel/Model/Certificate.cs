using HDNXUdemyModel.Base;
using NodaTime;

namespace HDNXUdemyModel.Model
{
    public class CertificateModel : BaseModel
    {
        public long IdStudent { get; set; }
        public long IdCourse { get; set; }
        public string CertificateCode { get; set; } = string.Empty;
        public LocalDateTime IssuedDate { get; set; }
        public string? FileUrl { get; set; }
    }
}
