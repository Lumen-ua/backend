using Server.Services;
using Microsoft.AspNetCore.Authorization;
using Server.DTOs.RepairsPage;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RepairsContentController : ControllerBase
    {
        private readonly RepairsContentService _contentService;
        private readonly AuthService _authService;

        public RepairsContentController(RepairsContentService contentService, AuthService authService)
        {
            _contentService = contentService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<RepairsContentDto>> Get()
        {
            try
            {
                var userId = _authService.GetUserIdFromClaims(User);
                var result = await _contentService.GetAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); 
                return StatusCode(500, "Server error: " + ex.Message);
            }
        }

        [HttpPost("progress")]
        public async Task<IActionResult> SaveProgress([FromBody] UpdateProgressDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            await _contentService.UpdateProgressAsync(userId, dto);
            return Ok();
        }

        [HttpPost("maintenance")]
        public async Task<IActionResult> SaveMaintenance([FromBody] UpdateMaintenanceDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            await _contentService.UpdateMaintenanceAsync(userId, dto);
            return Ok();
        }

        [HttpPost("emergency")]
        public async Task<IActionResult> SaveEmergency([FromBody] UpdateEmergencyDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            await _contentService.UpdateEmergencyAsync(userId, dto);
            return Ok();
        }
    }
}