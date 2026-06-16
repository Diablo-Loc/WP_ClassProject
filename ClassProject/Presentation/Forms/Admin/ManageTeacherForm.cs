using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ManageTeacherForm : Form
    {
        private readonly ITeacherRepository _teacherRepo;
        private bool _isEditMode = false;
        private int _selectedTeacherId = -1;

        public ManageTeacherForm()
        {
            InitializeComponent();
            _teacherRepo = new TeacherRepository();

            // Gán sự kiện chặn ký tự chữ cho ô số điện thoại
            if (txtPhone != null) txtPhone.KeyPress += TxtPhone_KeyPress;
        }

        private void ManageTeacherForm_Load(object sender, EventArgs e)
        {
            // ĐỒNG BỘ RBAC: Sử dụng Helper IsAdmin từ lõi UserSession để kiểm tra quyền truy cập thay vì check cứng số
            if (!UserSession.IsLoggedIn || !UserSession.IsAdmin)
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chỉ tài khoản Quản trị viên (Administrator) mới được quyền can thiệp danh sách giảng viên.",
                                "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Đóng form an toàn tránh xung đột UI Thread
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            StyleGrid();
            ResetFormToInsertMode();
            LoadTeacherGrid();
        }

        private void StyleGrid()
        {
            if (dgvTeachers == null) return;

            dgvTeachers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTeachers.AllowUserToAddRows = false;
            dgvTeachers.MultiSelect = false; // Chống xung đột ID khi chọn nhiều dòng
            dgvTeachers.RowTemplate.Height = 35;
            dgvTeachers.BackgroundColor = Color.White;
            dgvTeachers.BorderStyle = BorderStyle.None;

            // Đồng bộ phong cách Dark Slate thanh lịch cho tiêu đề bảng
            dgvTeachers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvTeachers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTeachers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvTeachers.RowHeadersVisible = false;
        }

        private void FormatGridColumns()
        {
            if (dgvTeachers.Columns.Count == 0) return;

            // Ẩn các cột định danh kỹ thuật không cần thiết đối với người dùng
            if (dgvTeachers.Columns.Contains("Id")) dgvTeachers.Columns["Id"].Visible = false;
            if (dgvTeachers.Columns.Contains("UserId")) dgvTeachers.Columns["UserId"].Visible = false;
            if (dgvTeachers.Columns.Contains("Status")) dgvTeachers.Columns["Status"].Visible = false;
            if (dgvTeachers.Columns.Contains("FirstName")) dgvTeachers.Columns["FirstName"].Visible = false;
            if (dgvTeachers.Columns.Contains("LastName")) dgvTeachers.Columns["LastName"].Visible = false;

            // Cấu hình nhãn tiếng Việt trực quan cho các cột
            if (dgvTeachers.Columns.Contains("MSGV")) dgvTeachers.Columns["MSGV"].HeaderText = "Mã số GV";
            if (dgvTeachers.Columns.Contains("FullName")) dgvTeachers.Columns["FullName"].HeaderText = "Họ và Tên";
            if (dgvTeachers.Columns.Contains("Gender")) dgvTeachers.Columns["Gender"].HeaderText = "Giới tính";
            if (dgvTeachers.Columns.Contains("DateOfBirth")) dgvTeachers.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
            if (dgvTeachers.Columns.Contains("Phone")) dgvTeachers.Columns["Phone"].HeaderText = "Số điện thoại";
            if (dgvTeachers.Columns.Contains("Email")) dgvTeachers.Columns["Email"].HeaderText = "Địa chỉ Email";
            if (dgvTeachers.Columns.Contains("AcademicRank")) dgvTeachers.Columns["AcademicRank"].HeaderText = "Học vị";
            if (dgvTeachers.Columns.Contains("StatusText")) dgvTeachers.Columns["StatusText"].HeaderText = "Trạng thái công tác";

            dgvTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadTeacherGrid()
        {
            try
            {
                // Tạm hủy sự kiện click để tránh hiện tượng nhảy dữ liệu ngược khi đang bind datasource
                dgvTeachers.CellClick -= dgvTeachers_CellClick;

                DataTable dt = _teacherRepo.GetAllTeachers();
                dgvTeachers.DataSource = dt;
                FormatGridColumns();

                if (lblTotalTeachers != null)
                {
                    lblTotalTeachers.Text = $"Tổng số giảng viên: {dt?.Rows.Count ?? 0}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách hệ thống: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dgvTeachers.CellClick += dgvTeachers_CellClick;
            }
        }

        private void dgvTeachers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra hàng được chọn hợp lệ theo Checklist chống bug
            if (e.RowIndex < 0 || dgvTeachers.CurrentRow == null) return;

            DataGridViewRow row = dgvTeachers.Rows[e.RowIndex];

            // Đọc dữ liệu an toàn tránh lỗi NullReferenceException
            if (row.Cells["Id"].Value != null && row.Cells["Id"].Value != DBNull.Value)
            {
                _selectedTeacherId = Convert.ToInt32(row.Cells["Id"].Value);

                txtTeacherCode.Text = row.Cells["MSGV"].Value?.ToString() ?? string.Empty;
                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString() ?? string.Empty;
                txtLastName.Text = row.Cells["LastName"].Value?.ToString() ?? string.Empty;
                cboGender.SelectedItem = row.Cells["Gender"].Value?.ToString() ?? "Nam";
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? string.Empty;
                cboRank.SelectedItem = row.Cells["AcademicRank"].Value?.ToString() ?? "Thạc sĩ";

                // Kiểm tra và chuyển đổi định dạng ngày sinh an toàn
                string dateVal = row.Cells["DateOfBirth"].Value?.ToString() ?? "";
                if (DateTime.TryParse(dateVal, out DateTime birthDate))
                {
                    dtpBirthDate.Value = birthDate;
                }
                else
                {
                    dtpBirthDate.Value = new DateTime(1990, 1, 1); // Giá trị mặc định phòng thủ
                }

                int status = Convert.ToInt32(row.Cells["Status"].Value ?? 1);
                chkActive.Checked = (status == 1);

                txtTeacherCode.Enabled = false; // Nghiêm cấm thay đổi Mã định danh khi sửa
                SwitchMode(editMode: true);
            }
        }

        private void SwitchMode(bool editMode)
        {
            _isEditMode = editMode;
            if (_isEditMode)
            {
                btnSave.Text = "💾 Cập nhật";
                btnSave.FillColor = Color.FromArgb(245, 158, 11); // Màu cam hổ phách
                if (chkActive != null) chkActive.Visible = true;
            }
            else
            {
                btnSave.Text = "(+) Thêm Giảng Viên";
                btnSave.FillColor = Color.FromArgb(16, 124, 65); // Xanh lục thương mại
                if (chkActive != null) chkActive.Visible = false;
                txtTeacherCode.Enabled = true;
            }
        }

        private void ResetFormToInsertMode()
        {
            txtTeacherCode.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;
            if (cboRank.Items.Count > 0) cboRank.SelectedIndex = 0;
            dtpBirthDate.Value = new DateTime(1990, 1, 1);
            _selectedTeacherId = -1;
            SwitchMode(editMode: false);
            txtTeacherCode.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msgv = txtTeacherCode.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string gender = cboGender.SelectedItem?.ToString() ?? "Nam";
            string rank = cboRank.SelectedItem?.ToString() ?? "Thạc sĩ";
            DateTime birth = dtpBirthDate.Value;

            // 1. Kiểm tra nghiệp vụ dữ liệu trống
            if (string.IsNullOrWhiteSpace(msgv) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Mã giảng viên, Họ và Tên đệm không được phép bỏ trống!", "Cảnh báo nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra biểu thức chính quy (Regex Formats)
            if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^(03|05|07|08|09)[0-9]{8}$"))
            {
                MessageBox.Show("Số điện thoại không đúng định dạng di động Việt Nam (Phải có 10 chữ số)!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Địa chỉ Email không tuân thủ định dạng cấu trúc chung!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Kiểm tra tính toàn vẹn (Chống trùng lập hồ sơ trên database)
            int? excludeId = _isEditMode ? (int?)_selectedTeacherId : null;
            if (_teacherRepo.IsDuplicateCheck(msgv, phone, email, excludeId))
            {
                MessageBox.Show("Mã số giảng viên, Số điện thoại hoặc Email này đã tồn tại trên một hồ sơ công tác khác!", "Xung đột dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Thực thi lưu trữ qua Repository
            if (_isEditMode)
            {
                int status = chkActive.Checked ? 1 : 0;
                if (_teacherRepo.UpdateTeacher(_selectedTeacherId, firstName, lastName, birth, gender, phone, email, rank, status))
                {
                    MessageBox.Show("Cập nhật thông tin giảng viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
            }
            else
            {
                // Tạo hồ sơ giảng viên mới
                int? linkedUserId = null;

                if (_teacherRepo.InsertTeacher(linkedUserId, msgv, firstName, lastName, birth, gender, phone, email, rank))
                {
                    MessageBox.Show("Thêm mới hồ sơ giảng viên vào hệ thống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
                else
                {
                    MessageBox.Show("Xử lý thêm mới thất bại, vui lòng kiểm tra kết nối SQL Server!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedTeacherId == -1 || dgvTeachers.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng click chọn một giảng viên cụ thể từ danh sách trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetName = $"{txtFirstName.Text} {txtLastName.Text}".Trim();
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn giảng viên [{targetName}] khỏi hệ thống?\nHành động này không thể hoàn tác!", "Xác nhận hành động xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                if (_teacherRepo.DeleteTeacher(_selectedTeacherId))
                {
                    MessageBox.Show("Đã xóa hồ sơ giảng viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadTeacherGrid();
            }
            else
            {
                DataTable dt = _teacherRepo.SearchTeachers(keyword);
                dgvTeachers.DataSource = dt;
                FormatGridColumns();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetFormToInsertMode();
            if (txtSearch != null) txtSearch.Clear();
            LoadTeacherGrid();
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép phím số và phím xóa Backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}