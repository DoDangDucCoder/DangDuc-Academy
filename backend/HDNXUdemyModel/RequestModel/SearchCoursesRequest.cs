namespace HDNXUdemyModel.RequestModel
{
    public class SearchCoursesRequest
    {
        public string? Keyword { get; set; }
        public int? PriceMin { get; set; }
        public int? PriceMax { get; set; }
        public string? Level { get; set; }
        public int? MinRating { get; set; }
        public long? CategoryId { get; set; }
        public string? SortBy { get; set; } // "price_asc", "price_desc", "rating", "newest"
    }
}
