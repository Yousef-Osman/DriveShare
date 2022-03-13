using System.ComponentModel.DataAnnotations;

namespace DriveShare.ViewModels;

public class FileViewModel
{
    [Required]
    public IFormFile File { get; set; }
}
