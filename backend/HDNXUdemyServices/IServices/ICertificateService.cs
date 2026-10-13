using HDNXUdemyModel.Model;

namespace HDNXUdemyServices.IServices
{
    public interface ICertificateService
    {
        Task<CertificateModel> GenerateCertificate(long idCourse, long idStudent);
        Task<CertificateModel?> GetCertificate(long idCourse, long idStudent);
        Task<byte[]> DownloadCertificate(long id);
    }
}
