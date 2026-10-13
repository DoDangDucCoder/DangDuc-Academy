using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.Model;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// PromotionOfCourseController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Promotion)]
    [Authorize(Roles = "Admin")]
    public class PromotionOfCourseController : BaseController
    {
        private readonly IStripeServices _stripeServices;
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// PromotionOfCourseController
        /// </summary>
        /// <param name="stripeServices"></param>
        /// <exception cref="ProjectException"></exception>
        public PromotionOfCourseController(IStripeServices stripeServices, IAuditLogService auditLogService)
        {
            _stripeServices = stripeServices ?? throw new ProjectException(nameof(_stripeServices));
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// CreatePromotionCoupon
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("coupon-promotion")]
        public async Task<IActionResult> CreatePromotionCoupon(CouponPromotionCode model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _stripeServices.CreateCouponForPromotion(model);
            await _auditLogService.WriteAsync(CurrentUserId(), "Coupon.Create", "Coupon", model.Id?.ToString(), null, new { model.NameOfCoupon, model.PercentOff, model.Status });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// DeleteCouponForPromotion
        /// </summary>
        /// <param name="stripeCouponId"></param>
        /// <returns></returns>
        [HttpPut("coupon-inactive/{stripeCouponId}")]
        public async Task<IActionResult> DeleteCouponForPromotion(string stripeCouponId)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _stripeServices.DeleteCouponForPromotion(stripeCouponId);
            await _auditLogService.WriteAsync(CurrentUserId(), "Coupon.Inactive", "Coupon", stripeCouponId, null, null);
            return Ok(result);
        }

        /// <summary>
        /// CreateStripePromotionCode
        /// </summary>
        /// <param name="idCoupon"></param>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        [HttpPost("promotion-code/{idCoupon}/{promotionCode}")]
        public async Task<IActionResult> CreateStripePromotionCode(string idCoupon, string promotionCode)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _stripeServices.CreateStripePromotionCode(idCoupon, promotionCode);
            await _auditLogService.WriteAsync(CurrentUserId(), "PromotionCode.Create", "PromotionCode", idCoupon, null, new { promotionCode });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// InactivePromotionCode
        /// </summary>
        /// <param name="promotionCodeId"></param>
        /// <returns></returns>
        [HttpPost("inactive-promotion-code/{promotionCodeId}")]
        public async Task<IActionResult> InactivePromotionCode(string promotionCodeId)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _stripeServices.InactivePromotionCode(promotionCodeId);
            await _auditLogService.WriteAsync(CurrentUserId(), "PromotionCode.Inactive", "PromotionCode", promotionCodeId, null, null);
            return Ok(result);
        }

        /// <summary>
        /// GetListCouponActiveOnSystem
        /// </summary>
        /// <returns></returns>
        [HttpGet("coupon-promotion-code/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetListCouponActiveOnSystem(int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<CouponModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<CouponModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _stripeServices.GetListCouponActiveOnSystem(pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// UpdatePromotionCoupon
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("coupon-promotion")]
        public async Task<IActionResult> UpdatePromotionCoupon(CouponPromotionCode model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _stripeServices.UpdateCouponPromotionCode(model);
            await _auditLogService.WriteAsync(CurrentUserId(), "Coupon.Update", "Coupon", model.Id?.ToString(), null, new { model.NameOfCoupon, model.PercentOff, model.Status });
            return Ok(result);
        }

        /// <summary>
        /// GetListPromotions
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("promotion-codes/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetListPromotions(int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<PromotionCodeModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<PromotionCodeModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _stripeServices.GetListPromotions(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
