using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string role);
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<string?> GetFirstAdminIdAsync();
        Task<bool> EmailExistsAsync(string email);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
    }
}
