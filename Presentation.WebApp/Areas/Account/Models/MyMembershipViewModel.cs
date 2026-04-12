namespace Presentation.WebApp.Areas.Account.Models;

public class MyMembershipViewModel
{
    public string? ProfileImageUrl { get; set; }
    public ChooseMembershipForm ChooseMembershipForm { get; set; } = new ChooseMembershipForm();
}
