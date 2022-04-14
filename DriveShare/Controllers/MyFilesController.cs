using DriveShare.Common;
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

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetUploads([FromForm] int start, string[] parameters, [FromForm] int length = 10)
    {
        try
        {
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var columnOrder = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumn = Request.Form[string.Concat("columns[", columnOrder, "][name]")].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var status = Convert.ToInt32(parameters[0]);

            //Main query to select all user active files
            var filesQuery = _context.Files.Where(a => a.UserId == GetUserId() && a.IsDeleted == false).AsQueryable();

            //Search query for certain values if a search value is available
            if (!string.IsNullOrEmpty(searchValue))
            {
                filesQuery = filesQuery.Where(a => a.FileName.ToLower().Contains(searchValue) ||
                                a.ContentType.ToLower().Contains(searchValue));
            }

            //Sorting records for any given column
            //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            //{
            //    filesQuery = filesQuery.OrderBy(a => string.Concat(sortColumn, " ", sortColumnDirection));
            //}

            var SelectQuery = filesQuery.Select(a => new FileDataViewModel()
            {
                Id = a.Id,
                FileName = a.FileName,
                Description = a.Description,
                FileSerial = a.FileSerial,
                ContentType = a.ContentType,
                DownloadCount = a.DownloadCount,
                Size = a.Size,
                CreatedOn = a.CreatedOn,
                LastModifiedOn = a.LastModifiedOn
            });

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                //System.Reflection.PropertyInfo prop = typeof(sortColumn).GetProperty("PropertyName");

                //query = query.OrderBy(x => prop.GetValue(x, null));

                if (sortColumnDirection == "asc")
                    SelectQuery = SelectQuery.OrderBy(a => a.GetType().GetProperty(sortColumn).GetValue(a, null));
                else
                    SelectQuery = SelectQuery.OrderByDescending(a => a.GetType().GetProperty(sortColumn).GetValue(a, null));
            }

            var aaa = SelectQuery.ToQueryString();

            var records = await SelectQuery.Skip(start).Take(length).ToListAsync();
            var totalCount = await SelectQuery.CountAsync();

            return Json(new
            {
                data = records,
                recordsFiltered = totalCount,
                recordsTotal = totalCount
            });
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
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

        return Json(new { RedirectUrl = "/MyFiles/Index" });
    }
}