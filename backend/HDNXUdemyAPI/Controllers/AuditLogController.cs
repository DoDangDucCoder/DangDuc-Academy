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
    [Route(RouterControllerName.AuditLog)]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : BaseController
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet("{pageIndex}/{pageSize}")]
        public async Task<RepositoryModel<PagedResult<AuditLogModel>>> GetLogs(
            int pageIndex,
            int pageSize,
            [FromQuery] string? action,
            [FromQuery] string? entityName,
            [FromQuery] long? userId,
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate)
        {
            return new RepositoryModel<PagedResult<AuditLogModel>>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _auditLogService.GetLogs(pageIndex, pageSize, action, entityName, userId, fromDate, toDate),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
