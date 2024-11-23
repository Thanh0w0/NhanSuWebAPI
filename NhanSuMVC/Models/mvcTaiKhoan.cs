using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcTaiKhoan
    {
        [Key]
        public int MaTK { get; set; }

        [Display(Name = "Tên Đăng Nhập")]
        public string TenDN { get; set; }

        [Display(Name = "Mật Khẩu")]
        public string MatKhau { get; set; }


        [Display(Name = "Ngày Tạo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Mã Quyền")]
        public int MaQuyen { get; set; }

        [ForeignKey("MaQuyen")]
        public mvcQuyen? Quyen { get; set; }

        [Display(Name = "Mã Nhân Viên")]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }
    }
}
