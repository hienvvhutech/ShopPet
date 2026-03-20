using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using System.Security.Claims;

namespace Shopping_Pet.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatHub> _logger;

        private static readonly Dictionary<string, HashSet<string>> Connections = new();

        public ChatHub(AppDbContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return base.OnConnectedAsync();

            lock (Connections)
            {
                if (!Connections.ContainsKey(userId))
                    Connections[userId] = new HashSet<string>();

                Connections[userId].Add(Context.ConnectionId);
            }

            _logger.LogInformation($"[SignalR] Connected: {userId} - {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                lock (Connections)
                {
                    if (Connections.TryGetValue(userId, out var list))
                    {
                        list.Remove(Context.ConnectionId);
                        if (list.Count == 0)
                            Connections.Remove(userId);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToAdmin(string message)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var time = DateTime.Now.ToString("HH:mm");

            if (string.IsNullOrEmpty(userId)) return;

            var adminId = _context.UserRoles
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == "Admin")
                .Select(x => x.UserId)
                .FirstOrDefault();

            if (adminId == null) return;

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderId = userId,
                ReceiverId = adminId,
                Message = message,
                Timestamp = DateTime.Now,
                IsRead = false
            });

            await _context.SaveChangesAsync();

            if (Connections.TryGetValue(adminId, out var adminConnections))
            {
                foreach (var conn in adminConnections)
                {
                    await Clients.Client(conn).SendAsync(
                        "ReceiveMessage",
                        userId,
                        "User",
                        message,
                        time
                    );
                }
            }
        }
        public async Task SendMessageToUser(string userId, string message)
        {
            var adminId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var time = DateTime.Now.ToString("HH:mm");

            if (string.IsNullOrEmpty(adminId)) return;

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderId = adminId,
                ReceiverId = userId,
                Message = message,
                Timestamp = DateTime.Now,
                IsRead = false
            });

            await _context.SaveChangesAsync();
            if (Connections.TryGetValue(userId, out var userConnections))
            {
                foreach (var conn in userConnections)
                {
                    await Clients.Client(conn).SendAsync(
                        "ReceiveMessage",
                        adminId,
                        "Admin",
                        message,
                        time
                    );
                }
            }
        }
    }
}
