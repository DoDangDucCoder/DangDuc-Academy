namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Tracks per-lesson completion status for a student
    /// </summary>
    public class LessonCompletionEntities : BaseEntities
    {
        public long IdStudent { get; set; }
        public long IdCourse { get; set; }
        public long IdContentDetail { get; set; }
        public bool IsCompleted { get; set; }
    }
}
