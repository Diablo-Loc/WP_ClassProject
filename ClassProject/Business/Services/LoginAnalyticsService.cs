using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;

namespace ClassProject.Business.Services
{
    public class LoginAnalyticsService
    {
        private readonly My_DB _db;

        public LoginAnalyticsService()
        {
            _db = new My_DB();
        }

        /// <summary>
        /// Phân tích hành vi để xác định xem lượt đăng nhập này có bất thường hay không.
        /// </summary>
        /// <param name="username">Tên tài khoản cần phân tích</param>
        /// <returns>Trả về chuỗi chứa lý do cảnh báo nếu có bất thường; trả về null nếu an toàn.</returns>
        public async Task<string> AnalyzeBehaviorAsync(string username)
        {
            bool isSuspicious = false;
            string anomalyReason = "";

            // --- QUY TẮC 1: PHÂN TÍCH KHUNG GIỜ NHẠY CẢM (GIỜ LẠ) ---
            int currentHour = DateTime.Now.Hour;
            // Doanh nghiệp thường cấu hình khung giờ nghiêm ngặt từ 23h đêm đến 4h sáng
            if (currentHour >= 23 || currentHour <= 4)
            {
                isSuspicious = true;
                anomalyReason += $"- Đăng nhập vào khung giờ đêm muộn/rạng sáng ({currentHour}h).\n";
            }

            // --- QUY TẮC 2: PHÂN TÍCH TẦN SUẤT THẤT BẠI (DÒ MẬT KHẨU / FAKE FACE) ---
            const string query = @"
                SELECT COUNT(*) 
                FROM UserLoginLogs 
                WHERE Username = @Username 
                  AND IsSuccess = 0 
                  AND LoginTime >= DATEADD(minute, -5, GETDATE());";

            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

                        if (conn.State != ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }

                        // Thực hiện đếm số lần fail trong 5 phút qua
                        int failedAttempts = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        // Chuẩn bảo mật doanh nghiệp: Quá 3 lần thử sai liên tiếp trong thời gian ngắn là dấu hiệu Brute-force
                        if (failedAttempts >= 3)
                        {
                            isSuspicious = true;
                            anomalyReason += $"- Phát hiện {failedAttempts} lần thử đăng nhập thất bại liên tiếp trong vòng 5 phút qua.\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi phân tích DB: " + ex.Message);
            }

            // Nếu có bất kỳ dấu hiệu bất thường nào, trả về tổng hợp lý do để chuẩn bị gửi mail
            return isSuspicious ? anomalyReason.TrimEnd() : null;
        }
    }
}