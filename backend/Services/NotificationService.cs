using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }
        public async Task CreateAsync(
            string userId,
            string title,
            string content,
            int? orderId   
        )
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                OrderId = orderId  
            };

            await _notificationRepo.AddAsync(notification);
        }

        public async Task<List<Notification>> GetMyNotificationsAsync(string userId)
        {
            return await _notificationRepo.GetByUserIdAsync(userId);
        }
    }
}
