using System;

namespace ClassProject.Models
{
    public class Classroom
    {
        private string _maLop;
        private string _tenLop;
        private string _gvcn;
        private string _maNganh;
        private string _status;

        // Properties mã lớp - Khóa chính
        public string MaLop
        {
            get => _maLop;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Mã lớp học không được để trống!");
                _maLop = value.Trim();
            }
        }

        // Tên lớp - Ràng buộc độ dài và dữ liệu trống
        public string TenLop
        {
            get => _tenLop;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Tên lớp học không được để trống!");
                _tenLop = value.Trim();
            }
        }

        // Giáo viên chủ nhiệm (Cho phép NULL nhưng nếu có phải chuẩn hóa)
        public string GVCN
        {
            get => _gvcn;
            set => _gvcn = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        // Mã ngành khóa ngoại (Cho phép NULL nhưng nếu có phải chuẩn hóa)
        public string MaNganh
        {
            get => _maNganh;
            set => _maNganh = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        // Trạng thái lớp học: 'Active' (Đang hoạt động), 'Inactive' (Tạm ngưng)
        public string Status
        {
            get => _status;
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "Active" && value != "Inactive")
                    throw new InvalidOperationException("Trạng thái lớp chỉ được là 'Active' hoặc 'Inactive'!");
                _status = string.IsNullOrWhiteSpace(value) ? "Active" : value.Trim();
            }
        }

        // Auto Properties bổ trợ phục vụ mapping hiển thị dữ liệu từ JOIN query
        public int SiSoThucTe { get; set; } // Tính toán động từ bảng Students, không lưu tĩnh
        public string TenNganh { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }

        // Constructors
        public Classroom()
        {
            _status = "Active";
        }

        public Classroom(string maLop, string tenLop, string gvcn = null, string maNganh = null, string status = "Active")
        {
            MaLop = maLop;
            TenLop = tenLop;
            GVCN = gvcn;
            MaNganh = maNganh;
            Status = status;
        }
    }
}