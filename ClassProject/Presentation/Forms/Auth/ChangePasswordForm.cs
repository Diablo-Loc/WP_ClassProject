using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClassProject.Business.Services;
using ClassProject.DataAccess.Entities;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class ChangePasswordForm : Form
    {
        // ---- WINDOWS API ĐỂ HỖ TRỢ KÉO THẢ FORM KHÔNG VIỀN MƯỢT MÀ ----
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public ChangePasswordForm()
        {
            InitializeComponent();

            // Đăng ký sự kiện kéo form qua panel nền pnlMain
            if (pnlMain != null)
            {
                pnlMain.MouseDown += PnlMain_MouseDown;
            }
        }

        private void PnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string oldPass = txtOldPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirmPass = txtConfirmPassword.Text.Trim();

            // Các bước validate (để trống, trùng nhau, độ mạnh mật khẩu...) giữ nguyên như cũ
            if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tất cả các trường thông tin!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và Xác nhận mật khẩu không trùng khớp!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (oldPass == newPass)
            {
                MessageBox.Show("Mật khẩu mới không được trùng với mật khẩu hiện tại!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newPass.Length < 6 || !Regex.IsMatch(newPass, @"[a-zA-Z]") || !Regex.IsMatch(newPass, @"[0-9]"))
            {
                MessageBox.Show("Mật khẩu mới phải từ 6 ký tự trở lên, bao gồm cả chữ cái và chữ số để đảm bảo an toàn!", "Mật khẩu yếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string currentUsername = UserSession.Username;

                // GỌI ĐẾN SERVICE XỬ LÝ DATABASE ĐÃ TÍCH HỢP BCRYPT Ở BƯỚC 1
                AccountService accountService = new ClassProject.Business.Services.AccountService();
                bool isSuccess = accountService.ChangePassword(currentUsername, oldPass, newPass);

                if (isSuccess)
                {
                    MessageBox.Show("Đổi mật khẩu thành công! Hệ thống sẽ tự động đăng xuất để áp dụng mật khẩu mới.",
                                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại!",
                                    "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra trong quá trình xử lý: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}