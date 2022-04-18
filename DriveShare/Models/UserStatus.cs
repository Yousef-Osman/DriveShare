using DriveShare.Data;

namespace DriveShare.Models;

public class UserStatus
{
    public int Id { get; set; }
    public string Status { get; set; }
    public ApplicationUser User { get; set; }
}
