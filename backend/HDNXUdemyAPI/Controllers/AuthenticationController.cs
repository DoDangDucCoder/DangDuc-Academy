using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// AuthenticationController
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Authentication)]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationServices _authenticationServices;

        /// <summary>
        /// AuthenticationController
        /// </summary>
        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices ?? throw new ProjectException(nameof(_authenticationServices));
        }

        /// <summary>
        /// LoginWithGoogle
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login-google")]
        public async Task<RepositoryModel<ResponeLogin>> LoginWithGoogle(LoginWithGoogle model)
        {
            RepositoryModel<ResponeLogin> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponeLogin(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _authenticationServices.LoginWithGoogle(model, HttpContext);
            return result;
        }

        /// <summary>
        /// LoginWithFaceBook
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        [HttpPost("login-facebook")]
        public async Task<RepositoryModel<ResponeLogin>> LoginWithFaceBook(LoginWithGoogle credential)
        {
            RepositoryModel<ResponeLogin> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponeLogin(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _authenticationServices.LoginWithFacebook(credential, HttpContext);
            return result;
        }

        /// <summary>
        /// RegisterUser
        /// </summary>
        /// <param name="email"></param>
        /// /// <param name="name"></param>
        /// /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Obsolete("Use POST /api/v1/authentication/register with a JSON body. This route is retained only for legacy clients.")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("register/{name}/{phone}/{email}/{password}")]
        [AllowAnonymous]
        public IActionResult RegisterUser(string name, string phone, string email, string password)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.BadRequest,
                Data = false,
                SystemMessage = "Legacy URL registration is disabled. Use POST /api/v1/authentication/register with a JSON body.",
                StatusCode = (int)HttpStatusCode.Gone
            };

            return StatusCode(StatusCodes.Status410Gone, result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterNormalUserRequest request)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new bool(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            result.Data = await _authenticationServices.RegisterNormalUser(request.Email ?? string.Empty, request.Password ?? string.Empty, request.Name ?? string.Empty, request.Phone ?? string.Empty);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// LoginNormalAccount
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginNormalAccount(LoginNormalUserRequest request)
        {
            RepositoryModel<ResponeLogin> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponeLogin(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _authenticationServices.LoginNormalAccount(request.Email ?? string.Empty, request.Password ?? string.Empty, HttpContext);
            return Ok(result);
        }

        [HttpGet("login/{email}/{password}")]
        [AllowAnonymous]
        [Obsolete("Use POST /api/v1/authentication/login with a JSON body. This legacy route keeps older clients working but must not be used by UI clients.")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LoginNormalAccount(string email, string password)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.BadRequest,
                Data = false,
                SystemMessage = "Legacy URL login is disabled. Use POST /api/v1/authentication/login with a JSON body.",
                StatusCode = (int)HttpStatusCode.Gone
            };

            return StatusCode(StatusCodes.Status410Gone, result);
        }

        /// <summary>
        /// VerifyLinkRegister
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("verify-account/{id}/{email}")]
        public async Task<IActionResult> VerifyLinkRegister(long id, string email)
        {
            bool isUpdate = await _authenticationServices.IsActiveAccountAfterRegister(email, id);
            if (isUpdate)
            {
                return Redirect(ProjectConfig.LinkCompletedRegister ?? string.Empty);
            }
            else
            {
                return Redirect(ProjectConfig.LinkRegisterError ?? string.Empty);
            }
        }
    }
}
