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
using System.Text.RegularExpressions;

namespace HDNXUdemyServices.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IUserRepository _userRepository;
        private readonly ISystemConfigRepository _systemConfigRepository;
        private readonly HttpClient _httpClient;
        private readonly IEmailServices _emailServices;
        private readonly IMapper _mapper;

        public AuthenticationServices(IUserRepository userRepository, HttpClient httpClient, IMapper mapper, ISystemConfigRepository systemConfigRepository,
            IEmailServices emailServices)
        {
            _userRepository = userRepository ?? throw new ProjectException(nameof(_userRepository));
            _httpClient = httpClient ?? throw new ProjectException(nameof(_httpClient));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _systemConfigRepository = systemConfigRepository ?? throw new ProjectException(nameof(_systemConfigRepository));
            _emailServices = emailServices ?? throw new ProjectException(nameof(_emailServices));
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
            }

            return returnData;
        }
    }
}
