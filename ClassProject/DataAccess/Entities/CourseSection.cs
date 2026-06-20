using System;

namespace ClassProject.DataAccess.Entities
{
    public class CourseSection
    {
        private string _maLopHP;
        private string _maMH;
        private int _maxStudents;

        // Khóa chính: NVARCHAR(30) -> ép Unicode Safe
        public string MaLopHP
        {
            get => _maLopHP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Mã lớp học phần không được để trống!");
                _maLopHP = value.Trim().ToUpper();
            }
        }

        // Khóa ngoại: NVARCHAR(10) liên kết sang bảng Course
        public string MaMH
        {
            get => _maMH;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Mã môn học gốc không được để trống!");
                _maMH = value.Trim().ToUpper();
            }
        }

        public int HocKy { get; set; }
        public string NamHoc { get; set; }

        public string MSGV { get; set; }

        public string PhongHoc { get; set; }

        public int MaxStudents
        {
            get => _maxStudents;
            set
            {
                if (value <= 0)
                    throw new InvalidOperationException("Giới hạn sĩ số tối đa phải lớn hơn 0!");
                _maxStudents = value;
            }
        }

        public int Status { get; set; } // 1: Active, 0: Locked
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }

        // Mở rộng hiển thị trực quan lên DataGridView
        public string TenMH { get; set; }
        public int SisoHienTai { get; set; }

        public CourseSection()
        {
            this.MaxStudents = 50;
            this.Status = 1;
        }
    }
}