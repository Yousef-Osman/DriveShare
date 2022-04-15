using DriveShare.Models;
using DriveShare.Models.Enums;
using DriveShare.Repositories.Interfaces;
using DriveShare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DriveShare.Controllers;

[Authorize]
public class MyFilesController : Controller
{
    private readonly IFileData _fileDataRepo;

    public MyFilesController(IFileData fileDataRepo)
    {
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
            var sortDirectionPrm = Request.Form["order[0][dir]"].FirstOrDefault();
            SortOrder sortDirection = (sortDirectionPrm == "asc") ? SortOrder.Ascending : SortOrder.Descending;

            //Main query to select all user active files
            var filesQuery = _fileDataRepo.GetUserFilesQuery(GetUserId());

            //Search query for certain values if a search value is available
            if (!string.IsNullOrEmpty(searchValue))
            {
                filesQuery = _fileDataRepo.GetSearchQuery(filesQuery, searchValue);
            }

            //Select required columns for the viewModel
            var query = _fileDataRepo.GetSelectQuery(filesQuery);

            //If sort column is not defined, sort by creation date (desc)
            query = _fileDataRepo.GetSortQuery(query, sortColumn, sortDirection);

            var records = await _fileDataRepo.GetFilteredAsync(query, start, length);
            var totalCount = await _fileDataRepo.GetCountAsync(query);

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
            await _fileDataRepo.CreateAsync(model, GetUserId());
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
    public async Task<IActionResult> DeleteFile([FromBody] FileData model)
    {
        try
        {
            if (!await _fileDataRepo.DeleteAsync(model.Id, GetUserId()))
                return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Json(new { RedirectUrl = "/MyFiles/Index" });
    }
}
