using HDNXUdemyModel.RequestModel;
using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface IYoutubeImportService
    {
        Task<List<YoutubePlaylistPreviewResponse>> PreviewChannel(YoutubePreviewChannelRequest request);

        Task<YoutubePlaylistPreviewResponse> PreviewPlaylist(YoutubePreviewPlaylistRequest request);

        Task<YoutubeImportPlaylistResponse> ImportPlaylist(YoutubeImportPlaylistRequest request);
    }
}
