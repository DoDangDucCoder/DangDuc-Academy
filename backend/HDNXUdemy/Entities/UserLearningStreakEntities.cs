using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("UserLearningStreaks", Schema = "sche_dev_HDNXUdemy")]
    public class UserLearningStreakEntities
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdUser { get; set; }
        [MaxLength(5)] public string Language { get; set; } = "all";
        public DateTime LastActiveDate { get; set; } = DateTime.UtcNow.Date;
        public int CurrentStreak { get; set; } = 1;
        public int LongestStreak { get; set; } = 1;
        public int TotalActiveDays { get; set; } = 1;
    }
}
