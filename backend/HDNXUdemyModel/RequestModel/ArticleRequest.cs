using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class ArticleRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
