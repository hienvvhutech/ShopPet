using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public async Task<CartItem?> GetByCartIdAndProductIdAsync(int cartId, int productId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<List<CartItem>> GetItemsByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task AddAsync(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(List<CartItem> items)
        {
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

    }
}
