using Shopping_Pet.DTOs.Dashboard;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class DashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public Task<DashboardSummaryModel> GetSummaryAsync()
        {
            return _dashboardRepo.GetSummaryAsync();
        }
        public Task<OrderStatusStats> GetOrderStatusStatsAsync()
        {
            return _dashboardRepo.GetOrderStatusStatsAsync();
        }

    }
}
