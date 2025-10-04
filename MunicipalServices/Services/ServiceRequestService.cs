using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private static readonly CustomLinkedList<ServiceRequest> _serviceRequests = new CustomLinkedList<ServiceRequest>();
        private static int _nextId = 3; // Start from 3 since i created 2 fake reports
        private static bool _initialized = false;

        public ServiceRequestService()
        {
            if (!_initialized)
            {
                InitializeWithSampleData();
                _initialized = true;
            }
        }

        public void AddServiceRequest(ServiceRequest request)
        {
            request.Id = _nextId++;
            request.DateReported = DateTime.Now;
            request.Priority = DeterminePriorityByCategory(request.Category);
            request.Status = "Submitted";

            _serviceRequests.Add(request);
        }

        // Get reports for a specific user
        public CustomLinkedList<ServiceRequest> GetUserServiceRequests(int userId)
        {
            var userReports = new CustomLinkedList<ServiceRequest>();

            // Always include the 2 sample reports (ID 1 and 2)
            foreach (var report in _serviceRequests)
            {
                if (report.Id <= 2 || report.UserId == userId)
                {
                    userReports.Add(report);
                }
            }

            return userReports;
        }

        // Get user report count (excluding sample reports)
        public int GetUserReportCount(int userId)
        {
            int count = 0;
            foreach (var report in _serviceRequests)
            {
                if (report.Id > 2 && report.UserId == userId)
                {
                    count++;
                }
            }
            return count;
        }

        public CustomLinkedList<ServiceRequest> GetAllServiceRequestsCustom()
        {
            return _serviceRequests;
        }

        public ServiceRequest? GetServiceRequestById(int id)
        {
            return _serviceRequests.GetById(id);
        }

        public DashboardStats GetDashboardStats()
        {
            var stats = new DashboardStats
            {
                TotalReports = _serviceRequests.Count,
                PendingReports = CountReportsByStatus("Submitted", "In Progress"),
                ResolvedReports = CountReportsByStatus("Resolved"),
                HighPriorityReports = CountReportsByMinimumPriority(3)
            };

            stats.ReportsByCategory = GetReportCountsByCategory();
            return stats;
        }

        public CustomLinkedList<ServiceRequest> GetRecentReportsCustom(int count = 5)
        {
            return GetSortedReportsByDate().Take(count);
        }

        // CHANGED: Return custom collections instead of List<T>
        public CustomLinkedList<ServiceRequest> GetAllServiceRequests()
        {
            return _serviceRequests;
        }

        public CustomLinkedList<ServiceRequest> GetRecentReports(int count = 5)
        {
            return GetRecentReportsCustom(count);
        }

        #region Private Helper Methods

        private void InitializeWithSampleData()
        {
            CreateSampleWaterIssueReport();
            CreateSampleRoadIssueReport();
        }

        private void CreateSampleWaterIssueReport()
        {
            var waterReport = new ServiceRequest
            {
                Id = 1,
                UserId = 0, // Sample report, no specific user
                Location = "Corner of Mandela Street and Church Avenue, Johannesburg CBD",
                Category = "Water & Sewer",
                Description = "Major water pipe burst causing flooding on the sidewalk and road. Water is gushing out continuously, making it dangerous for pedestrians and vehicles. The water pressure in nearby buildings has also been affected. This started early this morning around 6 AM and has been getting worse. Emergency repair needed urgently.",
                DateReported = DateTime.Now.AddDays(-2).AddHours(-3),
                Status = "In Progress",
                Priority = 4
            };

            waterReport.AttachedFiles.Add("/uploads/fake-water-burst-1.jpg");
            waterReport.AttachedFiles.Add("/uploads/fake-water-burst-2.jpg");
            _serviceRequests.Add(waterReport);
        }

        private void CreateSampleRoadIssueReport()
        {
            var roadReport = new ServiceRequest
            {
                Id = 2,
                UserId = 0, // Sample report, no specific user
                Location = "Sandton Drive near Nelson Mandela Square, Sandton",
                Category = "Roads & Transportation",
                Description = "Large pothole has developed in the main traffic lane causing vehicles to swerve dangerously. The pothole is approximately 2 meters wide and quite deep, filled with water after recent rains. Multiple vehicles have already suffered tire damage. Traffic flow is significantly impacted during peak hours. Please repair as soon as possible to prevent accidents and further vehicle damage.",
                DateReported = DateTime.Now.AddDays(-1).AddHours(-5),
                Status = "Submitted",
                Priority = 3
            };

            roadReport.AttachedFiles.Add("/uploads/fake-pothole-damage.jpg");
            _serviceRequests.Add(roadReport);
        }

        private int DeterminePriorityByCategory(string category)
        {
            return category switch
            {
                "Water & Sewer" => 4,
                "Electricity" => 4,
                "Public Safety" => 5,
                "Roads & Transportation" => 3,
                "Waste Management" => 2,
                "Parks & Recreation" => 1,
                "Building & Planning" => 2,
                _ => 1
            };
        }

        private int CountReportsByStatus(params string[] statuses)
        {
            int count = 0;
            foreach (var request in _serviceRequests)
            {
                foreach (var status in statuses)
                {
                    if (string.Equals(request.Status, status, StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        private int CountReportsByMinimumPriority(int minPriority)
        {
            int count = 0;
            foreach (var request in _serviceRequests)
            {
                if (request.Priority >= minPriority)
                    count++;
            }
            return count;
        }

        private CustomDictionary<string, int> GetReportCountsByCategory()
        {
            var categoryStats = new CustomDictionary<string, int>();

            foreach (var request in _serviceRequests)
            {
                if (categoryStats.ContainsKey(request.Category))
                {
                    int currentCount = categoryStats[request.Category];
                    categoryStats[request.Category] = currentCount + 1;
                }
                else
                {
                    categoryStats.Add(request.Category, 1);
                }
            }

            return categoryStats;
        }

        private CustomLinkedList<ServiceRequest> GetSortedReportsByDate()
        {
            // Convert to array for sorting, then back to custom list
            var tempArray = new ServiceRequest[_serviceRequests.Count];
            _serviceRequests.CopyTo(tempArray, 0);

            // Sort by date (newest first)
            Array.Sort(tempArray, (x, y) => y.DateReported.CompareTo(x.DateReported));

            var sortedReports = new CustomLinkedList<ServiceRequest>();
            foreach (var report in tempArray)
            {
                sortedReports.Add(report);
            }

            return sortedReports;
        }

        #endregion
    }
}