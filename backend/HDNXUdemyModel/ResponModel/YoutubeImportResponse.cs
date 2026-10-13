namespace HDNXUdemyModel.ResponModel
{
    public class YoutubePlaylistPreviewResponse
    {
        public string? PlaylistId { get; set; }

        public string? ChannelId { get; set; }

        public string? ChannelTitle { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int VideoCount { get; set; }

        public string? TotalDuration { get; set; }

        public List<YoutubeVideoPreviewResponse> Videos { get; set; } = new();
    }

    public class YoutubeVideoPreviewResponse
    {
        public string? VideoId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? Duration { get; set; }

        public int Position { get; set; }
    }

    public class YoutubeImportPlaylistResponse
    {
        public long CourseId { get; set; }

        public long ContentId { get; set; }

        public string? PlaylistId { get; set; }

        public string? CourseTitle { get; set; }

        public int ImportedLessons { get; set; }
    }
}
