// Đường dẫn: Web_ThoiTrang/Areas/Admin/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Web_ThoiTrang.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được truy cập vào khu vực này
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    } 
 }