using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;

namespace ClassProject.DataAccess.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly My_DB _db = new My_DB();

        // Sử dụng Async để không làm nghẽn UI Thread của Doanh nghiệp
        public async Task<DataTable> GetAllContactsAsync()
        {
            DataTable dt = new DataTable();
            // Chuẩn hóa cấu trúc: Dùng chuỗi để chứa cả MSGV (Giảng viên) và ContactID (Liên hệ ngoài)
            string query = @"
                SELECT 
                    'TEACHER_' + CAST(t.MSGV AS VARCHAR(50)) AS UniqueID,
                    (t.LastName + ' ' + t.FirstName) AS Name, 
                    ISNULL(t.Phone, '') AS Phone, 
                    ISNULL(t.Email, '') AS Email,
                    1 AS IsSystemData
                FROM dbo.Teachers t

                UNION ALL

                SELECT 
                    'CONTACT_' + CAST(ContactID AS VARCHAR(50)) AS UniqueID, 
                    Name, 
                    ISNULL(Phone, '') AS Phone, 
                    ISNULL(Email, '') AS Email,
                    0 AS IsSystemData
                FROM dbo.Contact
                ORDER BY IsSystemData DESC, Name ASC";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                // SqlDataAdapter không hỗ trợ Async trực tiếp cho Fill, dùng Task.Run để giải phóng UI
                await Task.Run(() => da.Fill(dt));
            }
            return dt;
        }

        public async Task<DataTable> SearchContactsAsync(string keyword)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT * FROM (
                    SELECT 'TEACHER_' + CAST(MSGV AS VARCHAR(50)) AS UniqueID, (LastName + ' ' + FirstName) AS Name, Phone, Email, 1 AS IsSystemData FROM dbo.Teachers
                    UNION ALL
                    SELECT 'CONTACT_' + CAST(ContactID AS VARCHAR(50)) AS UniqueID, Name, Phone, Email, 0 AS IsSystemData FROM dbo.Contact
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

        public async Task<bool> InsertContactAsync(string name, string phone, string email)
        {
            string query = "INSERT INTO dbo.Contact (Name, Phone, Email) VALUES (@Name, @Phone, @Email)";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name.Trim();
                cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateContactAsync(int contactId, string name, string phone, string email)
        {
            string query = "UPDATE dbo.Contact SET Name = @Name, Phone = @Phone, Email = @Email WHERE ContactID = @ContactID";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactId;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name.Trim();
                cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone.Trim();
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email.Trim();

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteContactAsync(int contactId)
        {
            string query = "DELETE FROM dbo.Contact WHERE ContactID = @ContactID";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactId;

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> IsPhoneOrEmailExistsAsync(string phone, string email, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email)) return false;

            // Loại bỏ hoàn toàn chuỗi động (Dynamic String) chống SQL Injection tuyệt đối
            string query = @"
                SELECT COUNT(1) FROM dbo.Contact 
                WHERE ((@Phone IS NOT NULL AND Phone = @Phone) OR (@Email IS NOT NULL AND Email = @Email))
                AND (@ExcludeId IS NULL OR ContactID <> @ExcludeId)";

            using (SqlConnection conn = _db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = string.IsNullOrEmpty(email) ? DBNull.Value : (object)email;
                cmd.Parameters.Add("@ExcludeId", SqlDbType.Int).Value = excludeId.HasValue ? (object)excludeId.Value : DBNull.Value;

                await conn.OpenAsync();
                return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            }
        }
    }
}