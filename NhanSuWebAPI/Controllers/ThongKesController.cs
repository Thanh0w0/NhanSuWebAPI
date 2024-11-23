using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanSuWebAPI.Models.Context;
using NhanSuWebAPI.Models.VMs;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKesController : ControllerBase
    {

        private readonly AppDBContext _context;

        public ThongKesController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("ThongKeLuongTheoPhongBan")]
        public async Task<ActionResult<IEnumerable<ThongKeLuongPhongBanVM>>> ThongKeLuongTheoPhongBan(int thang, int nam)
        {
            var thangNam = $"{thang:D2}/{nam}"; // Chuyển định dạng thành MM/yyyy

            // Thống kê lương theo phòng ban
            var thongKe = await _context.BangLuongs
                .Where(bl => bl.Thang == thang && bl.Nam == nam) // Lọc theo tháng và năm
                .Include(bl => bl.NhanVien)
                .ThenInclude(nv => nv.PhongBan)
                .GroupBy(bl => bl.NhanVien.PhongBan.TenPB)
                .Select(g => new ThongKeLuongPhongBanVM
                {
                    TenPB = g.Key,
                    TongLuongThucLanh = g.Sum(bl => bl.LuongThucLanh),  // Tổng lương thực lãnh
                })
                .ToListAsync();

            return Ok(thongKe);
        }


        [HttpGet("TKLuongTheoNam")]
        public async Task<ActionResult<IEnumerable<TKLuongTheoNamVM>>> TKLuongTheoNam(int nam)
        {
            // Lấy dữ liệu lương theo tháng cho năm được chỉ định
            var luongTheoThang = await _context.BangLuongs
                .Where(bl => bl.Nam == nam) // Lọc theo năm
                .GroupBy(bl => bl.Thang)
                .Select(g => new TKLuongTheoNamVM
                {
                    ThangNam = $"{g.Key:D2}/{nam}", // Định dạng lại tháng/năm
                    TongLuongThucLanh = g.Sum(bl => bl.LuongThucLanh),  // Tổng lương thực lãnh
                })
                .ToListAsync();

            // Tạo danh sách tháng từ 1 đến 12
            var danhSachThang = Enumerable.Range(1, 12).Select(month =>
            {
                var monthString = $"{month:D2}/{nam}";
                return new TKLuongTheoNamVM
                {
                    ThangNam = monthString,
                    TongLuongThucLanh = luongTheoThang.FirstOrDefault(x => x.ThangNam == monthString)?.TongLuongThucLanh ?? 0,
                };
            }).ToList();

            return Ok(danhSachThang);
        }


        [HttpGet("ThongKeChamCong")]
        public async Task<ActionResult<IEnumerable<TKChamCongVM>>> ThongKeChamCong(int thang, int nam, int maPB)
        {
            var result = await (from chamCong in _context.ChamCongs
                                join nhanVien in _context.NhanViens on chamCong.MaNV equals nhanVien.MaNV
                                where chamCong.NgayChamCong.Month == thang
                                      && chamCong.NgayChamCong.Year == nam
                                      && nhanVien.MaPB == maPB // Thêm điều kiện cho phòng ban
                                group chamCong by nhanVien.PhongBan.TenPB into g // Nhóm theo tên phòng ban
                                select new TKChamCongVM
                                {
                                    TenPB = g.Key,
                                    SoNhanVien = g.Count(), // Tổng số nhân viên có chấm công
                                    SoDiTre = g.Count(cc => cc.TrangThaiCC == "Đi Trễ"), // Số nhân viên đi trễ
                                    SoDiDungGio = g.Count(cc => cc.TrangThaiCC == "Đúng Giờ"), // Số nhân viên đến đúng giờ
                                }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("ThongKe")]
        public ActionResult<HomeAdminVM> GetThongKe()
        {
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // 1. Thống kê số lượng nhân viên đang làm việc
            int nvDangLam = _context.NhanViens.Count(nv => nv.TrangThai == "Đang Làm Việc");

            // 2. Thống kê số lượng nhân viên đã nghỉ
            int nvDaNghi = _context.NhanViens.Count(nv => nv.TrangThai == "Đã Nghỉ");

            // 3. Thống kê số lượng ngày nghỉ lễ trong tháng hiện tại
            var ngayLeThangHienTai = _context.NgayLes
                .Where(nl =>
                    (nl.NgayBD <= lastDayOfMonth && nl.NgayKT >= firstDayOfMonth))
                .ToList();

            // Tính tổng số ngày nghỉ lễ trong tháng hiện tại
            int nghiLeThangHienTai = 0;
            foreach (var ngayLe in ngayLeThangHienTai)
            {
                // Tính số ngày trong khoảng thời gian trùng với tháng hiện tại
                var start = ngayLe.NgayBD < firstDayOfMonth ? firstDayOfMonth : ngayLe.NgayBD;
                var end = ngayLe.NgayKT > lastDayOfMonth ? lastDayOfMonth : ngayLe.NgayKT;
                nghiLeThangHienTai += (end - start).Days + 1;
            }

            // 4. Thống kê số lượng nhân viên trong các phòng ban
            var slNhanVienPB = _context.PhongBans
                .Select(pb => new NhanVienTheoPBVM
                {
                    TenPhongBan = pb.TenPB,
                    SoLuongNhanVien = _context.NhanViens.Count(nv => nv.MaPB == pb.MaPB && nv.TrangThai == "Đang Làm Việc")
                }).ToList();

            // 5. Thống kê số lượng nhân viên trong các chức vụ
            var slNhanVienCV = _context.ChucVus
                .Select(cv => new NhanVienTheoCVVM
                {
                    TenChucVu = cv.TenCV,
                    SoLuongNhanVien = _context.NhanViens.Count(nv => nv.MaCV == cv.MaCV && nv.TrangThai == "Đang Làm Việc")
                }).ToList();

            // 6. Thống kê số lượng đơn nghỉ phép chưa duyệt
            var nghiPhepChuaDuyetThangHienTai = _context.NghiPheps
                .Where(np =>
                    np.TrangThai == "Chờ duyệt" &&
                    (
                        (np.TuNgay <= lastDayOfMonth && np.TuNgay >= firstDayOfMonth) ||
                        (np.DenNgay <= lastDayOfMonth && np.DenNgay >= firstDayOfMonth) ||
                        (np.TuNgay <= firstDayOfMonth && np.TuNgay >= lastDayOfMonth)
                    )
                ).ToList();

            // Đếm số lượng đơn nghỉ phép chưa duyệt
            int npChuaDuyet = nghiPhepChuaDuyetThangHienTai.Count;


            // Tạo đối tượng thống kê
            var homeAdminVM = new HomeAdminVM
            {
                NVDangLam = nvDangLam,
                NVDaNghi = nvDaNghi,
                NPChuaDuyet = npChuaDuyet,
                NghiLeThangHienTai = nghiLeThangHienTai,
                SLNhanVienPB = slNhanVienPB,
                SLNhanVienCV = slNhanVienCV
            };

            return Ok(homeAdminVM);
        }


    }
}
