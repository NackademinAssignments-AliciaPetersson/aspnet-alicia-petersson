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
            _ => View("Error")
        };        
    }
}
