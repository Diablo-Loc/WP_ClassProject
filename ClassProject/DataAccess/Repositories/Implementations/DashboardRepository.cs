using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 1. Lấy dữ liệu xếp loại học lực (Pie Chart)
        /// Thêm mssv = null để dùng chung cho cả Admin (Toàn trường) và Sinh viên (Cá nhân)
        /// </summary>
        public async Task<DataTable> GetAcademicRankingStatisticsAsync(string mssv = null)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetAcademicRankingStatistics", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Nếu truyền mssv, nạp tham số vào cmd để lọc dưới SQL
                    if (!string.IsNullOrEmpty(mssv))
                    {
                        cmd.Parameters.AddWithValue("@MSSV", mssv);
                    }

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await conn.OpenAsync();
                        await Task.Run(() => adapter.Fill(dt));
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 2. Lấy xu hướng nhập học / Tiến độ điểm học kỳ (Line Chart)
        /// </summary>
        public async Task<DataTable> GetEnrollmentTrendStatisticsAsync(string mssv = null)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetEnrollmentTrendStatistics", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (!string.IsNullOrEmpty(mssv))
                    {
                        cmd.Parameters.AddWithValue("@MSSV", mssv);
                    }

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await conn.OpenAsync();
                        await Task.Run(() => adapter.Fill(dt));
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 3. Lấy dữ liệu các thẻ thông số tổng quan (Summary Cards)
        /// </summary>
        public async Task<DataRow> GetDashboardSummaryCardsAsync(string mssv = null)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetDashboardSummaryCards", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (!string.IsNullOrEmpty(mssv))
                    {
                        cmd.Parameters.AddWithValue("@MSSV", mssv);
                    }

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await conn.OpenAsync();
                        await Task.Run(() => adapter.Fill(dt));
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 4. Lấy danh sách Top 10 sinh viên xuất sắc (DataGridView)
        /// Giữ nguyên hoàn toàn không đổi vì sinh viên hay admin đều xem chung bảng vinh danh này.
        /// </summary>
        public async Task<DataTable> GetTopStudentsRankingAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetTopStudentsRanking", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await conn.OpenAsync();
                        await Task.Run(() => adapter.Fill(dt));
                        return dt;
                    }
                }
            }
        }
        /// <summary>
        /// 5. Lấy trọn gói toàn bộ dữ liệu Dashboard cho Giảng viên (Thẻ số, 2 Biểu đồ, Grid)
        /// Sử dụng DataSet để gom 4 kết quả trả về từ Store Procedure chỉ trong 1 lần quét DB.
        /// </summary>
        public async Task<DataSet> GetTeacherDashboardDataSetAsync(string teacherId)
        {
            if (string.IsNullOrEmpty(teacherId))
                throw new ArgumentException("Mã số giảng viên không được để trống.", nameof(teacherId));

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_GetTeacherDashboardData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                    DataSet ds = new DataSet();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await conn.OpenAsync();
                        // Chạy Fill ngầm thông qua Task.Run để tối ưu hiệu năng UI Thread
                        await Task.Run(() => adapter.Fill(ds));
                        return ds;
                    }
                }
            }
        }
    }
}