using System.Security.Claims;
using CelebrationPassports.Web.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = CelebrationPassports.Web.Interfaces.IAuthenticationService;

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
        public IActionResult Register(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var result = await _authenticationService.RegisterAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Registration failed.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            await SignInAsync(result);

            return RedirectAfterAuth(returnUrl);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var result = await _authenticationService.LoginAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Invalid email or password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            await SignInAsync(result);

            return RedirectAfterAuth(returnUrl);
        }

        // Scoped to this one use case (claim link continuity) rather than a general
        // auth returnUrl system — Url.IsLocalUrl guards against open-redirect via a
        // crafted external returnUrl.
        private IActionResult RedirectAfterAuth(string? returnUrl) =>
            !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Dashboard");

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var sessionIdClaim = User.FindFirstValue("session_id");

            if (Guid.TryParse(sessionIdClaim, out var sessionId))
            {
                await _authenticationService.LogoutAsync(sessionId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        private async Task SignInAsync(AuthResult result)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new(ClaimTypes.Email, result.Email),
                new(ClaimTypes.GivenName, result.FirstName),
                new(ClaimTypes.Surname, result.LastName),
                new("session_id", result.SessionId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties();
            properties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = result.AccessToken },
                new AuthenticationToken { Name = "refresh_token", Value = result.RefreshToken },
                new AuthenticationToken { Name = "expires_on", Value = result.ExpiresOn.ToString("o") }
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
    }
}
