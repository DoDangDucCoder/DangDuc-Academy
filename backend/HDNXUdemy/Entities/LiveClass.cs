using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class LiveClassEntities : BaseEntities
    {
        public long IdCourse { get; set; }
        public long IdInstructor { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public LocalDateTime StartTime { get; set; }
        public LocalDateTime EndTime { get; set; }
        public string MeetingUrl { get; set; } = string.Empty;
    }
}
