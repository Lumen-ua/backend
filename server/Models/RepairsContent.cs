namespace Server.Models
{
    public class RepairsContent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompletedTopicsJson { get; set; } = "[]";
        public string MaintenanceStateJson { get; set; } = "{}";
        public string EmergencyFormJson { get; set; } = "{}";

        public User User { get; set; } = null!;
    }
}
