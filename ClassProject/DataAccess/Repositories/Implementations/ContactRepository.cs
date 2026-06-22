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

        // Lấy toàn bộ danh bạ hệ thống (Đã map qua bảng trung gian)
        public async Task<DataTable> GetAllContactsAsync()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, 
                       (t.FirstName + ' ' + t.LastName) AS Name, 
                       ISNULL(t.Phone, '') AS Phone, 
                       ISNULL(t.Email, '') AS Email, 
                       ISNULL(mgm.GroupID, -1) AS Group_ID,
                       ISNULL(g.Name, N'Hệ thống / Giảng viên') AS GroupName, 
                       1 AS IsSystemData 
                FROM dbo.Teachers t 
                LEFT JOIN dbo.MemberGroupMappings mgm ON ('TEACHER_' + CAST(t.MSGV AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
                WHERE t.Status = 1

                UNION ALL

                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, 
                       c.Name, 
                       ISNULL(c.Phone, '') AS Phone, 
                       ISNULL(c.Email, '') AS Email, 
                       ISNULL(mgm.GroupID, -1) AS Group_ID,
                       ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 
                       0 AS IsSystemData 
                FROM dbo.Contact c 
                LEFT JOIN dbo.MemberGroupMappings mgm ON ('CONTACT_' + CAST(c.ContactID AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
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
                    SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, (t.FirstName + ' ' + t.LastName) AS Name, t.Phone, t.Email, ISNULL(mgm.GroupID, -1) AS Group_ID, ISNULL(g.Name, N'Hệ thống / Giảng viên') AS GroupName, 1 AS IsSystemData 
                    FROM dbo.Teachers t
                    LEFT JOIN dbo.MemberGroupMappings mgm ON ('TEACHER_' + CAST(t.MSGV AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                    LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
                    WHERE t.Status = 1
                    
                    UNION ALL
                    
                    SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, c.Phone, c.Email, ISNULL(mgm.GroupID, -1) AS Group_ID, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData 
                    FROM dbo.Contact c 
                    LEFT JOIN dbo.MemberGroupMappings mgm ON ('CONTACT_' + CAST(c.ContactID AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                    LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
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

        // Lấy danh bạ theo User sở hữu & lấy phòng ban chính (IsPrimary = 1)
        public async Task<DataTable> GetAllContactsByUserAsync(int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, 
                       (t.FirstName + ' ' + t.LastName) AS Name, 
                       ISNULL(t.Phone, '') AS Phone, 
                       ISNULL(t.Email, '') AS Email, 
                       ISNULL(mgm.GroupID, -1) AS Group_ID, 
                       ISNULL(g.Name, N'Hệ thống / Giảng viên') AS GroupName, 
                       1 AS IsSystemData 
                FROM dbo.Teachers t 
                LEFT JOIN dbo.MemberGroupMappings mgm ON ('TEACHER_' + CAST(t.MSGV AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
                WHERE t.Status = 1
                
                UNION ALL
                
                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, 
                       c.Name, 
                       ISNULL(c.Phone, '') AS Phone, 
                       ISNULL(c.Email, '') AS Email, 
                       ISNULL(mgm.GroupID, -1) AS Group_ID, 
                       ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 
                       0 AS IsSystemData 
                FROM dbo.Contact c
                LEFT JOIN dbo.MemberGroupMappings mgm ON ('CONTACT_' + CAST(c.ContactID AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID
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

        // Lọc danh bạ theo phòng ban cụ thể (Quét từ bảng trung gian)
        public async Task<DataTable> GetContactsByGroupAsync(int groupId, int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, ISNULL(c.Phone, '') AS Phone, ISNULL(c.Email, '') AS Email, mgm.GroupID AS Group_ID, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData 
                FROM dbo.Contact c 
                INNER JOIN dbo.MemberGroupMappings mgm ON ('CONTACT_' + CAST(c.ContactID AS VARCHAR(50))) = mgm.UniqueID
                LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID 
                WHERE mgm.GroupID = @GroupID AND c.UserID = @UserID ORDER BY c.Name ASC";

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

        // Tìm kiếm danh bạ bảo mật có kèm thông tin phòng ban mới
        public async Task<DataTable> SearchContactsByUserAsync(string keyword, int userId)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT * FROM (
                    SELECT 'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID, (t.FirstName + ' ' + t.LastName) AS Name, t.Phone, t.Email, ISNULL(mgm.GroupID, -1) AS Group_ID, ISNULL(g.Name, N'Hệ thống / Giảng viên') AS GroupName, 1 AS IsSystemData 
                    FROM dbo.Teachers t 
                    LEFT JOIN dbo.MemberGroupMappings mgm ON ('TEACHER_' + CAST(t.MSGV AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                    LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID 
                    WHERE t.Status = 1
                    
                    UNION ALL
                    
                    SELECT 'CONTACT_' + CAST(c.ContactID AS VARCHAR(50)) AS UniqueID, c.Name, c.Phone, c.Email, ISNULL(mgm.GroupID, -1) AS Group_ID, ISNULL(g.Name, N'-- Chưa phân phòng --') AS GroupName, 0 AS IsSystemData 
                    FROM dbo.Contact c 
                    LEFT JOIN dbo.MemberGroupMappings mgm ON ('CONTACT_' + CAST(c.ContactID AS VARCHAR(50))) = mgm.UniqueID AND mgm.IsPrimary = 1
                    LEFT JOIN dbo.Groups g ON mgm.GroupID = g.ID 
                    WHERE c.UserID = @UserID
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

        #region --- 2. CÁC THAO TÁC CRUD CONTACT (Sử dụng Transaction) ---

        public async Task<bool> InsertContactAsync(string fname, string lname, string phone, string email, int? groupId, int userId)
        {
            string insertContactQuery = @"INSERT INTO dbo.Contact (Name, Fname, Lname, Phone, Email, UserID) 
                                          VALUES (@Name, @Fname, @Lname, @Phone, @Email, @UserID);
                                          SELECT SCOPE_IDENTITY();";

            string insertMappingQuery = @"INSERT INTO dbo.MemberGroupMappings (UniqueID, GroupID, IsPrimary) 
                                          VALUES (@UniqueID, @GroupID, 1);";

            string fullName = (fname.Trim() + " " + (lname ?? "").Trim()).Trim();

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int newContactId = 0;
                        using (SqlCommand cmd = new SqlCommand(insertContactQuery, conn, trans))
                        {
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = fullName;
                            cmd.Parameters.Add("@Fname", SqlDbType.NVarChar, 50).Value = fname.Trim();
                            cmd.Parameters.Add("@Lname", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(lname) ? DBNull.Value : (object)lname.Trim();
                            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();
                            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                            newContactId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }

                        if (groupId.HasValue && groupId.Value > 0 && newContactId > 0)
                        {
                            using (SqlCommand cmdMap = new SqlCommand(insertMappingQuery, conn, trans))
                            {
                                cmdMap.Parameters.AddWithValue("@UniqueID", "CONTACT_" + newContactId);
                                cmdMap.Parameters.AddWithValue("@GroupID", groupId.Value);
                                await cmdMap.ExecuteNonQueryAsync();
                            }
                        }

                        trans.Commit();
                        return newContactId > 0;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // ĐÃ FIX: Giữ nguyên int contactId của Interface gốc, tự check bảng chính xác
        public async Task<bool> UpdateContactAsync(int contactId, string fname, string lname, string phone, string email, int? groupId, int userId)
        {
            bool isTeacher = false;
            string actualTeacherMsgv = string.Empty;

            // Kiểm tra xem ID này thực tế là khóa chính hay là mã MSGV của bảng Teachers hay không
            string checkTeacherQuery = "SELECT TOP 1 MSGV FROM dbo.Teachers WHERE Id = @Id OR MSGV = CAST(@Id AS NVARCHAR(30))";

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (SqlCommand checkCmd = new SqlCommand(checkTeacherQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Id", contactId);
                    var obj = await checkCmd.ExecuteScalarAsync();
                    if (obj != null && obj != DBNull.Value)
                    {
                        isTeacher = true;
                        actualTeacherMsgv = obj.ToString(); // Lấy đúng chuỗi MSGV lưu trong DB hệ thống
                    }
                }
            }

            // Gán đúng cấu trúc UniqueID phục vụ map chính xác cho bảng Mapping trung gian
            string finalUniqueId = isTeacher ? $"TEACHER_{actualTeacherMsgv}" : $"CONTACT_{contactId}";

            string updateSql = "";
            if (isTeacher)
            {
                updateSql = @"UPDATE dbo.Teachers 
                              SET FirstName = @Fname, LastName = @Lname, Phone = @Phone, Email = @Email, Updated_At = GETDATE()
                              WHERE MSGV = CAST(@ContactID AS NVARCHAR(30)) OR Id = @ContactID;";
            }
            else
            {
                updateSql = @"UPDATE dbo.Contact 
                              SET Name = @Name, Fname = @Fname, Lname = @Lname, Phone = @Phone, Email = @Email 
                              WHERE ContactID = @ContactID AND (UserID = @UserID OR @UserID = 1);";
            }

            string deleteMappingQuery = "DELETE FROM dbo.MemberGroupMappings WHERE UniqueID = @UniqueID;";
            string insertMappingQuery = "INSERT INTO dbo.MemberGroupMappings (UniqueID, GroupID, IsPrimary) VALUES (@UniqueID, @GroupID, 1);";
            string fullName = (fname.Trim() + " " + (lname ?? "").Trim()).Trim();

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int rowsAffected = 0;
                        using (SqlCommand cmd = new SqlCommand(updateSql, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@ContactID", contactId);
                            cmd.Parameters.Add("@Fname", SqlDbType.NVarChar, 100).Value = fname.Trim();
                            cmd.Parameters.Add("@Lname", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(lname) ? DBNull.Value : (object)lname.Trim();

                            if (!isTeacher)
                            {
                                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = fullName;
                            }
                            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();
                            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                            rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }

                        if (isTeacher) rowsAffected = 1; // Cho phép cập nhật phòng ban nếu lệnh trên tìm thấy thực thể

                        if (rowsAffected > 0)
                        {
                            using (SqlCommand cmdDel = new SqlCommand(deleteMappingQuery, conn, trans))
                            {
                                cmdDel.Parameters.AddWithValue("@UniqueID", finalUniqueId);
                                await cmdDel.ExecuteNonQueryAsync();
                            }

                            if (groupId.HasValue && groupId.Value > 0)
                            {
                                using (SqlCommand cmdIns = new SqlCommand(insertMappingQuery, conn, trans))
                                {
                                    cmdIns.Parameters.AddWithValue("@UniqueID", finalUniqueId);
                                    cmdIns.Parameters.AddWithValue("@GroupID", groupId.Value);
                                    await cmdIns.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        trans.Commit();
                        return rowsAffected > 0;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> DeleteContactAsync(int contactId, int userId)
        {
            string deleteMappingQuery = "DELETE FROM dbo.MemberGroupMappings WHERE UniqueID = @UniqueID";
            string deleteContactQuery = "DELETE FROM dbo.Contact WHERE ContactID = @ContactID AND UserID = @UserID";

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmdMap = new SqlCommand(deleteMappingQuery, conn, trans))
                        {
                            cmdMap.Parameters.AddWithValue("@UniqueID", "CONTACT_" + contactId);
                            await cmdMap.ExecuteNonQueryAsync();
                        }

                        int result = 0;
                        using (SqlCommand cmdContact = new SqlCommand(deleteContactQuery, conn, trans))
                        {
                            cmdContact.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactId;
                            cmdContact.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                            result = await cmdContact.ExecuteNonQueryAsync();
                        }

                        trans.Commit();
                        return result > 0;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
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
            string query = "SELECT ID, Name, GroupCode, ParentID, IsSystemData FROM dbo.Groups WHERE UserID = @UserID OR IsSystemData = 1 ORDER BY ParentID ASC, Name ASC";

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
            string query = "INSERT INTO dbo.Groups (Name, UserID, ParentID, IsSystemData) VALUES (@Name, @UserID, NULL, 0)";
            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", groupName.Trim());
                cmd.Parameters.AddWithValue("@UserID", userId);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> InsertGroupExtendedAsync(string groupName, string groupCode, int? parentId, int userId, bool isSystem)
        {
            string query = "INSERT INTO dbo.Groups (Name, GroupCode, ParentID, IsSystemData, UserID) VALUES (@Name, @GroupCode, @ParentID, @IsSystem, @UserID)";
            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", groupName.Trim());
                cmd.Parameters.AddWithValue("@GroupCode", string.IsNullOrEmpty(groupCode) ? DBNull.Value : (object)groupCode.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@ParentID", parentId.HasValue ? (object)parentId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@IsSystem", isSystem ? 1 : 0);
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