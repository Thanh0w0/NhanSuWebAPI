using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuMVC.Models;
using NhanSuMVC.Models.ViewModels;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;

namespace NhanSuMVC.Areas.NhanVien.Controllers
{
    [Area("NhanVien")]
    [Authorize(Roles = "Nhân viên")]
    public class NVLuongController : Controller
    {
        public async Task<IActionResult> Index(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            ViewBag.SelectedYear = selectedYear;

            // Initially, no data is loaded for the view
            return View(new List<LuongVM>());
        }


        public async Task<IActionResult> LayLuongNV(int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
            // Default year is the current year if no year is selected
            int selectedYear = year ?? DateTime.Now.Year;

            // Call the API to get the total work data for the selected employee and year
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/BangLuongs/GetLuongNVTheoNam?maNV={maNhanVien}&nam={selectedYear}");

            ViewBag.Nam = selectedYear;

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var summary = JsonConvert.DeserializeObject<IEnumerable<LuongVM>>(jsonResponse);

                // Return the view with the data received
                return PartialView("_BangLuongNV", summary);
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            var message = $"Thêm thất bại. Lỗi: {errorMessage}";
            // If the request fails, return the view with an empty list
            return PartialView("_BangLuongNV", new List<LuongVM>());
        }


        public async Task<IActionResult> TKLuongNVTheoNam(int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
            int selectedYear = year ?? DateTime.Now.Year;

            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/BangLuongs/GetLuongNVTheoNam?maNV={maNhanVien}&nam={selectedYear}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var summary = JsonConvert.DeserializeObject<IEnumerable<LuongVM>>(jsonResponse);

                return Json(summary); // Trả về JSON thay vì PartialView
            }
            return BadRequest("Không thể lấy dữ liệu lương");
        }

        public async Task<IActionResult> ExportExcel(int? year)
        {
            var maNhanVien = User.Claims.FirstOrDefault(c => c.Type == "MaNhanVien")?.Value;
            int selectedYear = year ?? DateTime.Now.Year;
            var username = @User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Gọi API để lấy dữ liệu bảng lương
            HttpResponseMessage response = await APIClient.WebApiClient.GetAsync($"api/BangLuongs/GetLuongNVTheoNam?maNV={maNhanVien}&nam={selectedYear}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Không thể lấy dữ liệu lương");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var luongData = JsonConvert.DeserializeObject<List<LuongVM>>(jsonResponse);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("BangLuong");

            // **Thiết lập tiêu đề bảng**
            string title = $"BẢNG LƯƠNG NHÂN VIÊN NĂM {selectedYear}";
            worksheet.Cells["A1:AA1"].Merge = true;
            worksheet.Cells["A1"].Value = title;
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 25;

            // **Thêm họ tên và mã nhân viên**
            worksheet.Cells["A2"].Value = "Họ tên:";
            worksheet.Cells["B2"].Value = username;
            worksheet.Cells["C2"].Value = "Mã NV:";
            worksheet.Cells["D2"].Value = maNhanVien;

            // Merge các ô để tránh mất chữ
            worksheet.Cells["B2:D2"].Merge = true;

            // **Tạo header cho các cột dữ liệu từ hàng 3**
            string[] headers = {
        "Tháng/Năm", "Lương Cơ Bản", "Ngày Công Thực Tế",
        "Phụ Cấp Ăn Trưa", "Phụ Cấp Xăng Xe", "Phụ Cấp Điện Thoại", "Phụ Cấp Khác",
        "Tổng Phụ Cấp", "Tăng Ca Thường", "Tăng Ca Lễ", "Tăng Ca Nghỉ",
        "Tổng Tiền Tăng Ca", "Tổng Thưởng", "Tổng Phạt",
        "BHXH NLD", "BHYT NLD", "BHTN NLD", "Tổng BH NLD",
        "BHXH DN", "BHYT DN", "BHTN DN", "Tổng BH DN",
        "Giảm Trừ Gia Cảnh", "Thuế TNCN", "Tổng Trừ", "Tổng Thu Nhập", "Lương Thực Lãnh"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = headers[i];
            }

            // Style cho header
            using (var range = worksheet.Cells["A3:AA3"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Thêm dữ liệu vào Excel từ hàng 4
            int row = 4;
            foreach (var item in luongData)
            {
                worksheet.Cells[row, 1].Value = $"{item.Thang}/{item.Nam}";
                worksheet.Cells[row, 2].Value = item.LuongCB;
                worksheet.Cells[row, 3].Value = item.NgayCongThucTe;
                worksheet.Cells[row, 4].Value = item.PhuCapAnTrua;
                worksheet.Cells[row, 5].Value = item.PhuCapXangXe;
                worksheet.Cells[row, 6].Value = item.PhuCapDienThoai;
                worksheet.Cells[row, 7].Value = item.PhuCapKhac;
                worksheet.Cells[row, 8].Value = item.TongPhuCap;
                worksheet.Cells[row, 9].Value = item.TongTCThuong;
                worksheet.Cells[row, 10].Value = item.TongTCLe;
                worksheet.Cells[row, 11].Value = item.TongTCNghi;
                worksheet.Cells[row, 12].Value = item.TongTienTC;
                worksheet.Cells[row, 13].Value = item.TongThuong;
                worksheet.Cells[row, 14].Value = item.TongPhat;
                worksheet.Cells[row, 15].Value = item.BHXH_NLD;
                worksheet.Cells[row, 16].Value = item.BHYT_NLD;
                worksheet.Cells[row, 17].Value = item.BHTN_NLD;
                worksheet.Cells[row, 18].Value = item.BHXH_NLD + item.BHYT_NLD + item.BHTN_NLD;
                worksheet.Cells[row, 19].Value = item.BHXH_DN;
                worksheet.Cells[row, 20].Value = item.BHYT_DN;
                worksheet.Cells[row, 21].Value = item.BHTN_DN;
                worksheet.Cells[row, 22].Value = item.BHXH_DN + item.BHYT_DN + item.BHTN_DN;
                worksheet.Cells[row, 23].Value = 11000000;
                worksheet.Cells[row, 24].Value = item.ThueTNCN;
                worksheet.Cells[row, 25].Value = item.TongTU;
                worksheet.Cells[row, 26].Value = item.TongThuNhap;
                worksheet.Cells[row, 27].Value = item.LuongThucLanh;
                row++;
            }

            // **Thêm ngày lập và tên người lập ở góc dưới bên phải**
            worksheet.Cells[$"Z{row + 1}"].Value = $"NGÀY TẠO: {DateTime.Now:dd/MM/yyyy}";
            worksheet.Cells[$"Z{row + 2}"].Value = "NGƯỜI TẠO";
            worksheet.Cells[$"Z{row + 3}"].Value = username;
            worksheet.Cells[$"Z{row + 1}:Z{row + 3}"].Style.Font.Bold = true;
            worksheet.Cells[$"Z{row + 1}:Z{row + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // **Thêm border cho toàn bộ bảng**
            using (var range = worksheet.Cells[$"A3:AA{row - 1}"])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            // Điều chỉnh kích thước cột cho phù hợp với nội dung
            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"BangLuong_{maNhanVien}_{selectedYear}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


    }
}
