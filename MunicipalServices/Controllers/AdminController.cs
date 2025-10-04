using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;

namespace MunicipalServices.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILocalEventService _eventService;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IUserService _userService;

        public AdminController(
            ILocalEventService eventService,
            IServiceRequestService serviceRequestService,
            IUserService userService)
        {
            _eventService = eventService;
            _serviceRequestService = serviceRequestService;
            _userService = userService;
        }

        // Admin filter to check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var model = new AdminDashboardViewModel
            {
                EventStatistics = await _eventService.GetEventStatisticsAsync(),
                ServiceRequestStats = _serviceRequestService.GetDashboardStats(),
                RecentEvents = await _eventService.GetUpcomingEventsAsync(),
                RecentServiceRequests = _serviceRequestService.GetRecentReports(10),
                TotalUsers = (await _userService.GetAllUsersAsync()).Count
            };

            ViewData["Title"] = "Admin Dashboard";
            return View(model);
        }

        // Event Management
        [HttpGet]
        public async Task<IActionResult> Events()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var events = await _eventService.GetAllEventsAsync();

            ViewData["Title"] = "Manage Events";
            return View(events);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            ViewData["Title"] = "Create Event";
            return View(new LocalEvent());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(LocalEvent model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create the event directly without image handling
                var result = await _eventService.CreateEventAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Events");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Events");
            }

            ViewData["Title"] = "Edit Event";
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(LocalEvent model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _eventService.UpdateEventAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Events");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var result = await _eventService.DeleteEventAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Events");
        }

        // Service Request Management
        [HttpGet]
        public IActionResult ServiceRequests()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var requests = _serviceRequestService.GetAllServiceRequests();
            ViewData["Title"] = "Manage Service Requests";
            return View(requests);
        }

        [HttpGet]
        public IActionResult ServiceRequestDetails(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var request = _serviceRequestService.GetServiceRequestById(id);
            if (request == null)
            {
                TempData["ErrorMessage"] = "Service request not found.";
                return RedirectToAction("ServiceRequests");
            }

            ViewData["Title"] = $"Service Request Details - #{request.Id}";
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateServiceRequestStatus(int id, string status)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var request = _serviceRequestService.GetServiceRequestById(id);
                if (request == null)
                {
                    return Json(new { success = false, message = "Service request not found." });
                }

                // Update status
                request.Status = status;

                // Here you would normally save to database
                // Since we're using in-memory storage, the status is already updated

                // Return JSON response for AJAX request
                return Json(new
                {
                    success = true,
                    message = $"Service request #{id} status updated to '{status}'.",
                    requestId = id,
                    newStatus = status
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the status.",
                    error = ex.Message
                });
            }
        }

        // User Management
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var users = await _userService.GetAllUsersAsync();
            ViewData["Title"] = "Manage Users";
            return View(users);
        }
    }
}