using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Users;
using Shopping_Pet.Models;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(
            UserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpPost("staff")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });

            var (success, message) = await _userService.CreateStaffAsync(model);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }

        [HttpGet("staff")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetStaffList()
        {
            var staffList = await _userService.GetStaffListAsync();
            return Ok(staffList);
        }

        [HttpPut("staff/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStaff(string userId, [FromBody] UpdateStaffModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage) });

            var (success, message) = await _userService.UpdateStaffAsync(userId, model);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }

        [HttpDelete("staff/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteStaff(string userId)
        {
            var (success, message) = await _userService.DeleteStaffAsync(userId);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }

        [HttpGet("customers")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetCustomerList()
        {
            var customers = await _userService.GetCustomerListAsync();
            return Ok(customers);
        }

        [HttpPut("customers/{userId}/lock")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> LockOrUnlockCustomer(string userId, [FromQuery] bool lockAccount)
        {
            var (success, message) = await _userService.LockOrUnlockCustomerAsync(userId, lockAccount);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }


        [HttpGet("admin-id")]
        [Authorize]
        public async Task<IActionResult> GetAdminId()
        {
            var id = await _userService.GetAdminIdAsync();
            return id != null ? Ok(new { Id = id }) : NotFound(new { Message = "Không có admin nào" });
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) return NotFound(new { Message = "Không tìm thấy người dùng" });

            return Ok(new
            {
                user.FullName,
                user.Email,
                user.CreatedAt,
                user.PhoneNumber,
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var (success, message) = await _userService.UpdateCurrentUserProfileAsync(model);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var success = await _userService.ChangePasswordAsync(user, model);

            return success
                ? Ok(new { message = "Đổi mật khẩu thành công" })
                : BadRequest(new { message = "Mật khẩu cũ không đúng" });
        }

    }
}
