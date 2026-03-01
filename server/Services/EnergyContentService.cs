using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Energy;
using Server.Models;

namespace Server.Services
{
    public class EnergyContentService
    {
        private readonly AppDbContext _context;

        public EnergyContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EnergyContentDto> GetAsync(int userId)
        {
            var content = await _context.EnergyContent.FirstOrDefaultAsync(x => x.UserId == userId);
            if (content == null)
            {
                return new EnergyContentDto { CompletedTopicsJson = "[]" };
            }

            return new EnergyContentDto
            {
                CompletedTopicsJson = content.CompletedTopicsJson ?? "[]"
            };
        }

        public async Task<EnergyContentDto> CompleteTopicAsync(int userId, string topicKey)
        {
            if (string.IsNullOrWhiteSpace(topicKey))
                throw new ArgumentException("TopicKey is required");

            var content = await GetOrCreateAsync(userId);

            List<string> keys;
            try
            {
                keys = JsonSerializer.Deserialize<List<string>>(content.CompletedTopicsJson) ?? new List<string>();
            }
            catch
            {
                keys = new List<string>();
            }

            if (!keys.Contains(topicKey))
            {
                keys.Add(topicKey);
                content.CompletedTopicsJson = JsonSerializer.Serialize(keys);
                await _context.SaveChangesAsync();
            }

            return new EnergyContentDto
            {
                CompletedTopicsJson = content.CompletedTopicsJson
            };
        }

        private async Task<EnergyContent> GetOrCreateAsync(int userId)
        {
            var content = await _context.EnergyContent.FirstOrDefaultAsync(x => x.UserId == userId);

            if (content == null)
            {
                content = new EnergyContent
                {
                    UserId = userId,
                    CompletedTopicsJson = "[]"
                };

                await _context.EnergyContent.AddAsync(content);
                await _context.SaveChangesAsync();
            }

            return content;
        }
    }
}
