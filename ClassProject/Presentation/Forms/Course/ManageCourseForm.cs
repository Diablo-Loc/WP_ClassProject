using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ClassProject.DataAccess.Db; // Định vị không gian tên chứa thực thể cơ sở dữ liệu My_DB

namespace ClassProject.Presentation.Forms.Course
{
    public partial class ManageCourseForm : Form
    {
        private readonly My_DB db = new My_DB();

        public ManageCourseForm()
        {
            InitializeComponent();
            ConfigCustomUI();
        }

        private void ManageCourseForm_Load(object sender, EventArgs e)
        {
            InitSemesterComboBox();
            LoadCourseData();
        }

        /// Cấu hình các thuộc tính giao diện nâng cao trực tiếp bằng mã nguồn để đảm bảo độ mịn UI/UX hiện đại
        private void ConfigCustomUI()
        {
            this.BackColor = Color.FromArgb(246, 248, 251); // Màu nền chuẩn hệ thống

            // Tinh chỉnh lưới hiển thị Guna2DataGridView sang thiết kế Modern Flat
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
        }

        /// Khởi tạo dữ liệu mẫu cho ComboBox Học kỳ
        private void InitSemesterComboBox()
        {
            cboSemester.Items.Clear();
            cboSemester.Items.Add("Học kỳ 1");
            cboSemester.Items.Add("Học kỳ 2");
            cboSemester.Items.Add("Học kỳ 3");
            cboSemester.SelectedIndex = 0;
        }

        /// Đổ dữ liệu danh sách học phần gốc lên lưới hiển thị (UC-12) - KHỚP 100% CSDL SCRIPT
        private void LoadCourseData()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    // Đã sửa chính xác tên cột theo SQL: MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota
                    string query = "SELECT MaMH AS [Mã Môn], TenMH AS [Tên Môn Học], SoTC AS [Số Tín Chỉ], Tuan AS [Số Tuần], Hky AS [Học Kỳ], NamHoc AS [Năm Học], Mota AS [Mô Tả] FROM Course";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvCourses.DataSource = dt;

                    // Định độ rộng cột hiển thị thông minh
                    if (dgvCourses.Columns["Mã Môn"] != null) dgvCourses.Columns["Mã Môn"].Width = 90;
                    if (dgvCourses.Columns["Số Tín Chỉ"] != null) dgvCourses.Columns["Số Tín Chỉ"].Width = 90;
                    if (dgvCourses.Columns["Học Kỳ"] != null) dgvCourses.Columns["Học Kỳ"].Width = 80;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hệ thống không thể truy xuất dữ liệu: {ex.Message}", "Lỗi cấu trúc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Đẩy ngược dữ liệu từ dòng được chọn ở bảng trắng lên panel cấu hình nhập liệu bên trái
        private void dgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCourses.Rows[e.RowIndex];

                txtCourseID.Text = row.Cells["Mã Môn"].Value.ToString();
                txtCourseName.Text = row.Cells["Tên Môn Học"].Value.ToString();
                numCredits.Value = Convert.ToDecimal(row.Cells["Số Tín Chỉ"].Value);
                txtWeeks.Text = row.Cells["Số Tuần"].Value.ToString();
                txtDescription.Text = row.Cells["Mô Tả"].Value.ToString();

                // Đồng bộ Năm học nếu form giao diện có TextBox riêng, nếu không có bạn có thể bỏ qua dòng dưới
                // txtAcademicYear.Text = row.Cells["Năm Học"].Value.ToString();

                // Chuyển đổi giá trị số INT (1,2,3) từ CSDL ngược lại thành text ComboBox
                string hkyValue = row.Cells["Học Kỳ"].Value.ToString();
                if (hkyValue == "2") cboSemester.SelectedIndex = 1;
                else if (hkyValue == "3") cboSemester.SelectedIndex = 2;
                else cboSemester.SelectedIndex = 0;

