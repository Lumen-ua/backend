using Server.Helpers;
namespace Server.DTOs.Dashboard
{
    public class DashboardResponse
    {
        public decimal Balance { get; set; }
        public int ApprovedCount { get; set; }
        public string Level { get; set; } = string.Empty;

        public List<ServiceStat> Stats { get; set; } = new();
    }

    public class ServiceStat
    {
        public string Service { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
