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

                    var now = DateTime.UtcNow;

                    var payments = await db.Payments
                        .Where(p => p.Status == PaymentStatus.Processing)
                        .ToListAsync(stoppingToken);

                    if (payments.Count > 0)
                    {
                        var userIds = payments
                            .Select(p => p.UserId)
                            .Distinct()
                            .ToList();

                        var users = await db.Users
                            .Where(u => userIds.Contains(u.Id))
                            .ToDictionaryAsync(u => u.Id, stoppingToken);

                        foreach (var payment in payments)
                        {
                            var secondsPassed = (now - payment.CreatedAt).TotalSeconds;

                            if (secondsPassed > 30)
                            {
                                payment.Status = PaymentStatus.Approved;

                                if (users.TryGetValue(payment.UserId, out var user))
                                {
                                    user.Experience += 1;
                                }
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PaymentApprovalWorker error: {ex.Message}");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}