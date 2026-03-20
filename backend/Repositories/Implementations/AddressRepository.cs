using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
        }

        public async Task<Address?> GetDefaultAsync(string userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Address address)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task ResetDefaultAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
