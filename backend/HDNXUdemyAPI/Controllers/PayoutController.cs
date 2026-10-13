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
    [Route("api/v{version:apiVersion}/Payout")]
    [Authorize]
    public class PayoutController : BaseController
    {
        private readonly IPayoutService _payoutService;

        public PayoutController(IPayoutService payoutService)
        {
            _payoutService = payoutService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = CurrentUserId();
            var balance = await _payoutService.GetInstructorBalanceAsync(userId);
            return Ok(new RepositoryModel<decimal>
            {
                Data = balance,
                StatusCode = (int)HttpStatusCode.OK,
                RetCode = ERetCode.Successfull,
                PartnerCode = Messenger.SuccessFull
            });
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPayout([FromBody] decimal amount)
        {
            var userId = CurrentUserId();
            var success = await _payoutService.RequestPayoutAsync(userId, amount);
            if (success)
            {
                return Ok(new RepositoryModel<bool>
                {
                    Data = true,
                    StatusCode = (int)HttpStatusCode.Created,
                    RetCode = ERetCode.Successfull,
                    PartnerCode = Messenger.SuccessFull
                });
            }
            return BadRequest(new RepositoryModel<bool>
            {
                Data = false,
                StatusCode = (int)HttpStatusCode.BadRequest,
                RetCode = ERetCode.SystemError,
                SystemMessage = "Invalid amount or insufficient balance"
            });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var pending = await _payoutService.GetPendingPayoutsAsync();
            return Ok(new RepositoryModel<List<PayoutRequestModel>>
            {
                Data = pending,
                StatusCode = (int)HttpStatusCode.OK,
                RetCode = ERetCode.Successfull,
                PartnerCode = Messenger.SuccessFull
            });
        }

        [HttpPost("process")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessPayout([FromBody] ProcessPayoutDto dto)
        {
            var success = await _payoutService.ProcessPayoutAsync(dto.IdPayoutRequest, dto.IsApproved, dto.AdminNote);
            if (success)
            {
                return Ok(new RepositoryModel<bool>
                {
                    Data = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    RetCode = ERetCode.Successfull,
                    PartnerCode = Messenger.SuccessFull
                });
            }
            return BadRequest(new RepositoryModel<bool>
            {
                Data = false,
                StatusCode = (int)HttpStatusCode.BadRequest,
                RetCode = ERetCode.SystemError,
                SystemMessage = "Failed to process payout request"
            });
        }
    }

    public class ProcessPayoutDto
    {
        public long IdPayoutRequest { get; set; }
        public bool IsApproved { get; set; }
        public string AdminNote { get; set; } = string.Empty;
    }
}
