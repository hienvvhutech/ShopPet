using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetByUserIdAsync(string userId);
    }
}
