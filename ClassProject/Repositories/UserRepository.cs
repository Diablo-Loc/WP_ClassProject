using ClassProject.DataAccess.Db; 
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.Repositories
{
    public class UserRepository
    {
        private readonly My_DB _db = new My_DB();

        // Hàm kiểm tra username đã tồn tại chưa
        public bool ExistUser(string username)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Hàm kiểm tra email đã tồn tại chưa
        public bool ExistEmail(string email)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Hàm thực hiện insert dữ liệu đăng ký mới vào DB
        public bool InsertUser(string mssv, string firstName, string lastName, string username, string password, string email, byte[] image, string position, int valid)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = "INSERT INTO Users (MSSV, FirstName, LastName, Username, Password, Email, Image, Position, Valid) " +
                               "VALUES (@MSSV, @FirstName, @LastName, @Username, @Password, @Email, @Image, @Position, @Valid)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MSSV", mssv);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Image", (object)image ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@Valid", valid);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}