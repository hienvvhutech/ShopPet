using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Orders;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderModel model)
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

            var (success, message, orderId) = await _orderService.CreateOrderAsync(userId, model);
            if (success)
            {
                return Ok(new { Message = message, OrderId = orderId });
            }

            return BadRequest(new { Message = message });
        }

        [HttpGet]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var orders = await _orderService.GetOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var order = await _orderService.GetOrderByIdAsync(id, userId);
            if (order == null)
            {
                return NotFound(new { Message = "Order not found or not authorized" });
            }

            return Ok(order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _orderService.UpdateOrderStatusAsync(id, userId, model.Status, userRole);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _orderService.CancelOrderAsync(id, userId);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
        [HttpGet("admin")]
        [Authorize(Policy = "StaffOrAdmin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync(); 
            return Ok(orders);
        }

    }

    public class UpdateOrderStatusModel
    {
        public string Status { get; set; } 
    }
}

