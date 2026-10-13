using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/InstructorPaymentMethod")]
    [Authorize]
    public class InstructorPaymentMethodController : BaseController
    {
        private readonly IInstructorPaymentMethodRepository _repository;

        public InstructorPaymentMethodController(IInstructorPaymentMethodRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPaymentMethod()
        {
            var userId = CurrentUserId();
            var method = await _repository.GetObjectAsync(x => x.IdInstructor == userId);
            if (method == null)
            {
                return Ok(new RepositoryModel<InstructorPaymentMethodModel>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.OK,
                    RetCode = ERetCode.Successfull
                });
            }

            return Ok(new RepositoryModel<InstructorPaymentMethodModel>
            {
                Data = new InstructorPaymentMethodModel
                {
                    Id = method.Id,
                    IdInstructor = method.IdInstructor,
                    BankName = method.BankName,
                    AccountNumber = method.AccountNumber,
                    AccountHolderName = method.AccountHolderName,
                    PaypalEmail = method.PaypalEmail,
                    IsDefault = method.IsDefault
                },
                StatusCode = (int)HttpStatusCode.OK,
                RetCode = ERetCode.Successfull
            });
        }

        [HttpPost]
        public async Task<IActionResult> SavePaymentMethod([FromBody] InstructorPaymentMethodModel model)
        {
            var userId = CurrentUserId();
            var existing = await _repository.GetObjectAsync(x => x.IdInstructor == userId);

            if (existing != null && existing.Id > 0)
            {
                // Update existing
                existing.BankName = model.BankName;
                existing.AccountNumber = model.AccountNumber;
                existing.AccountHolderName = model.AccountHolderName;
                existing.PaypalEmail = model.PaypalEmail;
                existing.IsDefault = model.IsDefault;
                await _repository.UpdateAsync(existing);
            }
            else
            {
                // Create new
                var entity = new InstructorPaymentMethodEntities
                {
                    IdInstructor = userId,
                    BankName = model.BankName,
                    AccountNumber = model.AccountNumber,
                    AccountHolderName = model.AccountHolderName,
                    PaypalEmail = model.PaypalEmail,
                    IsDefault = true
                };
                await _repository.AddAsync(entity);
            }

            return Ok(new RepositoryModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                RetCode = ERetCode.Successfull,
                PartnerCode = Messenger.SuccessFull
            });
        }

        [HttpGet("instructor/{idInstructor}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInstructorPaymentMethod(long idInstructor)
        {
            var method = await _repository.GetObjectAsync(x => x.IdInstructor == idInstructor);
            if (method == null)
            {
                return Ok(new RepositoryModel<InstructorPaymentMethodModel>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.OK,
                    SystemMessage = "No payment method configured"
                });
            }

            return Ok(new RepositoryModel<InstructorPaymentMethodModel>
            {
                Data = new InstructorPaymentMethodModel
                {
                    Id = method.Id,
                    IdInstructor = method.IdInstructor,
                    BankName = method.BankName,
                    AccountNumber = method.AccountNumber,
                    AccountHolderName = method.AccountHolderName,
                    PaypalEmail = method.PaypalEmail,
                    IsDefault = method.IsDefault
                },
                StatusCode = (int)HttpStatusCode.OK,
                RetCode = ERetCode.Successfull
            });
        }
    }
}
