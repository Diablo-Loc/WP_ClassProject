using Microsoft.Data.SqlClient;
using System;
using System.Data;
using ClassProject.DataAccess.Db; // Khai báo namespace chứa lớp My_DB của bạn
using ClassProject.DataAccess.Models;

namespace ClassProject.DataAccess.Repositories
{
    public class MajorRepository
    {
        private readonly My_DB _db = new My_DB();

        /// <summary>
        /// 1. Lấy toàn bộ danh sách ngành học hoặc tìm kiếm theo từ khóa
        /// </summary>
        public DataTable GetMajors(string keyword = "")
        {
            DataTable table = new DataTable();

            // Gọi trực tiếp hàm GetConnection() từ lớp My_DB của bạn để khởi tạo kết nối
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY Created_At DESC) AS STT,
                        MaNganh AS [Mã Ngành], 
                        TenNganh AS [Tên Ngành Học], 
                        Created_At AS [Ngày Tạo], 
                        Updated_At AS [Ngày Cập Nhật]
                    FROM dbo.Major
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " AND (MaNganh LIKE @Keyword OR TenNganh LIKE @Keyword)";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = "%" + keyword.Trim() + "%";
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// 2. Kiểm tra sự tồn tại của Mã ngành trong hệ thống
        /// </summary>
        public bool IsMaNganhExists(string maNganh)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM dbo.Major WHERE MaNganh = @MaNganh";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaNganh", SqlDbType.Char, 10).Value = maNganh.Trim().ToUpper();
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// 3. Thêm mới một ngành học vào cơ sở dữ liệu
        /// </summary>
        public bool Insert(MajorModel major)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO dbo.Major (MaNganh, TenNganh, Created_At) 
                                 VALUES (@MaNganh, @TenNganh, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaNganh", SqlDbType.Char, 10).Value = major.MaNganh.Trim().ToUpper();
                    cmd.Parameters.Add("@TenNganh", SqlDbType.NVarChar, 100).Value = major.TenNganh.Trim();

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// 4. Cập nhật tên ngành học dựa trên mã ngành cố định
        /// </summary>
        public bool Update(MajorModel major)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"UPDATE dbo.Major 
                                 SET TenNganh = @TenNganh, Updated_At = GETDATE() 
                                 WHERE MaNganh = @MaNganh";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaNganh", SqlDbType.Char, 10).Value = major.MaNganh.Trim().ToUpper();
                    cmd.Parameters.Add("@TenNganh", SqlDbType.NVarChar, 100).Value = major.TenNganh.Trim();

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// 5. Xóa một ngành học khỏi hệ thống
        /// </summary>
        public bool Delete(string maNganh)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM dbo.Major WHERE MaNganh = @MaNganh";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaNganh", SqlDbType.Char, 10).Value = maNganh.Trim().ToUpper();

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}