using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.User;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;
        private readonly UserService _users;

        public UsersController(AppDbContext db, AuthService auth, UserService users)
        {
            _db = db;
            _auth = auth;
            _users = users;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = _auth.GetUserIdFromClaims(User);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Unauthorized(new { message = "User not found" });

            return Ok(new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest req)
        {
            var userId = _auth.GetUserIdFromClaims(User);
            var updated = await _users.UpdateAsync(userId, req);
            return Ok(updated);
        }

        [HttpPost("me/avatar")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ApiException(400, "File is required");

            if (!file.ContentType.StartsWith("image/"))
                throw new ApiException(400, "Only image files are allowed");

            var userId = _auth.GetUserIdFromClaims(User);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ApiException(404, "User not found");

            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDir = Path.Combine(wwwroot, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

            var safeName = $"u_{userId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
            var fullPath = Path.Combine(uploadsDir, safeName);

            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            user.AvatarUrl = $"/uploads/{safeName}";
            await _db.SaveChangesAsync();

            return Ok(new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl
            });
        }
    }
}