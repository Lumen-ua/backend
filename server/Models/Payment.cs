using System.ComponentModel.DataAnnotations;
using System;
namespace Server.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public string Service { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Processing;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public enum PaymentStatus
    {
        Processing,
        Approved,
        Refunded,
        Redirected
    }
}
