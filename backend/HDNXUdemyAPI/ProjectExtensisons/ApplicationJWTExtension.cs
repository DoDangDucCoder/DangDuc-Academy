using HDNXUdemyModel.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HDNXUdemyAPI.ProjectExtensisons
{
    /// <summary>
    /// ApplicationJWTExtensions
    /// </summary>
    public static class ApplicationJWTExtensions
    {
        /// <summary>
        /// CustomerApplicationJWTExtension
        /// </summary>
        /// <param name="services"></param>
        public static void CustomerApplicationJWTExtension(this IServiceCollection services)
        {
            var secret = ProjectConfig.Secret ?? string.Empty;
            if (Encoding.ASCII.GetByteCount(secret) < 32)
            {
                throw new InvalidOperationException("JWT secret must be at least 256 bits.");
            }

            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(a =>
            {
                a.RequireHttpsMetadata = true;
                a.SaveToken = false;
                a.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            }).AddCookie(c =>
            {
                c.Cookie.HttpOnly = true;
                c.ExpireTimeSpan = TimeSpan.FromDays(ProjectConfig.ExpiresDate);
                c.SlidingExpiration = true;
            });
        }
    }
}
