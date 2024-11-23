using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanSuWebAPI.Models;
using NhanSuWebAPI.Models.Context;
using NhanSuWebAPI.Models.VMs;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ChamCongsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ChamCongs

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChamCong>>> GetChamCongs([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (_context.ChamCongs == null)
            {
                return NotFound();
            }

            // Khởi tạo truy vấn chấm công
            var chamCongsQuery = _context.ChamCongs
                             .Include(cc => cc.NhanVien)
                             .ThenInclude(nv => nv.PhongBan)
                             .Where(cc => cc.NgayChamCong >= fromDate && cc.NgayChamCong <= toDate)
                             .OrderBy(cc => cc.NgayChamCong);



            // Lấy dữ liệu từ cơ sở dữ liệu
            var chamCongs = await chamCongsQuery.ToListAsync();

            return chamCongs;
        }




        //Lấy danh sách chấm công nhân viên (phía client)
        [HttpGet("GetChamCongTheoNV")]
        public async Task<ActionResult<IEnumerable<ChamCong>>> GetChamCongTheoNV(string maNV, int month, int year)
        {
            if (_context.ChamCongs == null)
            {
                return NotFound();
            }

            var fromDate = new DateTime(year, month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1); 

        
            var chamCongsQuery = _context.ChamCongs
                              .Include(cc => cc.NhanVien)
                              .ThenInclude(nv => nv.PhongBan)
                              .Where(cc => cc.MaNV == maNV &&
                                           cc.NgayChamCong >= fromDate &&
                                           cc.NgayChamCong <= toDate)
                              .OrderBy(cc => cc.NgayChamCong);

            // Execute the query and return the results
            var chamCongs = await chamCongsQuery.ToListAsync();

            return chamCongs;
        }




        [HttpGet("GetTongCongThang")]
        public async Task<ActionResult<IEnumerable<TongHopCongVM>>> GetTongCongThang(int month, int year)
        {
            DateTime fromDate = new DateTime(year, month, 1); // Ngày đầu tháng
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1); // Ngày cuối tháng

            // Lấy dữ liệu chấm công trong khoảng thời gian tháng yêu cầu
            var chamCongData = await _context.ChamCongs
                .Where(cc => cc.NgayChamCong >= fromDate && cc.NgayChamCong <= toDate)
                .Select(cc => new
                {
                    cc.MaNV,
                    cc.NhanVien.HoTen,
                    PhongBan = cc.NhanVien.PhongBan.TenPB,
                    cc.NgayChamCong,
                    cc.TrangThaiCC,
                    cc.SoGioLam,
                    cc.SoGioTC,
                    cc.ThoiGianVao,
                    cc.ThoiGianRa
                })
                .ToListAsync();

            // Nhóm và tính tổng hợp các thông tin chấm công theo nhân viên
            var summary = chamCongData
                .GroupBy(cc => new { cc.MaNV, cc.HoTen, cc.PhongBan })
                .Select(g => new TongHopCongVM
                {
                    MaNV = g.Key.MaNV,
                    TenNV = g.Key.HoTen,
                    PhongBan = g.Key.PhongBan,
                    SoNgayCong = g.Count(),
                    // Tính số ngày nghỉ không lương trong tháng
                    SoNgayNghiKhongLuong = _context.NghiPheps
                        .Where(np => np.MaNV == g.Key.MaNV &&
                                     np.TrangThai == "Đã duyệt" &&
                                     np.LoaiNP == "Không lương" &&
                                     np.TuNgay <= toDate && // Kiểm tra ngày bắt đầu trong tháng
                                     np.DenNgay >= fromDate) // Kiểm tra ngày kết thúc trong tháng
                        .ToList()  // Lấy tất cả các bản ghi để tính trên phía client
                        .Sum(np =>
                        {
                            // Tính số ngày nghỉ từ TuNgay đến DenNgay trong tháng
                            DateTime startDate = np.TuNgay < fromDate ? fromDate : np.TuNgay;
                            DateTime endDate = np.DenNgay > toDate ? toDate : np.DenNgay;
                            return (endDate - startDate).Days + 1;
                        }),
                    // Tính số ngày nghỉ có lương trong tháng
                    SoNgayNghiCoLuong = _context.NghiPheps
                        .Where(np => np.MaNV == g.Key.MaNV &&
                                     np.TrangThai == "Đã duyệt" &&
                                     np.LoaiNP == "Có lương" &&
                                     np.TuNgay <= toDate && // Kiểm tra ngày bắt đầu trong tháng
                                     np.DenNgay >= fromDate) // Kiểm tra ngày kết thúc trong tháng
                        .ToList()  // Lấy tất cả các bản ghi để tính trên phía client
                        .Sum(np =>
                        {
                            // Tính số ngày nghỉ từ TuNgay đến DenNgay trong tháng
                            DateTime startDate = np.TuNgay < fromDate ? fromDate : np.TuNgay;
                            DateTime endDate = np.DenNgay > toDate ? toDate : np.DenNgay;
                            return (endDate - startDate).Days + 1;
                        }),
                    // Tính số giờ làm thêm trong ngày thường (Thứ 2 - Thứ 6)
                    // Tính tổng số giờ tăng ca trong ngày thường (Thứ 2 - Thứ 6)
                    LamThemNgayThuong = g
                        .Where(cc => cc.TrangThaiCC == "Đi làm" &&
                                    cc.NgayChamCong.DayOfWeek >= DayOfWeek.Monday &&
                                    cc.NgayChamCong.DayOfWeek <= DayOfWeek.Friday)
                        .Sum(cc => cc.SoGioTC),  // Trực tiếp lấy tổng số giờ tăng ca từ trường SoGioTC



                    // Tính số giờ làm vào ngày lễ
                    LamNgayLe = (float)g.Where(cc => cc.TrangThaiCC == "Nghỉ Lễ")
                         .Sum(cc => cc.ThoiGianVao.HasValue && cc.ThoiGianRa.HasValue
                             ? (cc.ThoiGianRa.Value - cc.ThoiGianVao.Value).TotalHours
                             : cc.SoGioLam),
                    // Tính số giờ làm vào ngày nghỉ cuối tuần (Thứ 7 - Chủ Nhật)
                    LamNgayNghi = g.Where(cc => cc.TrangThaiCC == "Đi làm" &&
                                                 (cc.NgayChamCong.DayOfWeek == DayOfWeek.Saturday ||
                                                  cc.NgayChamCong.DayOfWeek == DayOfWeek.Sunday))
                                   .Sum(cc => cc.SoGioLam),
                    // Tính tổng số giờ làm thực tế trong tháng
                    TongGioLamThucTe = g.Sum(cc => cc.SoGioLam)
                })
                .ToList();

            return Ok(summary);
        }






        [HttpPost("SaveTongHopCong")]
        public async Task<ActionResult> SaveTongHopCong([FromBody] List<TongHopCong> danhSachTongHopCong)
        {
            // Kiểm tra danh sách công có rỗng hay không
            if (danhSachTongHopCong == null || !danhSachTongHopCong.Any())
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            foreach (var tongHopCong in danhSachTongHopCong)
            {
                // Kiểm tra xem bản ghi đã tồn tại hay chưa (dựa trên MaNV, Thang, Nam)
                var existingRecord = await _context.TongHopCongs
                    .FirstOrDefaultAsync(thc => thc.MaNV == tongHopCong.MaNV &&
                                                thc.Thang == tongHopCong.Thang &&
                                                thc.Nam == tongHopCong.Nam);

                if (existingRecord != null)
                {
                    // Nếu tồn tại, cập nhật bản ghi
                    existingRecord.SoNgayCong = tongHopCong.SoNgayCong;
                    existingRecord.SoNgayNghiKhongLuong = tongHopCong.SoNgayNghiKhongLuong;
                    existingRecord.SoNgayNghiCoLuong = tongHopCong.SoNgayNghiCoLuong;
                    existingRecord.LamThemNgayThuong = tongHopCong.LamThemNgayThuong;
                    existingRecord.LamNgayLe = tongHopCong.LamNgayLe;
                    existingRecord.LamNgayNghi = tongHopCong.LamNgayNghi;
                    existingRecord.TongGioLamThucTe = tongHopCong.TongGioLamThucTe;
                }
                else
                {
                    // Nếu không tồn tại, thêm mới bản ghi
                    await _context.TongHopCongs.AddAsync(tongHopCong);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Trả về kết quả thành công
            return Ok("Lưu dữ liệu thành công.");
        }





        //Lấy chi tiết chấm công theo tháng năm, mã nhân viên
        [HttpGet("GetChamCongNV")]
        public IActionResult GetChamCongNV(string maNV, int month, int year)
        {
            
            // Query to fetch attendance records directly from the ChamCong table
            var chamCongDetails = _context.ChamCongs
                .Where(cc => cc.MaNV == maNV &&
                             cc.NgayChamCong.Month == month &&
                             cc.NgayChamCong.Year == year)
                .ToList();

            // Check if any records were found
            if (!chamCongDetails.Any())
            {
                return NotFound("Không có dữ liệu chấm công.");
            }

            // Return the records as JSON
            return Ok(chamCongDetails);
        }

    }
}
