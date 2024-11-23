using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NhanSuMVC.Models.ViewModels;
using System.Security.Claims;
using System.Text;

namespace NhanSuMVC.Controllers
{
    public class DangNhapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(DangNhapVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Send login information (including plain password)
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await APIClient.WebApiClient.PostAsync("api/DangNhaps/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserVM>(responseData);

                if (user != null)
                {
                    // Save user info in Claims if login is successful
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenNV),
                new Claim("MaNhanVien", user.MaNV),
                new Claim(ClaimTypes.Role, user.Quyen),
                new Claim("AnhNhanVien", user.HinhAnh ?? "")
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect based on user role
                    if (user.Quyen == "Quản trị viên")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (user.Quyen == "Nhân viên")
                    {
                        return RedirectToAction("Index", "Home", new { area = "NhanVien" });
                    }
                }
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View("Index", model);
        }





        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "DangNhap");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
