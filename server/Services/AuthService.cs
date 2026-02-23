using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Auth;
using Server.Helpers;
using Server.Models;

namespace Server.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher;
        private readonly JwtTokenService _jwt;

        public AuthService(AppDbContext db, JwtTokenService jwt)
        {
            _db = db;
            _jwt = jwt;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Password) ||
                string.IsNullOrWhiteSpace(req.Name))
                throw new ApiException(400, "Email, name and password are required");

            string email = req.Email.Trim().ToLowerInvariant();

            bool exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
            if (exists) throw new ApiException(409, "User already exists");

            var user = new User
            {
                Email = email,
                Name = req.Name.Trim(),
                AvatarUrl = null
            };

            user.PasswordHash = _hasher.HashPassword(user, req.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            string token = _jwt.CreateToken(user);
            return new AuthResponse { Id = user.Id, Email = user.Email, Name = user.Name, AvatarUrl = user.AvatarUrl, Token = token };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                throw new ApiException(400, "Email and password are required");

            string email = req.Email.Trim().ToLowerInvariant();

            User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null) throw new ApiException(401, "Invalid email or password");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new ApiException(401, "Invalid email or password");

            string token = _jwt.CreateToken(user);
            return new AuthResponse { Id = user.Id, Email = user.Email, Name = user.Name, AvatarUrl = user.AvatarUrl, Token = token };
        }

        public async Task<User> GetByIdAsync(int id)
        {
            User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new ApiException(404, "User not found");
            return user;
        }

        public async Task<User?> TryGetByIdAsync(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public int GetUserIdFromClaims(ClaimsPrincipal principal)
        {
            string? sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!string.IsNullOrWhiteSpace(sub) && int.TryParse(sub, out int idFromSub))
                return idFromSub;

            string? nameId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(nameId) && int.TryParse(nameId, out int idFromNameId))
                return idFromNameId;

            throw new ApiException(401, "Not authorized");
        }
    }
}