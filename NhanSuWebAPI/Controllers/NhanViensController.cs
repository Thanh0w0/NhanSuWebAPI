using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanSuWebAPI.Models;
using NhanSuWebAPI.Models.Context;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanViensController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NhanViensController(AppDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/NhanViens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVien>>> GetNhanViens()
        {
            if (_context.NhanViens == null)
            {
                return NotFound();
            }

            // Sử dụng Include để lấy thông tin Phòng Ban và Chức Vụ liên quan
            var nhanViens = await _context.NhanViens
                                          .Include(nv => nv.PhongBan)  
                                          .Include(nv => nv.ChucVu)    
                                          .ToListAsync();

            return nhanViens;
        }

        // GET: api/NhanViens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> GetNhanVien(string id)
        {
          if (_context.NhanViens == null)
          {
              return NotFound();
          }
            var nhanVien = await _context.NhanViens.FindAsync(id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            return nhanVien;
        }

        // PUT: api/NhanViens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNhanVien(string id, [FromBody] NhanVien nhanVien)
        {
            if (id != nhanVien.MaNV)
            {
                return BadRequest("Mã nhân viên không khớp.");
            }

            var existingNhanVien = await _context.NhanViens.FindAsync(id);
            if (existingNhanVien == null)
            {
                return NotFound("Không tìm thấy nhân viên.");
            }

            // Cập nhật thông tin nhân viên
            existingNhanVien.HoTen = nhanVien.HoTen;
            existingNhanVien.NgaySinh = nhanVien.NgaySinh;
            existingNhanVien.GioiTinh = nhanVien.GioiTinh;
            existingNhanVien.DanToc = nhanVien.DanToc;
            existingNhanVien.TonGiao = nhanVien.TonGiao;
            existingNhanVien.CCCD = nhanVien.CCCD;
            existingNhanVien.NoiSinh = nhanVien.NoiSinh;
            existingNhanVien.DiaChi = nhanVien.DiaChi;
            existingNhanVien.SDT = nhanVien.SDT;
            existingNhanVien.HinhAnh = nhanVien.HinhAnh; // Đường dẫn hình ảnh đã xử lý ở MVC
            existingNhanVien.TrinhDo = nhanVien.TrinhDo;
            existingNhanVien.Email = nhanVien.Email;
            existingNhanVien.TrangThai = nhanVien.TrangThai;
            existingNhanVien.MaPB = nhanVien.MaPB;
            existingNhanVien.MaCV = nhanVien.MaCV;

            try
            {
                _context.Entry(existingNhanVien).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi cập nhật: {ex.Message}");
            }
        }




        // POST: api/NhanViens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostNhanVien([FromBody] NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Thêm nhân viên vào context
                _context.NhanViens.Add(nhanVien);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Thêm nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }


        // DELETE: api/NhanViens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhanVien(string id)
        {
            if (_context.NhanViens == null)
            {
                return NotFound();
            }
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            _context.NhanViens.Remove(nhanVien);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienExists(string id)
        {
            return (_context.NhanViens?.Any(e => e.MaNV == id)).GetValueOrDefault();
        }

        [HttpGet("TaoMaNV")]
        public string TaoMaNV()
        {
            // Lấy mã nhân viên cuối cùng
            var lastNhanVien = _context.NhanViens.OrderByDescending(nv => nv.MaNV).FirstOrDefault();

            // Nếu chưa có nhân viên nào, bắt đầu từ mã "NV0001"
            if (lastNhanVien == null)
            {
                return "NV0001";
            }

            // Tách phần số từ mã nhân viên cuối cùng
            string lastMaNV = lastNhanVien.MaNV;
            int numberPart = int.Parse(lastMaNV.Substring(2));

            // Tăng giá trị số lên 1
            int newNumber = numberPart + 1;

            // Tạo mã nhân viên mới với định dạng "NV000X"
            string newMaNV = "NV" + newNumber.ToString().PadLeft(4, '0');

            return newMaNV;
        }

        [HttpGet("GetNhanVienByMaNV/{maNV}")]
        public async Task<ActionResult<NhanVien>> GetNhanVienByMaNV(string maNV)
        {
            // Get employee information by employee code
            var nhanVien = await _context.NhanViens
                                          .Include(nv => nv.PhongBan) // Include related PhongBan information
                                          .Include(nv => nv.ChucVu)   // Include related ChucVu information
                                          .FirstOrDefaultAsync(nv => nv.MaNV == maNV); // Filter by maNV

            if (nhanVien == null)
            {
                return NotFound(); // Return 404 if the employee is not found
            }

            return Ok(nhanVien); // Return the employee information
        }

    }
}
