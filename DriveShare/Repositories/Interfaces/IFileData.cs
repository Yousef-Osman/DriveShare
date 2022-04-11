using DriveShare.Models;
using DriveShare.Models.Enums;

namespace DriveShare.Repositories.Interfaces
{
    public interface IFileData
    {
        List<FileData> GetFiles(string sortProperty, SortOrder sortOrder);
    }
}
