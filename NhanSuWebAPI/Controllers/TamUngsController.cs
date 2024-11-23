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
    public class TamUngsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TamUngsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/TamUngs
        [HttpGet("GetTamUngs")]
        public async Task<ActionResult<IEnumerable<TamUng>>> GetTamUngs(int? month, int? year)
        {
            if (_context.TamUngs == null)
            {
                return NotFound();
            }

            // Lấy danh sách tạm ứng có lọc theo tháng và năm
            var query = _context.TamUngs.AsQueryable();

            if (month.HasValue)
            {
                query = query.Where(t => t.NgayTamUng.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(t => t.NgayTamUng.Year == year.Value);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }


        // GET: api/TamUngs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TamUng>> GetTamUng(int id)
        {
          if (_context.TamUngs == null)
          {
              return NotFound();
          }
            var tamUng = await _context.TamUngs.FindAsync(id);

            if (tamUng == null)
            {
                return NotFound();
            }

            return tamUng;
        }

        // PUT: api/TamUngs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTamUng(int id, TamUng tamUng)
        {
            if (id != tamUng.MaTU)
            {
                return BadRequest();
            }

            _context.Entry(tamUng).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TamUngExists(id))
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

        // POST: api/TamUngs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TamUng>> PostTamUng(TamUng tamUng)
        {
          if (_context.TamUngs == null)
          {
              return Problem("Entity set 'AppDBContext.TamUngs'  is null.");
          }
            _context.TamUngs.Add(tamUng);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTamUng", new { id = tamUng.MaTU }, tamUng);
        }

        // DELETE: api/TamUngs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTamUng(int id)
        {
            if (_context.TamUngs == null)
            {
                return NotFound();
            }
            var tamUng = await _context.TamUngs.FindAsync(id);
            if (tamUng == null)
            {
                return NotFound();
            }

            _context.TamUngs.Remove(tamUng);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TamUngExists(int id)
        {
            return (_context.TamUngs?.Any(e => e.MaTU == id)).GetValueOrDefault();
        }
    }
}
