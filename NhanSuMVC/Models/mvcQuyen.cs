using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcQuyen
    {
        [Key]
        public int MaQuyen { get; set; }
        public string TenQuyen { get; set; }
    }
}
