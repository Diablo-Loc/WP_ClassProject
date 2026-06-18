using ClassProject.DataAccess.Db;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class GetNotificationRepository
    {
        public DataTable GetNotificationData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Title", typeof(string));

            My_DB db = new My_DB();
            string query = "";

            if (UserSession.RoleId == 0)
            {
                dt.Rows.Add(0, "📢 Vui lòng đăng nhập lại hệ thống.");
                return dt;
            }

            switch (UserSession.RoleId)
            {
                case 3: // GIÁO VỤ
                    query = @"SELECT TOP 5 Id, 
                      (N'🔔 Yêu cầu mới từ SV [' + MSSV + N'] đang chờ duyệt') AS Title
                      FROM dbo.Requests 
                      WHERE Status = N'Pending' 
                      ORDER BY Created_At DESC";
                    break;

                case 1: // SINH VIÊN
                    // CẬP NHẬT: Thêm điều kiện lọc chỉ lấy các yêu cầu Đã duyệt (Approved) hoặc Từ chối (Rejected)
                    query = @"SELECT TOP 5 Id, 
                      (N'📄 Yêu cầu ' + ISNULL(RequestType, N'Xử lý hồ sơ') + N': ' + 
                       CASE 
                            WHEN Status = N'Approved' THEN N'✅ Đã chấp nhận'
                            WHEN Status = N'Rejected' THEN N'❌ Bị từ chối'
                            ELSE Status 
                       END) AS Title
                      FROM dbo.Requests 
                      WHERE TRIM(MSSV) = @StudentCode 
                        AND Status IN (N'Approved', N'Rejected') 
                      ORDER BY Created_At DESC";
                    break;

                default: // CÁC VAI TRÒ KHÁC
                    query = @"SELECT 0 AS Id, N'📢 Hệ thống bảo mật UTEID hoạt động ổn định.' AS Title";
                    break;
            }

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (UserSession.RoleId == 1)
                        {
                            string currentMSSV = string.IsNullOrEmpty(UserSession.Username) ? "" : UserSession.Username.Trim();
                            cmd.Parameters.AddWithValue("@StudentCode", currentMSSV);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi nạp thông báo: " + ex.Message);
                dt.Rows.Clear();
                dt.Rows.Add(0, "⚠️ Lỗi kết nối dữ liệu thông báo.");
            }

            return dt;
        }
    }
}