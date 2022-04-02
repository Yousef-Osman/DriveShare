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

    public IActionResult SearchFiles(string searchValue)
    {
        var files = GetAllFiles(searchValue).ToList();
        return View(files);
    }

    public IActionResult BrowseFiles()
    {
        var files = GetAllFiles().ToList();
        return View(files);
    }

    private IEnumerable<FileDataViewModel> GetAllFiles(string searchValue = null)
    {
        var fileQuery = _context.Files.Where(a=> (searchValue != null)? a.FileName.Contains(searchValue) : true).Select(a => new FileDataViewModel()
        {
            FileName = a.FileName,
            ContentType = a.ContentType,
            Description = a.Description,
            DownloadCount = a.DownloadCount,
            Size = a.Size,
            CreatedOn = a.CreatedOn.ToString("dd-MMM-yyyy HH:mm"),
            LastModifiedOn = a.LastModifiedOn.HasValue ? a.LastModifiedOn.Value.ToString("dd-MMM-yyyy HH:mm") : " - "
        }).OrderByDescending(a => a.CreatedOn);

        return fileQuery;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
