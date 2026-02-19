namespace Server.DTOs.Payments
{
    public class CreatePaymentRequest
    {
        public string Service { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        
    }
}
