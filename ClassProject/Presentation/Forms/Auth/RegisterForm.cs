using BCrypt.Net;
using ClassProject.DataAccess.Db;
using ClassProject.Presentation.Forms.Auth;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly My_DB db = new My_DB();

        public RegisterForm()
        {
            InitializeComponent();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirm.Text.Trim();

            // 1. Kiểm tra đầu vào cơ bản (Validation)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // CHECKLIST: Chống lỗi NullReference bằng toán tử ?. và kiểm tra null an toàn
            if (cboPosition.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ (Student/HR)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedText = cboPosition.SelectedItem?.ToString() ?? "";
            int selectedRoleId = (selectedText == "Student") ? 1 : 2;

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // CHỐT CHẶN KIỂM TRA ĐỘ MẠNH MẬT KHẨU BẰNG REGEX
            // ^(?=.*[A-Z]) : Ít nhất 1 chữ cái viết hoa
            // (?=.*\d)     : Ít nhất 1 chữ số
            // (?=.*[\W_])  : Ít nhất 1 ký tự đặc biệt (như @, #, $, !, %,...)
            // .{8,}        : Tổng chiều dài tối thiểu từ 8 ký tự trở lên
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(password, passwordPattern))
            {
                MessageBox.Show("Mật khẩu không đủ độ an toàn!\n" +
                                "Yêu cầu: Tối thiểu 8 ký tự, chứa ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt.",
                                "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chuyển con trỏ chuột thành hình vòng xoay tải để người dùng biết hệ thống đang xử lý
                Cursor.Current = Cursors.WaitCursor;

                // Gọi API bất đồng bộ (Có từ khóa await)
                bool isDisposable = await IsDisposableEmail(email);

                // Trả con trỏ chuột về bình thường
                Cursor.Current = Cursors.Default;

                if (isDisposable)
                {
                    MessageBox.Show("Hệ thống phát hiện đây là Email tạm thời (Disposable Email) dùng để spam!\n" +
                                    "Vui lòng sử dụng các dịch vụ Email chính thức (Gmail, Outlook, Mail Trường,...) để đăng ký tài khoản.",
                                    "Từ chối đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return; // Chặn đứng luồng không cho chạy xuống lưu DB
                }
            }
            catch
            {
                Cursor.Current = Cursors.Default;
            }
            // 2. Xử lý nghiệp vụ tương tác Cơ sở dữ liệu thông qua Transaction
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // CHẶNG A: Kiểm tra Username/Email đã tồn tại (CHECKLIST: Sử dụng SqlParameter chống SQL Injection)
                            const string checkUserQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username OR Email = @email";
                            using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn, tx))
                            {
                                checkCmd.Parameters.AddWithValue("@username", username);
                                checkCmd.Parameters.AddWithValue("@email", email);

                                if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                                {
                                    MessageBox.Show("Username hoặc Email đã được sử dụng trên hệ thống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    tx.Rollback(); // Phải Rollback rõ ràng trước khi đóng luồng hàm
                                    return;
                                }
                            }

                            // Trạng thái tài khoản bình thường là Status = 1. Tài khoản bị khóa mới là Status = 2.
                            int validStatus = 0;  // Mặc định: 0 = Chờ duyệt đối với HR
                            int statusValue = 1;  // Mặc định: 1 = Trạng thái hoạt động bình thường (Active)
                            int? targetStudentId = null;

                            // CHẶNG B: Nếu chọn chức vụ là Sinh viên -> Xác thực chéo với danh sách hồ sơ gốc
                            if (selectedRoleId == 1)
                            {
                                string checkStudentQuery =
                                    "SELECT Id, UserId FROM dbo.Students WHERE Email = @email";

                                using (SqlCommand cmd = new SqlCommand(checkStudentQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@email", email);

                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (!reader.Read())
                                        {
                                            MessageBox.Show(
                                                "Email này chưa tồn tại trong danh sách sinh viên của trường!",
                                                "Đăng ký thất bại",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);

                                            reader.Close();
                                            tx.Rollback();
                                            return;
                                        }

                                        if (reader["UserId"] != DBNull.Value)
                                        {
                                            MessageBox.Show(
                                                "Sinh viên này đã có tài khoản hệ thống!",
                                                "Thông báo",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);

                                            reader.Close();
                                            tx.Rollback();
                                            return;
                                        }

                                        targetStudentId = Convert.ToInt32(reader["Id"]);
                                    }
                                }
                                //chờ admin duyệt.
                                validStatus = 0;
                                statusValue = 1;
                            }

                            // CHẶNG C: Thực hiện tạo mới tài khoản vào bảng Users
                            string insertUserQuery = "INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status) " +
                                                     "OUTPUT INSERTED.Id " +
                                                     "VALUES (@username, @email, @password, @roleId, @valid, @status)";

                            int newUserId = 0;
                            using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, conn, tx))
                            {
                                insertCmd.Parameters.AddWithValue("@username", username);
                                insertCmd.Parameters.AddWithValue("@email", email);
                                // Mã hóa bảo mật mật khẩu bằng thư viện BCrypt theo yêu cầu kiến trúc hệ thống
                                insertCmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                                insertCmd.Parameters.AddWithValue("@roleId", selectedRoleId);
                                insertCmd.Parameters.AddWithValue("@valid", validStatus);
                                insertCmd.Parameters.AddWithValue("@status", statusValue);

                                newUserId = (int)insertCmd.ExecuteScalar();
                            }

                            // CHẶNG D: Liên kết ID tài khoản Users vừa sinh ngược lại vào bảng danh sách Sinh viên
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

                            // Cam kết thực thi và lưu toàn bộ tiến trình vào Database một cách an toàn toàn vẹn dữ liệu
                            tx.Commit();

                            // Hiển thị thông báo phản hồi tương ứng theo nhóm chức vụ
                            if (selectedRoleId == 1)
                            {
                                MessageBox.Show(
                                    "Đăng ký tài khoản Sinh viên thành công!\nVui lòng chờ Admin phê duyệt trước khi đăng nhập.",
                                    "Chờ phê duyệt",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Đăng ký tài khoản HR thành công!\nVui lòng chờ Admin phê duyệt trước khi đăng nhập.",
                                    "Chờ phê duyệt",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }

                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback(); // Hoàn tác toàn bộ thay đổi nếu phát sinh bất kỳ lỗi dữ liệu nào

                            // CHECKLIST: Ghi log hệ thống rõ ràng để phục vụ việc Debug, chống nuốt Exception thô
                            System.Diagnostics.Debug.WriteLine($"[Error - Register Inner Flow]: {ex.Message}");
                            MessageBox.Show("Có lỗi xảy ra trong quá trình xử lý lưu trữ dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // CHECKLIST: Ghi log lỗi kết nối tầng ngoài
                System.Diagnostics.Debug.WriteLine($"[Error - Database Connection]: {ex.Message}");
                MessageBox.Show("Không thể thiết lập kết nối đến Cơ sở dữ liệu: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Hàm gọi API kiểm tra xem Email có phải là email rác/tạm thời hay không
        private async Task<bool> IsDisposableEmail(string email)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Thiết lập Timeout 4 giây để nếu mạng quá yếu thì tự ngắt, không bắt User đợi lâu
                    client.Timeout = TimeSpan.FromSeconds(4);

                    // API endpoint miễn phí từ Kickbox (không cần tạo tài khoản, không cần API Key)
                    string url = $"https://open.kickbox.com/v1/disposable/{Uri.EscapeDataString(email)}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Phân tích kết quả JSON trả về. Định dạng API: {"disposable": true} hoặc {"disposable": false}
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
                // Nếu có lỗi mạng hoặc sập API, ghi log debug và trả về false (cho qua) để không chặn người dùng thật
                System.Diagnostics.Debug.WriteLine($"[API Error - Disposable Check Fail]: {ex.Message}");
            }
            return false;
        }

        private void btnOpenScanner_Click(object sender, EventArgs e)
        {
            // Khởi tạo Form quét riêng biệt dưới dạng một Hộp thoại (Dialog)
            using (CardScannerForm scannerForm = new CardScannerForm())
            {
                // Hiển thị Form quét lên và chờ người dùng xử lý xong
                if (scannerForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu người dùng bấm "Xác nhận và Điền" ở Form kia, lấy kết quả đổ vào txtUsername
                    string mssvResult = scannerForm.DetectedMSSV;

                    if (!string.IsNullOrEmpty(mssvResult))
                    {
                        txtUsername.Text = mssvResult;

                        // Tiện tay chuyển luôn Combobox chức vụ sang "Student" cho họ vì họ vừa quét thẻ SV
                        if (cboPosition.Items.Contains("Student"))
                        {
                            cboPosition.SelectedItem = "Student";
                        }
                    }
                }
            }
        }
    }
}