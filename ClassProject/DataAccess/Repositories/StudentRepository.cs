using ClassProject.DataAccess.Db;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class StudentRepository
    {
        private readonly string _connString;
        private readonly My_DB _db = new My_DB();

        public StudentRepository()
        {
        }

        public DataTable GetStudents()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT s.UserId, s.MSSV, s.FirstName, s.LastName, s.DateOfBirth, 
                                        s.Gender, s.Phone, s.Address, s.Hometown, s.Email, s.Picture,
                                        s.MaLop, s.MaNganh,
                                        ISNULL(c.TenLop, N'Chưa xếp lớp') AS TenLop, 
                                        ISNULL(m.TenNganh, N'Chưa phân ngành') AS TenNganh 
                                 FROM dbo.Students s
                                 LEFT JOIN dbo.Classroom c ON s.MaLop = c.MaLop
                                 LEFT JOIN dbo.Major m ON s.MaNganh = m.MaNganh
                                 ORDER BY s.Id DESC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public Student GetStudentByMssv(string mssv)
        {
            if (string.IsNullOrWhiteSpace(mssv)) return null;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT s.UserId, s.MSSV, s.FirstName, s.LastName, s.DateOfBirth, 
                                        s.Gender, s.Phone, s.Address, s.Hometown, s.Email, s.Picture, 
                                        s.MaLop, s.MaNganh,
                                        c.TenLop, m.TenNganh 
                                 FROM dbo.Students s
                                 LEFT JOIN dbo.Classroom c ON s.MaLop = c.MaLop
                                 LEFT JOIN dbo.Major m ON s.MaNganh = m.MaNganh 
                                 WHERE s.MSSV = @mssv";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null,
                                Mssv = reader["MSSV"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                                Gender = reader["Gender"]?.ToString() ?? "",
                                Phone = reader["Phone"]?.ToString() ?? "",
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                                Hometown = reader["Hometown"] != DBNull.Value ? reader["Hometown"].ToString() : null,
                                Email = reader["Email"]?.ToString() ?? "",
                                Picture = reader["Picture"] != DBNull.Value ? reader["Picture"] as byte[] : null,
                                MaLop = reader["MaLop"] != DBNull.Value ? reader["MaLop"].ToString() : null,
                                MaNganh = reader["MaNganh"] != DBNull.Value ? reader["MaNganh"].ToString() : null,
                                TenLop = reader["TenLop"] != DBNull.Value ? reader["TenLop"].ToString() : "Chưa xếp lớp",
                                TenNganh = reader["TenNganh"] != DBNull.Value ? reader["TenNganh"].ToString() : "Chưa phân ngành"
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool AddStudent(Student student)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string sqlStudent = @"INSERT INTO dbo.Students 
                                       (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture, MaLop, MaNganh)
                                     VALUES 
                                       (@UserId, @MSSV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, @Picture, @MaLop, @MaNganh);";

                using (SqlCommand cmd = new SqlCommand(sqlStudent, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = (object)student.UserId ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@MSSV", SqlDbType.NVarChar, 30) { Value = student.Mssv?.Trim() });
                    cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = student.FirstName?.Trim() });
                    cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = student.LastName?.Trim() });
                    cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = (object)student.DateOfBirth ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) { Value = student.Gender?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 15) { Value = student.Phone?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 200) { Value = student.Address?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Hometown", SqlDbType.NVarChar, 100) { Value = student.Hometown?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = student.Email?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Picture", SqlDbType.VarBinary) { Value = student.Picture ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@MaLop", SqlDbType.VarChar, 20) { Value = student.MaLop?.Trim() ?? (object)DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@MaNganh", SqlDbType.Char, 10) { Value = student.MaNganh?.Trim() ?? (object)DBNull.Value });

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            if (ex.Message.Contains("Email") || ex.Message.Contains("UQ_Students_Email"))
                            {
                                throw new InvalidOperationException("Địa chỉ Email này đã tồn tại trong hệ thống sinh viên!");
                            }
                            throw new InvalidOperationException("Mã số sinh viên (MSSV) này đã được sử dụng!");
                        }
                        throw;
                    }
                }
            }
        }

        public bool UpdateStudent(Student student)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string sql = @"UPDATE dbo.Students 
                               SET FirstName=@fn, LastName=@ln, DateOfBirth=@db, Gender=@gr, 
                                   Phone=@ph, Address=@ad, Hometown=@ht, Email=@em, 
                                   Picture = ISNULL(@pc, Picture),
                                   MaLop=@ml, MaNganh=@mn
                               WHERE MSSV = @mssv";

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = student.Mssv?.Trim() });
                    command.Parameters.Add(new SqlParameter("@fn", SqlDbType.NVarChar, 100) { Value = student.FirstName?.Trim() });
                    command.Parameters.Add(new SqlParameter("@ln", SqlDbType.NVarChar, 100) { Value = student.LastName?.Trim() });
                    command.Parameters.Add(new SqlParameter("@db", SqlDbType.DateTime) { Value = (object)student.DateOfBirth ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@gr", SqlDbType.NVarChar, 10) { Value = student.Gender?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ph", SqlDbType.NVarChar, 15) { Value = student.Phone?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ad", SqlDbType.NVarChar, 200) { Value = student.Address?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ht", SqlDbType.NVarChar, 100) { Value = student.Hometown?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@em", SqlDbType.NVarChar, 100) { Value = student.Email?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@pc", SqlDbType.VarBinary) { Value = student.Picture ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@ml", SqlDbType.VarChar, 20) { Value = student.MaLop?.Trim() ?? (object)DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@mn", SqlDbType.Char, 10) { Value = student.MaNganh?.Trim() ?? (object)DBNull.Value });

                    try
                    {
                        conn.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            throw new InvalidOperationException("Email cập nhật đã trùng với một sinh viên khác trong hệ thống!");
                        }
                        throw;
                    }
                }
            }
        }

        public bool DeleteStudent(string mssv)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "DELETE FROM dbo.Students WHERE MSSV = @id";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 30) { Value = mssv });
                    conn.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsMssvExist(string mssv)
        {
            if (string.IsNullOrWhiteSpace(mssv)) return false;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM dbo.Students WHERE MSSV = @mssv";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public DataTable SearchStudents(string keyword, string genderFilter)
        {
            keyword = keyword?.Trim() ?? "";
            DataTable dt = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                // Giữ nguyên logic so khớp không dấu thông minh của bạn
                string query = @"SELECT s.UserId, s.MSSV, s.FirstName, s.LastName, s.DateOfBirth, 
                                        s.Gender, s.Phone, s.Address, s.Hometown, s.Email, s.Picture,
                                        s.MaLop, s.MaNganh,
                                        ISNULL(c.TenLop, N'Chưa xếp lớp') AS TenLop, 
                                        ISNULL(m.TenNganh, N'Chưa phân ngành') AS TenNganh 
                                 FROM dbo.Students s
                                 LEFT JOIN dbo.Classroom c ON s.MaLop = c.MaLop
                                 LEFT JOIN dbo.Major m ON s.MaNganh = m.MaNganh
                                 WHERE (s.MSSV COLLATE Latin1_General_CI_AI LIKE @key 
                                 OR s.FirstName COLLATE Latin1_General_CI_AI LIKE @key 
                                 OR s.LastName COLLATE Latin1_General_CI_AI LIKE @key
                                 OR s.Email COLLATE Latin1_General_CI_AI LIKE @key
                                 OR s.Phone COLLATE Latin1_General_CI_AI LIKE @key)";

                if (!string.IsNullOrEmpty(genderFilter) && genderFilter != "Tất cả")
                {
                    query += " AND s.Gender = @gender";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@key", SqlDbType.NVarChar) { Value = "%" + keyword + "%" });

                    if (!string.IsNullOrEmpty(genderFilter) && genderFilter != "Tất cả")
                    {
                        cmd.Parameters.Add(new SqlParameter("@gender", SqlDbType.NVarChar, 10) { Value = genderFilter });
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

        public int GetTotalStudentsCount() => ExecuteScalarCount("SELECT COUNT(*) FROM dbo.Students");
        public int GetTotalMaleStudentsCount() => ExecuteScalarCount("SELECT COUNT(*) FROM dbo.Students WHERE Gender = N'Nam'");
        public int GetTotalFemaleStudentsCount() => ExecuteScalarCount("SELECT COUNT(*) FROM dbo.Students WHERE Gender = N'Nữ'");

        private int ExecuteScalarCount(string query)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool RegisterCourse(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                string checkSql = "SELECT COUNT(*) FROM dbo.DKMH WHERE MSSV = @Mssv AND MaLopHP = @MaLopHP";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.Add(new SqlParameter("@Mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });

                    // ĐỒNG BỘ: Sửa thành VarChar(30) cho khớp chính xác với bảng DKMH vật lý trong SQL
                    checkCmd.Parameters.Add(new SqlParameter("@MaLopHP", SqlDbType.VarChar, 30) { Value = maLopHP.Trim() });

                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        return false;
                    }
                }

                string insertSql = @"INSERT INTO dbo.DKMH (MSSV, MaLopHP, RegistrationDate) 
                                     VALUES (@Mssv, @MaLopHP, GETDATE())";

                using (SqlCommand insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.Add(new SqlParameter("@Mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });

                    insertCmd.Parameters.Add(new SqlParameter("@MaLopHP", SqlDbType.VarChar, 30) { Value = maLopHP.Trim() });

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public bool ImportStudentWithAccount(string username, string hashedPassword, Student student)
        {
            // Lấy chuỗi kết nối từ cấu trúc cơ sở dữ liệu hiện tại của dự án của bạn
            // Giả định _db.GetConnection() trả về đối tượng SqlConnection
            using (SqlConnection conn = (SqlConnection)_db.GetConnection())
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                // Khởi tạo Transaction bảo vệ tính toàn vẹn dữ liệu
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // BƯỚC 1: Chèn tài khoản mới vào bảng Users với RoleId = 1 (Sinh viên)
                        // Sử dụng SELECT SCOPE_IDENTITY() để lấy ra ID tự tăng vừa tạo ngay lập tức
                        string userQuery = @"
                    INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status, Created_At)
                    VALUES (@Username, @Email, @Password, 1, 1, 1, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                        int newUserId = 0;
                        using (SqlCommand cmdUser = new SqlCommand(userQuery, conn, trans))
                        {
                            cmdUser.Parameters.AddWithValue("@Username", username);
                            cmdUser.Parameters.AddWithValue("@Email", student.Email);
                            cmdUser.Parameters.AddWithValue("@Password", hashedPassword);

                            newUserId = Convert.ToInt32(cmdUser.ExecuteScalar());
                        }

                        // BƯỚC 2: Chèn thông tin vào bảng Students với UserId vừa lấy được từ bảng Users
                        string studentQuery = @"
                    INSERT INTO dbo.Students (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Created_At)
                    VALUES (@UserId, @MSSV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, GETDATE());";

                        using (SqlCommand cmdStudent = new SqlCommand(studentQuery, conn, trans))
                        {
                            cmdStudent.Parameters.AddWithValue("@UserId", newUserId);
                            cmdStudent.Parameters.AddWithValue("@MSSV", student.Mssv);
                            cmdStudent.Parameters.AddWithValue("@FirstName", student.FirstName);
                            cmdStudent.Parameters.AddWithValue("@LastName", student.LastName);

                            // Xử lý kiểm tra dữ liệu ngày tháng rỗng an toàn
                            if (student.DateOfBirth.HasValue)
                                cmdStudent.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth.Value);
                            else
                                cmdStudent.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);

                            cmdStudent.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(student.Gender) ? DBNull.Value : (object)student.Gender);
                            cmdStudent.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(student.Phone) ? DBNull.Value : (object)student.Phone);
                            cmdStudent.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(student.Address) ? DBNull.Value : (object)student.Address);
                            cmdStudent.Parameters.AddWithValue("@Hometown", string.IsNullOrEmpty(student.Hometown) ? DBNull.Value : (object)student.Hometown);
                            cmdStudent.Parameters.AddWithValue("@Email", student.Email);

                            cmdStudent.ExecuteNonQuery();
                        }

                        // BƯỚC 3: Nếu cả 2 bước chạy không lỗi, xác nhận lưu dữ liệu vĩnh viễn vào SQL Server
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Có lỗi phát sinh (Trùng Email hệ thống, sai kiểu dữ liệu...), hủy bỏ toàn bộ thao tác của dòng này
                        trans.Rollback();
                        System.Diagnostics.Debug.WriteLine("Lỗi khi thực thi ImportTransaction: " + ex.Message);
                        return false;
                    }
                }
            }
        }
    }
}