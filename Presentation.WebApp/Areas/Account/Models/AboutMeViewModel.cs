namespace Presentation.WebApp.Areas.Account.Models;

public class AboutMeViewModel
{
    public string? ProfileImageUrl { get; set; }
    public AboutMeForm AboutMeForm { get; set; } = new AboutMeForm();
    public string? Message { get; set; }
}
