namespace NhanSuMVC.Models.ViewModels
{
    public class UserVM
    {
        public string MaNV { get; set; } // Tên quyền
        public string TenNV { get; set; } // Tên nhân viên
        public string HinhAnh { get; set; } // Hình ảnh nhân viên
        public string Quyen { get; set; }
        public string MatKhau { get; set; }

        public int MaQuyen { get; set; }

    }
}
