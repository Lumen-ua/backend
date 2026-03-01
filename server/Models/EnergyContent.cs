namespace Server.Models
{
    public class EnergyContent
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CompletedTopicsJson { get; set; } = "[]";

        public User User { get; set; } = null!;
    }
}