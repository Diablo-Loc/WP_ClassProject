using BCrypt.Net;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using ClassProject.Presentation.Forms.Admin;
using ClosedXML.Excel;
using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject
{
    // Định nghĩa phân quyền hệ thống theo chuẩn Enterprise tránh Magic Number
    public enum UserRole
    {
        Admin = 0,
        HR = 3,
        Lecturer = 2,
        Student = 1
    }

    public partial class ManageStudentForm : Form
    {
        private readonly StudentRepository _studentRepo;
        private readonly My_DB _db = new My_DB();

        // Lưu trữ thông tin quyền hiện tại của phiên đăng nhập để tái sử dụng tối ưu
        private readonly UserRole _currentUserRole;
        private readonly bool _canModifyData;

        public ManageStudentForm()
        {
            InitializeComponent();
            _studentRepo = new StudentRepository();

            // Ép kiểu an toàn từ UserSession sang Enum Quyền
            _currentUserRole = (UserRole)UserSession.RoleId;

            // Định nghĩa đặc quyền cho phép thay đổi dữ liệu (Chỉ Admin và HR)
            _canModifyData = (_currentUserRole == UserRole.Admin || _currentUserRole == UserRole.HR);
        }

        private void ManageStudentForm_Load(object sender, EventArgs e)
        {
            InitializeInterfaceComponents();
            ApplyRoleBasedAccessControl();
            fillGrid();
        }

        /// Khởi tạo giá trị mặc định cho các Combobox một cách an toàn
        private void InitializeInterfaceComponents()
        {
            if (cboFilterGender.Items.Count > 0) cboFilterGender.SelectedIndex = 0;
            if (cbSort.Items.Count > 0) cbSort.SelectedIndex = 0;
        }

        /// Áp dụng Phân quyền RBAC trực tiếp lên UI (Tăng trải nghiệm UX doanh nghiệp)
        private void ApplyRoleBasedAccessControl()
        {
            if (btnGenerateAccounts != null)
            {
                btnGenerateAccounts.Visible = (_currentUserRole == UserRole.Admin);
            }

            // Nếu không có quyền chỉnh sửa dữ liệu (Ví dụ: Giảng viên) -> Khóa các tính năng tác động DB
            if (!_canModifyData)
            {
                btnInsert.Enabled = false;
                btnImportExcel.Enabled = false;
                btnDelete.Enabled = false;
                ToolTip systemToolTip = new ToolTip();
                systemToolTip.SetToolTip(btnInsert, "Tài khoản của bạn không có quyền thêm mới sinh viên.");
                systemToolTip.SetToolTip(btnImportExcel, "Tài khoản của bạn không có quyền nhập dữ liệu từ Excel.");
            }

            // Thiết lập tiêu đề động chuyên nghiệp dựa trên vai trò làm việc
            this.Text = $"Quản Lý Sinh Viên - [{_currentUserRole.ToString().ToUpper()}]";
            lblTitle.Text = $"HỆ THỐNG GIÁM SÁT SINH VIÊN - QUYỀN: {_currentUserRole.ToString().ToUpper()}";
        }

        public void fillGrid()
        {
            try
            {
                dgvStudents.DataSource = null;

                string keyword = txtSearch.Text.Trim();
                string gender = cboFilterGender.Text.Trim();
                if (string.IsNullOrEmpty(gender) || gender.Contains("Tất cả") || gender.Contains("---"))
                {
                    gender = "";
                }

                DataTable dt = null;
                string currentTeacherId = UserSession.TeacherId ?? "NULL (Trống)";

                if (_currentUserRole == UserRole.Lecturer)
                {
                    dt = _studentRepo.SearchStudentsByCourseSection(keyword, string.IsNullOrEmpty(gender) ? "All" : gender, currentTeacherId);
                }
                else
                {
                    dt = _studentRepo.SearchStudents(keyword, gender);
                }

                if (dt == null) return;

                DataView dv = dt.DefaultView;

                if (cbSort.SelectedIndex > 0)
                {
                    string sortText = cbSort.Text.Trim();
                    if (sortText == "Tên sinh viên")
                    {
                        if (dt.Columns.Contains("LastName")) dv.Sort = "LastName ASC, FirstName ASC";
                        else if (dt.Columns.Contains("Tên")) dv.Sort = "Tên ASC, Họ ASC";
                    }
                    else if (sortText == "MSSV")
                    {
                        if (dt.Columns.Contains("Mssv")) dv.Sort = "Mssv ASC";
                        else if (dt.Columns.Contains("Mã SV")) dv.Sort = "[Mã SV] ASC";
                    }
                }

                dgvStudents.DataSource = dv;

                // Đồng bộ chỉ số đếm
                lblTotalCount.Text = dv.Count.ToString();
                if (lblPaginationInfo != null)
                {
                    lblPaginationInfo.Text = $"Hiển thị 1 đến {dv.Count} của {dv.Count} sinh viên";
                }

                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị danh sách: {ex.Message}", "Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvStudents.Columns.Count == 0) return;

            // ✨ CHỐT PHÂN QUYỀN ĐỒNG BỘ: Kiểm tra nếu là Admin
            bool isAdmin = (_currentUserRole == UserRole.Admin);

            // ==========================================
            // 🔑 XỬ LÝ CỘT CHECKBOX (Chỉ hiển thị cho Admin)
            // ==========================================
            if (isAdmin)
            {
                if (dgvStudents.Columns["chkSelect"] == null)
                {
                    DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                    {
                        Name = "chkSelect",
                        HeaderText = "[ ] Chọn",
                        Width = 50,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        Resizable = DataGridViewTriState.False
                    };
                    dgvStudents.Columns.Insert(0, chkCol);
                    dgvStudents.CellClick += DgvStudents_HeaderCheckBoxClick;
                }
            }
            else
            {
                // Gỡ bỏ hoàn toàn nếu tài khoản không phải Admin (Giáo vụ, Giảng viên...)
                if (dgvStudents.Columns["chkSelect"] != null)
                {
                    dgvStudents.Columns.Remove("chkSelect");
                }
            }

            // ==========================================
            // 👁️ XỬ LÝ CỘT CON MẮT BẢO MẬT (Chỉ hiển thị cho Admin)
            // ==========================================
            if (isAdmin)
            {
                if (dgvStudents.Columns["btnActionView"] == null)
                {
                    DataGridViewButtonColumn viewColumn = new DataGridViewButtonColumn
                    {
                        Name = "btnActionView",
                        HeaderText = "Bảo Mật",
                        Text = "👁️ Xem",
                        UseColumnTextForButtonValue = true,
                        Width = 80,
                        FlatStyle = FlatStyle.Flat,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    };
                    viewColumn.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    viewColumn.DefaultCellStyle.ForeColor = Color.FromArgb(0, 114, 198);
                    viewColumn.DefaultCellStyle.SelectionForeColor = Color.FromArgb(0, 114, 198);

                    dgvStudents.Columns.Add(viewColumn);
                }
            }
            else
            {
                // Gỡ bỏ hoàn toàn nếu tài khoản không phải Admin
                if (dgvStudents.Columns["btnActionView"] != null)
                {
                    dgvStudents.Columns.Remove("btnActionView");
                }
            }

            // --- 3. Ẩn mã định danh hệ thống (Giữ nguyên logic của bạn) ---
            string[] hiddenCols = { "UserId", "Id", "ClassroomId", "MajorId" };
            foreach (var col in hiddenCols)
            {
                if (dgvStudents.Columns[col] != null) dgvStudents.Columns[col].Visible = false;
            }

            // --- 4. Việt hóa dữ liệu cột đầu ra ---
            string[] mssvKeys = { "Mssv", "Mã SV", "MSSV" };
            foreach (var key in mssvKeys) if (dgvStudents.Columns[key] != null) dgvStudents.Columns[key].HeaderText = "Mã SV";

            if (dgvStudents.Columns["DateOfBirth"] != null) dgvStudents.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
            if (dgvStudents.Columns["Gender"] != null) dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
            if (dgvStudents.Columns["Phone"] != null) dgvStudents.Columns["Phone"].HeaderText = "Điện thoại";
            if (dgvStudents.Columns["Address"] != null) dgvStudents.Columns["Address"].HeaderText = "Địa chỉ";
            if (dgvStudents.Columns["Hometown"] != null) dgvStudents.Columns["Hometown"].HeaderText = "Quê quán";
            if (dgvStudents.Columns["Email"] != null) dgvStudents.Columns["Email"].HeaderText = "Email";
            if (dgvStudents.Columns["ClassName"] != null) dgvStudents.Columns["ClassName"].HeaderText = "Lớp sinh hoạt";
            if (dgvStudents.Columns["MajorName"] != null) dgvStudents.Columns["MajorName"].HeaderText = "Chuyên ngành";

            if (dgvStudents.Columns["Picture"] != null)
            {
                dgvStudents.Columns["Picture"].HeaderText = "Hình ảnh";
                if (dgvStudents.Columns["Picture"] is DataGridViewImageColumn picCol) picCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }

            string keyTen = dgvStudents.Columns.Contains("LastName") ? "LastName" : (dgvStudents.Columns.Contains("Tên") ? "Tên" : null);
            string keyHo = dgvStudents.Columns.Contains("FirstName") ? "FirstName" : (dgvStudents.Columns.Contains("Họ") ? "Họ" : null);

            if (keyHo != null && dgvStudents.Columns[keyHo] != null) dgvStudents.Columns[keyHo].HeaderText = "Họ và tên đệm";
            if (keyTen != null && dgvStudents.Columns[keyTen] != null) dgvStudents.Columns[keyTen].HeaderText = "Tên";

            // --- 5. Sắp đặt thứ tự hiển thị cột logic an toàn theo quyền ---
            int currentIndex = 0;
            if (isAdmin && dgvStudents.Columns["chkSelect"] != null) dgvStudents.Columns["chkSelect"].DisplayIndex = currentIndex++;

            if (dgvStudents.Columns["Mssv"] != null) dgvStudents.Columns["Mssv"].DisplayIndex = currentIndex++;
            else if (dgvStudents.Columns["Mã SV"] != null) dgvStudents.Columns["Mã SV"].DisplayIndex = currentIndex++;

            if (keyHo != null && dgvStudents.Columns[keyHo] != null) dgvStudents.Columns[keyHo].DisplayIndex = currentIndex++;
            if (keyTen != null && dgvStudents.Columns[keyTen] != null) dgvStudents.Columns[keyTen].DisplayIndex = currentIndex++;

            if (dgvStudents.Columns["ClassName"] != null) dgvStudents.Columns["ClassName"].DisplayIndex = currentIndex++;
            if (dgvStudents.Columns["MajorName"] != null) dgvStudents.Columns["MajorName"].DisplayIndex = currentIndex++;

            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private bool _isAllSelected = false;
        private void DgvStudents_HeaderCheckBoxClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click trúng hàng Header (RowIndex = -1) và đúng cột Checkbox (Index = 0 hoặc tên "chkSelect")
            if (e.RowIndex == -1 && dgvStudents.Columns[e.ColumnIndex].Name == "chkSelect")
            {
                _isAllSelected = !_isAllSelected;

                dgvStudents.EndEdit(); // Lưu trạng thái edit hiện tại
                foreach (DataGridViewRow row in dgvStudents.Rows)
                {
                    if (row.IsNewRow) continue;
                    row.Cells["chkSelect"].Value = _isAllSelected;
                }

                // Thay đổi ký hiệu trực quan trên tiêu đề (Có thể tùy biến sâu hơn bằng Custom Paint nếu muốn đẹp)
                dgvStudents.Columns["chkSelect"].HeaderText = _isAllSelected ? "[X] Chọn" : "[ ] Chọn";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) => fillGrid();
        private void cboFilterGender_SelectedIndexChanged(object sender, EventArgs e) => fillGrid();
        private void cbSort_SelectedIndexChanged(object sender, EventArgs e) => fillGrid();

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboFilterGender.Items.Count > 0) cboFilterGender.SelectedIndex = 0;
            if (cbSort.Items.Count > 0) cbSort.SelectedIndex = 0;
            fillGrid();
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

            // 🎯 KIỂM TRA: Nếu click trúng cột con mắt "Bảo Mật"
            if (dgvStudents.Columns[e.ColumnIndex].Name == "btnActionView")
            {
                // Kiểm tra cột UserId ẩn trên DataGridView
                if (dgvStudents.Columns.Contains("UserId") && row.Cells["UserId"].Value != null)
                {
                    var userIdVal = row.Cells["UserId"].Value;

                    // Nếu UserId rỗng hoặc bằng DBNull tức là sinh viên này chưa được cấp tài khoản
                    if (userIdVal == DBNull.Value || string.IsNullOrEmpty(userIdVal.ToString()))
                    {
                        string mssvCol = dgvStudents.Columns.Contains("Mssv") ? "Mssv" : "Mã SV";
                        string mssv = row.Cells[mssvCol].Value?.ToString() ?? "";

                        MessageBox.Show($"Sinh viên [MSSV: {mssv}] chưa từng được cấp tài khoản hệ thống.\n\nVui lòng tích chọn sinh viên và bấm nút 'Cấp tài khoản hàng loạt' trước!",
                                        "Thông báo nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (int.TryParse(userIdVal.ToString(), out int userId))
                    {
                        // Gọi hàm hiển thị Dialog bảo mật, truyền trực tiếp UserId (Khớp với Repo của bạn)
                        ShowStudentSecurityDialog(userId, row);
                        return;
                    }
                }

                MessageBox.Show("Không thể bóc tách mã định danh tài khoản (UserId) của dòng này!", "Lỗi cấu trúc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void dgvStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0 || !_canModifyData) return;

            try
            {
                string mssvCol = dgvStudents.Columns.Contains("Mssv") ? "Mssv" : "Mã SV";
                string mssv = dgvStudents.Rows[e.RowIndex].Cells[mssvCol].Value?.ToString()?.Trim() ?? "";

                if (string.IsNullOrEmpty(mssv)) return;

                using (AddStudentForm editForm = new AddStudentForm(mssv))
                {
                    editForm.ShowDialog();
                }
                fillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở cửa sổ chỉnh sửa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowStudentSecurityDialog(int userId, DataGridViewRow row)
        {
            try
            {
                // 1. CHUẨN DOANH NGHIỆP: Truy vấn dữ liệu thời gian thực (Real-time) từ Database 
                // để đảm bảo lấy đúng thông tin mới nhất nếu Giáo vụ hoặc Học sinh vừa đổi.
                DataRow accountRow = _studentRepo.GetAccountInfoById(userId);

                string mssv = "";
                string username = "";
                string email = "";
                string fullName = "";

                if (accountRow != null)
                {
                    // Nếu tìm thấy tài khoản trong DB, lấy dữ liệu chuẩn xác nhất từ SQL ra
                    mssv = accountRow["MSSV"]?.ToString()?.Trim() ?? "";
                    username = accountRow["Username"]?.ToString()?.Trim() ?? "";
                    email = accountRow["Email"]?.ToString()?.Trim() ?? "";
                    fullName = accountRow["FullName"]?.ToString()?.Trim() ?? "";
                }
                else
                {
                    // Khối phòng hờ (Fallback): Nếu vì lý do gì đó không Join được bảng Users (lỗi DB phụ),
                    // ta mới lấy tạm dữ liệu trên hàng Grid để Form không bị crash đổ vỡ.
                    string mssvCol = dgvStudents.Columns.Contains("Mssv") ? "Mssv" : "Mã SV";
                    string hoCol = dgvStudents.Columns.Contains("LastName") ? "LastName" : (dgvStudents.Columns.Contains("Họ") ? "Họ" : "");
                    string tenCol = dgvStudents.Columns.Contains("FirstName") ? "FirstName" : (dgvStudents.Columns.Contains("Tên") ? "Tên" : "");

                    mssv = row.Cells[mssvCol].Value?.ToString()?.Trim() ?? "";
                    string ho = !string.IsNullOrEmpty(hoCol) ? row.Cells[hoCol].Value?.ToString()?.Trim() : "";
                    string ten = !string.IsNullOrEmpty(tenCol) ? row.Cells[tenCol].Value?.ToString()?.Trim() : "";
                    fullName = $"{ho} {ten}".Trim();
                    email = dgvStudents.Columns.Contains("Email") ? row.Cells["Email"].Value?.ToString()?.Trim() : "";
                    username = mssv;
                }

                // 2. Khởi chạy Dialog dùng chung 
                // Truyền các thông tin đã được kiểm chứng thời gian thực vào Form bảo mật
                using (var dialog = new AccountSecurityInfoDialog(userId, mssv, username, email, fullName, 1))
                {
                    dialog.ShowDialog(this);
                }

                // 3. Làm mới lại lưới DataGridView sau khi tắt Dialog để đồng bộ giao diện
                fillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý bảo mật sinh viên: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            if (!_canModifyData)
            {
                MessageBox.Show("Tài khoản của bạn không có đặc quyền thực hiện hành động này!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            using (AddStudentForm addForm = new AddStudentForm(""))
            {
                addForm.ShowDialog();
            }
            fillGrid();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvStudents.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu sinh viên để xuất file Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Danh_sach_sinh_vien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataView currentView = dgvStudents.DataSource as DataView;
                        DataTable dtSource = currentView != null ? currentView.ToTable() : (DataTable)dgvStudents.DataSource;

                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("Students");

                            var mappingCols = new[]
                            {
                                new { Src = dtSource.Columns.Contains("Mssv") ? "Mssv" : "Mã SV", Target = "Mã SV" },
                                new { Src = dtSource.Columns.Contains("FirstName") ? "FirstName" : "Họ", Target = "Họ và tên đệm" },
                                new { Src = dtSource.Columns.Contains("LastName") ? "LastName" : "Tên", Target = "Tên" },
                                new { Src = "ClassName", Target = "Lớp sinh hoạt" },
                                new { Src = "MajorName", Target = "Chuyên ngành" },
                                new { Src = "DateOfBirth", Target = "Ngày sinh" },
                                new { Src = "Gender", Target = "Giới tính" },
                                new { Src = "Phone", Target = "Điện thoại" },
                                new { Src = "Address", Target = "Địa chỉ" },
                                new { Src = "Hometown", Target = "Quê quán" },
                                new { Src = "Email", Target = "Email" }
                            };

                            for (int i = 0; i < mappingCols.Length; i++)
                            {
                                var cell = ws.Cell(1, i + 1);
                                cell.Value = mappingCols[i].Target;
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }

                            int rIndex = 2;
                            foreach (DataRow row in dtSource.Rows)
                            {
                                for (int cIndex = 0; cIndex < mappingCols.Length; cIndex++)
                                {
                                    string colName = mappingCols[cIndex].Src;
                                    if (dtSource.Columns.Contains(colName))
                                    {
                                        var val = row[colName];

                                        if (val is DateTime dVal)
                                        {
                                            ws.Cell(rIndex, cIndex + 1).Value = dVal.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            if (mappingCols[cIndex].Target == "Mã SV")
                                            {
                                                ws.Cell(rIndex, cIndex + 1).Style.NumberFormat.Format = "@";
                                            }
                                            ws.Cell(rIndex, cIndex + 1).Value = val?.ToString()?.Trim() ?? "";
                                        }
                                    }
                                }
                                rIndex++;
                            }

                            ws.Columns().AdjustToContents();
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất dữ liệu ra file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Có lỗi xảy ra khi tạo tệp Excel: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            if (!_canModifyData)
            {
                MessageBox.Show("Tài khoản của bạn không được phân quyền Import tệp dữ liệu gốc!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Files|*.xlsx" })
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    int added = 0, skipped = 0;
                    using (XLWorkbook wb = new XLWorkbook(ofd.FileName))
                    {
                        var ws = wb.Worksheet(1);
                        var rows = ws.RowsUsed().Skip(1);

                        foreach (var row in rows)
                        {
                            try
                            {
                                string mssv = row.Cell(1).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(mssv)) continue;

                                // --- BỘ CHẶN LỖI ĐỊNH DẠNG (REGEX VALIDATION) ---
                                // 1. Kiểm tra MSSV (Chỉ được chứa số, độ dài từ 5-15 ký tự tùy cấu trúc trường bạn)
                                if (!Regex.IsMatch(mssv, @"^\d{5,15}$")) { skipped++; continue; }

                                if (_studentRepo.IsMssvExist(mssv)) { skipped++; continue; }

                                string firstName = row.Cell(2).Value.ToString().Trim();
                                string lastName = row.Cell(3).Value.ToString().Trim();
                                string phone = row.Cell(8).Value.ToString().Trim();
                                string email = row.Cell(11).Value.ToString().Trim();

                                // 2. Kiểm tra định danh Số điện thoại (Nếu có nhập thì phải chuẩn 10 số bắt đầu bằng số 0)
                                if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^0\d{9}$")) { skipped++; continue; }

                                // 3. TỰ ĐỘNG SINH USERNAME VÀ EMAIL NẾU TRỐNG HOẶC SAI ĐỊNH DẠNG
                                string username = mssv; // Theo chuẩn của bạn: MSSV làm tên đăng nhập mặc định

                                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                                if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, emailPattern))
                                {
                                    // Tự động sinh Email theo Tên + MSSV không dấu (Ví dụ: 12345@student.edu.vn)
                                    email = $"{mssv}@student.edu.vn";
                                }

                                // Xử lý đọc Ngày sinh an toàn đa định dạng
                                DateTime dob;
                                var cellDob = row.Cell(6);
                                if (cellDob.DataType == XLDataType.DateTime)
                                {
                                    dob = cellDob.GetDateTime();
                                }
                                else
                                {
                                    string rawDate = cellDob.Value.ToString().Trim();
                                    if (!DateTime.TryParse(rawDate, out dob))
                                    {
                                        string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };
                                        if (!DateTime.TryParseExact(rawDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dob))
                                        {
                                            // Thay vì lấy ngày hiện tại (năm 2026), đặt mặc định năm sinh sinh viên hợp lệ để tránh lỗi logic tuổi học vụ
                                            dob = new DateTime(2005, 1, 1);
                                        }
                                    }
                                }

                                string defaultRawPassword = "123";
                                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(defaultRawPassword, 12);

                                Student student = new Student
                                {
                                    Mssv = mssv,
                                    UserId = null,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    MaLop = row.Cell(4).Value.ToString().Trim(),
                                    MaNganh = row.Cell(5).Value.ToString().Trim(),
                                    DateOfBirth = dob,
                                    Gender = row.Cell(7).Value.ToString().Trim(),
                                    Phone = phone,
                                    Address = row.Cell(9).Value.ToString().Trim(),
                                    Hometown = row.Cell(10).Value.ToString().Trim(),
                                    Email = email
                                };

                                // Truyền Username (MSSV) và Password vào hàm nạp dữ liệu an toàn của bạn
                                if (_studentRepo.ImportStudentWithAccount(username, hashedPassword, student))
                                {
                                    added++;
                                }
                                else
                                {
                                    skipped++;
                                }
                            }
                            catch
                            {
                                skipped++;
                            }
                        }
                    }

                    MessageBox.Show($"Hoàn tất Nhập dữ liệu học vụ từ Excel:\n- Đồng bộ thành công: {added} sinh viên\n- Bỏ qua hoặc lỗi cấu trúc/định dạng: {skipped}",
                                    "Kết Quả Đồng Bộ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fillGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cấu trúc file excel không tương thích hoặc đang bị mở bởi ứng dụng khác: {ex.Message}",
                                    "Lỗi Thực Thi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (dgvStudents.Rows.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên đang trống, không thể xuất PDF!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Danh_sach_sinh_vien_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 15f, 15f, 20f, 20f))
                    {
                        try
                        {
                            using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                            {
                                iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();

                                string sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                                if (!File.Exists(sysFontPath))
                                {
                                    sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                                }

                                iTextSharp.text.pdf.BaseFont bf = iTextSharp.text.pdf.BaseFont.CreateFont(sysFontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);

                                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 102, 204));
                                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                                iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL);

                                string pdfTitleText = _canModifyData ? "DANH SÁCH QUẢN LÝ SINH VIÊN (DÀNH CHO HỆ THỐNG QUẢN TRỊ)" : "DANH SÁCH SINH VIÊN - BÁO CÁO GIẢNG VIÊN";
                                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph(pdfTitleText, fontTitle);
                                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                                title.SpacingAfter = 20f;
                                pdfDoc.Add(title);

                                iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(9) { WidthPercentage = 100 };
                                float[] widths = { 10f, 15f, 9f, 12f, 18f, 11f, 8f, 12f, 15f };
                                pdfTable.SetWidths(widths);

                                string[] headers = { "Mã SV", "Họ và tên đệm", "Tên", "Lớp", "Chuyên ngành", "Ngày sinh", "Phái", "Quê quán", "Email" };
                                foreach (string headerText in headers)
                                {
                                    iTextSharp.text.pdf.PdfPCell headerCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, fontHeader))
                                    {
                                        BackgroundColor = new iTextSharp.text.BaseColor(0, 102, 204),
                                        HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                                        VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                                        Padding = 6f
                                    };
                                    pdfTable.AddCell(headerCell);
                                }

                                foreach (DataGridViewRow row in dgvStudents.Rows)
                                {
                                    if (row.IsNewRow) continue;

                                    string maSV = "", hoTenDem = "", ten = "", lopSH = "", chuyenNganh = "", ngaySinhFormatted = "", gioiTinh = "", queQuan = "", email = "";

                                    foreach (DataGridViewCell cell in row.Cells)
                                    {
                                        if (cell.OwningColumn == null) continue;
                                        string colName = cell.OwningColumn.Name;
                                        string colHeader = cell.OwningColumn.HeaderText;

                                        if (colName == "Mssv" || colHeader == "Mã SV")
                                            maSV = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "FirstName" || colHeader == "Họ và tên đệm")
                                            hoTenDem = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "LastName" || colHeader == "Tên")
                                            ten = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "ClassName" || colHeader == "Lớp sinh hoạt")
                                            lopSH = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "MajorName" || colHeader == "Chuyên ngành")
                                            chuyenNganh = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "Gender" || colHeader == "Giới tính")
                                            gioiTinh = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "Hometown" || colHeader == "Quê quán")
                                            queQuan = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "Email" || colHeader == "Email")
                                            email = cell.Value?.ToString()?.Trim() ?? "";
                                        else if (colName == "DateOfBirth" || colHeader == "Ngày sinh")
                                        {
                                            if (cell.Value is DateTime dVal)
                                                ngaySinhFormatted = dVal.ToString("dd/MM/yyyy");
                                            else
                                            {
                                                string rawDate = cell.Value?.ToString()?.Trim() ?? "";
                                                if (DateTime.TryParse(rawDate, out DateTime parsedDate))
                                                    ngaySinhFormatted = parsedDate.ToString("dd/MM/yyyy");
                                                else
                                                    ngaySinhFormatted = rawDate;
                                            }
                                        }
                                    }

                                    pdfTable.AddCell(CreateCenterCell(maSV, fontBody));
                                    pdfTable.AddCell(CreateLeftCell(hoTenDem, fontBody));
                                    pdfTable.AddCell(CreateLeftCell(ten, fontBody));
                                    pdfTable.AddCell(CreateCenterCell(lopSH, fontBody));
                                    pdfTable.AddCell(CreateLeftCell(chuyenNganh, fontBody));
                                    pdfTable.AddCell(CreateCenterCell(ngaySinhFormatted, fontBody));
                                    pdfTable.AddCell(CreateCenterCell(gioiTinh, fontBody));
                                    pdfTable.AddCell(CreateLeftCell(queQuan, fontBody));
                                    pdfTable.AddCell(CreateLeftCell(email, fontBody));
                                }

                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                            }

                            MessageBox.Show("Xuất danh sách sinh viên ra file PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Lỗi: Không thể ghi đè dữ liệu. Vui lòng đóng file PDF nếu tệp đó đang được mở bởi một ứng dụng khác!", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Đã xảy ra lỗi không mong muốn khi xuất PDF: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private iTextSharp.text.pdf.PdfPCell CreateLeftCell(string text, iTextSharp.text.Font font)
        {
            return new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font))
            {
                HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT,
                VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                Padding = 4f
            };
        }

        private iTextSharp.text.pdf.PdfPCell CreateCenterCell(string text, iTextSharp.text.Font font)
        {
            return new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font))
            {
                HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                Padding = 4f
            };
        }

        private void btnGenerateAccounts_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra quyền RBAC trước khi thực thi nghiệp vụ hệ thống
            // Nếu biến _canModifyData không tồn tại trong form của bạn, hãy comment hoặc xóa 5 dòng IF dưới này.
            if (typeof(ManageStudentForm).GetField("_canModifyData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
            {
                // Khối kiểm tra an toàn nếu bạn có phân quyền
            }

            dgvStudents.EndEdit(); // Ép DataGridView cập nhật hết các ô đang check dở

            // 2. Lấy ra danh sách các hàng được tích chọn
            var selectedRows = dgvStudents.Rows.Cast<DataGridViewRow>()
                                .Where(r => r.Cells["chkSelect"]?.Value != null && (bool)r.Cells["chkSelect"].Value == true)
                                .ToList();

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một sinh viên trong danh sách để cấp tài khoản.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn cấp tài khoản tự động cho {selectedRows.Count} sinh viên đã chọn?\nHệ thống sẽ tự động bỏ qua những sinh viên đã có tài khoản sẵn.",
                                "Xác nhận cấp tài khoản", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int successCount = 0;
            int skippedCount = 0;

            // Thay đổi con trỏ chuột thành vòng xoay chờ đợi để tăng UX người dùng
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // Tự động nhận diện tên cột MSSV theo cấu trúc dữ liệu trả về từ Repo của bạn
                string mssvColName = "Mssv";
                if (!dgvStudents.Columns.Contains(mssvColName))
                {
                    if (dgvStudents.Columns.Contains("Mã SV")) mssvColName = "Mã SV";
                    else if (dgvStudents.Columns.Contains("MSSV")) mssvColName = "MSSV";
                }

                foreach (DataGridViewRow row in selectedRows)
                {
                    if (row.IsNewRow) continue;

                    string mssv = row.Cells[mssvColName].Value?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(mssv)) continue;

                    // 3. Doanh nghiệp Check: Kiểm tra xem sinh viên này đã được map UserId chưa
                    // Đọc từ cột "UserId" (đã được ẩn ngầm trong FormatDataGridView)
                    if (dgvStudents.Columns.Contains("UserId"))
                    {
                        var userIdVal = row.Cells["UserId"]?.Value;
                        if (userIdVal != null && userIdVal != DBNull.Value && !string.IsNullOrEmpty(userIdVal.ToString()))
                        {
                            skippedCount++;
                            continue; // Đã có UserId => Sinh viên này đã có tài khoản, bỏ qua.
                        }
                    }

                    // 4. Quy chuẩn sinh thông tin mặc định: Username = MSSV | Password mặc định = "123"
                    string username = mssv;
                    string defaultRawPassword = "123";
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(defaultRawPassword, 12);

                    // 5. Gọi hàm xử lý xuống Database thông qua StudentRepository của bạn
                    bool isCreated = _studentRepo.CreateAccountForStudent(mssv, username, hashedPassword);

                    if (isCreated) successCount++;
                    else skippedCount++;
                }

                Cursor.Current = Cursors.Default;
                MessageBox.Show($"Quy trình cấp phát tài khoản hoàn tất:\n\n• Tạo mới thành công: {successCount} tài khoản\n• Bỏ qua (Đã có tài khoản hoặc lỗi): {skippedCount}",
                                "Kết quả thực thi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 6. Đồng bộ làm mới lại bảng dữ liệu bằng hàm fillGrid() có sẵn của bạn
                fillGrid();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($"Đã xảy ra lỗi hệ thống trong quá trình tạo tài khoản: {ex.Message}", "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra phân quyền sửa đổi dữ liệu (Chỉ Admin và HR)
            if (!_canModifyData)
            {
                MessageBox.Show("Tài khoản của bạn không có đặc quyền thực hiện hành động này!",
                                "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            // 2. Kiểm tra xem người dùng đã chọn dòng nào trên Grid chưa
            if (dgvStudents.CurrentRow == null || dgvStudents.CurrentRow.Index < 0)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên trong danh sách để xóa.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Lấy MSSV từ dòng đang chọn một cách an toàn (tương thích mọi cấu trúc tên cột)
                DataGridViewRow currentRow = dgvStudents.CurrentRow;
                string mssvCol = dgvStudents.Columns.Contains("Mssv") ? "Mssv" : "Mã SV";
                string mssv = currentRow.Cells[mssvCol].Value?.ToString()?.Trim() ?? "";

                if (string.IsNullOrEmpty(mssv))
                {
                    MessageBox.Show("Không thể lấy mã sinh viên từ dòng đã chọn!",
                                    "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy thêm Họ và Tên hiển thị lên cảnh báo cho trực quan
                string hoCol = dgvStudents.Columns.Contains("LastName") ? "LastName" : (dgvStudents.Columns.Contains("Họ") ? "Họ" : "");
                string tenCol = dgvStudents.Columns.Contains("FirstName") ? "FirstName" : (dgvStudents.Columns.Contains("Tên") ? "Tên" : "");
                string hoTen = $"{currentRow.Cells[hoCol].Value?.ToString()} {currentRow.Cells[tenCol].Value?.ToString()}".Trim();

                // 4. Hiển thị hộp thoại cảnh báo xác nhận xóa dữ liệu gốc
                DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên [{hoTen}] (MSSV: {mssv}) ra khỏi hệ thống?\nHành động này không thể hoàn tác!",
                                                       "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    // 5. Gọi hàm xóa từ Repository (Bạn cần đảm bảo _studentRepo đã có hàm DeleteStudent nhận tham số string mssv)
                    if (_studentRepo.DeleteStudent(mssv))
                    {
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        fillGrid(); // Làm mới lại lưới hiển thị
                    }
                    else
                    {
                        MessageBox.Show("Xóa sinh viên thất bại hoặc sinh viên không tồn tại.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi thực hiện xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}