using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;
using System.Net.Http.Headers;

namespace NhanSuMVC.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [Authorize(Roles = "Nhân viên")]
    public class NVChamCongController : Controller
    {
        public async Task<IActionResult> Index(int? month, int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;

            // Use the current month and year if not provided
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            List<mvcChamCong> chamCongList = new List<mvcChamCong>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = APIClient.WebApiClient.GetAsync($"api/ChamCongs/GetChamCongTheoNV?maNV={maNhanVien}&month={selectedMonth}&year={selectedYear}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    chamCongList = JsonConvert.DeserializeObject<List<mvcChamCong>>(jsonData);
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                var message = $"Thêm thất bại. Lỗi: {errorMessage}";
            }

           
            return View(chamCongList);
        }




        public async Task<IActionResult> TongCongThangNV(int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
            // Default year is the current year if no year is selected
            int selectedYear = year ?? DateTime.Now.Year;

            // Call the API to get the total work data for the selected employee and year
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/TongHopCongs/GetCongNamNV?maNV={maNhanVien}&nam={selectedYear}");

            ViewBag.Nam = selectedYear;

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var summary = JsonConvert.DeserializeObject<IEnumerable<mvcTongHopCong>>(jsonResponse);

                // Return the view with the data received
                return View(summary);
            }

            // If the request fails, return the view with an empty list
            return View(new List<mvcTongHopCong>());
        }



    }
}
