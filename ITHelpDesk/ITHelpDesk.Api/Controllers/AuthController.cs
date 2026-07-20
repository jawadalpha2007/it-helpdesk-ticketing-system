using ITHelpDesk.Application.DTOs;
using ITHelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var success = await _authService.RegisterAsync(request);

            if (!success)
                return BadRequest(new { message = "Email already exists." });

            return Ok(new { message = "User registered successfully." });
        }

    }
}