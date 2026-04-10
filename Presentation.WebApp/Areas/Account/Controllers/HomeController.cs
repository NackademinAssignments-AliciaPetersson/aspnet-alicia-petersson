using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult AboutMe(AboutMeViewModel viewModel)
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
            var viewModel = new AboutMeViewModel() { Message = result.ErrorMessage ?? "Could not remove account. Try again later" };
            return RedirectToAction(nameof(AboutMe));
        }

        await authService.SignOutUserAsync();

        return Redirect("/");
    }
}
