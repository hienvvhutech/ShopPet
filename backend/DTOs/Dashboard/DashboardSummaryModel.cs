namespace Shopping_Pet.DTOs.Dashboard
{
    public class DashboardSummaryModel
    {
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public OrderStatusStats OrderStatuses { get; set; } = new();
        public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new();

    }
    public class OrderStatusStats
    {
        public int New { get; set; }
        public int Processing { get; set; }
        public int Shipped { get; set; }
        public int Cancelled { get; set; }
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = null!;
        public decimal Revenue { get; set; }
    }
}
