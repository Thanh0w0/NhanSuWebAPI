using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class SaoLuuController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        // Action này sẽ được gọi khi nhấn vào nút Sao Lưu
        [HttpPost]
        public IActionResult SaoLuu(string backupType)
        {
            try
            {
                // Gọi API sao lưu dữ liệu với tham số backupType
                HttpResponseMessage response = APIClient.WebApiClient.PostAsync($"api/Backup/backup-data?backupType={backupType}", null).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Nếu sao lưu thành công, hiển thị thông báo cho người dùng
                    return Json(new { success = true, message = "Sao lưu thành công!" });
                }
                else
                {
                    // Nếu không thành công, thông báo lỗi
                    return Json(new { success = false, message = "Sao lưu thất bại." });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu API không thể gọi
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }


    }
}
