namespace HDNXUdemyData.Entities
{
    public class RefreshTokenEntities : BaseEntities
    {
        public long UserId { get; set; }

        public string? Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public string? ReplacedByToken { get; set; }
    }
}
