using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.SignUp;

public class SignUpForm
{
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address must be valid")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address *", Prompt = "example@domain.com")]
    public string Email { get; set; } = null!;
}
