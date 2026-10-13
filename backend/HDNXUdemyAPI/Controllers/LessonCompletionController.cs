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
    /// Mark lessons as complete and track course progress
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/LessonCompletion")]
    [Authorize]
    public class LessonCompletionController : BaseController
    {
        private readonly ILessonCompletionRepository _completionRepository;

        public LessonCompletionController(ILessonCompletionRepository completionRepository)
        {
            _completionRepository = completionRepository;
        }

        /// <summary>
        /// Mark a lesson as complete
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> MarkComplete([FromBody] MarkCompleteRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _completionRepository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdContentDetail == request.IdContentDetail);

            if (existing != null && existing.Id > 0)
            {
                existing.IsCompleted = true;
                await _completionRepository.UpdateAsync(existing);
            }
            else
            {
                await _completionRepository.AddAsync(new LessonCompletionEntities
                {
                    IdStudent = userId,
                    IdCourse = request.IdCourse,
                    IdContentDetail = request.IdContentDetail,
                    IsCompleted = true
                });
            }

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Unmark a lesson (set incomplete)
        /// </summary>
        [HttpPost("uncomplete")]
        public async Task<IActionResult> MarkIncomplete([FromBody] MarkCompleteRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _completionRepository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdContentDetail == request.IdContentDetail);

            if (existing != null && existing.Id > 0)
            {
                existing.IsCompleted = false;
                await _completionRepository.UpdateAsync(existing);
            }

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get course completion progress (percentage)
        /// </summary>
        [HttpGet("progress/{idCourse}")]
        public async Task<IActionResult> GetProgress(long idCourse)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var completions = await _completionRepository.GetAsync(
                x => x.IdStudent == userId && x.IdCourse == idCourse);

            var completionList = completions.ToList();
            var totalCompleted = completionList.Count(x => x.IsCompleted);
            var totalLessons = completionList.Count;

            return Ok(new RepositoryModel<object>
            {
                Data = new
                {
                    totalCompleted,
                    totalLessons,
                    percentage = totalLessons > 0 ? Math.Round((double)totalCompleted / totalLessons * 100, 1) : 0
                },
                StatusCode = (int)HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Get completed lesson IDs for a course
        /// </summary>
        [HttpGet("completed/{idCourse}")]
        public async Task<IActionResult> GetCompletedLessons(long idCourse)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var completions = await _completionRepository.GetAsync(
                x => x.IdStudent == userId && x.IdCourse == idCourse && x.IsCompleted);

            var completedIds = completions.Select(x => x.IdContentDetail).ToList();

            return Ok(new RepositoryModel<List<long>> { Data = completedIds, StatusCode = (int)HttpStatusCode.OK });
        }
    }

    public class MarkCompleteRequest
    {
        public long IdCourse { get; set; }
        public long IdContentDetail { get; set; }
    }
}
