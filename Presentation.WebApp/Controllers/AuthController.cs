using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Application.Modules.Members.Inputs;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.Register;

namespace Presentation.WebApp.Controllers;

[Route("auth")]
public class AuthController(IAuthService authService, IMemberService memberService) : Controller
{
    private const string EmailSessionKey = "EmailSessionKey";

    [HttpGet("sign-up")]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost("sign-up")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var normalizedEmail = form.Email.Trim().ToLowerInvariant();

        if (await authService.DoesUserExistAsync(normalizedEmail))
        {
            ModelState.AddModelError(nameof(form.Email), "User with the same email address already exists");
            return View(form);
        }

        HttpContext.Session.SetString(EmailSessionKey, normalizedEmail);

        return RedirectToAction(nameof(SetPassword));
    }

    [HttpGet("set-password")]
    public IActionResult SetPassword()
    {
        var sessionEmail = HttpContext.Session.GetString(EmailSessionKey);
        if (string.IsNullOrWhiteSpace(sessionEmail))
            return RedirectToAction(nameof(SignUp));

        var form = new SetPasswordForm { Email = sessionEmail };

        return View(form);
    }

    [HttpPost("set-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordForm form, CancellationToken ct = default)
    {
        var sessionEmail = HttpContext.Session.GetString(EmailSessionKey);
        if (string.IsNullOrWhiteSpace(sessionEmail))
            return RedirectToAction(nameof(SignUp));

        if (!ModelState.IsValid)
            return View(form);

        if (sessionEmail != form.Email)
        {
            form.Email = sessionEmail;
            ModelState.AddModelError(nameof(form.Email), "Can't change email in this view. Start over if you want to change your email address");
            return View(form);
        }

        var memberInput = new CreateMemberInput(sessionEmail, form.Password);
        var result = await memberService.CreateMemberAsync(memberInput, ct);

        if (!result.Success)
        {
            ViewData["ErrorMessage"] = result.ErrorMessage;
            return View(form);
        }

        HttpContext.Session.Remove(EmailSessionKey);

        return RedirectToAction(nameof(SignIn));
    }

    [HttpGet("sign-in")]
    public IActionResult SignIn()
    {
        return View();
    }
}
