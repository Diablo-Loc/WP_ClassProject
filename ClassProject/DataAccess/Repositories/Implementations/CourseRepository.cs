using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class CourseRepository
    {
        private readonly string _connString;
        private readonly My_DB _db = new My_DB();
        public CourseRepository()
        {

        }

        public DataTable GetCourses()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota FROM dbo.Course ORDER BY MaMH ASC";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn)) { adapter.Fill(table); }
            }
            return table;
        }

        public bool AddCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                // TC02: Chặn trùng tên môn học (Unicode Safe với LOWER)
                string checkNameQuery = "SELECT COUNT(1) FROM dbo.Course WHERE LOWER(TenMH) = LOWER(@tenMH)";
                using (SqlCommand checkCmd = new SqlCommand(checkNameQuery, conn))
                {
                    checkCmd.Parameters.Add("@tenMH", SqlDbType.NVarChar, 100).Value = course.TenMH;
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        throw new InvalidOperationException($"[TC02] Tên môn học '{course.TenMH}' đã tồn tại!");
                }

                string query = @"INSERT INTO dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota, Created_At) 
                                 VALUES (@maMH, @tenMH, @soTC, @tuan, @hky, @namHoc, @mota, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Chuyển toàn bộ sang SqlDbType.NVarChar theo quy định an toàn hệ thống
                    cmd.Parameters.Add("@maMH", SqlDbType.NVarChar, 10).Value = course.MaMH;
                    cmd.Parameters.Add("@tenMH", SqlDbType.NVarChar, 100).Value = course.TenMH;
                    cmd.Parameters.Add("@soTC", SqlDbType.Int).Value = (object)course.SoTC ?? DBNull.Value;
                    cmd.Parameters.Add("@tuan", SqlDbType.Int).Value = (object)course.Tuan ?? DBNull.Value;
                    cmd.Parameters.Add("@hky", SqlDbType.Int).Value = (object)course.Hky ?? DBNull.Value;
                    cmd.Parameters.Add("@namHoc", SqlDbType.NVarChar, 20).Value = (object)course.NamHoc ?? DBNull.Value;
                    cmd.Parameters.Add("@mota", SqlDbType.NVarChar, 500).Value = (object)course.Mota ?? DBNull.Value;

                    try
                    {
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                            throw new InvalidOperationException($"[TC01] Mã môn học '{course.MaMH}' đã tồn tại!");
                        throw;
                    }
                }
            }
        }

        public bool UpdateCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE dbo.Course 
                                 SET TenMH = @tenMH, SoTC = @soTC, Tuan = @tuan, Hky = @hky, NamHoc = @namHoc, Mota = @mota, Updated_At = GETDATE() 
                                 WHERE MaMH = @maMH";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maMH", SqlDbType.NVarChar, 10).Value = course.MaMH;
                    cmd.Parameters.Add("@tenMH", SqlDbType.NVarChar, 100).Value = course.TenMH;
                    cmd.Parameters.Add("@soTC", SqlDbType.Int).Value = (object)course.SoTC ?? DBNull.Value;
                    cmd.Parameters.Add("@tuan", SqlDbType.Int).Value = (object)course.Tuan ?? DBNull.Value;
                    cmd.Parameters.Add("@hky", SqlDbType.Int).Value = (object)course.Hky ?? DBNull.Value;
                    cmd.Parameters.Add("@namHoc", SqlDbType.NVarChar, 20).Value = (object)course.NamHoc ?? DBNull.Value;
                    cmd.Parameters.Add("@mota", SqlDbType.NVarChar, 500).Value = (object)course.Mota ?? DBNull.Value;

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteCourse(string maMH)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                // Chặn xóa nếu môn học đã được dùng để mở Lớp học phần (CourseSection)
                string checkSectionQuery = "SELECT COUNT(1) FROM dbo.CourseSection WHERE MaMH = @maMH";
                using (SqlCommand checkCmd = new SqlCommand(checkSectionQuery, conn))
                {
                    checkCmd.Parameters.Add("@maMH", SqlDbType.NVarChar, 10).Value = maMH.Trim();
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        throw new InvalidOperationException("Không thể xóa môn học này vì đã có các Lớp học phần đang sử dụng!");
                }

                string deleteQuery = "DELETE FROM dbo.Course WHERE MaMH = @maMH";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.Add("@maMH", SqlDbType.NVarChar, 10).Value = maMH.Trim();
                    return deleteCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable SearchCourses(string keyword)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota FROM dbo.Course WHERE MaMH LIKE @key OR TenMH LIKE @key ORDER BY MaMH ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@key", SqlDbType.NVarChar, 100).Value = "%" + keyword.Trim() + "%";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) { adapter.Fill(table); }
                }
            }
            return table;
        }
        public DataTable FilterCoursesBySemester(int hky)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota FROM dbo.Course WHERE Hky = @hky ORDER BY MaMH ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@hky", SqlDbType.Int).Value = hky;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }
    }
}