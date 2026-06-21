using BCrypt.Net;
using ClassProject.DataAccess.Db;
using ClassProject.Presentation.Forms.Auth;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace ClassProject.Presentation.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly My_DB db = new My_DB();

        public RegisterForm()
        {
            InitializeComponent();
        }
        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Thiết lập đổ bóng mờ xung quanh khung Card trắng giống Form Login
            guna2ShadowForm1.SetShadowForm(this);
        }

        // Luồng xử lý chính: Đóng vai trò Controller điều hướng nghiệp vụ sạch sẽ (SOLID)
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đầu vào giao diện (Validation)
            if (!ValidateFormInput()) return;

            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            int selectedRoleId = (cboPosition.SelectedItem?.ToString() == "Student") ? 1 : 2;

            // 2. Kiểm tra trùng lặp tài khoản sớm (Tránh spam gửi Mail vô ích)
            if (IsAccountDuplicated(username, email)) return;

            // 3. Kiểm tra Email rác qua API bất đồng bộ
            if (await IsDisposableEmailActive(email)) return;

            // 4. THỬ THÁCH XÁC THỰC OTP (Gọi form trung gian độc lập)
            if (!await OvercomeOtpChallenge(email)) return;

            // 5. GHI DỮ LIỆU XUỐNG DATABASE KHI ĐÃ VƯỢT QUA CÁC BƯỚC XÁC THỰC
            ExecuteDatabaseRegistration(username, email, password, selectedRoleId);
        }

        #region Tầng Hàm Bổ Trợ & Xác Thực Giao Diện (Helpers & Validation)

        private bool ValidateFormInput()
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtConfirm.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboPosition.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ (Student/HR)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string email = txtEmail.Text.Trim();
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(txtPassword.Text, passwordPattern))
            {
                MessageBox.Show("Mật khẩu không đủ độ an toàn!\n" +
                                "Yêu cầu: Tối thiểu 8 ký tự, chứa ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt.",
                                "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool IsAccountDuplicated(string username, string email)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    const string checkUserQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username OR Email = @email";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        checkCmd.Parameters.AddWithValue("@email", email);

                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Username hoặc Email đã được sử dụng trên hệ thống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối kiểm tra trùng lặp tài khoản: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }

        private async Task<bool> IsDisposableEmailActive(string email)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                bool isDisposable = await IsDisposableEmail(email);
                Cursor.Current = Cursors.Default;

                if (isDisposable)
                {
                    MessageBox.Show("Hệ thống phát hiện đây là Email tạm thời (Disposable Email) dùng để spam!\n" +
                                    "Vui lòng sử dụng các dịch vụ Email chính thức để đăng ký.",
                                    "Từ chối đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return true;
                }
            }
            catch
            {
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        #endregion

        #region Luồng Nghiệp Vụ OTP (OTP Business Logic Flow)

        // Kích hoạt thử thách OTP độc lập. Trả về true nếu User vượt qua thành công.
        private async Task<bool> OvercomeOtpChallenge(string email)
        {
            // Sinh mã ngẫu nhiên bảo mật cao (Cryptographic Random)
            string generatedOtp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            DateTime otpExpireTime = DateTime.Now.AddMinutes(5);

            // Gửi Email chứa OTP dạng Không đồng bộ (Async)
            Cursor.Current = Cursors.WaitCursor;
            bool isMailSent = await SendOtpEmailAsync(email, generatedOtp);
            Cursor.Current = Cursors.Default;

            if (!isMailSent)
            {
                MessageBox.Show("Không thể gửi mã xác thực tới Email của bạn. Vui lòng kiểm tra kết nối mạng!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Gọi Hộp thoại Form OTP được đóng gói chống brute-force
            using (OtpVerificationForm otpForm = new OtpVerificationForm(generatedOtp, otpExpireTime))
            {
                if (otpForm.ShowDialog() == DialogResult.OK)
                {
                    return true; // Xác thực thành công hoàn toàn
                }
            }

            // Người dùng chủ động hủy hoặc bị khóa form do nhập sai quá số lần
            MessageBox.Show("Tiến trình đăng ký bị hủy do chưa hoàn thành xác thực mã OTP.", "Xác thực thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        #endregion

        #region Tầng Kết Nối Dữ Liệu & API (Data Access & Infrastructure)

        private void ExecuteDatabaseRegistration(string username, string email, string password, int selectedRoleId)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            int validStatus = 0;  // Mặc định: 0 = Chờ duyệt đối với tài khoản mới
                            int statusValue = 1;  // Mặc định: 1 = Active
                            int? targetStudentId = null;

                            // Nếu đăng ký với vai trò Sinh viên -> Tiến hành đối chiếu hồ sơ gốc
                            if (selectedRoleId == 1)
                            {
                                string checkStudentQuery = "SELECT Id, UserId FROM dbo.Students WHERE Email = @email";
                                using (SqlCommand cmd = new SqlCommand(checkStudentQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@email", email);
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (!reader.Read())
                                        {
                                            MessageBox.Show("Email này chưa tồn tại trong danh sách sinh viên ban đầu của trường!", "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            reader.Close();
                                            tx.Rollback();
                                            return;
                                        }

                                        if (reader["UserId"] != DBNull.Value)
                                        {
                                            MessageBox.Show("Hồ sơ sinh viên này đã được liên kết với một tài khoản khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            reader.Close();
                                            tx.Rollback();
                                            return;
                                        }
                                        targetStudentId = Convert.ToInt32(reader["Id"]);
                                    }
                                }
                            }

                            // Thực hiện lưu tài khoản mới vào bảng Users
                            string insertUserQuery = "INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status) " +
                                                     "OUTPUT INSERTED.Id " +
                                                     "VALUES (@username, @email, @password, @roleId, @valid, @status)";

                            int newUserId = 0;
                            using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, conn, tx))
                            {
                                insertCmd.Parameters.AddWithValue("@username", username);
                                insertCmd.Parameters.AddWithValue("@email", email);
                                insertCmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                                insertCmd.Parameters.AddWithValue("@roleId", selectedRoleId);
                                insertCmd.Parameters.AddWithValue("@valid", validStatus);
                                insertCmd.Parameters.AddWithValue("@status", statusValue);

                                newUserId = (int)insertCmd.ExecuteScalar();
                            }

                            // Cập nhật ngược lại cột kết nối UserId bên bảng Sinh Viên
                            if (selectedRoleId == 1 && targetStudentId.HasValue)
                            {
                                string updateStudentQuery = "UPDATE dbo.Students SET UserId = @userId WHERE Id = @studentId";
                                using (SqlCommand updateCmd = new SqlCommand(updateStudentQuery, conn, tx))
                                {
                                    updateCmd.Parameters.AddWithValue("@userId", newUserId);
                                    updateCmd.Parameters.AddWithValue("@studentId", targetStudentId.Value);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            tx.Commit();

                            MessageBox.Show("Đăng ký tài khoản thành công!\nVui lòng chờ Ban giám hiệu hoặc Ban quản trị duyệt trước khi đăng nhập.",
                                            "Đăng ký hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback();
                            System.Diagnostics.Debug.WriteLine($"[Error - Register Transaction Inner]: {ex.Message}");
                            MessageBox.Show("Có lỗi dữ liệu phát sinh trong quá trình lưu trữ: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Error - DB Connection Fault]: {ex.Message}");
                MessageBox.Show("Không thể thiết lập kết nối đến Cơ sở dữ liệu để tạo tài khoản: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> SendOtpEmailAsync(string targetEmail, string otpCode)
        {
            try
            {
                // Lấy thông tin cấu hình từ App.config để đảm bảo đồng bộ toàn hệ thống
                string smtpEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string smtpPassword = ConfigurationManager.AppSettings["AppPassword"];

                if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword) || smtpEmail.Contains("your-email"))
                {
                    System.Diagnostics.Debug.WriteLine("[SMTP Error]: Chưa cấu hình Email gửi trong App.config!");
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Hệ Thống Quản Lý Trường Học", smtpEmail));
                message.To.Add(new MailboxAddress("", targetEmail));
                message.Subject = "Mã Xác Thực Đăng Ký Tài Khoản Mới";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <div style='font-family: Segoe UI, Arial, sans-serif; max-width: 500px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #0f172a; color: white; padding: 20px; text-align: center;'>
                        <h2 style='margin: 0; font-size: 20px; letter-spacing: 1px;'>XÁC THỰC AN TOÀN</h2>
                    </div>
                    <div style='padding: 24px; background-color: white; color: #334155;'>
                        <p>Xin chào,</p>
                        <p>Bạn đang thực hiện đăng ký tài khoản trên hệ thống quản lý. Dưới đây là mã OTP xác thực của bạn:</p>
                        <div style='background-color: #f1f5f9; border-radius: 6px; padding: 15px; text-align: center; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #0f172a;'>{otpCode}</span>
                        </div>
                        <p style='color: #e11d48; font-size: 13px; font-style: italic;'>* Mã xác thực này có hiệu lực trong vòng 5 phút và chỉ sử dụng được 1 lần duy nhất.</p>
                    </div>
                </div>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // Tăng timeout lên 8 giây phòng trường hợp mạng phản hồi chậm
                    client.Timeout = 8000;
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpEmail, smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết ra cửa sổ Output để bạn dễ debug xem thực tế lỗi gì (Sai mật khẩu, hay nghẽn cổng)
                System.Diagnostics.Debug.WriteLine($"[SMTP Mail Error Detail]: {ex.ToString()}");
                return false;
            }
        }

        private async Task<bool> IsDisposableEmail(string email)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(4);
                    string url = $"https://open.kickbox.com/v1/disposable/{Uri.EscapeDataString(email)}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(jsonResult))
                        {
                            if (doc.RootElement.TryGetProperty("disposable", out JsonElement disposableProp))
                            {
                                return disposableProp.GetBoolean();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API Error - Disposable Check Fail]: {ex.Message}");
            }
            return false;
        }

        #endregion

        #region Các Tác Vụ Giao Diện Khác (UI Form Control Actions)

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpenScanner_Click(object sender, EventArgs e)
        {
            using (CardScannerForm scannerForm = new CardScannerForm(ScannerMode.OnlyMSSV))
            {
                if (scannerForm.ShowDialog() == DialogResult.OK)
                {
                    string mssvResult = scannerForm.DetectedMSSV;
                    if (!string.IsNullOrEmpty(mssvResult))
                    {
                        txtUsername.Text = mssvResult;

                        // Tự động chuyển Combobox chức vụ sang "Student" cho tiện lợi
                        if (cboPosition.Items.Contains("Student"))
                        {
                            cboPosition.SelectedItem = "Student";
                        }
                    }
                }
            }
        }

        #endregion
    }
}