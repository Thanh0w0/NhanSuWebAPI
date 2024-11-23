using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Claims;

namespace NhanSuMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản trị viên")]
    public class ChamCongController : Controller
    {

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int recordCount = 10, int pageIndex = 1)
        {
            IEnumerable<mvcChamCong> chamCongList = new List<mvcChamCong>();
            DateTime selectedFromDate = fromDate ?? DateTime.Today;
            DateTime selectedToDate = toDate ?? DateTime.Today;
            using (HttpResponseMessage response = await APIClient.WebApiClient.GetAsync(
                $"api/ChamCongs?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    chamCongList = await response.Content.ReadAsAsync<IEnumerable<mvcChamCong>>();
                }
            }

            // Tính tổng số bản ghi
            int totalRecords = chamCongList.Count();

            // Phân trang tại MVC Controller
            var pagedChamCongList = chamCongList
                .Skip((pageIndex - 1) * recordCount)
                .Take(recordCount)
                .ToList();

            // Thiết lập các thông số phân trang
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageIndex = pageIndex;
            ViewBag.RecordCount = recordCount;
            ViewBag.FromDate = selectedFromDate;
            ViewBag.ToDate = selectedToDate;

            return View(pagedChamCongList);
        }


        public async Task<IActionResult> TongCongThang(int? month, int? year)
        {
            // Sử dụng giá trị mặc định nếu month và year không được cung cấp
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;


            // Gọi API và truyền tháng và năm
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/ChamCongs/GetTongCongThang?month={selectedMonth}&year={selectedYear}");

            // Lưu tháng và năm vào ViewBag để duy trì trên giao diện
            ViewBag.Thang = selectedMonth;
            ViewBag.Nam = selectedYear;

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var summary = JsonConvert.DeserializeObject<IEnumerable<TongCongThangVM>>(jsonResponse);

                // Trả về view với dữ liệu đã nhận
                return View(summary);
            }

            // Nếu không thành công, trả về view với danh sách rỗng
            return View(new List<TongCongThangVM>());
        }






        [HttpPost]
        public JsonResult Add([FromBody] List<mvcTongHopCong> bangCong)
        {
            // Debug log trên server
            System.Diagnostics.Debug.WriteLine("Dữ liệu nhận được từ client: " + JsonConvert.SerializeObject(bangCong));

            if (bangCong == null || !bangCong.Any())
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            if (ModelState.IsValid)
            {
                // Gửi dữ liệu đến API
                HttpResponseMessage response = APIClient.WebApiClient.PostAsJsonAsync("api/ChamCongs/SaveTongHopCong", bangCong).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Lưu thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Lưu thất bại." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }





        [HttpGet]
        public async Task<IActionResult> ChiTietCongNV(string maNV, int? month, int? year, string tenNV)
        {
            if (string.IsNullOrEmpty(maNV) || !month.HasValue || !year.HasValue)
            {
                return BadRequest("Không có tham số");
            }

            // Construct the API endpoint URL
            string apiUrl = $"api/ChamCongs/GetChamCongNV?maNV={maNV}&month={month}&year={year}";

            // Call the API
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a list of ChamCong
                var chamCongDetails = await response.Content.ReadAsAsync<List<mvcChamCong>>();

                // Set the month and year in ViewBag for the view
                ViewBag.SelectedMonth = month.Value;
                ViewBag.SelectedYear = year.Value;
                ViewBag.TenNV = tenNV;
                ViewBag.MaNV = maNV;


                // Pass the data to the view
                return View(chamCongDetails);
            }
            else
            {
                // Handle errors (e.g., log it, return a specific view, etc.)
                ModelState.AddModelError(string.Empty, "Unable to fetch data from the API.");
                return View(new List<mvcChamCong>()); // Return an empty list if API call fails
            }
        }


        //Xuất tổng hợp công
        [HttpGet]
        public async Task<IActionResult> ExportExcelCongThang(int month, int year)
        {
            var response = await APIClient.WebApiClient.GetAsync($"api/ChamCongs/GetTongCongThang?month={month}&year={year}");
            if (response.IsSuccessStatusCode)
            {
                var bangtongCong = await response.Content.ReadFromJsonAsync<List<TongCongThangVM>>();

                return ExportToExcel(bangtongCong, month, year); // Call method to export Excel
            }

            TempData["Error"] = "Có lỗi khi xuất bảng lương.";
            return RedirectToAction("Index", new { month, year });
        }

        private IActionResult ExportToExcel(List<TongCongThangVM> data, int month, int year)
        {
            var username = @User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add($"Tong Cong Thang {month}/{year}");

                // Thêm tiêu đề chính
                worksheet.Cells[1, 1].Value = $"BẢNG TỔNG HỢP CÔNG THÁNG {month} NĂM {year} CÔNG TY CỔ PHẦN FTI SÀI GÒN";
                worksheet.Cells[1, 1, 1, 10].Merge = true; // Gộp ô từ cột 1 đến cột 10
                worksheet.Cells[1, 1].Style.Font.Bold = true; // Đặt kiểu chữ in đậm
                worksheet.Cells[1, 1].Style.Font.Size = 18; // Thay đổi kích thước chữ
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Căn giữa

                // Đặt tiêu đề cho các cột (dòng thứ 2)
                string[] headers = {
            "Mã NV", "Họ tên", "Phòng ban", "Số ngày công",
            "Số ngày nghỉ (không lương)", "Số ngày nghỉ (có lương)",
            "Tổng giờ làm thêm ngày thường", "Tổng giờ làm ngày nghỉ",
            "Tổng giờ làm ngày lễ", "Tổng giờ làm thực tế"
        };

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[2, col].Value = headers[col - 1];
                    worksheet.Cells[2, col].Style.Font.Bold = true;
                    worksheet.Cells[2, col].Style.Font.Size = 12;
                    worksheet.Cells[2, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[2, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue); // Tô màu header
                }

                // Thêm dữ liệu vào worksheet
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    worksheet.Cells[i + 3, 1].Value = item.MaNV;
                    worksheet.Cells[i + 3, 2].Value = item.TenNV;
                    worksheet.Cells[i + 3, 3].Value = item.PhongBan;
                    worksheet.Cells[i + 3, 4].Value = item.SoNgayCong;
                    worksheet.Cells[i + 3, 5].Value = item.SoNgayNghiKhongLuong;
                    worksheet.Cells[i + 3, 6].Value = item.SoNgayNghiCoLuong;
                    worksheet.Cells[i + 3, 7].Value = item.LamThemNgayThuong;
                    worksheet.Cells[i + 3, 8].Value = item.LamNgayNghi;
                    worksheet.Cells[i + 3, 9].Value = item.LamNgayLe;
                    worksheet.Cells[i + 3, 10].Value = item.TongGioLamThucTe;
                }

                // Thiết lập định dạng cho các cột
                worksheet.Cells.AutoFitColumns();

                // Thiết lập border cho toàn bộ bảng dữ liệu
                int totalRows = data.Count + 2; // Bao gồm cả header
                int totalColumns = headers.Length;
                var dataRange = worksheet.Cells[2, 1, totalRows, totalColumns];
                dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                // Thêm ngày lập ở góc dưới bên phải của bảng
                var ngayLapCell = worksheet.Cells[totalRows + 2, totalColumns - 1];
                ngayLapCell.Value = $"Ngày lập: {DateTime.Now:dd/MM/yyyy}";
                ngayLapCell.Style.Font.Italic = true;
                ngayLapCell.Style.Font.Size = 12;
                ngayLapCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[totalRows + 2, totalColumns - 1, totalRows + 2, totalColumns].Merge = true;

                // Thêm "Người lập" dưới "Ngày lập"
                var nguoiLapCell = worksheet.Cells[totalRows + 3, totalColumns - 1];
                nguoiLapCell.Value = "NGƯỜI LẬP:";
                nguoiLapCell.Style.Font.Italic = true;
                nguoiLapCell.Style.Font.Size = 12;
                nguoiLapCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[totalRows + 3, totalColumns - 1, totalRows + 3, totalColumns].Merge = true;

                // Thêm tên người lập ở dòng tiếp theo
                var tenNguoiLapCell = worksheet.Cells[totalRows + 4, totalColumns - 1];
                tenNguoiLapCell.Value = username; // Có thể thay thế bằng tên người lập nếu cần
                tenNguoiLapCell.Style.Font.Size = 12;
                tenNguoiLapCell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet.Cells[totalRows + 4, totalColumns - 1, totalRows + 4, totalColumns].Merge = true;

                // Trả về file Excel dưới dạng file download
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"TongCongThang_{month}_{year}.xlsx");
            }
        }


        //Xuất chi tiết công nhân viên

        [HttpGet]
        public async Task<IActionResult> ExportChiTietCongNV(string maNV, int month, int year)
        {
            // Fetch the data for the specified month and year
            string apiUrl = $"api/ChamCongs/GetChamCongNV?month={month}&year={year}&maNV={maNV}";
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Không thể lấy dữ liệu");
            }

            var chamCongDetails = await response.Content.ReadAsAsync<List<mvcChamCong>>();

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Chi Tiết Công");


                worksheet.Cells[2, 1].Value = "Mã nhân viên:";
                worksheet.Cells[2, 2].Value = maNV;
                worksheet.Cells[3, 1].Value = "Tháng/Năm:";
                worksheet.Cells[3, 2].Value = $"{month}/{year}";

                // Set the headers for the data table starting from row 5
                worksheet.Cells[5, 1].Value = "Ngày chấm công";
                worksheet.Cells[5, 2].Value = "Giờ vào";
                worksheet.Cells[5, 3].Value = "Giờ ra";
                worksheet.Cells[5, 4].Value = "Trạng thái";

                // Style the header row
                using (var range = worksheet.Cells[5, 1, 5, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Fill the worksheet with data, starting from row 6
                int row = 6;
                foreach (var item in chamCongDetails)
                {
                    worksheet.Cells[row, 1].Value = item.NgayChamCong.ToString("dd-MM-yyyy");

                    // Format ThoiGianVao and ThoiGianRa based on their data type
                    worksheet.Cells[row, 2].Value = item.ThoiGianVao is TimeSpan timeIn
                        ? timeIn.ToString(@"hh\:mm\:ss")
                        : item.ThoiGianVao.ToString();

                    worksheet.Cells[row, 3].Value = item.ThoiGianRa is TimeSpan timeOut
                        ? timeOut.ToString(@"hh\:mm\:ss")
                        : item.ThoiGianRa.ToString();

                    worksheet.Cells[row, 4].Value = item.TrangThaiCC;
                    row++;
                }

                // Set the content type and file name
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();
                string excelName = $"ChiTietCong_{maNV}_{month}_{year}.xlsx";
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }



    }
}
