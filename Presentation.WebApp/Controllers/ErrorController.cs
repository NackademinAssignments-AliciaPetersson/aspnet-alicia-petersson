using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class ErrorController : Controller
{
    [Route("error/{statusCode}")]
    public IActionResult HandleErrorCode(int statusCode)
    {
        Response.StatusCode = statusCode;

        return statusCode switch
        {
            404 => View("NotFound"),
            401 => View("Denied"),
            403 => View("Denied"),
            _ => View("Error")
        };        
    }
}
