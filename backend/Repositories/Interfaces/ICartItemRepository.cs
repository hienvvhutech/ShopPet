using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(int id);
        Task<CartItem?> GetByCartIdAndProductIdAsync(int cartId, int productId);
        Task<List<CartItem>> GetItemsByCartIdAsync(int cartId);
        Task AddAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task RemoveAsync(CartItem item);
        Task RemoveRangeAsync(List<CartItem> items);

    }
}
