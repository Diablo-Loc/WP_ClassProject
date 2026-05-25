using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BCrypt.Net;

namespace ClassProject.Presentation.Forms
{
    public partial class RegisterForm : Form
    {
        My_DB db = new My_DB();
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirm.Text.Trim();

            // 1. Kiểm tra đầu vào cơ bản
            if (username == "" || email == "" || password == "" || confirmPassword == "")
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboPosition.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ (Student/HR)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedText = cboPosition.SelectedItem.ToString();

            int selectedRoleId = (selectedText == "Student") ? 1 : 2;

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Email không đúng định dạng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Xử lý Database
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // CHẶNG A: Kiểm tra Username/Email đã tồn tại trong bảng Users chưa
                            const string checkUserQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username OR Email = @email";
                            using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn, tx))
                            {
                                checkCmd.Parameters.AddWithValue("@username", username);
                                checkCmd.Parameters.AddWithValue("@email", email);
                                if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                                {
                                    MessageBox.Show("Username hoặc Email đã được sử dụng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                            int validStatus = 0; // Mặc định là 0 (Chờ duyệt đối với HR)
                            int? targetStudentId = null; // Biến lưu ID của sinh viên nếu tìm thấy

                            // CHẶNG B: Nếu là Sinh viên -> Bắt buộc phải có trong danh sách của nhà trường (bảng Students)
                            if (selectedRoleId == 1)
                            {
                                // Quét Email để tìm hồ sơ sinh viên do HR đã nhập
                                string checkStudentQuery = "SELECT Id, UserId FROM dbo.Students WHERE Email = @email";
                                using (SqlCommand cmd = new SqlCommand(checkStudentQuery, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@email", email);
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (!reader.Read())
                                        {
                                            MessageBox.Show("Email này chưa có trong danh sách sinh viên của trường!\nVui lòng liên hệ HR để được nhập hồ sơ trước.", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return; // Bắt buộc chặn lại
                                        }

                                        if (reader["UserId"] != DBNull.Value)
                                        {
                                            MessageBox.Show("Sinh viên này đã được đăng ký và kích hoạt tài khoản rồi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        targetStudentId = Convert.ToInt32(reader["Id"]);
                                    }
                                }
                                validStatus = 1; // Học sinh đã có hồ sơ -> Cho phép Valid = 1 luôn (Đăng nhập được ngay)
                            }

                            // CHẶNG C: Tạo tài khoản trong bảng Users
                            string insertUserQuery = "INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid) " +
                                                     "OUTPUT INSERTED.Id " + // Lấy Id vừa tạo
                                                     "VALUES (@username, @email, @password, @roleId, @valid)";
                            int newUserId = 0;
                            using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, conn, tx))
                            {
                                insertCmd.Parameters.AddWithValue("@username", username);
                                insertCmd.Parameters.AddWithValue("@email", email);
                                insertCmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));
                                insertCmd.Parameters.AddWithValue("@roleId", selectedRoleId);
                                insertCmd.Parameters.AddWithValue("@valid", validStatus);

                                newUserId = (int)insertCmd.ExecuteScalar();
                            }

                            // CHẶNG D: Nếu là sinh viên, liên kết Id tài khoản ngược lại vào bảng Students
                            if (selectedRoleId == 1 && targetStudentId.HasValue)
                            {
                                string updateStudentQuery = "UPDATE dbo.Students SET UserId = @userId WHERE Id = @studentId";
                                using (SqlCommand updateCmd = new SqlCommand(updateStudentQuery, conn, tx))
                                {
                                    updateCmd.Parameters.AddWithValue("@userId", newUserId);
                                    updateCmd.Parameters.AddWithValue("@studentId", targetStudentId.Value);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            tx.Commit(); // Lưu toàn bộ giao dịch vào Database

                            // Thông báo tùy theo chức vụ
                            if (selectedRoleId == 1)
                                MessageBox.Show("Kích hoạt tài khoản Sinh viên thành công! Bạn có thể đăng nhập ngay bây giờ.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("Đăng ký tài khoản HR thành công! Vui lòng đợi Admin duyệt trước khi đăng nhập.", "Chờ duyệt", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tx.Rollback(); // Nếu có lỗi bất kỳ, hủy bỏ toàn bộ (không tạo user rác)
                            MessageBox.Show("Lỗi trong quá trình ghi dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblBacktoLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
