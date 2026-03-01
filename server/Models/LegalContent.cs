namespace Server.Models
{
    public class LegalContent
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CompletedSimulationsJson { get; set; } = "[]";

        public User User { get; set; } = null!;
    }
}
