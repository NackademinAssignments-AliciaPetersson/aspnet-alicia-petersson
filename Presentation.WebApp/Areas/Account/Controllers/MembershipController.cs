using Application.Abstractions.Services;
using Application.Modules.MembershipTypes.Outputs;
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

        var membershipTypesResult = await membershipTypeService.GetMembershipTypesAsync();
        if (!membershipTypesResult.Success || membershipTypesResult.Value is null)
        {
            ViewData["ErrorMessage"] = "Could not load membership types.";
            return View(new MyMembershipViewModel());
        }

        var viewModel = new MyMembershipViewModel
        {
            ChooseMembershipForm = new ChooseMembershipForm() {
                AvailableOptions = [.. membershipTypesResult.Value]
            },
            ProfileImageUrl = accountResult.Value?.ImageUrl ?? "/images/default_profile_image.png"
        };

        return View(viewModel);
    }

    [HttpPost("my-membership")]
    public async Task<IActionResult> MyMembership(MyMembershipViewModel viewModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        //reloads avalibale Membership types. Profile image url loaded via hidden input
        var membershipTypesResult = await membershipTypeService.GetMembershipTypesAsync();
        if (!membershipTypesResult.Success || membershipTypesResult.Value is null)
        {
            ViewData["ErrorMessage"] = "Could not load membership types.";
            return View(new MyMembershipViewModel());
        }

        viewModel.ChooseMembershipForm.AvailableOptions = [.. membershipTypesResult.Value];        

        if (!ModelState.IsValid)
            return View(viewModel);

        return View(viewModel);
    }
}
