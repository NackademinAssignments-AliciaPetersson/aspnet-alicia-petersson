using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.SignIn;

public class SignInForm
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address *", Prompt = "example@domain.com")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password *", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }
}
