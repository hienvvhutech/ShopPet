using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shopping_Pet.Services;
using Shopping_Pet.DTOs.Notifications;

namespace Shopping_Pet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var list = await _notificationService.GetMyNotificationsAsync(userId);

            var result = list.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                OrderId = n.OrderId
            });

            return Ok(result);
        }
    }
}
