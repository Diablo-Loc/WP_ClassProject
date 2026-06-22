using System;
using System.Data;
using System.Threading.Tasks;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class ContactRepository : IContactRepository
    {
        private readonly My_DB _db = new My_DB();

        #region --- 1. CÁC TRUY VẤN LOAD DỮ LIỆU (READ) ---

        // Lấy toàn bộ danh bạ hệ thống
        public async Task<DataTable> GetAllContactsAsync()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, (t.FirstName + ' ' + t.LastName) AS Name, ISNULL(t.Phone, '') AS Phone, ISNULL(t.Email, '') AS Email, N'Hệ thống / Giảng viên' AS GroupName, 1 AS IsSystemData FROM dbo.Teachers t WHERE t.Status = 1
                UNION ALL
                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, ISNULL(c.Phone, '') AS Phone, ISNULL(c.Email, '') AS Email, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData FROM dbo.Contact c LEFT JOIN dbo.Groups g ON c.Group_ID = g.ID
                ORDER BY IsSystemData DESC, Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                await Task.Run(() => da.Fill(dt));
            }
            return dt;
        }

        // Tìm kiếm trên toàn bộ hệ thống 
        public async Task<DataTable> SearchContactsAsync(string keyword)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT * FROM (
                    SELECT 'TEACHER_' + CAST(MSGV AS VARCHAR(50)) AS UniqueID, (FirstName + ' ' + LastName) AS Name, Phone, Email, N'Hệ thống / Giảng viên' AS GroupName, 1 AS IsSystemData FROM dbo.Teachers WHERE Status = 1
                    UNION ALL
                    SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, c.Phone, c.Email, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData FROM dbo.Contact c LEFT JOIN dbo.Groups g ON c.Group_ID = g.ID
                ) AS Combined
                WHERE Name LIKE @keyword OR Phone LIKE @keyword OR Email LIKE @keyword
                ORDER BY IsSystemData DESC, Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword.Trim() + "%");
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        // Lấy danh bạ gộp có JOIN lấy tên phòng ban (Dùng chính cho ContactForm của bạn)
        public async Task<DataTable> GetAllContactsByUserAsync(int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, 
                       (t.FirstName + ' ' + t.LastName) AS Name, 
                       ISNULL(t.Phone, '') AS Phone, 
                       ISNULL(t.Email, '') AS Email, 
                       -1 AS Group_ID, 
                       N'Hệ thống / Giảng viên' AS GroupName, -- Cột hiển thị phòng ban hệ thống
                       1 AS IsSystemData 
                FROM dbo.Teachers t WHERE t.Status = 1
                
                UNION ALL
                
                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, 
                       c.Name, 
                       ISNULL(c.Phone, '') AS Phone, 
                       ISNULL(c.Email, '') AS Email, 
                       ISNULL(c.Group_ID, -1) AS Group_ID, 
                       ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, -- LEFT JOIN lấy tên phòng ban thực tế từ bảng Groups
                       0 AS IsSystemData 
                FROM dbo.Contact c
                LEFT JOIN dbo.Groups g ON c.Group_ID = g.ID
                WHERE c.UserID = @UserID
                ORDER BY IsSystemData DESC, Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        // Lọc danh bạ theo nhóm phòng ban cụ thể
        public async Task<DataTable> GetContactsByGroupAsync(int groupId, int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, ISNULL(c.Phone, '') AS Phone, ISNULL(c.Email, '') AS Email, c.Group_ID, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData 
                FROM dbo.Contact c 
                LEFT JOIN dbo.Groups g ON c.Group_ID = g.ID 
                WHERE c.Group_ID = @GroupID AND c.UserID = @UserID ORDER BY c.Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        // Tìm kiếm danh bạ bảo mật có kèm tên phòng ban
        public async Task<DataTable> SearchContactsByUserAsync(string keyword, int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT * FROM (
                    SELECT 'TEACHER_' + CAST(MSGV AS VARCHAR(50)) AS UniqueID, (FirstName + ' ' + LastName) AS Name, Phone, Email, -1 AS Group_ID, N'Hệ thống / Giảng viên' AS GroupName, 1 AS IsSystemData FROM dbo.Teachers WHERE Status = 1
                    UNION ALL
                    SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, c.Phone, c.Email, ISNULL(c.Group_ID, -1) AS Group_ID, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData FROM dbo.Contact c LEFT JOIN dbo.Groups g ON c.Group_ID = g.ID WHERE c.UserID = @UserID
                ) AS Combined
                WHERE Name LIKE @keyword OR Phone LIKE @keyword OR Email LIKE @keyword
                ORDER BY IsSystemData DESC, Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword.Trim() + "%");
                cmd.Parameters.AddWithValue("@UserID", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        #endregion

        #region --- 2. CÁC THAO TÁC CRUD CONTACT ---

        public async Task<bool> InsertContactAsync(string fname, string lname, string phone, string email, int? groupId, int userId)
        {
            string query = @"INSERT INTO dbo.Contact (Name, Fname, Lname, Phone, Email, Group_ID, UserID) 
                             VALUES (@Name, @Fname, @Lname, @Phone, @Email, @GroupID, @UserID)";

            string fullName = (fname.Trim() + " " + (lname ?? "").Trim()).Trim();

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = fullName;
                cmd.Parameters.Add("@Fname", SqlDbType.NVarChar, 50).Value = fname.Trim();
                cmd.Parameters.Add("@Lname", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(lname) ? DBNull.Value : (object)lname.Trim();
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();
                cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId.HasValue ? (object)groupId.Value : DBNull.Value;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateContactAsync(int contactId, string fname, string lname, string phone, string email, int? groupId, int userId)
        {
            string query = @"UPDATE dbo.Contact 
                             SET Name = @Name, Fname = @Fname, Lname = @Lname, Phone = @Phone, Email = @Email, Group_ID = @GroupID 
                             WHERE ContactID = @ContactID AND UserID = @UserID";

            string fullName = (fname.Trim() + " " + (lname ?? "").Trim()).Trim();

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactId;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = fullName;
                cmd.Parameters.Add("@Fname", SqlDbType.NVarChar, 50).Value = fname.Trim();
                cmd.Parameters.Add("@Lname", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(lname) ? DBNull.Value : (object)lname.Trim();
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();
                cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId.HasValue ? (object)groupId.Value : DBNull.Value;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteContactAsync(int contactId, int userId)
        {
            string query = "DELETE FROM dbo.Contact WHERE ContactID = @ContactID AND UserID = @UserID";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactId;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> IsPhoneOrEmailExistsAsync(string phone, string email, int? excludeId = null, int? userId = null)
        {
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email)) return false;

            string query = @"
                SELECT COUNT(1) FROM dbo.Contact 
                WHERE ((@Phone IS NOT NULL AND Phone = @Phone) OR (@Email IS NOT NULL AND Email = @Email))
                AND (@ExcludeId IS NULL OR ContactID <> @ExcludeId)
                AND (@UserID IS NULL OR UserID = @UserID)";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();
                cmd.Parameters.Add("@ExcludeId", SqlDbType.Int).Value = excludeId.HasValue ? (object)excludeId.Value : DBNull.Value;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId.HasValue ? (object)userId.Value : DBNull.Value;

                await conn.OpenAsync();
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
        }

        #endregion

        #region --- 3. QUẢN LÝ DANH MỤC NHÓM (GROUPS CRUD) ---

        public async Task<DataTable> GetGroupsByUserAsync(int userId)
        {
            DataTable dt = new DataTable();
            string query = "SELECT ID, Name FROM dbo.Groups WHERE UserID = @UserID ORDER BY Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        public async Task<bool> InsertGroupAsync(string groupName, int userId)
        {
            string query = "INSERT INTO dbo.Groups (Name, UserID) VALUES (@Name, @UserID)";
            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", groupName.Trim());
                cmd.Parameters.AddWithValue("@UserID", userId);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateGroupAsync(int groupId, string groupName, int userId)
        {
            string query = "UPDATE dbo.Groups SET Name = @Name WHERE ID = @GroupID AND UserID = @UserID";
            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@Name", groupName.Trim());
                cmd.Parameters.AddWithValue("@UserID", userId);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteGroupAsync(int groupId, int userId)
        {
            string query = "DELETE FROM dbo.Groups WHERE ID = @GroupID AND UserID = @UserID";
            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        #endregion
    }
}