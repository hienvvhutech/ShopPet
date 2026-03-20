using Shopping_Pet.DTOs.Dashboard;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryModel> GetSummaryAsync();
        Task<OrderStatusStats> GetOrderStatusStatsAsync();
        
    }
}
