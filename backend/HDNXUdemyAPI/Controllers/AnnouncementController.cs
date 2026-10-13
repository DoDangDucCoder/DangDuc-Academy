using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// Instructor Announcements — Send notifications to all enrolled students
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/Announcement")]
    [Authorize]
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPurcharseCourseRepository _purchaseRepository;

        public AnnouncementController(
            IAnnouncementRepository announcementRepository,
            INotificationRepository notificationRepository,
            IPurcharseCourseRepository purchaseRepository)
        {
            _announcementRepository = announcementRepository;
            _notificationRepository = notificationRepository;
            _purchaseRepository = purchaseRepository;
        }

        /// <summary>
        /// Create announcement (instructor only)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var announcement = new AnnouncementEntities
            {
                IdCourse = request.IdCourse,
                IdInstructor = userId,
                Title = request.Title,
                Content = request.Content
            };

            await _announcementRepository.AddAsync(announcement);

            // Send notification to all enrolled students
            var purchases = await _purchaseRepository.GetAsync(
                x => x.Status == (int)EStatus.Active);

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get announcements for a course
        /// </summary>
        [HttpGet("course/{idCourse}")]
        public async Task<IActionResult> GetAnnouncements(long idCourse)
        {
            var announcements = await _announcementRepository.GetAsync(
                x => x.IdCourse == idCourse && x.Status == (int)EStatus.Active);

            var result = announcements.OrderByDescending(x => x.CreateDate).Select(a => new
            {
                a.Id,
                a.Title,
                a.Content,
                a.IdInstructor,
                createdDate = a.CreateDate.ToString()
            }).ToList();

            return Ok(new RepositoryModel<object> { Data = result, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Delete announcement
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(long id)
        {
            var userId = CurrentUserId();
            var announcement = await _announcementRepository.GetByIdAsync(id);
            if (announcement == null || announcement.IdInstructor != userId)
                return Forbid();

            await _announcementRepository.DeleteByKey(id);
            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }
    }

    public class CreateAnnouncementRequest
    {
        public long IdCourse { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
