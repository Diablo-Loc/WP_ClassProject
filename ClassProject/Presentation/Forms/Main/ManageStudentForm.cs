using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using ClosedXML.Excel;
using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ClassProject
{
    /// <summary>
    /// Định nghĩa phân quyền hệ thống theo chuẩn Enterprise tránh Magic Number
    /// </summary>
    public enum UserRole
    {
        Admin = 0,
        HR = 3,
        Lecturer = 2, // Giảng viên
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

        private void ListStudentForm_Load(object sender, EventArgs e)
        {
            InitializeInterfaceComponents();
            ApplyRoleBasedAccessControl();
            fillGrid();
        }

        /// <summary>
        /// Khởi tạo giá trị mặc định cho các Combobox một cách an toàn
        /// </summary>
        private void InitializeInterfaceComponents()
        {
            if (cboFilterGender.Items.Count > 0) cboFilterGender.SelectedIndex = 0;
            if (cbSort.Items.Count > 0) cbSort.SelectedIndex = 0;
        }

        /// <summary>
        /// Áp dụng Phân quyền RBAC trực tiếp lên UI (Tăng trải nghiệm UX doanh nghiệp)
        /// </summary>
        private void ApplyRoleBasedAccessControl()
        {
            // Nếu không có quyền chỉnh sửa dữ liệu (Ví dụ: Giảng viên) -> Khóa các tính năng tác động DB
            if (!_canModifyData)
            {
                btnInsert.Enabled = false;
                btnImportExcel.Enabled = false;

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
                string gender = cboFilterGender.Text;
                if (gender.Contains("Tất cả")) gender = "Tất cả";

                DataTable dt = _studentRepo.SearchStudents(keyword, gender);
                if (dt == null) return;

                DataView dv = dt.DefaultView;

                // Xử lý sắp xếp động tối ưu hóa hiệu năng bằng DataView
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

                // Đồng bộ chỉ số đếm trực quan trên Dashboard thu nhỏ
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

            // 1. Ẩn mã định danh hệ thống (Tránh làm nhiễu thông tin)
            string[] hiddenCols = { "UserId", "Id", "ClassroomId", "MajorId" };
            foreach (var col in hiddenCols)
            {
                if (dgvStudents.Columns[col] != null) dgvStudents.Columns[col].Visible = false;
            }

            // 2. Việt hóa dữ liệu cột đầu ra
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

            // 3. Chuẩn hóa hiển thị Họ - Tên
            string keyHo = dgvStudents.Columns.Contains("FirstName") ? "FirstName" : (dgvStudents.Columns.Contains("Họ") ? "Họ" : null);
            string keyTen = dgvStudents.Columns.Contains("LastName") ? "LastName" : (dgvStudents.Columns.Contains("Tên") ? "Tên" : null);

            if (keyHo != null && dgvStudents.Columns[keyHo] != null) dgvStudents.Columns[keyHo].HeaderText = "Họ và tên đệm";
            if (keyTen != null && dgvStudents.Columns[keyTen] != null) dgvStudents.Columns[keyTen].HeaderText = "Tên";

            // 4. Sắp đặt thứ tự hiển thị cột logic
            int currentIndex = 0;
            if (dgvStudents.Columns["Mssv"] != null) dgvStudents.Columns["Mssv"].DisplayIndex = currentIndex++;
            else if (dgvStudents.Columns["Mã SV"] != null) dgvStudents.Columns["Mã SV"].DisplayIndex = currentIndex++;

            if (keyHo != null && dgvStudents.Columns[keyHo] != null) dgvStudents.Columns[keyHo].DisplayIndex = currentIndex++;
            if (keyTen != null && dgvStudents.Columns[keyTen] != null) dgvStudents.Columns[keyTen].DisplayIndex = currentIndex++;

            if (dgvStudents.Columns["ClassName"] != null) dgvStudents.Columns["ClassName"].DisplayIndex = currentIndex++;
            if (dgvStudents.Columns["MajorName"] != null) dgvStudents.Columns["MajorName"].DisplayIndex = currentIndex++;

            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private void dgvStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra chỉ số hàng hợp lệ HOẶC chặn nếu không có quyền can thiệp dữ liệu
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

        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            // Lớp bảo vệ bổ sung (Defensive Programming) phòng trường hợp cố tình bypass từ UI
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
            // ĐỘC LẬP QUYỀN: Cho phép cả Giảng viên xuất dữ liệu phục vụ giảng dạy
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

                            // Tạo hàng tiêu đề (Header Row)
                            for (int i = 0; i < mappingCols.Length; i++)
                            {
                                var cell = ws.Cell(1, i + 1);
                                cell.Value = mappingCols[i].Target;
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }

                            // Đổ dữ liệu chi tiết
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
                                                ws.Cell(rIndex, cIndex + 1).Style.NumberFormat.Format = "@"; // Ép định dạng Text tránh mất số 0 đầu
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
            // 1. Chặn tuyệt đối nếu tài khoản không cấu hình quyền Sửa đổi dữ liệu (Ví dụ: Giảng viên)
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
                        var rows = ws.RowsUsed().Skip(1); // Bỏ qua hàng tiêu đề Excel

                        foreach (var row in rows)
                        {
                            try
                            {
                                // Đọc MSSV và kiểm tra hợp lệ ngắt luồng sớm (Short-circuit validation)
                                string mssv = row.Cell(1).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(mssv)) continue;

                                // Nếu MSSV đã tồn tại trong hệ thống, bỏ qua để tránh trùng dữ liệu định danh
                                if (_studentRepo.IsMssvExist(mssv)) { skipped++; continue; }

                                // Đọc dữ liệu Email từ Excel để tạo tài khoản đồng bộ
                                string email = row.Cell(11).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(email)) email = $"{mssv}@student.edu.vn"; // Email dự phòng chuẩn hóa doanh nghiệp

                                // Xử lý đọc Ngày sinh (DateOfBirth) an toàn đa định dạng
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
                                            dob = DateTime.Now; // Trạng thái dự phòng lỗi định dạng nặng
                                        }
                                    }
                                }

                                // CHUẨN DOANH NGHIỆP: Thiết lập thông tin tài khoản đăng nhập mặc định cho Sinh viên
                                string defaultRawPassword = "123"; // Khớp với mật khẩu mặc định file seed test của bạn
                                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(defaultRawPassword, 12);

                                // Khởi tạo thực thể Student đầy đủ dữ liệu nền
                                Student student = new Student
                                {
                                    Mssv = mssv,
                                    UserId = null, // Sẽ được hàm Repository tự động gán sau khi sinh ID ngầm từ bảng Users
                                    FirstName = row.Cell(2).Value.ToString().Trim(),
                                    LastName = row.Cell(3).Value.ToString().Trim(),
                                    MaLop = row.Cell(4).Value.ToString().Trim(),
                                    MaNganh = row.Cell(5).Value.ToString().Trim(),
                                    DateOfBirth = dob,
                                    Gender = row.Cell(7).Value.ToString().Trim(),
                                    Phone = row.Cell(8).Value.ToString().Trim(),
                                    Address = row.Cell(9).Value.ToString().Trim(),
                                    Hometown = row.Cell(10).Value.ToString().Trim(),
                                    Email = email
                                };

                                // Gọi hàm Import tích hợp tạo tài khoản đồng bộ an toàn qua Transaction
                                // Truyền MSSV làm Username đăng nhập mặc định
                                if (_studentRepo.ImportStudentWithAccount(mssv, hashedPassword, student))
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

                    MessageBox.Show($"Hoàn tất Nhập dữ liệu học vụ từ Excel:\n- Đồng bộ thành công: {added} sinh viên\n- Bỏ qua hoặc lỗi cấu trúc: {skipped}",
                                    "Kết Quả Đồng Bộ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fillGrid(); // Làm mới lại bảng DataGridView dữ liệu
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

                                // Điều chỉnh Title động theo phân hệ làm việc chuyên nghiệp
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
    }
}