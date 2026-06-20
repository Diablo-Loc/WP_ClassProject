using System;
using System.Data;
using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class StaffRepository
    {
        private readonly My_DB _db = new My_DB();

        public bool CreateStaff(string username, string email, string passwordHash, string msnv, string firstName, string lastName, string phone, string department, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (SqlConnection conn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_CreateStaffAccount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@MSNV", msnv.Trim());
                    cmd.Parameters.AddWithValue("@FirstName", firstName.Trim());
                    cmd.Parameters.AddWithValue("@LastName", lastName.Trim());
                    cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Department", (object)department ?? DBNull.Value);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        errorMessage = ex.Message;
                        return false;
                    }
                }
            }
        }

        public bool UpdateStaff(string msnv, string firstName, string lastName, string phone, string department, out string errorMsg)
        {
            errorMsg = "";
            string query = @"UPDATE dbo.Staffs 
                             SET FirstName = @FirstName, LastName = @LastName, Phone = @Phone, Department = @Department 
                             WHERE MSNV = @MSNV";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MSNV", msnv.Trim());
                        cmd.Parameters.AddWithValue("@FirstName", firstName.Trim());
                        cmd.Parameters.AddWithValue("@LastName", lastName.Trim());
                        cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department", (object)department ?? DBNull.Value);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        public bool DeleteStaff(string msnv, out string errorMsg)
        {
            errorMsg = "";
            string query = @"UPDATE dbo.Staffs SET Status = 0 WHERE MSNV = @MSNV";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MSNV", msnv.Trim());
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
        public DataRow GetAccountInfoById(int staffId)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    // 🎯 THAY ĐỔI: Gọi trực tiếp Stored Procedure vừa tạo
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_GetStaffAccountSecurityInfo", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StaffId", staffId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi Proc GetAccountInfoById: " + ex.Message);
            }
            return null;
        }
        public bool ResetPassword(int staffId, string newPasswordHash, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ResetStaffPassword", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StaffId", staffId);
                        cmd.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true; // Nếu không vướng CATCH lỗi SQL, trả về true luôn cho an toàn
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
    }
}