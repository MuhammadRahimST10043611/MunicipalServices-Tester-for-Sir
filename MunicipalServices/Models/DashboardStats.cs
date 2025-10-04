namespace MunicipalServices.Models
{
    public class DashboardStats
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int ResolvedReports { get; set; }
        public int HighPriorityReports { get; set; }
        public CustomDictionary<string, int> ReportsByCategory { get; set; } = new CustomDictionary<string, int>();
    }
}