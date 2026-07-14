using Microsoft.AspNetCore.Mvc;

namespace CelebrationPassports.Web.Controllers
{
    public class CelebrationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}