using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class VideoPlaybackPositionEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdContentDetail { get; set; }
        public double PositionSeconds { get; set; }
        public double DurationSeconds { get; set; }
        public LocalDateTime LastUpdated { get; set; }
    }
}
