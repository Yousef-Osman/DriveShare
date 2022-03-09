using System.ComponentModel.DataAnnotations;

namespace DriveShare.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
