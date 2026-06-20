using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using ClassProject.Business.Services;
using MimeKit;
using MailKit.Net.Smtp;
using BCrypt.Net;
using ClassProject.DataAccess.Db;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClassProject.Presentation.Forms.Auth;
using System.Drawing;

namespace ClassProject.Presentation.Forms
{
    public partial class ForgetPassForm : Form
    {
        private readonly My_DB db = new My_DB();
        private readonly AiChatService _aiChatService = new AiChatService();
        private DateTime _lastSendOTP = DateTime.MinValue;
        private string _verifiedEmail = "";

        public ForgetPassForm()
        {
            InitializeComponent();
            SetOTPSectionVisible(false);
            pnlChatbot.Visible = false;
            AppendBotMessage("Xin chào! Tôi là Trợ lý AI Portal. Bạn cần tôi trợ giúp vấn đề gì dưới đây?");
            AppendQuickReplyMenu();
        }

        // Luồng gửi OTP: Đóng vai trò kiểm tra bảo mật đầu vào và kích hoạt luồng OTP chuyên biệt
        private async void btnSendOTP_Click(object sender, EventArgs e)
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
            if (!ValidateAccountSecurityStatus(email)) return;

            // 4. Tiến hành kích hoạt thử thách OTP độc lập
            _lastSendOTP = DateTime.Now;
            if (await OvercomeForgetPasswordOtp(email))
            {
                // Nếu vượt qua OTP thành công -> Cho phép hiện phần nhập mật khẩu mới ngay trên Form
                _verifiedEmail = email;
                SetOTPSectionVisible(true);

                // Vô hiệu hóa ô nhập Email cũ để tránh User thay đổi Email sau khi đã verify OTP
                txtEmail.Enabled = false;
                btnSendOTP.Enabled = false;
            }
        }

        // Luồng xử lý Lưu mật khẩu mới xuống Cơ sở dữ liệu
        private void btnReset_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirm.Text.Trim();

