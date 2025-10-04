using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;

namespace MunicipalServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IUserEngagementService _userEngagementService;

        public HomeController(IServiceRequestService serviceRequestService, IUserEngagementService userEngagementService)
        {
            _serviceRequestService = serviceRequestService;
            _userEngagementService = userEngagementService;
        }

        // Helper method to check if user is logged in
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }

        // Helper method to redirect to login if not authenticated
        private IActionResult RedirectToLoginIfNeeded()
        {
            if (!IsUserLoggedIn())
            {
                var controller = ControllerContext.ActionDescriptor.ControllerName;
                var action = ControllerContext.ActionDescriptor.ActionName;
                var returnUrl = $"/{controller}/{action}";

                return RedirectToAction("Login", "Auth", new { returnUrl = returnUrl });
            }
            return null;
        }

        // Allow public access to home page, but show different content based on login status
        public IActionResult Index()
        {
            var stats = _serviceRequestService.GetDashboardStats();

            // Get user-specific reports if logged in
            List<ServiceRequest> recentReportsList;
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                var userId = HttpContext.Session.GetInt32("UserId").Value;
                var userReports = _serviceRequestService.GetUserServiceRequests(userId);

                // Get last 5 reports
                var recentReports = userReports.TakeLast(5);
                recentReportsList = recentReports.ToList();
            }
            else
            {
                // Show only sample reports for non-logged in users
                var recentReports = _serviceRequestService.GetRecentReports();
                recentReportsList = recentReports.ToList();
            }

            ViewBag.Stats = stats;
            ViewBag.RecentReports = recentReportsList;

            // Only show engagement stats if user is logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                var engagementStats = _userEngagementService.GetUserEngagementStats();
                ViewBag.EngagementStats = engagementStats;
            }

            return View();
        }

        // Require login for local events
        public IActionResult LocalEvents()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            ViewData["Title"] = "Local Events & Announcements";
            return View();
        }

        // Require login for service status
        public IActionResult ServiceStatus()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            ViewData["Title"] = "Service Request Status";
            return View();
        }

        // Public privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Public error page
        public IActionResult Error()
        {
            return View();
        }
    }
}