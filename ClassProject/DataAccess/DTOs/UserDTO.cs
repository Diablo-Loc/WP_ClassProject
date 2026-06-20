using System;

namespace ClassProject.DataAccess.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Valid { get; set; } // 1: Đã xác thực, 0: Chưa
        public int Status { get; set; } // 0: Hoạt động, 1: Bị khóa
        public int FailedAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}