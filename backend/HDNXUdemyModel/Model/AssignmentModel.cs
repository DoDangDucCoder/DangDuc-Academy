using System;
using HDNXUdemyModel.Base;

namespace HDNXUdemyModel.Model
{
    public class AssignmentModel : BaseModel
    {
        public long IdCourse { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxScore { get; set; } = 100;
        public DateTime? DueDate { get; set; }
    }
}
