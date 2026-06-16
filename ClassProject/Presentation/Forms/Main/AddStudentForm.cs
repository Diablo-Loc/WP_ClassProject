using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class AddStudentForm : Form
    {
        private byte[] studentImage = null;
        private int targetMssv;
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();

        public AddStudentForm(int mssv)
        {
            InitializeComponent();
            targetMssv = mssv;

            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            if (targetMssv > 0)
            {
                lblFormTitle.Text = "Chỉnh sửa sinh viên";
                lblFormSubTitle.Text = "Chỉnh sửa thông tin sinh viên trong hệ thống";
                btnSave.Text = "Cập nhật";
                txtMSSV.Text = targetMssv.ToString();
                txtMSSV.ReadOnly = true;

                try
                {
                    Student sv = studentRepo.GetStudentByMssv(targetMssv);

                    if (sv != null)
                    {
                        txtFirstName.Text = sv.LastName;
                        txtLastName.Text = sv.FirstName;
                        dtpDateOfBirth.Value = sv.DateOfBirth;
                        cboGender.Text = sv.Gender;
                        txtPhone.Text = sv.Phone;
                        txtAddress.Text = sv.Address;
                        txtHometown.Text = sv.Hometown;
                        txtEmail.Text = sv.Email;

                        if (sv.Picture != null)
                        {
                            studentImage = sv.Picture;
                            using (MemoryStream ms = new MemoryStream(studentImage))
                            {
                                picStudent.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu sinh viên này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải thông tin: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picStudent.Image = Image.FromFile(ofd.FileName);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        picStudent.Image.Save(ms, picStudent.Image.RawFormat);
                        studentImage = ms.ToArray();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text)) { MessageBox.Show("Vui lòng nhập Mã số sinh viên!"); return; }
            if (!int.TryParse(txtMSSV.Text, out int mssv)) { MessageBox.Show("Mã số sinh viên phải là số!"); return; }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) { MessageBox.Show("Vui lòng nhập Họ và tên đệm!"); return; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) { MessageBox.Show("Vui lòng nhập Tên sinh viên!"); return; }
            if (!IsValidEmail(txtEmail.Text)) { MessageBox.Show("Email không đúng định dạng!"); return; }
            if (!IsValidPhone(txtPhone.Text)) { MessageBox.Show("Số điện thoại phải gồm 10 số!"); return; }
            if (string.IsNullOrWhiteSpace(txtHometown.Text)) { MessageBox.Show("Vui lòng nhập Quê quán!"); return; }

            try
            {
                Student sv = new Student
                {
                    UserId = null,
                    Mssv = mssv,
                    LastName = txtFirstName.Text.Trim(),
                    FirstName = txtLastName.Text.Trim(),
                    DateOfBirth = dtpDateOfBirth.Value,
                    Gender = cboGender.Text,
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Hometown = txtHometown.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Picture = studentImage
                };

                if (targetMssv > 0)
                {
                    if (studentRepo.UpdateStudent(sv))
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
                    if (studentRepo.IsMssvExist(sv.Mssv))
                    {
                        MessageBox.Show("Mã số sinh viên này đã tồn tại trong hệ thống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (studentRepo.AddStudent(sv))
                    {
                        MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Thêm mới thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dữ liệu không hợp lệ: " + ex.Message, "Lỗi kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^[0-9]{10}$");
        }
    }
}