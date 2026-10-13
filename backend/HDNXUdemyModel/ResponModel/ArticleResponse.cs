using System;

namespace HDNXUdemyModel.ResponModel
{
    public class ArticleResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        public long AuthorId { get; set; }
        public string AuthorName { get; set; }
        public long ViewCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
