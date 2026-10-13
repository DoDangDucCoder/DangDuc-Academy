using HDNXUdemyModel.Base;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HDNXUdemyAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class SubtitleController : BaseController
    {
        private readonly ISubtitleService _subtitleService;

        public SubtitleController(ISubtitleService subtitleService)
        {
            _subtitleService = subtitleService;
        }

        [HttpGet("video/{idContentDetail}")]
        public async Task<IActionResult> GetSubtitlesByContentDetail(long idContentDetail)
        {
            var data = await _subtitleService.GetSubtitlesByContentDetailAsync(idContentDetail);
            return Ok(new RepositoryModel<List<SubtitleModel>> { Data = data, StatusCode = 200 });
        }

        [HttpPost]
        public async Task<IActionResult> AddSubtitle([FromForm] long idContentDetail, [FromForm] string language, [FromForm] string label, IFormFile file)
        {
            var data = await _subtitleService.AddSubtitleAsync(idContentDetail, language, label, file);
            return Ok(new RepositoryModel<SubtitleModel> { Data = data, StatusCode = 200 });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubtitle(long id)
        {
            var data = await _subtitleService.DeleteSubtitleAsync(id);
            return Ok(new RepositoryModel<bool> { Data = data, StatusCode = 200 });
        }
    }
}
