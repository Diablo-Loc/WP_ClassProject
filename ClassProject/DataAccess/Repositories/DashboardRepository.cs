using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// 1. Lấy dữ liệu xếp loại học lực (Pie Chart)
        public async Task<DataTable> GetAcademicRankingStatisticsAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetAcademicRankingStatistics", conn))
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

        /// 2. Lấy xu hướng nhập học (Line Chart)
        public async Task<DataTable> GetEnrollmentTrendStatisticsAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetEnrollmentTrendStatistics", conn))
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

        /// 3. Lấy danh sách Top 10 sinh viên xuất sắc (DataGridView)
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

        /// 4. Lấy dữ liệu các thẻ thông số tổng quan (Summary Cards)
        public async Task<DataRow> GetDashboardSummaryCardsAsync()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("proc_GetDashboardSummaryCards", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
    }
}