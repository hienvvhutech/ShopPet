using Shopping_Pet.DTOs.Addresses;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class AddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<List<Address>> GetAddressesAsync(string userId)
        {
            return await _addressRepository.GetByUserIdAsync(userId);
        }

        public async Task<Address?> GetDefaultAddressAsync(string userId)
        {
            return await _addressRepository.GetDefaultAsync(userId);
        }

        public async Task<(bool Success, string Message)> CreateAsync(
            string userId,
            CreateAddressModel model)
        {
            if (model.IsDefault)
            {
                await _addressRepository.ResetDefaultAsync(userId);
            }

            var address = new Address
            {
                UserId = userId,
                Name = model.Name,
                Phone = model.Phone,
                AddressLine = model.AddressLine,
                IsDefault = model.IsDefault
            };

            await _addressRepository.AddAsync(address);
            return (true, "Thêm địa chỉ thành công");
        }

        public async Task<(bool Success, string Message)> SetDefaultAsync(
            string userId,
            int addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null || address.UserId != userId)
            {
                return (false, "Không tìm thấy địa chỉ");
            }

            await _addressRepository.ResetDefaultAsync(userId);
            address.IsDefault = true;
            await _addressRepository.UpdateAsync(address);

            return (true, "Đã đặt địa chỉ mặc định");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(
            string userId,
            int addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null || address.UserId != userId)
            {
                return (false, "Không tìm thấy địa chỉ");
            }

            await _addressRepository.DeleteAsync(address);
            return (true, "Xoá địa chỉ thành công");
        }
    }
}
