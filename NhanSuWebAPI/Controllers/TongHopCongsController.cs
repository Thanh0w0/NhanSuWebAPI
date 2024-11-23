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
    public class TongHopCongsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TongHopCongsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/TongHopCongs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TongHopCong>>> GetTongHopCongs()
        {
          if (_context.TongHopCongs == null)
          {
              return NotFound();
          }
            return await _context.TongHopCongs.ToListAsync();
        }


        [HttpGet("GetCongNamNV")]
        public async Task<ActionResult<List<TongHopCong>>> GetCongNamNV(string maNV, int nam)
        {
            if (_context.TongHopCongs == null)
            {
                return NotFound();
            }

            // Tạo danh sách các tháng từ 1 đến 12
            List<TongHopCong> tongHopCongList = new List<TongHopCong>();

            // Lấy dữ liệu từ cơ sở dữ liệu cho nhân viên và năm yêu cầu
            var tongHopCong = await _context.TongHopCongs
                .Where(t => t.MaNV == maNV && t.Nam == nam)
                .ToListAsync();

            // Duyệt qua từng tháng từ 1 đến 12 và kiểm tra xem có dữ liệu không
            for (int month = 1; month <= 12; month++)
            {
                var data = tongHopCong.FirstOrDefault(t => t.Thang == month); // Tìm dữ liệu cho tháng cụ thể

                if (data == null)
                {
                    // Nếu không có dữ liệu cho tháng này, tạo đối tượng với giá trị mặc định
                    tongHopCongList.Add(new TongHopCong
                    {
                        MaNV = maNV,
                        Nam = nam,
                        Thang = month,
                        SoNgayCong = 0, // Giá trị mặc định nếu không có dữ liệu
                        SoNgayNghiKhongLuong = 0,
                        SoNgayNghiCoLuong = 0,
                        LamThemNgayThuong = 0,
                        LamNgayLe = 0,
                        LamNgayNghi = 0,
                        TongGioLamThucTe = 0
                    });
                }
                else
                {
                    // Nếu có dữ liệu cho tháng này, thêm vào danh sách
                    tongHopCongList.Add(data);
                }
            }

            // Trả về danh sách tổng hợp công cho cả 12 tháng
            return Ok(tongHopCongList);
        }


        // PUT: api/TongHopCongs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTongHopCong(int id, TongHopCong tongHopCong)
        {
            if (id != tongHopCong.MaTH)
            {
                return BadRequest();
            }

            _context.Entry(tongHopCong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TongHopCongExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TongHopCongs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TongHopCong>> PostTongHopCong(TongHopCong tongHopCong)
        {
          if (_context.TongHopCongs == null)
          {
              return Problem("Entity set 'AppDBContext.TongHopCongs'  is null.");
          }
            _context.TongHopCongs.Add(tongHopCong);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTongHopCong", new { id = tongHopCong.MaTH }, tongHopCong);
        }

        // DELETE: api/TongHopCongs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTongHopCong(int id)
        {
            if (_context.TongHopCongs == null)
            {
                return NotFound();
            }
            var tongHopCong = await _context.TongHopCongs.FindAsync(id);
            if (tongHopCong == null)
            {
                return NotFound();
            }

            _context.TongHopCongs.Remove(tongHopCong);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TongHopCongExists(int id)
        {
            return (_context.TongHopCongs?.Any(e => e.MaTH == id)).GetValueOrDefault();
        }
    }
}
