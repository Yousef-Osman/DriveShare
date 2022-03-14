using System.ComponentModel.DataAnnotations;

namespace DriveShare.ViewModels;

public class UploadFileViewModel
{
    [Required]
    public IFormFile File { get; set; }
}
