using Microsoft.EntityFrameworkCore;

namespace NhanSuWebAPI.Models.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<PhongBan> PhongBans { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<LoaiHD> LoaiHDs { get; set; }
        
        public DbSet<NhanVien> NhanViens { get; set; }
        
        public DbSet<ChinhSach> ChinhSachs { get; set; }
        public DbSet<NghiPhep> NghiPheps { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
       
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<ChamCong> ChamCongs { get; set; }

        public DbSet<BangLuong> BangLuongs { get; set; }
        public DbSet<PhuCap> PhuCaps { get; set; }

        public DbSet<NgayLe> NgayLes { get; set; }
        public DbSet<TongHopCong> TongHopCongs { get; set; }
        public DbSet<TamUng> TamUngs { get; set; }





    }
}
