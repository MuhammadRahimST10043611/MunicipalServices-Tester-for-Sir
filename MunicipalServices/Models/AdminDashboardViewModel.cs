namespace MunicipalServices.Models
{
    public class AdminDashboardViewModel
    {
        public CustomDictionary<string, int> EventStatistics { get; set; } = new CustomDictionary<string, int>();
        public DashboardStats ServiceRequestStats { get; set; } = new DashboardStats();
        public CustomLinkedList<LocalEvent> RecentEvents { get; set; } = new CustomLinkedList<LocalEvent>();
        public CustomLinkedList<ServiceRequest> RecentServiceRequests { get; set; } = new CustomLinkedList<ServiceRequest>();
        public int TotalUsers { get; set; }

        // Additional admin statistics
        public int PendingEventApprovals { get; set; }
        public int TodaysEvents { get; set; }
        public int ThisWeekEvents { get; set; }
        public int ActiveUsers { get; set; }
        public CustomLinkedList<string> PopularCategories { get; set; } = new CustomLinkedList<string>();
        public CustomDictionary<string, int> MonthlyStats { get; set; } = new CustomDictionary<string, int>();

        // Performance metrics
        public double AverageResponseTime { get; set; }
        public int ResolvedToday { get; set; }
        public double UserSatisfactionRate { get; set; }

        // System status
        public bool DatabaseStatus { get; set; } = true;
        public bool SystemHealthy { get; set; } = true;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Helper methods
        public int GetTotalActiveEvents()
        {
            return RecentEvents.Count;
        }

        public int GetHighPriorityServiceRequests()
        {
            int count = 0;
            foreach (var request in RecentServiceRequests)
            {
                if (request.Priority >= 4)
                    count++;
            }
            return count;
        }

        public CustomLinkedList<LocalEvent> GetUpcomingEvents()
        {
            var upcoming = new CustomLinkedList<LocalEvent>();
            foreach (var eventItem in RecentEvents)
            {
                if (eventItem.EventDate >= DateTime.Now)
                {
                    upcoming.Add(eventItem);
                }
            }
            return upcoming;
        }

        public CustomLinkedList<ServiceRequest> GetPendingRequests()
        {
            var pending = new CustomLinkedList<ServiceRequest>();
            foreach (var request in RecentServiceRequests)
            {
                if (request.Status == "Submitted" || request.Status == "In Progress")
                {
                    pending.Add(request);
                }
            }
            return pending;
        }

        public string GetSystemStatusMessage()
        {
            if (!DatabaseStatus)
                return "Database connection issues detected";

            if (!SystemHealthy)
                return "System performance degraded";

            return "All systems operational";
        }

        public string GetSystemStatusColor()
        {
            if (!DatabaseStatus || !SystemHealthy)
                return "danger";

            return "success";
        }
    }
}