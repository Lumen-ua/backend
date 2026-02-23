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

            if (payment == null)
                throw new ApiException(404, "Payment not found");

            var secondsPassed = (DateTime.UtcNow - payment.CreatedAt).TotalSeconds;

            if (secondsPassed > 30)
                throw new ApiException(400, "Refund window expired");

            if (payment.Status != PaymentStatus.Processing)
                throw new ApiException(400, "Cannot refund");

            var user = await _db.Users.FirstAsync(u => u.Id == userId);

            user.Balance += payment.Amount;
            payment.Status = PaymentStatus.Refunded;

            await _db.SaveChangesAsync();
        }
        public async Task<List<TemplateResponse>> GetTemplatesAsync(int userId)
        {
            return await _db.Templates
                .Where(t => t.UserId == userId)
                .Select(t => new TemplateResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    Service = t.Service,
                    Type = t.Type,
                    Value = t.Value
                })
                .ToListAsync();
        }

        public async Task<TemplateResponse> CreateTemplateAsync(int userId, CreateTemplateRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                throw new ApiException(400, "Template name required");

            var template = new Template
            {
                Name = req.Name,
                Service = req.Service,
                Type = req.Type,
                Value = req.Value,
                UserId = userId
            };

            _db.Templates.Add(template);
            await _db.SaveChangesAsync();

            return new TemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Service = template.Service,
                Type = template.Type,
                Value = template.Value
            };
        }
        
        public async Task DeleteTemplateAsync(int userId, int id)
        {
            var template = await _db.Templates
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (template == null)
                throw new ApiException(404, "Template not found");

            _db.Templates.Remove(template);
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
