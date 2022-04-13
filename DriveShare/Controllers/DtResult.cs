using DriveShare.ViewModels;

namespace DriveShare.Controllers
{
    internal class DtResult<T>
    {
        public int Draw { get; set; }
        public object RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<FileDataViewModel> Data { get; set; }
    }
}