using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class StatisticRepository
    {
        private readonly string _connectionString;

        public StatisticRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region ==================== 1. PHÂN HỆ TOÀN TRƯỜNG (ADMIN / HR) ====================

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

        /// Lấy thông tin tóm tắt 3 thẻ số liệu Dashboard toàn trường (Bất đồng bộ)
        public async Task<DataRow> GetDashboardCardMetricsAsync()
        {
            DataTable dt = new DataTable();

            const string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM dbo.Students) AS TotalStudents,
                    (SELECT ISNULL(AVG(DiemTK), 0.0) FROM dbo.Score) AS AvgGPA,
                    (SELECT CASE WHEN COUNT(DISTINCT MSSV) = 0 THEN 0.0 
                            ELSE (COUNT(DISTINCT CASE WHEN DiemTK >= 9.0 THEN MSSV END) * 100.0 / COUNT(DISTINCT MSSV)) END 
                     FROM dbo.Score) AS ExcellentRate";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        #endregion

        #region ==================== 2. PHÂN HỆ GIẢNG VIÊN (INSTRUCTOR) ====================

        /// Lấy 3 thẻ số liệu KPI nhưng chỉ giới hạn trong danh sách sinh viên thuộc các lớp mà Giảng viên này dạy
        public async Task<DataRow> GetDashboardCardMetricsByInstructorAsync(string instructorId)
        {
            DataTable dt = new DataTable();

            // INNER JOIN với CourseSection để lọc chính xác theo cột MSGV thay vì MaGV
            const string query = @"
        SELECT 
            (SELECT COUNT(DISTINCT sc.MSSV) 
             FROM dbo.Score sc 
             INNER JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP 
             WHERE cs.MSGV = @InstructorID) AS TotalStudents,
             
            (SELECT ISNULL(AVG(sc.DiemTK), 0.0) 
             FROM dbo.Score sc 
             INNER JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP 
             WHERE cs.MSGV = @InstructorID) AS AvgGPA,
             
            (SELECT CASE WHEN COUNT(DISTINCT sc.MSSV) = 0 THEN 0.0 
                    ELSE (COUNT(DISTINCT CASE WHEN sc.DiemTK >= 9.0 THEN sc.MSSV END) * 100.0 / COUNT(DISTINCT sc.MSSV)) END 
             FROM dbo.Score sc 
             INNER JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP 
             WHERE cs.MSGV = @InstructorID) AS ExcellentRate";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@InstructorID", instructorId);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    dt.Load(reader);
                }
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        /// Lấy danh sách Top 10 sinh viên có điểm cao nhất trong các môn do Giảng viên này phụ trách
        public DataTable GetTopStudentsByInstructor(string instructorId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Đã sửa st.FullName thành phép cộng chuỗi (st.LastName + ' ' + st.FirstName)
                const string query = @"
            SELECT TOP 10 
                s.MSSV, 
                (st.LastName + ' ' + st.FirstName) AS FullName, 
                s.DiemTK AS GPA,
                CASE 
                    WHEN s.DiemTK >= 9.0 THEN N'Xuất sắc'
                    WHEN s.DiemTK >= 8.0 THEN N'Giỏi'
                    WHEN s.DiemTK >= 6.5 THEN N'Khá'
                    WHEN s.DiemTK >= 5.0 THEN N'Trung bình'
                    ELSE N'Yếu'
                END AS Classification
            FROM dbo.Score s
            INNER JOIN dbo.Students st ON s.MSSV = st.MSSV
            INNER JOIN dbo.CourseSection cs ON s.MaLopHP = cs.MaLopHP
            WHERE cs.MSGV = @InstructorID
            ORDER BY s.DiemTK DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InstructorID", instructorId);

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
                    throw new Exception("Lỗi khi lấy danh sách xếp hạng của giảng viên từ Database: " + ex.Message, ex);
                }
            }
        }

        /// Thống kê số lượng sinh viên theo từng nhóm học lực trong các lớp của Giảng viên (để vẽ PieChart)
        public DataTable GetAcademicRankingStatsByInstructor(string instructorId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                const string query = @"
            SELECT 
                CASE 
                    WHEN sc.DiemTK >= 9.0 THEN N'Xuất sắc'
                    WHEN sc.DiemTK >= 8.0 THEN N'Giỏi'
                    WHEN sc.DiemTK >= 6.5 THEN N'Khá'
                    WHEN sc.DiemTK >= 5.0 THEN N'Trung bình'
                    ELSE N'Yếu'
                END AS RankingGroup,
                COUNT(*) AS StudentCount
            FROM dbo.Score sc
            INNER JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP
            WHERE cs.MSGV = @InstructorID
            GROUP BY 
                CASE 
                    WHEN sc.DiemTK >= 9.0 THEN N'Xuất sắc'
                    WHEN sc.DiemTK >= 8.0 THEN N'Giỏi'
                    WHEN sc.DiemTK >= 6.5 THEN N'Khá'
                    WHEN sc.DiemTK >= 5.0 THEN N'Trung bình'
                    ELSE N'Yếu'
                END";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InstructorID", instructorId);

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
                    throw new Exception("Lỗi khi lấy số liệu thống kê học lực của giảng viên từ Database: " + ex.Message, ex);
                }
            }
        }

        #endregion
    }
}