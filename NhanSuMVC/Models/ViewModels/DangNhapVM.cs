using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models.ViewModels
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string TenDN { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
        public int MaQuyen { get; set; } // Mã quyền
        public string? Quyen { get; set; } // Tên quyền
  
    }
}
