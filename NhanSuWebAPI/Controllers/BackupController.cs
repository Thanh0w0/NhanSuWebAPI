using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NhanSuWebAPI.Models.Context;

namespace NhanSuWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BackupController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("backup-data")]
        public IActionResult BackupData(string backupType)
        {
            try
            {
                // Lấy dữ liệu từ các bảng cần sao lưu
                var chucVus = _context.ChucVus.ToList();
                var phongBans = _context.PhongBans.ToList();
                var quyens = _context.Quyens.ToList();
                var loaiHDs = _context.LoaiHDs.ToList();
                var nhanViens = _context.NhanViens.ToList();
                var chinhSachs = _context.ChinhSachs.ToList();
                var nghiPheps = _context.NghiPheps.ToList();
                var hopDongs = _context.HopDongs.ToList();
                var taiKhoans = _context.TaiKhoans.ToList();
                var chamCongs = _context.ChamCongs.ToList();
                var phuCaps = _context.PhuCaps.ToList();
                var ngayLes = _context.NgayLes.ToList();
                var tongHopCongs = _context.TongHopCongs.ToList();
                var tamUngs = _context.TamUngs.ToList();
                var bangLuongs = _context.BangLuongs.ToList();

                // Tạo đối tượng chứa tất cả dữ liệu
                var data = new
                {
                    ChucVus = chucVus,
                    PhongBans = phongBans,
                    Quyens = quyens,
                    LoaiHDs = loaiHDs,
                    NhanViens = nhanViens,
                    ChinhSachs = chinhSachs,
                    NghiPheps = nghiPheps,
                    HopDongs = hopDongs,
                    TaiKhoans = taiKhoans,
                    ChamCongs = chamCongs,
                    PhuCaps = phuCaps,
                    NgayLes = ngayLes,
                    TongHopCongs = tongHopCongs,
                    TamUngs = tamUngs,
                    BangLuongs = bangLuongs
                };

                string fileName;
                string filePath;

                if (backupType == "json")
                {
                    // Lưu dữ liệu dưới dạng JSON
                    var jsonSettings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var jsonData = JsonConvert.SerializeObject(data, jsonSettings);
                    fileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Backups", fileName);

                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    System.IO.File.WriteAllText(filePath, jsonData);
                }
                else if (backupType == "bak")
                {
                    // Lưu dữ liệu dưới dạng BAK
                    fileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Backups", fileName);

                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    System.IO.File.WriteAllText(filePath, "Dữ liệu sao lưu BAK");
                }
                else
                {
                    return BadRequest(new { success = false, message = "Loại file sao lưu không hợp lệ." });
                }

                // Trả về file để tải xuống
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }


    }
}

