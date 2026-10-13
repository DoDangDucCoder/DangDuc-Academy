using Asp.Versioning;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class RefundController : BaseController
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestInput input)
        {
            var idStudent = CurrentUserId();
            var res = await _refundService.RequestRefund(idStudent, input.IdPurchaseCourse, input.Reason);
            return Ok(res);
        }

        [HttpPost("process")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessRefund([FromBody] ProcessRefundInput input)
        {
            var res = await _refundService.ProcessRefund(input.IdRefundRequest, input.IsApproved, input.AdminNote);
            return Ok(res);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRefunds()
        {
            var res = await _refundService.GetPendingRefunds();
            return Ok(res);
        }

        [HttpGet("student/{idStudent}")]
        public async Task<IActionResult> GetRefundsByStudent(long idStudent)
        {
            var res = await _refundService.GetRefundsByStudent(idStudent);
            return Ok(res);
        }
    }

    public class RefundRequestInput
    {
        public long IdPurchaseCourse { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ProcessRefundInput
    {
        public long IdRefundRequest { get; set; }
        public bool IsApproved { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}
