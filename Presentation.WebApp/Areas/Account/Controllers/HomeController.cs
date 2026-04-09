using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Areas.Account.Controllers
{
    [Area("Account")]
    [Route("account")]
    [Authorize(Roles = "Member")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(AboutMe));
        }

        [HttpGet("about-me")]
        public IActionResult AboutMe()
        {
            return View();
        }


    }
}
