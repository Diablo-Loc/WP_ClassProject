using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class ClassRoomRepository
    {
        private readonly string _connString;

        public ClassRoomRepository(string connString)
        {
            _connString = connString;
        }

        // 1. Hàm lấy danh sách lớp (Câu 1)
        public DataTable GetClassrooms()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT MaLop, TenLop, SiSo, GVCN FROM Classroom ORDER BY MaLop ASC";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        // 2. Hàm thêm mới lớp học - Nhận vào thực thể Classroom (Câu 1)
        public bool AddClassroom(Classroom classroom)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    string query = @"INSERT INTO Classroom (MaLop, TenLop, SiSo, GVCN) 
                                     VALUES (@ma, @ten, @siso, @gvcn)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", classroom.MaLop);
                        cmd.Parameters.AddWithValue("@ten", classroom.TenLop);
                        cmd.Parameters.AddWithValue("@siso", classroom.SiSo);
                        cmd.Parameters.AddWithValue("@gvcn", string.IsNullOrEmpty(classroom.GVCN) ? (object)DBNull.Value : classroom.GVCN);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 3. Hàm tìm kiếm theo tên lớp - Đồng bộ dùng _connString (Câu 2)
        public DataTable SearchByTenLop(string tenLop)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT MaLop, TenLop, SiSo, GVCN FROM Classroom WHERE TenLop LIKE @ten";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ten", "%" + tenLop.Trim() + "%");
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        // 4. Hàm cập nhật thông tin lớp - Nhận vào thực thể Classroom (Câu 3)
        public bool UpdateClassroom(Classroom classroom)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    string query = "UPDATE Classroom SET TenLop = @ten, SiSo = @siso, GVCN = @gvcn WHERE MaLop = @ma";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", classroom.MaLop);
                        cmd.Parameters.AddWithValue("@ten", classroom.TenLop);
                        cmd.Parameters.AddWithValue("@siso", classroom.SiSo);
                        cmd.Parameters.AddWithValue("@gvcn", string.IsNullOrEmpty(classroom.GVCN) ? (object)DBNull.Value : classroom.GVCN);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }

        // 5. Hàm xóa lớp học theo Mã lớp (Câu 3)
        public bool DeleteClassroom(string maLop)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    string query = "DELETE FROM Classroom WHERE MaLop = @ma";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", maLop);
                        
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch { return false; }
            }
        }
    }
}