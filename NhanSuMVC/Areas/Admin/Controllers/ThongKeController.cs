using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models.ViewModels;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class ThongKeController : Controller
    {
        public IActionResult Index(int? thang, int? nam)
        {
            return View();
        }

        public IActionResult TKTheoPB(int? thang, int? nam)
        {
           
            thang ??= DateTime.Now.Month;
            nam ??= DateTime.Now.Year;

            
            var response = APIClient.WebApiClient.GetAsync($"api/ThongKes/ThongKeLuongTheoPhongBan?thang={thang}&nam={nam}").Result;

            
            if (response.IsSuccessStatusCode)
            {
                var tkLuongTheoPB = response.Content.ReadAsAsync<List<TKLuongTheoPBVM>>().Result; 
                ViewBag.Thang = thang;
                ViewBag.Nam = nam;
                return View(tkLuongTheoPB);
            }

            // Handle error case
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            return View(new List<TKLuongTheoPBVM>()); 
        }

        [HttpGet]
        public async Task<IActionResult> TKTheoNam(int nam)
        {
            // Call the API to get salary statistics for the selected year
            var tkLuongTheoNamResponse = await APIClient.WebApiClient.GetAsync($"api/ThongKes/TKLuongTheoNam?nam={nam}");

            if (tkLuongTheoNamResponse.IsSuccessStatusCode)
            {
                var tkLuongTheoNam = await tkLuongTheoNamResponse.Content.ReadAsAsync<IEnumerable<TKLuongTheoNamVM>>();

                // Pass the data to the view
                return View(tkLuongTheoNam);
            }

            // Handle errors or return an empty model
            return View(new List<TKLuongTheoNamVM>());
        }





    }
}
