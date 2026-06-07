using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class AccountManageForm : Form
    {
        private My_DB db = new My_DB();
        private DataTable dtAccounts;

        public AccountManageForm()
        {
            InitializeComponent();
        }

        private void AccountManageForm_Load(object sender, EventArgs e)
        {
            // Làm sạch và gắn sự kiện
            dgvPendingUsers.CellContentClick -= dgvPendingUsers_CellContentClick;
            dgvPendingUsers.CellContentClick += dgvPendingUsers_CellContentClick;
            dgvPendingUsers.CellDoubleClick -= dgvPendingUsers_CellDoubleClick;
            dgvPendingUsers.CellDoubleClick += dgvPendingUsers_CellDoubleClick;

            dgvPendingUsers.Paint -= dgvPendingUsers_Paint;
            dgvPendingUsers.Paint += dgvPendingUsers_Paint;

            // Đăng ký sự kiện CellPainting để tự động vẽ Checkbox tổng lên tiêu đề cột "Chọn"
            dgvPendingUsers.CellPainting -= dgvPendingUsers_CellPainting;
            dgvPendingUsers.CellPainting += dgvPendingUsers_CellPainting;
            dgvPendingUsers.CellClick -= dgvPendingUsers_CellClick;
            dgvPendingUsers.CellClick += dgvPendingUsers_CellClick;
            dgvPendingUsers.CurrentCellDirtyStateChanged -= dgvPendingUsers_CurrentCellDirtyStateChanged;
            dgvPendingUsers.CurrentCellDirtyStateChanged += dgvPendingUsers_CurrentCellDirtyStateChanged;
            if (this.Controls.Find("txtSearchPending", true).Length > 0)
            {
                TextBox txtSearch = (TextBox)this.Controls.Find("txtSearchPending", true)[0];
                txtSearch.TextChanged -= TxtSearchPending_TextChanged;
                txtSearch.TextChanged += TxtSearchPending_TextChanged;
            }

            if (this.Controls.Find("btnBulkAccept", true).Length > 0)
            {
                Button btnAcceptAll = (Button)this.Controls.Find("btnBulkAccept", true)[0];
                btnAcceptAll.Visible = true;
                btnAcceptAll.Click -= BtnBulkAccept_Click;
                btnAcceptAll.Click += BtnBulkAccept_Click;
            }

            if (this.Controls.Find("btnBulkDelete", true).Length > 0)
            {
                Button btnDeleteAll = (Button)this.Controls.Find("btnBulkDelete", true)[0];
                btnDeleteAll.Visible = true;
                btnDeleteAll.Click -= BtnBulkDelete_Click;
                btnDeleteAll.Click += BtnBulkDelete_Click;
            }

            LoadAllAccounts();
        }

        private void LoadAllAccounts()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string query = @"SELECT u.Username, r.RoleName AS Position, u.Status 
                             FROM dbo.Users u 
                             INNER JOIN dbo.Roles r ON u.RoleId = r.Id 
                             WHERE u.RoleId != 0 AND u.Status != -1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        dtAccounts = new DataTable();
                        da.Fill(dtAccounts);

                        if (dgvPendingUsers.Columns["colSelect"] != null) dgvPendingUsers.Columns.Remove("colSelect");
                        if (dgvPendingUsers.Columns["colAccept"] != null) dgvPendingUsers.Columns.Remove("colAccept");
                        if (dgvPendingUsers.Columns["colLock"] != null) dgvPendingUsers.Columns.Remove("colLock");
                        if (dgvPendingUsers.Columns["colDelete"] != null) dgvPendingUsers.Columns.Remove("colDelete");

                        dgvPendingUsers.DataSource = dtAccounts;

                        dgvPendingUsers.AllowUserToAddRows = false;
                        dgvPendingUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvPendingUsers.RowHeadersVisible = false;

                        // Cấu hình phông nền tiêu đề hiện đại
                        dgvPendingUsers.EnableHeadersVisualStyles = false;
                        dgvPendingUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                        dgvPendingUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                        dgvPendingUsers.ColumnHeadersHeight = 32;

                        if (dgvPendingUsers.Columns["Username"] != null) dgvPendingUsers.Columns["Username"].HeaderText = "Tên tài khoản";
                        if (dgvPendingUsers.Columns["Position"] != null) dgvPendingUsers.Columns["Position"].HeaderText = "Chức vụ / Quyền";
                        if (dgvPendingUsers.Columns["Status"] != null) dgvPendingUsers.Columns["Status"].HeaderText = "Trạng thái hệ thống";

                        dgvPendingUsers.CellFormatting -= dgvPendingUsers_CellFormatting;
                        dgvPendingUsers.CellFormatting += dgvPendingUsers_CellFormatting;

                        DataGridButtonInit();

                        // 1. Cho phép chỉnh sửa trên tổng thể GridView
                        dgvPendingUsers.ReadOnly = false;

                        // 2. Chế độ kích hoạt chỉnh sửa ngay khi con trỏ chuột vừa đi vào ô (giúp tích nhạy hơn)
                        dgvPendingUsers.EditMode = DataGridViewEditMode.EditOnEnter;

                        // 3. Khóa không cho sửa nội dung chữ của các cột dữ liệu lấy từ SQL
                        if (dgvPendingUsers.Columns["Username"] != null) dgvPendingUsers.Columns["Username"].ReadOnly = true;
                        if (dgvPendingUsers.Columns["Position"] != null) dgvPendingUsers.Columns["Position"].ReadOnly = true;
                        if (dgvPendingUsers.Columns["Status"] != null) dgvPendingUsers.Columns["Status"].ReadOnly = true;

                        // 4. Đảm bảo cột Checkbox được phép tương tác tích chọn thoải mái
                        if (dgvPendingUsers.Columns["colSelect"] != null) dgvPendingUsers.Columns["colSelect"].ReadOnly = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị danh sách tài khoản: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridButtonInit()
        {
            if (dgvPendingUsers.Columns["colSelect"] == null)
            {
                DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
                checkCol.Name = "colSelect";
                checkCol.HeaderText = "";

                checkCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                checkCol.Width = 45;
                checkCol.Resizable = DataGridViewTriState.False;
                checkCol.SortMode = DataGridViewColumnSortMode.NotSortable;

                checkCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                checkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Insert(0, checkCol);
            }

            if (dgvPendingUsers.Columns["colAccept"] == null)
            {
                DataGridViewButtonColumn btnAccept = new DataGridViewButtonColumn();
                btnAccept.Name = "colAccept";
                btnAccept.HeaderText = "Duyệt";
                btnAccept.Text = "✔ Duyệt";
                btnAccept.UseColumnTextForButtonValue = true;
                btnAccept.FlatStyle = FlatStyle.Flat;
                btnAccept.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnAccept.Width = 85;
                btnAccept.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnAccept);
            }

            if (dgvPendingUsers.Columns["colLock"] == null)
            {
                DataGridViewButtonColumn btnLock = new DataGridViewButtonColumn();
                btnLock.Name = "colLock";
                btnLock.HeaderText = "Khóa hệ thống";
                btnLock.FlatStyle = FlatStyle.Flat;
                btnLock.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnLock.Width = 110;
                btnLock.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnLock);
            }

            if (dgvPendingUsers.Columns["colDelete"] == null)
            {
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.Name = "colDelete";
                btnDelete.HeaderText = "Xóa/Từ chối";
                btnDelete.FlatStyle = FlatStyle.Flat;
                btnDelete.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnDelete.Width = 110;
                btnDelete.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvPendingUsers.Columns.Add(btnDelete);
            }

            dgvPendingUsers.Columns["colSelect"].DisplayIndex = 0;
            dgvPendingUsers.Columns["colAccept"].DisplayIndex = dgvPendingUsers.Columns.Count - 3;
            dgvPendingUsers.Columns["colLock"].DisplayIndex = dgvPendingUsers.Columns.Count - 2;
            dgvPendingUsers.Columns["colDelete"].DisplayIndex = dgvPendingUsers.Columns.Count - 1;
        }

        private void dgvPendingUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPendingUsers.Rows.Count) return;

            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                int statusVal = Convert.ToInt32(e.Value);
                if (statusVal == 0) { e.Value = "⏳ Chờ phê duyệt"; e.CellStyle.ForeColor = Color.Orange; }
                else if (statusVal == 1) { e.Value = "🟢 Đang hoạt động"; e.CellStyle.ForeColor = Color.Green; }
                else if (statusVal == 2) { e.Value = "🔴 Đã khóa"; e.CellStyle.ForeColor = Color.Red; }
                else if (statusVal == 3) { e.Value = "❌ Đã từ chối"; e.CellStyle.ForeColor = Color.Gray; }
            }

            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "colLock")
            {
                var statusCell = dgvPendingUsers.Rows[e.RowIndex].Cells["Status"];
                if (statusCell != null && statusCell.Value != null)
                {
                    int statusVal = Convert.ToInt32(statusCell.Value);
                    e.Value = (statusVal == 2) ? "🔓 Mở khóa" : "🔒 Khóa";
                }
            }

            if (dgvPendingUsers.Columns[e.ColumnIndex].Name == "colDelete")
            {
                var statusCell = dgvPendingUsers.Rows[e.RowIndex].Cells["Status"];
                if (statusCell != null && statusCell.Value != null)
                {
                    int statusVal = Convert.ToInt32(statusCell.Value);
                    e.Value = (statusVal == 0) ? "✖ Từ chối" : "🗑 Xóa mềm";
                }
            }
        }

        private void TxtSearchPending_TextChanged(object sender, EventArgs e)
        {
            if (dtAccounts == null) return;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            string keyword = txt.Text.Replace("'", "''");
            dtAccounts.DefaultView.RowFilter = string.Format("Username LIKE '%{0}%' OR Position LIKE '%{0}%'", keyword);
        }

        private void dgvPendingUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPendingUsers.Rows.Count || e.ColumnIndex < 0) return;

            string colName = dgvPendingUsers.Columns[e.ColumnIndex].Name;
            string selectedUsername = dgvPendingUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
            int currentStatus = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["Status"].Value);

            if (colName == "colAccept")
            {
                if (currentStatus == 1)
                {
                    MessageBox.Show("Tài khoản này đã ở trạng thái hoạt động!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string query = "UPDATE dbo.Users SET Status = 1 WHERE Username = @user";
                ExecuteDatabaseQuery(query, selectedUsername, $"Kích hoạt thành công tài khoản: {selectedUsername}");
            }
            else if (colName == "colLock")
            {
                if (currentStatus == 0 || currentStatus == 3)
                {
                    MessageBox.Show("Tài khoản chưa phê duyệt hoặc đã bị từ chối, không thể thực hiện khóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int targetStatus = (currentStatus == 2) ? 1 : 2;
                string confirmText = (targetStatus == 2) ? $"Xác nhận KHÓA tài khoản {selectedUsername}?" : $"Xác nhận MỞ KHÓA tài khoản {selectedUsername}?";
                string successText = (targetStatus == 2) ? $"Đã khóa tài khoản: {selectedUsername}" : $"Đã mở khóa tài khoản: {selectedUsername}";

                if (MessageBox.Show(confirmText, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = "UPDATE dbo.Users SET Status = @targetStatus WHERE Username = @user";
                    ExecuteDatabaseQueryWithParam(query, selectedUsername, targetStatus, successText);
                }
            }
            else if (colName == "colDelete")
            {
                if (currentStatus == 0)
                {
                    if (MessageBox.Show($"Từ chối yêu cầu đăng ký của tài khoản {selectedUsername}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string query = "UPDATE dbo.Users SET Status = 3 WHERE Username = @user";
                        ExecuteDatabaseQuery(query, selectedUsername, $"Đã từ chối tài khoản {selectedUsername}");
                    }
                }
                else
                {
                    if (MessageBox.Show($"Bạn có chắc chắn muốn XÓA MỀM tài khoản {selectedUsername}? (Dữ liệu lịch sử, bảng điểm vẫn được lưu trữ an toàn trong hệ thống)", "Xác nhận xóa an toàn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        string query = "UPDATE dbo.Users SET Status = -1 WHERE Username = @user";
                        ExecuteDatabaseQuery(query, selectedUsername, $"Đã xóa mềm tài khoản {selectedUsername} khỏi danh sách hiển thị.");
                    }
                }
            }
        }

        private void ExecuteDatabaseQuery(string query, string username, string successNotify)
        {
            ExecuteDatabaseQueryWithParam(query, username, null, successNotify);
        }

        private void ExecuteDatabaseQueryWithParam(string query, string username, int? status, string successNotify)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        if (status.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@targetStatus", status.Value);
                        }
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show(successNotify, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAllAccounts();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý Database: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            List<string> selectedUsers = new List<string>();
            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (row.Cells["colSelect"] != null && Convert.ToBoolean(row.Cells["colSelect"].Value) == true)
                {
                    if (Convert.ToInt32(row.Cells["Status"].Value) == 0)
                    {
                        selectedUsers.Add(row.Cells["Username"].Value.ToString());
                    }
                }
            }

            if (selectedUsers.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản 'Chờ phê duyệt'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận duyệt hàng loạt {selectedUsers.Count} tài khoản đã chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ExecuteBulkStatusUpdate(selectedUsers, 1, "Đã phê duyệt hàng loạt các tài khoản thành công!");
            }
        }

        private void BtnBulkDelete_Click(object sender, EventArgs e)
        {
            List<string> selectedPendingUsers = new List<string>();
            List<string> selectedActiveUsers = new List<string>();

            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (row.Cells["colSelect"] != null && Convert.ToBoolean(row.Cells["colSelect"].Value) == true)
                {
                    string uName = row.Cells["Username"].Value.ToString();
                    int status = Convert.ToInt32(row.Cells["Status"].Value);

                    if (status == 0) selectedPendingUsers.Add(uName);
                    else if (status == 1 || status == 2) selectedActiveUsers.Add(uName);
                }
            }

            if (selectedPendingUsers.Count == 0 && selectedActiveUsers.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string confirmMsg = $"Bạn có chắc chắn muốn xử lý hàng loạt các tài khoản đã chọn?\n" +
                               $"- Từ chối {selectedPendingUsers.Count} tài khoản đăng ký mới.\n" +
                               $"- Xóa mềm {selectedActiveUsers.Count} tài khoản đang hoạt động (Bảo toàn dữ liệu lịch sử).";

            if (MessageBox.Show(confirmMsg, "Cảnh báo xử lý hàng loạt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (selectedPendingUsers.Count > 0)
                    ExecuteBulkStatusUpdate(selectedPendingUsers, 3, null);

                if (selectedActiveUsers.Count > 0)
                    ExecuteBulkStatusUpdate(selectedActiveUsers, -1, null);

                MessageBox.Show("Đã xử lý thay đổi trạng thái hàng loạt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (this.Controls.Find("txtSearchPending", true).Length > 0)
                {
                    ((TextBox)this.Controls.Find("txtSearchPending", true)[0]).Clear();
                }
                LoadAllAccounts();
            }
        }

        private void ExecuteBulkStatusUpdate(List<string> usernames, int targetStatus, string successMessage)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"UPDATE dbo.Users SET Status = {targetStatus} WHERE Username IN (");

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        for (int i = 0; i < usernames.Count; i++)
                        {
                            string paramName = "@u" + i;
                            sb.Append(paramName);
                            if (i < usernames.Count - 1) sb.Append(",");

                            cmd.Parameters.AddWithValue(paramName, usernames[i]);
                        }
                        sb.Append(")");

                        cmd.CommandText = sb.ToString();
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();

                        if (!string.IsNullOrEmpty(successMessage))
                        {
                            MessageBox.Show(successMessage, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAllAccounts();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý hàng loạt: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TỰ ĐỘNG VẼ Ô "CHỌN TẤT CẢ" LÊN TIÊU ĐỀ GRIDVIEW
        private bool isHeaderChecked = false;

        private void dgvPendingUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Kiểm tra xem có phải ô tiêu đề của cột "colSelect" hay không
            if (e.RowIndex == -1 && e.ColumnIndex >= 0 && dgvPendingUsers.Columns[e.ColumnIndex].Name == "colSelect")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                // Xác định kích thước và vị trí chính giữa của ô Checkbox tổng
                Size checkboxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                Point pt = new Point(
                    e.CellBounds.X + (e.CellBounds.Width - checkboxSize.Width) / 2,
                    e.CellBounds.Y + (e.CellBounds.Height - checkboxSize.Height) / 2
                );

                // Vẽ trạng thái Checkbox dựa trên biến điều khiển toàn cục
                System.Windows.Forms.VisualStyles.CheckBoxState state = isHeaderChecked ?
                    System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal :
                    System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;

                CheckBoxRenderer.DrawCheckBox(e.Graphics, pt, state);
                e.Handled = true;
            }
        }

        private void dgvPendingUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Người dùng click vào tiêu đề cột Chọn
            if (e.RowIndex == -1 && e.ColumnIndex >= 0 && dgvPendingUsers.Columns[e.ColumnIndex].Name == "colSelect")
            {
                isHeaderChecked = !isHeaderChecked;
                dgvPendingUsers.InvalidateCell(e.ColumnIndex, e.RowIndex);

                // Cập nhật tích chọn/bỏ tích toàn bộ các hàng bên dưới
                foreach (DataGridViewRow row in dgvPendingUsers.Rows)
                {
                    row.Cells["colSelect"].Value = isHeaderChecked;
                }
                dgvPendingUsers.RefreshEdit();
            }
        }

        private void dgvPendingUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Đảm bảo click vào hàng hợp lệ và KHÔNG click trúng các cột nút bấm hành động hoặc Checkbox
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string colName = dgvPendingUsers.Columns[e.ColumnIndex].Name;
                if (colName != "colSelect" && colName != "colAccept" && colName != "colLock" && colName != "colDelete")
                {
                    if (dgvPendingUsers.Rows[e.RowIndex].Cells["Username"]?.Value != null)
                    {
                        string selectedUsername = dgvPendingUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();

                        // Khởi tạo Form chi tiết và truyền Username sang
                        AccountDetailForm detailForm = new AccountDetailForm(selectedUsername);
                        detailForm.ShowDialog();
                    }
                }
            }
        }
        // Sự kiện ép DataGridView cập nhật giá trị Checkbox ngay lập tức khi người dùng vừa tích chọn
        private void dgvPendingUsers_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPendingUsers.IsCurrentCellDirty && dgvPendingUsers.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvPendingUsers.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}