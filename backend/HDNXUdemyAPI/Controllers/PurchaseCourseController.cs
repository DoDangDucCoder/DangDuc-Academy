using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// PurchaseCourseController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.PurchaseCourse)]
    [Authorize]
    public class PurchaseCourseController : BaseController
    {
        private readonly IPurcharseCourseServices _purcharseCourseServices;
        private readonly IStripeServices _stripeServices;

        /// <summary>
        /// PurchaseCourseController
        /// </summary>
        /// <param name="purcharseCourseServices"></param>
        /// <param name="stripeServices"></param>
        /// <exception cref="ProjectException"></exception>
        public PurchaseCourseController(IPurcharseCourseServices purcharseCourseServices, IStripeServices stripeServices)
        {
            _purcharseCourseServices = purcharseCourseServices ?? throw new ProjectException(nameof(_purcharseCourseServices));
            _stripeServices = stripeServices ?? throw new ProjectException(nameof(_stripeServices));
        }

        /// <summary>
        /// GenPurchaseOrder
        /// </summary>
        /// <param name="idStudent"></param>
        /// <returns></returns>
        [HttpGet("gen-purchase-code/{idStudent}")]
        public IActionResult GenPurchaseOrder(long idStudent)
        {
            RepositoryModel<string> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = string.Empty,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            if (!IsAdmin() && idStudent != CurrentUserId())
            {
                return Forbid();
            }

            result.Data = _purcharseCourseServices.GenPurchaseOrder(idStudent);
            return Ok(result);
        }

        /// <summary>
        /// CreateRequestPurchase
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create-purchase")]
        public async Task<IActionResult> CreateRequestPurchase(PurcharseCourseModel model)
        {
            RepositoryModel<PurcharseCourseModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PurcharseCourseModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            if (!IsAdmin())
            {
                var currentUserId = CurrentUserId();
                if (model.IdStudent != 0 && model.IdStudent != currentUserId)
                {
                    return Forbid();
                }

                model.IdStudent = currentUserId;
            }

            result.Data = await _purcharseCourseServices.CreateRequestPurchase(model);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// UpdateStatusPurchase
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("purchase-course/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<RepositoryModel<bool>> UpdateStatusPurchase(long id, PurcharseCourseModel model)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _purcharseCourseServices.UpdateStatusPurchase(id, model);
            return result;
        }

        /// <summary>
        /// IsCheckCoursePurchase
        /// </summary>
        /// <param name="idCourse"></param>
        /// <returns></returns>
        [HttpGet("check-purchase-course/{idCourse}")]
        public async Task<RepositoryModel<bool>> IsCheckCoursePurchase(long idCourse)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _purcharseCourseServices.IsCheckCoursePurchase(idCourse);
            return result;
        }

        /// <summary>
        /// GetListPurcharseCourses
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-list-purchase-course/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetListPurcharseCourses(int pageIndex, int pageSize)
        {
            RepositoryModel<PagedResult<PurcharseCourseModel>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PagedResult<PurcharseCourseModel>(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _purcharseCourseServices.GetListPurcharseCourses(pageIndex, pageSize, IsAdmin() ? null : CurrentUserId());
            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        /// <summary>
        /// GetPurchaseCorseDetail
        /// </summary>
        /// <param name="idPurchase"></param>
        /// <returns></returns>
        [HttpGet("get-detail-purchase-course/{idPurchase}")]
        public async Task<IActionResult> GetPurchaseCorseDetail(long idPurchase)
        {
            RepositoryModel<PurcharseCourseModel> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new PurcharseCourseModel(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _purcharseCourseServices.GetPurchaseCorseDetail(idPurchase);
            if (result.Data?.Id == null)
            {
                result.RetCode = ERetCode.NoExitData;
                result.StatusCode = (int)HttpStatusCode.NotFound;
                result.SystemMessage = "Purchase not found.";
                return NotFound(result);
            }

            if (!IsAdmin() && result.Data.IdStudent != CurrentUserId())
            {
                return Forbid();
            }

            result.StatusCode = (int)HttpStatusCode.OK;
            return Ok(result);
        }

        /// <summary>
        /// CreateCheckoutSession
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("create-stripe-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] PurcharseCourseModel? model)
        {
            RepositoryModel<CheckoutSessionResponse> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new CheckoutSessionResponse(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            if (model == null || model.ListPurchaseCourseDetails == null || !model.ListPurchaseCourseDetails.Any())
            {
                result.RetCode = ERetCode.SystemError;
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.SystemMessage = "At least one course is required for checkout.";
                return BadRequest(result);
            }

            if (model.TotalPrice < 0 || model.ListPurchaseCourseDetails.Any(x => x.IdCourse <= 0 || x.PriceOfCourse < 0 || x.PriceOfDiscount < 0))
            {
                result.RetCode = ERetCode.SystemError;
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                result.SystemMessage = "Checkout payload is invalid.";
                return BadRequest(result);
            }

            if (!IsAdmin())
            {
                var currentUserId = CurrentUserId();
                if (model.IdStudent != 0 && model.IdStudent != currentUserId)
                {
                    return Forbid();
                }

                model.IdStudent = currentUserId;
            }

            result.Data = await _stripeServices.CreateCheckoutSession(model);
            return Ok(result);
        }
    }
}
