using ClassProject.DataAccess.Db;
using BCrypt.Net;
using ClassProject.Presentation.Forms;
using ClassProject.Presentation.Forms.Main; // Đảm bảo nạp đúng namespace của MainForm
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

            if (username == "" || password == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Password!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Lấy password hash từ DB
                    string query = "SELECT Id, Password, RoleId FROM Users WHERE Username = @user";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", username);

                    SqlDataReader reader = cmd.ExecuteReader();

                    string? hashedPassword = null;
                    int userId = 0;
                    int roleId = -1;

                    if (reader.Read())
                    {
                        hashedPassword = reader["Password"].ToString();
                        userId = Convert.ToInt32(reader["Id"]);
                        roleId = Convert.ToInt32(reader["RoleId"]);
                    }

                    reader.Close();

                    // Kiểm tra username tồn tại và verify password qua BCrypt
                    if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
                        // Lưu trạng thái Remember Me
                        if (chkRememberMe.Checked)
                        {
                            Properties.Settings.Default.Username = username;
                            Properties.Settings.Default.Password = password;
                            Properties.Settings.Default.RememberMe = true;
                        }
                        else
                        {
                            Properties.Settings.Default.Username = "";
                            Properties.Settings.Default.Password = "";
                            Properties.Settings.Default.RememberMe = false;
                        }
                        Properties.Settings.Default.Save();

                        // THÔNG BÁO THÀNH CÔNG THEO QUYỀN
                        string roleName = roleId == 0 ? "Admin" : (roleId == 1 ? "Sinh viên" : "Giảng viên");
                        MessageBox.Show($"Đăng nhập tài khoản {roleName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();

                        // Truyền cả roleId và userId sang để MainForm xử lý phân quyền và mở form con
                        using (MainForm mainForm = new MainForm(roleId, userId))
                        {
                            mainForm.ShowDialog();
                        }

                        // Khi tắt MainForm (hoặc bấm Đăng xuất), quay trở lại hiện Form Login
                        if (Application.OpenForms.Count > 0)
                        {
                            this.Show();
                            txtPassword.Clear();
                            txtPassword.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối DB: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            f.Show();
            this.Hide();
        }

        private void lblForgetPassword_Click(object sender, EventArgs e)
        {
            ForgetPassForm f = new ForgetPassForm();
            f.Show();
            this.Hide();
        }
    }
}