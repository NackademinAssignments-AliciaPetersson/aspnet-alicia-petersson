using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Route("training-offers")]
public class TrainingsController : Controller
{
    [Route("pt-training")]
    public IActionResult PersonalTraining()
    {
        return View();
    }

    [Route("group-training")]
    public IActionResult GroupTraining()
    {
        return View();
    }

    [Route("padel")]
    public IActionResult Padel()
    {
        return View();
    }
}
