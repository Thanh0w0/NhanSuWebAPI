using System.ComponentModel.DataAnnotations;

namespace NhanSuWebAPI.Models.VMs
{
    public class LoginVM
    {
        [Required]
        public string TenDN { get; set; }

        [Required]
        public string MatKhau { get; set; }
    }
}
