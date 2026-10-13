using Asp.Versioning;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// Import YouTube playlists into course content.
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route(RouterControllerName.YoutubeImport)]
    //[Authorize(Roles = "Admin")]
    public class YoutubeImportController : BaseController
    {
        private readonly IYoutubeImportService _youtubeImportService;
        private readonly IAuditLogService _auditLogService;

        public YoutubeImportController(IYoutubeImportService youtubeImportService, IAuditLogService auditLogService)
        {
            _youtubeImportService = youtubeImportService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Preview public playlists from a YouTube channel URL.
        /// </summary>
        [HttpPost("preview-channel")]
        public async Task<RepositoryModel<List<YoutubePlaylistPreviewResponse>>> PreviewChannel(YoutubePreviewChannelRequest request)
        {
            var result = new RepositoryModel<List<YoutubePlaylistPreviewResponse>>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _youtubeImportService.PreviewChannel(request),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            return result;
        }

        /// <summary>
        /// Preview videos in a YouTube playlist before importing it.
        /// </summary>
        [HttpPost("preview-playlist")]
        public async Task<RepositoryModel<YoutubePlaylistPreviewResponse>> PreviewPlaylist(YoutubePreviewPlaylistRequest request)
        {
            var result = new RepositoryModel<YoutubePlaylistPreviewResponse>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = await _youtubeImportService.PreviewPlaylist(request),
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.OK
            };

            return result;
        }

        /// <summary>
        /// Import a YouTube playlist as one course with lessons.
        /// </summary>
        [HttpPost("import-playlist")]
        public async Task<IActionResult> ImportPlaylist(YoutubeImportPlaylistRequest request)
        {
            var importResult = await _youtubeImportService.ImportPlaylist(request);
            await _auditLogService.WriteAsync(CurrentUserId(), "YoutubeImport.ImportPlaylist", "Course", importResult.CourseId.ToString(), null, request);

            var result = new RepositoryModel<YoutubeImportPlaylistResponse>
            {
                PartnerCode = Messenger.SuccessFull,
                RetCode = ERetCode.Successfull,
                Data = importResult,
                SystemMessage = string.Empty,
                StatusCode = (int)HttpStatusCode.Created
            };

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
