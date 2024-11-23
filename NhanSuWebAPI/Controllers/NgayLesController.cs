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
    public class NgayLesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NgayLesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/NgayLes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NgayLe>>> GetNgayLes()
        {
          if (_context.NgayLes == null)
          {
              return NotFound();
          }
            return await _context.NgayLes.ToListAsync();
        }

        // GET: api/NgayLes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NgayLe>> GetNgayLe(int id)
        {
          if (_context.NgayLes == null)
          {
              return NotFound();
          }
            var ngayLe = await _context.NgayLes.FindAsync(id);

            if (ngayLe == null)
            {
                return NotFound();
            }

            return ngayLe;
        }

        // PUT: api/NgayLes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNgayLe(int id, NgayLe ngayLe)
        {
            if (id != ngayLe.MaNL)
            {
                return BadRequest();
            }

            _context.Entry(ngayLe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NgayLeExists(id))
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

        // POST: api/NgayLes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NgayLe>> PostNgayLe(NgayLe ngayLe)
        {
            if (_context.NgayLes == null)
            {
                return Problem("Entity set 'AppDBContext.NgayLes' is null.");
            }

            // Bắt đầu transaction để đảm bảo các thao tác thêm vào cơ sở dữ liệu thành công cùng một lúc
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Thêm ngày nghỉ lễ vào cơ sở dữ liệu
                    _context.NgayLes.Add(ngayLe);
                    await _context.SaveChangesAsync();

                    var employees = await _context.NhanViens.ToListAsync();

                    // Lặp qua từng ngày trong khoảng từ NgayBD đến NgayKT.AddDays(-1)
                    for (DateTime date = ngayLe.NgayBD; date < ngayLe.NgayKT; date = date.AddDays(1))
                    {
                        foreach (var employee in employees)
                        {
                            var attendance = new ChamCong
                            {
                                MaNV = employee.MaNV,
                                NgayChamCong = date,
                                ThoiGianVao = null, // Không có thời gian vào vì đây là ngày nghỉ lễ
                                ThoiGianRa = null, // Không có thời gian ra
                                SoGioLam = 0, // Không có giờ làm
                                SoGioTC = 0, // Không có giờ tăng ca
                                TrangThaiCC = "Nghỉ Lễ" // Trạng thái là nghỉ lễ
                            };

                            _context.ChamCongs.Add(attendance);
                        }
                    }

                    // Lưu tất cả chấm công
                    await _context.SaveChangesAsync();

                    // Commit transaction nếu tất cả các thao tác thành công
                    await transaction.CommitAsync();

                    // Trả về kết quả thành công
                    return Ok(new { success = true, message = "Ngày nghỉ lễ và chấm công đã được thêm thành công." });
                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, rollback transaction để không thay đổi cơ sở dữ liệu
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi thêm ngày nghỉ lễ và chấm công.", error = ex.Message });
                }
            }
        }



        // DELETE: api/NgayLes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNgayLe(int id)
        {
            if (_context.NgayLes == null)
            {
                return NotFound();
            }
            var ngayLe = await _context.NgayLes.FindAsync(id);
            if (ngayLe == null)
            {
                return NotFound();
            }

            _context.NgayLes.Remove(ngayLe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NgayLeExists(int id)
        {
            return (_context.NgayLes?.Any(e => e.MaNL == id)).GetValueOrDefault();
        }

        
        
    }
}
