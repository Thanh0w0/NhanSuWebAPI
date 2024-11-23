using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcNghiPhep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int MaNP { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string? LyDo { get; set; }
        public string TrangThai { get; set; }
        public string LoaiNP { get; set; }

        [Range(0, int.MaxValue)]
        public int? SoNgayNghiCoLuong { get; set; } 

    }
}
