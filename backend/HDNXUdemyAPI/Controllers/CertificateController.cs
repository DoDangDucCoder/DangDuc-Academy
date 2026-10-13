using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Certificate)]
    public class CertificateController : BaseController
    {
        private readonly ICertificateService _certificateService;
        private readonly IAuditLogService _auditLogService;

        public CertificateController(ICertificateService certificateService, IAuditLogService auditLogService)
        {
            _certificateService = certificateService;
            _auditLogService = auditLogService;
        }

        [HttpPost("course/{idCourse}/student/{idStudent}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<RepositoryModel<CertificateModel>> Generate(long idCourse, long idStudent)
        {
            var studentId = IsAdmin() ? idStudent : CurrentUserId();
            return new RepositoryModel<CertificateModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _certificateService.GenerateCertificate(idCourse, studentId),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
        }

        [HttpGet("course/{idCourse}/student/{idStudent}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<RepositoryModel<CertificateModel?>> GetCertificate(long idCourse, long idStudent)
        {
            var studentId = IsAdmin() ? idStudent : CurrentUserId();
            return new RepositoryModel<CertificateModel?>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _certificateService.GetCertificate(idCourse, studentId),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpGet("{id}/download")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Download(long id)
        {
            var bytes = await _certificateService.DownloadCertificate(id);
            await _auditLogService.WriteAsync(CurrentUserId(), "Certificate.Download", "Certificate", id.ToString(), null, null);
            return File(bytes, "application/pdf", $"certificate-{id}.pdf");
        }
    }
}
