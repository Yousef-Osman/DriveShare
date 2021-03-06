using DriveShare.Data;

namespace DriveShare.Models;

public class FileData : BaseClass
{
    public string FileName { get; set; }
    public string FileSerial { get; set; }
    public string ContentType { get; set; }
    public string Description { get; set; }
    public decimal Size { get; set; }
    public int DownloadCount { get; set; }
    public DateTime? LastDownloaded { get; set; }
    public bool IsPrivate { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
