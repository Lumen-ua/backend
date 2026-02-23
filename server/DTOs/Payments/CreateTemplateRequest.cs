namespace Server.DTOs.Payments
{
    public class CreateTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}