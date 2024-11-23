using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class NgayLeController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<mvcNgayLe> ngayLeList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/NgayLes").Result;

            if (response.IsSuccessStatusCode)
            {
                ngayLeList = response.Content.ReadAsAsync<IEnumerable<mvcNgayLe>>().Result;

                // Biến đổi dữ liệu thành định dạng mà FullCalendar yêu cầu
                var holidays = ngayLeList.Select(n => new
                {
                    Title = n.TenNgayLe,
                    Start = n.NgayBD.ToString("yyyy-MM-dd"), // Định dạng ngày tháng theo yêu cầu của FullCalendar
                    End = n.NgayKT.ToString("yyyy-MM-dd")
                }).ToList();

                ViewBag.Holidays = holidays; // Truyền danh sách ngày lễ vào ViewBag
            }
            else
            {
                ViewBag.Holidays = new List<object>(); // Trả về danh sách rỗng nếu có lỗi
            }

            return View(); // Trả về view Index
        }




        [HttpPost]
        public async Task<JsonResult> Add(mvcNgayLe ngayLe)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Gọi API để thêm ngày nghỉ lễ
                    HttpResponseMessage response = await APIClient.WebApiClient.PostAsJsonAsync("api/NgayLes", ngayLe);

                    // Kiểm tra trạng thái thành công
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Thêm thành công!" });
                    }
                    else
                    {
                        // Lấy thông báo lỗi từ phản hồi của API (nếu có)
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = $"Thêm thất bại. Lỗi: {errorMessage}" });
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có ngoại lệ
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

    }
}
