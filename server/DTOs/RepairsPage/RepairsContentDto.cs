namespace Server.DTOs.RepairsPage
{
    public class RepairsContentDto
    {
        public string CompletedTopicsJson { get; set; } = "[]";
        public string MaintenanceStateJson { get; set; } = "{}";
        public string EmergencyFormJson { get; set; } = "{}";
    }

    public class UpdateProgressDto { public string CompletedTopicsJson { get; set; } = "[]"; }
    public class UpdateMaintenanceDto { public string MaintenanceStateJson { get; set; } = "{}"; }
    public class UpdateEmergencyDto { public string EmergencyFormJson { get; set; } = "{}"; }
}