using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class AccountSecurityInfoDialog : Form
    {
        private TextBox txtPassword;
        private Button btnTogglePassword;
        private Button btnCopyPassword;
        private Button btnGenerateNewPass;
        private Label lblHeader;

        private bool _isPasswordHidden = true;
        private string _rawPasswordGenerated = "";

        private int _targetId;
        private int _roleId;

        // Giữ nguyên Constructor cũ để bạn KHÔNG PHẢI sửa các Form danh mục khác
        public AccountSecurityInfoDialog(int targetId, string code, string username, string email, string fullName, int roleId)
        {
            _targetId = targetId;
            _roleId = roleId;

            // 1. Tự động cấu hình Text hiển thị theo RoleId
            string roleText = _roleId == 1 ? "SINH VIÊN" : (_roleId == 2 ? "GIẢNG VIÊN" : "GIÁO VỤ");
            string codeLabel = _roleId == 1 ? "Mã số sinh viên" : (_roleId == 2 ? "Mã số giảng viên" : "Mã nhân sự giáo vụ");

            // 2. Thiết kế giao diện Form
            this.Text = $"Quản lý & Bảo mật tài khoản {roleText}";
            this.Size = new Size(460, 440);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10);
            this.BackColor = Color.White;

            lblHeader = new Label() { Text = $"🔒 THÔNG TIN BẢO MẬT TÀI KHOẢN {roleText}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(0, 114, 198), Location = new Point(20, 15), Size = new Size(410, 30) };
            Label lblInfo = new Label() { Text = $"• Đối tượng: {fullName}\n\n• {codeLabel}: {code}\n\n• Tài khoản hệ thống: {username}\n\n• Email nhận tin: {email}\n\n• Mật khẩu cấp phát mới (Nếu cần):", Location = new Point(20, 55), Size = new Size(410, 110) };

            txtPassword = new TextBox() { Text = "••••••••", Location = new Point(20, 175), Size = new Size(330, 27), ReadOnly = true, UseSystemPasswordChar = true };

            btnTogglePassword = new Button() { Text = "👁️ Xem", Location = new Point(355, 174), Size = new Size(65, 29), FlatStyle = FlatStyle.Flat, Enabled = false, BackColor = Color.FromArgb(240, 240, 240) };
            btnTogglePassword.FlatAppearance.BorderColor = Color.DarkGray;
            btnTogglePassword.Click += BtnTogglePassword_Click;

            btnGenerateNewPass = new Button() { Text = "🔄 Phát sinh & Cập nhật mật khẩu mới", Location = new Point(20, 215), Size = new Size(330, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.Orange, ForeColor = Color.Black };
            btnGenerateNewPass.Click += BtnGenerateNewPass_Click;

            btnCopyPassword = new Button() { Text = "📋 Copy", Location = new Point(355, 217), Size = new Size(65, 29), FlatStyle = FlatStyle.Flat, Enabled = false, BackColor = Color.FromArgb(240, 240, 240) };
            btnCopyPassword.FlatAppearance.BorderColor = Color.DarkGray;
            btnCopyPassword.Click += BtnCopyPassword_Click;

            Label lblFooter = new Label() { Text = $"* Chú ý an toàn: Mật khẩu cũ được mã hóa một chiều nên hệ thống không thể hiển thị lại tại đây. Chỉ bấm nút 'Phát sinh' nếu bạn thực sự muốn đổi mật khẩu mới cho người dùng này.", Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.Gray, Location = new Point(20, 270), Size = new Size(400, 55) };
            Button btnClose = new Button() { Text = "Đóng lại", Location = new Point(175, 345), Size = new Size(100, 35), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 114, 198), ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblHeader, lblInfo, txtPassword, btnTogglePassword, btnGenerateNewPass, btnCopyPassword, lblFooter, btnClose });
        }

        private void BtnGenerateNewPass_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn RESET mật khẩu mới không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            _rawPasswordGenerated = GenerateRandomPassword(8);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(_rawPasswordGenerated);

            // ✨ CẢI TIẾN THÔNG MINH: Đọc cả UserId lẫn Id hồ sơ, đảm bảo Form ngoài truyền cái nào vào cũng chạy đúng!
            string targetTable = _roleId == 1 ? "dbo.Students" : (_roleId == 2 ? "dbo.Teachers" : "dbo.Staffs");
            string updateQuery = $@"UPDATE u SET u.Password = @Password 
                                   FROM LoginDB.dbo.Users u 
                                   INNER JOIN {targetTable} t ON t.UserId = u.Id 
                                   WHERE t.UserId = @TargetId OR t.Id = @TargetId";

            bool isUpdated = false;
            string errorMsg = "";

            try
            {
                My_DB db = new My_DB();
                using (SqlConnection conn = db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", passwordHash);
                        cmd.Parameters.AddWithValue("@TargetId", _targetId);
                        conn.Open();
                        isUpdated = cmd.ExecuteNonQuery() > 0;
                    }
                }
                if (!isUpdated) errorMsg = "Không tìm thấy tài khoản tương ứng với thông tin ID hệ thống.";
            }
            catch (Exception ex) { errorMsg = ex.Message; }

            if (isUpdated)
            {
                txtPassword.Text = _rawPasswordGenerated;
                btnTogglePassword.Enabled = btnCopyPassword.Enabled = true;
                lblHeader.Text = "🔑 ĐÃ CẤP LẠI MẬT KHẨU THÀNH CÔNG";
                lblHeader.ForeColor = Color.DarkRed;
                btnGenerateNewPass.BackColor = Color.LightGray;
                btnGenerateNewPass.Enabled = false;
                MessageBox.Show("Mật khẩu mới đã được kích hoạt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Lỗi hệ thống: {errorMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            _isPasswordHidden = !_isPasswordHidden;
            txtPassword.UseSystemPasswordChar = _isPasswordHidden;
            btnTogglePassword.Text = _isPasswordHidden ? "👁️ Xem" : "🔒 Ẩn";
            btnTogglePassword.BackColor = _isPasswordHidden ? Color.FromArgb(240, 240, 240) : Color.LightPink;
        }

        private void BtnCopyPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rawPasswordGenerated))
            {
                Clipboard.SetText(_rawPasswordGenerated);
                MessageBox.Show("Đã copy mật khẩu!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#$";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--) { res.Append(validChars[rnd.Next(validChars.Length)]); }
            return res.ToString();
        }
    }
}