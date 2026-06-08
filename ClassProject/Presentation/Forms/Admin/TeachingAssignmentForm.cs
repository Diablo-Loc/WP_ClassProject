using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class TeachingAssignmentForm : Form
    {
        private readonly TeachingAssignmentRepository _repo;
        private readonly My_DB db = new My_DB();

        public TeachingAssignmentForm()
        {
            InitializeComponent();
            _repo = new TeachingAssignmentRepository(db.GetConnection().ConnectionString);
        }

        private void TeachingAssignmentForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(241, 245, 249);

            StyleGrid();
            RefreshData();
        }

        /// Hàm tập trung xử lý việc làm mới toàn bộ dữ liệu trên giao diện (ComboBox và GridView)
        private void RefreshData()
        {
            this.Cursor = Cursors.WaitCursor; // Đổi con trỏ chuột sang trạng thái chờ chuyên nghiệp
            try
            {
                LoadTeachersToComboBox();
                LoadCoursesToComboBox();
                LoadAssignmentsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi làm mới dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Cursor = Cursors.Default; // Trả con trỏ chuột về trạng thái bình thường
            }
        }

        // SỰ KIỆN NÚT LÀM MỚI (btnRefresh) CLICK
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        // 1. Nạp danh sách Giảng viên vào ComboBox cboTeacher
        private void LoadTeachersToComboBox()
        {
            try
            {
                DataTable dt = _repo.GetTeacherList();
                cboTeacher.DataSource = dt;
                cboTeacher.DisplayMember = "Username"; // Hiển thị tên tài khoản giảng viên
                cboTeacher.ValueMember = "Id";         // Giá trị ngầm là Id của giảng viên
                cboTeacher.SelectedIndex = -1;         // Mặc định ban đầu để trống, chưa chọn ai
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách giảng viên: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Nạp danh sách Môn học vào ComboBox cboCourse
        private void LoadCoursesToComboBox()
        {
            try
            {
                DataTable dt = _repo.GetCourseList();
                cboCourse.DataSource = dt;
                cboCourse.DisplayMember = "TenMH";   // Hiển thị tên môn học đầy đủ
                cboCourse.ValueMember = "MaMH";      // Giá trị ngầm là Mã môn học (CHAR(10))
                cboCourse.SelectedIndex = -1;        // Mặc định ban đầu để trống
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách môn học: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. Tải toàn bộ danh sách đã phân công lên bảng DataGridView
        private void LoadAssignmentsGrid()
        {
            try
            {
                dgvAssignments.DataSource = _repo.GetAssignments();
                FormatGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phân công: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. Làm đẹp giao diện hiển thị cho DataGridView dgvAssignments
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

            dgvAssignments.ColumnHeadersVisible = true;
            dgvAssignments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAssignments.ColumnHeadersHeight = 35;

            dgvAssignments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvAssignments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAssignments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvAssignments.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvAssignments.RowsDefaultCellStyle.BackColor = Color.White;
            dgvAssignments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvAssignments.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvAssignments.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);
        }

        // 5. Đặt tên tiếng Việt trực quan cho các cột trên GridView
        private void FormatGridColumns()
        {
            if (dgvAssignments.Columns.Count > 0)
            {
                if (dgvAssignments.Columns.Contains("ID")) dgvAssignments.Columns["ID"].Visible = false;

                if (dgvAssignments.Columns.Contains("HRID")) dgvAssignments.Columns["HRID"].HeaderText = "Mã GV";
                if (dgvAssignments.Columns.Contains("HRName")) dgvAssignments.Columns["HRName"].HeaderText = "Tài khoản Giảng viên";
                if (dgvAssignments.Columns.Contains("MaMH")) dgvAssignments.Columns["MaMH"].HeaderText = "Mã Môn học";
                if (dgvAssignments.Columns.Contains("TenMH")) dgvAssignments.Columns["TenMH"].HeaderText = "Tên Môn học";

                dgvAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        // 6. Sự kiện xử lý khi bấm nút "Phân công" (btnAssign)
        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (cboTeacher.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Giảng viên và Môn học cần phân công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int teacherId = Convert.ToInt32(cboTeacher.SelectedValue);
            string maMH = cboCourse.SelectedValue.ToString();

            bool success = _repo.AssignTeaching(teacherId, maMH);

            if (success)
            {
                MessageBox.Show("Phân công giảng dạy thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAssignmentsGrid(); // Làm mới lại bảng danh sách ngay lập tức
            }
            else
            {
                MessageBox.Show("Giảng viên này đã được phân công dạy môn học này rồi!", "Hệ thống kiểm tra trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 7. Sự kiện xử lý khi bấm nút "Xóa phân công" (btnDelete)
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAssignments.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn bản ghi phân công trên bảng mà bạn muốn hủy bỏ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn hủy phân công giảng dạy được chọn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvAssignments.CurrentRow.Cells["ID"].Value);

                if (_repo.DeleteAssignment(id))
                {
                    MessageBox.Show("Đã hủy bỏ phân công giảng dạy thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAssignmentsGrid();
                }
                else
                {
                    MessageBox.Show("Không thể xóa dữ liệu vào lúc này, vui lòng thử lại!", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}