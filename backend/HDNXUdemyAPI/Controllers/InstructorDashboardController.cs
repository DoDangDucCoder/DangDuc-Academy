using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.InstructorDashboard)]
    [Authorize(Roles = "Admin,Teacher")]
    public class InstructorDashboardController : BaseController
    {
        private readonly IInstructorDashboardService _dashboardService;

        public InstructorDashboardController(IInstructorDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("{instructorId}")]
        public async Task<RepositoryModel<InstructorDashboardModel>> GetDashboard(long instructorId)
        {
            var id = IsAdmin() && instructorId > 0 ? instructorId : CurrentUserId();
            return new RepositoryModel<InstructorDashboardModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _dashboardService.GetDashboard(id),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
