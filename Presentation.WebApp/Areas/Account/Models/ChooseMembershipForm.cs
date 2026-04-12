using Application.Modules.MembershipTypes.Outputs;
using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Areas.Account.Models;

public sealed class ChooseMembershipForm
{
    public List<MembershipTypeOutput> AvailableOptions { get; set; } = [];

    [Required(ErrorMessage = "You must select a membership type")]
    public string SelectedMembership { get; set; } = "";
}
