using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using Microsoft.AspNetCore.Http;

namespace HDNXUdemyServices.IServices
{
    public interface IAuthenticationServices
    {
        Task<ResponeLogin> LoginWithGoogle(LoginWithGoogle model, HttpContext httpContext);

        Task<ResponeLogin> LoginWithFacebook(LoginWithGoogle credential, HttpContext httpContext);

        Task<bool> RegisterNormalUser(string email, string password, string name, string phone);

        Task<ResponeLogin> LoginNormalAccount(string email, string password, HttpContext httpContext);

        Task<ResponeLogin> GeneralToken(string email, HttpContext httpContext);

        Task<bool> IsActiveAccountAfterRegister(string email, long id);

        Task<ResponeLogin> RefreshAccessToken(string refreshToken, HttpContext httpContext);

        Task<bool> RevokeRefreshToken(string refreshToken);

        Task<bool> ForgotPassword(string email);

        Task<bool> ResetPassword(string email, string token, string newPassword);
    }
}