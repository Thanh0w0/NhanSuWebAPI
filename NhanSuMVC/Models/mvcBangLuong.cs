using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhanSuMVC.Models
{
    public class mvcBangLuong
    {

        public int MaLuong { get; set; }

        // Thông tin tháng và năm để xác định thời gian tính lương
        [Range(1, 12)]
        public int Thang { get; set; }

        public int Nam { get; set; }

        // Các khoản thu nhập
        [Column(TypeName = "decimal(18, 2)")]
        public decimal LuongCB { get; set; } // Lương cơ bản
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongPhuCap { get; set; } // Tổng phụ cấp
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongThuong { get; set; } // Tổng tiền thưởng
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongThuNhap { get; set; } // Tổng thu nhập trước thuế

        // Các khoản bảo hiểm cá nhân đóng góp
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHXH_NLD { get; set; } // BHXH người lao động
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHTN_NLD { get; set; } // BHTN người lao động
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHYT_NLD { get; set; } // BHYT người lao động

        // Các khoản bảo hiểm doanh nghiệp đóng góp
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHXH_DN { get; set; } // BHXH doanh nghiệp
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHTN_DN { get; set; } // BHTN doanh nghiệp
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BHYT_DN { get; set; } // BHYT doanh nghiệp

        // Các khoản khác
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongPhat { get; set; } // Tổng phạt
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTU { get; set; }

        // Các khoản thuế và lương thực nhận
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ThueTNCN { get; set; } // Thuế TNCN

        [Column(TypeName = "decimal(18, 2)")]
        public decimal LuongThucLanh { get; set; } // Lương thực lãnh sau thuế và các khoản trừ

        // Tổng tiền tăng ca và tiền làm việc trong ngày lễ/tết
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTienTC { get; set; }

        [Required]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public mvcNhanVien? NhanVien { get; set; }

    }
}
