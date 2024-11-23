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
    public class BangLuongsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BangLuongsController(AppDBContext context)
        {
            _context = context;
        }


        [HttpGet("TinhLuong")]
        public async Task<IActionResult> TinhLuong(int thang, int nam)
        {
            var danhSachNhanVien = await _context.NhanViens.ToListAsync();

            // Tính số ngày công chuẩn dựa trên số ngày trong tháng
            int soNgayCongChuan = 0;
            int soNgayTrongThang = DateTime.DaysInMonth(nam, thang);

            if (soNgayTrongThang == 28 || soNgayTrongThang == 29) // Tháng 2
            {
                soNgayCongChuan = 20;
            }
            else if (soNgayTrongThang == 30) // Tháng 4, 6, 9, 11
            {
                soNgayCongChuan = 22;
            }
            else if (soNgayTrongThang == 31) // Tháng 1, 3, 5, 7, 8, 10, 12
            {
                soNgayCongChuan = 23;
            }

            var ketQuaLuong = new List<LuongNhanVienVM>();

            foreach (var nhanVien in danhSachNhanVien)
            {
                var hopDong = await _context.HopDongs
                            .Where(h => h.MaNV == nhanVien.MaNV &&
                                        (h.NgayVaoLam.Year < nam ||
                                         (h.NgayVaoLam.Year == nam && h.NgayVaoLam.Month <= thang)) &&
                                        (h.NgayKetThuc.Year > nam ||
                                         (h.NgayKetThuc.Year == nam && h.NgayKetThuc.Month >= thang)))
                            .FirstOrDefaultAsync();

                if (hopDong == null)
                    continue;

                decimal luongCB = hopDong.LuongCB;
                luongCB = Math.Round(luongCB, 0);
                var danhSachPhuCap = await _context.PhuCaps
                    .Where(pc => pc.MaNV == nhanVien.MaNV &&
                                 pc.NgayBatDau.Year <= nam && pc.NgayKetThuc.Year >= nam &&
                                 pc.NgayBatDau.Month <= thang && pc.NgayKetThuc.Month >= thang)
                    .ToListAsync();

                decimal phuCapAnTrua = danhSachPhuCap
    .Where(pc => pc.LoaiPC == "Phụ cấp ăn trưa")
    .Sum(pc => pc.SoTien);

                decimal phuCapXangXe = danhSachPhuCap
                    .Where(pc => pc.LoaiPC == "Phụ cấp xăng xe")
                    .Sum(pc => pc.SoTien);

                decimal phuCapDienThoai = danhSachPhuCap
                    .Where(pc => pc.LoaiPC == "Phụ cấp điện thoại")
                    .Sum(pc => pc.SoTien);

                decimal phuCapKhac = danhSachPhuCap
                    .Where(pc => pc.LoaiPC != "Phụ cấp ăn trưa" && pc.LoaiPC != "Phụ cấp xăng xe" && pc.LoaiPC != "Phụ cấp điện thoại")
                    .Sum(pc => pc.SoTien);

                // Tính tổng tất cả các phụ cấp
                decimal tongPhuCap = danhSachPhuCap.Sum(pc => pc.SoTien);


                // Rounding the amounts to the nearest whole number
                phuCapAnTrua = Math.Round(phuCapAnTrua, 0);
                phuCapXangXe = Math.Round(phuCapXangXe, 0);
                phuCapDienThoai = Math.Round(phuCapDienThoai, 0);
                phuCapKhac = Math.Round(phuCapKhac, 0);
                tongPhuCap = Math.Round(tongPhuCap, 0);


                // Lấy danh sách thưởng theo mã nhân viên
                var thongKeThuong = await _context.ChinhSachs
                    .Where(cs => cs.LoaiCS == "Khen thưởng" && cs.MaNV == nhanVien.MaNV &&
                                 cs.NgayTao.Month == thang && cs.NgayTao.Year == nam)
                    .ToListAsync();
                decimal tongThuong = Math.Round(thongKeThuong.Sum(cs => cs.SoTien), 0);

                // Lấy danh sách phạt theo mã nhân viên
                var thongKePhat = await _context.ChinhSachs
                    .Where(cs => cs.LoaiCS == "Kỷ luật" && cs.MaNV == nhanVien.MaNV &&
                                 cs.NgayTao.Month == thang && cs.NgayTao.Year == nam)
                    .ToListAsync();
                decimal tongPhat = Math.Round(thongKePhat.Sum(cs => cs.SoTien), 0);


                decimal BHXH_NLD = Math.Round(luongCB * 0.08M, 0);
                decimal BHTN_NLD = Math.Round(hopDong.NgayKetThuc.Subtract(hopDong.NgayVaoLam).TotalDays >= 90 ? luongCB * 0.015M : 0, 0);
                decimal BHYT_NLD = Math.Round(hopDong.NgayKetThuc.Subtract(hopDong.NgayVaoLam).TotalDays >= 90 ? luongCB * 0.01M : 0, 0);
                decimal BHXH_DN = Math.Round(luongCB * 0.17M, 0);
                decimal BHTN_DN = Math.Round(hopDong.NgayKetThuc.Subtract(hopDong.NgayVaoLam).TotalDays >= 90 ? luongCB * 0.03M : 0, 0);
                decimal BHYT_DN = Math.Round(hopDong.NgayKetThuc.Subtract(hopDong.NgayVaoLam).TotalDays >= 90 ? luongCB * 0.01M : 0, 0);

                var tongTamUng = await _context.TamUngs
                    .Where(tu => tu.MaNV == nhanVien.MaNV && tu.NgayTamUng.Month == thang && tu.NgayTamUng.Year == nam)
                    .SumAsync(tu => tu.SoTien);
                decimal tongTamUngRounded = Math.Round(tongTamUng, 0);


                var tongHopCong = await _context.TongHopCongs
                    .Where(t => t.MaNV == nhanVien.MaNV && t.Thang == thang && t.Nam == nam)
                    .FirstOrDefaultAsync();

                float ngayCongThucTe = tongHopCong?.SoNgayCong ?? 0;

                decimal luongThucTe = decimal.Ceiling((luongCB / soNgayCongChuan) * (decimal)ngayCongThucTe);

                decimal tienTangCaThuong = decimal.Ceiling((decimal)tongHopCong.LamThemNgayThuong * luongCB * 1.5m / 8);
                decimal tienTangCaLe = decimal.Ceiling((decimal)tongHopCong.LamNgayLe * luongCB * 3m / 8);
                decimal tienTangCaNghi = decimal.Ceiling((decimal)tongHopCong.LamNgayNghi * luongCB * 2m / 8);
                decimal tongTienTC = decimal.Ceiling(tienTangCaThuong + tienTangCaLe + tienTangCaNghi);

                decimal tongThuNhap = decimal.Ceiling(luongThucTe + tongPhuCap + tongThuong + tongTienTC);
                decimal thueTNCN = decimal.Ceiling(TinhThueTNCN(tongThuNhap));
                decimal luongThucLanh = decimal.Ceiling(tongThuNhap - tongPhat - (BHXH_NLD + BHTN_NLD + BHYT_NLD) - thueTNCN);


                ketQuaLuong.Add(new LuongNhanVienVM
                {
                    MaNV = nhanVien.MaNV,
                    HoTen = nhanVien.HoTen,
                    Thang = thang,
                    Nam = nam,
                    LuongCB = luongCB,
                    NgayCongThucTe = ngayCongThucTe,
                    TongPhuCap = tongPhuCap,
                    PhuCapAnTrua = phuCapAnTrua,
                    PhuCapXangXe = phuCapXangXe,
                    PhuCapDienThoai = phuCapDienThoai,
                    PhuCapKhac = phuCapKhac,
                    TongThuong = tongThuong,
                    TongPhat = tongPhat,
                    BHXH_NLD = BHXH_NLD,
                    BHTN_NLD = BHTN_NLD,
                    BHYT_NLD = BHYT_NLD,
                    BHXH_DN = BHXH_DN,
                    BHTN_DN = BHTN_DN,
                    BHYT_DN = BHYT_DN,
                    TongTCThuong = tienTangCaThuong,
                    TongTCNghi = tienTangCaNghi,
                    TongTCLe = tienTangCaLe,
                    TongTienTC = tongTienTC,
                    TongThuNhap = tongThuNhap,
                    ThueTNCN = thueTNCN,
                    LuongThucLanh = luongThucLanh,
                    TongTU = tongTamUng
                });
            }

            return Ok(ketQuaLuong);
        }

        [HttpPost("SaveBangLuong")]
        public async Task<IActionResult> SaveBangLuong(int thang, int nam, List<BangLuong> bangLuong)
        {
            if (bangLuong == null || bangLuong.Count == 0)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            foreach (var item in bangLuong)
            {
                // Kiểm tra nếu bảng lương đã tồn tại cho nhân viên trong tháng và năm đó
                var existingRecord = await _context.BangLuongs
                    .FirstOrDefaultAsync(x => x.MaNV == item.MaNV && x.Thang == thang && x.Nam == nam);

                if (existingRecord != null)
                {
                    // Nếu bảng lương đã tồn tại, cập nhật lại các thông tin
                    existingRecord.LuongCB = item.LuongCB;
                    existingRecord.TongPhuCap = item.TongPhuCap;
                    existingRecord.TongThuong = item.TongThuong;
                    existingRecord.TongThuNhap = item.TongThuNhap;
                    existingRecord.BHXH_NLD = item.BHXH_NLD;
                    existingRecord.BHYT_NLD = item.BHYT_NLD;
                    existingRecord.BHTN_NLD = item.BHTN_NLD;
                    existingRecord.BHXH_DN = item.BHXH_DN;
                    existingRecord.BHYT_DN = item.BHYT_DN;
                    existingRecord.BHTN_DN = item.BHTN_DN;
                    existingRecord.TongPhat = item.TongPhat;
                    existingRecord.TongTU = item.TongTU;
                    existingRecord.ThueTNCN = item.ThueTNCN;
                    existingRecord.LuongThucLanh = item.LuongThucLanh;
                    existingRecord.TongTienTC = item.TongTienTC;
                }
                else
                {
                    // Nếu chưa tồn tại, tạo mới bảng lương
                    var newBangLuong = new BangLuong
                    {
                        Thang = thang,
                        Nam = nam,
                        MaNV = item.MaNV,
                        LuongCB = item.LuongCB,
                        TongPhuCap = item.TongPhuCap,
                        TongThuong = item.TongThuong,
                        TongThuNhap = item.TongThuNhap,
                        BHXH_NLD = item.BHXH_NLD,
                        BHYT_NLD = item.BHYT_NLD,
                        BHTN_NLD = item.BHTN_NLD,
                        BHXH_DN = item.BHXH_DN,
                        BHYT_DN = item.BHYT_DN,
                        BHTN_DN = item.BHTN_DN,
                        TongPhat = item.TongPhat,
                        TongTU = item.TongTU,
                        ThueTNCN = item.ThueTNCN,
                        LuongThucLanh = item.LuongThucLanh,
                        TongTienTC = item.TongTienTC
                    };

                    // Thêm vào DbSet
                    _context.BangLuongs.Add(newBangLuong);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return Ok("Lưu bảng lương thành công.");
        }





        // Hàm tính thuế thu nhập cá nhân
        private decimal TinhThueTNCN(decimal tongThuNhap)
        {
            decimal giamTruBanThan = 11000000;

            // Tính thu nhập chịu thuế sau khi giảm trừ
            decimal thuNhapChiuThue = tongThuNhap - giamTruBanThan;

            // Nếu thu nhập chịu thuế < 0 thì không tính thuế
            if (thuNhapChiuThue <= 0)
            {
                return 0;
            }

            // Tính thuế TNCN dựa trên thu nhập chịu thuế
            decimal thueTNCN = 0;

            // Thuế suất theo từng bậc
            if (thuNhapChiuThue <= 5000000)
            {
                thueTNCN = thuNhapChiuThue * 0.05M;
            }
            else if (thuNhapChiuThue <= 10000000)
            {
                thueTNCN = thuNhapChiuThue * 0.1M;
            }
            else if (thuNhapChiuThue <= 18000000)
            {
                thueTNCN = 5000000 * 0.05M + 5000000 * 0.1M + (thuNhapChiuThue - 10000000) * 0.15M;
            }
            else if (thuNhapChiuThue <= 32000000)
            {
                thueTNCN = 5000000 * 0.05M + 5000000 * 0.1M + 8000000 * 0.15M + (thuNhapChiuThue - 18000000) * 0.2M;
            }
            else if (thuNhapChiuThue <= 52000000)
            {
                thueTNCN = 5000000 * 0.05M + 5000000 * 0.1M + 8000000 * 0.15M + 14000000 * 0.2M + (thuNhapChiuThue - 32000000) * 0.25M;
            }
            else if (thuNhapChiuThue <= 80000000)
            {
                thueTNCN = 5000000 * 0.05M + 5000000 * 0.1M + 8000000 * 0.15M + 14000000 * 0.2M + 20000000 * 0.25M + (thuNhapChiuThue - 52000000) * 0.3M;
            }
            else
            {
                thueTNCN = 5000000 * 0.05M + 5000000 * 0.1M + 8000000 * 0.15M + 14000000 * 0.2M + 20000000 * 0.25M + 28000000 * 0.3M + (thuNhapChiuThue - 80000000) * 0.35M;
            }

            return thueTNCN;
        }




        [HttpGet("GetLuongNVTheoNam")]
        public async Task<IActionResult> GetLuongNVTheoNam([FromQuery] string maNV, [FromQuery] int nam)
        {
            // Lấy tháng hiện tại
            int thangHienTai = DateTime.Now.Month;

            // Tạo danh sách kết quả
            var danhSachLuong = new List<LuongNhanVienVM>();

            // Vòng lặp qua từng tháng từ 1 đến tháng hiện tại
            for (int thang = 1; thang <= thangHienTai; thang++)
            {
                // Lấy thông tin bảng lương cho tháng hiện tại
                var luongThang = await _context.BangLuongs
                    .Where(bl => bl.MaNV == maNV && bl.Nam == nam && bl.Thang == thang)
                    .Join(_context.NhanViens,
                        bl => bl.MaNV,
                        nv => nv.MaNV,
                        (bl, nv) => new { bl, nv })
                    .Join(_context.TongHopCongs,
                        blnv => new { blnv.bl.MaNV, blnv.bl.Thang, blnv.bl.Nam },
                        thc => new { thc.MaNV, thc.Thang, thc.Nam },
                        (blnv, thc) => new { blnv, thc })
                    .Select(x => new LuongNhanVienVM
                    {
                        MaNV = x.blnv.nv.MaNV,
                        HoTen = x.blnv.nv.HoTen,
                        LuongCB = x.blnv.bl.LuongCB,
                        Thang = x.blnv.bl.Thang,
                        Nam = x.blnv.bl.Nam,
                        NgayCongThucTe = x.thc.SoNgayCong,

                        BHXH_NLD = x.blnv.bl.BHXH_NLD,
                        BHTN_NLD = x.blnv.bl.BHTN_NLD,
                        BHYT_NLD = x.blnv.bl.BHYT_NLD,
                        BHXH_DN = x.blnv.bl.BHXH_DN,
                        BHTN_DN = x.blnv.bl.BHTN_DN,
                        BHYT_DN = x.blnv.bl.BHYT_DN,

                        TongTCThuong = (decimal)x.thc.LamThemNgayThuong,
                        TongTCNghi = (decimal)x.thc.LamNgayNghi,
                        TongTCLe = (decimal)x.thc.LamNgayLe,
                        TongTienTC = x.blnv.bl.TongTienTC,

                        TongThuNhap = x.blnv.bl.TongThuNhap,
                        ThueTNCN = x.blnv.bl.ThueTNCN,
                        LuongThucLanh = x.blnv.bl.LuongThucLanh,
                        TongTU = x.blnv.bl.TongTU,
                    })
                    .FirstOrDefaultAsync();

                // Nếu không có bảng lương tháng này thì bỏ qua
                if (luongThang == null) continue;

                // Lấy danh sách phụ cấp theo khoảng thời gian `NgayBatDau` và `NgayKetThuc`
                var phuCapsThang = await _context.PhuCaps
                    .Where(pc => pc.MaNV == maNV &&
                                 pc.NgayBatDau.Year <= nam && pc.NgayKetThuc.Year >= nam && // Điều kiện năm
                                 ((pc.NgayBatDau.Year < nam) || (pc.NgayBatDau.Year == nam && pc.NgayBatDau.Month <= thang)) && // Điều kiện tháng bắt đầu
                                 ((pc.NgayKetThuc.Year > nam) || (pc.NgayKetThuc.Year == nam && pc.NgayKetThuc.Month >= thang))) // Điều kiện tháng kết thúc
                    .GroupBy(pc => pc.LoaiPC)
                    .Select(g => new
                    {
                        LoaiPhuCap = g.Key,
                        TongPhuCap = g.Sum(pc => pc.SoTien)
                    })
                    .ToListAsync();

                // Cập nhật phụ cấp vào bảng lương
                foreach (var phuCap in phuCapsThang)
                {
                    switch (phuCap.LoaiPhuCap)
                    {
                        case "Phụ cấp ăn trưa":
                            luongThang.PhuCapAnTrua = phuCap.TongPhuCap;
                            break;
                        case "Phụ cấp xăng xe":
                            luongThang.PhuCapXangXe = phuCap.TongPhuCap;
                            break;
                        case "Phụ cấp điện thoại":
                            luongThang.PhuCapDienThoai = phuCap.TongPhuCap;
                            break;
                        default:
                            luongThang.PhuCapKhac += phuCap.TongPhuCap;
                            break;
                    }

                    // Cộng dồn tổng phụ cấp
                    luongThang.TongPhuCap += phuCap.TongPhuCap;
                }

                // Thêm bảng lương đã cập nhật phụ cấp vào danh sách
                danhSachLuong.Add(luongThang);
            }

            // Kiểm tra nếu không có dữ liệu
            if (danhSachLuong.Count == 0)
            {
                return NotFound("Không tìm thấy dữ liệu bảng lương và phụ cấp cho nhân viên trong khoảng thời gian này.");
            }

            return Ok(danhSachLuong);
        }








    }

}
