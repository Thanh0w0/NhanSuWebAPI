using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("ChucVu")]
    public class ChucVu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCV { get; set; }
        public string TenCV { get; set; }

    }
}
