using DriveShare.Data;
using DriveShare.Helpers;
using DriveShare.Models;
using DriveShare.Models.Enums;
using DriveShare.Repositories.Interfaces;
using DriveShare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DriveShare.Controllers;

[Authorize]
public class MyFilesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IFileData _fileDataRepo;

    public MyFilesController(ApplicationDbContext context, IWebHostEnvironment env, IFileData fileDataRepo)
    {
        _context = context;
        _env = env;
        _fileDataRepo = fileDataRepo;
    }

    public async Task<IActionResult> GetFileData(string sortOrder)
    {
        //flip the viewData value for the next click
        ViewData["UploadDateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "uploadDate" : "";
        ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
        ViewData["DownloadSortParm"] = sortOrder == "download" ? "download_desc" : "download";

        ViewData["UploadDateSortIcon"] = "";
        ViewData["NameSortIcon"] = "";
        ViewData["DownloadSortIcon"] = "";

        var files = _context.Files.Where(a => a.UserId == GetUserId() && a.IsDeleted == false).Select(a => new FileDataViewModel()
        {
            Id = a.Id,
            FileName = a.FileName,
            FileSerial = a.FileSerial,
            ContentType = a.ContentType,
            DownloadCount = a.DownloadCount,
            Size = a.Size,
            CreatedOn = a.CreatedOn,
            LastModifiedOn = a.LastModifiedOn
        }).AsQueryable();

        var sortModel = new SortModel();

        switch (sortOrder)
        {
            case "name":
                files = files.OrderBy(s => s.FileName);
                //sortModel.SortedProperty = "FileName";
                //sortModel.SortedOrder = SortOrder.Ascending;
                ViewData["NameSortIcon"] = "fa-long-arrow-alt-down";
                break;
            case "name_desc":
                files = files.OrderByDescending(s => s.FileName);
                //sortModel.SortedProperty = "FileName";
                //sortModel.SortedOrder = SortOrder.Descending;
                ViewData["NameSortIcon"] = "fa-long-arrow-alt-up";
                break;
            case "download":
                files = files.OrderBy(s => s.DownloadCount);
                //sortModel.SortedProperty = "DownloadCount";
                //sortModel.SortedOrder = SortOrder.Ascending;
                ViewData["DownloadSortIcon"] = "fa-long-arrow-alt-down";
                break;
            case "download_desc":
                files = files.OrderByDescending(s => s.DownloadCount);
                //sortModel.SortedProperty = "DownloadCount";
                //sortModel.SortedOrder = SortOrder.Descending;
                ViewData["DownloadSortIcon"] = "fa-long-arrow-alt-up";
                break;
            case "uploadDate":
                files = files.OrderBy(s => s.CreatedOn);
                //sortModel.SortedProperty = "CreatedOn";
                //sortModel.SortedOrder = SortOrder.Ascending;
                ViewData["UploadDateSortIcon"] = "fa-long-arrow-alt-down";
                break;
            default: //default is order by uploadDate desc
                files = files.OrderByDescending(s => s.CreatedOn);
                //sortModel.SortedProperty = "CreatedOn";
                //sortModel.SortedOrder = SortOrder.Descending;
                ViewData["UploadDateSortIcon"] = "fa-long-arrow-alt-up";
                break;
        }

        return View(nameof(Index) ,await files.ToListAsync());
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var uploads = await _context.Files.Where(a => a.UserId == userId && a.IsDeleted == false).Select(a => new FileDataViewModel()
        {
            Id = a.Id,
            FileName = a.FileName,
            FileSerial = a.FileSerial,
            ContentType = a.ContentType,
            DownloadCount = a.DownloadCount,
            Size = a.Size,
            CreatedOn = a.CreatedOn,
            LastModifiedOn = a.LastModifiedOn
            //CreatedOn = a.CreatedOn.ToString("dd-MMM-yyyy HH:mm"),
            //LastModifiedOn = a.LastModifiedOn.HasValue ? a.LastModifiedOn.Value.ToString("dd-MMM-yyyy HH:mm") : " - "
        }).ToListAsync();

        return View(uploads);
    }



    [HttpGet]
    public IActionResult UploadFile()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(UploadFileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
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
                UserId = GetUserId(),
                Size = model.File.Length
            };

            await _context.Files.AddAsync(upload);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An error occurred while saving the File, Please Try Again.");
        }

        return View(model);
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
        return userId;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromBody] FileData file)
    {

        var image = await _context.Files.FindAsync(file.Id);
        if (image == null || image.UserId != GetUserId())
            return NotFound();

        try
        {
            image.IsDeleted = true;
            image.DeletedOn = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }     

        return Json(new { RedirectUrl = "/MyFiles/Index"});
    }
}