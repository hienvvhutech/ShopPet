using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.DTOs.Chat;
using Shopping_Pet.Models;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("{receiverId}")]
        public async Task<IActionResult> GetMessages(string receiverId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return Unauthorized();

            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                         || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var unreadMessages = messages
                .Where(m => m.ReceiverId == senderId && !m.IsRead)
                .ToList();

            if (unreadMessages.Count > 0)
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatDto dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return Unauthorized();

            var chat = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                Timestamp = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(chat);
            await _context.SaveChangesAsync();

            return Ok(chat);
        }

        [HttpGet("unread-counts")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (adminId == null) return Unauthorized();

            var unreadCounts = await _context.ChatMessages
                .Where(m => m.ReceiverId == adminId && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    UnreadCount = g.Count()
                })
                .ToListAsync();

            return Ok(unreadCounts);
        }
    }
}
