using DriveShare.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriveShare.Controllers;

[Authorize]
public class MyFilesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult UploadFile()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadFile(UploadFileViewModel fileViewModel)
    {
        return View();
    }
}