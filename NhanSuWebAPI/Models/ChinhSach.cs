using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("ChinhSach")]
    public class ChinhSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCS { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }

        [Display(Name = "Ngày Tạo")]
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
