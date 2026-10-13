namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Courses within a learning path (ordered sequence)
    /// </summary>
    public class LearningPathCourseEntities : BaseEntities
    {
        public long IdLearningPath { get; set; }
        public long IdCourse { get; set; }
        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}
