using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ClassProject.Commons.DTOs;
using ClassProject.DataAccess.Db;

namespace ClassProject.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        // Khởi tạo đối tượng quản lý Database tập trung kế thừa từ App.config
        private readonly My_DB _db = new My_DB();

        public List<UserDTO> GetAllAccounts()
        {
            var list = new List<UserDTO>();
            string query = @"SELECT u.Id, u.Username, u.Email, u.RoleId, r.RoleName, 
                                    u.Valid, u.Status, u.FailedAttempts, u.LockoutEnd, u.LastLogin, u.Created_At
                             FROM dbo.Users u
                             INNER JOIN dbo.Roles r ON u.RoleId = r.Id
                             ORDER BY u.Created_At DESC";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserDTO
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            RoleId = reader.GetInt32(3),
                            RoleName = reader.IsDBNull(4) ? "Chưa phân quyền" : reader.GetString(4),
                            Valid = reader.GetInt32(5),
                            Status = reader.GetInt32(6),
                            FailedAttempts = reader.GetInt32(7),
                            LockoutEnd = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                            LastLogin = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9),
                            CreatedAt = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10)
                        });
                    }
                }
            }
            return list;
        }

        public DataTable GetRoles()
        {
            var dt = new DataTable();
            string query = "SELECT Id, RoleName FROM dbo.Roles";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public bool CreateAccount(string username, string email, string password, int roleId, int status)
        {
            string query = @"INSERT INTO dbo.Users (Username, Email, Password, RoleId, Status, Valid, Created_At) 
                             VALUES (@Username, @Email, @Password, @RoleId, @Status, 1, GETDATE())";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@Status", status);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateAccount(int id, string email, int roleId, int status, int valid)
        {
            string query = @"UPDATE dbo.Users 
                             SET Email = @Email, RoleId = @RoleId, Status = @Status, Valid = @Valid, Updated_At = GETDATE()
                             WHERE Id = @Id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Valid", valid);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ChangePassword(int id, string newHashPassword)
        {
            string query = "UPDATE dbo.Users SET Password = @Password, FailedAttempts = 0, LockoutEnd = NULL, Updated_At = GETDATE() WHERE Id = @Id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Password", newHashPassword ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteAccount(int id)
        {
            string query = "DELETE FROM dbo.Users WHERE Id = @Id";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool IsUsernameOrEmailExists(string username, string email, int? excludeId = null)
        {
            string query = "SELECT COUNT(1) FROM dbo.Users WHERE (Username = @Username OR Email = @Email)";
            if (excludeId.HasValue)
            {
                query += " AND Id <> @ExcludeId";
            }

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool UpdateSingleStatus(string username, int targetStatus, int? targetValid = null)
        {
            string query = "UPDATE dbo.Users SET Status = @Status";
            if (targetValid.HasValue) query += ", Valid = @Valid";
            query += " WHERE Username = @Username";

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Status", targetStatus);
                cmd.Parameters.AddWithValue("@Username", username ?? (object)DBNull.Value);
                if (targetValid.HasValue) cmd.Parameters.AddWithValue("@Valid", targetValid.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateBulkStatus(List<string> usernames, int targetStatus, int? targetValid = null)
        {
            if (usernames == null || usernames.Count == 0) return false;

            var sb = new System.Text.StringBuilder();
            int validVal = targetValid ?? ((targetStatus == 1) ? 1 : 0);

            // Tối ưu hóa: Tham số hóa cả Status và Valid thay vì nối chuỗi trực tiếp
            sb.Append("UPDATE dbo.Users SET Status = @TargetStatus");
            if (targetValid.HasValue || targetStatus == 1) sb.Append(", Valid = @TargetValid");
            sb.Append(" WHERE Username IN (");

            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand())
            {
                cmd.Parameters.AddWithValue("@TargetStatus", targetStatus);
                if (targetValid.HasValue || targetStatus == 1) cmd.Parameters.AddWithValue("@TargetValid", validVal);

                for (int i = 0; i < usernames.Count; i++)
                {
                    string paramName = "@u" + i;
                    sb.Append(paramName);
                    if (i < usernames.Count - 1) sb.Append(",");
                    cmd.Parameters.AddWithValue(paramName, usernames[i]);
                }
                sb.Append(")");

                cmd.CommandText = sb.ToString();
                cmd.Connection = conn;

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}