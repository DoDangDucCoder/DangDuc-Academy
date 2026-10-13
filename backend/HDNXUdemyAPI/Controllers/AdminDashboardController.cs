using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/admin-dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : BaseController
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<RepositoryModel<AdminDashboardModel>> GetDashboard()
        {
            return new RepositoryModel<AdminDashboardModel>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _dashboardService.GetDashboardAsync(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
