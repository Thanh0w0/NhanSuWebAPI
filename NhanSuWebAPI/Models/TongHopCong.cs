using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuWebAPI.Models
{
    [Table("TongHopCong")]
    public class TongHopCong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTH { get; set; } 

        [Required]
        public string MaNV { get; set; } // Mã nhân viên

        [ForeignKey("MaNV")]
        public virtual NhanVien? NhanVien { get; set; } // Thông tin nhân viên

        public int Thang { get; set; } // Tháng và năm tổng hợp công
        public int Nam { get; set; }

        public int SoNgayCong { get; set; } // Số ngày công trong tháng

        public int SoNgayNghiKhongLuong { get; set; } // Số ngày nghỉ không lương

        public int SoNgayNghiCoLuong { get; set; } // Số ngày nghỉ có lương

        public float LamThemNgayThuong { get; set; } // Số giờ làm thêm vào ngày thường

        public float LamNgayLe { get; set; } // Số giờ làm thêm vào ngày lễ

        public float LamNgayNghi { get; set; } // Số giờ làm thêm vào ngày nghỉ (thứ 7, Chủ nhật)

        public float TongGioLamThucTe { get; set; } // Tổng giờ làm thực tế
    }
}
