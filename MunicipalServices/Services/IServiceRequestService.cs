using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface IServiceRequestService
    {
        void AddServiceRequest(ServiceRequest request);
        CustomLinkedList<ServiceRequest> GetAllServiceRequestsCustom();
        ServiceRequest? GetServiceRequestById(int id);
        DashboardStats GetDashboardStats();
        CustomLinkedList<ServiceRequest> GetRecentReportsCustom(int count = 5);

        // CHANGED: Return custom collections instead of List<T>
        CustomLinkedList<ServiceRequest> GetAllServiceRequests();
        CustomLinkedList<ServiceRequest> GetRecentReports(int count = 5);

        // NEW: User-specific methods
        CustomLinkedList<ServiceRequest> GetUserServiceRequests(int userId);
        int GetUserReportCount(int userId);
    }
}