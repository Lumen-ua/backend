using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Budget;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetContentController : ControllerBase
    {
        private readonly BudgetContentService _budgetService;
        private readonly AuthService _authService;

        public BudgetContentController(BudgetContentService budgetService, AuthService authService)
        {
            _budgetService = budgetService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<BudgetContentDto>> Get()
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var dto = await _budgetService.GetAsync(userId);
            return Ok(dto);
        }

        [HttpPost("complete")]
        public async Task<ActionResult<BudgetContentDto>> Complete([FromBody] CompleteSimulationDto dto)
        {
            var userId = _authService.GetUserIdFromClaims(User);
            var updated = await _budgetService.CompleteSimulationAsync(userId, dto.SimulationKey);
            return Ok(updated);
        }
    }
}