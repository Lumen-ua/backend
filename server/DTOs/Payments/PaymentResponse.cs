using Server.Models;

namespace Server.DTOs.Payments
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string Service { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
