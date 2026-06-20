using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ClassProject.Business.Services
{
    public class EmailAlertService
    {
        // 🔒 Cấu hình thông tin Mail Server giữ nguyên cố định
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;

        /// <summary>
        /// Gửi email cảnh báo bảo mật bất đồng bộ, tự giải phóng tài nguyên sau khi gửi.
        /// </summary>
        public async Task SendSecurityAlertAsync(string recipientEmail, string username, string anomalyReasons)
        {
            // Kiểm tra tính hợp lệ của Email nhận trước khi xử lý tiếp
            if (string.IsNullOrWhiteSpace(recipientEmail) || !recipientEmail.Contains("@"))
                return;

            try
            {
                // Lấy thông tin tài khoản gửi từ App.config thay vì hardcode hằng số
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string appPassword = ConfigurationManager.AppSettings["AppPassword"];

                // Kiểm tra an toàn cấu hình hệ thống
                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
                {
                    System.Diagnostics.Debug.WriteLine("[Error] EmailAlertService: Thiếu cấu hình mật khẩu/email trong App.config!");
                    return;
                }

                using (MailMessage mail = new MailMessage())
                {
                    // Sử dụng biến senderEmail vừa lấy từ file cấu hình
                    mail.From = new MailAddress(senderEmail, "HỆ THỐNG AN NINH ACADEMIC");
                    mail.To.Add(recipientEmail);
                    mail.Subject = "⚠️ [CẢNH BÁO BẢO MẬT] Phát hiện hành vi đăng nhập bất thường!";

                    // Chuyển đổi ký tự xuống dòng từ C# (\n) sang định dạng ngắt dòng HTML (<br/>)
                    string formattedReasons = anomalyReasons.Replace("\n", "<br/>");

                    // Soạn thảo Template HTML chuẩn doanh nghiệp
                    mail.Body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                        <div style='background-color: #d9534f; color: white; padding: 20px; text-align: center;'>
                            <h2 style='margin: 0; font-size: 22px;'>CẢNH BÁO ĐĂNG NHẬP BẤT THƯỜNG</h2>
                        </div>
                        <div style='padding: 20px; color: #333333; line-height: 1.6;'>
                            <p>Xin chào <b>{username}</b>,</p>
                            <p>Hệ thống giám sát hành vi của chúng tôi vừa ghi nhận dấu hiệu truy cập nghi vấn vào tài khoản của bạn.</p>
                            
                            <div style='background-color: #f9f2f2; border-left: 4px solid #d9534f; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                                <strong style='color: #c9302c;'>Chi tiết phát hiện rủi ro:</strong><br/>
                                <span style='font-size: 14px;'>{formattedReasons}</span>
                            </div>

                            <table style='width: 100%; border-collapse: collapse; margin-top: 15px; font-size: 14px;'>
                                <tr>
                                    <td style='padding: 6px 0; color: #666;'>Thời gian hệ thống:</td>
                                    <td style='padding: 6px 0; font-weight: bold;'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 6px 0; color: #666;'>Môi trường yêu cầu:</td>
                                    <td style='padding: 6px 0; font-weight: bold;'>Ứng dụng Desktop Giám sát</td>
                                </tr>
                            </table>

                            <hr style='border: 0; border-top: 1px solid #eeeeee; margin: 20px 0;'/>
                            <p style='font-size: 13px; color: #666666;'>
                                <b>Hành động khuyến nghị:</b> Nếu hành vi này <b>KHÔNG</b> phải do bạn thực hiện, tài khoản của bạn có thể đang bị rò rỉ thông tin. Vui lòng đăng nhập lập tức để đổi mật khẩu hoặc liên hệ Phòng Quản trị CNTT để tạm khóa tài khoản.
                            </p>
                        </div>
                        <div style='background-color: #f5f5f5; padding: 15px; text-align: center; font-size: 12px; color: #999999;'>
                            Đây là email tự động từ hệ thống bảo mật an ninh. Vui lòng không phản hồi lại email này.
                        </div>
                    </div>";

                    mail.IsBodyHtml = true;

                    // Khởi tạo Client gửi dữ liệu lên Mail Server
                    using (SmtpClient smtp = new SmtpClient(SmtpHost, SmtpPort))
                    {
                        // Sử dụng biến senderEmail và appPassword động
                        smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
                        smtp.EnableSsl = true;

                        // Gửi bất đồng bộ hoàn toàn
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi nội bộ để phục vụ Debug, không quăng ngoại lệ làm gián đoạn ứng dụng chính
                System.Diagnostics.Debug.WriteLine("Lỗi thực thi gửi Email: " + ex.Message);
            }
        }
    }
}