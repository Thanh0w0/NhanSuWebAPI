using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [Authorize(Roles = "Nhân viên")]
    public class NghiPhepNVController : Controller
    {
        public async Task<IActionResult> Index(int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
            ViewBag.MaNhanVien = maNhanVien;

            // Gọi API để lấy thông tin nghỉ phép
            var responseTK = await APIClient.WebApiClient.GetAsync($"api/NghiPheps/GetTKNghiPhep?maNV={maNhanVien}");

            if (responseTK.IsSuccessStatusCode)
            {
                var leaveStatistics = await responseTK.Content.ReadFromJsonAsync<TKNghiPhepDto>();
                ViewBag.LeaveStatistics = leaveStatistics;
            }
            else
            {
                ViewBag.LeaveStatistics = null; // Hoặc xử lý lỗi khác
            }
            int selectedYear = year ?? DateTime.Now.Year;
            var responseList = await APIClient.WebApiClient.GetAsync($"api/NghiPheps/GetListNghiPhepNV?maNV={maNhanVien}&nam={selectedYear}");

            if (responseList.IsSuccessStatusCode)
            {
                var lsNghiPhep = await responseList.Content.ReadFromJsonAsync<IEnumerable<mvcNghiPhep>>();
                return View(lsNghiPhep); // Trả về lịch sử nghỉ phép cho view
            }
            else
            {
                var errorContent = await responseList.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Không thể lấy danh sách nghỉ phép. Lỗi: {errorContent}");
                return View(new List<mvcNghiPhep>()); // Trả về danh sách rỗng
            }
        }





        [HttpPost]
        public IActionResult Create([FromBody] mvcNghiPhep nghiPhep)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/NghiPheps", nghiPhep).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Đăng ký nghỉ phép thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Đăng ký nghỉ phép thất bại." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }



    }

    public class TKNghiPhepDto
    {
        public int TongSoNgayNghiCoLuongTheoQuyDinh { get; set; }
        public int TongSoNgayNghiCoLuongDaNghi { get; set; }
        public int TongSoNgayNghiKhongLuongDaNghi { get; set; }
        public int SoNgayNghiCoLuongConLai { get; set; }
    }
}
