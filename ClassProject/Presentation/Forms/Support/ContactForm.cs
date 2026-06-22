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
        private readonly IContactRepository _contactRepo;

        private bool _isEditMode = false;
        private string _selectedUniqueId = string.Empty;
        private int _selectedContactId = -1;
        private readonly int _currentUserId = UserSession.UserId;
        private readonly System.Windows.Forms.Timer _searchDebounceTimer;

        public ContactForm(IContactRepository contactRepo)
        {
            InitializeComponent();
            _contactRepo = contactRepo ?? throw new ArgumentNullException(nameof(contactRepo));

            this.txtPhone.KeyPress += txtPhone_KeyPress;

            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 350 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        private async void ContactForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadGroupsComboBoxAsync();
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

            if (dgvContacts.Columns.Contains("UniqueID")) dgvContacts.Columns["UniqueID"].Visible = false;
            if (dgvContacts.Columns.Contains("IsSystemData")) dgvContacts.Columns["IsSystemData"].Visible = false;
            if (dgvContacts.Columns.Contains("Group_ID")) dgvContacts.Columns["Group_ID"].Visible = false;

            if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên nhân sự";
            if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại";
            if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";
            if (dgvContacts.Columns.Contains("GroupName")) dgvContacts.Columns["GroupName"].HeaderText = "Phòng ban / Khối";

            dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async Task LoadGroupsComboBoxAsync()
        {
            try
            {
                if (cboGroups == null) return;

                // FIX: Dùng đúng tài khoản đang đăng nhập để lấy danh mục phòng ban đồng bộ với lưới dữ liệu
                DataTable dtGroups = await _contactRepo.GetGroupsByUserAsync(_currentUserId);

                if (dtGroups == null)
                {
                    dtGroups = new DataTable();
                    dtGroups.Columns.Add("ID", typeof(int));
                    dtGroups.Columns.Add("Name", typeof(string));
                }

                DataRow dr = dtGroups.NewRow();
                dr["ID"] = -1; // Đổi thành -1 cho khớp logic lọc dữ liệu rỗng
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

                int isSystemData = 0;
                var sysValue = row.Cells["IsSystemData"].Value;
                if (sysValue != null && sysValue != DBNull.Value)
                {
                    if (sysValue is bool boolVal)
                        isSystemData = boolVal ? 1 : 0;
                    else
                        int.TryParse(sysValue.ToString(), out isSystemData);
                }

                txtName.Text = row.Cells["Name"].Value?.ToString() ?? string.Empty;
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? string.Empty;

                // --- ĐỒNG BỘ HÓA COMBOBOX PHÒNG BAN ---
                if (cboGroups != null)
                {
                    var groupIdValue = row.Cells["Group_ID"].Value;
                    if (groupIdValue != null && groupIdValue != DBNull.Value)
                    {
                        if (int.TryParse(groupIdValue.ToString(), out int gId) && gId > 0)
                        {
                            cboGroups.SelectedValue = gId;

                            // Phòng hờ nếu danh sách ComboBox không chứa ID này thì đẩy về dòng 0
                            if (cboGroups.SelectedValue == null || (int)cboGroups.SelectedValue != gId)
                            {
                                cboGroups.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            cboGroups.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        cboGroups.SelectedIndex = 0;
                    }
                }

                string numberOnly = Regex.Match(_selectedUniqueId, @"\d+").Value;
                if (!string.IsNullOrEmpty(numberOnly) && int.TryParse(numberOnly, out int parsedId))
                {
                    _selectedContactId = parsedId;
                }
                else
                {
                    _selectedContactId = -1;
                }

                // --- KIỂM TRA PHÂN QUYỀN GIAO DIỆN ---
                if (isSystemData == 1 && !UserSession.IsAdmin)
                {
                    _isEditMode = false;
                    btnInsert.Enabled = false;
                    btnDelete.Enabled = false;
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
                    btnDelete.Enabled = (isSystemData == 0 || UserSession.IsAdmin);
                    if (cboGroups != null) cboGroups.Enabled = true;

                    if (lblTotalContacts != null)
                    {
                        if (isSystemData == 1 && UserSession.IsAdmin)
                        {
                            lblTotalContacts.ForeColor = Color.DarkBlue;
                            lblTotalContacts.Text = "[ADMIN] Quyền quản trị: Đang chỉnh sửa thông tin Giảng viên hệ thống";
                        }
                        else
                        {
                            lblTotalContacts.ForeColor = Color.Black;
                            lblTotalContacts.Text = $"Đang chọn liên hệ: {txtName.Text}";
                        }
                    }

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

            int? groupId = null;
            if (cboGroups != null && cboGroups.SelectedValue != null && cboGroups.SelectedValue != DBNull.Value)
            {
                if (int.TryParse(cboGroups.SelectedValue.ToString(), out int parsedGroupId) && parsedGroupId > 0)
                {
                    groupId = parsedGroupId;
                }
            }

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
                string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
                if (!Regex.IsMatch(emailInput, emailPattern, RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("Định dạng địa chỉ Email không hợp lệ!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmail.Focus();
                    return;
                }
            }

            int? currentExcludeId = _isEditMode ? (int?)_selectedContactId : null;
            if (await _contactRepo.IsPhoneOrEmailExistsAsync(phoneInput, emailInput, currentExcludeId, _currentUserId))
            {
                MessageBox.Show("Số điện thoại hoặc Email này đã tồn tại trong danh bạ của bạn!", "Trùng bản ghi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ĐÃ FIX: Chuyển sang dùng biến _selectedUniqueId cho hàm Update để tránh gán nhầm mapping bảng phòng ban con
                bool result = _isEditMode
                ? await _contactRepo.UpdateContactAsync(_selectedContactId, fname, lname, phoneInput, emailInput, groupId, _currentUserId)
                : await _contactRepo.InsertContactAsync(fname, lname, phoneInput, emailInput, groupId, _currentUserId);

                if (result)
                {
                    MessageBox.Show("Xử lý thông tin danh bạ thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // FIX THỨ TỰ: Load lại danh sách lên lưới trước để lấy tên phòng ban mới từ DB
                    await LoadContactGridAsync();

                    // Sau đó mới reset các ô nhập liệu về trạng thái ban đầu
                    ResetFormToInsertMode();
                }
                else
                {
                    MessageBox.Show("Cập nhật dữ liệu thất bại. Vui lòng kiểm tra lại quyền hạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nghiệp vụ phát sinh: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            bool isSystemTeacher = _selectedUniqueId.StartsWith("TEACHER_") || _selectedUniqueId.StartsWith("MSGV_");

            if (_selectedContactId == -1 || (isSystemTeacher && !UserSession.IsAdmin))
            {
                MessageBox.Show("Không thể xóa dữ liệu thuộc quyền quản lý của hệ thống!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string confirmMsg = isSystemTeacher
                ? "CẢNH BÁO: Bạn đang xóa một Giảng viên hệ thống ra khỏi danh bạ chung toàn trường! Xác nhận hành động?"
                : "Bạn có chắc chắn muốn gỡ bỏ vĩnh viễn liên hệ này khỏi danh bạ cá nhân?";

            if (MessageBox.Show(confirmMsg, "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (await _contactRepo.DeleteContactAsync(_selectedContactId, _currentUserId))
                    {
                        await LoadContactGridAsync();
                        ResetFormToInsertMode();
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