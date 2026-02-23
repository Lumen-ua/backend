using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Payments;
using Server.Services;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _service;
        private readonly AuthService _auth;

        public PaymentsController(PaymentService service, AuthService auth)
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var data = await _service.GetHistoryAsync(userId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] CreatePaymentRequest req)
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var payment = await _service.CreateAsync(userId, req);
            return Ok(payment);
        }
 
        [HttpPost("{id}/refund")]
        public async Task<IActionResult> Refund(int id)
        {
            var userId = _auth.GetUserIdFromClaims(User);
            await _service.RefundAsync(userId, id);
            return Ok(new { success = true });
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var templates = await _service.GetTemplatesAsync(userId);
            return Ok(templates);
        }

        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest req)
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var template = await _service.CreateTemplateAsync(userId, req);
            return Ok(template);
        }

        [HttpDelete("templates/{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userId = _auth.GetUserIdFromClaims(User);
            await _service.DeleteTemplateAsync(userId, id);
            return Ok(new { success = true });
        }
    }
}