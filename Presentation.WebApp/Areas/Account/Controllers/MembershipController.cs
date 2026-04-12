using Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Areas.Account.Models;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;

[Area("Account")]
[Route("account")]
[Authorize(Roles = "Member")]
public class MembershipController(IMemberService memberService, IMembershipTypeService membershipTypeService) : Controller
{
    [HttpGet("my-membership")]
    public async Task<IActionResult> MyMembership()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        var accountResult = await memberService.GetMemberDetailsAsync(userId);
        if (!accountResult.Success)
            return RedirectToAction(nameof(SignOut));

        var membershipTypes = await membershipTypeService.GetMembershipTypesAsync();

        var viewModel = new MyMembershipViewModel
        {
            ProfileImageUrl = accountResult.Value?.ImageUrl ?? "/images/default_profile_image.png"
        };

        return View(viewModel);
    }
}
