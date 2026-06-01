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

        // 4. Lấy danh sách đã đăng ký nạp lên DataGridView
        public DataTable GetRegistrationList(string mssv)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    
                    string query = @"SELECT 
                                ROW_NUMBER() OVER (ORDER BY r.RegistrationDate DESC) AS STT,
                                r.MaMH AS CourseId,
                                c.TenMH AS CourseName,
                                ISNULL(c.SoTC, 0) AS Credits,
                                N'Chưa phân công' AS Teacher,
                                ISNULL(c.Hky, 0) AS Semester
                             FROM dbo.DKMH r
                             JOIN dbo.Course c ON r.MaMH = c.MaMH
                             WHERE r.MSSV = @mssv";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi kết nối");
                }
            }
            return dt;
        }

        // 5. Tính tổng số tín chỉ sinh viên đã đăng ký
        public int GetTotalCreditsRegistered(string mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT ISNULL(SUM(c.SoTC), 0) 
                             FROM dbo.CourseRegistration r 
                             JOIN dbo.Course c ON r.CourseId = c.MaMH 
                             WHERE r.Mssv = @mssv";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch { return 0; }
            }
        }

        //6. Hàm đếm tổng số môn học sinh viên đó đã đăng ký
        public int GetTotalCoursesRegistered(string mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM dbo.CourseRegistration WHERE Mssv = @mssv";
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