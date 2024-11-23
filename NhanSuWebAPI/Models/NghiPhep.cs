﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhanSuWebAPI.Models
{
    [Table("NghiPhep")]
    public class NghiPhep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int MaNP { get; set; }

        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
        public NhanVien? NhanVien { get; set; }
  
        public DateTime TuNgay { get; set; } 
        public DateTime DenNgay { get; set; } 
        public string? LyDo { get; set; }
        public string TrangThai { get; set; }
        public string LoaiNP { get; set; }

        public int? SoNgayNghiCoLuong { get; set; } 


    }
}