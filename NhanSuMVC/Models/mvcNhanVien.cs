using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuMVC.Models
{
    public class mvcNhanVien
    {
        [Key]
        [Display(Name = "Mã NV")]
        public string MaNV { get; set; }

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "Họ Tên không được để trống.")]
        public string HoTen { get; set; }

        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Ngày Sinh không được để trống.")]
        public DateTime NgaySinh { get; set; }

        [Display(Name = "Giới Tính")]
        [Required(ErrorMessage = "Giới Tính không được để trống.")]
        public string GioiTinh { get; set; }

        [Display(Name = "Dân Tộc")]
        [Required(ErrorMessage = "Dân Tộc không được để trống.")]
        public string DanToc { get; set; }

        [Display(Name = "Tôn Giáo")]
        [Required(ErrorMessage = "Tôn Giáo không được để trống.")]
        public string TonGiao { get; set; }

        [Display(Name = "CCCD")]
        [Required(ErrorMessage = "CCCD không được để trống.")]
        [StringLength(12, ErrorMessage = "CCCD phải có 12 ký tự.")]

        public string CCCD { get; set; }

        [Display(Name = "Nơi Sinh")]
        [Required(ErrorMessage = "Nơi Sinh không được để trống.")]
        public string NoiSinh { get; set; }

        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "Địa Chỉ không được để trống.")]
        public string DiaChi { get; set; }

        [Display(Name = "Số Điện Thoại")]
        [Required(ErrorMessage = "Số Điện Thoại không được để trống.")]
        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Trình Độ")]
        [Required(ErrorMessage = "Trình Độ không được để trống.")]
        public string TrinhDo { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Required(ErrorMessage = "Email không được để trống.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Trạng Thái")]
        [Required(ErrorMessage = "Trạng Thái không được để trống.")]
        public string TrangThai { get; set; }
        [Display(Name = "Phòng Ban")]
        [Required(ErrorMessage = "Phòng Ban không được để trống.")]
        public int MaPB { get; set; }

        [ForeignKey("MaPB")]
        public mvcPhongBan? PhongBan { get; set; }
        [Display(Name = "Chức Vụ")]
        [Required(ErrorMessage = "Chức Vụ không được để trống.")]
        public int MaCV { get; set; }

        [ForeignKey("MaCV")]
        public mvcChucVu? ChucVu { get; set; }

        [NotMapped]
        public IFormFile? ImageUpload { get; set; }

        public mvcNhanVien()
        {
            HinhAnh = "~/img/account.png";
        }
    }
}
