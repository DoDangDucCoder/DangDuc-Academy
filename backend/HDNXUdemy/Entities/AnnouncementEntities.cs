namespace HDNXUdemyData.Entities
{
    public class AnnouncementEntities : BaseEntities
    {
        public long IdCourse { get; set; }
        public long IdInstructor { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
