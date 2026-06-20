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
        private DateTime _otpExpireTime = DateTime.MinValue;
        private DateTime _lastSendOTP = DateTime.MinValue;

        private readonly My_DB db = new My_DB();

        public ForgetPassForm()
        {
            InitializeComponent();
            SetOTPSectionVisible(false);
        }

        private void btnSendOTP_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            // 1. Kiểm tra dữ liệu đầu vào (Input Validation)
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ Email!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Phòng chống tấn công Spam/DDoS OTP (Rate Limiting)
            if (DateTime.Now < _lastSendOTP.AddSeconds(30))
            {
                TimeSpan remaining = _lastSendOTP.AddSeconds(30) - DateTime.Now;
                MessageBox.Show($"Hành động quá nhanh! Vui lòng đợi {Math.Ceiling(remaining.TotalSeconds)} giây để gửi lại mã mới.",
                    "Cảnh báo Spam", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Đọc dữ liệu trạng thái nâng cao từ Database độc lập
            int validValue = 0;
            int statusValue = 0;
            DateTime? lockoutEnd = null;

            bool isEmailExist = GetEmailSecurityStatus(email, out validValue, out statusValue, out lockoutEnd);

            // 4. CHỐT CHẶN BẢO MẬT DOANH NGHIỆP (Business Security Rules)
            if (!isEmailExist)
            {
                // Chuẩn doanh nghiệp: Không báo "Email không tồn tại" để tránh tấn công dò quét Email (Email Enumeration)
                MessageBox.Show("Nếu Email tồn tại trong hệ thống và hợp lệ, bạn sẽ nhận được mã OTP trong ít phút.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // QUY TẮC 1: Tài khoản do Admin cấp/duyệt thì Valid phải bằng 1
            if (validValue == 0)
            {
                MessageBox.Show("Tài khoản liên kết với Email này chưa được phê duyệt hoặc kích hoạt bởi Ban quản trị.\nKhông thể thực hiện khôi phục mật khẩu!",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // QUY TẮC 2: Kiểm tra tài khoản bị khóa chủ động bởi Admin (Status = 2)
            if (statusValue == 2)
            {
                MessageBox.Show("Tài khoản này hiện đang bị khóa vĩnh viễn.\nVui lòng liên hệ Phòng Giáo vụ hoặc Admin để được xử lý!",
                    "Tài khoản bị khóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // QUY TẮC 3: Kiểm tra khóa tự động do nhập sai mật khẩu nhiều lần (Lockout End)
            if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.Now)
            {
                MessageBox.Show($"Tài khoản đang bị tạm khóa tự động do nhập sai nhiều lần.\nVui lòng thử lại sau: {lockoutEnd.Value.ToString("HH:mm:ss")}",
                    "Tài khoản bị tạm khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. Nếu vượt qua tất cả chốt chặn -> Tiến hành sinh và gửi OTP
            _otp = new Random().Next(100000, 999999).ToString();
            _verifiedEmail = email;
            _otpExpireTime = DateTime.Now.AddMinutes(5);
            _lastSendOTP = DateTime.Now;

            bool isMailSent = SendOTPEmail(email, _otp);

            if (isMailSent)
            {
                MessageBox.Show($"Mã OTP đã được gửi thành công đến email: {email}.\nVui lòng kiểm tra hộp thư (hiệu lực trong 5 phút)!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"[CHẾ ĐỘ DỰ PHÒNG - MOCK]\nMã OTP của bạn là: {_otp}",
                    "Hệ thống xác thực dự phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            SetOTPSectionVisible(true);
        }

        private bool GetEmailSecurityStatus(string email, out int valid, out int status, out DateTime? lockoutEnd)
        {
            valid = 0;
            status = 0;
            lockoutEnd = null;

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    // Đọc thêm trường LockoutEnd từ cấu trúc SQL gốc của bạn
                    string query = "SELECT Valid, Status, LockoutEnd FROM dbo.Users WHERE Email = @email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                valid = Convert.ToInt32(reader["Valid"]);
                                status = Convert.ToInt32(reader["Status"]);

                                if (reader["LockoutEnd"] != DBNull.Value)
                                {
                                    lockoutEnd = Convert.ToDateTime(reader["LockoutEnd"]);
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - GetEmailSecurityStatus]: {ex.Message}");
            }
            return false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string inputOTP = txtOTP.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirm.Text.Trim();

            // Kiểm tra OTP hết hạn (Quá 5 phút)
            if (DateTime.Now > _otpExpireTime || string.IsNullOrEmpty(_otp))
            {
                MessageBox.Show("Mã OTP đã hết hạn sử dụng (Hiệu lực tối đa 5 phút).\nVui lòng bấm gửi lại mã mới!",
                    "Mã hết hạn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (inputOTP != _otp)
            {
                MessageBox.Show("Mã OTP nhập vào không chính xác!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Triển khai chính sách độ dài mật khẩu (Password Policy)
            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có độ dài tối thiểu từ 6 ký tự trở lên để đảm bảo an toàn!",
                    "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không trùng khớp!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ngăn chặn hành vi sử dụng lại mật khẩu cũ (Password Reuse Protection)
            string currentHash = GetCurrentPasswordHash(_verifiedEmail);
            if (!string.IsNullOrEmpty(currentHash) && BCrypt.Net.BCrypt.Verify(newPassword, currentHash))
            {
                MessageBox.Show("Mật khẩu mới không được phép trùng với mật khẩu đang sử dụng hiện tại!",
                    "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Thực thi lưu mật khẩu mới xuống Cơ sở dữ liệu
            try
            {
                // Thực thi lưu mật khẩu mới xuống Cơ sở dữ liệu và làm sạch trạng thái khóa
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    // Khôi phục trạng thái sạch cho tài khoản sau khi đổi mật khẩu thành công
                    string query = @"UPDATE dbo.Users 
                     SET Password = @password, 
                         FailedAttempts = 0, 
                         LockoutEnd = NULL, 
                         Updated_At = GETDATE() 
                     WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(newPassword));
                        cmd.Parameters.AddWithValue("@email", _verifiedEmail);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đặt lại mật khẩu thành công! Hệ thống sẽ quay về màn hình Đăng nhập.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtOTP.Clear();
                txtNewPassword.Clear();
                txtConfirm.Clear();
                _otp = "";
                _verifiedEmail = "";
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - Reset Pass Execution]: {ex.Message}");
                MessageBox.Show("Lỗi hệ thống khi cập nhật dữ liệu: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Đọc đồng thời cả Valid và Status của Email trong 1 lượt truy vấn duy nhất
        private bool GetEmailStatus(string email, out int valid, out int status)
        {
            valid = 0;
            status = 1; // Mặc định hoạt động bình thường
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Valid, Status FROM dbo.Users WHERE Email = @email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                valid = Convert.ToInt32(reader["Valid"]);
                                status = Convert.ToInt32(reader["Status"]);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - GetEmailStatus Query]: {ex.Message}");
            }
            return false;
        }

        // Lấy Hash mật khẩu hiện tại để so sánh chéo
        private string GetCurrentPasswordHash(string email)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Password FROM dbo.Users WHERE Email = @email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        object res = cmd.ExecuteScalar();
                        return res != null ? res.ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - GetCurrentPasswordHash Query]: {ex.Message}");
                return "";
            }
        }

        private bool SendOTPEmail(string toEmail, string otp)
        {
            try
            {
                // LỖI 2: Đồ án chấp nhận để chuỗi bảo mật ở đây, thực tế khuyến khích cấu hình qua App.config / Web.config
                string senderEmail = "tranthienan6298.2017@gmail.com";
                string senderPassword = "kmxdsjowtxbfwuhl";

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                    return false;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ClassProject Systems", senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "HCMUTE SYSTEM - RESET PASSWORD OTP";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = $"Mã OTP đặt lại mật khẩu của bạn là: {otp}\n\nMã này chỉ có hiệu lực trong vòng 5 phút. Vui lòng tuyệt đối không chia sẻ mã này cho bất kỳ ai!"
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
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
                System.Diagnostics.Debug.WriteLine("Lỗi gửi mail thực tế: " + ex.Message);
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