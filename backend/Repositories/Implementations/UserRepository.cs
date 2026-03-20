using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string role)
        {
            return (await _userManager.GetUsersInRoleAsync(role)).ToList();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }
        public async Task<string?> GetFirstAdminIdAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.FirstOrDefault()?.Id;
        }
    }
}
