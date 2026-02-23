using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Budget;
using Server.Models;

namespace Server.Services
{
    public class BudgetContentService
    {
        private readonly AppDbContext _context;

        public BudgetContentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BudgetContentDto> GetAsync(int userId)
        {
            var content = await _context.BudgetContent.FirstOrDefaultAsync(x => x.UserId == userId);
            if (content == null)
            {
                return new BudgetContentDto { CompletedSimulationsJson = "[]" };
            }

            return new BudgetContentDto
            {
                CompletedSimulationsJson = content.CompletedSimulationsJson ?? "[]"
            };
        }

        public async Task<BudgetContentDto> CompleteSimulationAsync(int userId, string simulationKey)
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

            return new BudgetContentDto
            {
                CompletedSimulationsJson = content.CompletedSimulationsJson
            };
        }

        private async Task<BudgetContent> GetOrCreateAsync(int userId)
        {
            var content = await _context.BudgetContent.FirstOrDefaultAsync(x => x.UserId == userId);

            if (content == null)
            {
                content = new BudgetContent
                {
                    UserId = userId,
                    CompletedSimulationsJson = "[]"
                };

                await _context.BudgetContent.AddAsync(content);
                await _context.SaveChangesAsync();
            }

            return content;
        }
    }
}