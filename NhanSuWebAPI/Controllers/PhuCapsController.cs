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
    public class PhuCapsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PhuCapsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PhuCaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhuCap>>> GetPhuCaps()
        {
          if (_context.PhuCaps == null)
          {
              return NotFound();
          }
            return await _context.PhuCaps.ToListAsync();
        }

        // GET: api/PhuCaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhuCap>> GetPhuCap(int id)
        {
          if (_context.PhuCaps == null)
          {
              return NotFound();
          }
            var phuCap = await _context.PhuCaps.FindAsync(id);

            if (phuCap == null)
            {
                return NotFound();
            }

            return phuCap;
        }

        // PUT: api/PhuCaps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhuCap(int id, PhuCap phuCap)
        {
            if (id != phuCap.MaPC)
            {
                return BadRequest();
            }

            _context.Entry(phuCap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhuCapExists(id))
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

        // POST: api/PhuCaps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhuCap>> PostPhuCap(PhuCap phuCap)
        {
          if (_context.PhuCaps == null)
          {
              return Problem("Entity set 'AppDBContext.PhuCaps'  is null.");
          }
            _context.PhuCaps.Add(phuCap);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhuCap", new { id = phuCap.MaPC }, phuCap);
        }

        // DELETE: api/PhuCaps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhuCap(int id)
        {
            if (_context.PhuCaps == null)
            {
                return NotFound();
            }
            var phuCap = await _context.PhuCaps.FindAsync(id);
            if (phuCap == null)
            {
                return NotFound();
            }

            _context.PhuCaps.Remove(phuCap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhuCapExists(int id)
        {
            return (_context.PhuCaps?.Any(e => e.MaPC == id)).GetValueOrDefault();
        }


        [HttpGet("GetNhanVienByMaNV/{maNV}")]
        public async Task<ActionResult<IEnumerable<PhuCap>>> GetNhanVienByMaNV(string maNV)
        {
            // Find all contracts associated with the employee code (maNV)
            var phuCaps = await _context.PhuCaps
                .Where(h => h.MaNV == maNV)
                .ToListAsync();

            if (phuCaps == null || !phuCaps.Any())
            {
                return NotFound();
            }

            return Ok(phuCaps); 
        }
    }
}
