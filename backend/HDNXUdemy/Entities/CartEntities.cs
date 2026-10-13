using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    [Table("Carts")]
    public class CartEntities : BaseEntities
    {
        [Column("IdStudent")]
        public long IdStudent { get; set; }

        public virtual UserEntities Student { get; set; }

        public virtual ICollection<CartItemEntities> CartItems { get; set; }
    }
}
