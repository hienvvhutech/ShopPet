using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Addresses;
using Shopping_Pet.Services;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressesController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var addresses = await _addressService.GetAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("default")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetDefault()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var address = await _addressService.GetDefaultAddressAsync(userId);
            return Ok(address);
        }

        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> Create([FromBody] CreateAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });
            var (success, message) =
                await _addressService.CreateAsync(userId, model);
            if (success)
                return Ok(new { Message = message });
            return BadRequest(new { Message = message });
        }

        [HttpPut("{id}/set-default")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var (success, message) =
                await _addressService.SetDefaultAsync(userId, id);

            if (success)
                return Ok(new { Message = message });

            return BadRequest(new { Message = message });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var (success, message) =
                await _addressService.DeleteAsync(userId, id);

            if (success)
                return Ok(new { Message = message });

            return BadRequest(new { Message = message });
        }
    }
}
