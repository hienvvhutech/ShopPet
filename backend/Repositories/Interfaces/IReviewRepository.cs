using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<List<Review>> GetVisibleByProductIdAsync(int productId);
        Task<List<Review>> GetAllByProductIdAsync(int productId);
        Task<List<Review>> GetByUserIdAsync(string userId);
        Task<List<Review>> GetAllAsync();

        Task<Review?> GetByUserAndProductAsync(string userId, int productId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task HideReviewAsync(int reviewId);
        Task UnhideReviewAsync(int reviewId);
       
    }
}
