using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcChamCong
    {

        public int MaCC { get; set; }

        [Display(Name = "Mã Nhân Viên")]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

        [Required]
        public DateTime NgayChamCong { get; set; }

        
        public TimeSpan? ThoiGianVao { get; set; }

        public TimeSpan? ThoiGianRa { get; set; }

        
        public float SoGioLam { get; set; }
        public float SoGioTC { get; set; }

        [Required]
        public string TrangThaiCC { get; set; }
    }
}
