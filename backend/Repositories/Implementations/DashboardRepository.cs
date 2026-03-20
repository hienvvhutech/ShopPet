using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.DTOs.Dashboard;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderStatusStats> GetOrderStatusStatsAsync()
        {
            return new OrderStatusStats
            {
                New = await _context.Orders.CountAsync(o => o.Status == "New"),
                Processing = await _context.Orders.CountAsync(o => o.Status == "Pending"),
                Shipped = await _context.Orders.CountAsync(o => o.Status == "Delivered"),
                Cancelled = await _context.Orders.CountAsync(o => o.Status == "Cancelled")
            };
        }

        public async Task<DashboardSummaryModel> GetSummaryAsync()
        {
            try
            {
                var totalOrders = await _context.Orders.CountAsync();
                var totalUsers = await _context.Users.CountAsync();
                var totalProducts = await _context.Products.CountAsync();

                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                var statusStats = new OrderStatusStats
                {
                    New = await _context.Orders.CountAsync(o => o.Status == "New"),
                    Processing = await _context.Orders.CountAsync(o => o.Status == "Pending"),
                    Shipped = await _context.Orders.CountAsync(o => o.Status == "Delivered"),
                    Cancelled = await _context.Orders.CountAsync(o => o.Status == "Cancelled")
                };

                var rawRevenue = await _context.Orders
                    .Where(o => o.Status == "Delivered")
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Revenue = g.Sum(o => (decimal?)o.TotalAmount ?? 0)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                var monthlyRevenues = rawRevenue
                    .Select(x => new MonthlyRevenue
                    {
                        Month = $"{x.Month:00}/{x.Year}",
                        Revenue = x.Revenue
                    })
                    .ToList();

                return new DashboardSummaryModel
                {
                    TotalOrders = totalOrders,
                    TotalUsers = totalUsers,
                    TotalProducts = totalProducts,
                    TotalRevenue = totalRevenue,
                    OrderStatuses = statusStats,
                    MonthlyRevenues = monthlyRevenues
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Lỗi GetSummaryAsync(): " + ex.Message);
                throw;
            }
        }
    }
}
