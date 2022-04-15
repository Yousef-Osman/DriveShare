using DriveShare.Data;
using DriveShare.Models.Enums;
using DriveShare.Repositories.Interfaces;
using DriveShare.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DriveShare.Controllers;

public class HomeController : Controller
{
    private readonly IFileData _fileDataRepo;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HomeController> _logger;
    private readonly string SortColumn = "CreatedOn";

    public HomeController(IFileData fileDataRepo,
                          IWebHostEnvironment env,
                          ILogger<HomeController> logger)
    {
        _logger = logger;
        _env = env;
        _fileDataRepo = fileDataRepo;
    }

    public async Task<IActionResult> Index()
    {
        var files = await _fileDataRepo.GetLatestFilesAsync();
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
        var start = 0;
        var length = 10;

        var filesQuery = _fileDataRepo.GetAllFilesQuery(SortColumn, SortOrder.Descending, searchValue);
        var files = await _fileDataRepo.GetFilteredAsync(filesQuery, start, length);

        return View(files);
    }

    public async Task<IActionResult> BrowseFiles()
    {
        var start = 0;
        var length = 10;

        var filesQuery = _fileDataRepo.GetAllFilesQuery(SortColumn, SortOrder.Descending, null);
        var files = await _fileDataRepo.GetFilteredAsync(filesQuery, start, length);

        return View(files);
    }

    public async Task<IActionResult> DownloadFile(string id)
    {
        try
        {
            var file = await _fileDataRepo.DownloadFileAsync(id);

            if (file == null)
                return NotFound();

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
