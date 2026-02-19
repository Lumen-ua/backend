using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;
        private readonly AuthService _auth;

        public DashboardController(DashboardService service, AuthService auth)
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var result = await _service.GetAsync(userId);
            return Ok(result);
        }
    }
}
