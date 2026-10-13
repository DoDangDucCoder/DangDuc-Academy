using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    public class PasswordResetTokenEntities : BaseEntities
    {
        [Column("UserId")]
        public long UserId { get; set; }

        [Column("Token")]
        public string Token { get; set; } = string.Empty;

        [Column("ExpiresAt")]
        public DateTime ExpiresAt { get; set; }

        [Column("IsUsed")]
        public bool IsUsed { get; set; }
    }
}
