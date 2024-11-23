namespace NhanSuMVC.Models.ViewModels
{
    public class TongCongThangVM
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string PhongBan { get; set; }
        public int SoNgayCong { get; set; }
        public int SoNgayNghiKhongLuong { get; set; }
        public float LamThemNgayThuong { get; set; }
        public float LamNgayLe { get; set; }
        public float LamNgayNghi { get; set; }

        public int SoNgayNghiCoLuong { get; set; }
        public float TongGioLamThucTe { get; set; }
    }
}
