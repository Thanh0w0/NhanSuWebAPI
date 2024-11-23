namespace NhanSuWebAPI.Models.VMs
{
    public class HomeAdminVM
    {
        public int NVDangLam { get; set; }
        public int NVDaNghi { get; set; }
        public int NPChuaDuyet { get; set; }
        public int NghiLeThangHienTai { get; set; }
        public List<NhanVienTheoPBVM> SLNhanVienPB { get; set; }
        public List<NhanVienTheoCVVM> SLNhanVienCV { get; set; }
    }

    public class NhanVienTheoPBVM
    {
        public string TenPhongBan { get; set; }
        public int SoLuongNhanVien { get; set; }
    }

    public class NhanVienTheoCVVM
    {
        public string TenChucVu { get; set; }
        public int SoLuongNhanVien { get; set; }
    }
}
