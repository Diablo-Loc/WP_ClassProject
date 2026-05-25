using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
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
        private int targetMssv;
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();

        // Constructor nhận vào mssv (nếu = 0 là THÊM MỚI, nếu > 0 là CHỈNH SỬA)
        public AddStudentForm(int mssv)
        {
            InitializeComponent();
            targetMssv = mssv;

            // Khởi tạo tầng xử lý dữ liệu thông qua Repository
            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            // Chế độ CHỈNH SỬA SINH VIÊN (Sạch bóng SQL, gọi dữ liệu qua Repo)
            if (targetMssv > 0)
            {
                this.Text = "Chỉnh sửa thông tin sinh viên";
                btnAdd.Text = "Cập nhật";
                txtMSSV.Text = targetMssv.ToString();
                txtMSSV.ReadOnly = true; // Khóa trường khóa chính

                try
                {
                    // Lấy thực thể student thuần từ tầng Repo
                    Student sv = studentRepo.GetStudentByMssv(targetMssv);

                    if (sv != null)
                    {
                        txtFirstName.Text = sv.FirstName;
                        txtLastName.Text = sv.LastName;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nhanh định dạng cơ bản trước khi nạp vào Model
            if (string.IsNullOrWhiteSpace(txtMSSV.Text)) { MessageBox.Show("Nhập MSSV"); return; }
            if (!int.TryParse(txtMSSV.Text, out int mssv)) { MessageBox.Show("MSSV phải là số"); return; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) { MessageBox.Show("Nhập họ sinh viên"); return; }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) { MessageBox.Show("Nhập tên sinh viên"); return; }
            if (!IsValidEmail(txtEmail.Text)) { MessageBox.Show("Email không đúng định dạng!"); return; }
            if (!IsValidPhone(txtPhone.Text)) { MessageBox.Show("Số điện thoại phải gồm 10 số!"); return; }
            if (studentImage == null) { MessageBox.Show("Vui lòng chọn ảnh thẻ sinh viên!"); return; }

            try
            {
                // 2. Nạp dữ liệu vào Model để Model tự kiểm tra logic nghiệp vụ (Validation trong set)
                Student sv = new Student();
                sv.UserId = null; // Khi HR thêm mới, học sinh chưa có tài khoản đăng nhập -> Để NULL
                sv.Mssv = mssv;
                sv.FirstName = txtFirstName.Text.Trim();
                sv.LastName = txtLastName.Text.Trim();
                sv.DateOfBirth = dtpDateOfBirth.Value;
                sv.Gender = cboGender.Text;
                sv.Phone = txtPhone.Text.Trim();
                sv.Address = txtAddress.Text.Trim();
                sv.Hometown = txtHometown.Text.Trim();
                sv.Email = txtEmail.Text.Trim();
                sv.Picture = studentImage;

                // 3. Thực thi thông qua Tầng Repository tách biệt
                if (targetMssv > 0)
                {
                    // --- CHẾ ĐỘ CẬP NHẬT ---
                    if (studentRepo.UpdateStudent(sv))
                    {
                        MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK; // Đánh dấu thành công để Form cha tự reload lưới
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // --- CHẾ ĐỘ THÊM MỚI ---
                    if (studentRepo.IsMssvExist(sv.Mssv))
                    {
                        MessageBox.Show("Mã số sinh viên này đã tồn tại trong hệ thống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (studentRepo.AddStudent(sv))
                    {
                        MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK; // Đánh dấu để Form danh sách biết có dữ liệu mới
                        btnClear.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Thêm mới thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Bắt toàn bộ các lỗi Business Logic do Model ném ra (Ví dụ: ảnh > 5MB, chặn ngày tương lai,...)
                MessageBox.Show("Dữ liệu không hợp lệ: " + ex.Message, "Lỗi kiểm tra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (targetMssv > 0)
            {
                // Đang sửa thì không cho clear trắng mã chính
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
            else
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
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^[0-9]{10}$");
        }
    }
}
