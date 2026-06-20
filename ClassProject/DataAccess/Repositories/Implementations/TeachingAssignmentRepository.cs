using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class TeachingAssignmentRepository
    {
        private readonly string _connectionString;

        public TeachingAssignmentRepository(string connectionString)
        {
            _connectionString = string.IsNullOrEmpty(connectionString)
                ? throw new ArgumentNullException(nameof(connectionString), "Chuỗi kết nối cơ sở dữ liệu không được để trống.")
                : connectionString;
        }

        /// 1. Lấy danh sách phân công sử dụng Stored Procedure (Bất đồng bộ)
        public async Task<DataTable> GetAssignmentsAsync()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("proc_GetTeachingAssignments", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        /// 2. Đếm số lượng môn một giảng viên đang dạy thực tế trong DB (Bất đồng bộ)
        public async Task<int> GetCurrentCourseCountAsync(int teacherId)
        {
            const string query = "SELECT COUNT(1) FROM dbo.TeachingAssignment WHERE HRID = @HRID";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@HRID", SqlDbType.Int).Value = teacherId;

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        /// 3. Kiểm tra trùng lặp bản ghi phân công (Bất đồng bộ)
        public async Task<bool> IsDuplicateAssignmentAsync(int teacherId, string maMH)
        {
            if (string.IsNullOrWhiteSpace(maMH)) return false;

            const string query = "SELECT TOP 1 1 FROM dbo.TeachingAssignment WHERE HRID = @HRID AND MaMH = @MaMH";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@HRID", SqlDbType.Int).Value = teacherId;
                cmd.Parameters.Add("@MaMH", SqlDbType.VarChar, 20).Value = maMH.Trim(); // Giả định MaMH có độ dài max 20 ký tự

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null;
            }
        }

        /// 4. Lấy danh sách Giảng viên (RoleId = 2) (Bất đồng bộ)
        public async Task<DataTable> GetTeacherListAsync()
        {
            var dt = new DataTable();
            const string query = "SELECT Id, Username FROM dbo.Users WHERE RoleId = 2";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
        /// 5. Lấy danh sách Môn học (Bất đồng bộ)
        public async Task<DataTable> GetCourseListAsync()
        {
            var dt = new DataTable();
            const string query = "SELECT MaMH, TenMH FROM dbo.Course";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        /// 6. Thêm bản ghi phân công giảng dạy mới (Bất đồng bộ)
        public async Task<bool> InsertAssignmentAsync(int teacherId, string maMH)
        {
            if (string.IsNullOrWhiteSpace(maMH))
                throw new ArgumentException("Mã môn học không được để trống.", nameof(maMH));

            const string query = "INSERT INTO dbo.TeachingAssignment (HRID, MaMH) VALUES (@HRID, @MaMH)";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@HRID", SqlDbType.Int).Value = teacherId;
                cmd.Parameters.Add("@MaMH", SqlDbType.VarChar, 20).Value = maMH.Trim();

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        /// 7. Xóa phân công giảng dạy dựa trên ID khóa chính (Bất đồng bộ)
        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            const string query = "DELETE FROM dbo.TeachingAssignment WHERE ID = @ID";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
        /// 8. Lấy danh sách phân công phục vụ báo cáo có bộ lọc (Bất đồng bộ)
        public async Task<DataTable> GetAssignmentsReportAsync(int? teacherId, string maMH)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("proc_GetTeachingAssignments_Report", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Cấu hình tham số lọc bộ lọc (Nếu null thì truyền DBNull.Value)
                cmd.Parameters.Add("@HRID", SqlDbType.Int).Value = teacherId.HasValue ? (object)teacherId.Value : DBNull.Value;
                cmd.Parameters.Add("@MaMH", SqlDbType.VarChar, 20).Value = !string.IsNullOrWhiteSpace(maMH) ? (object)maMH.Trim() : DBNull.Value;

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
    }
}