using System;

namespace ClassProject.Models
{
    public class Student
    {
        // Fields
        private int _userId;
        private int _mssv;
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;
        private string _gender;
        private string _phone;
        private string _address;
        private string _hometown;
        private string _email;
        private byte[] _picture;

        // Properties
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (value <= 0)
                    throw new Exception("UserId không hợp lệ!");
                _userId = value;
            }
        }
        public int Mssv
        {
            get { return _mssv; }
            set
            {
                if (value <= 0)
                    throw new Exception("MSSV không hợp lệ!");
                _mssv = value;
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Tên không được để trống!");
                _firstName = value;
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Họ không được để trống!");
                _lastName = value;
            }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                if (value > DateTime.Now)
                    throw new Exception("Ngày sinh không được lớn hơn hiện tại!");
                _dateOfBirth = value;
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                if (value != "Nam" && value != "Nữ")
                    throw new Exception("Giới tính chỉ được là 'Nam' hoặc 'Nữ'!");
                _gender = value;
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 10 || value.Length > 11)
                    throw new Exception("Số điện thoại không hợp lệ!");
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
                if (string.IsNullOrEmpty(value) || !value.Contains("@"))
                    throw new Exception("Email không hợp lệ!");
                _email = value;
            }
        }

        public byte[] Picture
        {
            get { return _picture; }
            set
            {
                if (value != null && value.Length > 5 * 1024 * 1024)
                    throw new Exception("Ảnh không được vượt quá 5MB!");
                _picture = value;
            }
        }

        // Constructor
        public Student(){ }
        public Student(int userId, int mssv, string firstName, string lastName, DateTime dateOfBirth,
                       string gender, string phone, string address, string hometown,
                       string email, byte[] picture = null)
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
        }

        // Họ tên đầy đủ
        public string FullName => $"{LastName} {FirstName}";

        // Hiển thị thông tin
        public override string ToString()
        {
            return $"MSSV      : {Mssv}\n" +
                   $"Họ tên    : {FullName}\n" +
                   $"Ngày sinh : {DateOfBirth:dd/MM/yyyy}\n" +
                   $"Giới tính : {Gender}\n" +
                   $"SĐT       : {Phone}\n" +
                   $"Địa chỉ   : {Address}\n" +
                   $"Quê quán  : {Hometown}\n" +
                   $"Email     : {Email}\n" +
                   $"Ảnh       : {(Picture != null ? "Có" : "Chưa có")}";
        }
    }
}
