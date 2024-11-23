using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;
using OfficeOpenXml;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class BangLuongController : Controller
    {
        public async Task<IActionResult> Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;

            // Initially, no data is loaded for the view
            return View(new List<LuongVM>());
        }

        public async Task<IActionResult> TinhLuong(int month, int year)
        {
            var response = await APIClient.WebApiClient.GetAsync($"api/BangLuongs/TinhLuong?thang={month}&nam={year}");
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var summary = JsonConvert.DeserializeObject<IEnumerable<LuongVM>>(jsonResponse);
                return PartialView("_BangLuongPartialView", summary);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            var message = $"Thêm thất bại. Lỗi: {errorMessage}";
            return PartialView("_BangLuongPartialView", new List<LuongVM>());
        }




        [HttpPost]
        public async Task<IActionResult> LuuLuong(int month, int year, [FromBody] List<mvcBangLuong> bangLuong)
        {
            Console.WriteLine($"Tháng: {month}, Năm: {year}");
            Console.WriteLine($"Số lượng bản ghi lương: {bangLuong?.Count}");
            try
            {
                var response = await APIClient.WebApiClient.PostAsJsonAsync($"api/BangLuongs/SaveBangLuong?thang={month}&nam={year}", bangLuong);

                if (response.IsSuccessStatusCode)
                {
                    return Json("Lưu thành công");
                }
                else
                {
                    return Json("Lưu thất bại");
                }
            }
            catch (Exception ex)
            {
                return Json("Có lỗi: " + ex.Message);
            }
            
        }




        public async Task<ActionResult> ExportExcel(int month, int year)
        {
            try
            {
                var response = await APIClient.WebApiClient.GetAsync($"api/BangLuongs/TinhLuong?thang={month}&nam={year}");
                var username = @User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi khi gọi API: {response.ReasonPhrase}");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var model = JsonConvert.DeserializeObject<List<LuongVM>>(responseString);

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Bảng Lương");

                       

                        // Tiêu đề bảng
                        worksheet.Cells[4, 1].Value = $"BẢNG LƯƠNG THÁNG {month} NĂM {year} CÔNG TY CỔ PHẦN FTI SÀI GÒN";
                        worksheet.Cells[4, 1].Style.Font.Bold = true;
                        worksheet.Cells[4, 1].Style.Font.Size = 14;
                        worksheet.Cells[4, 1, 4, 25].Merge = true;
                        worksheet.Cells[4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[4, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        // Header
                        string[] headers = { "STT", "Mã NV", "Họ tên", "Lương cơ bản", "Ngày công thực tế", "Phụ cấp ăn trưa", "Phụ cấp xăng xe",
                                     "Phụ cấp điện thoại", "Phụ cấp khác", "Tổng phụ cấp", "Tổng thưởng", "Tổng phạt",
                                     "BHXH NLĐ", "BHYT NLĐ", "BHTN NLĐ", "Tổng khấu trừ NLĐ",
                                     "BHXH Công ty", "BHYT Công ty", "BHTN Công ty", "Tổng bảo hiểm công ty",
                                     "Giảm trừ bản thân", "Thuế TNCN", "Tạm ứng", "Lương thực lãnh" };

                        for (int col = 1; col <= headers.Length; col++)
                        {
                            worksheet.Cells[5, col].Value = headers[col - 1];
                            worksheet.Cells[5, col].Style.Font.Bold = true;
                            worksheet.Cells[5, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[5, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Tô màu xanh header
                        }

                        // Tự động điều chỉnh kích thước cột
                        worksheet.Cells[5, 1, 5, 24].AutoFitColumns();

                        // Dữ liệu
                        int row = 6;
                        decimal[] totals = new decimal[24];

                        foreach (var item in model)
                        {
                            worksheet.Cells[row, 1].Value = row - 5;
                            worksheet.Cells[row, 2].Value = item.MaNV;
                            worksheet.Cells[row, 3].Value = item.HoTen;
                            worksheet.Cells[row, 4].Value = item.LuongCB;
                            worksheet.Cells[row, 5].Value = item.NgayCongThucTe;
                            worksheet.Cells[row, 6].Value = item.PhuCapAnTrua;
                            worksheet.Cells[row, 7].Value = item.PhuCapXangXe;
                            worksheet.Cells[row, 8].Value = item.PhuCapDienThoai;
                            worksheet.Cells[row, 9].Value = item.PhuCapKhac;
                            worksheet.Cells[row, 10].Value = item.TongPhuCap;
                            worksheet.Cells[row, 11].Value = item.TongThuong;
                            worksheet.Cells[row, 12].Value = item.TongPhat;
                            worksheet.Cells[row, 13].Value = item.BHXH_NLD;
                            worksheet.Cells[row, 14].Value = item.BHYT_NLD;
                            worksheet.Cells[row, 15].Value = item.BHTN_NLD;
                            worksheet.Cells[row, 16].Value = item.BHXH_NLD + item.BHYT_NLD + item.BHTN_NLD;
                            worksheet.Cells[row, 17].Value = item.BHXH_DN;
                            worksheet.Cells[row, 18].Value = item.BHYT_DN;
                            worksheet.Cells[row, 19].Value = item.BHTN_DN;
                            worksheet.Cells[row, 20].Value = item.BHXH_DN + item.BHYT_DN + item.BHTN_DN;
                            worksheet.Cells[row, 21].Value = 11000000;
                            worksheet.Cells[row, 22].Value = item.ThueTNCN;
                            worksheet.Cells[row, 23].Value = item.TongTU;
                            worksheet.Cells[row, 24].Value = item.LuongThucLanh;

                            // Tính tổng
                            for (int i = 3; i <= 24; i++)
                            {
                                if (decimal.TryParse(worksheet.Cells[row, i].Text, out decimal value))
                                {
                                    totals[i - 1] += value;
                                }
                            }

                            row++;
                        }

                        // Hàng tổng
                        worksheet.Cells[row, 1].Value = "Tổng";
                        for (int i = 4; i <= 24; i++)
                        {
                            worksheet.Cells[row, i].Value = totals[i - 1].ToString("N0");
                        }
                        worksheet.Cells[row, 1, row, 24].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, 24].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                        // Ngày lập và người lập
                        // Ngày lập và người lập
                        worksheet.Cells[row + 2, 1].Value = "NGƯỜI LẬP";
                        worksheet.Cells[row + 2, 1].Style.Font.Bold = true;
                        worksheet.Cells[row + 2, 1].Style.Font.Size = 14;

                      

                        // Thêm Username người lập ở hàng dưới "Người lập" (cách 2 hàng)
                        worksheet.Cells[row + 4, 1].Value = username;
                        worksheet.Cells[row + 4, 1].Style.Font.Italic = true;
                        worksheet.Cells[row + 4, 1].Style.Font.Size = 14;
                        worksheet.Cells[row + 4, 1, row + 4, 3].Merge = true;
                        worksheet.Cells[row + 4, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        // Thiết lập Border cho toàn bộ khung bảng
                        var dataRange = worksheet.Cells[5, 1, row, 24];
                        dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                        // Tăng độ rộng của cột họ tên
                        worksheet.Column(3).Width = 20; // Tăng độ rộng của cột họ tên



                        var stream = new MemoryStream(package.GetAsByteArray());
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BangLuong.xlsx");
                    }
                }
                else
                {
                    throw new Exception("Dữ liệu không phải là JSON.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }





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

        }


        private async Task<IEnumerable<mvcPhongBan>> GetPhongBanList()
        {
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync("api/PhongBans");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<mvcPhongBan>>();
            }
            return Enumerable.Empty<mvcPhongBan>();
        }

       
        

    }
}
