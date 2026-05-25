using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class MainForm : Form
    {
        private int roleId;
        private int userId;
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();
        private DataTable dtPending;

        public MainForm(int roleId, int userId)
        {
            InitializeComponent();
            this.roleId = roleId;
            this.userId = userId;
            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);

            dgvPendingUsers.CellContentClick += DgvPendingUsers_CellContentClick;
            dgvPendingUsers.Paint += DgvPendingUsers_Paint;

            if (this.Controls.Find("txtSearchPending", true).Length > 0)
                txtSearchPending.TextChanged += TxtSearchPending_TextChanged;
            txtSearchPending.ForeColor = Color.Gray;
            if (this.Controls.Find("btnBulkAccept", true).Length > 0)
                btnBulkAccept.Click += BtnBulkAccept_Click;
            if (this.Controls.Find("btnBulkDelete", true).Length > 0)
                btnBulkDelete.Click += BtnBulkDelete_Click;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 1. HIỂN THỊ ROLE LÊN GIAO DIỆN
            if (roleId == 0)
            {
                lblRole.Text = "Quyền: ADMIN";
                lblRole.ForeColor = Color.Red;
            }
            else if (roleId == 1)
            {
                lblRole.Text = "Quyền: STUDENT";
                lblRole.ForeColor = Color.Gray;
            }
            else
            {
                lblRole.Text = "Quyền: HR (GIẢNG VIÊN)";
                lblRole.ForeColor = Color.Orange;
            }

            // 2. PHÂN QUYỀN HIỂN THỊ UI (Menu + Khu vực bên dưới)
            if (roleId == 0) // Là ADMIN
            {
                if (adminToolStripMenuItem != null) adminToolStripMenuItem.Visible = true;

                // Admin thì xem bảng duyệt tài khoản, ẩn biểu đồ
                dgvPendingUsers.Visible = true;
                chkSelectAll.Visible = true;
                if (picChart != null) picChart.Visible = false;

                //Đổi giao diện khối màu cho ADMIN
                lblTotalStudents.Text = "Tổng số SV: 0";
                lblMaleStudents.Text = "Chờ duyệt: 0";
                lblFemaleStudents.Text = "Đã duyệt: 0";

                //Hiện thanh tìm kiếm và nút bấm cho Admin ---
                if (this.Controls.Find("txtSearchPending", true).Length > 0) txtSearchPending.Visible = true;
                if (this.Controls.Find("btnBulkAccept", true).Length > 0) btnBulkAccept.Visible = true;
                if (this.Controls.Find("btnBulkDelete", true).Length > 0) btnBulkDelete.Visible = true;

                LoadPendingUsers(); // Tải danh sách chờ duyệt
            }
            else // Là STUDENT hoặc HR
            {
                if (adminToolStripMenuItem != null) adminToolStripMenuItem.Visible = false;

                // Không phải Admin thì xem biểu đồ, ẩn bảng duyệt
                dgvPendingUsers.Visible = false;
                chkSelectAll.Visible = false;
                if (picChart != null) picChart.Visible = true;

                // Ẩn thanh tìm kiếm và nút bấm ---
                if (this.Controls.Find("txtSearchPending", true).Length > 0) txtSearchPending.Visible = false;
                if (this.Controls.Find("btnBulkAccept", true).Length > 0) btnBulkAccept.Visible = false;
                if (this.Controls.Find("btnBulkDelete", true).Length > 0) btnBulkDelete.Visible = false;
            }

            // 3. Nạp số liệu Thống kê & Vẽ biểu đồ (nếu picChart đang hiển thị)
            LoadDashboard();
        }

        // KHU VỰC 1: XỬ LÝ ADMIN DUYỆT TÀI KHOẢN (UC-04)
        private void LoadPendingUsers()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    // SQL CÁCH 2: Chỉ tương tác bảng Users + Roles, loại bỏ hoàn toàn bảng Students dữ liệu gốc
                    // Thêm điều kiện u.RoleId != 0 để không hiển thị chính tài khoản Admin trong danh sách chờ duyệt
                    string query = @"SELECT u.Id, u.Username, u.Email, r.RoleName, u.Created_At 
                                     FROM dbo.Users u
                                     INNER JOIN dbo.Roles r ON u.RoleId = r.Id
                                     WHERE u.Valid = 0 AND u.RoleId != 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        dtPending = new DataTable();
                        da.Fill(dtPending);

                        dgvPendingUsers.DataSource = dtPending;

                        // 1. Cấu hình UI cơ bản
                        dgvPendingUsers.AllowUserToAddRows = false;
                        dgvPendingUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvPendingUsers.RowHeadersVisible = false;

                        // 2. Ẩn Id và Đổi tên tiêu đề hiển thị cho thân thiện (Giữ nguyên tên cột gốc dưới code)
                        if (dgvPendingUsers.Columns["Id"] != null) dgvPendingUsers.Columns["Id"].Visible = false;
                        if (dgvPendingUsers.Columns["Username"] != null) dgvPendingUsers.Columns["Username"].HeaderText = "Tên tài khoản";
                        if (dgvPendingUsers.Columns["Email"] != null) dgvPendingUsers.Columns["Email"].HeaderText = "Email liên hệ";
                        if (dgvPendingUsers.Columns["RoleName"] != null) dgvPendingUsers.Columns["RoleName"].HeaderText = "Quyền yêu cầu";
                        if (dgvPendingUsers.Columns["Created_At"] != null) dgvPendingUsers.Columns["Created_At"].HeaderText = "Ngày đăng ký";

                        if (dgvPendingUsers.Columns["colSelect"] == null)
                        {
                            DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
                            checkCol.Name = "colSelect";
                            checkCol.HeaderText = "Chọn";
                            checkCol.Width = 50;
                            checkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            checkCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dgvPendingUsers.Columns.Insert(0, checkCol);
                        }

                        if (dgvPendingUsers.Columns["colAccept"] is DataGridViewButtonColumn btnAccept)
                        {
                            btnAccept.UseColumnTextForButtonValue = true;
                            btnAccept.Text = "✔ Duyệt";
                            btnAccept.FlatStyle = FlatStyle.Flat;
                            btnAccept.DisplayIndex = dgvPendingUsers.Columns.Count - 1;
                        }

                        if (dgvPendingUsers.Columns["colDelete"] is DataGridViewButtonColumn btnDelete)
                        {
                            btnDelete.UseColumnTextForButtonValue = true;
                            btnDelete.Text = "✖ Từ chối";
                            btnDelete.FlatStyle = FlatStyle.Flat;
                            btnDelete.DisplayIndex = dgvPendingUsers.Columns.Count - 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách chờ duyệt: " + ex.Message);
            }
        }

        //Tìm kiếm dữ liệu
        private void TxtSearchPending_TextChanged(object sender, EventArgs e)
        {
            if (dtPending == null) return;
            string keyword = txtSearchPending.Text.Replace("'", "''");
            dtPending.DefaultView.RowFilter = string.Format("Username LIKE '%{0}%' OR Email LIKE '%{0}%'", keyword);
        }

        //Nút Duyệt hàng loạt
        private void BtnBulkAccept_Click(object sender, EventArgs e)
        {
            List<string> selectedIds = new List<string>();

            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colSelect"].Value) == true)
                {
                    selectedIds.Add(row.Cells["Id"].Value.ToString());
                }
            }

            if (selectedIds.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Duyệt {selectedIds.Count} tài khoản đã chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string idList = string.Join(",", selectedIds);
                    string query = $"UPDATE dbo.Users SET Valid = 1 WHERE Id IN ({idList})";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Đã duyệt hàng loạt thành công!");
                        LoadPendingUsers();
                        LoadDashboard();
                    }
                }
            }
        }

        //Nút Xóa/Từ chối hàng loạt
        private void BtnBulkDelete_Click(object sender, EventArgs e)
        {
            List<string> selectedIds = new List<string>();

            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colSelect"].Value) == true)
                {
                    selectedIds.Add(row.Cells["Id"].Value.ToString());
                }
            }

            if (selectedIds.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa vĩnh viễn {selectedIds.Count} yêu cầu đã chọn?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string idList = string.Join(",", selectedIds);
                    string query = $"DELETE FROM dbo.Users WHERE Id IN ({idList})";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Đã xóa hàng loạt thành công!");
                        if (this.Controls.Find("txtSearchPending", true).Length > 0) txtSearchPending.Clear();
                        LoadPendingUsers();
                        LoadDashboard();
                    }
                }
            }
        }

        // Bắt sự kiện Click vào các nút Accept / Delete trên lưới (GIỮ NGUYÊN CODE CŨ)
        private void DgvPendingUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvPendingUsers.Columns[e.ColumnIndex].Name;

            // Bỏ qua nếu click vào cột Checkbox
            if (colName == "colSelect") return;

            int selectedUserId = Convert.ToInt32(dgvPendingUsers.Rows[e.RowIndex].Cells["Id"].Value);
            string selectedUsername = dgvPendingUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();

            if (colName == "colAccept")
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string query = "UPDATE dbo.Users SET Valid = 1 WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedUserId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Đã duyệt tài khoản: {selectedUsername}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPendingUsers();
                        LoadDashboard();
                    }
                }
            }
            else if (colName == "colDelete")
            {
                DialogResult dialog = MessageBox.Show($"Từ chối và xóa tài khoản {selectedUsername}?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    using (SqlConnection conn = db.GetConnection())
                    {
                        string query = "DELETE FROM dbo.Users WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedUserId);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            MessageBox.Show($"Đã xóa tài khoản: {selectedUsername}");
                            LoadPendingUsers();
                            LoadDashboard();
                        }
                    }
                }
            }
        }

        // KHU VỰC 2: THỐNG KÊ & BIỂU ĐỒ
        public void LoadDashboard()
        {
            try
            {
                // 1. Lấy dữ liệu cũ cho Sinh viên/HR (Giữ nguyên để không lỗi code cũ)
                int totalStudents = studentRepo.GetTotalStudentsCount();
                int maleStudents = studentRepo.GetTotalMaleStudentsCount();
                int femaleStudents = studentRepo.GetTotalFemaleStudentsCount();

                // 2. Phân nhánh hiển thị số liệu chuẩn theo quyền (Role)
                if (roleId == 0) // Nếu là ADMIN -> Đổi toàn bộ sang hệ quy chiếu TÀI KHOẢN (Users)
                {
                    int totalUsers = 0;
                    int pendingUsers = 0;
                    int approvedUsers = 0;

                    using (SqlConnection conn = db.GetConnection())
                    {
                        conn.Open();

                        // Đếm TỔNG SỐ TÀI KHOẢN hệ thống
                        string queryTotal = "SELECT COUNT(*) FROM dbo.Users";
                        using (SqlCommand cmd = new SqlCommand(queryTotal, conn))
                        {
                            totalUsers = (int)cmd.ExecuteScalar();
                        }

                        // Đếm số tài khoản CHỜ DUYỆT (Valid = 0)
                        string queryPending = "SELECT COUNT(*) FROM dbo.Users WHERE Valid = 0";
                        using (SqlCommand cmd = new SqlCommand(queryPending, conn))
                        {
                            pendingUsers = (int)cmd.ExecuteScalar();
                        }

                        // Đếm số tài khoản ĐÃ DUYỆT (Valid = 1)
                        string queryApproved = "SELECT COUNT(*) FROM dbo.Users WHERE Valid = 1";
                        using (SqlCommand cmd = new SqlCommand(queryApproved, conn))
                        {
                            approvedUsers = (int)cmd.ExecuteScalar();
                        }
                    }

                    // Hiển thị đồng bộ thông tin về Tài khoản cho Admin
                    lblTotalStudents.Text = $"Tổng tài khoản: {totalUsers}"; // Đổi chữ "Tổng số SV" thành "Tổng tài khoản"
                    lblMaleStudents.Text = $"Chờ duyệt: {pendingUsers}";
                    lblFemaleStudents.Text = $"Đã duyệt: {approvedUsers}";
                }
                else // Nếu là STUDENT hoặc HR -> Giữ nguyên hiển thị về SINH VIÊN (Students)
                {
                    lblTotalStudents.Text = $"Tổng số SV: {totalStudents}";
                    lblMaleStudents.Text = $"Nam: {maleStudents}";
                    lblFemaleStudents.Text = $"Nữ: {femaleStudents}";

                    // Vẽ biểu đồ tròn dựa trên giới tính sinh viên
                    if (picChart != null && picChart.Visible)
                    {
                        LoadDataAndChart(totalStudents, maleStudents, femaleStudents);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng điều khiển: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataAndChart(int total, int male, int female)
        {
            try
            {
                if (total == 0 || picChart == null) return;

                float totalF = (float)total;
                float maleAngle = ((float)male / totalF) * 360f;
                float femaleAngle = ((float)female / totalF) * 360f;

                Bitmap bmp = new Bitmap(picChart.Width, picChart.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(this.BackColor);

                    int diameter = Math.Min(picChart.Width, picChart.Height) - 40;
                    int x = (picChart.Width - diameter) / 2;
                    int y = (picChart.Height - diameter) / 2;
                    Rectangle rect = new Rectangle(x, y, diameter, diameter);

                    float currentStartAngle = -90f;

                    if (male > 0)
                    {
                        Color maleColor = lblMaleStudents.Parent != null ? lblMaleStudents.Parent.BackColor : Color.Cyan;
                        using (SolidBrush maleBrush = new SolidBrush(maleColor))
                        {
                            g.FillPie(maleBrush, rect, currentStartAngle, maleAngle);
                        }
                        currentStartAngle += maleAngle;
                    }

                    if (female > 0)
                    {
                        Color femaleColor = lblFemaleStudents.Parent != null ? lblFemaleStudents.Parent.BackColor : Color.Pink;
                        using (SolidBrush femaleBrush = new SolidBrush(femaleColor))
                        {
                            g.FillPie(femaleBrush, rect, currentStartAngle, femaleAngle);
                        }
                    }

                    int holeDiameter = (int)(diameter * 0.60);
                    int hX = x + (diameter - holeDiameter) / 2;
                    int hY = y + (diameter - holeDiameter) / 2;

                    using (SolidBrush bgBrush = new SolidBrush(this.BackColor))
                    {
                        g.FillEllipse(bgBrush, hX, hY, holeDiameter, holeDiameter);
                    }
                }
                picChart.Image = bmp;
            }
            catch { }
        }

        private void DgvPendingUsers_Paint(object sender, PaintEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv.Rows.Count == 0)
            {
                // Cập nhật text để phù hợp cả khi tìm kiếm không ra kết quả
                string message = "Chưa có tài khoản nào chờ duyệt hoặc không tìm thấy.";
                using (Font font = new Font("Segoe UI", 12, FontStyle.Italic))
                {
                    SizeF size = e.Graphics.MeasureString(message, font);
                    float x = (dgv.Width - size.Width) / 2;
                    float y = (dgv.Height - size.Height) / 2;
                    e.Graphics.DrawString(message, font, Brushes.DimGray, x, y);
                }
            }
        }

        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (roleId == 1)
            {
                MessageBox.Show("Tài khoản Sinh viên không có quyền thực hiện chức năng này!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            this.Hide();
            using (AddStudentForm f = new AddStudentForm(0)) { f.ShowDialog(); }
            this.Show();
            LoadDashboard();
        }

        private void listStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (ListStudentForm f = new ListStudentForm(roleId)) { f.ShowDialog(); }
            this.Show();
            LoadDashboard();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvPendingUsers.Rows)
            {
                row.Cells["colSelect"].Value = chkSelectAll.Checked;
            }
            dgvPendingUsers.EndEdit();
        }
    }
}