                txtCourseID.ReadOnly = true; // Khóa trường khóa chính tránh xung đột dữ liệu toàn vẹn
            }
        }

        /// Xử lý nghiệp vụ THÊM MÔN HỌC MỚI (UC-10) - KHỚP 100% CSDL SCRIPT
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseID.Text) || string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các thông tin bắt buộc (Mã môn và Tên môn)!", "Cảnh báo nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    // Lệnh SQL chèn dữ liệu chuẩn hoá theo cấu trúc thực tế bảng Course
                    string query = "INSERT INTO Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota) VALUES (@MaMH, @TenMH, @SoTC, @Tuan, @Hky, @NamHoc, @Mota)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaMH", txtCourseID.Text.Trim().ToUpper());
                    cmd.Parameters.AddWithValue("@TenMH", txtCourseName.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTC", (int)numCredits.Value);
                    cmd.Parameters.AddWithValue("@Tuan", string.IsNullOrWhiteSpace(txtWeeks.Text) ? 15 : Convert.ToInt32(txtWeeks.Text.Trim()));

                    // Logic bóc tách chuỗi chữ từ ComboBox (VD: "Học kỳ 2") thành số nguyên INT (2) để đẩy vào DB
                    int semesterInt = 1;
                    if (cboSemester.Text.Contains("2")) semesterInt = 2;
                    else if (cboSemester.Text.Contains("3")) semesterInt = 3;
                    cmd.Parameters.AddWithValue("@Hky", semesterInt);

                    // Gán năm học mặc định khớp theo script (2026-2027) hoặc từ ô nhập liệu của bạn
                    cmd.Parameters.AddWithValue("@NamHoc", "2026-2027");
                    cmd.Parameters.AddWithValue("@Mota", txtDescription.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Tạo mới học phần gốc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetFields();
                    LoadCourseData();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Lỗi trùng lặp dữ liệu khóa chính Primary Key trong SQL
            {
                MessageBox.Show("Mã môn học này đã tồn tại trên hệ thống dữ liệu!", "Xung đột khóa chính", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm dữ liệu: {ex.Message}", "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Xử lý nghiệp vụ SỬA THÔNG TIN MÔN HỌC GỐC (UC-11) - KHỚP 100% CSDL SCRIPT
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!txtCourseID.ReadOnly)
            {
                MessageBox.Show("Vui lòng chọn một môn học cụ thể từ danh sách bên phải để hiệu chỉnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    // Lệnh UPDATE chuẩn hoá tên cột trường dữ liệu
                    string query = "UPDATE Course SET TenMH = @TenMH, SoTC = @SoTC, Tuan = @Tuan, Hky = @Hky, Mota = @Mota WHERE MaMH = @MaMH";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaMH", txtCourseID.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenMH", txtCourseName.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTC", (int)numCredits.Value);
                    cmd.Parameters.AddWithValue("@Tuan", Convert.ToInt32(txtWeeks.Text.Trim()));

                    int semesterInt = 1;
                    if (cboSemester.Text.Contains("2")) semesterInt = 2;
                    else if (cboSemester.Text.Contains("3")) semesterInt = 3;
                    cmd.Parameters.AddWithValue("@Hky", semesterInt);

                    cmd.Parameters.AddWithValue("@Mota", txtDescription.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật thông tin học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetFields();
                    LoadCourseData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật cấu trúc: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Xử lý nghiệp vụ XÓA MÔN HỌC KHỎI HỆ THỐNG GỐC (UC-12) - BẢO VỆ TOÀN VẸN RÀNG BUỘC KHÓA NGOẠI
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseID.Text)) return;

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn môn học [{txtCourseID.Text.Trim()}] ra khỏi danh mục đào tạo gốc không?",
                "Hành động nguy hiểm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = db.GetConnection())
                    {
                        string query = "DELETE FROM Course WHERE MaMH = @MaMH";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaMH", txtCourseID.Text.Trim());

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Hệ thống đã loại bỏ môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ResetFields();
                        LoadCourseData();
                    }
                }
                catch (SqlException ex) when (ex.Number == 547) // Bắt lỗi 547: Vi phạm ràng buộc khóa ngoại (Foreign Key) sang bảng DKMH hoặc Score
                {
                    MessageBox.Show("Lỗi ràng buộc hệ thống: Học phần này hiện đang có dữ liệu sinh viên đăng ký học phần (bảng DKMH) hoặc đã có dữ liệu nhập điểm số (bảng Score), không thể xóa bỏ thô để bảo toàn tính minh bạch!",
                        "Từ chối hành động", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi thực thi xóa bản ghi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// Tìm kiếm thời gian thực (Real-time Search) khi gõ ký tự vào ô tìm kiếm trên Grid
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string query = @"SELECT MaMH AS [Mã Môn], TenMH AS [Tên Môn Học], SoTC AS [Số Tín Chỉ], Tuan AS [Số Tuần], Hky AS [Học Kỳ], Mota AS [Mô Tả] 
                                     FROM Course 
                                     WHERE TenMH LIKE @Search OR MaMH LIKE @Search";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvCourses.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        /// Làm trống toàn bộ form nhập liệu để sẵn sàng tạo môn mới
        private void ResetFields()
        {
            txtCourseID.Text = "";
            txtCourseName.Text = "";
            numCredits.Value = 3;
            txtWeeks.Text = "15";
            cboSemester.SelectedIndex = 0;
            txtDescription.Text = "";
            txtCourseID.ReadOnly = false; // Mở khóa cho phép nhập mã môn học mới
        }

        private void btnClear_Click(object sender, EventArgs e) => ResetFields();
    }
}