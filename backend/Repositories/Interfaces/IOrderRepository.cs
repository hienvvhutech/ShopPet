using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<List<Order>> GetAllAsync();

        Task<Order?> GetByIdAndUserAsync(int orderId, string userId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
    }
}
