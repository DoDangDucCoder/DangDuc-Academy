using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.Article)]
    public class ArticleController : BaseController
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("paged/{pageIndex}/{pageSize}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedArticles(int pageIndex, int pageSize, [FromQuery] string search = null)
        {
            RepositoryModel<PagedResult<ArticleResponse>> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _articleService.GetPagedArticles(pageIndex, pageSize, search),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticleById(long id)
        {
            var data = await _articleService.GetArticleById(id);
            if (data == null)
            {
                return NotFound();
            }

            RepositoryModel<ArticleResponse> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = data,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            return Ok(result);
        }

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticleBySlug(string slug)
        {
            var data = await _articleService.GetArticleBySlug(slug);
            if (data == null)
            {
                return NotFound();
            }

            // Optional: Increment view count here
            await _articleService.IncrementViewCount(slug);

            RepositoryModel<ArticleResponse> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = data,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleRequest request)
        {
            var data = await _articleService.CreateArticle(request);
            RepositoryModel<ArticleResponse> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = data,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArticle(long id, [FromBody] ArticleRequest request)
        {
            var data = await _articleService.UpdateArticle(id, request);
            if (data == null)
            {
                return NotFound();
            }

            RepositoryModel<ArticleResponse> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = data,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticle(long id)
        {
            var success = await _articleService.DeleteArticle(id);
            if (!success)
            {
                return NotFound();
            }

            RepositoryModel<bool> result = new()
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = true,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };
            return Ok(result);
        }
    }
}
