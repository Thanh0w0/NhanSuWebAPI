using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class NghiPhepController : Controller
    {
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            IEnumerable<mvcNghiPhep> nghiPhepVMList = new List<mvcNghiPhep>();

            // Thiết lập giá trị mặc định nếu không có ngày được chọn
            DateTime selectedFromDate = fromDate ?? DateTime.Today;
            DateTime selectedToDate = toDate ?? DateTime.Today;

            // Gọi API để lấy danh sách nghỉ phép trong khoảng thời gian đã chọn
            using (HttpResponseMessage response = await APIClient.WebApiClient.GetAsync(
                $"api/NghiPheps?fromDate={selectedFromDate:yyyy-MM-dd}&toDate={selectedToDate:yyyy-MM-dd}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    nghiPhepVMList = await response.Content.ReadAsAsync<IEnumerable<mvcNghiPhep>>();
                }
            }

            ViewBag.FromDate = selectedFromDate;
            ViewBag.ToDate = selectedToDate;

            // Trả về View với danh sách NghiPhepVM
            return View(nghiPhepVMList);
        }


        public IActionResult Edit(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/NghiPheps/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var nghiPhep = response.Content.ReadAsAsync<mvcNghiPhep>().Result;
                return View(nghiPhep);
            }
            return View(null);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateNP(int id, string trangThai, string loaiNP, string maNV)
        {
            // Tạo object chứa dữ liệu gửi tới API
            var updateData = new
            {
                TrangThai = trangThai,
                LoaiNP = loaiNP,
                MaNV = maNV
            };

            try
            {
                // Gọi API với phương thức POST
                HttpResponseMessage response = await APIClient.WebApiClient.PostAsJsonAsync($"api/NghiPheps/CapNhatTrangThai/{id}", updateData);

                var fullUrl = $"api/NghiPhep/CapNhatTrangThai/{id}";
                Console.WriteLine("URL gọi API: " + fullUrl);
                // Kiểm tra phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message = "Cập nhật trạng thái nghỉ phép thành công." });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Cập nhật trạng thái thất bại" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }




    }
}
