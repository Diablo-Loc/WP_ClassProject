using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories
{
    public class TeachingAssignmentRepository
    {
        private readonly string _connectionString;

        // Hàm khởi tạo nhận chuỗi kết nối từ Hệ thống kết nối DB của bạn
        public TeachingAssignmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// 1. Lấy danh sách phân công giảng dạy (Đã nâng cấp lấy thêm cột đếm số môn)
        public DataTable GetAssignments()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetTeachingAssignments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// 2. BÀI TẬP TỰ LÀM: Lấy số lượng môn học hiện tại một Giảng viên đang dạy để kiểm tra giới hạn 5 môn
        public int GetCurrentCourseCount(int teacherId)
        {
            string query = "SELECT COUNT(*) FROM dbo.TeachingAssignment WHERE HRID = @HRID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HRID", teacherId);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// 3. Lấy danh sách Giảng viên (Tài khoản có RoleId = 2) để đổ vào ComboBox
        public DataTable GetTeacherList()
        {
            DataTable dt = new DataTable();
            string query = "SELECT Id, Username FROM dbo.Users WHERE RoleId = 2";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// 4. Lấy danh sách Môn học từ bảng Course để đổ vào ComboBox
        public DataTable GetCourseList()
        {
            DataTable dt = new DataTable();
            string query = "SELECT MaMH, TenMH FROM dbo.Course";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// 5. Thêm phân công giảng dạy mới (Có tích hợp kiểm tra trùng lặp)
        public bool AssignTeaching(int teacherId, string maMH)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Kiểm tra an toàn: Xem cặp Giảng viên + Môn học này đã được phân công từ trước chưa
                string checkQuery = "SELECT COUNT(*) FROM dbo.TeachingAssignment WHERE HRID = @HRID AND MaMH = @MaMH";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@HRID", teacherId);
                    checkCmd.Parameters.AddWithValue("@MaMH", maMH);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        return false; // Trả về false nếu đã tồn tại phân công này
                    }
                }

                // Tiến hành chèn dữ liệu phân công mới
                string insertQuery = "INSERT INTO dbo.TeachingAssignment (HRID, MaMH) VALUES (@HRID, @MaMH)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@HRID", teacherId);
                    insertCmd.Parameters.AddWithValue("@MaMH", maMH);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// 6. Xóa phân công giảng dạy dựa trên ID khóa chính tự tăng
        public bool DeleteAssignment(int id)
        {
            string query = "DELETE FROM dbo.TeachingAssignment WHERE ID = @ID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}