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
    public class TaiKhoansController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TaiKhoansController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/TaiKhoans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>> GetTaiKhoans()
        {
          if (_context.TaiKhoans == null)
          {
              return NotFound();
          }
            return await _context.TaiKhoans.ToListAsync();
        }

        // GET: api/TaiKhoans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaiKhoan>> GetTaiKhoan(int id)
        {
          if (_context.TaiKhoans == null)
          {
              return NotFound();
          }
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return taiKhoan;
        }

        // PUT: api/TaiKhoans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(int id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTK)
            {
                return BadRequest();
            }

            _context.Entry(taiKhoan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaiKhoanExists(id))
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

        // POST: api/TaiKhoans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan(TaiKhoan taiKhoan)
        {
          if (_context.TaiKhoans == null)
          {
              return Problem("Entity set 'AppDBContext.TaiKhoans'  is null.");
          }
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaiKhoan", new { id = taiKhoan.MaTK }, taiKhoan);
        }

        // DELETE: api/TaiKhoans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaiKhoan(int id)
        {
            if (_context.TaiKhoans == null)
            {
                return NotFound();
            }
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaiKhoanExists(int id)
        {
            return (_context.TaiKhoans?.Any(e => e.MaTK == id)).GetValueOrDefault();
        }


        [HttpGet("GetNhanVienByMaNV/{maNV}")]
        public async Task<ActionResult<TaiKhoan>> GetNhanVienByMaNV(string maNV)
        {
            // Tìm tài khoản theo mã nhân viên
            var taiKhoan = await _context.TaiKhoans
                .Include(q => q.Quyen)
                .Include(t => t.NhanVien) 
                .FirstOrDefaultAsync(t => t.MaNV == maNV);

            if (taiKhoan == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            // Trả về thông tin tài khoản và nhân viên
            return Ok(taiKhoan);
        }

    }
}
