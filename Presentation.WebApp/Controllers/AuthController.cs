using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Application.Modules.Members.Inputs;
using Domain.Common.Validators;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.SignIn;
using Presentation.WebApp.Models.SignUp;

namespace Presentation.WebApp.Controllers;

[Route("auth")]
public class AuthController(IAuthService authService, IMemberService memberService, SignInManager<AuthenticationUser> signInManager) : Controller
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

        var normalizedEmail = EmailValidation.Validate(form.Email, "Email");

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

        var signedIn = await authService.SignInLocalUserAsync(form.Email, form.Password);
        if (!signedIn.Success)
            return RedirectToAction(nameof(SignIn));

        return RedirectWhenSignedIn ?? Redirect("/");

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

    #region External Sign In & Sign Up
    [HttpPost("external-login")]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalSignIn(string provider, string? returnUrl = null)
    {
        var callbackUrl = Url.Action(nameof(ExternalSignInCallback), "Auth", new
        {
            returnUrl
        });

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalSignInCallback(string? returnUrl = null, string? remoteError = null, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            TempData["ErrorMessage"] = $"External provider error: {remoteError}";
            return RedirectToAction(nameof(SignIn), new { returnUrl });
        }

        var authResult = await authService.SignInExternalMemberAsync("Member");
        if (!authResult.Success)
        {
            TempData["ErrorMessage"] = authResult.ErrorMessage;
            return RedirectToAction(nameof(SignIn), new { returnUrl });
        }

        CreateExternalMemberInput? createMemberInput = authResult.Value;
        if (createMemberInput is not null) 
        { 
            var memberResult = await memberService.CreateMemberForExternalUserAsync(createMemberInput, ct);
            if (!memberResult.Success)
            {
                TempData["ErrorMessage"] = memberResult.ErrorMessage;
                return RedirectToAction(nameof(SignIn), new { returnUrl });
            }
        }

        if (!string.IsNullOrWhiteSpace(returnUrl))
            return Redirect(returnUrl);

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
