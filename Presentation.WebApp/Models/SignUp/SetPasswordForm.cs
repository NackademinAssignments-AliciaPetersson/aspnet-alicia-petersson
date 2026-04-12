using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.SignUp;

public class SetPasswordForm
{
    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character"
    )]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Password must be confirmed")]
    [Compare(nameof(Password), ErrorMessage = "Password must match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confim Password")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "I Accept the user terms & conditions.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Accepting the user terms & conditions is required")]
    public bool TermsAndConditions { get; set; }
}
