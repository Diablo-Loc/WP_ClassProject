using ClassProject.DataAccess.Db;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class AddStudentForm : Form
    {
        byte[] studentImage = null;
        private int currentUserId;
        public AddStudentForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            // Nếu currentUserId > 0, nghĩa là form đang mở ở chế độ SỬA SINH VIÊN
            if (currentUserId > 0)
            {
                this.Text = "Chỉnh sửa thông tin sinh viên";
                btnAdd.Text = "Cập nhật"; 
                txtMSSV.Text = currentUserId.ToString();
                txtMSSV.ReadOnly = true; // Khóa không cho sửa mã SV (khóa chính)

                My_DB db = new My_DB();
                using (SqlConnection conn = db.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture FROM Students WHERE Mssv = @mssv";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@mssv", currentUserId);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtFirstName.Text = reader["FirstName"].ToString();
                            txtLastName.Text = reader["LastName"].ToString();
                            dtpDateOfBirth.Value = Convert.ToDateTime(reader["DateOfBirth"]);
                            cboGender.Text = reader["Gender"].ToString();
                            txtPhone.Text = reader["Phone"].ToString();
                            txtAddress.Text = reader["Address"].ToString();
                            txtHometown.Text = reader["Hometown"].ToString();
                            txtEmail.Text = reader["Email"].ToString();

                            if (reader["Picture"] != DBNull.Value)
                            {
                                studentImage = (byte[])reader["Picture"];
                                using (MemoryStream ms = new MemoryStream(studentImage))
                                {
                                    picStudent.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                picStudent.Image = null;
                                studentImage = null;
                            }
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi tải thông tin sinh viên: " + ex.Message);
                    }
                }
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picStudent.Image = Image.FromFile(ofd.FileName);

                MemoryStream ms = new MemoryStream();

                picStudent.Image.Save(ms, picStudent.Image.RawFormat);

                studentImage = ms.ToArray();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // ==========================================
            // 1. KIỂM TRA DỮ LIỆU ĐẦU VÀO (VALIDATION)
            // ==========================================
            if (txtMSSV.Text.Trim() == "") { MessageBox.Show("Nhập MSSV"); return; }
            if (!int.TryParse(txtMSSV.Text, out _)) { MessageBox.Show("MSSV phải là số"); return; }
            if (txtLastName.Text.Trim() == "") { MessageBox.Show("Nhập tên"); return; }
            if (dtpDateOfBirth.Value > DateTime.Now) { MessageBox.Show("Ngày sinh không hợp lệ!", "Cảnh báo"); return; }
            if (!IsValidEmail(txtEmail.Text)) { MessageBox.Show("Email không hợp lệ"); return; }
            if (!IsValidPhone(txtPhone.Text)) { MessageBox.Show("Số điện thoại không hợp lệ"); return; }
            if (studentImage == null) { MessageBox.Show("Chọn ảnh"); return; }

            // ==========================================
            // 2. XỬ LÝ DATABASE CHÍNH XÁC
            // ==========================================
            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Khởi tạo đối tượng Student từ thông tin trên form
                    Student sv = new Student(
                        int.Parse(txtMSSV.Text),
                        txtFirstName.Text,
                        txtLastName.Text,
                        dtpDateOfBirth.Value,
                        cboGender.Text,
                        txtPhone.Text,
                        txtAddress.Text,
                        txtHometown.Text,
                        txtEmail.Text,
                        studentImage
                    );

                    // TÁCH BIỆT HOÀN TOÀN CÁC NHÁNH BẰNG IF-ELSE
                    if (currentUserId > 0)
                    {
                        // ---------------------------------------------------------
                        // CHẾ ĐỘ SỬA: CHỈ gọi UpdateStudent (Tuyệt đối không gọi AddStudent)
                        // ---------------------------------------------------------
                        if (sv.UpdateStudent(conn.ConnectionString))
                        {
                            MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // ---------------------------------------------------------
                        // CHẾ ĐỘ THÊM MỚI: Kiểm tra trùng mã rồi mới gọi AddStudent
                        // ---------------------------------------------------------
                        if (sv.IsMssvExist(int.Parse(txtMSSV.Text), conn.ConnectionString))
                        {
                            MessageBox.Show("Mã số sinh viên này đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (sv.AddStudent(conn.ConnectionString))
                        {
                            MessageBox.Show("Thêm sinh viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnClear.PerformClick();
                        }
                        else
                        {
                            MessageBox.Show("Thêm thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi DB: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMSSV.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtHometown.Clear();
            txtEmail.Clear();

            cboGender.SelectedIndex = -1;
            cboGender.Text = "";

            dtpDateOfBirth.Value = DateTime.Now;

            picStudent.Image = null;

            studentImage = null;
        }

        bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, pattern);
        }
        bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^[0-9]{10}$");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlBackground_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
