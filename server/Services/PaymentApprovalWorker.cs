using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services
{
    public class PaymentApprovalWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentApprovalWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var payments = await db.Payments
                        .Where(p => p.Status == PaymentStatus.Processing &&
                                    p.CreatedAt <= DateTime.UtcNow.AddSeconds(-30))
                        .ToListAsync(stoppingToken);

                    var userIds = payments.Select(p => p.UserId).Distinct().ToList();

                    var users = await db.Users
                        .Where(u => userIds.Contains(u.Id))
                        .ToDictionaryAsync(u => u.Id, stoppingToken);

                    foreach (var p in payments)
                    {
                        p.Status = PaymentStatus.Approved;

                        if (users.TryGetValue(p.UserId, out var user))
                            user.Experience += 1;
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PaymentApprovalWorker error: {ex.Message}");
                    await Task.Delay(5000, stoppingToken); 
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
