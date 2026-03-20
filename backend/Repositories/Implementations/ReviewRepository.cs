using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<List<Review>> GetVisibleByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetAllByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task HideReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.IsHidden = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnhideReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                review.IsHidden = false;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Review>> GetByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

    }
}
