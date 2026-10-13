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
    [Route(RouterControllerName.LiveClass)]
    public class LiveClassController : BaseController
    {
        private readonly ILiveClassService _liveClassService;

        public LiveClassController(ILiveClassService liveClassService)
        {
            _liveClassService = liveClassService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<RepositoryModel<LiveClassModel>> Create(LiveClassModel model)
        {
            if (!IsAdmin()) model.IdInstructor = CurrentUserId();
            return new RepositoryModel<LiveClassModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _liveClassService.CreateLiveClass(model),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<RepositoryModel<List<LiveClassModel>>> Get([FromQuery] long? idCourse, [FromQuery] long? idInstructor)
        {
            return new RepositoryModel<List<LiveClassModel>>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _liveClassService.GetLiveClasses(idCourse, idInstructor),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<RepositoryModel<bool>> Update(long id, LiveClassModel model)
        {
            if (!IsAdmin()) model.IdInstructor = CurrentUserId();
            return new RepositoryModel<bool>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _liveClassService.UpdateLiveClass(id, model),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
