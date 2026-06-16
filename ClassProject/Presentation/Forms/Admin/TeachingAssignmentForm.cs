using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class TeachingAssignmentForm : Form
    {
        private readonly TeachingAssignmentRepository _repo;
        private readonly My_DB db = new My_DB();

        // Biến cờ hiệu bảo vệ: Chặn Spam Click và chặn kích hoạt lọc dữ liệu sai thời điểm lúc khởi tạo
        private bool _isActionExecuting = false;
        private bool _isFormReady = false;

        public TeachingAssignmentForm()
        {
            InitializeComponent();
            _repo = new TeachingAssignmentRepository(db.GetConnection().ConnectionString);
        }

        private async void TeachingAssignmentForm_Load(object sender, EventArgs e)
        {
            // Thiết lập màu nền Workspace theo phong cách tối giản hiện đại (Màu Slate nhẹ)
            this.BackColor = Color.FromArgb(241, 245, 249);

            StyleGrid();

            // Tải dữ liệu danh mục song song đa luồng
            await InitializeFormComponentsAsync();

            // Đánh dấu hệ thống đã nạp xong cấu trúc dữ liệu an toàn
            _isFormReady = true;

            // Đăng ký các sự kiện lọc thời gian thực (Real-time Filtering)
            cboTeacher.SelectedIndexChanged += FilterComboBox_Changed;
            cboCourse.SelectedIndexChanged += FilterComboBox_Changed;
        }

        /// Nghiệp vụ Doanh nghiệp: Tải ngầm song song cấu trúc dữ liệu của Form giúp tối ưu hóa hiệu năng
        private async Task InitializeFormComponentsAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Đồng loạt kích hoạt luồng bốc dữ liệu từ SQL Server về bộ nhớ RAM của ứng dụng
                Task<DataTable> teachersTask = _repo.GetTeacherListAsync();
                Task<DataTable> coursesTask = _repo.GetCourseListAsync();

                await Task.WhenAll(teachersTask, coursesTask);

                // 1. Đổ dữ liệu an toàn vào danh sách Giảng viên (Thêm nhãn mặc định để phục vụ tính năng lọc)
                DataTable dtTeachers = teachersTask.Result;
                DataRow drTeacherDefault = dtTeachers.NewRow();
                drTeacherDefault["Id"] = -1;
                drTeacherDefault["Username"] = "-- Tất cả giảng viên --"; // Nhãn chuẩn thân thiện doanh nghiệp
                dtTeachers.Rows.InsertAt(drTeacherDefault, 0);

                cboTeacher.DataSource = dtTeachers;
                cboTeacher.DisplayMember = "Username";
                cboTeacher.ValueMember = "Id";

                // 2. Đổ dữ liệu an toàn vào danh sách Môn học (Thêm nhãn mặc định để phục vụ tính năng lọc)
                DataTable dtCourses = coursesTask.Result;
                DataRow drCourseDefault = dtCourses.NewRow();
                drCourseDefault["MaMH"] = "";
                drCourseDefault["TenMH"] = "-- Tất cả môn học --";
                dtCourses.Rows.InsertAt(drCourseDefault, 0);

                cboCourse.DataSource = dtCourses;
                cboCourse.DisplayMember = "TenMH";
                cboCourse.ValueMember = "MaMH";

                // 3. Tải danh sách phân công lên bảng DataGridView dựa trên bộ lọc mặc định ban đầu
                await QueryAndBindGridAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi khởi tạo danh mục: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// Lắp ráp Bộ lọc thông minh kết hợp Stored Procedure 'proc_GetTeachingAssignments_Report' để nạp dữ liệu lên lưới
        private async Task QueryAndBindGridAsync()
        {
            try
            {
                int? teacherId = null;
                string maMH = null;

                // Đọc giá trị an toàn từ ComboBox Giảng viên
                if (cboTeacher.SelectedValue != null && Convert.ToInt32(cboTeacher.SelectedValue) != -1)
                {
                    teacherId = Convert.ToInt32(cboTeacher.SelectedValue);
                }

                // Đọc giá trị an toàn từ ComboBox Môn học
                if (cboCourse.SelectedValue != null && !string.IsNullOrWhiteSpace(cboCourse.SelectedValue.ToString()))
                {
                    maMH = cboCourse.SelectedValue.ToString().Trim();
                }

                // Kích hoạt thủ tục lưu trữ lọc có tham số của bạn dưới dạng bất đồng bộ
                DataTable dtReport = await _repo.GetAssignmentsReportAsync(teacherId, maMH);
                dgvAssignments.DataSource = dtReport;
                FormatGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình truy vấn bảng lưới dữ liệu: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Sự kiện tự động kích hoạt Lọc thời gian thực khi người dùng thay đổi lựa chọn trên ComboBox
        private async void FilterComboBox_Changed(object sender, EventArgs e)
        {
            if (_isFormReady)
            {
                await QueryAndBindGridAsync();
            }
        }

        /// Xử lý sự kiện nút "Làm mới" (btnLoad_Click) - Đồng bộ theo tệp Designer thực tế của bạn
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_isActionExecuting) return;

            // Khóa tạm thời các sự kiện đổi Index để tránh việc truy vấn SQL lặp đi lặp lại nhiều lần
            _isFormReady = false;

            if (cboTeacher.Items.Count > 0) cboTeacher.SelectedIndex = 0;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;

            _isFormReady = true;

            this.Cursor = Cursors.WaitCursor;
            await QueryAndBindGridAsync();
            this.Cursor = Cursors.Default;
        }

        /// Xử lý nghiệp vụ nút "Phân công" (btnAssign_Click)
        private async void btnAssign_Click(object sender, EventArgs e)
        {
            if (_isActionExecuting) return;

            // Kiểm tra tính hợp lệ của dữ liệu đầu vào (Data Validation)
            if (cboTeacher.SelectedValue == null || Convert.ToInt32(cboTeacher.SelectedValue) == -1)
            {
                MessageBox.Show("Vui lòng lựa chọn một Giảng viên cụ thể để thực hiện phân công công tác giảng dạy!", "Nhắc nhở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboCourse.SelectedValue == null || string.IsNullOrWhiteSpace(cboCourse.SelectedValue.ToString()))
            {
                MessageBox.Show("Vui lòng lựa chọn một Môn học cụ thể để thực hiện phân công công tác giảng dạy!", "Nhắc nhở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int teacherId = Convert.ToInt32(cboTeacher.SelectedValue);
            string maMH = cboCourse.SelectedValue.ToString().Trim();
            string teacherName = cboTeacher.Text;
            string courseName = cboCourse.Text;

            try
            {
                _isActionExecuting = true; // Bật cờ hiệu khóa cứng giao diện chống Spam Click phá hỏng luồng dữ liệu
                this.Cursor = Cursors.WaitCursor;

                // RÀNG BUỘC 1: Kiểm tra trùng lặp bằng tiến trình chạy ngầm
                bool isDuplicate = await _repo.IsDuplicateAssignmentAsync(teacherId, maMH);
                if (isDuplicate)
                {
                    MessageBox.Show($"Ràng buộc hệ thống: Giảng viên [{teacherName}] đã được phân công đảm nhiệm bộ môn [{courseName}] này từ trước!", "Trùng Lặp Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // RÀNG BUỘC 2: Kiểm tra giới hạn số lượng môn đảm nhiệm thực tế (Chặn quá tải lao động)
                int currentCourses = await _repo.GetCurrentCourseCountAsync(teacherId);
                if (currentCourses >= 5) // Ngưỡng an toàn tối đa 5 môn/giảng viên của trường học
                {
                    MessageBox.Show($"Cảnh báo nghiệp vụ: Giảng viên [{teacherName}] đã đạt giới hạn tối đa định ngạch 5 môn giảng dạy. Không thể bổ sung phân công thêm!", "Vượt Quá Giới Hạn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thực hiện chèn ghi nhận thông tin phân công xuống DB qua tiến trình bất đồng bộ
                bool success = await _repo.InsertAssignmentAsync(teacherId, maMH);
                if (success)
                {
                    MessageBox.Show($"Chúc mừng! Phân công giảng viên [{teacherName}] phụ trách môn học [{courseName}] thành công.", "Ghi Nhận Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Nạp lại bảng dữ liệu mới nhất
                    await QueryAndBindGridAsync();
                }
                else
                {
                    MessageBox.Show("Hệ thống bận hoặc xảy ra lỗi kết nối cục bộ. Không thể hoàn tất thao tác phân công!", "Lỗi Xử Lý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực hiện phân công: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isActionExecuting = false;
                this.Cursor = Cursors.Default;
            }
        }

        /// Xử lý nghiệp vụ nút "Xóa/Hủy phân công" (btnDelete_Click)
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_isActionExecuting) return;

            // Kiểm tra xem người dùng đã chọn hàng nào trên GridView chưa
            if (dgvAssignments.CurrentRow == null || dgvAssignments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng click chuột chọn một dòng phân công cụ thể trên bảng dữ liệu bên dưới để thực hiện hủy bỏ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đọc thông tin pháp lý của dòng phân công được trỏ tới
            int id = Convert.ToInt32(dgvAssignments.CurrentRow.Cells["ID"].Value);
            string teacherName = dgvAssignments.Columns.Contains("HRName") ? dgvAssignments.CurrentRow.Cells["HRName"].Value?.ToString() : "Giảng viên";
            string courseName = dgvAssignments.Columns.Contains("TenMH") ? dgvAssignments.CurrentRow.Cells["TenMH"].Value?.ToString() : "Môn học";

            DialogResult dialogResult = MessageBox.Show($"Xác nhận nghiêm túc: Bạn có chắc chắn muốn tiến hành hủy phân công giảng dạy môn học [{courseName}] của thầy/cô [{teacherName}] không?",
                "Xác Nhận Hủy Nghiệp Vụ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    _isActionExecuting = true;
                    this.Cursor = Cursors.WaitCursor;

                    if (await _repo.DeleteAssignmentAsync(id))
                    {
                        MessageBox.Show("Hệ thống đã loại bỏ và hủy bản ghi phân công giảng dạy thành công.", "Hủy Bỏ Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Cập nhật lại lưới
                        await QueryAndBindGridAsync();
                    }
                    else
                    {
                        MessageBox.Show("Bản ghi dữ liệu không còn tồn tại hoặc đã bị thay đổi bởi một phiên làm việc khác!", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi hệ thống khi thực thi lệnh xóa: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _isActionExecuting = false;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void StyleGrid()
        {
            if (dgvAssignments == null) return;

            dgvAssignments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAssignments.AllowUserToAddRows = false;
            dgvAssignments.EnableHeadersVisualStyles = false;
            dgvAssignments.RowTemplate.Height = 35;
            dgvAssignments.GridColor = Color.FromArgb(241, 245, 249);
            dgvAssignments.BackgroundColor = Color.White;
            dgvAssignments.BorderStyle = BorderStyle.None;
            dgvAssignments.ReadOnly = true;

            dgvAssignments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAssignments.ColumnHeadersHeight = 38;

            // Layout Header phong cách Slate Dark lịch lãm, tinh tế, sạch sẽ
            dgvAssignments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvAssignments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAssignments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);

            dgvAssignments.RowsDefaultCellStyle.BackColor = Color.White;
            dgvAssignments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvAssignments.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvAssignments.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);
        }

        private void FormatGridColumns()
        {
            if (dgvAssignments.Columns.Count == 0) return;

            // Ẩn các cột định danh hệ thống (Technical ID fields) để tăng diện tích hiển thị
            if (dgvAssignments.Columns.Contains("ID")) dgvAssignments.Columns["ID"].Visible = false;
            if (dgvAssignments.Columns.Contains("HRID")) dgvAssignments.Columns["HRID"].Visible = false;
            if (dgvAssignments.Columns.Contains("MaMH")) dgvAssignments.Columns["MaMH"].Visible = false;

            // Đặt tiêu đề hiển thị tiếng Việt có dấu chuẩn báo cáo hành chính doanh nghiệp
            if (dgvAssignments.Columns.Contains("HRName")) dgvAssignments.Columns["HRName"].HeaderText = "Tài Khoản Giảng Viên";
            if (dgvAssignments.Columns.Contains("TenMH")) dgvAssignments.Columns["TenMH"].HeaderText = "Tên Môn Học Đảm Nhiệm";
            if (dgvAssignments.Columns.Contains("TotalAssigned")) dgvAssignments.Columns["TotalAssigned"].HeaderText = "Số Lượng Môn Đang Dạy";

            dgvAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}