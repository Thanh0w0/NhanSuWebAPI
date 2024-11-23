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
    public class ChinhSachsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ChinhSachsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/ChinhSachs
        [HttpGet("GetChinhSachs")]
        public async Task<ActionResult<IEnumerable<ChinhSach>>> GetChinhSachs(int month, int year)
        {
            if (_context.ChinhSachs == null)
            {
                return NotFound();
            }

            // Lọc chính sách theo tháng và năm
            var chinhSachs = await _context.ChinhSachs
                .Where(c => c.NgayTao.Year == year && c.NgayTao.Month == month)
                .ToListAsync();

            if (chinhSachs == null || !chinhSachs.Any())
            {
                return NotFound();
            }

            return chinhSachs;
        }


        // GET: api/ChinhSachs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChinhSach>> GetChinhSach(int id)
        {
          if (_context.ChinhSachs == null)
          {
              return NotFound();
          }
            var ChinhSach = await _context.ChinhSachs.FindAsync(id);

            if (ChinhSach == null)
            {
                return NotFound();
            }

            return ChinhSach;
        }

        // PUT: api/ChinhSachs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChinhSach(int id, ChinhSach ChinhSach)
        {
            if (id != ChinhSach.MaCS)
            {
                return BadRequest();
            }

            _context.Entry(ChinhSach).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChinhSachExists(id))
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

        // POST: api/ChinhSachs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChinhSach>> PostChinhSach(ChinhSach ChinhSach)
        {
          if (_context.ChinhSachs == null)
          {
              return Problem("Entity set 'AppDBContext.ChinhSachs'  is null.");
          }
            _context.ChinhSachs.Add(ChinhSach);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChinhSach", new { id = ChinhSach.MaCS }, ChinhSach);
        }

        // DELETE: api/ChinhSachs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChinhSach(int id)
        {
            if (_context.ChinhSachs == null)
            {
                return NotFound();
            }
            var ChinhSach = await _context.ChinhSachs.FindAsync(id);
            if (ChinhSach == null)
            {
                return NotFound();
            }

            _context.ChinhSachs.Remove(ChinhSach);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChinhSachExists(int id)
        {
            return (_context.ChinhSachs?.Any(e => e.MaCS == id)).GetValueOrDefault();
        }
    }
}
