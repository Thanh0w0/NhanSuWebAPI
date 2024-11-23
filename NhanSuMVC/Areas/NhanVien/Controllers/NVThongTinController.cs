using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;

namespace NhanSuMVC.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [Authorize(Roles = "Nhân viên")]
    public class NVThongTinController : Controller
    {
        public IActionResult Index()
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;

            var nhanVienResponse = APIClient.WebApiClient.GetAsync("api/NhanViens/GetNhanVienByMaNV/" + maNhanVien).Result;
            var hopDongResponse = APIClient.WebApiClient.GetAsync("api/HopDongs/GetNhanVienByMaNV/" + maNhanVien).Result;
            var taiKhoanResponse = APIClient.WebApiClient.GetAsync("api/TaiKhoans/GetNhanVienByMaNV/" + maNhanVien).Result;
            var phuCapResponse = APIClient.WebApiClient.GetAsync("api/PhuCaps/GetNhanVienByMaNV/" + maNhanVien).Result;

            // Check if nhanVienResponse is successful
            if (!nhanVienResponse.IsSuccessStatusCode)
            {
                // Handle error for NhanVien
                return View("Error", "Không tìm thấy thông tin nhân viên.");
            }

            // Deserialize nhanVien
            var nhanVien = nhanVienResponse.Content.ReadAsAsync<mvcNhanVien>().Result;


            //PrepareTaiKhoan();
            PrepareTaiKhoan();
            // Initialize hopDong and taiKhoan
            List<mvcHopDong> hopDong = new List<mvcHopDong>();
            mvcTaiKhoan taiKhoan = null; // Changed to a single object
            List<mvcPhuCap> phuCap = new List<mvcPhuCap>();

            // Check hopDongResponse
            if (hopDongResponse.IsSuccessStatusCode)
            {
                hopDong = hopDongResponse.Content.ReadAsAsync<List<mvcHopDong>>().Result;
            }

            // Check taiKhoanResponse
            if (taiKhoanResponse.IsSuccessStatusCode)
            {
                taiKhoan = taiKhoanResponse.Content.ReadAsAsync<mvcTaiKhoan>().Result;
            }

            // Check phuCapResponse
            if (phuCapResponse.IsSuccessStatusCode)
            {
                phuCap = phuCapResponse.Content.ReadAsAsync<List<mvcPhuCap>>().Result;
            }
            // Create the ViewModel
            var nhanVienVM = new NhanVienVM
            {
                NhanVien = nhanVien,
                HopDong = hopDong,
                TaiKhoan = taiKhoan, // Use single object
                PhuCap = phuCap
            };



            return View(nhanVienVM);
        }

        public async Task PrepareTaiKhoan()
        {
            var loaiTK = GetQuyenList();
            ViewBag.Quyens = new SelectList(loaiTK, "MaQuyen", "TenQuyen");

        }

        public IEnumerable<mvcQuyen> GetQuyenList()
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/Quyens").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<mvcQuyen>>().Result;
            }
            return new List<mvcQuyen>();
        }

        public JsonResult EditTaiKhoan(mvcTaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before sending to API
                if (!string.IsNullOrEmpty(taiKhoan.MatKhau))
                {
                    taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(taiKhoan.MatKhau);
                }

                // Call the API to update the account
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/TaiKhoans/" + taiKhoan.MaTK, taiKhoan).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

    }
}
