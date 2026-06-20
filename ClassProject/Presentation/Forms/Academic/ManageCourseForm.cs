using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;

namespace ClassProject.Presentation.Forms.Course
{
    public partial class ManageCourseForm : Form
    {
        private readonly My_DB db = new My_DB();
        private readonly CourseRepository _courseRepo;

        public ManageCourseForm()
        {
            InitializeComponent();

            _courseRepo = new CourseRepository();

            ConfigCustomUI();
        }

        private void ManageCourseForm_Load(object sender, EventArgs e)
        {
            // 🌟 CHỐT CHẶN BẢO MẬT TẦNG 1: Chỉ Admin (Role 0) hoặc Giáo vụ/HR (Role 2) mới có quyền quản lý danh mục môn học
            if (!UserSession.IsLoggedIn || (!UserSession.IsAdmin && !UserSession.IsStaff))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chức năng cấu hình danh mục môn học gốc chỉ dành cho Ban quản trị hoặc phòng Giáo vụ/HR.",
                                "Cảnh Báo An Ninh", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Đóng form an toàn tránh xung đột luồng UI khi Form đang load
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            // Phòng thủ kiểm soát độ dài chuỗi nhập liệu tránh tràn bộ đệm (Data Overflow)
            txtCourseID.MaxLength = 20;   // Khớp CHAR/VARCHAR trong DB
            txtCourseName.MaxLength = 100; // Khớp NVARCHAR trong DB
            if (txtSearch != null) txtSearch.MaxLength = 100;

            InitSemesterComboBox();
            LoadCourseData();
            ResetFields(); // Đưa form về trạng thái mặc định ban đầu
        }

        // Cấu hình giao diện nâng cao trực tiếp bằng mã nguồn (Modern Flat)
        private void ConfigCustomUI()
        {
            this.BackColor = Color.FromArgb(246, 248, 251);

            // Tinh chỉnh lưới hiển thị Guna2DataGridView
            dgvCourses.AllowUserToAddRows = false;
            dgvCourses.RowHeadersVisible = false;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            dgvCourses.GridColor = Color.FromArgb(230, 235, 245);

            // Cấu hình thanh Header của bảng dữ liệu
            dgvCourses.ColumnHeadersHeight = 38;
            dgvCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 50, 135);
            dgvCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            // Cấu hình các dòng bản ghi
            dgvCourses.RowTemplate.Height = 32;
            dgvCourses.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvCourses.DefaultCellStyle.ForeColor = Color.FromArgb(60, 65, 75);

            // Gán sự kiện thời gian thực cho ô Tìm kiếm và Chặn nhập chữ ô Số tuần
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtWeeks.KeyPress += txtWeeks_KeyPress;
        }

        private void InitSemesterComboBox()
        {
            cboSemester.Items.Clear();
            cboSemester.Items.Add("Học kỳ 1");
            cboSemester.Items.Add("Học kỳ 2");
            cboSemester.Items.Add("Học kỳ 3");
            cboSemester.SelectedIndex = 0;
        }

