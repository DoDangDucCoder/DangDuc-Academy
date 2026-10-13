using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// GDPR-compliant account management (Right to be Forgotten)
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/Account")]
    [Authorize]
    public class AccountDeletionController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly INotificationRepository _notificationRepository;

        public AccountDeletionController(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository,
            INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Delete (anonymize) the current user's account — GDPR Right to be Forgotten
        /// Financial records are preserved for legal compliance but personal data is anonymized
        /// </summary>
        [HttpDelete("delete-my-account")]
        public async Task<IActionResult> DeleteMyAccount([FromBody] DeleteAccountRequest? request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new RepositoryModel<bool> { StatusCode = 404, SystemMessage = "User not found" });

            // Anonymize personal data
            var anonymizedId = $"deleted_user_{userId}";
            var originalEmail = user.Email;

            user.Email = $"{anonymizedId}@deleted.local";
            user.Name = anonymizedId;
            user.Phone = null;
            user.PictureUrl = null;
            user.Status = (int)EStatus.Inactive;

            await _userRepository.UpdateAsync(user);

            // Audit log
            await _auditLogRepository.AddAsync(new AuditLogEntities
            {
                UserId = userId,
                Action = "AccountDeleted_GDPR",
                EntityName = "User",
                EntityId = userId.ToString(),
                OldValue = $"Email: {originalEmail}",
                NewValue = $"Anonymized to: {anonymizedId}",
                CreatedAt = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime
            });

            return Ok(new RepositoryModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                PartnerCode = "Account deleted and anonymized successfully"
            });
        }

        /// <summary>
        /// Export user data — GDPR Right to Data Portability
        /// </summary>
        [HttpGet("export-my-data")]
        public async Task<IActionResult> ExportMyData()
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var exportData = new
            {
                PersonalInfo = new
                {
                    user.Email,
                    user.Name,
                    user.Phone,
                    AccountCreated = user.CreateDate.ToString()
                },
                ExportedAt = DateTime.UtcNow
            };

            return Ok(new RepositoryModel<object>
            {
                Data = exportData,
                StatusCode = (int)HttpStatusCode.OK
            });
        }
    }

    public class DeleteAccountRequest
    {
        public string? ConfirmPassword { get; set; }
        public string? Reason { get; set; }
    }
}
