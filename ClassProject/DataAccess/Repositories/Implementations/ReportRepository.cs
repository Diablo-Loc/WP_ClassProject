using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories.Interfaces;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly My_DB _db = new My_DB();

        public async Task<DataTable> GetCoursesAsync()
        {
            DataTable dt = new DataTable();
            string query = "SELECT MaMH, TenMH FROM dbo.Course";

            using (SqlConnection conn = new SqlConnection(_db.GetConnection().ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    // Chạy Async giúp Form không bị giật lag khi mở
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        public async Task<DataTable> GetScoreReportDataAsync(string maMH)
        {
            DataTable dt = new DataTable();

            // Khuyên dùng: Sử dụng cụ thể cột thay vì SELECT * // và ép kiểu an toàn cho Điểm số phòng trường hợp DB null
            string query = @"SELECT 
                                s.MSSV, 
                                (s.FirstName + ' ' + s.LastName) AS HoTen, 
                                c.MaMH, 
                                c.TenMH, 
                                ISNULL(sc.DiemQT, 0) AS DiemQT, 
                                ISNULL(sc.DiemCK, 0) AS DiemCK, 
                                ISNULL(sc.DiemTK, 0) AS DiemTK
                             FROM dbo.Score sc
                             INNER JOIN dbo.Students s ON sc.MSSV = s.MSSV
                             INNER JOIN dbo.Course c ON sc.MaMH = c.MaMH
                             WHERE 1=1";

            if (!string.IsNullOrEmpty(maMH))
            {
                query += " AND sc.MaMH = @MaMH";
            }

            using (SqlConnection conn = new SqlConnection(_db.GetConnection().ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrEmpty(maMH))
                {
                    // Loại bỏ nối chuỗi, dùng tham số hóa chuẩn xác pass test case bảo mật
                    cmd.Parameters.AddWithValue("@MaMH", maMH);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }
    }
}