using ClassProject.DataAccess.Db;
using BCrypt.Net;
using ClassProject.Presentation.Forms;
using Microsoft.Data.SqlClient;

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
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Password!");
                return;
            }

            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Lấy password hash từ DB thay vì so sánh thẳng
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

                    // Kiểm tra username tồn tại và verify password
                    if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
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

                        if (roleId == 0)
                        {
                            MessageBox.Show("Đăng nhập Admin thành công!");

                            AddStudentForm f = new AddStudentForm(userId);
                            f.Show();

                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Đăng nhập thành công!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối DB: " + ex.Message);
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