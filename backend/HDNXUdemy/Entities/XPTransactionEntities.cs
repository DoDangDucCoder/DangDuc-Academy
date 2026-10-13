namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// XP points transactions log
    /// </summary>
    public class XPTransactionEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty; // lesson_complete, quiz_pass, streak_bonus, review_given, first_purchase
        public long? IdCourse { get; set; }
    }
}
