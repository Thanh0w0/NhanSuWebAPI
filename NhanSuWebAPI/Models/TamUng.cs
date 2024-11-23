using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("TamUng")]
    public class TamUng
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTU { get; set; } // Mã tạm ứng, tự động tăng

    
        public string MaNV { get; set; } // Mã nhân viên

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; } // Liên kết với bảng NhanVien

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SoTien { get; set; } // Số tiền tạm ứng

        [StringLength(255)]
        public string LyDo { get; set; } // Lý do tạm ứng

        [Required]
        public DateTime NgayTamUng { get; set; } // Ngày tạm ứng

        [StringLength(50)]
        public string? TrangThai { get; set; }



    }
}
