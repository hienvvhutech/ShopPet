using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _context.OrderItems.FindAsync(id);
        }
        public async Task<bool> HasUserPurchasedProductAsync(string userId, int productId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.ProductId == productId && oi.Order.UserId == userId);
        }

        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderItem item)
        {
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderItem item)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
