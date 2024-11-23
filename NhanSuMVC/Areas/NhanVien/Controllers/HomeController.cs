using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [Authorize(Roles = "Nhân viên")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var employeeName = HttpContext.Session.GetString("TenNhanVien");
            var imageUrl = HttpContext.Session.GetString("AnhNhanVien");

            // Truyền vào ViewData
            ViewData["EmployeeName"] = employeeName;
            ViewData["EmployeeImage"] = !string.IsNullOrEmpty(imageUrl) ? imageUrl : "/images/default-avatar.png";

            return View();
        }


       
        
    }
}
