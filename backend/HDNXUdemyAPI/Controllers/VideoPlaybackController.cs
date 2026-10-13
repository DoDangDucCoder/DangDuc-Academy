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
    /// API for saving/restoring video playback position (cross-device resume)
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/VideoPlayback")]
    [Authorize]
    public class VideoPlaybackController : BaseController
    {
        private readonly IVideoPlaybackPositionRepository _repository;

        public VideoPlaybackController(IVideoPlaybackPositionRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Save current playback position (called every 10 seconds by frontend)
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SavePosition([FromBody] SavePositionRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _repository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdContentDetail == request.IdContentDetail);

            if (existing != null && existing.Id > 0)
            {
                existing.PositionSeconds = request.PositionSeconds;
                existing.DurationSeconds = request.DurationSeconds;
                existing.LastUpdated = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime;
                await _repository.UpdateAsync(existing);
            }
            else
            {
                var entity = new VideoPlaybackPositionEntities
                {
                    IdStudent = userId,
                    IdContentDetail = request.IdContentDetail,
                    PositionSeconds = request.PositionSeconds,
                    DurationSeconds = request.DurationSeconds,
                    LastUpdated = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime
                };
                await _repository.AddAsync(entity);
            }

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get saved playback position for a specific lesson
        /// </summary>
        [HttpGet("{idContentDetail}")]
        public async Task<IActionResult> GetPosition(long idContentDetail)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var position = await _repository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdContentDetail == idContentDetail);

            if (position == null || position.Id == 0)
            {
                return Ok(new RepositoryModel<double> { Data = 0, StatusCode = (int)HttpStatusCode.OK });
            }

            return Ok(new RepositoryModel<double> { Data = position.PositionSeconds, StatusCode = (int)HttpStatusCode.OK });
        }
    }

    public class SavePositionRequest
    {
        public long IdContentDetail { get; set; }
        public double PositionSeconds { get; set; }
        public double DurationSeconds { get; set; }
    }
}
