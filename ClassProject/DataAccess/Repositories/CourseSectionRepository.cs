using ClassProject.DataAccess.Db;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class CourseSectionRepository
    {
        private readonly My_DB _db = new My_DB();

        public CourseSectionRepository()
        {
        }

        public DataTable GetCourseSections()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                // ĐỒNG BỘ VẬT LÝ: Ghép FirstName + LastName theo chuẩn (Họ + Tên lót + Tên)
                // Lưu ý: Nếu DB của bạn thiết kế t.LastName là Họ, t.FirstName là Tên, hãy đổi lại là t.LastName + ' ' + t.FirstName
                string query = @"
                    SELECT 
                        cs.MaLopHP, 
                        cs.MaMH, 
                        c.TenMH,
                        cs.HocKy, 
                        cs.NamHoc, 
                        cs.MSGV, 
                        ISNULL(t.LastName + ' ' + t.FirstName, N'Chưa phân công') AS TenGiangVien,
                        cs.PhongHoc, 
                        cs.MaxStudents,
                        cs.Status,
                        (SELECT COUNT(1) FROM dbo.DKMH dk WHERE dk.MaLopHP = cs.MaLopHP) AS SisoHienTai
                    FROM dbo.CourseSection cs
                    INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
                    LEFT JOIN dbo.Teachers t ON cs.MSGV = t.MSGV
                    ORDER BY cs.NamHoc DESC, cs.HocKy DESC, cs.MaLopHP ASC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public bool AddSection(CourseSection section)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO dbo.CourseSection 
                                 (MaLopHP, MaMH, HocKy, NamHoc, MSGV, PhongHoc, MaxStudents, Status, Created_At)
                                 VALUES (@maLopHP, @maMH, @hocKy, @namHoc, @msgv, @phongHoc, @maxStudents, @status, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // ĐỒNG BỘ VẬT LÝ: Đổi sang SqlDbType.VarChar cho các trường Mã hệ thống
                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = section.MaLopHP.Trim();
                    cmd.Parameters.Add("@maMH", SqlDbType.VarChar, 20).Value = section.MaMH.Trim();

                    cmd.Parameters.Add("@hocKy", SqlDbType.Int).Value = section.HocKy;
                    cmd.Parameters.Add("@namHoc", SqlDbType.NVarChar, 20).Value = section.NamHoc.Trim();

                    // ĐỒNG BỘ VẬT LÝ: Trả MSGV về VarChar để khớp chuẩn khóa ngoại bảng Teachers
                    cmd.Parameters.Add("@msgv", SqlDbType.VarChar, 30).Value = string.IsNullOrWhiteSpace(section.MSGV) ? DBNull.Value : (object)section.MSGV.Trim();

                    // Giữ nguyên NVarChar vì phòng học và trạng thái có thể có ký tự đặc biệt/Unicode
                    cmd.Parameters.Add("@phongHoc", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(section.PhongHoc) ? DBNull.Value : (object)section.PhongHoc;
                    cmd.Parameters.Add("@maxStudents", SqlDbType.Int).Value = section.MaxStudents;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = section.Status;

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                            throw new InvalidOperationException($"Mã lớp học phần '{section.MaLopHP}' đã tồn tại trong hệ thống!");
                        throw;
                    }
                }
            }
        }

        public bool UpdateSection(CourseSection section)
        {
            if (section == null) throw new ArgumentNullException(nameof(section));

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE dbo.CourseSection 
                                 SET MSGV = @msgv, PhongHoc = @phongHoc, MaxStudents = @maxStudents, Status = @status, Updated_At = GETDATE()
                                 WHERE MaLopHP = @maLopHP";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // ĐỒNG BỘ VẬT LÝ: Đổi sang VarChar đồng bộ với cấu trúc bảng
                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = section.MaLopHP.Trim();
                    cmd.Parameters.Add("@msgv", SqlDbType.VarChar, 30).Value = string.IsNullOrWhiteSpace(section.MSGV) ? DBNull.Value : (object)section.MSGV.Trim();

                    cmd.Parameters.Add("@phongHoc", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(section.PhongHoc) ? DBNull.Value : (object)section.PhongHoc;
                    cmd.Parameters.Add("@maxStudents", SqlDbType.Int).Value = section.MaxStudents;
                    cmd.Parameters.Add("@status", SqlDbType.Int).Value = section.Status;

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteSection(string maLopHP)
        {
            if (string.IsNullOrWhiteSpace(maLopHP)) return false;

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                // ĐỒNG BỘ VẬT LÝ: Kiểm tra ràng buộc đăng ký môn học (DKMH) trước khi xóa
                string checkQuery = "SELECT COUNT(1) FROM dbo.DKMH WHERE MaLopHP = @maLopHP";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = maLopHP.Trim();
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        throw new InvalidOperationException("Không thể xóa lớp học phần này vì đã có dữ liệu sinh viên đăng ký học!");
                }

                string deleteQuery = "DELETE FROM dbo.CourseSection WHERE MaLopHP = @maLopHP";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.Add("@maLopHP", SqlDbType.VarChar, 30).Value = maLopHP.Trim();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}