using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using ClassProject.DataAccess.Repositories;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ContactForm : Form
    {
        private readonly IContactRepository _contactRepo;
        private bool _isEditMode = false;
        private string _selectedUniqueId = string.Empty;
        private int _selectedContactId = -1;

        // Cơ chế Debounce chống overload truy vấn SQL Server khi gõ phím liên tục
        private readonly System.Windows.Forms.Timer _searchDebounceTimer;

        public ContactForm()
        {
            InitializeComponent();

            // Khởi tạo thông qua Interface (Chuẩn DI/IoC)
            _contactRepo = new ContactRepository();

            this.txtPhone.KeyPress += txtPhone_KeyPress;

            // Cấu hình bộ đếm thời gian Debounce: Chờ người dùng ngừng gõ 350ms mới gửi lệnh xuống DB
            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 350 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }

        private async void ContactForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
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

            if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên / Khối phòng ban";
            if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại liên lạc";
            if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";

            dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async Task LoadContactGridAsync()
        {
            try
            {
                dgvContacts.CellClick -= dgvContacts_CellClick;

                DataTable dt = await _contactRepo.GetAllContactsAsync();
                dgvContacts.DataSource = dt;
                FormatGridColumns();

                int total = dt?.Rows.Count ?? 0;
                if (lblTotalContacts != null)
                {
                    lblTotalContacts.Text = $"Tổng số liên lạc hệ thống: {total}";
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

            string phoneInput = txtPhone.Text.Trim();
            string emailInput = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(phoneInput))
            {
                string phonePattern = @"^(0[35789][0-9]{8})|(02[0-9]{8,9})$";
                if (!Regex.IsMatch(phoneInput, phonePattern))
                {
                    MessageBox.Show("Định dạng số điện thoại (Di động hoặc Máy bàn cố định) không hợp lệ!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            int? currentExcludeId = _isEditMode ? (int?)_selectedContactId : null;
            if (await _contactRepo.IsPhoneOrEmailExistsAsync(phoneInput, emailInput, currentExcludeId))
            {
                MessageBox.Show("Số điện thoại hoặc Email này đã tồn tại trong hệ thống!", "Trùng bản ghi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool result = _isEditMode
                    ? await _contactRepo.UpdateContactAsync(_selectedContactId, nameInput, phoneInput, emailInput)
                    : await _contactRepo.InsertContactAsync(nameInput, phoneInput, emailInput);

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

            if (MessageBox.Show("Bạn có chắc chắn muốn gỡ bỏ vĩnh viễn liên hệ này?", "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (await _contactRepo.DeleteContactAsync(_selectedContactId))
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

        // TỐI ƯU DOANH NGHIỆP: Thay vì gọi DB ngay, ta reset bộ đếm thời gian Debounce
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        // Thực thi tìm kiếm an toàn sau khi người dùng dừng gõ phím
        private async void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop(); // Dừng Timer để tránh lặp lại vòng lặp tick

            string keyword = txtSearch.Text.Trim();
            try
            {
                DataTable dt = string.IsNullOrEmpty(keyword)
                    ? await _contactRepo.GetAllContactsAsync()
                    : await _contactRepo.SearchContactsAsync(keyword);

                dgvContacts.DataSource = dt;
                FormatGridColumns();

                if (lblTotalContacts != null)
                {
                    lblTotalContacts.Text = string.IsNullOrEmpty(keyword)
                        ? $"Tổng số liên lạc hệ thống: {dt?.Rows.Count ?? 0}"
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
                e.SuppressKeyPress = true; // Khóa tiếng bíp hệ thống
                _searchDebounceTimer.Stop(); // Ngắt đếm ngược Debounce để chạy ngay lập tức
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