using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories
{
    public class StatisticRepository
    {
        private readonly string _connectionString;

        public StatisticRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetAcademicRankingStats()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_GetAcademicRankingStatistics", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                try
                {
                    conn.Open();
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lấy dữ liệu biểu đồ thống kê từ Database: " + ex.Message, ex);
                }
            }
        }

        public DataTable GetTopStudents()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_GetTopStudentsRanking", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                try
                {
                    conn.Open();
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi khi lấy danh sách bảng xếp hạng từ Database: " + ex.Message, ex);
                }
            }
        }
    }
}