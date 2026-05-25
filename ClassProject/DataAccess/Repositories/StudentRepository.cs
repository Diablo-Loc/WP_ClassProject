using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace ClassProject.DataAccess.Repositories
{
    public class StudentRepository
    {
        private readonly string _connString;

        public StudentRepository(string connString)
        {
            _connString = connString;
        }

        // 1. Hàm lấy danh sách toàn bộ sinh viên
        public DataTable GetStudents()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT * FROM Students ORDER BY Id DESC";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        // 2. lấy hs theo mssv: Truy vấn dựa trên ép kiểu tương thích NVARCHAR của MSSV dưới DB
        public Student GetStudentByMssv(int mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                // Truy vấn dựa trên ép kiểu tương thích NVARCHAR của MSSV dưới DB
                string query = "SELECT UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture FROM Students WHERE CAST(MSSV AS INT) = @mssv";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mssv", mssv);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student(
                                reader["UserId"] != DBNull.Value && Convert.ToInt32(reader["UserId"]) != 0 ? Convert.ToInt32(reader["UserId"]) : 1,
                                Convert.ToInt32(reader["MSSV"]), // Chuyển đổi an toàn từ chuỗi sang số nguyên cho Model C#
                                reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : "",
                                reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : "",
                                reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : DateTime.Now,
                                reader["Gender"] != DBNull.Value ? reader["Gender"].ToString() : "Nam",
                                reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "",
                                reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                reader["Hometown"] != DBNull.Value ? reader["Hometown"].ToString() : null,
                                reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                                reader["Picture"] != DBNull.Value ? (byte[])reader["Picture"] : null
                            );
                        }
                    }
                }
            }
            return null; // Không tìm thấy
        }

        // 3. THÊM MỚI SINH VIÊN (Luồng 1: HR thêm danh sách trước, UserId tạm thời để NULL)
        public bool AddStudent(Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();

                    // Chỉ Insert vào bảng Students.
                    string sqlStudent = @"INSERT INTO Students 
                                       (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture)
                                   VALUES 
                                       (@UserId, @MSSV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, @Picture)";

                    using (SqlCommand cmd = new SqlCommand(sqlStudent, conn))
                    {
                        // CHÚ Ý CHỖ NÀY: Xử lý giá trị null ép về DBNull.Value cho SQL hiểu
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = (object)student.UserId ?? DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@MSSV", SqlDbType.NVarChar) { Value = student.Mssv.ToString() });
                        cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar) { Value = student.FirstName });
                        cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar) { Value = student.LastName });
                        cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = student.DateOfBirth });
                        cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar) { Value = student.Gender });
                        cmd.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar) { Value = student.Phone });
                        cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = student.Address ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Hometown", SqlDbType.NVarChar) { Value = student.Hometown ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = student.Email });
                        cmd.Parameters.Add(new SqlParameter("@Picture", SqlDbType.VarBinary) { Value = student.Picture ?? (object)DBNull.Value });

                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // 4. Hàm cập nhật thông tin: Nhận vào thực thể Student chứa dữ liệu mới
        public bool UpdateStudent(Student student)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string sql = @"UPDATE Students 
                                   SET FirstName=@fn, LastName=@ln, DateOfBirth=@db, Gender=@gr, 
                                       Phone=@ph, Address=@ad, Hometown=@ht, Email=@em, Picture=@pc 
                                   WHERE CAST(MSSV AS INT) = @mssv";

                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.Parameters.AddWithValue("@mssv", student.Mssv);
                        command.Parameters.AddWithValue("@fn", student.FirstName);
                        command.Parameters.AddWithValue("@ln", student.LastName);
                        command.Parameters.AddWithValue("@db", student.DateOfBirth);
                        command.Parameters.AddWithValue("@gr", student.Gender);
                        command.Parameters.AddWithValue("@ph", student.Phone);
                        command.Parameters.AddWithValue("@ad", student.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ht", student.Hometown ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@em", student.Email);
                        command.Parameters.AddWithValue("@pc", student.Picture ?? (object)DBNull.Value);

                        conn.Open();
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // 5. Hàm xóa sinh viên theo MSSV
        public bool DeleteStudent(int mssv)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = "DELETE FROM Students WHERE CAST(MSSV AS INT) = @id";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@id", mssv);
                        conn.Open();
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // 6. Hàm kiểm tra MSSV đã tồn tại chưa
        public bool IsMssvExist(int mssv)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT COUNT(*) FROM Students WHERE CAST(MSSV AS INT) = @mssv";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mssv", mssv);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // 7. Hàm tìm kiếm nâng cao kết hợp bộ lọc kép từ ComboBox
        public DataTable SearchStudents(string keyword, string genderFilter)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = @"SELECT UserId, MSSV as Mssv, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture 
                                 FROM Students 
                                 WHERE (MSSV LIKE @key 
                                 OR FirstName LIKE @key 
                                 OR LastName LIKE @key
                                 OR Email LIKE @key
                                 OR Phone LIKE @key)";

                if (!string.IsNullOrEmpty(genderFilter) && genderFilter != "Tất cả")
                {
                    query += " AND Gender = @gender";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@key", "%" + keyword.Trim() + "%");

                    if (!string.IsNullOrEmpty(genderFilter) && genderFilter != "Tất cả")
                    {
                        cmd.Parameters.AddWithValue("@gender", genderFilter);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
        // 8. THỐNG KÊ: Đếm tổng số sinh viên hiện có trong hệ thống
        public int GetTotalStudentsCount()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT COUNT(*) FROM dbo.Students";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // 9. THỐNG KÊ: Đếm tổng số sinh viên Nam (Dựa trên chữ N'Nam' hỗ trợ Unicode tiếng Việt)
        public int GetTotalMaleStudentsCount()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT COUNT(*) FROM dbo.Students WHERE Gender = N'Nam'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // 10. THỐNG KÊ: Đếm tổng số sinh viên Nữ (Dựa trên chữ N'Nữ' hỗ trợ Unicode tiếng Việt)
        public int GetTotalFemaleStudentsCount()
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                string query = "SELECT COUNT(*) FROM dbo.Students WHERE Gender = N'Nữ'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
        // 11. ĐĂNG KÝ MÔN HỌC (Đã sửa đổi đồng bộ biến _connString toàn cục)
        public bool RegisterCourse(string mssv, string courseId)
        {
            using (SqlConnection conn = new SqlConnection(_connString))
            {
                try
                {
                    conn.Open();

                    // 1. KIỂM TRA TRƯỚC
                    string checkSql = "SELECT COUNT(*) FROM CourseRegistration WHERE Mssv = @Mssv AND CourseId = @CourseId";
                    using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Mssv", mssv);
                        checkCmd.Parameters.AddWithValue("@CourseId", courseId);

                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0) return false;
                    }

                    // 2. TIẾN HÀNH CHÈN DỮ LIỆU
                    string insertSql = @"INSERT INTO CourseRegistration (Mssv, CourseId, RegistrationDate) 
                                         VALUES (@Mssv, @CourseId, @RegistrationDate)";

                    using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Mssv", mssv);
                        insertCmd.Parameters.AddWithValue("@CourseId", courseId);
                        insertCmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);

                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}