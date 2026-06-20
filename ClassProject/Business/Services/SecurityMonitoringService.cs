using System;
using System.Threading.Tasks;
using ClassProject.DataAccess.Repositories;

namespace ClassProject.Business.Services
{
    public class SecurityMonitoringService
    {
        private readonly LoginLogRepository _logRepository;
        private readonly LoginAnalyticsService _analyticsService;
        private readonly EmailAlertService _emailService;

        public SecurityMonitoringService()
        {
            _logRepository = new LoginLogRepository();
            _analyticsService = new LoginAnalyticsService();
            _emailService = new EmailAlertService();
        }

        /// <summary>
        /// Điểm tiếp nhận xử lý và phân tích an ninh sau mỗi lượt đăng nhập (Chuẩn Kiến trúc Doanh nghiệp)
        /// </summary>
        /// <param name="username">Tên tài khoản người dùng thử đăng nhập</param>
        /// <param name="isSuccess">Trạng thái đăng nhập (true/false)</param>
        /// <param name="method">Phương thức ('PASSWORD' hoặc 'FACE_ID')</param>
        /// <param name="userEmail">Email của người dùng (lấy từ DB trước đó) để gửi cảnh báo nếu cần</param>
        /// <param name="failureReason">Lý do thất bại nếu có</param>
        public void ProcessSecurityAudit(string username, bool isSuccess, string method, string userEmail, string failureReason = null)
        {
            // Sử dụng Task.Run để đẩy TOÀN BỘ tiến trình giám sát xuống Thread Pool ngầm.
            // Giao diện (UI) sẽ được giải phóng ngay lập tức, người dùng vào app mượt mà không bị khựng 1 giây nào.
            _ = Task.Run(async () =>
            {
                try
                {
                    // 1. Ghi nhật ký vào CSDL ngầm
                    await _logRepository.RecordLogAsync(username, isSuccess, method, failureReason);

                    // 2. Chạy thuật toán AI quét phân tích hành vi nghi vấn
                    string anomalyReasons = await _analyticsService.AnalyzeBehaviorAsync(username);

                    // 3. Nếu phát hiện bất thường -> Kích hoạt gửi Email cảnh báo bảo mật
                    if (!string.IsNullOrEmpty(anomalyReasons) && !string.IsNullOrEmpty(userEmail))
                    {
                        await _emailService.SendSecurityAlertAsync(userEmail, username, anomalyReasons);
                    }
                }
                catch (Exception ex)
                {
                    // Đảm bảo tầng giám sát an ninh có lỗi gì cũng không làm sập phần mềm của người dùng
                    System.Diagnostics.Debug.WriteLine("Lỗi thực thi kiểm toán an ninh: " + ex.Message);
                }
            });
        }
    }
}