using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(int id);
        Task AddAsync(OrderItem item);
        Task UpdateAsync(OrderItem item);
        Task DeleteAsync(OrderItem item);

        Task<bool> HasUserPurchasedProductAsync(string userId, int productId);
    }
}
