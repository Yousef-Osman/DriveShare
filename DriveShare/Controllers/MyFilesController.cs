using DriveShare.Data;
using DriveShare.Models;
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

    public MyFilesController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
            CreatedOn = a.CreatedOn.ToString("dd-MMM-yyyy HH:mm"),
            LastModifiedOn = a.LastModifiedOn.HasValue ? a.LastModifiedOn.Value.ToString("dd-MMM-yyyy HH:mm") : " - "
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

    public async Task<IActionResult> DeleteFile(string id)
    {
        var image = await _context.Files.FindAsync(id);
        if (image == null || image.UserId == GetUserId())
            return NotFound();

        try
        {
            image.IsDeleted = true;
            image.DeletedOn = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            //send error view here
        }

        return RedirectToAction(nameof(Index));
    }
}
