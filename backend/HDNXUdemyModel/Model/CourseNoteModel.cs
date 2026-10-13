using HDNXUdemyModel.Base;

namespace HDNXUdemyModel.Model
{
    public class CourseNoteModel : BaseModel
    {
        public long IdStudent { get; set; }
        public long IdCourse { get; set; }
        public long IdContentDetail { get; set; }
        public int Timestamp { get; set; }
        public string? Content { get; set; }
    }
}
