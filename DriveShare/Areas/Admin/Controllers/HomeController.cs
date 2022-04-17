using Microsoft.AspNetCore.Mvc;

namespace DriveShare.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
