namespace DriveShare.ViewModels
{
    public class FileDataViewModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public decimal Size { get; set; }
        public int DownloadCount { get; set; }
        public string Created { get; set; }
    }
}
