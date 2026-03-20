using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Reviews;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewModel model)
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

            var (success, message, reviewId) = await _reviewService.CreateReviewAsync(userId, model);
            if (success)
            {
                return Ok(new { Message = message, ReviewId = reviewId });
            }

            return BadRequest(new { Message = message });
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductAsync(productId);
            return Ok(reviews);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] CreateReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _reviewService.UpdateReviewAsync(id, userId, model, userRole);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var (success, message) = await _reviewService.DeleteReviewAsync(id, userId, userRole);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
        [HttpPut("{id}/hide")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleHideReview(int id, [FromQuery] bool hide)
        {
            var (success, message) = await _reviewService.HideReviewAsync(id, hide);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var reviews = await _reviewService.GetReviewsByUserAsync(userId);
            return Ok(reviews);
        }
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllAsync();
            return Ok(reviews);
        }

    }
}
