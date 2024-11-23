using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcLoaiHD
    {
        [Key]
        
        public int MaLoaiHD { get; set; }
        public string TenLoaiHD { get; set; }

    }
}
