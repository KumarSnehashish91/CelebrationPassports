using CelebrationPassports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CelebrationPassports.Controllers
{
    // Every other controller requires auth; this one (default MVC scaffold leftovers)
    // didn't, which meant an unauthenticated visitor hitting /Home or /Home/Privacy
    // directly saw a page instead of being forced to /Account/Login. Error stays
    // reachable regardless of auth state — UseExceptionHandler("/Home/Error") needs it
    // for unauthenticated failures too.
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
