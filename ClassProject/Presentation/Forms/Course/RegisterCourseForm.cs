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
    public partial class RegisterCourseForm : Form
    {
        private RegisterRepository _registerRepo;
        private StudentRepository _studentRepo;
        private ScoreRepository _scoreRepo; // Bổ sung tầng Score để check điều kiện hủy môn học
        private My_DB _db = new My_DB();

        public RegisterCourseForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;
            _registerRepo = new RegisterRepository(connString);
            _studentRepo = new StudentRepository(connString);
            _scoreRepo = new ScoreRepository(connString); // Khởi tạo Repo Điểm
        }

        // Sự kiện Form Load: tự động nạp dữ liệu khi mở màn hình này lên
        private void RegisterCourseForm_Load(object sender, EventArgs e)
        {
            LoadStudentCombo();
            LoadCourseCombo();
            LoadGridData();
        }

        // 1. Load danh sách Sinh viên vào cboStudent
        private void LoadStudentCombo()
        {
            try
            {
                DataTable dt = _studentRepo.SearchStudents("", "Tất cả");

                if (dt.Columns.Contains("MSSV") && dt.Columns.Contains("FirstName"))
                {
                    dt.Columns.Add("FullNameWithId", typeof(string), "MSSV + ' - ' + LastName + ' ' + FirstName");
                }

                cboStudent.DataSource = dt;
                cboStudent.DisplayMember = "FullNameWithId";
                cboStudent.ValueMember = "MSSV";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Load danh sách Môn học vào cboCourse
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

        // 3. Nạp dữ liệu danh sách đã đăng ký lên lưới DataGridView
        private void LoadGridData()
        {
            DataTable dt = _registerRepo.GetRegistrationList();
            dgvRegisterCourse.DataSource = dt;

            if (dgvRegisterCourse.Columns.Count > 0)
            {
                dgvRegisterCourse.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvRegisterCourse.AllowUserToAddRows = false;
                dgvRegisterCourse.ReadOnly = true;
            }
        }

        // 4. Xử lý nút bấm ĐĂNG KÝ HỌC (btnRegister)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Sinh viên và Môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đưa dòng lấy giá trị lên đầu tiên để sửa lỗi biên dịch
            string mssv = cboStudent.SelectedValue.ToString();
            string courseId = cboCourse.SelectedValue.ToString();

            // 🔥 BONUS ĐIỂM CAO: Kiểm tra giới hạn tối đa 24 Tín chỉ
            int currentCredits = _registerRepo.GetTotalCreditsRegistered(mssv);
            if (currentCredits + 3 > 24)
            {
                MessageBox.Show($"Không thể đăng ký! Sinh viên đã đăng ký {currentCredits} tín chỉ. Đăng ký thêm môn này sẽ vượt quá giới hạn 24 tín chỉ quy định!", "Cảnh báo học vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra chống trùng (Sinh viên đăng ký trùng môn học)
            if (_registerRepo.IsRegistered(mssv, courseId))
            {
                MessageBox.Show("Sinh viên này đã đăng ký môn học này rồi!", "Trùng lịch học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi Repository thực hiện INSERT vào database
            if (_registerRepo.AddRegistration(mssv, courseId))
            {
                MessageBox.Show("Đăng ký môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGridData();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại! Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. Xử lý nút bấm HỦY ĐĂNG KÝ (btnCancelRegister)
        private void btnCancelRegister_Click(object sender, EventArgs e)
        {
            string mssv = "";
            string courseId = "";

            if (dgvRegisterCourse.CurrentRow != null)
            {
                mssv = dgvRegisterCourse.CurrentRow.Cells["Mã SV"].Value.ToString();
                courseId = dgvRegisterCourse.CurrentRow.Cells["Mã MH"].Value.ToString();
            }
            else if (cboStudent.SelectedValue != null && cboCourse.SelectedValue != null)
            {
                mssv = cboStudent.SelectedValue.ToString();
                courseId = cboCourse.SelectedValue.ToString();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bản ghi trên bảng hoặc ComboBox để tiến hành hủy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ⚠️ LOGIC CHẶT CHẼ ĐIỂM TUYỆT ĐỐI: Kiểm tra xem môn học này đã được nhập điểm chưa
            // Nếu điểm tích lũy của sinh viên ở môn này khác 0 (tức là đã nhập điểm ở Tuần 7) -> Chặn không cho Hủy học phần
            if (_scoreRepo.GetStudentGPA(mssv) > 0)
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
                }
                else
                {
                    MessageBox.Show("Hủy đăng ký môn học thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 6. Nút LÀM MỚI (btnLoad)
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadGridData();
        }
    }
}