using System;

namespace ClassProject.DataAccess.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Cho phép null nếu tài khoản chưa được liên kết
        public string MSGV { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AcademicRank { get; set; }
        public int Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime? Updated_At { get; set; }

        // Thuộc tính mở rộng phục vụ hiển thị nhanh lên UI
        public string FullName => $"{LastName} {FirstName}".Trim();
    }
}