using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Legal;
using Server.Models;

namespace Server.Services
{
    public class LegalContentService
    {
        private readonly AppDbContext _context;

        public LegalContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LegalContentDto> GetAsync(int userId)
        {
            var content = await _context.LegalContent.FirstOrDefaultAsync(x => x.UserId == userId);
            if (content == null)
            {
                return new LegalContentDto { CompletedSimulationsJson = "[]" };
            }

            return new LegalContentDto
            {
                CompletedSimulationsJson = content.CompletedSimulationsJson ?? "[]"
            };
        }

        public async Task<LegalContentDto> CompleteSimulationAsync(int userId, string simulationKey)
        {
            if (string.IsNullOrWhiteSpace(simulationKey))
                throw new ArgumentException("SimulationKey is required");

            var content = await GetOrCreateAsync(userId);

            List<string> keys;
            try
            {
                keys = JsonSerializer.Deserialize<List<string>>(content.CompletedSimulationsJson) ?? new List<string>();
            }
            catch
            {
                keys = new List<string>();
            }

            if (!keys.Contains(simulationKey))
            {
                keys.Add(simulationKey);
                content.CompletedSimulationsJson = JsonSerializer.Serialize(keys);
                await _context.SaveChangesAsync();
            }

            return new LegalContentDto
            {
                CompletedSimulationsJson = content.CompletedSimulationsJson
            };
        }

        private async Task<LegalContent> GetOrCreateAsync(int userId)
        {
            var content = await _context.LegalContent.FirstOrDefaultAsync(x => x.UserId == userId);

            if (content == null)
            {
                content = new LegalContent
                {
                    UserId = userId,
                    CompletedSimulationsJson = "[]"
                };

                await _context.LegalContent.AddAsync(content);
                await _context.SaveChangesAsync();
            }

            return content;
        }
    }
}
