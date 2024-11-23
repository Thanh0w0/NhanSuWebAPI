using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        public string MaNV {  get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public string CCCD { get; set; }
        public string NoiSinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; } 
        public string HinhAnh { get; set; }
        public string TrinhDo { get; set; }
        public string Email { get; set; }
        public string TrangThai {  get; set; }

        public int MaPB { get; set; }

        [ForeignKey("MaPB")]
        public PhongBan? PhongBan { get; set; }

        public int MaCV { get; set; }

        [ForeignKey("MaCV")]
        public ChucVu? ChucVu { get; set; }


 
    }
}
