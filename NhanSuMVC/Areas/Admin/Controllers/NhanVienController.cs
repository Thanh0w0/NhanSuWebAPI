using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;
using System.Text;

namespace NhanSuMVC.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class NhanVienController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public NhanVienController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<mvcNhanVien> nhanVienList;
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/NhanViens").Result;
            nhanVienList = response.Content.ReadAsAsync<IEnumerable<mvcNhanVien>>().Result;
            PrepareTaiKhoan();
            return View(nhanVienList);
        }

        //Create nhân viên
        public async Task<IActionResult> Create()
        {
            // Lấy Mã Nhân Viên từ API và lưu vào Session
            var maNVResponse = await APIClient.WebApiClient.GetAsync("api/NhanViens/TaoMaNV");
            var maNV = (await maNVResponse.Content.ReadAsStringAsync()).Trim('"');
            HttpContext.Session.SetString("MaNV", maNV);
            PrepareNhanVien();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNhanVien([FromForm] mvcNhanVien nhanVien, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (ModelState.IsValid)
            {
                // Xử lý hình ảnh
                if (nhanVien.ImageUpload != null && nhanVien.ImageUpload.Length > 0)
                {
                    // Tạo tên file ảnh mới
                    string fileName = Path.GetFileNameWithoutExtension(nhanVien.ImageUpload.FileName);
                    string extension = Path.GetExtension(nhanVien.ImageUpload.FileName);
                    fileName = $"{fileName}_{DateTime.Now:yyyyMMdd}{extension}";

                    // Đường dẫn lưu ảnh
                    string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    string fullPath = Path.Combine(uploadPath, fileName);

                    // Lưu ảnh vào thư mục
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await nhanVien.ImageUpload.CopyToAsync(stream);
                    }

                    // Gán đường dẫn ảnh vào thuộc tính HinhAnh
                    nhanVien.HinhAnh = "/images/" + fileName;
                }

                // Xóa thuộc tính ImageUpload trước khi gửi đến API
                nhanVien.ImageUpload = null;

                // Gửi yêu cầu POST đến API để thêm nhân viên
                HttpResponseMessage response = await APIClient.WebApiClient.PostAsJsonAsync("api/NhanViens", nhanVien);

                if (response.IsSuccessStatusCode)
                {
                    // Thành công: chuyển hướng về Index và thông báo
                    TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    // Lỗi: chuyển hướng về Index và thông báo lỗi
                    TempData["ErrorMessage"] = $"Thêm thất bại. Lỗi: {errorMessage}";
                    return RedirectToAction("Index");
                }
            }

            // Nếu dữ liệu không hợp lệ: chuyển hướng về Index và thông báo lỗi
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
            return RedirectToAction("Index");
        }



        // Preapare data
        public async Task PrepareNhanVien()
        {
          

            // Kiểm tra và lưu dữ liệu Phòng Ban vào Session
            var phongBansData = HttpContext.Session.GetString("PhongBans");
            List<SelectListItem> phongBans;

            if (!string.IsNullOrEmpty(phongBansData))
            {
                // Deserialize dữ liệu từ Session
                phongBans = JsonConvert.DeserializeObject<List<SelectListItem>>(phongBansData);
            }
            else
            {
                // Lấy dữ liệu từ API nếu không có trong Session
                var phongBanList = await GetPhongBanList();

                // Chuyển đổi danh sách phong ban thành SelectListItem
                phongBans = phongBanList.Select(pb => new SelectListItem
                {
                    Value = pb.MaPB.ToString(),
                    Text = pb.TenPB
                }).ToList();

                // Lưu vào Session
                HttpContext.Session.SetString("PhongBans", JsonConvert.SerializeObject(phongBans));
            }

            ViewBag.PhongBans = phongBans != null && phongBans.Any()
                ? new SelectList(phongBans, "Value", "Text")
                : new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

            // Kiểm tra và lưu dữ liệu Chức Vụ vào Session
            var chucVusData = HttpContext.Session.GetString("ChucVus");
            List<SelectListItem> chucVus;

            if (!string.IsNullOrEmpty(chucVusData))
            {
                chucVus = JsonConvert.DeserializeObject<List<SelectListItem>>(chucVusData);
            }
            else
            {
                var chucVuList = await GetChucVuList();

                chucVus = chucVuList.Select(cv => new SelectListItem
                {
                    Value = cv.MaCV.ToString(),
                    Text = cv.TenCV
                }).ToList();

                HttpContext.Session.SetString("ChucVus", JsonConvert.SerializeObject(chucVus));
            }

            ViewBag.ChucVus = chucVus != null && chucVus.Any()
                ? new SelectList(chucVus, "Value", "Text")
                : new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");

            // Kiểm tra và lưu dữ liệu Trạng Thái vào Session
            var trangThaisData = HttpContext.Session.GetString("TrangThais");
            List<SelectListItem> trangThais;

            if (!string.IsNullOrEmpty(trangThaisData))
            {
                trangThais = JsonConvert.DeserializeObject<List<SelectListItem>>(trangThaisData);
            }
            else
            {
                trangThais = new List<SelectListItem>
        {
            new SelectListItem { Text = "Đang làm việc", Value = "Đang Làm Việc" },
            new SelectListItem { Text = "Đã nghỉ", Value = "Đã nghỉ" }
        };

                HttpContext.Session.SetString("TrangThais", JsonConvert.SerializeObject(trangThais));
            }

            ViewBag.TrangThais = trangThais;

            // Kiểm tra và lưu dữ liệu Giới Tính vào Session
            var gioiTinhsData = HttpContext.Session.GetString("GioiTinhs");
            List<SelectListItem> gioiTinhs;

            if (!string.IsNullOrEmpty(gioiTinhsData))
            {
                gioiTinhs = JsonConvert.DeserializeObject<List<SelectListItem>>(gioiTinhsData);
            }
            else
            {
                gioiTinhs = new List<SelectListItem>
        {
            new SelectListItem { Text = "Nam", Value = "Nam" },
            new SelectListItem { Text = "Nữ", Value = "Nữ" }
        };

                HttpContext.Session.SetString("GioiTinhs", JsonConvert.SerializeObject(gioiTinhs));
            }

            ViewBag.GioiTinhs = gioiTinhs;

            // Kiểm tra và lưu dữ liệu Trình Độ vào Session
            var trinhDosData = HttpContext.Session.GetString("TrinhDos");
            List<SelectListItem> trinhDos;

            if (!string.IsNullOrEmpty(trinhDosData))
            {
                trinhDos = JsonConvert.DeserializeObject<List<SelectListItem>>(trinhDosData);
            }
            else
            {
                trinhDos = new List<SelectListItem>
        {
            new SelectListItem { Text = "12/12", Value = "12/12" },
            new SelectListItem { Text = "Trung Cấp", Value = "Trung Cấp" },
            new SelectListItem { Text = "Cao Đẳng", Value = "Cao Đẳng" },
            new SelectListItem { Text = "Cử Nhân", Value = "Cử Nhân" }
        };

                HttpContext.Session.SetString("TrinhDos", JsonConvert.SerializeObject(trinhDos));
            }

            ViewBag.TrinhDos = trinhDos;
        }


        public async Task PrepareTaiKhoan()
        {
            var loaiTK = GetQuyenList();
            ViewBag.Quyens = new SelectList(loaiTK, "MaQuyen", "TenQuyen");

        }

        public async Task PrepareHopDong()
        {
            // Kiểm tra Session cho danh sách LoaiHopDong
            var loaiHDData = HttpContext.Session.GetString("LoaiHopDongs");
            List<SelectListItem> loaiHDs;

            if (!string.IsNullOrEmpty(loaiHDData))
            {
                // Deserialize dữ liệu từ Session
                loaiHDs = JsonConvert.DeserializeObject<List<SelectListItem>>(loaiHDData);
            }
            else
            {
                // Lấy dữ liệu từ API nếu không có trong Session
                var loaiHDList = await GetLoaiHDList();

                // Chuyển đổi danh sách loại hợp đồng thành SelectListItem
                loaiHDs = loaiHDList.Select(lhd => new SelectListItem
                {
                    Value = lhd.MaLoaiHD.ToString(),
                    Text = lhd.TenLoaiHD
                }).ToList();

                // Lưu vào Session
                HttpContext.Session.SetString("LoaiHopDongs", JsonConvert.SerializeObject(loaiHDs));
            }

            // Gán ViewBag.LoaiHDs từ dữ liệu đã có
            ViewBag.LoaiHDs = loaiHDs != null && loaiHDs.Any()
                ? new SelectList(loaiHDs, "Value", "Text")
                : new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
        }

        



        // Get data
        private async Task<IEnumerable<mvcPhongBan>> GetPhongBanList()
        {
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync("api/PhongBans");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<mvcPhongBan>>();
            }
            return Enumerable.Empty<mvcPhongBan>();
        }

        private async Task<IEnumerable<mvcChucVu>> GetChucVuList()
        {
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync("api/ChucVus");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<mvcChucVu>>();
            }
            return Enumerable.Empty<mvcChucVu>();
        }

        private async Task<IEnumerable<mvcLoaiHD>> GetLoaiHDList()
        {
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync("api/LoaiHDs");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<mvcLoaiHD>>();
            }
            return Enumerable.Empty<mvcLoaiHD>();
        }

        private async Task<IEnumerable<mvcLoaiPC>> GetLoaiPCList()
        {
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync("api/LoaiPCs");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<mvcLoaiPC>>();
            }
            return Enumerable.Empty<mvcLoaiPC>();
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





        //Add

        [HttpPost]
        public IActionResult AddTaiKhoan(mvcTaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(taiKhoan.MatKhau);

                // Cập nhật mật khẩu đã mã hóa vào đối tượng tài khoản
                taiKhoan.MatKhau = hashedPassword;

                // Lưu vào cơ sở dữ liệu
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/TaiKhoans", taiKhoan).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm thất bại" });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        [HttpPost]
        public IActionResult AddHopDong(mvcHopDong hopDong)
        {

            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/HopDongs", hopDong).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm thất bại" });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        [HttpPost]
        public IActionResult AddPhuCap(mvcPhuCap phuCap)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/PhuCaps", phuCap).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm thành công" });
                }
                else
                {
                    var errorResponse = response.Content.ReadAsStringAsync().Result; // Get detailed error from the API
                    return Json(new { success = false, message = "Thêm thất bại", details = errorResponse });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }




        //Edit
        public IActionResult Edit(string id)
        {
            // Fetch employee data from the API
            var nhanVienResponse = APIClient.WebApiClient.GetAsync("api/NhanViens/GetNhanVienByMaNV/" + id).Result;
            var hopDongResponse = APIClient.WebApiClient.GetAsync("api/HopDongs/GetNhanVienByMaNV/" + id).Result;
            var taiKhoanResponse = APIClient.WebApiClient.GetAsync("api/TaiKhoans/GetNhanVienByMaNV/" + id).Result;
            var phuCapResponse = APIClient.WebApiClient.GetAsync("api/PhuCaps/GetNhanVienByMaNV/" + id).Result;

            // Check if nhanVienResponse is successful
            if (!nhanVienResponse.IsSuccessStatusCode)
            {
                // Handle error for NhanVien
                return View("Error", "Không tìm thấy thông tin nhân viên.");
            }

            // Deserialize nhanVien
            var nhanVien = nhanVienResponse.Content.ReadAsAsync<mvcNhanVien>().Result;

            // Prepare additional data (dropdowns, etc.)
            PrepareNhanVien();
            PrepareHopDong();
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




        //Edit Nhân viên

        public JsonResult GetNhanVienByMaNV(string id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/NhanViens/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var nhanVien = response.Content.ReadAsAsync<mvcNhanVien>().Result;
                return Json(nhanVien);
            }
            return Json(null);
        }


        [HttpPost]
        public JsonResult EditNhanVien([FromForm] mvcNhanVien nhanVien, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy thông tin nhân viên hiện tại từ API để lấy đường dẫn ảnh cũ
                    var existingEmployeeResponse = APIClient.WebApiClient.GetAsync($"api/NhanViens/{nhanVien.MaNV}").Result;
                    if (!existingEmployeeResponse.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên!" });
                    }

                    // Lấy thông tin nhân viên hiện tại
                    var existingEmployee = existingEmployeeResponse.Content.ReadAsAsync<mvcNhanVien>().Result;
                    var oldImagePath = existingEmployee.HinhAnh;

                    // Kiểm tra xem có upload hình mới không
                    if (nhanVien.ImageUpload != null && nhanVien.ImageUpload.Length > 0)
                    {
                        // Tạo tên file duy nhất
                        string fileName = Path.GetFileNameWithoutExtension(nhanVien.ImageUpload.FileName);
                        string extension = Path.GetExtension(nhanVien.ImageUpload.FileName);
                        fileName = $"{fileName}_{DateTime.Now:yyyyMMdd}{extension}";

                        // Đường dẫn vật lý đến thư mục "images" để lưu file
                        string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Đường dẫn lưu file đầy đủ
                        string fullPath = Path.Combine(uploadPath, fileName);

                        // Cập nhật đường dẫn ảnh mới vào thuộc tính HinhAnh
                        nhanVien.HinhAnh = $"/images/{fileName}";

                        // Lưu file ảnh vào thư mục "images"
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            nhanVien.ImageUpload.CopyTo(fileStream);
                        }

                        // Xóa ảnh cũ nếu tồn tại và khác ảnh mới
                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != nhanVien.HinhAnh)
                        {
                            string oldImageFullPath = Path.Combine(webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImageFullPath))
                            {
                                System.IO.File.Delete(oldImageFullPath);
                            }
                        }
                    }
                    else
                    {
                        // Nếu không upload hình mới, giữ nguyên đường dẫn hình cũ
                        nhanVien.HinhAnh = oldImagePath;
                    }

                    // Gửi yêu cầu PUT tới API để cập nhật thông tin nhân viên
                    var response = APIClient.WebApiClient.PutAsJsonAsync($"api/NhanViens/{nhanVien.MaNV}", nhanVien).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Cập nhật thành công!" });
                    }
                    else
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        return Json(new { success = false, message = $"Cập nhật thất bại! {content}" });
                    }
                }
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi xử lý: {ex.Message}" });
            }
        }






        //Edit Hợp đồng
        public JsonResult GetHopDongById(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/HopDongs/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var hopDong = response.Content.ReadAsAsync<mvcHopDong>().Result;
                return Json(hopDong);
            }
            return Json(null);
        }

        public JsonResult EditHopDong(mvcHopDong hopDong)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/HopDongs/" + hopDong.MaHD, hopDong).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }
            }
            return Json("Dữ liệu không hợp lệ");
        }


        //Edit Tài khoản
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


        //Edit Phụ cấp
        public JsonResult GetPhuCap(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.GetAsync("api/PhuCaps/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var phuCap = response.Content.ReadAsAsync<mvcPhuCap>().Result;
                return Json(phuCap);
            }
            return Json(null);
        }

        public JsonResult UpdatePhuCap(mvcPhuCap phuCap)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = APIClient.WebApiClient.PutAsJsonAsync("api/PhuCaps/" + phuCap.MaPC, phuCap).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại" });
                }
            }
            return Json("Dữ liệu không hợp lệ");
        }



        //Delete
        [HttpPost]
        public JsonResult DeleteHopDong(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/HopDongs/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xóa thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Xóa thất bại" });
            }
        }



        [HttpPost]
        public JsonResult DeletePhuCap(int id)
        {
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/PhuCaps/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xóa thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Xóa thất bại" });
            }
        }


        [HttpPost]
        public JsonResult DeleteNhanVien(string MaNV)
        {
            HttpResponseMessage response = APIClient.WebApiClient.DeleteAsync("api/NhanViens/" + MaNV).Result;
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Xóa thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Xóa thất bại" });
            }
        }
    }
}
