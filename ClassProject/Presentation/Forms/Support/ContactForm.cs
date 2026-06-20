using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using ClassProject.DataAccess.Repositories.Implementations;
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

        // Giả lập ID người dùng đang đăng nhập hệ thống (Hãy thay bằng Session thực tế của bạn)
        private readonly int _currentUserId = UserSession.UserId;

        // Bộ đếm thời gian Debounce chống overload SQL
        private readonly System.Windows.Forms.Timer _searchDebounceTimer;

        public ContactForm()
        {
            InitializeComponent();

            // Khởi tạo thông qua Interface (Chuẩn DI/IoC)
            _contactRepo = new ContactRepository();

            this.txtPhone.KeyPress += txtPhone_KeyPress;

            // Cấu hình Debounce: 350ms
            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 350 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        private async void ContactForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadGroupsComboBoxAsync(); // Tải danh mục nhóm vào ComboBox trước
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

            if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên / Khối phòng ban";
            if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại liên lạc";
            if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";

            dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Tải danh mục nhóm lên ComboBox phục vụ phân loại danh bạ
        private async Task LoadGroupsComboBoxAsync()
        {
            try
            {
                // Giả định ComboBox của bạn tên là cboGroups
                if (Controls.Find("cboGroups", true).Length == 0) return;
                var cboGroups = (ComboBox)Controls.Find("cboGroups", true)[0];

                DataTable dtGroups = await _contactRepo.GetGroupsByUserAsync(_currentUserId);

                // Tạo một dòng mặc định "Không phân nhóm"
                DataRow dr = dtGroups.NewRow();
                dr["ID"] = DBNull.Value;
                dr["Name"] = "-- Không phân nhóm --";
                dtGroups.Rows.InsertAt(dr, 0);

                cboGroups.DataSource = dtGroups;
                cboGroups.DisplayMember = "Name";
                cboGroups.ValueMember = "ID";
            }
            catch { /* Phòng vệ nếu UI chưa kéo ComboBox */ }
        }

        private async Task LoadContactGridAsync()
        {
            try
            {
                dgvContacts.CellClick -= dgvContacts_CellClick;

                // Bảo mật: Chỉ tải danh bạ hệ thống kèm danh bạ RIÊNG của người dùng này
                DataTable dt = await _contactRepo.GetAllContactsByUserAsync(_currentUserId);
                dgvContacts.DataSource = dt;
                FormatGridColumns();

                int total = dt?.Rows.Count ?? 0;
                if (lblTotalContacts != null)
                {
                    lblTotalContacts.Text = $"Danh bạ khả dụng của bạn: {total}";
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

                // Đồng bộ hiển thị ComboBox Nhóm
                if (Controls.Find("cboGroups", true).Length > 0 && row.Cells["Group_ID"].Value != null)
                {
                    var cboGroups = (ComboBox)Controls.Find("cboGroups", true)[0];
                    if (row.Cells["Group_ID"].Value == DBNull.Value || Convert.ToInt32(row.Cells["Group_ID"].Value) == -1)
                        cboGroups.SelectedIndex = 0;
                    else
                        cboGroups.SelectedValue = row.Cells["Group_ID"].Value;
                }

                if (isSystemData == 1)
                {
                    btnInsert.Enabled = false;
                    btnDelete.Enabled = false;
                    lblTotalContacts.Text = "Chế độ: Dữ liệu nhân sự mặc định (Chỉ đọc)";
                    lblTotalContacts.ForeColor = Color.DarkRed;
                }
                else
                {
                    btnInsert.Enabled = true;
                    btnDelete.Enabled = true;
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

            if (Controls.Find("cboGroups", true).Length > 0)
            {
                ((ComboBox)Controls.Find("cboGroups", true)[0]).SelectedIndex = 0;
            }

            SwitchMode(editMode: false);
            txtName.Focus();
        }

        private async void btnInsert_Click(object sender, EventArgs e)
        {
            string nameInput = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(nameInput))
            {
                MessageBox.Show("Vui lòng điền tên liên hệ hoặc tên phòng ban!", "Dữ liệu trống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // THUẬT TOÁN TÁCH HỌ & TÊN: Giúp chia nhỏ trường Name thành Fname và Lname cho DB mới
            string fname = string.Empty;
            string lname = string.Empty;
            string[] nameParts = Regex.Split(nameInput, @"\s+");

            if (nameParts.Length == 1)
            {
                fname = nameParts[0]; // Chỉ có 1 từ -> coi như Tên
            }
            else
            {
                fname = nameParts[nameParts.Length - 1]; // Từ cuối cùng là Tên (Fname)
                lname = string.Join(" ", nameParts, 0, nameParts.Length - 1); // Các từ trước là Họ đệm (Lname)
            }

            string phoneInput = txtPhone.Text.Trim();
            string emailInput = txtEmail.Text.Trim();

            // Lấy Group_ID từ ComboBox an toàn
            int? groupId = null;
            if (Controls.Find("cboGroups", true).Length > 0)
            {
                var cboGroups = (ComboBox)Controls.Find("cboGroups", true)[0];
                if (cboGroups.SelectedValue != null && cboGroups.SelectedValue != DBNull.Value)
                {
                    groupId = Convert.ToInt32(cboGroups.SelectedValue);
                }
            }

            // Validate Regex Phone & Email
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
                string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
                if (!Regex.IsMatch(emailInput, emailPattern))
                {
                    MessageBox.Show("Định dạng địa chỉ Email không hợp lệ!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmail.Focus();
                    return;
                }
            }

            // Kiểm tra trùng lắp theo phạm vi dữ liệu của riêng User này
            int? currentExcludeId = _isEditMode ? (int?)_selectedContactId : null;
            if (await _contactRepo.IsPhoneOrEmailExistsAsync(phoneInput, emailInput, currentExcludeId, _currentUserId))
            {
                MessageBox.Show("Số điện thoại hoặc Email này đã tồn tại trong danh bạ của bạn!", "Trùng bản ghi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Thực thi gọi hàm Repo mới truyền đầy đủ cấu trúc tham số
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
                    // Xóa an toàn kẹp mã User bảo mật
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
                // Bảo mật: Sử dụng hàm Search tương thích bảo mật theo UserID
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