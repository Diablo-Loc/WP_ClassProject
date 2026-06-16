using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class ScoreRepository
    {
        private readonly string _connectionString;
        private readonly My_DB _db = new My_DB();

        public ScoreRepository()
        {
        }

        public bool SaveScore(string mssv, string maLopHP, decimal diemQt, decimal diemCk, decimal diemTk, string mota)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    MERGE dbo.Score AS target
                    USING (SELECT @mssv AS MSSV, @maLopHP AS MaLopHP) AS src
                    ON target.MSSV = src.MSSV AND target.MaLopHP = src.MaLopHP
                    WHEN MATCHED THEN
                        UPDATE SET DiemQT = @qt, DiemCK = @ck, DiemTK = @tk, Mota = @mota
                    WHEN NOT MATCHED THEN
                        INSERT (MSSV, MaLopHP, DiemQT, DiemCK, DiemTK, Mota) 
                        VALUES (@mssv, @maLopHP, @qt, @ck, @tk, @mota);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();

                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = maLopHP.Trim();

                    cmd.Parameters.Add("@qt", SqlDbType.Decimal).Value = diemQt;
                    cmd.Parameters.Add("@ck", SqlDbType.Decimal).Value = diemCk;
                    cmd.Parameters.Add("@tk", SqlDbType.Decimal).Value = diemTk;
                    cmd.Parameters.Add("@mota", SqlDbType.NVarChar, 200).Value = (object)mota ?? DBNull.Value;

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Lỗi hệ thống khi lưu điểm: " + ex.Message);
                    }
                }
            }
        }

        public DataTable GetScoreList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT 
                                    sc.MSSV AS [Mã SV],
                                    (s.LastName + ' ' + s.FirstName) AS [Họ và Tên],
                                    sc.MaLopHP AS [Mã Lớp HP],
                                    c.TenMH AS [Tên Môn Học],
                                    cs.HocKy AS [Học Kỳ],
                                    cs.NamHoc AS [Năm Học],
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
                                 JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP
                                 JOIN dbo.Course c ON cs.MaMH = c.MaMH";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Không thể tải danh sách điểm: " + ex.Message);
                    }
                }
            }
            return dt;
        }

        public bool DeleteScore(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM dbo.Score WHERE MSSV = @mssv AND MaLopHP = @maLopHP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();

                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = maLopHP.Trim();

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Lỗi khi xóa điểm: " + ex.Message);
                    }
                }
            }
        }

        public decimal GetStudentGPA(string mssv)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT ISNULL(AVG(DiemTK), 0) FROM dbo.Score WHERE MSSV = @mssv";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    try
                    {
                        conn.Open();
                        return Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
        }

        public bool HasCourseScore(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM dbo.Score WHERE MSSV = @mssv AND MaLopHP = @maLopHP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();

                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = maLopHP.Trim();
                    try
                    {
                        conn.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public DataTable GetQuickStats()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM dbo.Students) AS TotalStudents,
                        (SELECT COUNT(*) FROM dbo.CourseSection) AS TotalCourses,
                        (SELECT COUNT(*) FROM dbo.Score) AS TotalScoresEntered,
                        ISNULL((SELECT ROUND(AVG(DiemTK), 2) FROM dbo.Score), 0) AS AverageSchoolScore";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch { }
                }
            }
            return dt;
        }

        public DataTable GetStudentTranscripts(string mssv)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT 
                                    MaLopHP AS [Mã Lớp HP],
                                    MaMH AS [Mã Môn],
                                    TenMH AS [Tên Môn Học],
                                    SoTC AS [Số Tín Chỉ],
                                    HocKy AS [Học Kỳ],
                                    NamHoc AS [Năm Học],
                                    DiemQT AS [Điểm QT],
                                    DiemCK AS [Điểm CK],
                                    DiemTK AS [Điểm TK]
                                 FROM dbo.vw_StudentTranscript 
                                 WHERE MSSV = @mssv";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            conn.Open();
                            da.Fill(dt);
                        }
                        catch (SqlException ex)
                        {
                            throw new Exception("Lỗi khi tải bảng điểm cá nhân: " + ex.Message);
                        }
                    }
                }
            }
            return dt;
        }
    }
}