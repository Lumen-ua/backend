namespace Server.Models
{
    public class Template
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
