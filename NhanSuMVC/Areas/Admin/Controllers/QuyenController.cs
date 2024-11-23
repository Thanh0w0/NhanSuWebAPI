using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class QuyenController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<mvcQuyen> quyenList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/Quyens").Result;
            quyenList = response.Content.ReadAsAsync<IEnumerable<mvcQuyen>>().Result;
            return View(quyenList);
        }


        [HttpPost]
        public JsonResult Add(mvcQuyen quyen)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/Quyens", quyen).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/Quyens/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var quyen = response.Content.ReadAsAsync<mvcQuyen>().Result;
                return Json(quyen);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Update(mvcQuyen quyen)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/Quyens/" + quyen.MaQuyen, quyen).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/Quyens/" + id).Result;
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
