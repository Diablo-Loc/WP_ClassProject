using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClassProject.Models;

namespace ClassProject.DataAccess.Repositories
{
    public class ContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // 1. Lấy toàn bộ danh sách liên hệ
        public DataTable GetContacts()
        {
            DataTable dt = new DataTable();
            string query = "SELECT ContactID, Name, Phone, Email FROM dbo.Contact ORDER BY ContactID DESC";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // 2. Thêm mới liên hệ
        public bool AddContact(Contact contact)
        {
            string query = "INSERT INTO dbo.Contact (Name, Phone, Email) VALUES (@Name, @Phone, @Email)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", contact.Name);
                    cmd.Parameters.AddWithValue("@Phone", (object)contact.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)contact.Email ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 3. Cập nhật thông tin liên hệ
        public bool UpdateContact(Contact contact)
        {
            string query = "UPDATE dbo.Contact SET Name = @Name, Phone = @Phone, Email = @Email WHERE ContactID = @ContactID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ContactID", contact.ContactID);
                    cmd.Parameters.AddWithValue("@Name", contact.Name);
                    cmd.Parameters.AddWithValue("@Phone", (object)contact.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)contact.Email ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 4. Xóa liên hệ theo ID
        public bool DeleteContact(int contactId)
        {
            string query = "DELETE FROM dbo.Contact WHERE ContactID = @ContactID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ContactID", contactId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Tìm kiếm liên hệ theo Tên hoặc Số điện thoại
        public DataTable SearchContacts(string keyword)
        {
            DataTable dt = new DataTable();
            string query = "SELECT ContactID, Name, Phone, Email FROM dbo.Contact " +
                           "WHERE Name LIKE @Keyword OR Phone LIKE @Keyword ORDER BY ContactID DESC";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}