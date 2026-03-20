using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Payments;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentModel model)
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

            var (success, message, paymentId) = await _paymentService.CreatePaymentAsync(userId, model);
            if (success)
            {
                return Ok(new { Message = message, PaymentId = paymentId });
            }

            return BadRequest(new { Message = message });
        }

        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetPaymentByOrder(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var payment = await _paymentService.GetPaymentByOrderAsync(orderId, userId);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found or not authorized" });
            }

            return Ok(payment);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusModel model)
        {
            var (success, message) = await _paymentService.UpdatePaymentStatusAsync(id, model.Status, model.TransactionId);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
    }

    public class UpdatePaymentStatusModel
    {
        public string Status { get; set; } 
        public string TransactionId { get; set; } 
    }
}
