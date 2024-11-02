using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoFrontend_MVC.Models;
using TodoFrontend_MVC.Services;

namespace TodoFrontend_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Signup() => View();

        [HttpPost]
        public async Task<IActionResult> Signup(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var success = await _userService.SignupAsync(model);
                if (success) return RedirectToAction("Login");
                ModelState.AddModelError("", "Signup failed.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session or any authentication cookies
            HttpContext.Session.Clear(); // if you are using sessions
            Response.Cookies.Delete(".AspNetCore.Cookies"); // if you are using cookie authentication

            // Redirect to the login page
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Login(TodoFrontend_MVC.Models.LoginRequest  loginRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = await _userService.LoginAsync(loginRequest);
                if (userId.HasValue)
                {
                    // Store the UserId in session
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                    HttpContext.Session.SetString("Email", loginRequest.Email);

                    //return RedirectToAction("Dashboard"); // Redirect to Dashboard after successful login
                    return RedirectToAction("GetTasks", "Task");

                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(loginRequest);
        }


        [HttpGet]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId; // Pass the UserId to the view
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var success = await _userService.ForgotPasswordAsync(request);
                if (success)
                {
                    ViewBag.Message = "A password reset link has been sent to your email.";
                    return View();
                }
                ModelState.AddModelError("", "Failed to send reset link. Please try again.");
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordRequest
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View(request);
            }

            var response = await _userService.ResetPasswordAsync(request);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Failed to reset password. Token may be invalid or expired.");
                return View(request);
            }
        }
    }
}
