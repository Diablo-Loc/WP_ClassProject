using System;
using System.Collections.Generic;
using System.Text;

namespace ClassProject.Models
{
    public static class UserSession
    {
        public static int RoleId { get; set; }
        public static int UserId { get; set; }
        public static string MSSV { get; set; } = "";
        public static string Username { get; set; } = "";

        // Hàm này dùng để xóa sạch dữ liệu khi bấm nút "Đăng xuất"
        public static void Clear()
        {
            RoleId = -1;
            UserId = -1;
            MSSV = "";
            Username = "";
        }
    }
}
