

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Energy;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnergyContentController : ControllerBase
    {
        private readonly EnergyContentService _energyService;
        private readonly AuthService _authService;

        public EnergyContentController(EnergyContentService energyService, AuthService authService)
        {
            _energyService = energyService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<EnergyContentDto>> Get()
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var dto = await _energyService.GetAsync(userId);
            return Ok(dto);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<EnergyContentDto>> Complete([FromBody] CompleteTopicDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var updated = await _energyService.CompleteTopicAsync(userId, dto.TopicKey);
            return Ok(updated);
        }
    }
}










