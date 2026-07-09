using CelebrationPassports.Web.Interfaces;
using CelebrationPassports.Web.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authenticationService.RegisterAsync(model);

            if (!result)
            {
                ModelState.AddModelError("", "Registration failed.");
                return View(model);
            }

            ViewBag.SuccessMessage = "🎉 Your Celebration Passport has been created successfully.";

            ModelState.Clear();

            return View(new RegisterViewModel());
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }

        

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authenticationService.LoginAsync(model);

            if (!result)
            {
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View(model);
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}