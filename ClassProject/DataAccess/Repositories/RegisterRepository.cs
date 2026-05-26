using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class RegisterRepository
    {
        private string _connectionString;

        public RegisterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =========================================================
        // LOGIC TUẦN 6: ĐĂNG KÝ MÔN HỌC
        // =========================================================

        // 1. Hàm Đăng ký môn học (INSERT)
        public bool AddRegistration(string mssv, string courseId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO dbo.CourseRegistration (Mssv, CourseId, RegistrationDate) VALUES (@mssv, @courseId, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@courseId", courseId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 2. Hàm Hủy đăng ký (DELETE)
        public bool CancelRegistration(string mssv, string courseId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM dbo.CourseRegistration WHERE Mssv = @mssv AND CourseId = @courseId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@courseId", courseId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 3. Kiểm tra sinh viên đã đăng ký môn này chưa
        public bool IsRegistered(string mssv, string courseId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM dbo.CourseRegistration WHERE Mssv = @mssv AND CourseId = @courseId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@courseId", courseId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 4. Lấy danh sách đã đăng ký nạp lên DataGridView (Đã FIX JOIN c.MaMH)
        public DataTable GetRegistrationList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                                        r.Mssv AS [Mã SV],
                                        (s.LastName + ' ' + s.FirstName) AS [Họ và Tên],
                                        r.CourseId AS [Mã MH],
                                        c.TenMH AS [Tên Môn Học],
                                        r.RegistrationDate AS [Ngày Đăng Ký]
                                     FROM dbo.CourseRegistration r
                                     JOIN dbo.Students s ON r.Mssv = s.MSSV
                                     JOIN dbo.Course c ON r.CourseId = c.MaMH";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.Fill(dt);
                    }
                }
                catch { }
            }
            return dt;
        }

        // =========================================================
        // LOGIC TUẦN 7: QUẢN LÝ ĐIỂM SỐ (SCORE)
        // =========================================================

        // Trước khi lưu điểm chi tiết, ta cần đảm bảo bảng CourseRegistration 
        // có đủ cột DiemQT và DiemCK để chạy. Hàm này sẽ tự kiểm tra và cập nhật Database tự động.
        public void EnsureScoreColumnsExist()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        IF COL_LENGTH('dbo.CourseRegistration', 'DiemQT') IS NULL
                            ALTER TABLE dbo.CourseRegistration ADD DiemQT DECIMAL(4,2) NULL;
                        IF COL_LENGTH('dbo.CourseRegistration', 'DiemCK') IS NULL
                            ALTER TABLE dbo.CourseRegistration ADD DiemCK DECIMAL(4,2) NULL;
                    ";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }

        // 5. Hàm Lưu điểm số cho Sinh viên (Cập nhật Score, DiemQT, DiemCK)
        public bool SaveScore(string mssv, string courseId, decimal diemQt, decimal diemCk, decimal diemTk)
        {
            EnsureScoreColumnsExist();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    // Cập nhật điểm số dựa trên bản ghi đăng ký môn học đã tồn tại
                    string query = @"UPDATE dbo.CourseRegistration 
                                     SET DiemQT = @diemQt, 
                                         DiemCK = @diemCk, 
                                         Score = @diemTk 
                                     WHERE Mssv = @mssv AND CourseId = @courseId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        cmd.Parameters.AddWithValue("@courseId", courseId);
                        cmd.Parameters.AddWithValue("@diemQt", diemQt);
                        cmd.Parameters.AddWithValue("@diemCk", diemCk);
                        cmd.Parameters.AddWithValue("@diemTk", diemTk); // Score lưu điểm tổng kết

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 6. Lấy bảng điểm tổng hợp hiển thị lên Form Quản Lý Điểm
        public DataTable GetScoreList()
        {
            EnsureScoreColumnsExist();
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                                        r.Mssv AS [Mã SV],
                                        (s.LastName + ' ' + s.FirstName) AS [Họ và Tên],
                                        r.CourseId AS [Mã MH],
                                        c.TenMH AS [Tên Môn Học],
                                        ISNULL(r.DiemQT, 0) AS [Điểm QT (40%)],
                                        ISNULL(r.DiemCK, 0) AS [Điểm CK (60%)],
                                        ISNULL(r.Score, 0) AS [Điểm Tổng Kết]
                                     FROM dbo.CourseRegistration r
                                     JOIN dbo.Students s ON r.Mssv = s.MSSV
                                     JOIN dbo.Course c ON r.CourseId = c.MaMH";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.Fill(dt);
                    }
                }
                catch { }
            }
            return dt;
        }
        public int GetTotalCreditsRegistered(string mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    // Lấy tổng số tín chỉ bằng cách SUM cột SoTC từ bảng Course thông qua liên kết DKMH
                    string query = @"SELECT ISNULL(SUM(c.SoTC), 0) 
                             FROM dbo.DKMH r 
                             JOIN dbo.Course c ON r.MaMH = c.MaMH 
                             WHERE r.MSSV = @mssv";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch { return 0; }
            }
        }
    }
}