using HDNXUdemyModel.Model;
using System.Threading.Tasks;

namespace HDNXUdemyServices.IServices
{
    public interface ICartService
    {
        Task<CartModel> GetCartAsync(long studentId);
        Task<CartModel> AddToCartAsync(long studentId, long courseId);
        Task<CartModel> RemoveFromCartAsync(long studentId, long courseId);
        Task<bool> ClearCartAsync(long studentId);
    }
}
