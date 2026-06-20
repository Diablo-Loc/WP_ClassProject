using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms; // Đảm bảo có thư viện này để dùng MessageBox nếu cần, hoặc bỏ đi nếu xử lý ở tầng UI

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class StudentRepository
    {
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

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        // Đảm bảo mở kết nối tường minh
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        public bool AddStudent(Student student)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    string sqlStudent = @"INSERT INTO Students 
                                       (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture, MaLop, MaNganh)
                                   VALUES 
                                       (@UserId, @MSSV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, @Picture, @MaLop, @MaNganh)";

                    using (SqlCommand cmd = new SqlCommand(sqlStudent, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = (object)student.UserId ?? DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@MSSV", SqlDbType.NVarChar) { Value = student.Mssv.ToString() });
                        cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar) { Value = student.FirstName });
                        cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar) { Value = student.LastName });
                        cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = (object)student.DateOfBirth ?? DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar) { Value = student.Gender ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar) { Value = student.Phone ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = student.Address ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Hometown", SqlDbType.NVarChar) { Value = student.Hometown ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = student.Email });
                        cmd.Parameters.Add(new SqlParameter("@Picture", SqlDbType.VarBinary) { Value = student.Picture ?? (object)DBNull.Value });
                        cmd.Parameters.Add(new SqlParameter("@MaLop", SqlDbType.VarChar, 20) { Value = string.IsNullOrEmpty(student.MaLop) ? DBNull.Value : (object)student.MaLop.Trim() });
                        cmd.Parameters.Add(new SqlParameter("@MaNganh", SqlDbType.Char, 10) { Value = string.IsNullOrEmpty(student.MaNganh) ? DBNull.Value : (object)student.MaNganh.Trim() });

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

                    if (conn.State == ConnectionState.Closed) conn.Open();
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

        public bool AddStudentWithAccount(Student student, string username, string hashedPassword, int roleId)
        {
            // Kiểm tra trùng Email diện rộng trước khi thực hiện hành động
            if (IsEmailExists(student.Email, null))
            {
                throw new InvalidOperationException("Địa chỉ Email này đã tồn tại trên hệ thống (bảng Users hoặc Students)!");
            }

            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlUser = @"INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status, Created_At)
                                           VALUES (@Username, @Email, @Password, @RoleId, 1, 1, GETDATE());
                                           SELECT SCOPE_IDENTITY();";

                        int newUserId = 0;
                        using (SqlCommand cmdUser = new SqlCommand(sqlUser, conn, trans))
                        {
                            cmdUser.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username.Trim() });
                            cmdUser.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = student.Email.Trim() });
                            cmdUser.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = hashedPassword });
                            cmdUser.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });

                            newUserId = Convert.ToInt32(cmdUser.ExecuteScalar());
                        }

                        string sqlStudent = @"INSERT INTO dbo.Students 
                                               (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture, MaLop, MaNganh, Created_At)
                                             VALUES 
                                               (@UserId, @MSSV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, @Picture, @MaLop, @MaNganh, GETDATE());";

                        using (SqlCommand cmdStudent = new SqlCommand(sqlStudent, conn, trans))
                        {
                            cmdStudent.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = newUserId });
                            cmdStudent.Parameters.Add(new SqlParameter("@MSSV", SqlDbType.NVarChar, 30) { Value = student.Mssv.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = student.FirstName.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = student.LastName.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = (object)student.DateOfBirth ?? DBNull.Value });
                            cmdStudent.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) { Value = string.IsNullOrEmpty(student.Gender) ? DBNull.Value : (object)student.Gender.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 15) { Value = string.IsNullOrEmpty(student.Phone) ? DBNull.Value : (object)student.Phone.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 200) { Value = string.IsNullOrEmpty(student.Address) ? DBNull.Value : (object)student.Address.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@Hometown", SqlDbType.NVarChar, 100) { Value = string.IsNullOrEmpty(student.Hometown) ? DBNull.Value : (object)student.Hometown.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = student.Email.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@Picture", SqlDbType.VarBinary) { Value = student.Picture ?? (object)DBNull.Value });
                            cmdStudent.Parameters.Add(new SqlParameter("@MaLop", SqlDbType.VarChar, 20) { Value = string.IsNullOrEmpty(student.MaLop) ? DBNull.Value : (object)student.MaLop.Trim() });
                            cmdStudent.Parameters.Add(new SqlParameter("@MaNganh", SqlDbType.Char, 10) { Value = string.IsNullOrEmpty(student.MaNganh) ? DBNull.Value : (object)student.MaNganh.Trim() });

                            cmdStudent.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            if (ex.Message.Contains("Username"))
                                throw new InvalidOperationException("Tên tài khoản (Username) tự động sinh ra đã trùng lặp!");
                            if (ex.Message.Contains("Email"))
                                throw new InvalidOperationException("Địa chỉ Email hệ thống này đã tồn tại!");
                            throw new InvalidOperationException("Mã số sinh viên (MSSV) này đã được sử dụng!");
                        }
                        throw;
                    }
                }
            }
        }

        public bool UpdateStudent(Student student)
        {
            // Kiểm tra trùng Email trước khi Update
            if (IsEmailExists(student.Email, student.Mssv))
            {
                throw new InvalidOperationException("Email cập nhật đã trùng với một tài khoản/sinh viên khác trong hệ thống!");
            }

            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"UPDATE dbo.Students 
                                       SET FirstName=@fn, LastName=@ln, DateOfBirth=@db, Gender=@gr, 
                                           Phone=@ph, Address=@ad, Hometown=@ht, Email=@em, 
                                           Picture = ISNULL(@pc, Picture),
                                           MaLop=@ml, MaNganh=@mn, Updated_At=GETDATE()
                                       WHERE MSSV = @mssv";

                        using (SqlCommand command = new SqlCommand(sql, conn, trans))
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

                            command.ExecuteNonQuery();
                        }

                        if (student.UserId.HasValue && student.UserId.Value > 0)
                        {
                            string sqlUserUpdate = "UPDATE dbo.Users SET Email = @Email, Updated_At = GETDATE() WHERE Id = @UserId";
                            using (SqlCommand cmdUser = new SqlCommand(sqlUserUpdate, conn, trans))
                            {
                                cmdUser.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = student.Email.Trim() });
                                cmdUser.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = student.UserId.Value });
                                cmdUser.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteStudent(string mssv)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int? userId = null;
                        string findUserQuery = "SELECT UserId FROM dbo.Students WHERE MSSV = @mssv";
                        using (SqlCommand cmdFind = new SqlCommand(findUserQuery, conn, trans))
                        {
                            cmdFind.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = mssv });
                            var res = cmdFind.ExecuteScalar();
                            if (res != null && res != DBNull.Value) userId = Convert.ToInt32(res);
                        }

                        string query = "DELETE FROM dbo.Students WHERE MSSV = @id";
                        using (SqlCommand command = new SqlCommand(query, conn, trans))
                        {
                            command.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 30) { Value = mssv });
                            command.ExecuteNonQuery();
                        }

                        if (userId.HasValue)
                        {
                            string deleteUser = "DELETE FROM dbo.Users WHERE Id = @UserId";
                            using (SqlCommand cmdUser = new SqlCommand(deleteUser, conn, trans))
                            {
                                cmdUser.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId.Value });
                                cmdUser.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
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
                    if (conn.State == ConnectionState.Closed) conn.Open();
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
                        if (conn.State == ConnectionState.Closed) conn.Open();
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
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool RegisterCourse(string mssv, string maLopHP)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string checkSql = "SELECT COUNT(*) FROM dbo.DKMH WHERE MSSV = @Mssv AND MaLopHP = @MaLopHP";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.Add(new SqlParameter("@Mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });
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
            try
            {
                return AddStudentWithAccount(student, username, hashedPassword, 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi thực thi ImportTransaction: " + ex.Message);
                return false;
            }
        }

        public bool CreateAccountForStudent(string mssv, string username, string hashedPassword)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string email = "";
                        string sqlGetEmail = "SELECT Email FROM dbo.Students WHERE MSSV = @mssv";
                        using (SqlCommand cmdGet = new SqlCommand(sqlGetEmail, conn, trans))
                        {
                            cmdGet.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });
                            var res = cmdGet.ExecuteScalar();
                            if (res != null && res != DBNull.Value) email = res.ToString().Trim();
                        }

                        // Kiểm tra xem Email của sinh viên này có bị trùng bên bảng Users chưa trước khi gán tài khoản
                        string sqlCheckUserEmail = "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email";
                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheckUserEmail, conn, trans))
                        {
                            cmdCheck.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
                            if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                            {
                                throw new InvalidOperationException("Không thể tạo tài khoản! Email của sinh viên này đã được liên kết với một tài khoản khác.");
                            }
                        }

                        string sqlUser = @"INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status, Created_At)
                                   VALUES (@Username, @Email, @Password, 1, 1, 1, GETDATE());
                                   SELECT SCOPE_IDENTITY();";

                        int newUserId = 0;
                        using (SqlCommand cmdUser = new SqlCommand(sqlUser, conn, trans))
                        {
                            cmdUser.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username.Trim() });
                            cmdUser.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
                            cmdUser.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = hashedPassword });

                            newUserId = Convert.ToInt32(cmdUser.ExecuteScalar());
                        }

                        string sqlUpdateStudent = "UPDATE dbo.Students SET UserId = @UserId, Updated_At = GETDATE() WHERE MSSV = @mssv";
                        using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdateStudent, conn, trans))
                        {
                            cmdUpdate.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = newUserId });
                            cmdUpdate.Parameters.Add(new SqlParameter("@mssv", SqlDbType.NVarChar, 30) { Value = mssv.Trim() });

                            cmdUpdate.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message, "Lỗi tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
        }

        public DataRow GetAccountInfoById(int userId)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = @"SELECT s.UserId, s.MSSV, u.Username, s.Email, 
                                            (s.LastName + ' ' + s.FirstName) AS FullName
                                     FROM dbo.Students s
                                     INNER JOIN dbo.Users u ON s.UserId = u.Id
                                     WHERE s.UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            da.Fill(dt);

                            if (dt.Rows.Count > 0) return dt.Rows[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi GetAccountInfoById (Student): " + ex.Message);
            }
            return null;
        }

        public bool ResetPassword(int userId, string newPasswordHash, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = @"UPDATE dbo.Users 
                                     SET Password = @NewPasswordHash, Updated_At = GETDATE() 
                                     WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
                        cmd.Parameters.Add(new SqlParameter("@NewPasswordHash", SqlDbType.NVarChar, 255) { Value = newPasswordHash });

                        if (conn.State == ConnectionState.Closed) conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        // ĐÃ FIX: Hàm kiểm tra Email diện rộng quét qua cả hai bảng để chống trùng triệt để
        public bool IsEmailExists(string email, string currentMssv)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // 1. Quét trùng trong bảng Students (bỏ qua sinh viên hiện tại nếu đang edit)
                    string queryStudent = "SELECT COUNT(1) FROM dbo.Students WHERE Email = @Email";
                    if (!string.IsNullOrEmpty(currentMssv))
                    {
                        queryStudent += " AND MSSV != @CurrentMssv";
                    }

                    using (SqlCommand cmd = new SqlCommand(queryStudent, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email.Trim());
                        if (!string.IsNullOrEmpty(currentMssv))
                        {
                            cmd.Parameters.AddWithValue("@CurrentMssv", currentMssv.Trim());
                        }

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) return true;
                    }

                    // 2. Quét trùng luôn trong bảng Users của toàn hệ thống
                    string queryUser = "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email";
                    // Nếu là hành động update sinh viên cũ (đã có tài khoản), loại bỏ chính tài khoản đó ra khỏi danh sách check trùng
                    if (!string.IsNullOrEmpty(currentMssv))
                    {
                        queryUser += " AND Id != (SELECT ISNULL(UserId, 0) FROM dbo.Students WHERE MSSV = @CurrentMssv)";
                    }

                    using (SqlCommand cmdUser = new SqlCommand(queryUser, conn))
                    {
                        cmdUser.Parameters.AddWithValue("@Email", email.Trim());
                        if (!string.IsNullOrEmpty(currentMssv))
                        {
                            cmdUser.Parameters.AddWithValue("@CurrentMssv", currentMssv.Trim());
                        }

                        if (Convert.ToInt32(cmdUser.ExecuteScalar()) > 0) return true;
                    }
                }
            }
            catch { return false; }

            return false;
        }
    }
}