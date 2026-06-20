using System;

namespace ClassProject.DataAccess.Entities
{
    public class Course
    {
        private string _maMH;
        private string _tenMH;
        private int? _soTC;
        private int? _tuan;

        // Khóa chính: NVARCHAR(10) trong DB mới -> dùng string, tự động viết hoa
        public string MaMH
        {
            get => _maMH;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Mã môn học không được để trống!");
                _maMH = value.Trim().ToUpper();
            }
        }

        // TC06: Chặn trống, giới hạn tối đa 100 ký tự theo đúng Schema của bạn
        public string TenMH
        {
            get => _tenMH;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Tên môn học không được để trống!");
                if (value.Trim().Length > 100)
                    throw new InvalidOperationException("[TC06] Tên môn học không được vượt quá 100 ký tự!");
                _tenMH = value.Trim();
            }
        }

        // TC03 & TC04: Giới hạn số tín chỉ từ 1 đến 10
        public int? SoTC
        {
            get => _soTC;
            set
            {
                if (value.HasValue && (value.Value < 1 || value.Value > 10))
                    throw new InvalidOperationException("[TC03/TC04] Số tín chỉ phải nằm trong khoảng từ 1 đến 10!");
                _soTC = value;
            }
        }

        // TC05: Số tuần học phải > 0 và <= 30
        public int? Tuan
        {
            get => _tuan;
            set
            {
                if (value.HasValue && (value.Value <= 0 || value.Value > 30))
                    throw new InvalidOperationException("[TC05] Số tuần học phải lớn hơn 0 và không quá 30 tuần!");
                _tuan = value;
            }
        }

        public int? Hky { get; set; }
        public string NamHoc { get; set; } // Phục vụ quản lý năm học của môn nếu có
        public string Mota { get; set; }   // Khớp với cột Mota NVARCHAR(500)
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }

        public Course() { }

        public Course(string maMH, string tenMH, int soTC, int tuan, int hky, string mota)
        {
            this.MaMH = maMH;
            this.TenMH = tenMH;
            this.SoTC = soTC;
            this.Tuan = tuan;
            this.Hky = hky;
            this.Mota = mota;
        }
    }
}