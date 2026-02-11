namespace Server.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = "Lumen";
        public string Audience { get; set; } = "LumenClient";
        public int ExpiresMinutes { get; set; } = 120;
    }
}
