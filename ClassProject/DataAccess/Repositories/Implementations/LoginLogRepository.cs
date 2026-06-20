using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;

namespace ClassProject.DataAccess.Repositories
{
    public class LoginLogRepository
    {
        private readonly My_DB _db;

        public LoginLogRepository()
        {
            _db = new My_DB();
        }

        /// <summary>
        /// Ghi nhận nhật ký đăng nhập vào Cơ sở dữ liệu (Hoàn toàn bất đồng bộ)
        /// </summary>
        public async Task RecordLogAsync(string username, bool isSuccess, string loginMethod, string failureReason)
        {
            const string query = @"
                INSERT INTO UserLoginLogs (Username, IsSuccess, LoginMethod, IPAddress, UserAgent, FailureReason) 
                VALUES (@Username, @IsSuccess, @LoginMethod, @IPAddress, @UserAgent, @FailureReason);";

            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Gán các giá trị tham số để chống lỗi SQL Injection
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                        cmd.Parameters.Add("@IsSuccess", SqlDbType.Bit).Value = isSuccess;
                        cmd.Parameters.Add("@LoginMethod", SqlDbType.NVarChar, 20).Value = loginMethod;
                        cmd.Parameters.Add("@FailureReason", SqlDbType.NVarChar, 250).Value = (object)failureReason ?? DBNull.Value;

                        // Thu thập footprint cơ bản trên môi trường WinForms doanh nghiệp
                        cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 45).Value = GetLocalIPAddress();
                        cmd.Parameters.Add("@UserAgent", SqlDbType.NVarChar, 500).Value = $"WinForms_App; OS:{Environment.OSVersion}; Machine:{Environment.MachineName}";

                        if (conn.State != ConnectionState.Open)
                        {
                            await conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Chuẩn doanh nghiệp: Lỗi hệ thống ghi log không được phép làm sập luồng nghiệp vụ chính của khách hàng
                // Bạn có thể tích hợp ghi log ra file vật lý (.txt) ở đây nếu cần thiết
                System.Diagnostics.Debug.WriteLine("Lỗi ghi log DB: " + ex.Message);
            }
        }

        /// <summary>
        /// Hàm bổ trợ lấy IP nội bộ của máy trạm chạy ứng dụng WinForms
        /// </summary>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}