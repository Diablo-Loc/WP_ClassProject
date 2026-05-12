using ClassProject.DataAccess.Db;
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
            if (txtMSSV.Text.Trim() == "")
            {
                MessageBox.Show("Nhập MSSV");
                return;
            }

            if (!int.TryParse(txtMSSV.Text, out _))
            {
                MessageBox.Show("MSSV phải là số");
                return;
            }

            if (txtLastName.Text.Trim() == "")
            {
                MessageBox.Show("Nhập tên");
                return;
            }

            if (dtpDateOfBirth.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không được lớn hơn ngày hiện tại!", "Cảnh báo");
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ");
                return;
            }

            if (!IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Số điện thoại không hợp lệ");
                return;
            }

            if (studentImage == null)
            {
                MessageBox.Show("Chọn ảnh");
                return;
            }
            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    Student sv = new Student(
                        //currentUserId, // UserId
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
                    if (sv.IsMssvExist(int.Parse(txtMSSV.Text), conn.ConnectionString))
                    {
                        MessageBox.Show("Mã số sinh viên này đã tồn tại!");
                        return;
                    }
                    if (sv.AddStudent(conn.ConnectionString))
                    {
                        MessageBox.Show("Thêm sinh viên thành công");
                        btnClear.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Thêm thất bại");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi DB: " + ex.Message);
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
    }
}
