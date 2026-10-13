namespace HDNXUdemyModel.RequestModel
{
    public class YoutubePreviewChannelRequest
    {
        public string? ChannelUrl { get; set; }
    }

    public class YoutubePreviewPlaylistRequest
    {
        public string? PlaylistUrlOrId { get; set; }
    }

    public class YoutubeImportPlaylistRequest
    {
        public string? PlaylistUrlOrId { get; set; }

        public long IdCategory { get; set; }

        public int IdAuthor { get; set; }

        public string? LevelCourse { get; set; }

        public int Language { get; set; }

        public bool PublishNow { get; set; }

        public bool IsLearningFree { get; set; } = true;
    }
}
