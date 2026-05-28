using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace HDNXUdemyConvertVideoAPI.Controllers
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
        /// LoginNormalAccount
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginNormalAccount([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LoginNormalUserRequest? request)
        {
            RepositoryModel<ResponeLogin> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponeLogin(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            if (Request.Query.ContainsKey("email") || Request.Query.ContainsKey("password"))
            {
                result.PartnerCode = InterCode.ErrorMiddleware;
                result.RetCode = ERetCode.BadRequest;
                result.SystemMessage = "Query-string login is disabled. Use POST /api/v1/authentication/login with a JSON body.";
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest(result);
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                result.PartnerCode = InterCode.ErrorMiddleware;
                result.RetCode = ERetCode.BadRequest;
                result.SystemMessage = "Email and password are required.";
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest(result);
            }

            result.Data = await _authenticationServices.LoginNormalAccount(request.Email ?? string.Empty, request.Password ?? string.Empty, HttpContext);
            return Ok(result);
        }

        /// <summary>
        /// Legacy query-string login is disabled because it exposes passwords in URLs.
        /// </summary>
        [HttpPost("login-legacy")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LoginNormalAccountLegacy([FromQuery] string? email, [FromQuery] string? password)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.BadRequest,
                Data = false,
                SystemMessage = "Query-string login is disabled. Use POST /api/v1/authentication/login with a JSON body.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            return BadRequest(result);
        }

        /// <summary>
        /// Disabled legacy upload token minting endpoint.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("general-token/{email}")]
        [Authorize(Roles = "Admin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GeneralToken(string email)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = InterCode.ErrorMiddleware,
                RetCode = ERetCode.BadRequest,
                Data = false,
                SystemMessage = "General token minting is disabled. Use scoped upload credentials instead of user JWTs.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            return BadRequest(result);
        }
    }
}
