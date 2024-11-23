using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NhanSuMVC.Models;
using System.Net.Http.Json;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class PhongBanController : Controller
    {


        public IActionResult Index()
        {
            IEnumerable<mvcPhongBan> phongBanList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/PhongBans").Result;
            phongBanList = response.Content.ReadAsAsync<IEnumerable<mvcPhongBan>>().Result;
            return View(phongBanList);
        }


        [HttpPost]
        [HttpPost]
        public JsonResult Add(mvcPhongBan phongBan)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/PhongBans", phongBan).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/PhongBans/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var phongBan = response.Content.ReadAsAsync<mvcPhongBan>().Result;
                return Json(phongBan);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult Update(mvcPhongBan phongBan)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/PhongBans/" + phongBan.MaPB, phongBan).Result;
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
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/PhongBans/" + id).Result;
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
