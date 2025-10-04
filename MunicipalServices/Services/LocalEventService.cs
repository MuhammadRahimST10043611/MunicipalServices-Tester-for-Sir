using Microsoft.EntityFrameworkCore;
using MunicipalServices.Data;
using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public class LocalEventService : ILocalEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomDictionary<string, CustomLinkedList<LocalEvent>> _eventCache;
        private readonly CustomPriorityQueue<LocalEvent> _priorityQueue;
        private readonly CustomSet<string> _categories;

        public LocalEventService(ApplicationDbContext context)
        {
            _context = context;
            _eventCache = new CustomDictionary<string, CustomLinkedList<LocalEvent>>();
            _priorityQueue = new CustomPriorityQueue<LocalEvent>();
            _categories = new CustomSet<string>();
            InitializeCache();
        }

        private async void InitializeCache()
        {
            try
            {
                var events = await _context.LocalEvents.Where(e => e.IsActive).ToListAsync();

                foreach (var eventItem in events)
                {
                    _categories.Add(eventItem.Category);
                    _priorityQueue.Enqueue(eventItem, eventItem.Priority);
                }
            }
            catch (Exception)
            {
                // Handle initialization error gracefully
            }
        }

        public async Task<CustomLinkedList<LocalEvent>> GetAllEventsAsync()
        {
            var cacheKey = "all_events";

            if (_eventCache.TryGetValue(cacheKey, out var cachedEvents))
            {
                return cachedEvents;
            }

            var events = await _context.LocalEvents
                .Where(e => e.IsActive)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            var customList = new CustomLinkedList<LocalEvent>();
            foreach (var eventItem in events)
            {
                customList.Add(eventItem);
                _categories.Add(eventItem.Category);
            }

            _eventCache.Add(cacheKey, customList);
            return customList;
        }

        public async Task<CustomLinkedList<LocalEvent>> GetUpcomingEventsAsync()
        {
            var cacheKey = "upcoming_events";

            if (_eventCache.TryGetValue(cacheKey, out var cachedEvents))
            {
                return cachedEvents;
            }

            var events = await _context.LocalEvents
                .Where(e => e.IsActive && e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            var customList = new CustomLinkedList<LocalEvent>();
            foreach (var eventItem in events)
            {
                customList.Add(eventItem);
            }

            _eventCache.Add(cacheKey, customList);
            return customList;
        }

        public async Task<LocalEvent?> GetEventByIdAsync(int id)
        {
            return await _context.LocalEvents
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<ServiceResult> CreateEventAsync(LocalEvent eventModel)
        {
            var result = new ServiceResult();

            try
            {
                eventModel.CreatedAt = DateTime.UtcNow;
                eventModel.IsActive = true;

                _context.LocalEvents.Add(eventModel);
                await _context.SaveChangesAsync();

                // Update cache and data structures
                _categories.Add(eventModel.Category);
                _priorityQueue.Enqueue(eventModel, eventModel.Priority);
                ClearEventCache();

                result.Success = true;
                result.Message = "Event created successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to create event: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateEventAsync(LocalEvent eventModel)
        {
            var result = new ServiceResult();

            try
            {
                var existingEvent = await _context.LocalEvents.FindAsync(eventModel.Id);
                if (existingEvent == null)
                {
                    result.Success = false;
                    result.Message = "Event not found.";
                    return result;
                }

                existingEvent.Title = eventModel.Title;
                existingEvent.Description = eventModel.Description;
                existingEvent.Category = eventModel.Category;
                existingEvent.EventDate = eventModel.EventDate;
                existingEvent.Location = eventModel.Location;
                existingEvent.Priority = eventModel.Priority;
                // ImagePath is transient and not saved to DB

                await _context.SaveChangesAsync();

                // Update cache
                _categories.Add(eventModel.Category);
                ClearEventCache();

                result.Success = true;
                result.Message = "Event updated successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to update event: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> DeleteEventAsync(int id)
        {
            var result = new ServiceResult();

            try
            {
                var eventItem = await _context.LocalEvents.FindAsync(id);
                if (eventItem == null)
                {
                    result.Success = false;
                    result.Message = "Event not found.";
                    return result;
                }

                eventItem.IsActive = false;
                await _context.SaveChangesAsync();

                ClearEventCache();

                result.Success = true;
                result.Message = "Event deleted successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to delete event: {ex.Message}";
            }

            return result;
        }

        public async Task<EventSearchViewModel> SearchEventsAsync(EventSearchViewModel searchModel, string? sessionId = null, int? userId = null)
        {
            var result = new EventSearchViewModel
            {
                SearchTerm = searchModel.SearchTerm,
                Category = searchModel.Category,
                StartDate = searchModel.StartDate,
                EndDate = searchModel.EndDate,
                SortBy = searchModel.SortBy
            };

            try
            {
                // Record search for recommendations
                if (!string.IsNullOrEmpty(searchModel.SearchTerm) || !string.IsNullOrEmpty(searchModel.Category))
                {
                    await RecordSearchAsync(searchModel.SearchTerm ?? "", searchModel.Category ?? "", sessionId, userId);
                }

                var query = _context.LocalEvents.Where(e => e.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(searchModel.SearchTerm))
                {
                    query = query.Where(e => e.Title.Contains(searchModel.SearchTerm) ||
                                           e.Description.Contains(searchModel.SearchTerm) ||
                                           e.Location.Contains(searchModel.SearchTerm));
                }

                if (!string.IsNullOrEmpty(searchModel.Category))
                {
                    query = query.Where(e => e.Category == searchModel.Category);
                }

                if (searchModel.StartDate.HasValue)
                {
                    query = query.Where(e => e.EventDate >= searchModel.StartDate.Value);
                }

                if (searchModel.EndDate.HasValue)
                {
                    query = query.Where(e => e.EventDate <= searchModel.EndDate.Value);
                }

                // Apply sorting using custom data structures
                var events = await query.ToListAsync();
                result.TotalEvents = await _context.LocalEvents.CountAsync(e => e.IsActive);
                result.FilteredEvents = events.Count;

                // Sort using custom priority queue or custom sorting
                var sortedEvents = SortEvents(events, searchModel.SortBy);

                foreach (var eventItem in sortedEvents)
                {
                    result.Events.Add(eventItem);
                }

                // Get categories
                result.Categories = await GetCategoriesAsync();

                // Get recommendations
                result.RecommendedEvents = await GetRecommendedEventsAsync(sessionId, userId);

                // Get recent searches
                result.RecentSearches = await GetRecentSearchesAsync(sessionId, userId);
            }
            catch (Exception)
            {
                // Handle search errors gracefully
            }

            return result;
        }

        public async Task<CustomLinkedList<LocalEvent>> GetRecommendedEventsAsync(string? sessionId = null, int? userId = null)
        {
            var recommendations = new CustomLinkedList<LocalEvent>();

            try
            {
                // Get user's search history
                var searchHistory = await GetUserSearchHistoryAsync(sessionId, userId);

                // Use priority queue to rank events based on user preferences
                var recommendationQueue = new CustomPriorityQueue<LocalEvent>();
                var allEvents = await GetUpcomingEventsAsync();

                foreach (var eventItem in allEvents)
                {
                    int score = CalculateRecommendationScore(eventItem, searchHistory);
                    if (score > 0)
                    {
                        recommendationQueue.Enqueue(eventItem, score);
                    }
                }

                // Get top 5 recommendations
                int count = 0;
                while (recommendationQueue.Any() && count < 5)
                {
                    recommendations.Add(recommendationQueue.Dequeue());
                    count++;
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }

            return recommendations;
        }

        public async Task<CustomLinkedList<string>> GetCategoriesAsync()
        {
            var categories = new CustomLinkedList<string>();

            // Use set to ensure unique categories
            var uniqueCategories = await _context.LocalEvents
                .Where(e => e.IsActive)
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();

            foreach (var category in uniqueCategories)
            {
                categories.Add(category);
                _categories.Add(category);
            }

            return categories;
        }

        public async Task<CustomDictionary<string, int>> GetEventStatisticsAsync()
        {
            var statistics = new CustomDictionary<string, int>();

            try
            {
                var totalEvents = await _context.LocalEvents.CountAsync(e => e.IsActive);
                var upcomingEvents = await _context.LocalEvents.CountAsync(e => e.IsActive && e.EventDate >= DateTime.Now);
                var pastEvents = totalEvents - upcomingEvents;

                statistics.Add("Total", totalEvents);
                statistics.Add("Upcoming", upcomingEvents);
                statistics.Add("Past", pastEvents);

                // Category breakdown
                var categoryStats = await _context.LocalEvents
                    .Where(e => e.IsActive)
                    .GroupBy(e => e.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var stat in categoryStats)
                {
                    statistics.Add($"Category_{stat.Category}", stat.Count);
                }
            }
            catch (Exception)
            {
                // Return empty stats on error
            }

            return statistics;
        }

        public async Task RecordSearchAsync(string searchTerm, string category, string? sessionId = null, int? userId = null)
        {
            try
            {
                var searchHistory = new UserSearchHistory
                {
                    UserId = userId,
                    SearchTerm = searchTerm,
                    Category = category,
                    SessionId = sessionId ?? "",
                    SearchDate = DateTime.UtcNow
                };

                _context.UserSearchHistories.Add(searchHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle search recording error gracefully
            }
        }

        public async Task IncrementViewCountAsync(int eventId)
        {
            try
            {
                var eventItem = await _context.LocalEvents.FindAsync(eventId);
                if (eventItem != null)
                {
                    eventItem.ViewCount++;
                    await _context.SaveChangesAsync();
                    ClearEventCache();
                }
            }
            catch (Exception)
            {
                // Handle view count error gracefully
            }
        }

        private CustomLinkedList<LocalEvent> SortEvents(List<LocalEvent> events, string sortBy)
        {
            var sorted = new CustomLinkedList<LocalEvent>();

            switch (sortBy?.ToLower())
            {
                case "priority":
                    var priorityQueue = new CustomPriorityQueue<LocalEvent>();
                    foreach (var eventItem in events)
                    {
                        priorityQueue.Enqueue(eventItem, eventItem.Priority);
                    }
                    while (priorityQueue.Any())
                    {
                        sorted.Add(priorityQueue.Dequeue());
                    }
                    break;

                case "popularity":
                    events = events.OrderByDescending(e => e.ViewCount).ToList();
                    foreach (var eventItem in events)
                    {
                        sorted.Add(eventItem);
                    }
                    break;

                case "date":
                default:
                    events = events.OrderBy(e => e.EventDate).ToList();
                    foreach (var eventItem in events)
                    {
                        sorted.Add(eventItem);
                    }
                    break;
            }

            return sorted;
        }

        private async Task<CustomLinkedList<UserSearchHistory>> GetUserSearchHistoryAsync(string? sessionId, int? userId)
        {
            var history = new CustomLinkedList<UserSearchHistory>();

            try
            {
                var query = _context.UserSearchHistories.AsQueryable();

                if (userId.HasValue)
                {
                    query = query.Where(h => h.UserId == userId.Value);
                }
                else if (!string.IsNullOrEmpty(sessionId))
                {
                    query = query.Where(h => h.SessionId == sessionId);
                }
                else
                {
                    return history; // No user context
                }

                var searches = await query
                    .OrderByDescending(h => h.SearchDate)
                    .Take(20)
                    .ToListAsync();

                foreach (var search in searches)
                {
                    history.Add(search);
                }
            }
            catch (Exception)
            {
                // Return empty history on error
            }

            return history;
        }

        private int CalculateRecommendationScore(LocalEvent eventItem, CustomLinkedList<UserSearchHistory> searchHistory)
        {
            int score = eventItem.Priority; // Base score from event priority

            foreach (var search in searchHistory)
            {
                // Category match
                if (eventItem.Category.Equals(search.Category, StringComparison.OrdinalIgnoreCase))
                {
                    score += 5;
                }

                // Search term match
                if (!string.IsNullOrEmpty(search.SearchTerm) &&
                    (eventItem.Title.Contains(search.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     eventItem.Description.Contains(search.SearchTerm, StringComparison.OrdinalIgnoreCase)))
                {
                    score += 3;
                }

                // Recency factor
                var daysSinceSearch = (DateTime.UtcNow - search.SearchDate).Days;
                if (daysSinceSearch <= 7)
                {
                    score += 2;
                }
                else if (daysSinceSearch <= 30)
                {
                    score += 1;
                }
            }

            // Popularity factor
            score += eventItem.ViewCount / 10;

            return score;
        }

        private async Task<CustomLinkedList<string>> GetRecentSearchesAsync(string? sessionId, int? userId)
        {
            var recentSearches = new CustomLinkedList<string>();

            try
            {
                var query = _context.UserSearchHistories.AsQueryable();

                if (userId.HasValue)
                {
                    query = query.Where(h => h.UserId == userId.Value);
                }
                else if (!string.IsNullOrEmpty(sessionId))
                {
                    query = query.Where(h => h.SessionId == sessionId);
                }
                else
                {
                    return recentSearches;
                }

                var searches = await query
                    .Where(h => !string.IsNullOrEmpty(h.SearchTerm))
                    .OrderByDescending(h => h.SearchDate)
                    .Select(h => h.SearchTerm)
                    .Distinct()
                    .Take(5)
                    .ToListAsync();

                foreach (var search in searches)
                {
                    recentSearches.Add(search);
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }

            return recentSearches;
        }

        private void ClearEventCache()
        {
            // Clear cache to force refresh
            var keys = _eventCache.GetKeys();
            foreach (var key in keys)
            {
                // Use a simple approach to clear the cache
                var newCache = new CustomDictionary<string, CustomLinkedList<LocalEvent>>();
                _eventCache.Clear(); // Clear all items
                break; // Exit after clearing
            }
        }
    }
}