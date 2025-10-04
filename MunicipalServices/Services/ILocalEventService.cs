using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface ILocalEventService
    {
        Task<CustomLinkedList<LocalEvent>> GetAllEventsAsync();
        Task<CustomLinkedList<LocalEvent>> GetUpcomingEventsAsync();
        Task<LocalEvent?> GetEventByIdAsync(int id);
        Task<ServiceResult> CreateEventAsync(LocalEvent eventModel);
        Task<ServiceResult> UpdateEventAsync(LocalEvent eventModel);
        Task<ServiceResult> DeleteEventAsync(int id);
        Task<EventSearchViewModel> SearchEventsAsync(EventSearchViewModel searchModel, string? sessionId = null, int? userId = null);
        Task<CustomLinkedList<LocalEvent>> GetRecommendedEventsAsync(string? sessionId = null, int? userId = null);
        Task<CustomLinkedList<string>> GetCategoriesAsync();
        Task<CustomDictionary<string, int>> GetEventStatisticsAsync();
        Task RecordSearchAsync(string searchTerm, string category, string? sessionId = null, int? userId = null);
        Task IncrementViewCountAsync(int eventId);
    }
}