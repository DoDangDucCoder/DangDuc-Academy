using HDNXUdemyModel.Constant;
using HDNXUdemyServices.CommonFunction;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// BaseController
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    public class BaseController : Controller
    {
        protected long CurrentUserId()
        {
            return long.TryParse(User.FindFirstValue("user-id"), out long userId) ? userId : 0;
        }

        protected bool IsAdmin()
        {
            return User.IsInRole(ERoles.Admin.GetEnumDescription());
        }
    }
}
