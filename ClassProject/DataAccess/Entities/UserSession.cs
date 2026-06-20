using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.DataAccess.Entities
{
    public static class UserSession
    {
        // Các trường backing-field private để bảo vệ dữ liệu chống ghi đè tùy tiện
        private static int _roleId = -1;
        private static int _userId = -1;
        private static string _mssv = string.Empty;
        private static string _username = string.Empty;
        private static string _fullName = string.Empty;
        private static string _email = string.Empty;
        private static string _teacherId = string.Empty;

        //THÀNH PHẦN AI AUTOLOGOUT: HỆ THỐNG GIÁM SÁT TIMEOUT
        private static System.Windows.Forms.Timer _sessionTimer;
        private static DateTime _lastActivityTime;
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30); // Ngưỡng 30 phút

        #region Properties (Thread-safe hoặc đơn giản hóa cho WinForms)

        public static int RoleId => _roleId;
        public static int UserId => _userId;
        public static string MSSV => _mssv;
        public static string Username => _username;
        public static string FullName => _fullName;
        public static string Email => _email;
        public static string TeacherId => _teacherId;

        /// Trạng thái đăng nhập thực tế của hệ thống
        public static bool IsLoggedIn => _userId != -1;

        #region Trợ lý Phân quyền (RBAC Helpers) - Chuẩn Global chống Hardcode

        public static bool IsAdmin => _roleId == 0;
        public static bool IsStudent => _roleId == 1;
        public static bool IsStaff => _roleId == 3;

        public static bool IsTeacher => _roleId == 2;

        public static string RoleName
        {
            get
            {
                if (IsAdmin) return "Administrator";
                if (IsStudent) return "Student";
                if (IsStaff) return "Giáo vụ / Phòng đào tạo";
                if (IsTeacher) return "Giảng viên";
                return "Guest";
            }
        }

        #endregion

        #endregion

        #region Các Thao Tác Nghiệp Vụ Cốt Lõi (Core Operations)

        /// Khởi tạo một phiên làm việc mới sau khi xác thực thành công.
        public static void Initialize(int userId, string username, int roleId, string email = "", string fullName = "", string mssv = "", string teacherId = "")
        {
            if (userId <= 0)
                throw new ArgumentException("Id người dùng không hợp lệ để khởi tạo phiên.");

            _userId = userId;
            _username = username?.Trim() ?? string.Empty;
            _roleId = roleId;
            _email = email?.Trim() ?? string.Empty;
            _fullName = fullName?.Trim() ?? string.Empty;
            _mssv = mssv?.Trim() ?? string.Empty;
            _teacherId = teacherId?.Trim() ?? string.Empty;
            _lastActivityTime = DateTime.Now;
            StartSessionMonitoring();
        }

        /// BÀI BÁO TƯƠNG TÁC: Gọi hàm này từ form chính khi người dùng click chuột/gõ phím
        public static void RefreshActivity()
        {
            if (IsLoggedIn)
            {
                _lastActivityTime = DateTime.Now;
            }
        }

        // Khởi tạo và chạy Timer ngầm theo dõi 30 phút
        private static void StartSessionMonitoring()
        {
            if (_sessionTimer == null)
            {
                _sessionTimer = new System.Windows.Forms.Timer();
                _sessionTimer.Interval = 10000; // Cứ 10 giây quét một lần (tối ưu CPU)
                _sessionTimer.Tick += SessionTimer_Tick;
            }
            _sessionTimer.Start();
        }

        // Xử lý sự kiện khi hết hạn 30 phút không tương tác
        private static void SessionTimer_Tick(object sender, EventArgs e)
        {
            if (!IsLoggedIn) return;

            if (DateTime.Now - _lastActivityTime >= SessionTimeout)
            {
                StopSessionMonitoring();
                Clear(); // Xóa sạch dữ liệu

                MessageBox.Show("Phiên làm việc đã hết hạn do 30 phút bạn không tương tác.\nHệ thống sẽ tự động đăng xuất để bảo mật thông tin!",
                                "Thông báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Khởi động lại toàn bộ ứng dụng, đẩy người dùng về màn hình Login ban đầu
                Application.Restart();
            }
        }

        private static void StopSessionMonitoring()
        {
            if (_sessionTimer != null)
            {
                _sessionTimer.Stop();
            }
        }

        /// Cập nhật riêng biệt MSSV sau khi Form Main đồng bộ từ Database.
        public static void UpdateStudentMssv(string mssv)
        {
            if (IsStudent)
            {
                _mssv = mssv?.Trim() ?? string.Empty;
            }
        }

        /// Cập nhật riêng biệt mã số giảng viên (MSGV) sau khi đồng bộ.
        public static void UpdateTeacherId(string teacherId)
        {
            if (IsTeacher)
            {
                _teacherId = teacherId?.Trim() ?? string.Empty;
            }
        }

        /// Xóa bỏ hoàn toàn dấu vết phiên làm việc khi đăng xuất.
        public static void Clear()
        {
            _roleId = -1;
            _userId = -1;
            _mssv = string.Empty;
            _username = string.Empty;
            _email = string.Empty;
            _fullName = string.Empty;  
            _teacherId = string.Empty;
            StopSessionMonitoring();
        }

        #endregion
    }
}