using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("PhuCap")]
    public class PhuCap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPC { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }

        public string LoaiPC { get; set; }

        public decimal SoTien { get; set; }
        public DateTime NgayBatDau { get; set; }   
        public DateTime NgayKetThuc { get; set; } 

        public string? GhiChu {  get; set; }
    }
}
