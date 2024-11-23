using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcTamUng
    {
        public int MaTU { get; set; } // Mã tạm ứng, tự động tăng


        public string MaNV { get; set; } // Mã nhân viên

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; } // Liên kết với bảng NhanVien

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SoTien { get; set; } // Số tiền tạm ứng

        [StringLength(255)]
        public string LyDo { get; set; } // Lý do tạm ứng

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayTamUng { get; set; } // Ngày tạm ứng

        [StringLength(50)]
        public string? TrangThai { get; set; }
    }
}
