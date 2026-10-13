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
    /// API for tracking video engagement events (heatmap analytics)
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/VideoEngagement")]
    [Authorize]
    public class VideoEngagementController : BaseController
    {
        private readonly IVideoEngagementRepository _repository;

        public VideoEngagementController(IVideoEngagementRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Batch save engagement events (called every 30 seconds by frontend)
        /// </summary>
        [HttpPost("batch")]
        public async Task<IActionResult> SaveBatchEvents([FromBody] BatchEngagementRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();
            if (request.Events == null || request.Events.Count == 0)
                return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });

            foreach (var evt in request.Events)
            {
                var entity = new VideoEngagementEntities
                {
                    IdStudent = userId,
                    IdContentDetail = evt.IdContentDetail,
                    EventType = evt.EventType,
                    VideoPosition = evt.VideoPosition,
                    VideoDuration = evt.VideoDuration,
                    UserAgent = Request.Headers.UserAgent.ToString()
                };
                await _repository.AddAsync(entity);
            }

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get engagement summary for a content detail (Admin/Instructor only)
        /// </summary>
        [HttpGet("summary/{idContentDetail}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEngagementSummary(long idContentDetail)
        {
            var events = await _repository.GetAsync(x => x.IdContentDetail == idContentDetail);
            var eventList = events.ToList();

            var summary = new
            {
                TotalEvents = eventList.Count,
                UniqueViewers = eventList.Select(e => e.IdStudent).Distinct().Count(),
                PlayCount = eventList.Count(e => e.EventType == "play"),
                PauseCount = eventList.Count(e => e.EventType == "pause"),
                SeekCount = eventList.Count(e => e.EventType == "seek"),
                CompletionCount = eventList.Count(e => e.EventType == "ended"),
                AverageDropoffPosition = eventList.Where(e => e.EventType == "ended" && e.VideoDuration > 0)
                    .Select(e => e.VideoPosition / e.VideoDuration * 100)
                    .DefaultIfEmpty(0)
                    .Average()
            };

            return Ok(new RepositoryModel<object> { Data = summary, StatusCode = (int)HttpStatusCode.OK });
        }
    }

    public class BatchEngagementRequest
    {
        public List<EngagementEvent> Events { get; set; } = new();
    }

    public class EngagementEvent
    {
        public long IdContentDetail { get; set; }
        public string EventType { get; set; } = string.Empty;
        public double VideoPosition { get; set; }
        public double VideoDuration { get; set; }
    }
}
