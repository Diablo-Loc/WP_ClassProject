using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class RegisterRepository
    {
        private readonly string _connString;
        private readonly My_DB _db = new My_DB();

        public RegisterRepository()
        {
        }

        // 1. Lấy danh sách các Lớp học phần một sinh viên ĐÃ ĐĂNG KÝ (Đã sửa lỗi 207 & Đọc tên giảng viên)
        public DataTable GetRegistrationList(string mssv)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                // Truy vấn trực tiếp từ View để lấy trường 'TenGiangVien' đã được đồng bộ với CSDL mới
                string query = @"
            SELECT 
                STT,
                MaLopHP,
                TenMH,
                SoTC,
                TenGiangVien, -- An toàn, không lo sai lệch cấu trúc cột vật lý
                PhongHoc,
                RegistrationDate
            FROM dbo.vw_StudentRegistrationDetail
            WHERE MSSV = @mssv";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        // 2. Đếm tổng số môn học (Lớp HP) mà sinh viên đã đăng ký
        public int GetTotalCoursesRegistered(string mssv)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM dbo.DKMH WHERE MSSV = @mssv";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // 3. Tính tổng số tín chỉ tích lũy hiện tại của sinh viên trong học kỳ 
        public int GetTotalCreditsRegistered(string mssv)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                //  ĐÃ SỬA: Thay "INNER JOIN JOIN" thành "INNER JOIN" chuẩn cú pháp
                string query = @"
            SELECT ISNULL(SUM(c.SoTC), 0)
            FROM dbo.DKMH dk
            INNER JOIN dbo.CourseSection cs ON dk.MaLopHP = cs.MaLopHP
            INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
            WHERE dk.MSSV = @mssv";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // 4. Lấy số tín chỉ của một Lớp học phần bất kỳ để tính toán trước khi bấm đăng ký
        public int GetCreditsOfSection(string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT ISNULL(c.SoTC, 0) 
                    FROM dbo.CourseSection cs
                    INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
                    WHERE cs.MaLopHP = @maLopHP";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maLopHP", SqlDbType.NVarChar, 30).Value = maLopHP.Trim();
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // 5. Kiểm tra xem sinh viên đã đăng ký Lớp học phần này chưa (Tránh trùng lịch)
        public bool IsRegistered(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM dbo.DKMH WHERE MSSV = @mssv AND MaLopHP = @maLopHP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    cmd.Parameters.Add("@maLopHP", SqlDbType.NVarChar, 30).Value = maLopHP.Trim();
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // 6. Kiểm tra xem lớp học phần đã đạt ngưỡng sĩ số trần hay chưa
        public bool IsSectionFull(string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT 
                        CASE 
                            WHEN (SELECT COUNT(1) FROM dbo.DKMH WHERE MaLopHP = @maLopHP) >= MaxStudents THEN 1 
                            ELSE 0 
                        END
                    FROM dbo.CourseSection
                    WHERE MaLopHP = @maLopHP";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maLopHP", SqlDbType.NVarChar, 30).Value = maLopHP.Trim();
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }

        // 7. Thực thi nghiệp vụ Đăng ký học phần (Thêm vào bảng DKMH)
        public bool AddRegistration(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "INSERT INTO dbo.DKMH (MSSV, MaLopHP, RegistrationDate) VALUES (@mssv, @maLopHP, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    cmd.Parameters.Add("@maLopHP", SqlDbType.NVarChar, 30).Value = maLopHP.Trim();
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 8. Thực thi nghiệp vụ Hủy đăng ký học phần
        public bool CancelRegistration(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM dbo.DKMH WHERE MSSV = @mssv AND MaLopHP = @maLopHP";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@mssv", SqlDbType.NVarChar, 30).Value = mssv.Trim();
                    cmd.Parameters.Add("@maLopHP", SqlDbType.NVarChar, 30).Value = maLopHP.Trim();
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}