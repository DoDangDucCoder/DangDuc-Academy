using DNXUdemyData.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    public class SubtitleEntities : BaseEntities
    {
        [ForeignKey("ContentCourseDetail")]
        public long IdContentDetail { get; set; }

        public string Language { get; set; } = "vi";

        public string Label { get; set; } = "Tiáº¿ng Viá»‡t";

        public string FileUrl { get; set; } = string.Empty;

        public string? PublicId { get; set; }
        
        public virtual ContentCourseDetailEntities? ContentCourseDetail { get; set; }
    }
}
