using HDNXUdemyModel.Base;
using System.Collections.Generic;

namespace HDNXUdemyModel.Model
{
    public class CartModel : BaseModel
    {
        public long IdStudent { get; set; }
        public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }

    public class CartItemModel : BaseModel
    {
        public long IdCart { get; set; }
        public long IdCourse { get; set; }
        public decimal Price { get; set; }
        public string? CourseName { get; set; }
        public string? CourseThumbnail { get; set; }
    }
}
