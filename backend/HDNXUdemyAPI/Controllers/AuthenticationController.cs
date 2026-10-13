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
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// AuthenticationController
        /// </summary>
        public AuthenticationController(IAuthenticationServices authenticationServices, IAuditLogService auditLogService)
        {
            _authenticationServices = authenticationServices ?? throw new ProjectException(nameof(_authenticationServices));
            _auditLogService = auditLogService;
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
            await _auditLogService.WriteAsync(result.Data.UserId, "Auth.LoginGoogle", "User", result.Data.UserId?.ToString(), null, new { result.Data.Email });
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
            await _auditLogService.WriteAsync(result.Data.UserId, "Auth.LoginFacebook", "User", result.Data.UserId?.ToString(), null, new { result.Data.Email });
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
            await _auditLogService.WriteAsync(null, "Auth.Register", "User", null, null, new { request.Email, request.Name, request.Phone });
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

            try
            {
                result.Data = await _authenticationServices.LoginNormalAccount(request.Email ?? string.Empty, request.Password ?? string.Empty, HttpContext);
                await _auditLogService.WriteAsync(result.Data.UserId, "Auth.Login", "User", result.Data.UserId?.ToString(), null, new { result.Data.Email });
            }
            catch
            {
                await _auditLogService.WriteAsync(null, "Auth.LoginFailed", "User", null, null, new { request.Email });
                throw;
            }
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

        /// <summary>
        /// RefreshToken - Exchange a valid refresh token for a new access + refresh token pair
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            RepositoryModel<ResponeLogin> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = new ResponeLogin(),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _authenticationServices.RefreshAccessToken(request.RefreshToken ?? string.Empty, HttpContext);
            await _auditLogService.WriteAsync(result.Data.UserId, "Auth.RefreshToken", "User", result.Data.UserId?.ToString(), null, new { result.Data.Email });
            return Ok(result);
        }

        /// <summary>
        /// RevokeToken - Invalidate a refresh token (secure logout)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest request)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _authenticationServices.RevokeRefreshToken(request.RefreshToken ?? string.Empty);
            await _auditLogService.WriteAsync(CurrentUserId(), "Auth.RevokeToken", "User", CurrentUserId().ToString(), null, null);
            return Ok(result);
        }

        /// <summary>
        /// ForgotPassword - Send OTP code to email for password reset
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _authenticationServices.ForgotPassword(request.Email);
            await _auditLogService.WriteAsync(null, "Auth.ForgotPassword", "User", null, null, new { request.Email });
            return Ok(result);
        }

        /// <summary>
        /// ResetPassword - Verify OTP code and set new password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = false,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            result.Data = await _authenticationServices.ResetPassword(request.Email, request.Token, request.NewPassword);
            await _auditLogService.WriteAsync(null, "Auth.ResetPassword", "User", null, null, new { request.Email });
            return Ok(result);
        }
    }
}
