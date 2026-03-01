using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.RepairsPage;
using Server.Models;

namespace Server.Services
{
    public class RepairsContentService
    {
        private readonly AppDbContext _context;

        public RepairsContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RepairsContentDto> GetAsync(int userId)
        {
            var content = await _context.RepairsContent
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (content == null)
                return new RepairsContentDto();

            return new RepairsContentDto
            {
                CompletedTopicsJson = content.CompletedTopicsJson,
                MaintenanceStateJson = content.MaintenanceStateJson,
                EmergencyFormJson = content.EmergencyFormJson,
                CompletedAchievementsJson = content.CompletedAchievementsJson
            };
        }

        public async Task UpdateProgressAsync(int userId, UpdateProgressDto dto)
        {
            var content = await GetOrCreateContentAsync(userId);
            content.CompletedTopicsJson = dto.CompletedTopicsJson;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMaintenanceAsync(int userId, UpdateMaintenanceDto dto)
        {
            var content = await GetOrCreateContentAsync(userId);
            content.MaintenanceStateJson = dto.MaintenanceStateJson;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmergencyAsync(int userId, UpdateEmergencyDto dto)
        {
            var content = await GetOrCreateContentAsync(userId);
            content.EmergencyFormJson = dto.EmergencyFormJson;
            await _context.SaveChangesAsync();
        }

        public async Task<RepairsContent> GetOrCreateContentAsync(int userId)
        {
            var content = await _context.RepairsContent
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (content == null)
            {
                content = new RepairsContent
                {
                    UserId = userId,
                    CompletedTopicsJson = "[]",
                    MaintenanceStateJson = "{}",
                    EmergencyFormJson = "{}",
                    CompletedAchievementsJson = "[]"
                };
                await _context.RepairsContent.AddAsync(content);
                await _context.SaveChangesAsync();
            }

            return content;
        }

        public async Task<RepairsContentDto> AddAchievementAsync(int userId, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Achievement key is required");

            var content = await GetOrCreateContentAsync(userId);

            List<string> keys;

            try
            {
                keys = JsonSerializer.Deserialize<List<string>>(content.CompletedAchievementsJson)
                    ?? new List<string>();
            }
            catch
            {
                keys = new List<string>();
            }

            if (!keys.Contains(key))
            {
                keys.Add(key);
                content.CompletedAchievementsJson = JsonSerializer.Serialize(keys);
                await _context.SaveChangesAsync();
            }

            return new RepairsContentDto
            {
                CompletedTopicsJson = content.CompletedTopicsJson,
                MaintenanceStateJson = content.MaintenanceStateJson,
                EmergencyFormJson = content.EmergencyFormJson,
                CompletedAchievementsJson = content.CompletedAchievementsJson
            };
        }
    }
}