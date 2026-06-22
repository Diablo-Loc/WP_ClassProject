using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Interfaces;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ContactForm : Form
    {
        // Khai báo Interface (Đúng chuẩn Loose Coupling - Liên kết lỏng)
        private readonly IContactRepository _contactRepo;

        private bool _isEditMode = false;
        private string _selectedUniqueId = string.Empty;
        private int _selectedContactId = -1;

        // Lấy thông tin phiên làm việc của User đang đăng nhập hệ thống
        private readonly int _currentUserId = UserSession.UserId;

        // Bộ đếm thời gian Debounce chống quá tải (overload) câu lệnh SQL truy vấn tìm kiếm
        private readonly System.Windows.Forms.Timer _searchDebounceTimer;

        /// <summary>
        /// Constructor nhận lớp Repository từ DI Container truyền vào 
        /// </summary>
        public ContactForm(IContactRepository contactRepo)
        {
            InitializeComponent();

            // Gán giá trị thông qua cơ chế Constructor Injection (Nếu null sẽ báo lỗi ngay lập tức)
            _contactRepo = contactRepo ?? throw new ArgumentNullException(nameof(contactRepo));

            // Đăng ký sự kiện hạn chế nhập chữ cho ô số điện thoại
            this.txtPhone.KeyPress += txtPhone_KeyPress;

            // Cấu hình Debounce thời gian tìm kiếm: 350ms
            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 350 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        private async void ContactForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadGroupsComboBoxAsync(); // Nạp danh mục phòng ban vào ComboBox trước
            ResetFormToInsertMode();
            await LoadContactGridAsync();
        }

        private void StyleGrid()
        {
            if (dgvContacts == null) return;
            dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContacts.AllowUserToAddRows = false;
            dgvContacts.MultiSelect = false;
            dgvContacts.RowTemplate.Height = 35;
            dgvContacts.BackgroundColor = Color.White;
            dgvContacts.BorderStyle = BorderStyle.None;
            dgvContacts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvContacts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvContacts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvContacts.RowHeadersVisible = true;
            dgvContacts.RowHeadersWidth = 45;
            dgvContacts.RowPostPaint += dgvContacts_RowPostPaint;
        }

        private void FormatGridColumns()
        {
            if (dgvContacts.Columns.Count == 0) return;

            // Ẩn các cột logic xử lý ngầm của hệ thống
            if (dgvContacts.Columns.Contains("UniqueID")) dgvContacts.Columns["UniqueID"].Visible = false;
            if (dgvContacts.Columns.Contains("IsSystemData")) dgvContacts.Columns["IsSystemData"].Visible = false;
            if (dgvContacts.Columns.Contains("Group_ID")) dgvContacts.Columns["Group_ID"].Visible = false;

            // Định dạng tiêu đề hiển thị chuẩn hóa cho người dùng cuối
            if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên nhân sự";
            if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại";
            if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";
            if (dgvContacts.Columns.Contains("GroupName")) dgvContacts.Columns["GroupName"].HeaderText = "Phòng ban / Khối";

            dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Tải danh mục nhóm phòng ban trực tiếp thông qua định danh ComboBox có sẵn trên Designer
        private async Task LoadGroupsComboBoxAsync()
        {
            try
            {
                if (cboGroups == null) return;

                DataTable dtGroups = await _contactRepo.GetGroupsByUserAsync(_currentUserId);

                // Tạo dòng mặc định tương thích logic DB SET NULL khi không chọn phòng ban
                DataRow dr = dtGroups.NewRow();
                dr["ID"] = DBNull.Value;
                dr["Name"] = "-- Chưa phân phòng --";
                dtGroups.Rows.InsertAt(dr, 0);

                cboGroups.DataSource = dtGroups;
                cboGroups.DisplayMember = "Name";
                cboGroups.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh mục phòng ban: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task LoadContactGridAsync()
        {
            try
            {
                // Ngắt sự kiện CellClick tạm thời nhằm tránh kích hoạt sai luồng logic khi đổ dữ liệu
                dgvContacts.CellClick -= dgvContacts_CellClick;

                DataTable dt = await _contactRepo.GetAllContactsByUserAsync(_currentUserId);
                dgvContacts.DataSource = dt;
                FormatGridColumns();

                if (lblTotalContacts != null)
                {
                    lblTotalContacts.Text = $"Danh bạ khả dụng của bạn: {dt?.Rows.Count ?? 0}";
                    lblTotalContacts.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu khi tải danh sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dgvContacts.CellClick += dgvContacts_CellClick;
            }
        }

        private void dgvContacts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvContacts.Rows[e.RowIndex];
            if (row.Cells["UniqueID"].Value != null)
            {
                _selectedUniqueId = row.Cells["UniqueID"].Value.ToString();
                int isSystemData = Convert.ToInt32(row.Cells["IsSystemData"].Value);

                txtName.Text = row.Cells["Name"].Value?.ToString() ?? string.Empty;
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? string.Empty;

                // Đồng bộ hiển thị lựa chọn ComboBox Phòng ban trực tiếp và an toàn
                if (cboGroups != null && row.Cells["Group_ID"].Value != null)
                {
                    if (row.Cells["Group_ID"].Value == DBNull.Value || Convert.ToInt32(row.Cells["Group_ID"].Value) == -1)
                        cboGroups.SelectedIndex = 0;
                    else
                        cboGroups.SelectedValue = row.Cells["Group_ID"].Value;
                }

                // Bảo mật phân quyền dữ liệu: Hệ thống (Chỉ đọc) và Cá nhân (Được sửa đổi)
                if (isSystemData == 1)
                {
                    btnInsert.Enabled = false;
                    btnDelete.Enabled = false;

                    // Khóa ComboBox phòng ban không cho phép chỉnh sửa dữ liệu dùng chung toàn trường
                    if (cboGroups != null) cboGroups.Enabled = false;

                    if (lblTotalContacts != null)
                    {
                        lblTotalContacts.Text = "Chế độ: Dữ liệu giảng viên hệ thống (Chỉ đọc)";
                        lblTotalContacts.ForeColor = Color.DarkRed;
                    }
                }
                else
                {
                    btnInsert.Enabled = true;
                    btnDelete.Enabled = true;

                    // Mở khóa ComboBox phòng ban phục vụ tính năng "Đổi phòng ban" của liên hệ cá nhân
                    if (cboGroups != null) cboGroups.Enabled = true;

                    if (lblTotalContacts != null)
                    {
                        lblTotalContacts.ForeColor = Color.Black;
                        lblTotalContacts.Text = $"Đang chọn liên hệ: {txtName.Text}";
                    }

                    _selectedContactId = Convert.ToInt32(_selectedUniqueId.Replace("CONTACT_", ""));
                    SwitchMode(editMode: true);
                }
            }
        }

        private void SwitchMode(bool editMode)
        {
            _isEditMode = editMode;
            if (_isEditMode)
            {
                btnInsert.Text = "💾 Cập nhật";
                btnInsert.FillColor = Color.FromArgb(245, 158, 11);
            }
            else
            {
                btnInsert.Text = "(+) Thêm liên hệ";
                btnInsert.FillColor = Color.FromArgb(16, 124, 65);
            }
        }

        private void ResetFormToInsertMode()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            _selectedContactId = -1;
            _selectedUniqueId = string.Empty;

            btnInsert.Enabled = true;
            btnDelete.Enabled = true;

            // Đưa ComboBox về lựa chọn đầu tiên và mở khóa sẵn sàng thêm mới
            if (cboGroups != null)
            {
                cboGroups.SelectedIndex = 0;
                cboGroups.Enabled = true;
            }

            SwitchMode(editMode: false);
            txtName.Focus();
        }

        private async void btnInsert_Click(object sender, EventArgs e)
        {
            string nameInput = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(nameInput))
            {
                MessageBox.Show("Vui lòng điền tên liên hệ nhân sự!", "Dữ liệu trống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Thuật toán bóc tách chuỗi thành Họ đệm (LastName) và Tên (FirstName)
            string fname = string.Empty;
            string lname = string.Empty;
            string[] nameParts = Regex.Split(nameInput, @"\s+");

            if (nameParts.Length == 1)
            {
                fname = nameParts[0];
            }
            else
            {
                fname = nameParts[nameParts.Length - 1];
                lname = string.Join(" ", nameParts, 0, nameParts.Length - 1);
            }

            string phoneInput = txtPhone.Text.Trim();
            string emailInput = txtEmail.Text.Trim();

            // Trích xuất an toàn giá trị Group_ID phục vụ đổi phòng ban từ thuộc tính SelectedValue
            int? groupId = null;
            if (cboGroups != null && cboGroups.SelectedValue != null && cboGroups.SelectedValue != DBNull.Value)
            {
                groupId = Convert.ToInt32(cboGroups.SelectedValue);
            }

            // Kiểm định biểu thức chính quy (Regex Validation)
            if (!string.IsNullOrEmpty(phoneInput))
            {
                string phonePattern = @"^(0[35789][0-9]{8})|(02[0-9]{8,9})$";
                if (!Regex.IsMatch(phoneInput, phonePattern))
                {
                    MessageBox.Show("Định dạng số điện thoại không hợp lệ!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPhone.Focus();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(emailInput))
            {
                // Regex chuẩn quốc tế, chấp nhận mọi loại email thật và email giáo dục nhiều cấp tên miền
                string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

                if (!Regex.IsMatch(emailInput, emailPattern, RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("Định dạng địa chỉ Email không hợp lệ!\nHệ thống hỗ trợ cả Email cá nhân và Email giáo dục (Ví dụ: nguyenvanb@fe.edu.vn).",
                                    "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmail.Focus();
                    return;
                }
            }

            // Kiểm tra ràng buộc duy nhất trong phạm vi danh bạ riêng của tài khoản này
            int? currentExcludeId = _isEditMode ? (int?)_selectedContactId : null;
            if (await _contactRepo.IsPhoneOrEmailExistsAsync(phoneInput, emailInput, currentExcludeId, _currentUserId))
            {
                MessageBox.Show("Số điện thoại hoặc Email này đã tồn tại trong danh bạ của bạn!", "Trùng bản ghi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thực thi Cập nhật (Đổi phòng ban) hoặc Thêm mới tương thích cấu trúc database
                bool result = _isEditMode
                    ? await _contactRepo.UpdateContactAsync(_selectedContactId, fname, lname, phoneInput, emailInput, groupId, _currentUserId)
                    : await _contactRepo.InsertContactAsync(fname, lname, phoneInput, emailInput, groupId, _currentUserId);

                if (result)
                {
                    MessageBox.Show("Xử lý thông tin danh bạ thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    await LoadContactGridAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nghiệp vụ phát sinh ngoài ý muốn: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedContactId == -1 || _selectedUniqueId.StartsWith("TEACHER_"))
            {
                MessageBox.Show("Không thể xóa dữ liệu thuộc quyền quản lý của hệ thống!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn gỡ bỏ vĩnh viễn liên hệ này khỏi danh bạ cá nhân?", "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (await _contactRepo.DeleteContactAsync(_selectedContactId, _currentUserId))
                    {
                        ResetFormToInsertMode();
                        await LoadContactGridAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thực hiện xóa bản ghi: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private async void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();

            string keyword = txtSearch.Text.Trim();
            try
            {
                DataTable dt = string.IsNullOrEmpty(keyword)
                    ? await _contactRepo.GetAllContactsByUserAsync(_currentUserId)
                    : await _contactRepo.SearchContactsByUserAsync(keyword, _currentUserId);

                dgvContacts.DataSource = dt;
                FormatGridColumns();

                if (lblTotalContacts != null)
                {
                    lblTotalContacts.Text = string.IsNullOrEmpty(keyword)
                        ? $"Danh bạ khả dụng của bạn: {dt?.Rows.Count ?? 0}"
                        : $"Kết quả tìm kiếm phù hợp: {dt?.Rows.Count ?? 0}";
                    lblTotalContacts.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Quá trình tìm kiếm dữ liệu thất bại: {ex.Message}", "Cảnh báo truy vấn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvContacts_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string stt = (e.RowIndex + 1).ToString();
            Font rFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Brush rBrush = new SolidBrush(Color.FromArgb(100, 116, 139));
            var grid = (DataGridView)sender;
            float x = e.RowBounds.Location.X + (grid.RowHeadersWidth - e.Graphics.MeasureString(stt, rFont).Width) / 2;
            float y = e.RowBounds.Location.Y + (e.RowBounds.Height - rFont.Height) / 2;
            e.Graphics.DrawString(stt, rFont, rBrush, x, y);
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8) e.Handled = true;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _searchDebounceTimer.Stop();
                SearchDebounceTimer_Tick(this, EventArgs.Empty);
            }
        }

        private async void btnClear_Click(object sender, EventArgs e)
        {
            ResetFormToInsertMode();
            txtSearch.Clear();
            await LoadContactGridAsync();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            SearchDebounceTimer_Tick(this, EventArgs.Empty);
        }
    }
}