using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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

        // Định nghĩa hằng số quy chuẩn cấu trúc miền cho Sinh viên
        private const string STUDENT_EMAIL_SUBDOMAIN = "@student.";
        private const string REQUIRED_EMAIL_SUFFIX = ".edu.vn";

        public AddStudentForm(string mssv)
        {
            InitializeComponent();
            targetMssv = mssv;
            studentRepo = new StudentRepository();

            // ⚡ Đăng ký sự kiện tự động sinh Email theo MSSV khi người dùng nhập liệu
            txtMSSV.TextChanged += AutoGenerateStudentEmail_TextChanged;
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            isDataLoading = true;
            LoadMajorComboBox();

            cboMaNganh.SelectedIndexChanged -= CboMaNganh_SelectedIndexChanged;
            cboMaNganh.SelectedIndexChanged += CboMaNganh_SelectedIndexChanged;

            LoadClassroomComboBoxFiltered();
            isDataLoading = false;

            // CHẾ ĐỘ CHỈNH SỬA (EDIT)
            if (!string.IsNullOrEmpty(targetMssv))
            {
                // Tạm hủy bắt sự kiện sinh tự động để không ghi đè dữ liệu cũ từ DB
                txtMSSV.TextChanged -= AutoGenerateStudentEmail_TextChanged;

                this.Text = "Chỉnh sửa thông tin sinh viên";
                btnSave.Text = "Cập nhật";
                txtMSSV.Text = targetMssv;
                txtMSSV.ReadOnly = true;
                txtMSSV.BackColor = SystemColors.InactiveCaption;

                LoadStudentDataForEdit();
            }
        }

        #region 💡 TỰ ĐỘNG SINH EMAIL SINH VIÊN CHUẨN ĐỊNH DẠNG
        private void AutoGenerateStudentEmail_TextChanged(object sender, EventArgs e)
        {
            // Chỉ tự động gợi ý điền khi thêm mới
            if (string.IsNullOrEmpty(targetMssv))
            {
                string mssvRaw = txtMSSV.Text.Trim();
                if (string.IsNullOrWhiteSpace(mssvRaw))
                {
                    txtEmail.Clear();
                    return;
                }

                // Tự sinh cấu trúc: [mssv]@student.school.edu.vn (Sinh viên có thể tùy biến subdomain 'school')
                txtEmail.Text = mssvRaw.ToLower() + STUDENT_EMAIL_SUBDOMAIN + "school" + REQUIRED_EMAIL_SUFFIX;
            }
        }
        #endregion

        private void LoadMajorComboBox()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục Ngành ngoài dự kiến: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = "SELECT MaLop, TenLop FROM dbo.Classroom WHERE MaNganh = @MaNganh ORDER BY TenLop ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@MaNganh", SqlDbType.Char, 10) { Value = selectedMajor });
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục Lớp ngoài dự kiến: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtFirstName.Text = sv.FirstName?.Trim() ?? "";
                    txtLastName.Text = sv.LastName?.Trim() ?? "";

                    if (sv.DateOfBirth.HasValue)
                    {
                        if (sv.DateOfBirth.Value >= dtpDateOfBirth.MinDate && sv.DateOfBirth.Value <= dtpDateOfBirth.MaxDate)
                            dtpDateOfBirth.Value = sv.DateOfBirth.Value;
                    }

                    cboGender.Text = string.IsNullOrWhiteSpace(sv.Gender) ? "Nam" : sv.Gender.Trim();
                    txtPhone.Text = sv.Phone?.Trim() ?? "";
                    txtAddress.Text = sv.Address?.Trim() ?? "";
                    txtHometown.Text = sv.Hometown?.Trim() ?? "";
                    txtEmail.Text = sv.Email?.Trim() ?? "";

                    if (sv.UserId != null)
                    {
                        txtEmail.ReadOnly = true;
                        txtEmail.BackColor = SystemColors.InactiveCaption;

                        ToolTip toolTip = new ToolTip();
                        toolTip.SetToolTip(txtEmail, "Trường dữ liệu này được cố định do tài khoản đang hoạt động.");
                    }

                    if (!string.IsNullOrEmpty(sv.MaNganh))
                    {
                        cboMaNganh.SelectedValue = sv.MaNganh;
                        LoadClassroomComboBoxFiltered();
                    }
                    if (!string.IsNullOrEmpty(sv.MaLop)) cboMaLop.SelectedValue = sv.MaLop;

                    if (sv.Picture != null && sv.Picture.Length > 0)
                    {
                        studentImage = sv.Picture;
                        using (MemoryStream ms = new MemoryStream(studentImage))
                        {
                            Image oldImg = picStudent.Image;
                            picStudent.Image = Image.FromStream(ms);
                            oldImg?.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị hồ sơ hệ thống: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";
                ofd.Title = "Chọn ảnh đại diện sinh viên";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo fInfo = new FileInfo(ofd.FileName);
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
                                Image oldImg = picStudent.Image;
                                picStudent.Image = new Bitmap(testImg);
                                oldImg?.Dispose();

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
                        Image oldImg = picStudent.Image;
                        picStudent.Image = null;
                        oldImg?.Dispose();
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
                string email = txtEmail.Text.Trim().ToLower(); // Chuẩn hóa chữ thường
                string phone = txtPhone.Text.Trim();

                // [Các bước Validate 1, 2, 3, 4 giữ nguyên...]
                if (!Regex.IsMatch(mssv, @"^[A-Za-z0-9]{6,20}$"))
                {
                    MessageBox.Show("MSSV không hợp lệ! Độ dài yêu cầu từ 6-20 ký tự, không chứa khoảng trắng hoặc ký tự đặc biệt.", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMSSV.Focus(); return;
                }
                if (string.IsNullOrEmpty(lastName) || lastName.Length > 100)
                {
                    MessageBox.Show("Họ đệm không được trống và không vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLastName.Focus(); return;
                }
                if (string.IsNullOrEmpty(firstName) || firstName.Length > 100)
                {
                    MessageBox.Show("Tên sinh viên không được trống và không vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirstName.Focus(); return;
                }
                if (dtpDateOfBirth.Value.Date > DateTime.Today || dtpDateOfBirth.Value.Year < 1900)
                {
                    MessageBox.Show("Ngày sinh không hợp lệ!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || email.Length > 100)
                {
                    MessageBox.Show("Địa chỉ Email không đúng định dạng tiêu chuẩn!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus(); return;
                }
                if (!email.StartsWith(mssv, StringComparison.OrdinalIgnoreCase) ||
                    !email.Contains(STUDENT_EMAIL_SUBDOMAIN, StringComparison.OrdinalIgnoreCase) ||
                    !email.EndsWith(REQUIRED_EMAIL_SUFFIX, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Email sinh viên không khớp quy chuẩn quản lý!\n\nCấu trúc bắt buộc: {mssv.ToLower()}{STUDENT_EMAIL_SUBDOMAIN}[tên_miền]{REQUIRED_EMAIL_SUFFIX}", "Xác thực Giáo vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus(); return;
                }
                if (!Regex.IsMatch(phone, @"^(03|05|07|08|09)\d{8}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! Yêu cầu sử dụng đầu số di động Việt Nam chuẩn và đủ 10 chữ số.", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus(); return;
                }
                if (cboMaNganh.SelectedValue == null || cboMaLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Chuyên ngành và Lớp hành chính!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ==========================================
                // ✨ ĐÃ THÊM: PASS ALL TEST CASES - CHECK TRÙNG TRƯỚC KHI XUỐNG DB
                // ==========================================
                if (string.IsNullOrEmpty(targetMssv)) // Chế độ THÊM MỚI
                {
                    // 1. Kiểm tra trùng MSSV bằng hàm có sẵn hoặc gọi truy vấn nhanh
                    if (studentRepo.GetStudentByMssv(mssv) != null)
                    {
                        MessageBox.Show($"Mã số sinh viên '{mssv}' đã tồn tại trên hệ thống. Không thể thêm trùng lặp!",
                                        "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMSSV.Focus();
                        return;
                    }

                    // 2. Kiểm tra trùng Email để triệt tiêu lỗi UNIQUE KEY 'UQ_Students_Email'
                    // (Giả định trong studentRepo của bạn đã có hàm CheckEmailExists, nếu chưa có, xem hướng dẫn viết nhanh ở dưới)
                    if (IsEmailAlreadyExists(email, string.Empty))
                    {
                        MessageBox.Show($"Địa chỉ Email hệ thống '{email}' đã được cấp phát cho một sinh viên khác trước đó!\n\nVui lòng kiểm tra lại danh sách hoặc dọn dẹp tài khoản rác.",
                                        "Trùng lặp Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtEmail.Focus();
                        return;
                    }
                }
                else // Chế độ CẬP NHẬT (EDIT)
                {
                    // Nếu sửa thông tin mà đổi email (nếu được phép), phải chắc chắn email mới không đụng hàng với ai khác
                    if (IsEmailAlreadyExists(email, targetMssv))
                    {
                        MessageBox.Show($"Không thể cập nhật! Email '{email}' đang được sử dụng bởi một sinh viên khác.",
                                        "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtEmail.Focus();
                        return;
                    }
                }

                // Đóng gói Model dữ liệu
                Student sv = new Student
                {
                    Mssv = mssv,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dtpDateOfBirth.Value,
                    Gender = string.IsNullOrWhiteSpace(cboGender.Text) ? "Nam" : cboGender.Text.Trim(),
                    Phone = phone,
                    Address = txtAddress.Text.Trim(),
                    Hometown = txtHometown.Text.Trim(),
                    Email = email,
                    Picture = studentImage,
                    MaNganh = cboMaNganh.SelectedValue.ToString(),
                    MaLop = cboMaLop.SelectedValue.ToString()
                };

                // Thực thi xuống DB
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
                MessageBox.Show(ex.Message, "Ràng Buộc Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                // Bẫy phòng hờ tầng cuối nếu tầng kiểm tra trên RAM lọt lưới do hạ tầng mạng chậm trễ (Race Condition)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Lỗi đồng bộ: Email hoặc Mã định danh này vừa mới được đăng ký ngầm bởi một phiên làm việc khác!", "Dữ liệu trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Mất kết nối đột ngột với SQL Server Engine!", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            Image oldImg = picStudent.Image;
            picStudent.Image = null;
            oldImg?.Dispose();
            this.Close();
        }
        private bool IsEmailAlreadyExists(string email, string currentMssv)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // Nếu ở chế độ Edit (currentMssv có giá trị), ta loại trừ chính nó ra khỏi danh sách kiểm tra trùng
                    string query = "SELECT COUNT(1) FROM dbo.Students WHERE Email = @Email";
                    if (!string.IsNullOrEmpty(currentMssv))
                    {
                        query += " AND MSSV != @CurrentMssv";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        if (!string.IsNullOrEmpty(currentMssv))
                        {
                            cmd.Parameters.AddWithValue("@CurrentMssv", currentMssv);
                        }

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch { return false; } // Phục hồi an toàn mặc định
        }
    }
}