namespace HDNXUdemyData.Entities
{
    public class VideoEngagementEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdContentDetail { get; set; }
        public string EventType { get; set; } = string.Empty; // play, pause, seek, ended, buffer
        public double VideoPosition { get; set; }
        public double VideoDuration { get; set; }
        public string? UserAgent { get; set; }
    }
}
