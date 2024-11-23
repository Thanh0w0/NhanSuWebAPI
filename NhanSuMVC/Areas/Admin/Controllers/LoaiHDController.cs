using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;

namespace NhanSuMVC.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class LoaiHDController : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<mvcLoaiHD> loaiHDList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/LoaiHDs").Result;
            loaiHDList = response.Content.ReadAsAsync<IEnumerable<mvcLoaiHD>>().Result;
            return View(loaiHDList);
        }


        [HttpPost]
        public JsonResult Add(mvcLoaiHD loaiHD)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/LoaiHDs", loaiHD).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/LoaiHDs/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var loaiHD = response.Content.ReadAsAsync<mvcLoaiHD>().Result;
                return Json(loaiHD);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Update(mvcLoaiHD loaiHD)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/LoaiHDs/" + loaiHD.MaLoaiHD, loaiHD).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/LoaiHDs/" + id).Result;
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
