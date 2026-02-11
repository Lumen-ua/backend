using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Auth;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "pong" });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            var res = await _authService.RegisterAsync(req);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var res = await _authService.LoginAsync(req);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> Current()
        {
            var userId = _authService.GetUserIdFromClaims(User);

            var user = await _authService.TryGetByIdAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                name = user.Name
            });
        }
    }
}