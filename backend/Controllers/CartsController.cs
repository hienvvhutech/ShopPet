using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Carts;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartsController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("items")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> AddCartItem([FromBody] AddCartItemModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message, cartItemId) = await _cartService.AddCartItemAsync(userId, model);
            if (success)
            {
                return Ok(new { Message = message, CartItemId = cartItemId });
            }

            return BadRequest(new { Message = message });
        }

        [HttpPut("items/{cartItemId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _cartService.UpdateCartItemAsync(userId, cartItemId, model);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [HttpDelete("items/{cartItemId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _cartService.RemoveCartItemAsync(userId, cartItemId);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [HttpGet]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "User not authenticated" });

                var cartDto = await _cartService.GetCartDtoAsync(userId);
                if (cartDto == null)
                    return NotFound(new { Message = "Cart not found" });

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi server", Detail = ex.Message });
            }
        }
        [HttpDelete("clear")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var (success, message) = await _cartService.ClearCartAsync(userId);
            if (success)
                return Ok(new { Message = message });

            return BadRequest(new { Message = message });
        }

    }
}