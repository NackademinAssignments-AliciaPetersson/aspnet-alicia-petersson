using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Presentation.WebApp.Areas.Account.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;

[Area("Account")]
[Route("account")]
[Authorize(Roles = "Member")]
public class HomeController(IAuthService authService, IMemberService memberService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(AboutMe));
    }

    [HttpGet("about-me")]
    public async Task<IActionResult> AboutMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            await authService.SignOutUserAsync();
            return Redirect("/");
        }

        var accountResult = await memberService.GetMemberDetailsAsync(userId);
        if (!accountResult.Success)
        {
            ViewData["ErrorMessage"] = accountResult.ErrorMessage ?? "Could not load profile details. Try again later";
            return View();
        }

        var viewModel = new AboutMeViewModel {
            AboutMeForm = new AboutMeForm
            {
                FirstName = accountResult.Value?.FirstName ?? "",
                LastName = accountResult.Value?.LastName ?? "",
                Email = accountResult.Value?.Email ?? "",
                PhoneNumber = accountResult.Value?.PhoneNumber ?? ""
            },
            ProfileImageUrl = accountResult.Value?.ImageUrl ?? "~/images/default_profile_image.png"
        };        
        

        return View(viewModel);
    }

    [HttpPost("about-me")]
    public async Task<IActionResult> AboutMe(AboutMeViewModel viewModel)
    {
        return View(viewModel);
    }

        [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        await authService.SignOutUserAsync();
        return Redirect("/");
    }

    [HttpGet("remove-account")]
    public async Task<IActionResult> RemoveAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await memberService.DeleteMemberAsync(userId);

        if (!result.Success)
        {
            ViewData["ErrorMessage"] = result.ErrorMessage ?? "Could not remove account. Try again later";
            return RedirectToAction(nameof(AboutMe));
        }

        await authService.SignOutUserAsync();

        return Redirect("/");
    }
}
