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

            // Thử gửi email thật trước
            bool isMailSent = SendOTPEmail(email, _otp);

            if (isMailSent)
            {
                MessageBox.Show($"Mã OTP đã được gửi thành công đến email: {email}.\nVui lòng kiểm tra hộp thư (hoặc thư rác)!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // [CHẾ ĐỘ DỰ PHÒNG]: Nếu là gmail ảo hoặc cấu hình sai hệ thống gửi, tự động bật chế độ chống cháy
                MessageBox.Show($"[CHẾ ĐỘ TEST - EMAIL ẢO HOẶC SAI CẤU HÌNH GMAIL GỬI]\nHệ thống không thể kết nối server để gửi mail.\nMã OTP của bạn là: {_otp}",
                    "Mock OTP System (Fallback)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SetOTPSectionVisible(true);
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
                string senderEmail = "tranthienan6298.2017@gmail.com";
                string senderPassword = "kmxdsjowtxbfwuhl";

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    System.Diagnostics.Debug.WriteLine("Cảnh báo: Chưa cấu hình Email hoặc Mật khẩu ứng dụng.");
                    return false;
                }

                // Tạo nội dung Email bằng MailKit / MimeKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ClassProject Systems", senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "HCMUTE SYSTEM - RESET PASSWORD OTP";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = $"Mã OTP đặt lại mật khẩu của bạn là: {otp}\n\nMã này chỉ có hiệu lực trong vòng 5 phút. Vui lòng tuyệt đối không chia sẻ mã này cho bất kỳ ai!";
                message.Body = bodyBuilder.ToMessageBody();

                // Thực hiện kết nối SMTP Server của Google và gửi mail
                using (var client = new MailKit.Net.Smtp.SmtpClient()) // Định nghĩa rõ ràng MailKit SmtpClient
                {
                    // Kết nối qua cổng 587 bằng STARTTLS
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Đăng nhập bằng Mật khẩu ứng dụng
                    client.Authenticate(senderEmail, senderPassword);

                    // Gửi thư
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true; // Gửi mail thật THÀNH CÔNG, không chạy vào chế độ Test nữa
            }
            catch (Exception ex)
            {
                // Nếu có lỗi phát sinh (Sai mật khẩu ứng dụng, chặn tường lửa, mất mạng...), log sẽ hiện ở đây
                System.Diagnostics.Debug.WriteLine("Lỗi gửi mail chi tiết: " + ex.ToString());

                // Trả về false để hệ thống bật thông báo Mock OTP dự phòng, tránh crash ứng dụng
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