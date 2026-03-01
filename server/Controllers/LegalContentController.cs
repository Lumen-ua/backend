
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Legal;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LegalContentController : ControllerBase
    {
        private readonly LegalContentService _legalService;
        private readonly AuthService _authService;

        public LegalContentController(LegalContentService legalService, AuthService authService)
        {
            _legalService = legalService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<LegalContentDto>> Get()
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var dto = await _legalService.GetAsync(userId);
            return Ok(dto);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<LegalContentDto>> Complete([FromBody] CompleteSimulationDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var updated = await _legalService.CompleteSimulationAsync(userId, dto.SimulationKey);
            return Ok(updated);
        }
    }
}

