using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcPhongBan
    {
        [Key]
        public int MaPB { get; set; }
        public string TenPB { get; set; }
    }
}
