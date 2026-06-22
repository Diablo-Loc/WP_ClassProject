using System;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Auth
{
    public partial class OtpVerificationForm : Form
    {
        private readonly string _correctOtp;
        private readonly DateTime _expireTime;
        private int _timeLeftSeconds;

        // Chống Brute-force mò mã OTP
        private int _failedAttempts = 0;
        private const int MaxFailedAttempts = 3;

        public OtpVerificationForm(string correctOtp, DateTime expireTime)
        {
            InitializeComponent();
            _correctOtp = correctOtp;
            _expireTime = expireTime;

            _timeLeftSeconds = (int)(_expireTime - DateTime.Now).TotalSeconds;

            if (_timeLeftSeconds > 0)
            {
                UpdateCountdownText();
                timerOtp.Start();
            }

            // Đảm bảo Form vừa mở là có thể gõ ngay không cần click chuột
            this.Load += (s, e) => txtOtpInput.Focus();
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            string inputOtp = txtOtpInput.Text.Trim();

            // 1. Kiểm tra hết hạn trước
            if (DateTime.Now > _expireTime)
            {
                MessageBox.Show("Mã OTP đã hết hiệu lực! Vui lòng yêu cầu gửi lại mã mới.", "Quá thời gian", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            // 2. Kiểm tra đúng/sai OTP
            if (inputOtp == _correctOtp)
            {
                MessageBox.Show("Xác thực mã OTP thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                _failedAttempts++;
                int remainingAttempts = MaxFailedAttempts - _failedAttempts;

                if (remainingAttempts <= 0)
                {
                    MessageBox.Show($"Bạn đã nhập sai mã OTP quá {MaxFailedAttempts} lần! Tiến trình bị hủy bỏ bảo mật.",
                                    "Từ chối xác thực", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Mã OTP không chính xác! Bạn còn {remainingAttempts} lần thử lại.",
                                    "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    txtOtpInput.Text = string.Empty;
                    txtOtpInput.Focus();
                }
            }
        }

        private void timerOtp_Tick(object sender, EventArgs e)
        {
            _timeLeftSeconds--;
            if (_timeLeftSeconds <= 0)
            {
                timerOtp.Stop();
                lblCountdown.Text = "Mã OTP đã hết hạn!";
                txtOtpInput.Enabled = false;
                btnVerify.Enabled = false;
            }
            else
            {
                UpdateCountdownText();
            }
        }

        // Tách hàm cập nhật Text đếm ngược để tránh lặp code (DRY Principle)
        private void UpdateCountdownText()
        {
            int minutes = _timeLeftSeconds / 60;
            int seconds = _timeLeftSeconds % 60;
            // Định dạng :D2 bảo đảm luôn có 2 chữ số (ví dụ: 05:02)
            lblCountdown.Text = $"Mã OTP hết hạn sau: {minutes:D2}:{seconds:D2}";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}