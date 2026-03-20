using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Dashboard;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{

    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var result = await _dashboardService.GetSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("order-status")]
        public async Task<ActionResult<OrderStatusStats>> GetOrderStatusStats()
        {
            var stats = await _dashboardService.GetOrderStatusStatsAsync();
            return Ok(stats);
        }

    }
}
