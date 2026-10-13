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
    /// Q&A Upvote/Downvote system for course questions and answers
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/QAVote")]
    [Authorize]
    public class QAVoteController : BaseController
    {
        private readonly IQAVoteRepository _voteRepository;

        public QAVoteController(IQAVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        /// <summary>
        /// Vote on a question or answer
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Vote([FromBody] VoteRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            // Check existing vote
            var existing = await _voteRepository.GetObjectAsync(
                x => x.IdQuestion == request.IdQuestion && x.IdUser == userId && x.IsForAnswer == request.IsForAnswer);

            if (existing != null && existing.Id > 0)
            {
                if (existing.IsUpvote == request.IsUpvote)
                {
                    // Same vote = remove (toggle off)
                    await _voteRepository.DeleteByKey(existing.Id);
                    return Ok(new RepositoryModel<string> { Data = "removed", StatusCode = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Change vote direction
                    existing.IsUpvote = request.IsUpvote;
                    await _voteRepository.UpdateAsync(existing);
                    return Ok(new RepositoryModel<string> { Data = "changed", StatusCode = (int)HttpStatusCode.OK });
                }
            }

            // New vote
            await _voteRepository.AddAsync(new QAVoteEntities
            {
                IdQuestion = request.IdQuestion,
                IdUser = userId,
                IsUpvote = request.IsUpvote,
                IsForAnswer = request.IsForAnswer
            });

            return Ok(new RepositoryModel<string> { Data = "voted", StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get vote count for a question/answer
        /// </summary>
        [HttpGet("count/{idQuestion}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVoteCount(long idQuestion, [FromQuery] bool isForAnswer = false)
        {
            var votes = await _voteRepository.GetAsync(
                x => x.IdQuestion == idQuestion && x.IsForAnswer == isForAnswer);

            var voteList = votes.ToList();
            var upvotes = voteList.Count(v => v.IsUpvote);
            var downvotes = voteList.Count(v => !v.IsUpvote);

            return Ok(new RepositoryModel<object>
            {
                Data = new { upvotes, downvotes, score = upvotes - downvotes },
                StatusCode = (int)HttpStatusCode.OK
            });
        }
    }

    public class VoteRequest
    {
        public long IdQuestion { get; set; }
        public bool IsUpvote { get; set; }
        public bool IsForAnswer { get; set; }
    }
}
