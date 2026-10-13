namespace HDNXUdemyModel.Model
{
    public class SubtitleModel
    {
        public long? Id { get; set; }
        public long IdContentDetail { get; set; }
        public string Language { get; set; } = "vi";
        public string Label { get; set; } = "Tiáº¿ng Viá»‡t";
        public string FileUrl { get; set; } = string.Empty;
        public string? PublicId { get; set; }
    }
}
