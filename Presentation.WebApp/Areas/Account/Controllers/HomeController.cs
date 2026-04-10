using Application.Abstractions.Identity;
using Application.Abstractions.Services;
using Application.Modules.Members.Inputs;
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
            return RedirectToAction(nameof(SignOut));

        var accountResult = await memberService.GetMemberDetailsAsync(userId);
        if (!accountResult.Success)
            return RedirectToAction(nameof(SignOut));

        var viewModel = new AboutMeViewModel {
            AboutMeForm = new AboutMeForm
            {
                FirstName = accountResult.Value?.FirstName ?? "",
                LastName = accountResult.Value?.LastName ?? "",
                Email = accountResult.Value?.Email ?? "",
                PhoneNumber = accountResult.Value?.PhoneNumber ?? ""
            },
            ProfileImageUrl = accountResult.Value?.ImageUrl ?? "/images/default_profile_image.png"
        };        
        

        return View(viewModel);
    }

    [HttpPost("about-me")]
    public async Task<IActionResult> AboutMe(AboutMeViewModel viewModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        if (!ModelState.IsValid)
            return View(viewModel);

        var imageUrl = viewModel.ProfileImageUrl ?? "/images/default_profile_image.png";
        if (viewModel.AboutMeForm.ProfileImage is not null && viewModel.AboutMeForm.ProfileImage.Length > 0)
        {
            imageUrl = await SaveProfileImageAsync(viewModel.AboutMeForm.ProfileImage);
        }

        var details = new UpdateMemberDetailsInput(
            userId,
            viewModel.AboutMeForm.FirstName,
            viewModel.AboutMeForm.LastName,
            viewModel.AboutMeForm.PhoneNumber,
            imageUrl
        );

        var result = await memberService.UpdateMemberDetailsAsync(details);
        if (!result.Success)
        {
            viewModel.ProfileImageUrl = imageUrl ?? "~/images/default_profile_image.png";
            ViewData["ErrorMessage"] = "Unable to save changes";
            return View(viewModel);
        }

        return RedirectToAction(nameof(AboutMe));
    }

    [HttpGet("sign-out")]
    [AllowAnonymous]
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

    private static async Task<string> SaveProfileImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/profiles/{fileName}";
    }
}
