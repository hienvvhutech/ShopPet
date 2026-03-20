using Shopping_Pet.Models;

namespace Shopping_Pet.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetByUserIdAsync(string userId);
        Task<Address?> GetDefaultAsync(string userId);
        Task<Address?> GetByIdAsync(int id);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(Address address);
        Task ResetDefaultAsync(string userId);
    }
}
