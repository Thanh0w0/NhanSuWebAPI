using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class TamUngController : Controller
    {
        public IActionResult Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;
            IEnumerable<mvcTamUng> chucVuList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync($"api/TamUngs/GetTamUngs?month={selectedMonth}&year={selectedYear}").Result;
            chucVuList = response.Content.ReadAsAsync<IEnumerable<mvcTamUng>>().Result;
            return View(chucVuList);
        }
        [HttpPost]
        public JsonResult Add(mvcTamUng tamUng)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/TamUngs", tamUng).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/TamUngs/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var tamUng = response.Content.ReadAsAsync<mvcTamUng>().Result;
                return Json(tamUng);
            }
            return Json(null);
        }

        [HttpPost]
        public JsonResult Update(mvcTamUng tamUng)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/TamUngs/" + tamUng.MaTU, tamUng).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/TamUngs/" + id).Result;
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
