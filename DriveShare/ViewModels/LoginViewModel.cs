using System.ComponentModel.DataAnnotations;

namespace DriveShare.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
