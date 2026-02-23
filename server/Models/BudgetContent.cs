namespace Server.Models
{
    public class BudgetContent
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        // JSON array of keys: ["budget_read_bill", "budget_calculate_indicators", ...]
        public string CompletedSimulationsJson { get; set; } = "[]";

        public User User { get; set; } = null!;
    }
}