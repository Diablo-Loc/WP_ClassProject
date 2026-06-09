using BCrypt.Net;
using ClassProject.DataAccess.Db;
using ClassProject.Models;
using ClassProject.Presentation.Forms;
using ClassProject.Presentation.Forms.Main;
using Microsoft.Data.SqlClient;
using System;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class LoginForm : Form
    {
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

            // 1. Tối ưu UX: Báo lỗi và focus đúng ô
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

            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // 2. CẬP NHẬT TRUY VẤN: Lấy thêm cột Status từ bảng Users
                    string query = @"SELECT Id, Password, RoleId, 
                                            ISNULL(Valid, 0) AS Valid, 
                                            ISNULL(Status, 0) AS Status, 
                                            ISNULL(FailedAttempts, 0) AS FailedAttempts, 
                                            LockoutEnd 
                                     FROM Users WHERE Username = @user";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", username);

                    bool userExists = false;
                    string hashedPassword = "";
                    int userId = 0;
                    int roleId = -1;
                    int valid = 0;
                    int status = 0;
                    int failedAttempts = 0;
                    DateTime? lockoutEnd = null;

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

                            if (reader["LockoutEnd"] != DBNull.Value)
                            {
                                lockoutEnd = Convert.ToDateTime(reader["LockoutEnd"]);
                            }
                        }
                    }

                    // 3. Xử lý các case bảo mật và nghiệp vụ
                    if (!userExists)
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtUsername.Focus();
                        return;
                    }

                    // Case 3a: Tài khoản đã bị Admin khóa hệ thống (Status = 2)
                    if (status == 2)
                    {
                        MessageBox.Show("Tài khoản của bạn đã bị Admin KHÓA TRUY CẬP hệ thống.\nVui lòng liên hệ phòng quản lý để được hỗ trợ!",
                                        "Tài khoản bị khóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    // Case 3b: Tài khoản đã bị Admin từ chối phê duyệt thẳng thừng (Status = 3)
                    if (status == 3)
                    {
                        MessageBox.Show("Yêu cầu đăng ký tài khoản này đã bị Admin TỪ CHỐI phê duyệt!",
                                        "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    // Case 3c: Tài khoản đã bị xóa mềm (Status = -1), coi như không tồn tại nữa
                    if (status == -1)
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtUsername.Focus();
                        return;
                    }

                    // Case: Tài khoản đang bị khóa tạm thời do tự nhập sai pass nhiều lần (LockoutEnd)
                    if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.Now)
                    {
                        TimeSpan waitTime = lockoutEnd.Value - DateTime.Now;
                        MessageBox.Show($"Tài khoản đang bị khóa do nhập sai nhiều lần.\nVui lòng thử lại sau {waitTime.Minutes} phút {waitTime.Seconds} giây.",
                                        "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    // Case: Tài khoản chưa được Admin duyệt (VALID = 0 hoặc Status = 0)
                    if (valid == 0 || status == 0)
                    {
                        MessageBox.Show("Tài khoản của bạn đang chờ duyệt, chưa được Admin phê duyệt vào hệ thống!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    // 4. Xác thực Mật khẩu qua BCrypt
                    if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
                        // Đăng nhập THÀNH CÔNG: Reset số lần sai về 0
                        UpdateLoginStatus(conn, username, 0, null);

                        // 🌟 BƯỚC THAY ĐỔI QUAN TRỌNG: Nạp thông tin phiên đăng nhập toàn cục (Global Session)
                        UserSession.UserId = userId;
                        UserSession.RoleId = roleId;
                        UserSession.Username = username;
                        // (Lưu ý: Thuộc tính UserSession.MSSV tạm thời để trống, f_main sẽ tự truy vấn sau nếu roleId == 1)

                        // Lưu trạng thái Remember Me
                        Properties.Settings.Default.Username = chkRememberMe.Checked ? username : "";
                        Properties.Settings.Default.Password = chkRememberMe.Checked ? password : "";
                        Properties.Settings.Default.RememberMe = chkRememberMe.Checked;
                        Properties.Settings.Default.Save();

                        string roleName = roleId == 0 ? "Admin" : (roleId == 1 ? "Sinh viên" : "Giảng viên");
                        MessageBox.Show($"Đăng nhập {roleName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();

                        // 🌟 BƯỚC THAY ĐỔI QUAN TRỌNG: Khởi tạo f_main() trống không cần truyền tham số
                        using (f_main mainForm = new f_main())
                        {
                            mainForm.ShowDialog();
                        }

                        if (Application.OpenForms.Count > 0)
                        {
                            this.Show();
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                    else
                    {
                        // Đăng nhập THẤT BẠI
                        DateTime? newLockout = null;

                        if (failedAttempts < 5)
                        {
                            failedAttempts++;
                        }

                        if (failedAttempts >= 5)
                        {
                            newLockout = DateTime.Now.AddMinutes(15); // Khóa tạm thời 15 phút
                            MessageBox.Show("Sai mật khẩu! Tài khoản đã bị khóa tạm thời 15 phút.", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Sai tài khoản hoặc mật khẩu! Bạn còn {5 - failedAttempts} lần thử.", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        UpdateLoginStatus(conn, username, failedAttempts, newLockout);

                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                if (Properties.Settings.Default.RememberMe)
                {
                    txtUsername.Text = Properties.Settings.Default.Username;
                    txtPassword.Text = Properties.Settings.Default.Password;
                    chkRememberMe.Checked = true;
                }
            }
            if (!string.IsNullOrEmpty(txtUsername.Text))
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
    }
}