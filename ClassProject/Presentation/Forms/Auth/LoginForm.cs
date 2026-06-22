using BCrypt.Net;
using ClassProject.Business.Services;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.Presentation.Forms;
using ClassProject.Presentation.Forms.Auth;
using ClassProject.Presentation.Forms.Main;
using Microsoft.Data.SqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class LoginForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly SecurityMonitoringService _securityService = new SecurityMonitoringService();

        public LoginForm(string registeredUser = "")
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
            if (!string.IsNullOrEmpty(registeredUser))
            {
                txtUsername.Text = registeredUser;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Vui lòng nhập Username!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            bool userExists = false;
            string hashedPassword = string.Empty;
            int userId = 0;
            int roleId = -1;
            int valid = 0;
            int status = 0;
            int failedAttempts = 0;
            DateTime? lockoutEnd = null;

            // BỔ SUNG: Biến hứng Email từ DB phục vụ UserSession
            string email = string.Empty;
            string fullName = string.Empty;
            string teacherId = string.Empty;
            string mssv = string.Empty;

            using (SqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // CẬP NHẬT: Thêm cột Email vào SELECT query
                    string query = @"
                        SELECT 
                            u.Id, u.Password, u.RoleId, u.Email,
                            ISNULL(u.Valid, 0) AS Valid, 
                            ISNULL(u.Status, 0) AS Status, 
                            ISNULL(u.FailedAttempts, 0) AS FailedAttempts, 
                            u.LockoutEnd,
                            -- LẤY THÊM MÃ VẬT LÝ Ở ĐÂY
                            ISNULL(t.MSGV, '') AS TeacherId,
                            ISNULL(s.MSSV, '') AS StudentMssv,
                            -- Ghép Họ + Tên dựa theo RoleId thực tế
                            CASE 
                                WHEN u.RoleId = 1 THEN ISNULL(s.LastName + ' ' + s.FirstName, '')
                                WHEN u.RoleId = 2 THEN ISNULL(t.LastName + ' ' + t.FirstName, '')
                                WHEN u.RoleId = 3 THEN ISNULL(st.LastName + ' ' + st.FirstName, '')
                                ELSE N'Hệ thống Administrator'
                            END AS FullName
                        FROM Users u
                        LEFT JOIN Students s ON u.Id = s.UserId
                        LEFT JOIN Teachers t ON u.Id = t.UserId
                        LEFT JOIN Staffs st ON u.Id = st.UserId
                        WHERE u.Username = @user";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userExists = true;
                                hashedPassword = reader["Password"].ToString();
                                userId = Convert.ToInt32(reader["Id"]);
                                roleId = Convert.ToInt32(reader["RoleId"]);
                                valid = Convert.ToInt32(reader["Valid"]);
                                status = Convert.ToInt32(reader["Status"]);
                                failedAttempts = Convert.ToInt32(reader["FailedAttempts"]);

                                email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty;
                                fullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : string.Empty;
                                teacherId = reader["TeacherId"] != DBNull.Value ? reader["TeacherId"].ToString() : string.Empty;
                                mssv = reader["StudentMssv"] != DBNull.Value ? reader["StudentMssv"].ToString() : string.Empty;

                                if (reader["LockoutEnd"] != DBNull.Value)
                                {
                                    lockoutEnd = DateTime.SpecifyKind(Convert.ToDateTime(reader["LockoutEnd"]), DateTimeKind.Local);
                                }
                            }
                        }
                    }

                    // BƯỚC 1: KIỂM TRA SỰ TỒN TẠI CỦA TÀI KHOẢN
                    if (!userExists || status == -1) // status = -1 là xóa mềm
                    {
                        _securityService.ProcessSecurityAudit(username, isSuccess: false, method: "PASSWORD", userEmail: "", failureReason: "Tai khoan khong ton tai");
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtUsername.Focus();
                        return;
                    }

                    // BƯỚC 2: KIỂM TRA KHÓA LOCKOUT TẠM THỜI (ANTI BRUTE-FORCE)
                    if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.Now)
                    {
                        TimeSpan waitTime = lockoutEnd.Value - DateTime.Now;
                        string timeString = waitTime.Minutes > 0
                            ? $"{waitTime.Minutes} phút {waitTime.Seconds} giây"
                            : $"{waitTime.Seconds} giây";
                        // KIỂM TOÁN NGẦM: Cố tình đăng nhập khi đang trong trạng thái bị khóa cứng
                        _securityService.ProcessSecurityAudit(username, isSuccess: false, method: "PASSWORD", userEmail: email, failureReason: "Co tinh truy cap khi dang lockout");
                        MessageBox.Show($"Tài khoản đang bị khóa do nhập sai quá 5 lần.\nVui lòng thử lại sau {timeString}.",
                                        "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txtPassword.Clear();
                        txtPassword.Focus();
                        return;
                    }

                    // BƯỚC 3: XÁC THỰC MẬT KHẨU BĂM (BCRYPT)
                    if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
                        // ĐẶC CÁCH BẢO MẬT: Nếu là Admin tối cao (RoleId == 0), bỏ qua mọi bước duyệt để cứu hộ hệ thống
                        if (roleId != 0)
                        {
                            // 1. CHỐT CHẶN PHÊ DUYỆT (Valid) - SQL gốc mặc định là 0
                            if (valid == 0)
                            {
                                MessageBox.Show("Tài khoản của bạn đang chờ phê duyệt. Chỉ tài khoản được Ban quản trị kích hoạt mới có thể đăng nhập hoặc dùng tính năng Quên mật khẩu!",
                                                "Tài khoản chưa kích hoạt", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            if (valid == 2)
                            {
                                MessageBox.Show("Đăng ký tài khoản này đã bị Ban quản trị TỪ CHỐI phê duyệt vĩnh viễn!",
                                                "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }

                            // 2. CHỐT CHẶN ĐIỀU HÀNH (Status) - Đồng bộ logic với ForgetPassForm
                            if (status == 1)
                            {
                                MessageBox.Show("Tài khoản của bạn hiện đang bị TẠM KHÓA để bảo trì hoặc kiểm tra.\nVui lòng thử lại sau!",
                                                "Tài khoản tạm khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (status == 2)
                            {
                                MessageBox.Show("Tài khoản của bạn đã bị Admin KHÓA TRUY CẬP vĩnh viễn do vi phạm chính sách!\nVui lòng liên hệ phòng quản lý để được hỗ trợ.",
                                                "Tài khoản bị khóa vĩnh viễn", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }

                        // ĐĂNG NHẬP THÀNH CÔNG -> Reset bộ đếm lỗi về 0
                        UpdateLoginStatus(conn, username, 0, null);

                        // CẬP NHẬT CHUẨN GLOBAL: Khởi tạo Identity Passport có kèm Email
                        // Do mới đăng nhập nên ta để trống mssv và teacherId để form Main đồng bộ sau
                        // KIỂM TOÁN NGẦM: Đăng nhập thành công, AI quét quy tắc "Giờ lạ"
                        _securityService.ProcessSecurityAudit(username, isSuccess: true, method: "PASSWORD", userEmail: email);
                        UserSession.Initialize(userId, username, roleId, email, fullName, mssv, teacherId);

                        // Xử lý Remember Me (DPAPI Bảo mật)
                        if (chkRememberMe.Checked)
                        {
                            Properties.Settings.Default.Username = username;
                            Properties.Settings.Default.Password = EncryptPassword(password);
                            Properties.Settings.Default.RememberMe = true;
                        }
                        else
                        {
                            Properties.Settings.Default.Username = string.Empty;
                            Properties.Settings.Default.Password = string.Empty;
                            Properties.Settings.Default.RememberMe = false;
                        }
                        Properties.Settings.Default.Save();

                        MessageBox.Show($"Đăng nhập vào hệ thống với vai trò [{UserSession.RoleName}] thành công!",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // THẤT BẠI: Tăng số lần gõ sai mật khẩu
                        failedAttempts++;
                        DateTime? newLockout = null;
                        string auditReason = "Sai mat khau lan " + failedAttempts;
                        if (failedAttempts >= 5)
                        {
                            newLockout = DateTime.Now.AddMinutes(15);
                            MessageBox.Show("Sai mật khẩu quá 5 lần! Tài khoản của bạn đã bị khóa tạm thời 15 phút.", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Sai tài khoản hoặc mật khẩu! Bạn còn {5 - failedAttempts} lần thử.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        UpdateLoginStatus(conn, username, failedAttempts, newLockout);
                        // KIỂM TOÁN NGẦM: Đăng nhập thất bại, AI quét quy tắc "Sai liên tiếp trong 5 phút"
                        _securityService.ProcessSecurityAudit(username, isSuccess: false, method: "PASSWORD", userEmail: email, failureReason: auditReason);
                        txtPassword.Clear();
                        txtPassword.Focus();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối hoặc thực thi CSDL: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } // Khối kết nối đóng hoàn toàn tại đây, giải phóng Connection cực kỳ an toàn trước khi chuyển Form

            // BƯỚC 4: ĐIỀU HƯỚNG SANG FORM CHÍNH NGOÀI CONNECTION POOL
            this.Hide();
            using (f_main mainForm = new f_main())
            {
                mainForm.ShowDialog();
            }
            this.Dispose();
            // THAY ĐỔI Ở ĐÂY: Khi mainForm đóng (ShowDialog kết thúc), thoát hẳn ứng dụng luôn
            Application.Exit();
            return; // Ngăn không cho chạy xuống đoạn code hiển thị lại LoginForm bên dưới
        }

        private string EncryptPassword(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string DecryptPassword(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedBytes = ProtectedData.Unprotect(cipherBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private void UpdateLoginStatus(SqlConnection conn, string username, int attempts, DateTime? lockoutEnd)
        {
            string query = "UPDATE Users SET FailedAttempts = @attempts, LockoutEnd = @lockout WHERE Username = @user";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@attempts", attempts);
                cmd.Parameters.AddWithValue("@user", username);

                if (lockoutEnd.HasValue)
                    cmd.Parameters.AddWithValue("@lockout", lockoutEnd.Value);
                else
                    cmd.Parameters.AddWithValue("@lockout", DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.RememberMe)
            {
                txtUsername.Text = Properties.Settings.Default.Username;
                string savedPassword = Properties.Settings.Default.Password;
                txtPassword.Text = DecryptPassword(savedPassword);
                chkRememberMe.Checked = true;
            }

            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                btnLogin.Focus();
            }
            else if (!string.IsNullOrEmpty(txtUsername.Text))
            {
                txtPassword.Focus();
            }
            else
            {
                txtUsername.Focus();
            }
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            RegisterForm f = new RegisterForm();
            f.FormClosed += (s, args) => this.Show();
            f.Show();
            this.Hide();
        }

        private void lblForgetPassword_Click(object sender, EventArgs e)
        {
            ForgetPassForm f = new ForgetPassForm();
            f.FormClosed += (s, args) => this.Show();
            f.Show();
            this.Hide();
        }

        private void btnOpenFaceID_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã gõ tên tài khoản vào ô nhập liệu chưa
            // (Bạn hãy đổi 'txtUsername' thành đúng tên ô TextBox nhập tài khoản trên giao diện của bạn nhé)
            string usernameInput = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(usernameInput))
            {
                MessageBox.Show("Vui lòng nhập Tên tài khoản trước khi mở xác thực Face ID!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            // 2.Truyền tài khoản và tham số 'false' (chế độ đăng nhập) vào Form
            using (FaceLoginForm faceForm = new FaceLoginForm(usernameInput, false))
            {
                this.Hide(); // Ẩn login cũ đi cho đẹp mắt
                var result = faceForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Nếu bên Form Quét mặt báo về OK -> Nghĩa là đã nạp UserSession xong -> Tiến hành mở thẳng f_main luôn!
                    using (f_main mainForm = new f_main())
                    {
                        mainForm.ShowDialog();
                    }
                }

                // Sau khi thoát f_main hoặc bấm nút hủy quét mặt -> Hiện lại màn hình Login ban đầu
                this.Show();
            }
        }
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                txtPassword.PasswordChar = '\0'; // Hiện rõ chữ mật khẩu
                chkShowPassword.Text = "Ẩn";     // Đổi chữ hiển thị thành "Ẩn" (hoặc "🙈")
            }
            else
            {
                txtPassword.PasswordChar = '●'; // Ẩn mật khẩu lại thành dấu chấm
                chkShowPassword.Text = "Hiện";   // Đổi chữ hiển thị thành "Hiện" (hoặc "👁")
            }
        }

    }
}