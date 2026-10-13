namespace HDNXUdemyData.Entities
{
    /// <summary>
    /// Course prerequisite relationships
    /// </summary>
    public class CoursePrerequisiteEntities : BaseEntities
    {
        public long IdCourse { get; set; }
        public long IdPrerequisiteCourse { get; set; }
    }
}
