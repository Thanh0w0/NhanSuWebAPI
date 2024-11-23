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
    public class HopDongsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public HopDongsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HopDong>>> GetHopDongs()
        {
            var hopDongs = await _context.HopDongs
                .Include(h => h.LoaiHD) // Include LoaiHopDong here
                .ToListAsync();

            if (hopDongs == null || hopDongs.Count == 0)
            {
                return NotFound();
            }

            return hopDongs;
        }

        // GET: api/HopDongs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HopDong>> GetHopDong(int id)
        {
          if (_context.HopDongs == null)
          {
              return NotFound();
          }
            var hopDong = await _context.HopDongs.FindAsync(id);

            if (hopDong == null)
            {
                return NotFound();
            }

            return hopDong;
        }

        // PUT: api/HopDongs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHopDong(int id, HopDong hopDong)
        {
            if (id != hopDong.MaHD)
            {
                return BadRequest();
            }

            _context.Entry(hopDong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HopDongExists(id))
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

        // POST: api/HopDongs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HopDong>> PostHopDong(HopDong hopDong)
        {
          if (_context.HopDongs == null)
          {
              return Problem("Entity set 'AppDBContext.HopDongs'  is null.");
          }
            _context.HopDongs.Add(hopDong);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHopDong", new { id = hopDong.MaHD }, hopDong);
        }

        // DELETE: api/HopDongs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHopDong(int id)
        {
            if (_context.HopDongs == null)
            {
                return NotFound();
            }
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null)
            {
                return NotFound();
            }

            _context.HopDongs.Remove(hopDong);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HopDongExists(int id)
        {
            return (_context.HopDongs?.Any(e => e.MaHD == id)).GetValueOrDefault();
        }



        [HttpGet("GetNhanVienByMaNV/{maNV}")]
        public async Task<ActionResult<IEnumerable<HopDong>>> GetNhanVienByMaNV(string maNV)
        {
            // Find all contracts associated with the employee code (maNV)
            var hopDongs = await _context.HopDongs
                .Include(h => h.LoaiHD) // Include the related LoaiHD (Contract Type) information
                .Where(h => h.MaNV == maNV)
                .ToListAsync();

            if (hopDongs == null || !hopDongs.Any())
            {
                return NotFound(); 
            }

            return Ok(hopDongs); // Return the list of contracts
        }



    }
}