        // Đổ dữ liệu lên Grid thông qua Repository (Đồng bộ các cột)
        private void LoadCourseData()
        {
            try
            {
                DataTable dt = _courseRepo.GetCourses();
                dgvCourses.DataSource = dt;

                // Đổi tên cột hiển thị thân thiện trên Grid
                if (dgvCourses.Columns["MaMH"] != null) dgvCourses.Columns["MaMH"].HeaderText = "Mã Môn";
                if (dgvCourses.Columns["TenMH"] != null) dgvCourses.Columns["TenMH"].HeaderText = "Tên Môn Học";
                if (dgvCourses.Columns["SoTC"] != null) dgvCourses.Columns["SoTC"].HeaderText = "Số Tín Chỉ";
                if (dgvCourses.Columns["Tuan"] != null) dgvCourses.Columns["Tuan"].HeaderText = "Số Tuần";
                if (dgvCourses.Columns["Hky"] != null) dgvCourses.Columns["Hky"].HeaderText = "Học Kỳ";
                if (dgvCourses.Columns["NamHoc"] != null) dgvCourses.Columns["NamHoc"].HeaderText = "Năm Học";
                if (dgvCourses.Columns["Mota"] != null) dgvCourses.Columns["Mota"].HeaderText = "Mô Tả";

                // Định độ rộng cột hiển thị thông minh
                if (dgvCourses.Columns["MaMH"] != null) dgvCourses.Columns["MaMH"].Width = 90;
                if (dgvCourses.Columns["SoTC"] != null) dgvCourses.Columns["SoTC"].Width = 90;
                if (dgvCourses.Columns["Hky"] != null) dgvCourses.Columns["Hky"].Width = 80;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hệ thống không thể truy xuất dữ liệu môn học: {ex.Message}", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Đẩy ngược dữ liệu từ dòng được chọn lên các Control nhập liệu
        private void dgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCourses.Rows[e.RowIndex];

                txtCourseID.Text = row.Cells["MaMH"].Value?.ToString()?.Trim() ?? "";
                txtCourseName.Text = row.Cells["TenMH"].Value?.ToString() ?? "";

                numCredits.Value = row.Cells["SoTC"].Value != DBNull.Value ? Convert.ToDecimal(row.Cells["SoTC"].Value) : 3;
                txtWeeks.Text = row.Cells["Tuan"].Value?.ToString() ?? "15";
                txtDescription.Text = row.Cells["Mota"].Value?.ToString() ?? "";

                string hkyValue = row.Cells["Hky"].Value?.ToString() ?? "1";
                if (hkyValue == "2") cboSemester.SelectedIndex = 1;
                else if (hkyValue == "3") cboSemester.SelectedIndex = 2;
                else cboSemester.SelectedIndex = 0;

                // Quản lý trạng thái điều khiển
                txtCourseID.ReadOnly = true;
                btnAdd.Enabled = false;
                btnEdit.Enabled = true;

                // 🌟 QUẢN LÝ QUYỀN NÚT XÓA: Chỉ Admin mới được mở nút Xóa, Giáo vụ (HR) bị khóa nút
                if (UserSession.IsAdmin)
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }
            }
        }

        // Nghiệp vụ THÊM MÔN HỌC MỚI (Admin & Giáo vụ đều thực hiện được)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseID.Text) || string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin Mã môn và Tên môn học!", "Dữ Liệu Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int semesterInt = 1;
                if (cboSemester.Text.Contains("2")) semesterInt = 2;
                else if (cboSemester.Text.Contains("3")) semesterInt = 3;

                var newCourse = new ClassProject.DataAccess.Entities.Course
                {
                    MaMH = txtCourseID.Text.Trim().ToUpper(), // Tự động chuẩn hóa viết hoa mã học phần
                    TenMH = txtCourseName.Text.Trim(),
                    SoTC = (int)numCredits.Value,
                    Tuan = string.IsNullOrWhiteSpace(txtWeeks.Text) ? 15 : Convert.ToInt32(txtWeeks.Text.Trim()),
                    Hky = semesterInt,
                    NamHoc = "2026-2027",
                    Mota = txtDescription.Text.Trim()
                };

                if (_courseRepo.AddCourse(newCourse))
                {
                    MessageBox.Show("Tạo mới học phần gốc vào danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFields();
                    LoadCourseData();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm dữ liệu: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nghiệp vụ CẬP NHẬT THÔNG TIN MÔN HỌC (Admin & Giáo vụ đều thực hiện được)
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!txtCourseID.ReadOnly)
            {
                MessageBox.Show("Vui lòng chọn một môn học cụ thể từ danh sách bên phải để hiệu chỉnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int semesterInt = 1;
                if (cboSemester.Text.Contains("2")) semesterInt = 2;
                else if (cboSemester.Text.Contains("3")) semesterInt = 3;

                var updatedCourse = new ClassProject.DataAccess.Entities.Course
                {
                    MaMH = txtCourseID.Text.Trim(),
                    TenMH = txtCourseName.Text.Trim(),
                    SoTC = (int)numCredits.Value,
                    Tuan = string.IsNullOrWhiteSpace(txtWeeks.Text) ? 15 : Convert.ToInt32(txtWeeks.Text.Trim()),
                    Hky = semesterInt,
                    NamHoc = "2026-2027",
                    Mota = txtDescription.Text.Trim()
                };

                if (_courseRepo.UpdateCourse(updatedCourse))
                {
                    MessageBox.Show("Cập nhật thông tin học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFields();
                    LoadCourseData();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật cấu trúc: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nghiệp vụ XÓA MÔN HỌC (Chỉ có quyền Admin)
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 🌟 CHỐT CHẶN BẢO MẬT VÒNG 2: Ngăn chặn triệt để hành vi cố tình bypass nút bấm
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("Quyền hạn bị từ chối! Tài khoản thuộc vai trò Giáo vụ/HR không được phép hủy hoặc xóa môn học gốc khỏi hệ thống.",
                                "Hạn Chế Thẩm Quyền", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCourseID.Text)) return;

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn môn học [{txtCourseID.Text.Trim()}] ra khỏi danh mục đào tạo không?\nHành động này chỉ thành công nếu môn chưa mở bất kỳ lớp học phần nào.",
                "Hành động nguy hiểm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    if (_courseRepo.DeleteCourse(txtCourseID.Text.Trim()))
                    {
                        MessageBox.Show("Hệ thống đã loại bỏ môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFields();
                        LoadCourseData();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Từ chối hành động", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi thực thi xóa bản ghi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvCourses.DataSource = _courseRepo.SearchCourses(txtSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tìm kiếm môn học: " + ex.Message);
            }
        }

        private void ResetFields()
        {
            txtCourseID.Text = "";
            txtCourseName.Text = "";
            numCredits.Value = 3;
            txtWeeks.Text = "15";
            cboSemester.SelectedIndex = 0;
            txtDescription.Text = "";

            txtCourseID.ReadOnly = false;
            dgvCourses.ClearSelection();

            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false; // Trạng thái mặc định luôn tắt nút xóa cho đến khi chọn dòng và thỏa mãn vai trò Admin
        }

        private void btnClear_Click(object sender, EventArgs e) => ResetFields();

        private void txtWeeks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboSearchHK_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSemester.SelectedIndex >= 0)
            {
                try
                {
                    int selectedSemester = cboSearchHK.SelectedIndex + 1;
                    dgvCourses.DataSource = _courseRepo.FilterCoursesBySemester(selectedSemester);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi lọc: {ex.Message}");
                }
            }
        }
    }
}