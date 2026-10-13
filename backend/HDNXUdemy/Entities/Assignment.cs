using System;

namespace HDNXUdemyData.Entities
{
    public class AssignmentEntities : BaseEntities
    {
        public long IdCourse { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxScore { get; set; } = 100;
        public DateTime? DueDate { get; set; }
    }
}
