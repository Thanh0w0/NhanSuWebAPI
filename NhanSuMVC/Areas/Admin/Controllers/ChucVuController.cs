using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class ChucVuController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<mvcChucVu> chucVuList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/ChucVus").Result;
            chucVuList = response.Content.ReadAsAsync<IEnumerable<mvcChucVu>>().Result;
            return View(chucVuList);
        }


        [HttpPost]
        public JsonResult Add(mvcChucVu chucVu)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/ChucVus", chucVu).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json("Thêm thành công");
                }
                else
                {
                    return Json("Thêm thất bại");
                }
            }
            return Json("Dữ liệu không hợp lệ");
        }


        public JsonResult Edit(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/ChucVus/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var chucVu = response.Content.ReadAsAsync<mvcChucVu>().Result;
                return Json(chucVu);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Update(mvcChucVu chucVu)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/ChucVus/" + chucVu.MaCV, chucVu).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json("Cập nhật thành công");
                }
                else
                {
                    return Json("Cập nhật thất bại");
                }
            }
            return Json("Dữ liệu không hợp lệ");
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/ChucVus/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                return Json("Xóa thành công");
            }
            else
            {
                return Json("Xóa thất bại");
            }
        }
    }
}
