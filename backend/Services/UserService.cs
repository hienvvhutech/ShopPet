using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shopping_Pet.DTOs.Auth;
using Shopping_Pet.DTOs.Users;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterModel model)
        {
            if (await _userRepository.EmailExistsAsync(model.Email))
                return (false, "Email đã được sử dụng");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return (true, "Đăng ký thành công");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<(bool Success, string Message, ApplicationUser User)> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return (false, "Email hoặc mật khẩu không đúng", null);
            }

            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                return (false, "Tài khoản của bạn đã bị khoá.", null);
            }

            return (true, "Đăng nhập thành công", user);
        }

        public async Task<(bool Success, string Message)> CreateStaffAsync(CreateStaffModel model)
        {
            if (await _userRepository.EmailExistsAsync(model.Email))
                return (false, "Email đã được sử dụng");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                return (true, "Tạo nhân viên thành công");
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<(bool Success, string Message)> UpdateStaffAsync(string userId, UpdateStaffModel model)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return (false, "Nhân viên không tồn tại");

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                if (await _userRepository.EmailExistsAsync(model.Email))
                    return (false, "Email đã được sử dụng");

                user.Email = model.Email;
                user.UserName = model.Email;
            }

            user.FullName = model.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return (true, "Cập nhật nhân viên thành công");
        }

        public async Task<List<ApplicationUser>> GetStaffListAsync()
        {
            return await _userRepository.GetUsersByRoleAsync("Employee");
        }

        public async Task<(bool Success, string Message)> DeleteStaffAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Nhân viên không tồn tại");

            var isEmployee = await _userRepository.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
                return (false, "Người dùng không phải là nhân viên");

            await _userRepository.DeleteAsync(user);
            return (true, "Xoá nhân viên thành công");
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<List<ApplicationUser>> GetCustomerListAsync()
        {
            return await _userRepository.GetUsersByRoleAsync("Customer");
        }

        public async Task<(bool Success, string Message)> LockOrUnlockCustomerAsync(string userId, bool lockAccount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Người dùng không tồn tại");

            user.LockoutEnabled = lockAccount;
            user.LockoutEnd = lockAccount ? DateTime.UtcNow.AddYears(100) : null;

            await _userRepository.UpdateAsync(user);
            return (true, lockAccount ? "Đã khóa tài khoản" : "Đã mở khóa tài khoản");
        }

        public async Task<string?> GetAdminIdAsync()
        {
            return await _userRepository.GetFirstAdminIdAsync();
        }


        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<(bool Success, string Message)> UpdateCurrentUserProfileAsync(UpdateProfileModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return (false, "Không tìm thấy người dùng");

            user.FullName = model.FullName;
            user.UpdatedAt = DateTime.UtcNow;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? (true, "Cập nhật thành công")
                : (false, "Cập nhật thất bại");
        }

        public async Task<bool> ChangePasswordAsync(
        ApplicationUser user,
        ChangePasswordRequest model)
        {
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            return result.Succeeded;
        }

    }
}
