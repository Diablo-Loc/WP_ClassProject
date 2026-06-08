using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassProject.Models
{
    public class Classroom
    {
        public string MaLop { get; set; }
        public string TenLop { get; set; }
        public int SiSo { get; set; }
        public string GVCN { get; set; }

        // Hàm khởi tạo không tham số
        public Classroom() { }

        // Hàm khởi tạo đầy đủ tham số để tiện dùng khi cần
        public Classroom(string maLop, string tenLop, int siSo, string gvcn)
        {
            MaLop = maLop;
            TenLop = tenLop;
            SiSo = siSo;
            GVCN = gvcn;
        }
    }
}
