using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class AddStudentForm : Form
    {
        private byte[] studentImage = null;
        private readonly string targetMssv;
        private readonly StudentRepository studentRepo;
        private readonly My_DB db = new My_DB();
        private bool isDataLoading = false;
        private bool isSaving = false;

        public AddStudentForm(string mssv)
        {
            InitializeComponent();
            targetMssv = mssv;
            studentRepo = new StudentRepository();
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            isDataLoading = true;
            LoadMajorComboBox();
            cboMaNganh.SelectedIndexChanged += CboMaNganh_SelectedIndexChanged;
            LoadClassroomComboBoxFiltered();
            isDataLoading = false;

            // CHẾ ĐỘ CHỈNH SỬA (EDIT)
            if (!string.IsNullOrEmpty(targetMssv))
            {
                this.Text = "Chỉnh sửa thông tin sinh viên";
                btnSave.Text = "Cập nhật";
                txtMSSV.Text = targetMssv;
                txtMSSV.ReadOnly = true;

                LoadStudentDataForEdit();
            }
        }

        private void LoadMajorComboBox()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT MaNganh, TenNganh FROM dbo.Major ORDER BY TenNganh ASC";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cboMaNganh.DataSource = dt;
                        cboMaNganh.DisplayMember = "TenNganh";
                        cboMaNganh.ValueMember = "MaNganh";
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Mất kết nối đến cơ sở dữ liệu khi tải danh mục Ngành!", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadClassroomComboBoxFiltered()
        {
            if (cboMaNganh.SelectedValue == null) return;
            string selectedMajor = cboMaNganh.SelectedValue.ToString();

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT MaLop, TenLop FROM dbo.Classroom WHERE MaNganh = @MaNganh ORDER BY TenLop ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNganh", selectedMajor);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            cboMaLop.DataSource = dt;
                            cboMaLop.DisplayMember = "TenLop";
                            cboMaLop.ValueMember = "MaLop";
                        }
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Mất kết nối đến cơ sở dữ liệu khi tải danh mục Lớp!", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboMaNganh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isDataLoading) LoadClassroomComboBoxFiltered();
        }

        private void LoadStudentDataForEdit()
        {
            try
            {
                Student sv = studentRepo.GetStudentByMssv(targetMssv);
                if (sv != null)
                {
                    txtFirstName.Text = sv.FirstName;
                    txtLastName.Text = sv.LastName;

                    if (sv.DateOfBirth.HasValue)
                    {
                        if (sv.DateOfBirth.Value >= dtpDateOfBirth.MinDate && sv.DateOfBirth.Value <= dtpDateOfBirth.MaxDate)
                            dtpDateOfBirth.Value = sv.DateOfBirth.Value;
                    }

                    cboGender.Text = string.IsNullOrWhiteSpace(sv.Gender) ? "Nam" : sv.Gender;
                    txtPhone.Text = sv.Phone;
                    txtAddress.Text = sv.Address;
                    txtHometown.Text = sv.Hometown;
                    txtEmail.Text = sv.Email;

                    // KHÓA EMAIL NẾU ĐÃ GẮN USER_ID 
                    if (sv.UserId != null)
                    {
                        txtEmail.ReadOnly = true;
                        txtEmail.BackColor = SystemColors.InactiveCaption;

                        // FIX LỖI lblEmailWarning: Thay vì gán trực tiếp vào label điều khiển (có thể thiếu cấu hình UI), 
                        // ta sử dụng thuộc tính ToolTip tích hợp sẵn trên ô text để thông báo thông minh cho người dùng.
                        ToolTip toolTip = new ToolTip();
                        toolTip.SetToolTip(txtEmail, "Trường dữ liệu này được cố định do tài khoản đang hoạt động.");
                    }

                    if (!string.IsNullOrEmpty(sv.MaNganh))
                    {
                        cboMaNganh.SelectedValue = sv.MaNganh;
                        LoadClassroomComboBoxFiltered();
                    }
                    if (!string.IsNullOrEmpty(sv.MaLop)) cboMaLop.SelectedValue = sv.MaLop;

                    // KIỂM TRA CHỐNG FILE ẢNH HỎNG
                    if (sv.Picture != null && sv.Picture.Length > 0)
                    {
                        studentImage = sv.Picture;
                        using (MemoryStream ms = new MemoryStream(studentImage))
                        {
                            picStudent.Image?.Dispose();
                            picStudent.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị hồ sơ: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo fInfo = new FileInfo(ofd.FileName);

                        // Đồng bộ giới hạn dung lượng ảnh <= 2MB chuẩn Enterprise chống phình DB
                        if (fInfo.Length > 2 * 1024 * 1024)
                        {
                            MessageBox.Show("Dung lượng ảnh vượt quá giới hạn cho phép (Tối đa 2MB)!", "Xác thực tệp tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (fInfo.Length == 0)
                        {
                            MessageBox.Show("Tệp tin hình ảnh bị trống (0 byte)!", "Xác thực tệp tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (Image testImg = Image.FromStream(fs))
                            {
                                picStudent.Image?.Dispose();
                                picStudent.Image = new Bitmap(testImg);

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    testImg.Save(ms, testImg.RawFormat);
                                    studentImage = ms.ToArray();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Tệp tin không đúng cấu trúc đồ họa chuẩn, nghi vấn giả mạo định dạng hoặc file hỏng!", "An Ninh Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        studentImage = null;
                        picStudent.Image = null;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isSaving) return;
            isSaving = true;

            try
            {
                string mssv = txtMSSV.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string firstName = txtFirstName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();

                if (!Regex.IsMatch(mssv, @"^[A-Za-z0-9]{6,20}$"))
                {
                    MessageBox.Show("MSSV không hợp lệ! Độ dài yêu cầu từ 6-20 ký tự, không trống, không chứa khoảng trắng hoặc ký tự đặc biệt.", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMSSV.Focus(); return;
                }

                if (string.IsNullOrEmpty(lastName) || lastName.Length > 100)
                {
                    MessageBox.Show("Họ đệm không được trống và không được vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLastName.Focus(); return;
                }
                if (string.IsNullOrEmpty(firstName) || firstName.Length > 100)
                {
                    MessageBox.Show("Tên sinh viên không được trống và không được vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirstName.Focus(); return;
                }

                if (dtpDateOfBirth.Value.Date > DateTime.Today || dtpDateOfBirth.Value.Year < 1900)
                {
                    MessageBox.Show("Ngày sinh không thể lớn hơn ngày hiện tại hoặc nhỏ hơn năm 1900!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || email.Length > 100)
                {
                    MessageBox.Show("Email không đúng định dạng quy chuẩn hoặc vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus(); return;
                }

                if (!Regex.IsMatch(phone, @"^[0-9]{10,15}$"))
                {
                    MessageBox.Show("Số điện thoại phải từ 10-15 ký số thuần túy!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus(); return;
                }

                if (cboMaNganh.SelectedValue == null || cboMaLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Chuyên ngành và Lớp hành chính hợp lệ!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Student sv = new Student
                {
                    Mssv = mssv,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dtpDateOfBirth.Value,
                    Gender = cboGender.Text,
                    Phone = phone,
                    Address = txtAddress.Text.Trim(),
                    Hometown = txtHometown.Text.Trim(),
                    Email = email,
                    Picture = studentImage,
                    MaNganh = cboMaNganh.SelectedValue.ToString(),
                    MaLop = cboMaLop.SelectedValue.ToString()
                };

                // Đẩy thực thi trực tiếp xuống Database thông qua khối try-catch bọc lỗi ngoại lệ bên dưới.
                if (string.IsNullOrEmpty(targetMssv))
                {
                    if (studentRepo.AddStudent(sv))
                    {
                        MessageBox.Show("Thêm mới hồ sơ sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    if (studentRepo.UpdateStudent(sv))
                    {
                        MessageBox.Show("Cập nhật thông tin hồ sơ sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Bắt chính xác thông điệp lỗi trùng MSSV hoặc trùng Email được đẩy ra từ tầng Engine SQL của StudentRepository
                MessageBox.Show(ex.Message, "Ràng Buộc Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                MessageBox.Show("Mất kết nối đột ngột với SQL Server Engine! Dữ liệu chưa được ghi nhận bảo an.", "Lỗi Hệ Thống Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ngoài dự phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isSaving = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            picStudent.Image?.Dispose();
            this.Close();
        }
    }
}