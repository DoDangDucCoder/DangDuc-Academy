using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("CartItems")]
    public class CartItemEntities : BaseEntities
    {
        [Column("IdCart")]
        public long IdCart { get; set; }

        [Column("IdCourse")]
        public long IdCourse { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        public virtual CartEntities Cart { get; set; }
        public virtual CourseEntities Course { get; set; }
    }
}
