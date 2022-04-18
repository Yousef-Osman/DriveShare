using DriveShare.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveShare.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [ForeignKey(nameof(UserStatus))]
    public int Status { get; set; } = 1;
    public UserStatus UserStatus { get; set; }
}
