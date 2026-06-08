using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class ScoreRepository
    {
        private string _connectionString;

        public ScoreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool SaveScore(string mssv, string maMH, decimal diemQt, decimal diemCk, decimal diemTk, string mota)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        MERGE dbo.Score AS target
                        USING (SELECT @mssv AS MSSV, @maMH AS MaMH) AS src
                        ON target.MSSV = src.MSSV AND target.MaMH = src.MaMH
                        WHEN MATCHED THEN
                            UPDATE SET DiemQT = @qt, DiemCK = @ck, DiemTK = @tk, Mota = @mota
                        WHEN NOT MATCHED THEN
                            INSERT (MSSV, MaMH, DiemQT, DiemCK, DiemTK, Mota) 
                            VALUES (@mssv, @maMH, @qt, @ck, @tk, @mota);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@maMH", maMH);
                        cmd.Parameters.AddWithValue("@qt", diemQt);
                        cmd.Parameters.AddWithValue("@ck", diemCk);
                        cmd.Parameters.AddWithValue("@tk", diemTk);
                        cmd.Parameters.AddWithValue("@mota", mota ?? "");
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        public DataTable GetScoreList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                                        sc.MSSV AS [Mã SV],
                                        (s.LastName + ' ' + s.FirstName) AS [Họ và Tên],
                                        sc.MaMH AS [Mã MH],
                                        c.TenMH AS [Tên Môn Học],
                                        c.Hky AS [Học Kỳ],
                                        c.NamHoc AS [Năm Học],
                                        sc.DiemQT AS [Điểm QT (40%)],
                                        sc.DiemCK AS [Điểm CK (60%)],
                                        sc.DiemTK AS [Điểm Tổng Kết],
                                        CASE 
                                            WHEN sc.DiemTK >= 8.5 THEN 4.0
                                            WHEN sc.DiemTK >= 8.0 THEN 3.5
                                            WHEN sc.DiemTK >= 7.0 THEN 3.0
                                            WHEN sc.DiemTK >= 6.5 THEN 2.5
                                            WHEN sc.DiemTK >= 5.5 THEN 2.0
                                            WHEN sc.DiemTK >= 5.0 THEN 1.5
                                            WHEN sc.DiemTK >= 4.0 THEN 1.0
                                            ELSE 0.0
                                        END AS [Hệ 4],
                                        CASE 
                                            WHEN sc.DiemTK >= 8.5 THEN N'Giỏi'
                                            WHEN sc.DiemTK >= 7.0 THEN N'Khá'
                                            WHEN sc.DiemTK >= 5.5 THEN N'Trung bình'
                                            WHEN sc.DiemTK >= 4.0 THEN N'Yếu'
                                            ELSE N'Kém'
                                        END AS [Xếp Loại],
                                        sc.Mota AS [Ghi Chú]
                                     FROM dbo.Score sc
                                     JOIN dbo.Students s ON sc.MSSV = s.MSSV
                                     JOIN dbo.Course c ON sc.MaMH = c.MaMH";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.Fill(dt);
                    }
                }
                catch { }
            }
            return dt;
        }

        public bool DeleteScore(string mssv, string maMH)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM dbo.Score WHERE MSSV = @mssv AND MaMH = @maMH";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@maMH", maMH);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        public decimal GetStudentGPA(string mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ISNULL(AVG(DiemTK), 0) FROM dbo.Score WHERE MSSV = @mssv";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        return Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }
                catch { return 0; }
            }
        }

        // 🔥 HÀM THÊM MỚI: Check chính xác xem môn học cụ thể này của SV đã có điểm chưa
        public bool HasCourseScore(string mssv, string maMH)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM dbo.Score WHERE MSSV = @mssv AND MaMH = @maMH";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@maMH", maMH);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
                catch { return false; }
            }
        }

        public DataTable GetQuickStats()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM dbo.Students) AS TotalStudents,
                    (SELECT COUNT(*) FROM dbo.Course) AS TotalCourses,
                    (SELECT COUNT(*) FROM dbo.Score) AS TotalScoresEntered,
                    ISNULL((SELECT ROUND(AVG(DiemTK), 2) FROM dbo.Score), 0) AS AverageSchoolScore";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.Fill(dt);
                    }
                }
                catch { }
            }
            return dt;
        }
    }
}