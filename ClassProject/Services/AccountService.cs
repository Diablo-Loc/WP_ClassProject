using System;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;
using BCrypt.Net;

namespace ClassProject.DataAccess
{
    public class AccountService
    {
        private readonly My_DB _db = new My_DB();

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                // 1. Lấy mật khẩu băm hiện tại từ Database của User này lên để kiểm tra
                string currentHashedPassword = string.Empty;
                string queryGetPass = "SELECT Password FROM Users WHERE Username = @user AND ISNULL(Status, 0) != -1";

                using (SqlCommand cmdGet = new SqlCommand(queryGetPass, conn))
                {
                    cmdGet.Parameters.AddWithValue("@user", username);
                    object result = cmdGet.ExecuteScalar();
                    if (result != null)
                    {
                        currentHashedPassword = result.ToString();
                    }
                }

                // Nếu không tìm thấy user hoặc pass trống
                if (string.IsNullOrEmpty(currentHashedPassword)) return false;

                // 2. Dùng BCrypt để xác thực xem mật khẩu cũ nhập vào có đúng không
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, currentHashedPassword))
                {
                    return false; // Mật khẩu cũ không chính xác
                }

                // 3. Đúng mật khẩu cũ -> Tiến hành BĂM mật khẩu mới bằng BCrypt
                string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // 4. Cập nhật chuỗi mật khẩu đã băm mới vào Database
                string queryUpdate = "UPDATE Users SET Password = @newPass WHERE Username = @user";
                using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@newPass", newHashedPassword);
                    cmdUpdate.Parameters.AddWithValue("@user", username);

                    int rows = cmdUpdate.ExecuteNonQuery();
                    return rows > 0; // Trả về true nếu cập nhật thành công
                }
            }
        }
    }
}