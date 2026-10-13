using HDNXUdemyModel.Constant;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("Articles")]
    public class Article : BaseEntities
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        
        public long AuthorId { get; set; }
        
        [DefaultValue(0)]
        public long ViewCount { get; set; }
    }
}
