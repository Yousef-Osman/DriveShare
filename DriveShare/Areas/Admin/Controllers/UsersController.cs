using DriveShare.Areas.Admin.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DriveShare.Areas.Admin.Controllers;
public class UsersController : AdminBaseController
{
    private readonly IUser _userRepo;

    public UsersController(IUser userRepo)
    {
        _userRepo = userRepo;
    }
    public async Task<IActionResult> Index()
    {
        var users = await _userRepo.GetUsersAsync(0);

        return View(users);
    }
}
