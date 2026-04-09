using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Application.Modules.Members.Inputs;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.SignIn;
using Presentation.WebApp.Models.SignUp;

namespace Presentation.WebApp.Controllers;

[Route("auth")]
public class AuthController(IAuthService authService, IMemberService memberService) : Controller
{
    private const string EmailSessionKey = "EmailSessionKey";

    #region Local Sign Up
    [HttpGet("sign-up")]
    public IActionResult SignUp()
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

        return View();
    }

    [HttpPost("sign-up")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpForm form)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

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
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

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
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

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
    #endregion

    #region Local Sign In
    [HttpGet("sign-in")]
    public IActionResult SignIn(string? returnUrl = null)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInForm form, string? returnUrl = null)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return redirect;

        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), "Incorrect email address or password");
            return View(form);
        }

        var signedIn = await authService.SignInLocalUserAsync(form.Email, form.Password, form.RememberMe);
        if (!signedIn.Success)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), signedIn?.ErrorMessage ?? "Incorrect email address or password");
            return View(form);
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectWhenSignedIn ?? Redirect("/");
    }
    #endregion

    #region private properties
    private IActionResult? RedirectWhenSignedIn
    {
        get
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return Redirect("/admin");

                if (User.IsInRole("Member"))
                    return Redirect("/account");

                return Redirect("/");
            }

            return null;
        }
    }

    #endregion
}
