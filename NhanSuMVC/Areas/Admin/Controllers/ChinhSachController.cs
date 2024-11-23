using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class ChinhSachController : Controller
    {
        public async Task<IActionResult> Index(int? month, int? year)
        {
            // Xử lý giá trị tháng và năm chọn
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            // Lấy danh sách chính sách từ API, có lọc theo tháng và năm
            IEnumerable<mvcChinhSach> chinhSachList;
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/ChinhSachs/GetChinhSachs?month={selectedMonth}&year={selectedYear}");

            if (response.IsSuccessStatusCode)
            {
                // Đọc dữ liệu từ API và chuyển thành danh sách mvcChinhSach
                chinhSachList = await response.Content.ReadAsAsync<IEnumerable<mvcChinhSach>>();
            }
            else
            {
                // Xử lý khi có lỗi từ API
                chinhSachList = new List<mvcChinhSach>();
                // Bạn có thể thêm thông báo lỗi cho người dùng ở đây
            }

            return View(chinhSachList);
        }


        [HttpPost]
        public JsonResult Add(mvcChinhSach chinhSach)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/ChinhSachs", chinhSach).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm thất bại." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        public JsonResult Edit(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/ChinhSachs/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var chinhSach = response.Content.ReadAsAsync<mvcChinhSach>().Result;
                return Json(chinhSach);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Update(mvcChinhSach chinhsach)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/ChinhSachs/" + chinhsach.MaCS, chinhsach).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm thất bại." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/ChinhSachs/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xoá thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Xoá thất bại." });
            }
        }
    }


}
