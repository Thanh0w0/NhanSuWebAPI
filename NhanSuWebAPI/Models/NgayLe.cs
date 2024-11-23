using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuWebAPI.Models
{
    [Table("NgayLe")]
    public class NgayLe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNL { get; set; } 

        public DateTime NgayBD { get; set; } 

        public DateTime NgayKT { get; set; } 

        public string TenNgayLe { get; set; } 
    }
}
