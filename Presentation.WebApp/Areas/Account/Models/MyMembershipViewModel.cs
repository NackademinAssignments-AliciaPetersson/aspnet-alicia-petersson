using Application.Modules.Members.Outputs;

namespace Presentation.WebApp.Areas.Account.Models;

public class MyMembershipViewModel
{
    public string? ProfileImageUrl { get; set; }
    public ChooseMembershipForm ChooseMembershipForm { get; set; } = new ChooseMembershipForm();
    public bool ShowChooseMembershipForm { get; set; } = true;
    public ActiveMembership? ActiveMembership { get; set; }
}
