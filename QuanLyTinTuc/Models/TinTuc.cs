using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTinTuc.Models
{
    [Table("TINTUC")]
    public class TinTuc
    {
        [Key]
        [StringLength(10)]
        public string MaTinTuc { get; set; }

        public string TieuDe { get; set; }

        public string TomTat { get; set; }

        public string NoiDung { get; set; }

        public DateTime? NgayGui { get; set; }

        public string NguoiGui { get; set; }

        public string URLAnh { get; set; }

        public string ChuDe { get; set; }
    }
}