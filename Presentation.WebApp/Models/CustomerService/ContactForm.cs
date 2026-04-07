using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.CustomerService;

public class ContactForm
{
    [Required(ErrorMessage = "You must enter a first name")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Must contain at lest {2} characters")]
    [Display(Name = "First Name *", Prompt = "Enter First Name")]
    public string FirstName { get; set; } = null!;


    [Required(ErrorMessage = "You must enter a last name")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Must contain at lest {2} characters")]
    [Display(Name = "Last Name *", Prompt = "Enter Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "You must enter an email address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address format")]
    [Display(Name = "Email Address *", Prompt = "Enter Email Address")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number", Prompt = "Enter Phone Number")]
    public string? PhoneNumber { get; set; }


    [Required(ErrorMessage = "You must enter a message")]
    [StringLength(4000, MinimumLength = 5, ErrorMessage = "Must contain at least {2} characters")]
    [Display(Name = "Message *", Prompt = "Message...")]
    public string Message { get; set; } = null!;

    public string? ErrorMessage { get; set; }

}
