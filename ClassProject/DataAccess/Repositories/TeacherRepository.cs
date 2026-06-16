using ClassProject.DataAccess.Db;
using ClassProject.Models;
using Microsoft.Data.SqlClient; 
using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly My_DB _db = new My_DB();

        public DataTable GetAllTeachers()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Id, UserId, MSGV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, AcademicRank, Status,
                            (FirstName + ' ' + LastName) AS FullName,
                            CASE WHEN Status = 1 THEN N'Đang công tác' ELSE N'Nghỉ việc / Đình chỉ' END AS StatusText
                            FROM dbo.Teachers ORDER BY MSGV ASC";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi truy vấn danh sách giảng viên: " + ex.Message);
            }
            return dt;
        }

        public DataTable SearchTeachers(string keyword)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Id, UserId, MSGV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, AcademicRank, Status,
                            (FirstName + ' ' + LastName) AS FullName,
                            CASE WHEN Status = 1 THEN N'Đang công tác' ELSE N'Nghỉ việc / Đình chỉ' END AS StatusText
                            FROM dbo.Teachers 
                            WHERE MSGV LIKE @Key OR FirstName LIKE @Key OR LastName LIKE @Key OR Phone LIKE @Key OR Email LIKE @Key
                            ORDER BY MSGV ASC";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Key", "%" + keyword.Trim() + "%");
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tìm kiếm giảng viên: " + ex.Message);
            }
            return dt;
        }

        public bool InsertTeacher(int? userId, string msgv, string firstName, string lastName, DateTime? dateOfBirth, string gender, string phone, string email, string academicRank)
        {
            string query = @"INSERT INTO dbo.Teachers (UserId, MSGV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, AcademicRank, Status, Created_At)
                            VALUES (@UserId, @MSGV, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Email, @AcademicRank, 1, GETDATE())";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MSGV", msgv.Trim());
                        cmd.Parameters.AddWithValue("@FirstName", firstName.Trim());
                        cmd.Parameters.AddWithValue("@LastName", lastName.Trim());
                        cmd.Parameters.AddWithValue("@DateOfBirth", (object)dateOfBirth ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicRank", (object)academicRank ?? DBNull.Value);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateTeacher(int id, string firstName, string lastName, DateTime? dateOfBirth, string gender, string phone, string email, string academicRank, int status)
        {
            string query = @"UPDATE dbo.Teachers 
                            SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                                Gender = @Gender, Phone = @Phone, Email = @Email, AcademicRank = @AcademicRank, 
                                Status = @Status, Updated_At = GETDATE()
                            WHERE Id = @Id";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@FirstName", firstName.Trim());
                        cmd.Parameters.AddWithValue("@LastName", lastName.Trim());
                        cmd.Parameters.AddWithValue("@DateOfBirth", (object)dateOfBirth ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AcademicRank", (object)academicRank ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", status);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTeacher(int id)
        {
            string query = "DELETE FROM dbo.Teachers WHERE Id = @Id";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsDuplicateCheck(string msgv, string phone, string email, int? excludeId = null)
        {
            // Kiểm tra trùng Mã số, hoặc trùng SĐT (nếu gõ), hoặc trùng Email (nếu gõ)
            string query = @"SELECT COUNT(1) FROM dbo.Teachers 
                            WHERE (MSGV = @MSGV 
                                   OR (@Phone <> '' AND Phone = @Phone) 
                                   OR (@Email <> '' AND Email = @Email))";

            if (excludeId.HasValue)
            {
                query += " AND Id <> @ExcludeId";
            }

            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MSGV", msgv.Trim());
                        cmd.Parameters.AddWithValue("@Phone", phone ?? "");
                        cmd.Parameters.AddWithValue("@Email", email ?? "");
                        if (excludeId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                        }

                        conn.Open();
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public Teacher GetTeacherByUserId(int userId)
        {
            // Truy vấn chuẩn xác theo các cột hiện có trong schema của bạn
            string query = "SELECT Id, UserId, MSGV, FirstName, LastName, Phone, Email, AcademicRank FROM dbo.Teachers WHERE UserId = @UserId";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Teacher
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : (int?)null,
                                    MSGV = reader["MSGV"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Phone = reader["Phone"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    AcademicRank = reader["AcademicRank"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi GetTeacherByUserId: " + ex.Message);
            }
            return null;
        }
        public bool UpdateTeacherProfile(Teacher teacher)
        {
            string query = @"UPDATE dbo.Teachers 
                            SET FirstName = @FirstName, LastName = @LastName, 
                                Phone = @Phone, Email = @Email, Updated_At = GETDATE()
                            WHERE Id = @Id";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", teacher.Id);
                        cmd.Parameters.AddWithValue("@FirstName", teacher.FirstName.Trim());
                        cmd.Parameters.AddWithValue("@LastName", teacher.LastName.Trim());
                        cmd.Parameters.AddWithValue("@Phone", (object)teacher.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)teacher.Email ?? DBNull.Value);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi UpdateTeacherProfile: " + ex.Message);
                return false;
            }
        }
        public bool InsertTeacherProfile(Teacher teacher)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                const string query = @"
            INSERT INTO dbo.Teachers (UserId, MSGV, FirstName, LastName, Email, Status, Created_At)
            VALUES (@UserId, @MSGV, @FirstName, @LastName, @Email, @Status, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", teacher.UserId);
                    cmd.Parameters.AddWithValue("@MSGV", teacher.MSGV);
                    cmd.Parameters.AddWithValue("@FirstName", teacher.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", teacher.LastName);
                    cmd.Parameters.AddWithValue("@Email", teacher.Email);
                    cmd.Parameters.AddWithValue("@Status", teacher.Status);

                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}