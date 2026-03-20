using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
    }
}
