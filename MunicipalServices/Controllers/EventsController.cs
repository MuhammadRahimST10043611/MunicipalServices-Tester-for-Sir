using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;

namespace MunicipalServices.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILocalEventService _eventService;

        public EventsController(ILocalEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var searchModel = new EventSearchViewModel();

            var userId = HttpContext.Session.GetInt32("UserId");
            var sessionId = HttpContext.Session.Id;

            var result = await _eventService.SearchEventsAsync(searchModel, sessionId, userId);

            ViewData["Title"] = "Local Events & Announcements";
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(EventSearchViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var sessionId = HttpContext.Session.Id;

            var result = await _eventService.SearchEventsAsync(model, sessionId, userId);

            ViewData["Title"] = "Local Events & Announcements - Search Results";
            return View("Index", result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);

            if (eventItem == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Index");
            }

            // Increment view count
            await _eventService.IncrementViewCountAsync(id);

            ViewData["Title"] = $"Event Details - {eventItem.Title}";
            return View(eventItem);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendations()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var sessionId = HttpContext.Session.Id;

            var recommendations = await _eventService.GetRecommendedEventsAsync(sessionId, userId);

            return PartialView("_RecommendationsPartial", recommendations);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _eventService.GetCategoriesAsync();
            return Json(categories.ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var statistics = await _eventService.GetEventStatisticsAsync();

            var stats = new Dictionary<string, int>();
            var keys = statistics.GetKeys();

            foreach (var key in keys)
            {
                if (statistics.TryGetValue(key, out var value))
                {
                    stats[key] = value;
                }
            }

            return Json(stats);
        }

        [HttpPost]
        public async Task<IActionResult> RecordSearch([FromBody] SearchRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var sessionId = HttpContext.Session.Id;

            await _eventService.RecordSearchAsync(request.SearchTerm ?? "", request.Category ?? "", sessionId, userId);

            return Json(new { success = true });
        }
    }

    public class SearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
    }
}