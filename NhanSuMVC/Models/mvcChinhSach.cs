using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcChinhSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCS { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

        [Display(Name = "Ngày Tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Số Tiền")]
        [DataType(DataType.Currency)]
        public decimal SoTien { get; set; }

        [Display(Name = "Nội Dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Ghi Chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Loại")]
        public string LoaiCS { get; set; }
    }
}
