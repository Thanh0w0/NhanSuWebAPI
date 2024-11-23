using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("LoaiHD")]
    public class LoaiHD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLoaiHD { get; set; }
        public string TenLoaiHD{ get; set; }
        
    }
}
