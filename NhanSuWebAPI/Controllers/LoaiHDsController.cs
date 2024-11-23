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
    public class LoaiHDsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LoaiHDsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/LoaiHDs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiHD>>> GetLoaiHDs()
        {
          if (_context.LoaiHDs == null)
          {
              return NotFound();
          }
            return await _context.LoaiHDs.ToListAsync();
        }

        // GET: api/LoaiHDs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiHD>> GetLoaiHD(int id)
        {
          if (_context.LoaiHDs == null)
          {
              return NotFound();
          }
            var loaiHD = await _context.LoaiHDs.FindAsync(id);

            if (loaiHD == null)
            {
                return NotFound();
            }

            return loaiHD;
        }

        // PUT: api/LoaiHDs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiHD(int id, LoaiHD loaiHD)
        {
            if (id != loaiHD.MaLoaiHD)
            {
                return BadRequest();
            }

            _context.Entry(loaiHD).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoaiHDExists(id))
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

        // POST: api/LoaiHDs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoaiHD>> PostLoaiHD(LoaiHD loaiHD)
        {
          if (_context.LoaiHDs == null)
          {
              return Problem("Entity set 'AppDBContext.LoaiHDs'  is null.");
          }
            _context.LoaiHDs.Add(loaiHD);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoaiHD", new { id = loaiHD.MaLoaiHD }, loaiHD);
        }

        // DELETE: api/LoaiHDs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiHD(int id)
        {
            if (_context.LoaiHDs == null)
            {
                return NotFound();
            }
            var loaiHD = await _context.LoaiHDs.FindAsync(id);
            if (loaiHD == null)
            {
                return NotFound();
            }

            _context.LoaiHDs.Remove(loaiHD);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoaiHDExists(int id)
        {
            return (_context.LoaiHDs?.Any(e => e.MaLoaiHD == id)).GetValueOrDefault();
        }
    }
}
