using DriveShare.Data;
using DriveShare.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DriveShare.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _env;

    public HomeController(ApplicationDbContext context,
                          ILogger<HomeController> logger,
                          IWebHostEnvironment env)
    {
        _context = context;
        _logger = logger;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var files = await GetAllFiles().OrderByDescending(a => a.DownloadCount).ThenByDescending(a => a.LastDownloaded)
                    .Take(4).ToListAsync();
        return View(files);
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> SearchFiles(string searchValue)
    {
        var files = await GetAllFiles(searchValue).OrderByDescending(a => a.CreatedOn).ToListAsync();
        return View(files);
    }

    public async Task<IActionResult> BrowseFiles()
    {
        var files = await GetAllFiles().OrderByDescending(a => a.CreatedOn).ToListAsync();
        return View(files);
    }



    private IQueryable<FileDataViewModel> GetAllFiles(string searchValue = null)
    {
        IQueryable<FileDataViewModel> fileQuery = _context.Files.Where(a => a.IsDeleted == false &&
            (string.IsNullOrEmpty(searchValue) ? true : a.FileName.ToLower().Contains(searchValue.ToLower())))
            .Select(a => new FileDataViewModel()
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSerial = a.FileSerial,
                ContentType = a.ContentType,
                Description = a.Description,
                DownloadCount = a.DownloadCount,
                Size = a.Size,
                CreatedOn = a.CreatedOn,
                LastModifiedOn = a.LastModifiedOn,
                LastDownloaded = a.LastDownloaded
            }).AsQueryable();

        return fileQuery;
    }

    public async Task<IActionResult> DownloadFile(string id)
    {
        try
        {
            var file = await _context.Files.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false && a.IsPrivate == false);

            if (file == null)
                return NotFound();

            file.DownloadCount++;
            file.LastDownloaded = DateTime.Now;

            _context.Update(file);
            _context.SaveChanges();

            Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
            Response.Headers.Add("Cache-Control", "no-cache");

            var filePath = Path.Combine(_env.WebRootPath, "Uploads", file.FileSerial);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/octet-stream", file.FileName);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
