namespace NhanSuMVC.Models.ViewModels
{
    public class NhanVienVM
    {
        // Employee information
        public mvcNhanVien NhanVien { get; set; }

        // Account details
        public mvcTaiKhoan TaiKhoan { get; set; }

        // Contract information
        public List<mvcHopDong> HopDong { get; set; }

        // Allowance information
        public List<mvcPhuCap> PhuCap { get; set; }

        public NhanVienVM()
        {
            NhanVien = new mvcNhanVien();
            TaiKhoan = new mvcTaiKhoan();
            HopDong = new List<mvcHopDong>();
            PhuCap = new List<mvcPhuCap>();
        }

    }
}
