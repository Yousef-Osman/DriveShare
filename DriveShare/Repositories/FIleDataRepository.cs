using DriveShare.Helpers;
using DriveShare.Data;
using DriveShare.Models;
using DriveShare.Models.Enums;
using DriveShare.Repositories.Interfaces;
using DriveShare.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace DriveShare.Repositories;
public class FIleDataRepository : IFileData
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public FIleDataRepository(ApplicationDbContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }

    public async Task<FileData> GetFileAsync(string id)
    {
        return await _context.Files.FindAsync(id);
    }

    public IQueryable<FileDataViewModel> GetAllFilesQuery(string sortColumn, SortOrder sortDirection, string searchValue)
    {
        var query = _context.Files.Where(a => a.IsDeleted == false && a.IsPrivate == false).AsQueryable();
        var selectQuery = GetSelectQuery(GetSearchQuery(query, searchValue));
        return GetSortQuery(selectQuery, sortColumn, sortDirection);
    }

    public async Task<List<FileDataViewModel>> GetLatestFilesAsync()
    {
        var query = _context.Files.Where(a => a.IsDeleted == false && a.IsPrivate == false).AsQueryable();
        var selectQuery = GetSelectQuery(GetSearchQuery(query, null));
        return await selectQuery.OrderByDescending(a => a.DownloadCount).ThenByDescending(a => a.LastDownloaded)
                                .Take(4).ToListAsync();
    }

    public IQueryable<FileData> GetUserFilesQuery(string userId)
    {
        return _context.Files.Where(a => a.UserId == userId && a.IsDeleted == false).AsQueryable();
    }

    public IQueryable<FileData> GetSearchQuery(IQueryable<FileData> query, string searchValue)
    {
        return query.Where(a => string.IsNullOrEmpty(searchValue) ? true : (a.FileName.ToLower().Contains(searchValue) ||
                                a.ContentType.ToLower().Contains(searchValue)));
    }

    public IQueryable<FileDataViewModel> GetSelectQuery(IQueryable<FileData> query)
    {
        return query.Select(a => new FileDataViewModel()
        {
            Id = a.Id,
            FileName = a.FileName,
            Description = a.Description,
            FileSerial = a.FileSerial,
            ContentType = a.ContentType,
            DownloadCount = a.DownloadCount,
            Size = a.Size,
            CreatedOn = a.CreatedOn,
            LastModifiedOn = a.LastModifiedOn,
            LastDownloaded = a.LastDownloaded
        });
    }

    public IQueryable<FileDataViewModel> GetSortQuery(IQueryable<FileDataViewModel> query, string sortColumn, SortOrder sortDirection)
    {
        return (!string.IsNullOrEmpty(sortColumn)) ?
                query.OrderByDynamic(sortColumn, sortDirection) :
                query.OrderByDescending(a => a.CreatedOn);
    }

    public async Task<List<FileDataViewModel>> GetFilteredAsync(IQueryable<FileDataViewModel> query, int start, int length)
    {
        return await query.Skip(start).Take(length).ToListAsync();
    }

    public async Task<int> GetCountAsync(IQueryable<FileDataViewModel> query)
    {
        return await query.CountAsync();
    }

    public async Task CreateAsync(UploadFileViewModel model, string userId)
    {
        var extention = Path.GetExtension(model.File.FileName);
        var fileSerial = string.Concat(Guid.NewGuid(), extention);
        var root = _env.WebRootPath;
        var path = Path.Combine(root, "Uploads", fileSerial);

        using (var stream = System.IO.File.Create(path))
        {
            await model.File.CopyToAsync(stream);
        }

        var upload = new FileData()
        {
            FileName = model.File.FileName,
            FileSerial = fileSerial,
            ContentType = extention.Remove(0, 1),
            UserId = userId,
            Size = model.File.Length
        };

        await _context.Files.AddAsync(upload);
        await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(FileData model)
    {
        _context.Update(model);
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var file = await GetFileAsync(id);

        if (file == null || file.UserId != userId)
            return false;

        file.IsDeleted = true;
        file.DeletedOn = DateTime.Now;

        return (await UpdateAsync(file) > 0) ? true : false;
    }

    public async Task<FileData> DownloadFileAsync(string id)
    {
        var file = await _context.Files.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false && a.IsPrivate == false);

        if (file != null)
        {
            file.DownloadCount++;
            file.LastDownloaded = DateTime.Now;

            _context.Update(file);
            _context.SaveChanges();
        }

        return file;
    }
}