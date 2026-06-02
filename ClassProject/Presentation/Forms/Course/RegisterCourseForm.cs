using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Course
{
    public partial class ClassroomForm : Form
    {
        private RegisterRepository _registerRepo;
        private StudentRepository _studentRepo;
        private ScoreRepository _scoreRepo;
        private My_DB _db = new My_DB();

        private bool _isBindingCombo = false;

        public object DataGridViewColumnAutoSizeMode { get; private set; }

        public ClassroomForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;
            _registerRepo = new RegisterRepository(connString);
            _studentRepo = new StudentRepository(connString);
            _scoreRepo = new ScoreRepository(connString);
        }

        private void RegisterCourseForm_Load(object sender, EventArgs e)
        {
            _isBindingCombo = true;
            LoadStudentCombo();
            LoadCourseCombo();
            _isBindingCombo = false;

            LoadGridData();
            UpdateQuickStats();
        }

        private void LoadStudentCombo()
        {
            try
            {
                DataTable dt = _studentRepo.SearchStudents("", "Tất cả");

                if (dt.Columns.Contains("Mssv") && dt.Columns.Contains("FirstName"))
                {
                    dt.Columns.Add("FullNameWithId", typeof(string), "Mssv + ' - ' + LastName + ' ' + FirstName");
                }
                else if (dt.Columns.Contains("MSSV") && dt.Columns.Contains("FirstName"))
                {
                    dt.Columns.Add("FullNameWithId", typeof(string), "MSSV + ' - ' + LastName + ' ' + FirstName");
                }

                cboStudent.DataSource = dt;
                cboStudent.DisplayMember = "FullNameWithId";
                cboStudent.ValueMember = dt.Columns.Contains("Mssv") ? "Mssv" : "MSSV";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCourseCombo()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = "SELECT MaMH, TenMH, (MaMH + ' - ' + TenMH) AS DisplayText FROM dbo.Course";
                    using (Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        cboCourse.DataSource = dt;
                        cboCourse.DisplayMember = "DisplayText";
                        cboCourse.ValueMember = "MaMH";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGridData()
        {
            // Nếu ComboBox đang nạp data hoặc chưa chọn ai thì xóa sạch bảng, tránh lỗi
            if (_isBindingCombo || cboStudent.SelectedValue == null)
            {
                dgvRegisterCourse.DataSource = null;
                return;
            }

            string mssv = cboStudent.SelectedValue.ToString();
            if (mssv.Contains("System.Data.DataRowView")) return;

            // 1. Lấy dữ liệu chuẩn từ tầng Repository đã sửa ở Bước 1
            DataTable dt = _registerRepo.GetRegistrationList(mssv);

            // 2. Reset làm sạch lưới cũ để nạp cấu trúc mới không bị chồng chéo
            dgvRegisterCourse.DataSource = null;
            dgvRegisterCourse.Columns.Clear();

            // 3. Đổ dữ liệu mới vào lưới
            dgvRegisterCourse.AutoGenerateColumns = true;
            dgvRegisterCourse.DataSource = dt;

            // 4. Bật thanh tiêu đề và đặt tên Tiếng Việt hiển thị theo đúng yêu cầu
            dgvRegisterCourse.ColumnHeadersVisible = true;
            dgvRegisterCourse.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            if (dgvRegisterCourse.Columns.Count > 0)
            {
                if (dgvRegisterCourse.Columns.Contains("STT")) dgvRegisterCourse.Columns["STT"].HeaderText = "STT";
                if (dgvRegisterCourse.Columns.Contains("CourseId")) dgvRegisterCourse.Columns["CourseId"].HeaderText = "Mã Môn Học";
                if (dgvRegisterCourse.Columns.Contains("CourseName")) dgvRegisterCourse.Columns["CourseName"].HeaderText = "Tên Môn Học";
                if (dgvRegisterCourse.Columns.Contains("Credits")) dgvRegisterCourse.Columns["Credits"].HeaderText = "Số Tín Chỉ";
                if (dgvRegisterCourse.Columns.Contains("Teacher")) dgvRegisterCourse.Columns["Teacher"].HeaderText = "Giảng Viên";
                if (dgvRegisterCourse.Columns.Contains("Semester")) dgvRegisterCourse.Columns["Semester"].HeaderText = "Học Kỳ";

                if (dgvRegisterCourse.Columns.Contains("STT"))
                {
                    dgvRegisterCourse.Columns["STT"].Width = 50;
                }

                dgvRegisterCourse.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvRegisterCourse.AllowUserToAddRows = false;
                dgvRegisterCourse.ReadOnly = true;
                dgvRegisterCourse.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        private void UpdateQuickStats()
        {
            if (_isBindingCombo || cboStudent.SelectedValue == null) return;

            string mssv = cboStudent.SelectedValue.ToString();
            if (mssv.Contains("System.Data.DataRowView")) return;

            int totalCourses = _registerRepo.GetTotalCoursesRegistered(mssv);
            int totalCredits = _registerRepo.GetTotalCreditsRegistered(mssv);

            lblTotalCourses.Text = $"{totalCourses}";
            lblTotalCredits.Text = $"{totalCredits}";
        }

        private void cboStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGridData();
            UpdateQuickStats();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Sinh viên và Môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mssv = cboStudent.SelectedValue.ToString();
            string courseId = cboCourse.SelectedValue.ToString();

            int currentCredits = _registerRepo.GetTotalCreditsRegistered(mssv);
            if (currentCredits + 3 > 24)
            {
                MessageBox.Show($"Không thể đăng ký! Sinh viên đã đăng ký {currentCredits} tín chỉ. Đăng ký thêm môn này sẽ vượt quá giới hạn 24 tín chỉ quy định!", "Cảnh báo học vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_registerRepo.IsRegistered(mssv, courseId))
            {
                MessageBox.Show("Sinh viên này đã đăng ký môn học này rồi!", "Trùng lịch học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_registerRepo.AddRegistration(mssv, courseId))
            {
                MessageBox.Show("Đăng ký môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGridData();
                UpdateQuickStats();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại! Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelRegister_Click(object sender, EventArgs e)
        {
            string mssv = "";
            string courseId = "";

            if (cboStudent.SelectedValue != null)
            {
                mssv = cboStudent.SelectedValue.ToString();
            }

            if (dgvRegisterCourse.CurrentRow != null)
            {
                // Kiểm tra nếu lấy dữ liệu từ dòng đang chọn thông qua cột CourseId mới cấu hình
                if (dgvRegisterCourse.CurrentRow.Cells["CourseId"].Value != null)
                {
                    courseId = dgvRegisterCourse.CurrentRow.Cells["CourseId"].Value.ToString();
                }
            }
            else if (cboCourse.SelectedValue != null)
            {
                courseId = cboCourse.SelectedValue.ToString();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bản ghi trên bảng hoặc ComboBox để tiến hành hủy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_scoreRepo.HasCourseScore(mssv, courseId))
            {
                MessageBox.Show("Môn học này đã được giảng viên vào điểm số thành phần! Bạn không thể thực hiện hủy đăng ký lớp học này.", "Hệ thống khóa học phần", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn HỦY đăng ký môn học [{courseId}] của sinh viên [{mssv}] không?",
                "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (_registerRepo.CancelRegistration(mssv, courseId))
                {
                    MessageBox.Show("Hủy đăng ký môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGridData();
                    UpdateQuickStats();
                }
                else
                {
                    MessageBox.Show("Hủy đăng ký môn học thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadGridData();
            UpdateQuickStats();
        }
    }
}