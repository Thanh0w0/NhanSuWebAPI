namespace NhanSuWebAPI.Models.VMs
{
    public class TKChamCongVM
    {
        public string TenPB { get; set; } // Tên phòng ban
        public int SoNhanVien { get; set; } // Tổng số nhân viên
        public int SoDiTre { get; set; } // Số nhân viên đi trễ
        public int SoDiDungGio { get; set; } // Số nhân viên đến đúng giờ
    }
}
