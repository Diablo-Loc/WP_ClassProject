using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassProject.DataAccess.Repositories.Implementations;
using ClassProject.DataAccess.DTOs;
using ClassProject.DataAccess.Repositories.Interfaces;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class AccountManageForm : Form
    {
        private readonly IAccountRepository _accountRepository;
        private List<UserDTO> _originalAccountList; // Lưu danh sách gốc để hỗ trợ tìm kiếm Local mượt mà
        private bool isHeaderChecked = false;       // Trạng thái nút chọn tất cả trên Header

        public AccountManageForm()
        {
            InitializeComponent();
            _accountRepository = new AccountRepository();
        }

        private void AccountManageForm_Load(object sender, EventArgs e)
        {
            // Hủy đăng ký để tránh trùng lặp sự kiện khi Load lại Form
            dgvPendingUsers.CellContentClick -= dgvPendingUsers_CellContentClick;
            dgvPendingUsers.CellContentClick += dgvPendingUsers_CellContentClick;
            dgvPendingUsers.CellDoubleClick -= dgvPendingUsers_CellDoubleClick;
            dgvPendingUsers.CellDoubleClick += dgvPendingUsers_CellDoubleClick;
            dgvPendingUsers.Paint -= dgvPendingUsers_Paint;
            dgvPendingUsers.Paint += dgvPendingUsers_Paint;
            dgvPendingUsers.CellPainting -= dgvPendingUsers_CellPainting;
            dgvPendingUsers.CellPainting += dgvPendingUsers_CellPainting;
            dgvPendingUsers.CellClick -= dgvPendingUsers_CellClick;
            dgvPendingUsers.CellClick += dgvPendingUsers_CellClick;
            dgvPendingUsers.CurrentCellDirtyStateChanged -= dgvPendingUsers_CurrentCellDirtyStateChanged;
            dgvPendingUsers.CurrentCellDirtyStateChanged += dgvPendingUsers_CurrentCellDirtyStateChanged;

            // Ánh xạ an toàn các Control tìm kiếm và nút bấm từ Designer hệ thống cũ
            BindControlEvents();

            // Tải dữ liệu lên Grid
            LoadAllAccounts();
        }

        private void BindControlEvents()
        {
            if (this.Controls.Find("txtSearchPending", true).Length > 0)
            {
                TextBox txtSearch = (TextBox)this.Controls.Find("txtSearchPending", true)[0];
                txtSearch.TextChanged -= TxtSearchPending_TextChanged;
                txtSearch.TextChanged += TxtSearchPending_TextChanged;
            }

            if (this.Controls.Find("btnBulkAccept", true).Length > 0)
            {
                Button btnAcceptAll = (Button)this.Controls.Find("btnBulkAccept", true)[0];
                btnAcceptAll.Click -= BtnBulkAccept_Click;
                btnAcceptAll.Click += BtnBulkAccept_Click;
            }

            if (this.Controls.Find("btnBulkDelete", true).Length > 0)
            {
                Button btnDeleteAll = (Button)this.Controls.Find("btnBulkDelete", true)[0];
                btnDeleteAll.Click -= BtnBulkDelete_Click;
                btnDeleteAll.Click += BtnBulkDelete_Click;
            }
        }

        private void LoadAllAccounts()
        {
            try
            {
                // BẢO MẬT: Lấy danh sách tài khoản, loại bỏ Admin (RoleId != 0) và tài khoản đã bị xóa mềm (Status != -1)
                _originalAccountList = _accountRepository.GetAllAccounts()
                                        .Where(u => u.Status != -1 && u.RoleId != 0)
                                        .ToList();

                // Làm sạch các cột tự tạo cũ tránh bị nhân bản cột khi refresh
                string[] customCols = { "colSelect", "colAccept", "colLock", "colDelete" };
                foreach (var col in customCols)
                {
                    if (dgvPendingUsers.Columns[col] != null)
                        dgvPendingUsers.Columns.Remove(col);
                }

                // Gán dữ liệu nguồn
                dgvPendingUsers.DataSource = null;
                dgvPendingUsers.DataSource = _originalAccountList;

                // Cấu hình hiển thị chuẩn UI
                dgvPendingUsers.AllowUserToAddRows = false;
                dgvPendingUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvPendingUsers.RowHeadersVisible = false;
                dgvPendingUsers.EnableHeadersVisualStyles = false;
                dgvPendingUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvPendingUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                dgvPendingUsers.ColumnHeadersHeight = 32;

                // Định danh lại Header của DTO
                if (dgvPendingUsers.Columns["Username"] != null) dgvPendingUsers.Columns["Username"].HeaderText = "Tên tài khoản";
                if (dgvPendingUsers.Columns["RoleName"] != null) dgvPendingUsers.Columns["RoleName"].HeaderText = "Chức vụ / Quyền";
                if (dgvPendingUsers.Columns["Status"] != null) dgvPendingUsers.Columns["Status"].HeaderText = "Trạng thái hệ thống";

                // Ẩn các cột bổ trợ kỹ thuật của DTO không cần hiển thị lên Grid diện rộng
                string[] hiddenCols = { "Id", "Email", "RoleId", "Valid", "FailedAttempts", "LockoutEnd", "LastLogin", "CreatedAt" };
                foreach (var col in hiddenCols)
                {
                    if (dgvPendingUsers.Columns[col] != null) dgvPendingUsers.Columns[col].Visible = false;
                }

                dgvPendingUsers.CellFormatting -= dgvPendingUsers_CellFormatting;
                dgvPendingUsers.CellFormatting += dgvPendingUsers_CellFormatting;

                // Khởi tạo lại các cột nút bấm chức năng nhanh
                DataGridButtonInit();

                // Phân quyền tương tác trên ô dữ liệu
                dgvPendingUsers.ReadOnly = false;
                dgvPendingUsers.EditMode = DataGridViewEditMode.EditOnEnter;

                foreach (DataGridViewColumn col in dgvPendingUsers.Columns)
                {
                    if (col.Name != "colSelect") col.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách tài khoản từ hệ thống: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridButtonInit()
        {
            if (dgvPendingUsers.Columns["colSelect"] == null)
            {
                DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn
                {
                    Name = "colSelect",
                    HeaderText = "",
                    Width = 45,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                checkCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                checkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Insert(0, checkCol);
            }

            if (dgvPendingUsers.Columns["colAccept"] == null)
            {
                DataGridViewButtonColumn btnAccept = new DataGridViewButtonColumn
                {
                    Name = "colAccept",
                    HeaderText = "Duyệt",
                    Text = "✔ Duyệt",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Flat,
                    Width = 85
                };
                btnAccept.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnAccept);
            }

            if (dgvPendingUsers.Columns["colLock"] == null)
            {
                DataGridViewButtonColumn btnLock = new DataGridViewButtonColumn
                {
                    Name = "colLock",
                    HeaderText = "Khóa hệ thống",
                    FlatStyle = FlatStyle.Flat,
                    Width = 110
                };
                btnLock.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnLock);
            }

            if (dgvPendingUsers.Columns["colDelete"] == null)
            {
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
                {
                    Name = "colDelete",
                    HeaderText = "Xóa/Từ chối",
                    FlatStyle = FlatStyle.Flat,
                    Width = 110
                };
                btnDelete.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnDelete);
            }

            // Sắp xếp thứ tự hiển thị trực quan các cột tính năng
            dgvPendingUsers.Columns["colSelect"].DisplayIndex = 0;
            dgvPendingUsers.Columns["colAccept"].DisplayIndex = dgvPendingUsers.Columns.Count - 3;
            dgvPendingUsers.Columns["colLock"].DisplayIndex = dgvPendingUsers.Columns.Count - 2;
            dgvPendingUsers.Columns["colDelete"].DisplayIndex = dgvPendingUsers.Columns.Count - 1;
        }

        private void dgvPendingUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPendingUsers.Rows.Count) return;

            // ĐỒNG BỘ LOGIC HIỂN THỊ TRỰC QUAN CHUẨN:
            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                int statusVal = Convert.ToInt32(e.Value);
                int validVal = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["Valid"].Value ?? 0);

                if (validVal == 0)
                {
                    e.Value = "⏳ Chờ phê duyệt";
                    e.CellStyle.ForeColor = Color.Orange;
                }
                else if (validVal == 2)
                {
                    e.Value = "❌ Đã từ chối";
                    e.CellStyle.ForeColor = Color.Gray;
                }
                else // Khi Valid == 1 (Đã duyệt)
                {
                    if (statusVal == 0) { e.Value = "🟢 Đang hoạt động"; e.CellStyle.ForeColor = Color.Green; }
                    else if (statusVal == 1) { e.Value = "🔴 Đã khóa"; e.CellStyle.ForeColor = Color.Red; }
                }
            }

            // Đổi text hiển thị động cho nút bấm Khóa/Mở khóa
            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "colLock")
            {
                var statusCell = dgvPendingUsers.Rows[e.RowIndex].Cells["Status"];
                if (statusCell != null && statusCell.Value != null)
                {
                    int statusVal = Convert.ToInt32(statusCell.Value);
                    e.Value = (statusVal == 1) ? "🔓 Mở khóa" : "🔒 Khóa";
                }
            }

            // Đổi text hiển thị động cho nút bấm Từ chối / Xóa mềm
            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "colDelete")
            {
                var validCell = dgvPendingUsers.Rows[e.RowIndex].Cells["Valid"];
                if (validCell != null && Convert.ToInt32(validCell.Value) == 0)
                {
                    e.Value = "✖ Từ chối";
                }
                else
                {
                    e.Value = "🗑 Xóa mềm";
                }
            }
        }

        private void TxtSearchPending_TextChanged(object sender, EventArgs e)
        {
            if (_originalAccountList == null) return;
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            string keyword = txt.Text.Trim().ToLower();

            var filteredList = _originalAccountList.Where(u =>
                u.Username.ToLower().Contains(keyword) ||
                u.RoleName.ToLower().Contains(keyword)
            ).ToList();

            // Tạm ngắt các sự kiện định dạng nếu cần để tăng hiệu năng (không bắt buộc)
            dgvPendingUsers.DataSource = null;
            dgvPendingUsers.DataSource = filteredList;

            // 💡 GIẢI PHÁP: Ẩn lại các cột kỹ thuật ngay sau khi gán DataSource mới
            string[] hiddenCols = { "Id", "Email", "RoleId", "Valid", "FailedAttempts", "LockoutEnd", "LastLogin", "CreatedAt" };
            foreach (var col in hiddenCols)
            {
                if (dgvPendingUsers.Columns[col] != null)
                    dgvPendingUsers.Columns[col].Visible = false;
            }

            // Định danh lại Header tiếng Việt cho danh sách sau khi lọc
            if (dgvPendingUsers.Columns["Username"] != null) dgvPendingUsers.Columns["Username"].HeaderText = "Tên tài khoản";
            if (dgvPendingUsers.Columns["RoleName"] != null) dgvPendingUsers.Columns["RoleName"].HeaderText = "Chức vụ / Quyền";
            if (dgvPendingUsers.Columns["Status"] != null) dgvPendingUsers.Columns["Status"].HeaderText = "Trạng thái hệ thống";

            // Khởi tạo lại nút bấm chức năng (Hàm cũ của bạn)
            DataGridButtonInit();
        }

        private void dgvPendingUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPendingUsers.Rows.Count || e.ColumnIndex < 0) return;

            string colName = dgvPendingUsers.Columns[e.ColumnIndex].Name;
            string selectedUsername = dgvPendingUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
            int currentStatus = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["Status"].Value);
            int currentValid = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["Valid"].Value);

            // Xử lý Hành động nút Duyệt tài khoản
            if (colName == "colAccept")
            {
                if (currentValid == 1 && currentStatus == 0)
                {
                    MessageBox.Show("Tài khoản này đã ở trạng thái hoạt động bình thường!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_accountRepository.UpdateSingleStatus(selectedUsername, 0, 1))
                {
                    MessageBox.Show($"Kích hoạt thành công tài khoản: {selectedUsername}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllAccounts();
                }
            }
            // Xử lý Hành động nút Khóa/Mở Khóa
            else if (colName == "colLock")
            {
                if (currentValid == 0)
                {
                    MessageBox.Show("Tài khoản chưa được phê duyệt, không thể thực hiện khóa hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int targetStatus = (currentStatus == 1) ? 0 : 1;
                string confirmText = (targetStatus == 1) ? $"Xác nhận KHÓA tài khoản {selectedUsername}?" : $"Xác nhận MỞ KHÓA tài khoản {selectedUsername}?";
                string successText = (targetStatus == 1) ? $"Đã khóa tài khoản: {selectedUsername}" : $"Đã mở khóa tài khoản: {selectedUsername}";

                if (MessageBox.Show(confirmText, "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_accountRepository.UpdateSingleStatus(selectedUsername, targetStatus, 1))
                    {
                        MessageBox.Show(successText, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllAccounts();
                    }
                }
            }
            // Xử lý Hành động nút Xóa mềm / Từ chối đăng ký
            else if (colName == "colDelete")
            {
                if (currentValid == 0)
                {
                    if (MessageBox.Show($"Từ chối yêu cầu đăng ký của tài khoản {selectedUsername}?", "Xác nhận từ chối", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (_accountRepository.UpdateSingleStatus(selectedUsername, 0, 2))
                        {
                            MessageBox.Show($"Đã từ chối tài khoản {selectedUsername}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAllAccounts();
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show($"Bạn có chắc chắn muốn XÓA MỀM tài khoản {selectedUsername}?", "Xác nhận xóa an toàn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        if (_accountRepository.UpdateSingleStatus(selectedUsername, -1, currentValid))
                        {
                            MessageBox.Show($"Đã xóa mềm tài khoản {selectedUsername} khỏi danh sách hiển thị quản trị.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAllAccounts();
                        }
                    }
                }
            }
        }

        private void dgvPendingUsers_Paint(object sender, PaintEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && dgv.Rows.Count == 0)
            {
                string message = "Hệ thống không tìm thấy dữ liệu tài khoản phù hợp.";
                using (Font font = new Font("Segoe UI", 12, FontStyle.Italic))
                {
                    SizeF size = e.Graphics.MeasureString(message, font);
                    float x = (dgv.Width - size.Width) / 2;
                    float y = (dgv.Height - size.Height) / 2;
                    e.Graphics.DrawString(message, font, Brushes.DimGray, x, y);
                }
            }
        }

        private void BtnBulkAccept_Click(object sender, EventArgs e)
        {
            // Đảm bảo các dòng đang edit dở dang được lưu lại trước khi quét
            dgvPendingUsers.EndEdit();

            List<string> selectedUsers = new List<string>();
            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (row.Cells["colSelect"]?.Value != null)
                {
                    bool.TryParse(row.Cells["colSelect"].Value.ToString(), out bool isChecked);
                    if (isChecked)
                    {
                        int validVal = Convert.ToInt32(row.Cells["Valid"].Value ?? 0);
                        // Chỉ duyệt các tài khoản thực sự đang 'Chờ phê duyệt' (Valid == 0)
                        if (validVal == 0 && row.Cells["Username"]?.Value != null)
                        {
                            selectedUsers.Add(row.Cells["Username"].Value.ToString());
                        }
                        if (validVal == 1 && row.Cells["Username"]?.Value != null)
                        {
                            selectedUsers.Add(row.Cells["Username"].Value.ToString());
                        }
                    }
                }
            }

            if (selectedUsers.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản ở trạng thái 'Chờ phê duyệt'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận phê duyệt hàng loạt {selectedUsers.Count} tài khoản đã chọn?", "Xác nhận hàng loạt", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_accountRepository.UpdateBulkStatus(selectedUsers, 0, 1))
                {
                    MessageBox.Show("Đã phê duyệt hàng loạt các tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset lại trạng thái nút chọn tất cả trên Header về false khi load lại
                    isHeaderChecked = false;
                    LoadAllAccounts();
                }
            }
        }

        private void BtnBulkDelete_Click(object sender, EventArgs e)
        {
            dgvPendingUsers.EndEdit();

            List<string> selectedPendingUsers = new List<string>();
            List<string> selectedActiveUsers = new List<string>();

            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (row.Cells["colSelect"]?.Value != null)
                {
                    bool.TryParse(row.Cells["colSelect"].Value.ToString(), out bool isChecked);
                    if (isChecked)
                    {
                        if (row.Cells["Username"]?.Value == null) continue;

                        string uName = row.Cells["Username"].Value.ToString();
                        int valid = Convert.ToInt32(row.Cells["Valid"].Value ?? 0);

                        if (valid == 0) selectedPendingUsers.Add(uName);
                        else selectedActiveUsers.Add(uName);
                    }
                }
            }

            if (selectedPendingUsers.Count == 0 && selectedActiveUsers.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản để xử lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string confirmMsg = $"Bạn có chắc chắn muốn xử lý hàng loạt các tài khoản đã chọn?\n" +
                                $"- Từ chối {selectedPendingUsers.Count} yêu cầu đăng ký mới.\n" +
                                $"- Xóa mềm {selectedActiveUsers.Count} tài khoản đang hoạt động hệ thống.";

            if (MessageBox.Show(confirmMsg, "Cảnh báo xử lý diện rộng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                bool isSuccess = false;

                if (selectedPendingUsers.Count > 0)
                    isSuccess = _accountRepository.UpdateBulkStatus(selectedPendingUsers, 0, 2);

                if (selectedActiveUsers.Count > 0)
                    isSuccess = _accountRepository.UpdateBulkStatus(selectedActiveUsers, -1, 1);

                MessageBox.Show("Đã xử lý thay đổi trạng thái hàng loạt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (this.Controls.Find("txtSearchPending", true).Length > 0)
                {
                    ((TextBox)this.Controls.Find("txtSearchPending", true)[0]).Clear();
                }

                isHeaderChecked = false; // Reset trạng thái Header Checkbox
                LoadAllAccounts();
            }
        }

        private void dgvPendingUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0 && dgvPendingUsers.Columns[e.ColumnIndex].Name == "colSelect")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                Size checkboxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                Point pt = new Point(
                    e.CellBounds.X + (e.CellBounds.Width - checkboxSize.Width) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - checkboxSize.Height) / 2
                );

                System.Windows.Forms.VisualStyles.CheckBoxState state = isHeaderChecked ?
                    System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;

                CheckBoxRenderer.DrawCheckBox(e.Graphics, pt, state);
                e.Handled = true;
            }
        }

        private void dgvPendingUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0 && dgvPendingUsers.Columns[e.ColumnIndex].Name == "colSelect")
            {
                isHeaderChecked = !isHeaderChecked;
                dgvPendingUsers.InvalidateCell(e.ColumnIndex, e.RowIndex);

                // Đưa Grid về trạng thái an toàn trước khi gán hàng loạt
                dgvPendingUsers.EndEdit();

                foreach (DataGridViewRow row in dgvPendingUsers.Rows)
                {
                    row.Cells["colSelect"].Value = isHeaderChecked;
                }

                // Ép dữ liệu lưu xuống bộ nhớ đệm ngay lập tức
                dgvPendingUsers.EndEdit();
            }
        }

        private void dgvPendingUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string colName = dgvPendingUsers.Columns[e.ColumnIndex].Name;
                if (colName != "colSelect" && colName != "colAccept" && colName != "colLock" && colName != "colDelete")
                {
                    if (dgvPendingUsers.Rows[e.RowIndex].Cells["Username"]?.Value != null)
                    {
                        string selectedUsername = dgvPendingUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
                        AccountDetailForm detailForm = new AccountDetailForm(selectedUsername);
                        detailForm.ShowDialog();
                    }
                }
            }
        }

        private void dgvPendingUsers_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPendingUsers.IsCurrentCellDirty && dgvPendingUsers.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvPendingUsers.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}