using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MimeKit;
using MailKit.Net.Smtp;
using BCrypt.Net;
using ClassProject.DataAccess.Db;

namespace ClassProject.Presentation.Forms
{
    public partial class ForgetPassForm : Form
    {
        private string _otp = "";
        private string _verifiedEmail = "";

        My_DB db = new My_DB();

        public ForgetPassForm()
        {
            InitializeComponent();
            // Ẩn phần OTP và mật khẩu mới lúc đầu
            SetOTPSectionVisible(false);
        }

        private void btnSendOTP_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (email == "")
            {
                MessageBox.Show("Vui lòng nhập email!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra email có trong DB không
            if (!EmailExists(email))
            {
                MessageBox.Show("Email không tồn tại trong hệ thống!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo OTP 6 số
            _otp = new Random().Next(100000, 999999).ToString();
            _verifiedEmail = email;

            // Gửi email
            if (SendOTPEmail(email, _otp))
            {
                MessageBox.Show("OTP đã được gửi đến email của bạn!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetOTPSectionVisible(true);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string inputOTP = txtOTP.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirm.Text.Trim();

            if (inputOTP != _otp || _otp == "")
            {
                MessageBox.Show("OTP không đúng!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword == "" || confirmPassword == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu không khớp!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password = @password WHERE Email = @email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(newPassword));
                        cmd.Parameters.AddWithValue("@email", _verifiedEmail);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đặt lại mật khẩu thành công!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoginForm f = new LoginForm();
                f.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            LoginForm f = new LoginForm();
            f.Show();
            this.Hide();
        }

        // ==================== HÀM HỖ TRỢ ====================
        private bool EmailExists(string email)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private bool SendOTPEmail(string toEmail, string otp)
        {
            try
            {
                // Đọc từ app.config
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"];
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderAppPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ClassProject", senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Mã OTP đặt lại mật khẩu";
                message.Body = new TextPart("plain")
                {
                    Text = $"Mã OTP của bạn là: {otp}\n\nMã này chỉ có hiệu lực trong 5 phút."
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(senderEmail, senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi email: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void SetOTPSectionVisible(bool visible)
        {
            lblOTP.Visible = visible;
            txtOTP.Visible = visible;
            lblNewPassword.Visible = visible;
            txtNewPassword.Visible = visible;
            lblConfirmPassword.Visible = visible;
            txtConfirm.Visible = visible;
            btnReset.Visible = visible;
        }
    }
}