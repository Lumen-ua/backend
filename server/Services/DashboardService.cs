using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Dashboard;
using Server.Helpers;
using Server.Models;

namespace Server.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _db;

        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardResponse> GetAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Payments)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new ApiException(404, "User not found");

            bool hasChanges = false;
            foreach (var payment in user.Payments)
            {
                if (payment.Status == PaymentStatus.Processing &&
                    (DateTime.UtcNow - payment.CreatedAt).TotalSeconds > 30)
                {
                    payment.Status = PaymentStatus.Approved;
                    user.Experience += 1; 
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await _db.SaveChangesAsync();
            }

            var approvedPaymentsCount = user.Payments
                .Count(p => p.Status == PaymentStatus.Approved);

            var stats = user.Payments
                .Where(p => p.Status == PaymentStatus.Approved)
                .GroupBy(p => p.Service)
                .Select(g => new ServiceStat
                {
                    Service = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            return new DashboardResponse
            {
                Balance = user.Balance,
                ApprovedCount = approvedPaymentsCount, 
                Level = LevelHelper.GetLevel(user.Experience), 
                Stats = stats
            };
        }
    }
}
