using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcLoaiPC
    {
        [Key]
        public int MaLoaiPC { get; set; }
        public string TenLoaiPC { get; set; }
        public double MucPC { get; set; }
    }
}
