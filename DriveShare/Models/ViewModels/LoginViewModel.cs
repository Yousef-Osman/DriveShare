using System.ComponentModel.DataAnnotations;

namespace DriveShare.Models.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
