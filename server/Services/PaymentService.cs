using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs.Payments;
using Server.Helpers;
using Server.Models;

namespace Server.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _db;

        public PaymentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PaymentResponse> CreateAsync(int userId, CreatePaymentRequest req)
        {
            if (req.Amount <= 0)
                throw new ApiException(400, "Invalid amount");

            if (string.IsNullOrWhiteSpace(req.Service))
                throw new ApiException(400, "Service is required");

            if (string.IsNullOrWhiteSpace(req.Identifier))
                throw new ApiException(400, "Identifier is required");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ApiException(404, "User not found");

            if (user.Balance < req.Amount)
                throw new ApiException(400, "Not enough funds");

            var payment = new Payment
            {
                Service = req.Service,
                Identifier = req.Identifier,
                Amount = req.Amount,
                UserId = userId
            };

            user.Balance -= req.Amount;

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return Map(payment);
        }

        public async Task RefundAsync(int userId, int paymentId)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

            if (payment == null) throw new ApiException(404, "Payment not found");

            if (payment.Status != PaymentStatus.Processing)
                throw new ApiException(400, "Cannot refund");

            if (payment.CreatedAt <= DateTime.UtcNow.AddSeconds(-30))
                throw new ApiException(400, "Refund window expired");

            var user = await _db.Users.FirstAsync(u => u.Id == userId);

            user.Balance += payment.Amount;
            payment.Status = PaymentStatus.Refunded;

            await _db.SaveChangesAsync();
        }

        public async Task<List<PaymentResponse>> GetHistoryAsync(int userId)
        {
            var payments = await _db.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return payments.Select(Map).ToList();
        }

        private PaymentResponse Map(Payment p) => new()
        {
            Id = p.Id,
            Service = p.Service,
            Identifier = p.Identifier,
            Amount = p.Amount,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        };
    }
}
