using Microsoft.AspNetCore.Mvc;
using Shopping_Pet.DTOs.Auth;
using Shopping_Pet.Services;
using System.Threading.Tasks;

namespace Shopping_Pet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public AuthController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage) });
            }

            var (success, message) = await _userService.RegisterAsync(model);
            return success ? Ok(new { Message = message }) : BadRequest(new { Message = message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage) });
            }

            var (success, message, user) = await _userService.LoginAsync(model);
            if (!success)
            {
                return BadRequest(new { Message = message });
            }

            var token = await _authService.GenerateJwtTokenAsync(user);
            var roles = await _userService.GetUserRolesAsync(user);
            return Ok(new { Token = token, Email = user.Email, Roles = roles, Id = user.Id });
        }

    }
}