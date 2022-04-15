using DriveShare.Models;
using DriveShare.Models.Enums;
using DriveShare.ViewModels;

namespace DriveShare.Repositories.Interfaces
{
    public interface IFileData
    {
        Task<FileData> GetFileAsync(string id);
        IQueryable<FileDataViewModel> GetAllFilesQuery(string sortColumn, SortOrder sortDirection, string searchValue);
        Task<List<FileDataViewModel>> GetLatestFilesAsync();
        IQueryable<FileData> GetUserFilesQuery(string userId);
        IQueryable<FileData> GetSearchQuery(IQueryable<FileData> query, string searchValue);
        IQueryable<FileDataViewModel> GetSelectQuery(IQueryable<FileData> query);
        IQueryable<FileDataViewModel> GetSortQuery(IQueryable<FileDataViewModel> query, string sortColumn, SortOrder sortDirection);
        Task<List<FileDataViewModel>> GetFilteredAsync(IQueryable<FileDataViewModel> query, int start, int length);
        Task<int> GetCountAsync(IQueryable<FileDataViewModel> query);
        Task CreateAsync(UploadFileViewModel model, string userId);
        Task<int> UpdateAsync(FileData model);
        Task<bool> DeleteAsync(string id, string userId);
        Task<FileData> DownloadFileAsync(string id);
    }
}