            // 1. Validate mật khẩu an toàn
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(newPassword, passwordPattern))
            {
                MessageBox.Show("Mật khẩu mới không đủ độ an toàn!\nYêu cầu: Tối thiểu 8 ký tự, chứa ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt.",
                    "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không trùng khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra chống trùng mật khẩu cũ công nghệ BCrypt
            string currentHash = GetCurrentPasswordHash(_verifiedEmail);
            if (!string.IsNullOrEmpty(currentHash) && BCrypt.Net.BCrypt.Verify(newPassword, currentHash))
            {
                MessageBox.Show("Mật khẩu mới không được phép trùng với mật khẩu đang sử dụng hiện tại!",
                    "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Cập nhật dữ liệu
            if (ExecutePasswordUpdate(_verifiedEmail, newPassword))
            {
                MessageBox.Show("Đặt lại mật khẩu bảo mật thành công! Hệ thống chuyển hướng về màn hình Đăng nhập.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        #region Tầng Nghiệp Vụ OTP độc lập (Mô hình doanh nghiệp)

        private async Task<bool> OvercomeForgetPasswordOtp(string email)
        {
            // Sinh mã bảo mật TOTP nâng cao từ Email + Secret Key
            string systemSecret = "HCMUTE_AI_SECRET_KEY_2026";
            string generatedOtp = GenerateTOTP(email + systemSecret);
            DateTime otpExpireTime = DateTime.Now.AddMinutes(5);

            // Gửi email bất đồng bộ tránh đơ UI
            Cursor.Current = Cursors.WaitCursor;
            bool isMailSent = await SendOTPEmailAsync(email, generatedOtp);
            Cursor.Current = Cursors.Default;

            string maskedEmailHint = MaskEmail(email);
            if (isMailSent)
            {
                MessageBox.Show($"Mã xác thực 2FA đã được gửi thành công đến hệ thống email: {maskedEmailHint}.\nVui lòng kiểm tra hộp thư!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"[CHẾ ĐỘ DỰ PHÒNG - MOCK]\nMã OTP 2FA bảo mật của bạn là: {generatedOtp}",
                    "Hệ thống xác thực dự phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Gọi Form OTP dùng chung độc lập chống Brute-Force mò mã
            using (OtpVerificationForm otpForm = new OtpVerificationForm(generatedOtp, otpExpireTime))
            {
                // Nếu người dùng nhập chuẩn và bấm xác nhận bên Form OTP
                if (otpForm.ShowDialog() == DialogResult.OK)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Tầng Xác Thực Bảo Mật & Kết Nối DB (Data & Security Helpers)

        private bool ValidateAccountSecurityStatus(string email)
        {
            int validValue = 0;
            int statusValue = 0;
            DateTime? lockoutEnd = null;

            bool isEmailExist = GetEmailSecurityStatus(email, out validValue, out statusValue, out lockoutEnd);

            if (!isEmailExist)
            {
                // Chuẩn doanh nghiệp: Email Enumeration Protection
                MessageBox.Show("Nếu Email tồn tại trong hệ thống và hợp lệ, bạn sẽ nhận được mã OTP trong ít phút.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (validValue == 0)
            {
                MessageBox.Show("Tài khoản liên kết với Email này chưa được phê duyệt hoặc kích hoạt bởi Ban quản trị.\nKhông thể thực hiện khôi phục mật khẩu!",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            if (statusValue == 2)
            {
                MessageBox.Show("Tài khoản này hiện đang bị khóa vĩnh viễn.\nVui lòng liên hệ Ban quản trị để được xử lý!",
                    "Tài khoản bị khóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.Now)
            {
                MessageBox.Show($"Tài khoản đang bị tạm khóa tự động do nhập sai nhiều lần.\nVui lòng thử lại sau: {lockoutEnd.Value:HH:mm:ss}",
                    "Tài khoản bị tạm khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ExecutePasswordUpdate(string email, string newPassword)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE dbo.Users 
                                     SET Password = @password, 
                                         FailedAttempts = 0, 
                                         LockoutEnd = NULL, 
                                         Updated_At = GETDATE() 
                                     WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(newPassword));
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - DB Update Pass]: {ex.Message}");
                MessageBox.Show("Lỗi hệ thống khi cập nhật dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Thuật Toán & Hạ Tầng Gửi Mail (Core Algorithms & Infra)

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            string[] parts = email.Split('@');
            string namePart = parts[0];
            string domainPart = parts[1];

            if (namePart.Length <= 2) return namePart + "****@" + domainPart;
            return namePart.Substring(0, 2) + new string('*', namePart.Length - 2) + "@" + domainPart;
        }

        private string GenerateTOTP(string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            long timeIndex = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / 300);
            byte[] challenge = BitConverter.GetBytes(timeIndex);

            if (BitConverter.IsLittleEndian) Array.Reverse(challenge);

            using (var hmac = new HMACSHA1(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(challenge);
                int offset = hash[hash.Length - 1] & 0xf;
                int binary = ((hash[offset] & 0x7f) << 24) |
                             ((hash[offset + 1] & 0xff) << 16) |
                             ((hash[offset + 2] & 0xff) << 8) |
                             (hash[offset + 3] & 0xff);

                return (binary % 1000000).ToString("D6");
            }
        }

        private async Task<bool> SendOTPEmailAsync(string toEmail, string otp)
        {
            try
            {
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string senderPassword = ConfigurationManager.AppSettings["AppPassword"];

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword)) return false;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("HCMUTE Portal Security", senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "🛡️ [HCMUTE SECURITY] - MÃ XÁC THỰC PHỤC HỒI MẬT KHẨU";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%); padding: 25px; text-align: center; color: white;'>
                        <h2 style='margin: 0; font-size: 22px; letter-spacing: 1px;'>HỆ THỐNG XÁC THỰC TRUNG TÂM</h2>
                    </div>
                    <div style='padding: 30px; background-color: #ffffff; color: #333333; line-height: 1.6;'>
                        <p>Xin chào Thành viên,</p>
                        <p>Chúng tôi nhận được yêu cầu cấp lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã bảo mật 2FA dưới đây để xác nhận:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <span style='display: inline-block; background-color: #f4f6f9; color: #1e3c72; font-size: 32px; font-weight: bold; padding: 12px 35px; border-radius: 6px; letter-spacing: 5px; border: 1px dashed #2a5298;'>{otp}</span>
                        </div>
                        <p style='color: #e74c3c; font-weight: bold; font-size: 13px;'>⚠️ Lưu ý bảo mật:</p>
                        <ul style='margin: 0; padding-left: 20px; font-size: 13px; color: #666666;'>
                            <li>Mã xác thực này có hiệu lực tối đa trong vòng 5 phút.</li>
                        </ul>
                    </div>
                </div>";

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi gửi mail thực tế: " + ex.Message);
                return false;
            }
        }

        private bool GetEmailSecurityStatus(string email, out int valid, out int status, out DateTime? lockoutEnd)
        {
            valid = 0; status = 0; lockoutEnd = null;
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
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
                System.Diagnostics.Debug.WriteLine($"[Error - GetCurrentPasswordHash]: {ex.Message}");
                return "";
            }
        }

        #endregion

        #region Giao Diện & Điều Khiển Ẩn/Hiện (UI Controls)

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetOTPSectionVisible(bool visible)
        {
            lblNewPassword.Visible = visible;
            txtNewPassword.Visible = visible;
            lblConfirmPassword.Visible = visible;
            txtConfirm.Visible = visible;
            btnReset.Visible = visible;
        }

        #endregion

        #region Luồng Xử Lý Giao Diện Chatbot AI (UI Logic Only)

        private void btnToggleChat_Click(object sender, EventArgs e)
        {
            pnlChatbot.Visible = !pnlChatbot.Visible;
            if (pnlChatbot.Visible) txtChatInput.Focus();
        }

        private async void btnSendChat_Click(object sender, EventArgs e)
        {
            await HandleUserChatAsync();
        }

        private async void txtChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await HandleUserChatAsync();
            }
        }

        private async Task HandleUserChatAsync()
        {
            string userMessage = txtChatInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AppendUserMessage(userMessage);
            txtChatInput.Clear();

            AppendBotMessage("AI đang suy nghĩ...");
            btnSendChat.Enabled = false;

            string aiResponse = await _aiChatService.FetchAiResponseAsync(userMessage);

            RemoveLastLoadingMessage();
            AppendBotMessage(aiResponse);
            btnSendChat.Enabled = true;
        }

        #endregion

        #region Các Hàm Định Dạng UI RichTextBox & Gợi ý nhanh (UI Helpers) Tối Ưu

        private void AppendUserMessage(string message)
        {
            rtbChatLog.SelectionStart = rtbChatLog.TextLength;
            rtbChatLog.SelectionLength = 0;
            rtbChatLog.SelectionColor = Color.Blue;
            rtbChatLog.SelectionFont = new Font(rtbChatLog.Font, FontStyle.Bold);
            rtbChatLog.AppendText("[Bạn]: ");

            rtbChatLog.SelectionColor = Color.Black;
            rtbChatLog.SelectionFont = new Font(rtbChatLog.Font, FontStyle.Regular);
            rtbChatLog.AppendText($"{message}\n\n");
            ScrollToBottom();
        }

        private void AppendBotMessage(string message)
        {
            rtbChatLog.SelectionStart = rtbChatLog.TextLength;
            rtbChatLog.SelectionLength = 0;
            rtbChatLog.SelectionColor = Color.DarkSlateGray;
            rtbChatLog.SelectionFont = new Font(rtbChatLog.Font, FontStyle.Bold);
            rtbChatLog.AppendText("[AI Assistant]: ");

            rtbChatLog.SelectionColor = Color.FromArgb(30, 41, 59);
            rtbChatLog.SelectionFont = new Font(rtbChatLog.Font, FontStyle.Regular);
            rtbChatLog.AppendText($"{message}\n\n");
            ScrollToBottom();
        }

        // Tạo danh sách các câu hỏi nhanh có thể click được bằng Button thật
        private void AppendQuickReplyMenu()
        {
            // Tìm và xóa Panel cũ nếu có
            Control[] oldPanels = pnlChatbot.Controls.Find("pnlQuickMenu", false);
            foreach (var old in oldPanels) pnlChatbot.Controls.Remove(old);

            string[] quickQuestions = new string[]
            {
        "1. Xem quy trình lấy lại mật khẩu",
        "2. Lỗi không nhận được OTP",
        "3. Tiêu chuẩn mật khẩu an toàn"
            };

            Panel pnlMenu = new Panel();
            pnlMenu.Name = "pnlQuickMenu";
            pnlMenu.Width = pnlChatbot.Width - 10;
            pnlMenu.Height = quickQuestions.Length * 32 + 5;

            // Định vị ngay trên ô nhập liệu txtChatInput
            pnlMenu.Location = new Point(5, txtChatInput.Top - pnlMenu.Height - 5);
            pnlMenu.BackColor = Color.FromArgb(248, 250, 252);
            pnlMenu.BorderStyle = BorderStyle.FixedSingle;

            int topOffset = 2;
            foreach (string question in quickQuestions)
            {
                Button btnQuick = new Button();
                btnQuick.Text = "  👉 " + question;
                btnQuick.TextAlign = ContentAlignment.MiddleLeft;
                btnQuick.Size = new Size(pnlMenu.Width - 6, 28);
                btnQuick.Location = new Point(2, topOffset);
                btnQuick.Font = new Font(rtbChatLog.Font.FontFamily, 9f, FontStyle.Regular);
                btnQuick.ForeColor = Color.FromArgb(2, 132, 199);
                btnQuick.BackColor = Color.White;
                btnQuick.Cursor = Cursors.Hand;
                btnQuick.FlatStyle = FlatStyle.Flat;
                btnQuick.FlatAppearance.BorderSize = 1;
                btnQuick.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
                btnQuick.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);

                btnQuick.Tag = question;
                btnQuick.Click += QuickQuestionButton_Click;

                pnlMenu.Controls.Add(btnQuick);
                topOffset += 30;
            }

            pnlChatbot.Controls.Add(pnlMenu);
            pnlMenu.BringToFront();

            // Thu hẹp rtbChatLog lại để nhường chỗ cho Menu không bị che khuất chữ dữ liệu cũ
            rtbChatLog.Height = pnlMenu.Top - rtbChatLog.Top - 5;
            ScrollToBottom();
        }

        private async void QuickQuestionButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            string rawQuestion = clickedButton.Tag.ToString();
            // Làm sạch chuỗi bỏ số thứ tự: "1. Xem quy trình..." -> "Xem quy trình..."
            string cleanQuestion = Regex.Replace(rawQuestion, @"^\d+\.\s*", "");

            // 1. Dọn dẹp Menu ngay lập tức để lấy lại không gian
            Control[] oldPanels = pnlChatbot.Controls.Find("pnlQuickMenu", false);
            foreach (var old in oldPanels) pnlChatbot.Controls.Remove(old);

            // 2. Trả lại chiều cao gốc cho rtbChatLog
            rtbChatLog.Height = txtChatInput.Top - rtbChatLog.Top - 5;

            // 3. Tiến hành gửi tin nhắn với bọc lót an toàn tránh đơ giao diện
            AppendUserMessage(cleanQuestion);
            AppendBotMessage("AI đang suy nghĩ...");

            // Khóa các tính năng nhập liệu/gửi để chống spam bấm liên tục
            btnSendChat.Enabled = false;
            txtChatInput.Enabled = false;

            try
            {
                // Chạy qua hàm băm kết hợp phân luồng điều hướng tĩnh/Cloud AI tổng hợp của bạn
                string aiResponse = await _aiChatService.FetchAiResponseAsync(cleanQuestion);

                RemoveLastLoadingMessage(); // Xóa chữ "AI đang suy nghĩ..." an toàn
                AppendBotMessage(aiResponse);
            }
            catch (Exception ex)
            {
                RemoveLastLoadingMessage();
                AppendBotMessage($"⚠️ Lỗi xử lý giao diện: {ex.Message}");
            }
            finally
            {
                // Luôn luôn mở lại điều khiển UI dù API thành công hay thất bại
                btnSendChat.Enabled = true;
                txtChatInput.Enabled = true;

                // 4. Tái hiện lại menu gợi ý cho lượt hỏi kế tiếp
                AppendQuickReplyMenu();
            }
        }

        // 🛡️ SỬA LỖI XÓA NHẦM: Hàm xóa chữ "AI đang suy nghĩ..." tuyệt đối chính xác dựa trên độ dài chuỗi
        private void RemoveLastLoadingMessage()
        {
            string targetPlaceholder = "[AI Assistant]: AI đang suy nghĩ...\n\n";
            if (rtbChatLog.Text.EndsWith(targetPlaceholder))
            {
                int startIndex = rtbChatLog.TextLength - targetPlaceholder.Length;
                rtbChatLog.Select(startIndex, targetPlaceholder.Length);
                rtbChatLog.SelectedText = "";
            }
            else
            {
                // Dự phòng nếu text bị thay đổi, xóa từ vị trí xuất hiện cuối cùng của dòng đợi
                int lastIndex = rtbChatLog.Text.LastIndexOf("[AI Assistant]: AI đang suy nghĩ...");
                if (lastIndex >= 0)
                {
                    rtbChatLog.Select(lastIndex, rtbChatLog.TextLength - lastIndex);
                    rtbChatLog.SelectedText = "";
                }
            }
        }

        private void ScrollToBottom()
        {
            rtbChatLog.SelectionStart = rtbChatLog.TextLength;
            rtbChatLog.ScrollToCaret();
        }

        #endregion
    }
}