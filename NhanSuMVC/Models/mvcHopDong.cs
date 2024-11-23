using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcHopDong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHD { get; set; }

        [Display(Name = "Mã Nhân Viên")]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

        [Display(Name = "Mã Loại Hợp Đồng")]
        public int MaLoaiHD { get; set; }

        [ForeignKey("MaLoaiHD")]
        public mvcLoaiHD? LoaiHD { get; set; }

        [Display(Name = "Ngày Vào Làm")]
        [DataType(DataType.Date)]
        public DateTime NgayVaoLam { get; set; }

        [Display(Name = "Ngày Kết Thúc")]
        [DataType(DataType.Date)]
        public DateTime NgayKetThuc { get; set; }

        [Display(Name = "Ghi Chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Trạng Thái")]
        public string TrangThai { get; set; }

        [Display(Name = "Lương Cơ Bản")]
        public decimal LuongCB { get; set; }



    }
}
