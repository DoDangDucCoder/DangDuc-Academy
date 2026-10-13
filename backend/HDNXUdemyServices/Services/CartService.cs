using HDNXUdemyData.Entities;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HDNXUdemyServices.Services
{
    public class CartService : ICartService
    {
        private readonly ProjectContext _context;

        public CartService(ProjectContext context)
        {
            _context = context;
        }

        public async Task<CartModel> GetCartAsync(long studentId)
        {
            var cart = await _context.Carts!
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Course)
                .FirstOrDefaultAsync(c => c.IdStudent == studentId);

            if (cart == null) return new CartModel { IdStudent = studentId };

            return new CartModel
            {
                Id = cart.Id,
                IdStudent = cart.IdStudent,
                CartItems = cart.CartItems.Select(ci => new CartItemModel
                {
                    Id = ci.Id,
                    IdCart = ci.IdCart,
                    IdCourse = ci.IdCourse,
                    Price = ci.Price,
                    CourseName = ci.Course?.Title,
                    CourseThumbnail = ci.Course?.PictureUrl
                }).ToList()
            };
        }

        public async Task<CartModel> AddToCartAsync(long studentId, long courseId)
        {
            var cart = await _context.Carts!.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.IdStudent == studentId);
            if (cart == null)
            {
                cart = new CartEntities { IdStudent = studentId, CartItems = new List<CartItemEntities>() };
                _context.Carts!.Add(cart);
            }

            if (!cart.CartItems.Any(ci => ci.IdCourse == courseId))
            {
                var course = await _context.Courses!.FindAsync(courseId);
                if (course != null)
                {
                    cart.CartItems.Add(new CartItemEntities
                    {
                        IdCourse = courseId,
                        Price = course.PriceOfDiscount > 0 ? course.PriceOfDiscount : course.PriceOfCourse
                    });
                }
            }
            await _context.SaveChangesAsync();
            return await GetCartAsync(studentId);
        }

        public async Task<CartModel> RemoveFromCartAsync(long studentId, long courseId)
        {
            var cart = await _context.Carts!.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.IdStudent == studentId);
            if (cart != null)
            {
                var item = cart.CartItems.FirstOrDefault(ci => ci.IdCourse == courseId);
                if (item != null)
                {
                    _context.CartItems!.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            return await GetCartAsync(studentId);
        }

        public async Task<bool> ClearCartAsync(long studentId)
        {
            var cart = await _context.Carts!.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.IdStudent == studentId);
            if (cart != null)
            {
                _context.CartItems!.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
