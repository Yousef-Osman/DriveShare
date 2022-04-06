using System.ComponentModel.DataAnnotations;

namespace DriveShare.ViewModels;

public class FileDataViewModel
{
    public string Id { get; set; }
    [Display(Name ="FileName")]
    public string FileName { get; set; }
    public string FileSerial { get; set; }
    [Display(Name = "FileType")]
    public string ContentType { get; set; }
    [Display(Name = "Description")]
    public string Description { get; set; }
    [Display(Name = "Size")]
    public decimal Size { get; set; }
    [Display(Name = "DownloadCount")]
    public int DownloadCount { get; set; }
    [Display(Name = "DateCreated")]
    public DateTime CreatedOn { get; set; }
    [Display(Name = "LastModified")]
    public DateTime? LastModifiedOn { get; set; }
    [Display(Name = "LastDownloaded")]
    public DateTime? LastDownloaded { get; set; }
}
