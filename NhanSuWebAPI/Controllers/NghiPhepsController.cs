using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanSuWebAPI.Models;
using NhanSuWebAPI.Models.Context;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NghiPhepsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NghiPhepsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/NghiPheps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NghiPhep>>> GetNghiPheps(DateTime? fromDate, DateTime? toDate)
        {
            // Kiểm tra null cho _context.NghiPheps
            if (_context.NghiPheps == null)
            {
                return NotFound();
            }

            // Kiểm tra null cho fromDate và toDate
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return BadRequest("fromDate and toDate must be provided.");
            }

            // Sử dụng Include để tải thông tin nhân viên liên quan
            var result = await _context.NghiPheps.Include(np => np.NhanVien) // Bao gồm thông tin nhân viên
                .Where(np =>
                    (np.TuNgay >= fromDate.Value && np.TuNgay <= toDate.Value) ||
                    (np.DenNgay >= fromDate.Value && np.DenNgay <= toDate.Value) ||
                    (np.TuNgay <= fromDate.Value && np.DenNgay >= toDate.Value)) // bao gồm cả khoảng thời gian
                .ToListAsync(); // Chuyển đổi thành danh sách

            return Ok(result);
        }

        // GET: api/NghiPheps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NghiPhep>> GetNghiPhep(int id)
        {
          if (_context.NghiPheps == null)
          {
              return NotFound();
          }
            var nghiPhep = await _context.NghiPheps.FindAsync(id);

            if (nghiPhep == null)
            {
                return NotFound();
            }

            return nghiPhep;
        }

        // PUT: api/NghiPheps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Phương thức POST để cập nhật trạng thái nghỉ phép
        [HttpPost("CapNhatTrangThai/{id}")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] NghiPhep request)
        {
            // Tìm bản ghi nghỉ phép trong cơ sở dữ liệu
            var nghiPhep = await _context.NghiPheps.FindAsync(id);
            if (nghiPhep == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy thông tin nghỉ phép." });
            }

            // Cập nhật trạng thái nghỉ phép
            nghiPhep.TrangThai = request.TrangThai;

            if (request.TrangThai == "Đã duyệt")
            {
                if (request.LoaiNP == "Có lương")
                {
                    // Trường hợp nghỉ phép có lương: thêm bản ghi chấm công
                    DateTime tuNgay = nghiPhep.TuNgay.Date;
                    DateTime denNgay = nghiPhep.DenNgay.Date;
                    string maNV = nghiPhep.MaNV;

                    for (DateTime ngay = tuNgay; ngay <= denNgay; ngay = ngay.AddDays(1))
                    {
                        var chamCong = new ChamCong
                        {
                            MaNV = maNV,
                            NgayChamCong = ngay,
                            ThoiGianVao = null,
                            ThoiGianRa = null,
                            SoGioLam = 0,
                            SoGioTC = 0,
                            TrangThaiCC = "Nghỉ phép"
                        };

                        _context.ChamCongs.Add(chamCong);
                    }
                }
               
            }

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật trạng thái nghỉ phép thành công." });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái nghỉ phép.", error = ex.Message });
            }
        }










        // POST: api/NghiPheps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NghiPhep>> PostNghiPhep(NghiPhep nghiPhep)
        {
            if (_context.NghiPheps == null)
            {
                return Problem("Entity set 'AppDBContext.NghiPheps' is null.");
            }

            // Validate the dates
            if (nghiPhep.TuNgay > nghiPhep.DenNgay)
            {
                return BadRequest("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");
            }



            _context.NghiPheps.Add(nghiPhep);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNghiPhep", new { id = nghiPhep.MaNP }, nghiPhep);
        }


       

        private bool NghiPhepExists(int id)
        {
            return (_context.NghiPheps?.Any(e => e.MaNP == id)).GetValueOrDefault();
        }




        //Lấy nghỉ phép theo mã nhân viên
        [HttpGet("GetListNghiPhepNV")]
        public ActionResult<IEnumerable<NghiPhep>> GetListNghiPhepNV(string maNV, int nam)
        {
            // Lấy danh sách nghỉ phép theo mã nhân viên và năm
            var nghiPhepList = _context.NghiPheps
                .Where(np => np.MaNV == maNV && np.TuNgay.Year == nam)
                .ToList();

            if (nghiPhepList == null || !nghiPhepList.Any())
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy dữ liệu
            }

            return Ok(nghiPhepList);  // Trả về danh sách thông tin nghỉ phép
        }


        [HttpGet("GetTKNghiPhep")]
        public async Task<IActionResult> GetTKNghiPhep(string maNV)
        {
            var nghiPhepList = await _context.NghiPheps
                .Where(np => np.MaNV == maNV)
                .ToListAsync();

            if (nghiPhepList == null || nghiPhepList.Count == 0)
            {
                return NotFound("Không tìm thấy dữ liệu nghỉ phép cho nhân viên này.");
            }

            // Giá trị mặc định cho tổng số ngày nghỉ có lương
            const int soNgayNghiCoLuongMacDinh = 12;

            // Tính tổng số ngày nghỉ có lương đã sử dụng
            var tongNgayNghiCoLuongDaNghi = nghiPhepList
                .Where(np => np.LoaiNP == "Có lương")
                .Sum(np => (np.DenNgay - np.TuNgay).Days + 1);

            // Tính tổng số ngày nghỉ không lương đã sử dụng
            var tongNgayNghiKhongLuongDaNghi = nghiPhepList
                .Where(np => np.LoaiNP == "Không lương")
                .Sum(np => (np.DenNgay - np.TuNgay).Days + 1);

            // Tính số ngày nghỉ có lương còn lại
            var soNgayNghiCoLuongConLai = soNgayNghiCoLuongMacDinh - tongNgayNghiCoLuongDaNghi;

            var result = new
            {
                TongSoNgayNghiCoLuongTheoQuyDinh = soNgayNghiCoLuongMacDinh,
                TongSoNgayNghiCoLuongDaNghi = tongNgayNghiCoLuongDaNghi,
                TongSoNgayNghiKhongLuongDaNghi = tongNgayNghiKhongLuongDaNghi,
                SoNgayNghiCoLuongConLai = soNgayNghiCoLuongConLai
            };

            return Ok(result);
        }
    }
}
