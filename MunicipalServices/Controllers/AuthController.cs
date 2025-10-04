using Microsoft.AspNetCore.Mvc;
using MunicipalServices.Models;
using MunicipalServices.Services;

namespace MunicipalServices.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Redirect if already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
                return RedirectToAction(isAdmin ? "Dashboard" : "Index", isAdmin ? "Admin" : "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var result = await _userService.LoginUserAsync(model);

            if (result.Success && result.User != null)
            {
                // Set session variables
                HttpContext.Session.SetInt32("UserId", result.User.Id);
                HttpContext.Session.SetString("UserName", result.User.Name);
                HttpContext.Session.SetString("IsAdmin", result.IsAdmin.ToString().ToLower());

                TempData["SuccessMessage"] = result.Message;

                // Handle return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Default redirect based on user type
                if (result.IsAdmin)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            // Redirect if already logged in
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message + " Please log in.";

                // Redirect to login with return URL
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Login", new { returnUrl = returnUrl });
                }

                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}