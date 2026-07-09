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

            try
            {
                var result = await _authenticationService.RegisterAsync(model);

                if (!result)
                {
                    ModelState.AddModelError("", "Registration failed.");
                    return View(model);
                }

                //return Content("SUCCESS");
                if (!result)
                {
                    ModelState.AddModelError("", "Registration failed.");
                    return View(model);
                }

                ViewBag.SuccessMessage = "🎉 Your Celebration Passport has been created successfully.";

                ModelState.Clear();

                return View(new RegisterViewModel());
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }

           
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }
    }
}