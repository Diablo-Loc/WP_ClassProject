using System;

namespace ClassProject.DataAccess.Entities
{
    public class Student
    {
        // Fields
        private int? _userId;
        private string _mssv;
        private string _firstName;
        private string _lastName;
        private DateTime? _dateOfBirth;
        private string _gender;
        private string _phone;
        private string _address;
        private string _hometown;
        private string _email;
        private byte[] _picture;

        // Properties
        public int? UserId
        {
            get { return _userId; }
            set
            {
                if (value.HasValue && value.Value <= 0)
                    throw new InvalidOperationException("UserId không hợp lệ!");
                _userId = value;
            }
        }

        public string Mssv
        {
            get { return _mssv; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("MSSV không được để trống!");
                _mssv = value;
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Tên không được để trống!");
                _firstName = value;
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("Họ không được để trống!");
                _lastName = value;
            }
        }

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                // Chỉ validate nếu có giá trị, tránh bẫy crash ứng dụng
                if (value.HasValue && value.Value > DateTime.Now)
                    throw new InvalidOperationException("Ngày sinh không được lớn hơn hiện tại!");

                if (value.HasValue && value.Value < new DateTime(1753, 1, 1))
                    throw new InvalidOperationException("Ngày sinh không hợp lệ cho hệ thống dữ liệu!");

                _dateOfBirth = value;
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                // Cho phép null hoặc chuỗi rỗng vì DB cho phép NULL, nếu có giá trị thì phải chuẩn Nam/Nữ
                if (!string.IsNullOrEmpty(value) && value != "Nam" && value != "Nữ")
                    throw new InvalidOperationException("Giới tính chỉ được là 'Nam' hoặc 'Nữ'!");
                _gender = value;
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                // Chống crash NullReference: Chỉ check độ dài khi chuỗi có dữ liệu
                if (!string.IsNullOrEmpty(value))
                {
                    string temp = value.Trim();
                    if (temp.Length < 10 || temp.Length > 11)
                        throw new InvalidOperationException("Số điện thoại phải từ 10 đến 11 số!");
                }
                _phone = value;
            }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string Hometown
        {
            get { return _hometown; }
            set { _hometown = value; }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                // Chống crash NullReference: Chỉ validate định dạng nếu Email khác null
                if (!string.IsNullOrEmpty(value) && !value.Contains("@"))
                    throw new InvalidOperationException("Định dạng Email không hợp lệ (Thiếu ký tự @)!");
                _email = value;
            }
        }

        public byte[] Picture
        {
            get { return _picture; }
            set
            {
                // Đồng bộ chuẩn 2MB chống phình DB như đã cam kết ở tầng giao diện
                if (value != null && value.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("Dung lượng hình ảnh vượt quá giới hạn cho phép (Tối đa 2MB)!");
                _picture = value;
            }
        }

        // Auto-implemented Properties bổ trợ hiển thị danh mục
        public string MaLop { get; set; }
        public string TenLop { get; set; }
        public string MaNganh { get; set; }
        public string TenNganh { get; set; }

        // Constructors
        public Student() { }

        public Student(int? userId, string mssv, string firstName, string lastName, DateTime? dateOfBirth,
               string gender, string phone, string address, string hometown,
               string email, byte[] picture = null, string maLop = null, string maNganh = null)
        {
            UserId = userId;
            Mssv = mssv;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Phone = phone;
            Address = address;
            Hometown = hometown;
            Email = email;
            Picture = picture;
            MaLop = maLop;
            MaNganh = maNganh;
        }

        public string FullName => $"{LastName} {FirstName}";

        public override string ToString()
        {
            return $"MSSV      : {Mssv}\n" +
                   $"Họ tên    : {FullName}\n" +
                   $"Lớp       : {TenLop ?? "Chưa xếp lớp"}\n" +
                   $"Ngành     : {TenNganh ?? "Chưa phân ngành"}\n" +
                   // Dùng toán tử điều kiện kiểm tra ngày sinh để tránh hiển thị ngày mặc định bẩn dữ liệu
                   $"Ngày sinh : {(DateOfBirth.HasValue ? DateOfBirth.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật")}\n" +
                   $"Giới tính : {Gender ?? "Chưa cập nhật"}\n" +
                   $"SĐT       : {Phone ?? "Chưa cập nhật"}\n" +
                   $"Địa chỉ   : {Address ?? "Chưa cập nhật"}\n" +
                   $"Quê quán  : {Hometown ?? "Chưa cập nhật"}\n" +
                   $"Email     : {Email ?? "Chưa cập nhật"}";
        }
    }
}