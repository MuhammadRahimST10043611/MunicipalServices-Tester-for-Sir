using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;

namespace MunicipalServices.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserEngagementService _userEngagementService;
        private readonly IReportProcessingService _reportProcessingService;

        public ServiceRequestController(
            IServiceRequestService serviceRequestService,
            IFileUploadService fileUploadService,
            IUserEngagementService userEngagementService,
            IReportProcessingService reportProcessingService)
        {
            _serviceRequestService = serviceRequestService;
            _fileUploadService = fileUploadService;
            _userEngagementService = userEngagementService;
            _reportProcessingService = reportProcessingService;
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

        [HttpGet]
        public IActionResult ReportIssue()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            var model = new ReportIssueViewModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult ReportDetails(int id)
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            var report = _serviceRequestService.GetServiceRequestById(id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("ViewReports");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIssue(ReportIssueViewModel model)
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Set the current user ID in the service
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _reportProcessingService.SetCurrentUserId(userId);

                // Convert ICollection<IFormFile> from model binding to custom collection
                if (Request.Form.Files.Count > 0)
                {
                    var customCollection = new CustomCollection<IFormFile>();
                    foreach (var file in Request.Form.Files)
                    {
                        if (file.Length > 0) // Only add non-empty files
                        {
                            customCollection.Add(file);
                        }
                    }
                    model.AttachedFiles = customCollection;
                }

                var result = await _reportProcessingService.ProcessReportSubmissionAsync(model);

                if (result.Success)
                {
                    if (result.ShowCelebration)
                    {
                        TempData["ShowCelebration"] = true;
                        TempData["CelebrationMessage"] = result.CelebrationMessage;
                    }

                    TempData["SuccessMessage"] = result.Message;
                    TempData["RequestId"] = result.RequestId;
                    return RedirectToAction("ReportSuccess");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while submitting your report. Please try again.");
                return View(model);
            }
        }

        public IActionResult ReportSuccess()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            ViewBag.RequestId = TempData["RequestId"];
            return View();
        }

        public IActionResult LocalEvents()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            ViewBag.Message = "Local Events and Announcements feature will be available soon.";
            return View();
        }

        public IActionResult ServiceStatus()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            ViewBag.Message = "Service Request Status tracking will be available soon.";
            return View();
        }

        public IActionResult ViewReports()
        {
            var loginCheck = RedirectToLoginIfNeeded();
            if (loginCheck != null) return loginCheck;

            // Get current user ID
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var reports = _serviceRequestService.GetUserServiceRequests(userId);

            var reportsList = new List<ServiceRequest>();
            foreach (var report in reports)
            {
                reportsList.Add(report);
            }

            return View(reportsList);
        }
    }
}