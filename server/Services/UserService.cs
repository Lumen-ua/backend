using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.User;
using Server.Helpers;
using Server.Models;

namespace Server.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher;

        public UserService(AppDbContext db)
        {
            _db = db;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<UserResponse> UpdateAsync(int userId, UpdateUserRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ApiException(404, "User not found");

            if (!string.IsNullOrWhiteSpace(req.Email))
                user.Email = req.Email.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(req.Name))
                user.Name = req.Name.Trim();

            if (!string.IsNullOrWhiteSpace(req.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(req.OldPassword))
                    throw new ApiException(400, "OldPassword is required to set a new password");

                var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.OldPassword);
                if (verify == PasswordVerificationResult.Failed)
                    throw new ApiException(401, "OldPassword is incorrect");

                user.PasswordHash = _hasher.HashPassword(user, req.NewPassword);
            }

            await _db.SaveChangesAsync();

            return new UserResponse { Id = user.Id, Email = user.Email, Name = user.Name, AvatarUrl = user.AvatarUrl};
        }
    }
}
