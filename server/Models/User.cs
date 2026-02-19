namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 1000;
        
        public int Experience { get; set; } = 0;

        public List<Payment> Payments { get; set; } = new();
        public List<Template> Templates { get; set; } = new();
    }
}
