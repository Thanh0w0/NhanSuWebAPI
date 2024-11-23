using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcPhuCap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPC { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

        public string LoaiPC { get; set; }
        public decimal SoTien { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayBatDau { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayKetThuc { get; set; }

        public string? GhiChu { get; set; }
    }
}
