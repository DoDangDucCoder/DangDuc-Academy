using AutoMapper;
using Google.Apis.Auth;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Exception;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.CommonFunction;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HDNXUdemyServices.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IUserRepository _userRepository;
        private readonly ISystemConfigRepository _systemConfigRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly HttpClient _httpClient;
        private readonly IEmailServices _emailServices;
        private readonly ISendEmailSMSServices _sendEmailServices;
        private readonly IMapper _mapper;

        public AuthenticationServices(IUserRepository userRepository, HttpClient httpClient, IMapper mapper, ISystemConfigRepository systemConfigRepository,
            IEmailServices emailServices, IRefreshTokenRepository refreshTokenRepository, IPasswordResetTokenRepository passwordResetTokenRepository,
            ISendEmailSMSServices sendEmailServices)
        {
            _userRepository = userRepository ?? throw new ProjectException(nameof(_userRepository));
            _httpClient = httpClient ?? throw new ProjectException(nameof(_httpClient));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _systemConfigRepository = systemConfigRepository ?? throw new ProjectException(nameof(_systemConfigRepository));
            _emailServices = emailServices ?? throw new ProjectException(nameof(_emailServices));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ProjectException(nameof(_refreshTokenRepository));
            _passwordResetTokenRepository = passwordResetTokenRepository ?? throw new ProjectException(nameof(_passwordResetTokenRepository));
            _sendEmailServices = sendEmailServices ?? throw new ProjectException(nameof(_sendEmailServices));
        }

        public async Task<ResponeLogin> LoginWithGoogle(LoginWithGoogle model, HttpContext httpContext)
        {
            var returnData = new ResponeLogin();
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { ProjectConfig.ClientId ?? string.Empty }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
                if (payload != null)
                {
                    var user = await _userRepository.GetObjectAsync(x => x.Email == payload.Email);
                    var userConvert = _mapper.Map<UserModel>(user);
                    returnData = await ConvertDataForLogin(user, payload.Email, payload.Picture, payload.Name, ETypeLogin.Google, httpContext);
                }
                else
                {
                    throw new ProjectBadRequestException(InternalCodeMessenger.NotFoundValue);
                }
            }
            catch (Exception ex)
            {
                throw new ProjectBadRequestException(ex.Message);
            }

            return returnData;
        }

        public async Task<ResponeLogin> LoginWithFacebook(LoginWithGoogle credential, HttpContext httpContext)
        {
            var returnData = new ResponeLogin();
            try
            {
                HttpResponseMessage meResponse = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token={credential.AuthToken}");
                var userContent = await meResponse.Content.ReadAsStringAsync();
                var userContentObj = JsonConvert.DeserializeObject<FacebookUserInfo>(userContent);

                if (userContentObj != null)
                {
                    var user = await _userRepository.GetObjectAsync(x => x.Email == userContentObj.Email);
                    returnData = await ConvertDataForLogin(user, userContentObj.Email ?? string.Empty,
                        credential.PhotoUrl ?? string.Empty, userContentObj.Name ?? string.Empty, ETypeLogin.Facebook, httpContext);
                }
                else
                {
                    throw new ProjectBadRequestException(InternalCodeMessenger.NotFoundValue);
                }
            }
            catch (Exception ex)
            {
                throw new ProjectBadRequestException(ex.Message);
            }

            return new ResponeLogin();
        }

        public async Task<bool> RegisterNormalUser(string email, string password, string name, string phone)
        {
            ValidateRegistration(email, password, name, phone);

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var existingUser = await _userRepository.GetObjectAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail);
            if (existingUser != null)
            {
                throw new ProjectConflictException("Email already exists.");
            }

            var dataInsert = new UserEntities()
            {
                Email = normalizedEmail,
                Password = PasswordHasher.Hash(password),
                Name = name.Trim(),
                Phone = phone.Trim(),
                RoleId = (int)ERoles.Student,
                TypeLogin = (int)ETypeLogin.Email,
            };
            var dataInsertReturn = await _userRepository.AddReturnModelAsync(dataInsert);
            if (dataInsertReturn != null)
            {
                string linkRequestUrl = $"{ProjectConfig.LinkRequestUrl}/{dataInsertReturn.Id}/{dataInsertReturn.Email}";
                // await _emailServices.SendEmailToSingUpEmail(email, linkRequestUrl);
            }
            return dataInsertReturn != null;
        }

        public async Task<ResponeLogin> LoginNormalAccount(string email, string password, HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ProjectBadRequestException("Email and password are required.");
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var getData = await _userRepository.GetObjectAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail);
            var returnData = new ResponeLogin();
            if (getData != null && PasswordHasher.Verify(password, getData.Password))
            {
                if (!PasswordHasher.IsHashed(getData.Password))
                {
                    getData.Password = PasswordHasher.Hash(password);
                    await _userRepository.UpdateAsync(getData);
                }

                returnData = await ConvertDataForLogin(getData, getData.Email ?? string.Empty,
                    getData.PictureUrl ?? string.Empty, getData.Name ?? string.Empty, ETypeLogin.Email, httpContext);
            }
            else
            {
                throw new ProjectBadRequestException(InternalCodeMessenger.NotFoundValue);
            }
            return returnData;
        }

        private static void ValidateRegistration(string email, string password, string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ProjectBadRequestException("Name, phone, email, and password are required.");
            }

            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                throw new ProjectBadRequestException("Invalid email format.");
            }

            if (password.Length < 8 ||
                !Regex.IsMatch(password, "[A-Z]") ||
                !Regex.IsMatch(password, "[a-z]") ||
                !Regex.IsMatch(password, "[0-9]"))
            {
                throw new ProjectBadRequestException("Password must be at least 8 characters and include uppercase, lowercase, and number.");
            }
        }

        public async Task<ResponeLogin> GeneralToken(string email, HttpContext httpContext)
        {
            var getData = await _userRepository.GetObjectAsync(x => x.Email == email);
            var returnData = new ResponeLogin();
            if (getData != null)
            {
                returnData = await ConvertDataForLogin(getData, getData.Email ?? string.Empty, getData.PictureUrl ?? string.Empty,
                    getData.Name ?? string.Empty, (ETypeLogin)getData.TypeLogin, httpContext);
                var getDataConfig = await _systemConfigRepository.GetObjectAsync(x => x.KeyConfig == KeyConfig.KeyTokenUpload);
                if (getDataConfig != null)
                {
                    getDataConfig.Value = returnData.Token;
                    await _systemConfigRepository.UpdateAsync(getDataConfig);
                }
                else
                {
                    var dataInsert = new SystemConfigEntities()
                    {
                        KeyConfig = KeyConfig.KeyTokenUpload,
                        Value = returnData.Token
                    };
                    await _systemConfigRepository.AddAsync(dataInsert);
                }
            }
            else
            {
                throw new ProjectBadRequestException(InternalCodeMessenger.NotFoundValue);
            }
            return returnData;
        }

        public async Task<bool> IsActiveAccountAfterRegister(string email, long id)
        {
            bool isUpdate = false;
            var getData = await _userRepository.GetObjectAsync(x => x.Email == email && x.Id == id);
            if (getData != null)
            {
                getData.IsActive = true;
                isUpdate = await _userRepository.UpdateAsync(getData);
            }
            return isUpdate;
        }

        private async Task<ResponeLogin> ConvertDataForLogin(UserEntities userData, string email, string pictureUrl, string name, ETypeLogin typeLogin, HttpContext httpContext)
        {
            var returnData = new ResponeLogin();
            if (userData == null)
            {
                userData = new UserEntities()
                {
                    IsRequestTeacher = false,
                    Email = email,
                    RoleId = (int)ERoles.Student,
                    TypeLogin = (int)typeLogin,
                    Password = Generator.GeneratePassword(10),
                    PictureUrl = pictureUrl,
                    Name = name,
                    IsActive = true,
                };
                var dataInsertReturn = await _userRepository.AddReturnModelAsync(userData);

                returnData = _mapper.Map<ResponeLogin>(dataInsertReturn);
            }
            else
            {
                returnData = _mapper.Map<ResponeLogin>(userData);
            }

            if (!string.IsNullOrEmpty(returnData.Email))
            {
                var claims = new List<Claim>
                    {
                        new Claim("user-email", returnData.Email.ToString()),
                        new Claim("user-name", returnData.UserName ?? string.Empty),
                        new Claim(ClaimTypes.Role, ((ERoles)returnData.RoleId).GetEnumDescription())
                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                Thread.CurrentPrincipal = claimsPrincipal;
                returnData.Token = HelperFunction.GenerateToken(ProjectConfig.Secret ?? string.Empty, ProjectConfig.ExpiresDate, returnData.UserId ?? 0,
                    returnData.UserName ?? string.Empty, string.Empty, returnData.RoleId);

                // Generate and store refresh token
                var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var refreshTokenEntity = new RefreshTokenEntities
                {
                    UserId = returnData.UserId ?? 0,
                    Token = refreshTokenValue,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false
                };
                await _refreshTokenRepository.AddAsync(refreshTokenEntity);

                returnData.RefreshToken = refreshTokenValue;
                returnData.TokenExpires = refreshTokenEntity.ExpiresAt;
            }

            return returnData;
        }

        public async Task<ResponeLogin> RefreshAccessToken(string refreshToken, HttpContext httpContext)
        {
            var storedToken = await _refreshTokenRepository.GetObjectAsync(x => x.Token == refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new ProjectBadRequestException("Invalid or expired refresh token.");
            }

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
            {
                throw new ProjectBadRequestException(InternalCodeMessenger.NotFoundValue);
            }

            // Revoke old token (token rotation)
            storedToken.IsRevoked = true;
            var newRefreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            storedToken.ReplacedByToken = newRefreshTokenValue;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // Generate new token pair
            var returnData = _mapper.Map<ResponeLogin>(user);
            returnData.Token = HelperFunction.GenerateToken(ProjectConfig.Secret ?? string.Empty, ProjectConfig.ExpiresDate, returnData.UserId ?? 0,
                returnData.UserName ?? string.Empty, string.Empty, returnData.RoleId);

            var newRefreshToken = new RefreshTokenEntities
            {
                UserId = user.Id,
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            returnData.RefreshToken = newRefreshTokenValue;
            returnData.TokenExpires = newRefreshToken.ExpiresAt;

            return returnData;
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetObjectAsync(x => x.Token == refreshToken);
            if (storedToken == null)
            {
                return false;
            }

            storedToken.IsRevoked = true;
            return await _refreshTokenRepository.UpdateAsync(storedToken);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetObjectAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail);
            if (user == null)
            {
                // Return true even if user not found (security: don't reveal if email exists)
                return true;
            }

            // Generate 6-digit OTP code
            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // Save reset token to database
            var resetToken = new PasswordResetTokenEntities
            {
                UserId = user.Id,
                Token = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };
            await _passwordResetTokenRepository.AddAsync(resetToken);

            // Send email with OTP code
            var subject = "HDNXUdemy - Đặt lại mật khẩu";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #6366f1;'>Đặt lại mật khẩu</h2>
                    <p>Xin chào <strong>{user.Name}</strong>,</p>
                    <p>Bạn đã yêu cầu đặt lại mật khẩu. Sử dụng mã OTP bên dưới:</p>
                    <div style='background: #f3f4f6; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #6366f1;'>{otpCode}</span>
                    </div>
                    <p style='color: #6b7280;'>Mã này sẽ hết hạn sau <strong>15 phút</strong>.</p>
                    <p style='color: #6b7280;'>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                    <hr style='border: 1px solid #e5e7eb; margin: 20px 0;' />
                    <p style='color: #9ca3af; font-size: 12px;'>HDNXUdemy - Nền tảng học trực tuyến</p>
                </div>";

            try
            {
                await _sendEmailServices.SendEmailToSingleEmail(normalizedEmail, subject, body);
            }
            catch
            {
                // Log error but don't fail - token is saved, user can retry
            }

            return true;
        }

        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetObjectAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail);
            if (user == null)
            {
                throw new ProjectBadRequestException("Invalid email or reset code.");
            }

            // Find valid reset token
            var resetToken = await _passwordResetTokenRepository.GetObjectAsync(x =>
                x.UserId == user.Id &&
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
            {
                throw new ProjectBadRequestException("Invalid or expired reset code.");
            }

            // Validate password strength
            if (newPassword.Length < 8 ||
                !Regex.IsMatch(newPassword, "[A-Z]") ||
                !Regex.IsMatch(newPassword, "[a-z]") ||
                !Regex.IsMatch(newPassword, "[0-9]"))
            {
                throw new ProjectBadRequestException("Password must be at least 8 characters and include uppercase, lowercase, and number.");
            }

            // Mark token as used
            resetToken.IsUsed = true;
            await _passwordResetTokenRepository.UpdateAsync(resetToken);

            // Update password
            user.Password = PasswordHasher.Hash(newPassword);
            return await _userRepository.UpdateAsync(user);
        }
    }
}
