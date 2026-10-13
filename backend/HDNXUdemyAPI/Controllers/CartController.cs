using Asp.Versioning;
using HDNXUdemyModel.Model;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HDNXUdemyAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Cart")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(CurrentUserId());
            return Ok(cart);
        }

        [HttpPost("add/{courseId}")]
        public async Task<IActionResult> AddToCart(long courseId)
        {
            var cart = await _cartService.AddToCartAsync(CurrentUserId(), courseId);
            return Ok(cart);
        }

        [HttpDelete("remove/{courseId}")]
        public async Task<IActionResult> RemoveFromCart(long courseId)
        {
            var cart = await _cartService.RemoveFromCartAsync(CurrentUserId(), courseId);
            return Ok(cart);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(CurrentUserId());
            return Ok(result);
        }
    }
}
