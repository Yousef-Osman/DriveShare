using DriveShare.Data;
using DriveShare.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DriveShare.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context,
                          ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> SearchFiles(string searchValue)
    {
        var files = await GetAllFiles(searchValue).ToListAsync();
        return View(files);
    }

    public async Task<IActionResult> BrowseFiles()
    {
        var files = await GetAllFiles().ToListAsync();
        return View(files);
    }

    private IQueryable<FileDataViewModel> GetAllFiles(string searchValue = null)
    {
        IQueryable<FileDataViewModel> fileQuery = _context.Files.Where(a => a.IsDeleted == false && 
            (string.IsNullOrEmpty(searchValue) ? true : a.FileName.ToLower().Contains(searchValue.ToLower())))
            .Select(a => new FileDataViewModel()
            {
                FileName = a.FileName,
                FileSerial = a.FileSerial,
                ContentType = a.ContentType,
                Description = a.Description,
                DownloadCount = a.DownloadCount,
                Size = a.Size,
                CreatedOn = a.CreatedOn,
                LastModifiedOn = a.LastModifiedOn
            }).OrderByDescending(a => a.CreatedOn).AsQueryable();

        return fileQuery;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
