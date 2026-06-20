using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    internal class RequestRepository
    {
        private readonly string _connectionString;

        public RequestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // CHỨC NĂNG CHO SINH VIÊN
        // 1. Sinh viên gửi request mới vào hệ thống
        public bool AddRequest(string mssv, string content)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Requests (MSSV, RequestContent) VALUES (@MSSV, @Content)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MSSV", mssv);
                cmd.Parameters.AddWithValue("@Content", content);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 2. Sinh viên xem lịch sử các request của chính mình
        public DataTable GetRequestsByStudent(string mssv)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, RequestContent, Status, AdminComment, Created_At FROM Requests WHERE MSSV = @MSSV ORDER BY Created_At DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MSSV", mssv);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. Search request của sinh viên theo từ khóa trong nội dung yêu cầu
        public DataTable SearchRequests(string mssv, string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Id, RequestContent, Status, AdminComment, Created_At 
                                 FROM Requests 
                                 WHERE MSSV = @MSSV AND RequestContent LIKE @Keyword 
                                 ORDER BY Created_At DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MSSV", mssv);
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // CHỨC NĂNG CHO ADMIN (ĐÃ ĐỒNG BỘ ĐỂ LẤY ĐỦ HỌ TÊN VÀ STATUS)
        // 1. Lấy toàn bộ danh sách các yêu cầu ĐANG CHỜ XỬ LÝ (Pending)
        public DataTable GetPendingRequests()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Thực hiện kết nối INNER JOIN sang bảng Students để lấy Họ Tên
                string query = @"SELECT r.Id, 
                                        r.MSSV, 
                                        (s.LastName + ' ' + s.FirstName) AS FullName, 
                                        r.RequestContent, 
                                        r.Status, 
                                        r.Created_At 
                                 FROM Requests r
                                 INNER JOIN Students s ON r.MSSV = s.MSSV
                                 WHERE r.Status = N'Pending' 
                                 ORDER BY r.Created_At ASC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }
            return dt;
        }

        // 2. Cập nhật trạng thái duyệt (Approved / Declined) kèm lời nhắn của Admin
        public bool UpdateRequestStatus(int requestId, string status, string comment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Requests 
                                 SET Status = @Status, AdminComment = @Comment, Updated_At = GETDATE() 
                                 WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Comment", string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment);
                cmd.Parameters.AddWithValue("@Id", requestId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 3. Tìm kiếm yêu cầu ở trạng thái Pending theo MSSV hoặc Họ Tên hoặc Nội dung (Dành cho Admin)
        public DataTable SearchPendingRequests(string keyword)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Hỗ trợ tìm kiếm thông minh theo cả Tên sinh viên và MSSV
                string query = @"SELECT r.Id, 
                                        r.MSSV, 
                                        (s.LastName + ' ' + s.FirstName) AS FullName, 
                                        r.RequestContent, 
                                        r.Status, 
                                        r.Created_At 
                                 FROM Requests r
                                 INNER JOIN Students s ON r.MSSV = s.MSSV
                                 WHERE r.Status = N'Pending' 
                                   AND (r.MSSV LIKE @Keyword OR s.FirstName LIKE @Keyword OR s.LastName LIKE @Keyword OR r.RequestContent LIKE @Keyword)
                                 ORDER BY r.Created_At ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}