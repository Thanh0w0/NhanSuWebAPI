using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuWebAPI.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTK { get; set; }

        [Display(Name = "Tên Đăng Nhập")]
        public string TenDN { get; set; }

        [Display(Name = "Mật Khẩu")]
        public string MatKhau { get; set; }


        [Display(Name = "Ngày Tạo")]
        [DataType(DataType.Date)]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Mã Quyền")]
        public int MaQuyen { get; set; }

        [ForeignKey("MaQuyen")]
        public Quyen? Quyen { get; set; } 

        [Display(Name = "Mã Nhân Viên")]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }
    }
}
