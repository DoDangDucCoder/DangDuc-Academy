using System.ComponentModel.DataAnnotations.Schema;
using DNXUdemyData.Entities;

namespace HDNXUdemyData.Entities
{
    [Table("CourseNotes")]
    public class CourseNoteEntities : BaseEntities
    {
        [Column("IdStudent")]
        public long IdStudent { get; set; }

        [Column("IdCourse")]
        public long IdCourse { get; set; }

        [Column("IdContentDetail")]
        public long IdContentDetail { get; set; }

        [Column("Timestamp")]
        public int Timestamp { get; set; }

        [Column("Content")]
        public string Content { get; set; }

        public virtual UserEntities Student { get; set; }
        public virtual CourseEntities Course { get; set; }
        public virtual ContentCourseDetailEntities ContentDetail { get; set; }
    }
}
