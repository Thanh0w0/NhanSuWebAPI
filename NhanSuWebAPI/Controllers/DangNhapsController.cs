using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanSuWebAPI.Models.Context;
using NhanSuWebAPI.Models.VMs;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangNhapsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DangNhapsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginVM model)
        {
            // Retrieve the user by username (TenDN)
            var user = _context.TaiKhoans
                .Include(u => u.Quyen)
                .Include(u => u.NhanVien) // Join with NhanVien to get employee details
                .FirstOrDefault(u => u.TenDN == model.TenDN);

            if (user == null)
            {
                return Unauthorized("Tên đăng nhập không đúng");
            }

            // Verify if the entered password matches the hashed password stored in the database
            if (!BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau))
            {
                return Unauthorized("Mật khẩu không đúng");
            }

            // Return user info, including full name and image
            var response = new
            {
                MaNV = user.MaNV,
                TenDN = user.TenDN,
                MaQuyen = user.MaQuyen,
                Quyen = user.Quyen?.TenQuyen,
                TenNV = user.NhanVien?.HoTen, // Full name
                HinhAnh = user.NhanVien?.HinhAnh // Image
            };

            return Ok(response);
        }


    }
}
