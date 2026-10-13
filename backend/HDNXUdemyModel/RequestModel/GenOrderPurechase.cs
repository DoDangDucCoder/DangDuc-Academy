using System.ComponentModel.DataAnnotations;

namespace HDNXUdemyModel.RequestModel
{
    public class GenOrderPurechase
    {
        [Required]
        [MinLength(1)]
        public List<int> IdCourses { get; set; } = new();

        [Required]
        [Range(1, int.MaxValue)]
        public int IdStudent { get; set; }
    }
}
