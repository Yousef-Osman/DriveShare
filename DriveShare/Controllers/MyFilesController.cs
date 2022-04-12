using DriveShare.Data;
using DriveShare.Models;
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

    public IActionResult GetUploads([FromForm] int start, string[] parameters, [FromForm] int length = 10)
    {
        try
        {
            var searchValue = Request.Form["search[value]"];
            var columnOrder = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[string.Concat("columns[", columnOrder, "][name]")];
            var sortColumnDirection = Request.Form["order[0][dir]"];
            var status = Convert.ToInt32(parameters[0]);

            //Querying tax Documents in the database and searching for certain values if a search value is available
            //IQueryable<Tax_Document> dbDocuments = _kPE01Context.Tax_Documents
            //    .Where(a => (status == 0 ? true : (a.StatusId == status)) &&
            //    (string.IsNullOrEmpty(searchValue) ? true : a.DocumentNumber.Contains(searchValue)));

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

            //Sorting records for any given column
            //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            //    dbDocuments = dbDocuments.OrderBy(a => string.Concat(sortColumn, " ", sortColumnDirection));

            List<FileDataViewModel> records = files.Skip(start).Take(length).ToList();
            var totalCount = files.Count();

            //create document view model list and format documents accordind to it
            var data = new List<FileDataViewModel>();

            foreach (var file in records)
            {
                data.Add(file);
            }

            return Json(new
            {
                data = data,
                recordsFiltered = totalCount,
                recordsTotal = totalCount
            });
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    public async Task<IActionResult> GetFileData(string sortOrder)
    {
        ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DownloadSortParm"] = sortOrder == "download" ? "download_desc" : "download";

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

        switch (sortOrder)
        {
            case "name_desc":
                files = files.OrderByDescending(s => s.FileName);
                break;
            case "download":
                files = files.OrderBy(s => s.DownloadCount);
                break;
            case "download_desc":
                files = files.OrderByDescending(s => s.DownloadCount);
                break;
            default:
                files = files.OrderBy(s => s.FileName);
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