using Application.Abstractions.Services;
using Application.Modules.ContactRequests.Inputs;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.CustomerService;

namespace Presentation.WebApp.Controllers;

public class CustomerServiceController(IContactRequestService crService) : Controller
{
    [HttpGet("contact-us")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("contact-us")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactForm form, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(form);

        var input = new ContactRequestInput
        (
           form.FirstName,
           form.LastName,
           form.Email,
           form.PhoneNumber,
           form.Message
       );

        var result = await crService.CreateContactRequestAsync(input, ct);

        TempData["IsContactSubmitSuccess"] = result.Success;
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Your message has been sent. We will get back to you shortly.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), "Your message could not be sent. Please try again later.");
            return View(form);
        }
    }
}
