using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Score
{
    public partial class ManageScoreForm : Form
    {
        private readonly ScoreRepository _scoreRepo;
        private readonly RegisterRepository _registerRepo;
        private readonly StudentRepository _studentRepo;
        private readonly My_DB _db = new My_DB();
        private bool _isBinding = false;

        public ManageScoreForm()
        {
            InitializeComponent();
            // Khởi tạo các tầng Repository
            _scoreRepo = new ScoreRepository();
            _registerRepo = new RegisterRepository();
            _studentRepo = new StudentRepository();

            // Guna2NumericUpDown sử dụng sự kiện ValueChanged
            numQT.ValueChanged += CalculateTotal;
            numCK.ValueChanged += CalculateTotal;

            // Cấu hình giới hạn điểm từ 0 -> 10 
            ConfigureNumericUpDown(numQT);
            ConfigureNumericUpDown(numCK);
        }

        private void ConfigureNumericUpDown(Guna.UI2.WinForms.Guna2NumericUpDown numControl)
        {
            if (numControl != null)
            {
                numControl.Minimum = 0.00m;
                numControl.Maximum = 10.00m;
                numControl.DecimalPlaces = 2;
                numControl.Increment = 0.1m;
            }
        }

        private void ManageScoreForm_Load(object sender, EventArgs e)
        {
            // CHỐT CHẶN BẢO MẬT: Chỉ cho phép tài khoản Admin, Giáo vụ (Staff) HOẶC Giảng viên có quyền thao tác bảng điểm
            if (!UserSession.IsLoggedIn || !(UserSession.IsAdmin || UserSession.IsStaff || UserSession.IsTeacher))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Phân hệ quản lý và nhập điểm số chỉ dành cho Admin, Giáo vụ ban đào tạo hoặc Giảng viên phụ trách.",
                                "Cảnh Báo Bảo Mật", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Đóng form an toàn thông qua hàng đợi thông điệp UI
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }
            PhanQuyenGiaoDien();
            _isBinding = true;
            try
            {
                // 1. Nạp danh sách Lớp học phần trước (Giữ nguyên dòng này của bạn)
                LoadCourseSectionCombo();

                // 2. 🌟 SỬA LỖI TRỐNG COMBOBOX CHO ADMIN: Lấy trực tiếp từ DataTable gốc
                if (cboCourse.DataSource is DataTable dtCourses && dtCourses.Rows.Count > 0)
                {
                    // Đảm bảo ComboBox chọn dòng đầu tiên
                    cboCourse.SelectedIndex = 0;

                    // Lấy chính xác Mã lớp học phần từ dòng đầu tiên của bảng dữ liệu
                    string firstMaLopHP = dtCourses.Rows[0]["MaLopHP"].ToString();

                    // Nạp sinh viên theo mã vừa lấy
                    LoadStudentByClassCombo(firstMaLopHP);
                }
                else
                {
                    LoadStudentByClassCombo("");
                }

                // 3. Giữ nguyên các hàm phía dưới của bạn...
                LoadGridData();
                LoadQuickStats();

                // Đăng ký sự kiện sau khi dữ liệu đã nạp xong để tránh kích hoạt giả
                cboCourse.SelectedIndexChanged += cboCourse_SelectedIndexChanged;
                cboHocKy.SelectedIndexChanged += ThucHienBoLocDGV;
                cboNamHoc.SelectedIndexChanged += ThucHienBoLocDGV;
                cboStudent.SelectedIndexChanged += cboStudent_SelectedIndexChanged;

                _isBinding = false;

                // Tự động cập nhật Học kỳ / Năm học cho lựa chọn đầu tiên
                UpdateSemesterAndYearInfo();
            }
            catch (Exception ex)
            {
                _isBinding = false;
                MessageBox.Show("Lỗi khởi tạo cấu trúc dữ liệu form: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isBinding) return;
            HienThiDiemTheoCapHocVienMonHoc();
        }

        private void ThucHienBoLocDGV(object sender, EventArgs e)
        {
            if (_isBinding) return;

            if (dgvScores.DataSource is DataTable dt)
            {
                List<string> filters = new List<string>();

                // 1. Lọc theo Học Kỳ
                if (cboHocKy.SelectedValue != null)
                {
                    string hkVal = cboHocKy.SelectedValue.ToString();
                    if (hkVal != "-- Tất cả --" && !string.IsNullOrEmpty(hkVal))
                    {
                        filters.Add(string.Format("CONVERT([Học Kỳ], System.String) = '{0}'", hkVal.Replace("'", "''")));
                    }
                }

                // 2. Lọc theo Năm Học
                if (cboNamHoc.SelectedValue != null)
                {
                    string nhVal = cboNamHoc.SelectedValue.ToString();
                    if (nhVal != "-- Tất cả --" && !string.IsNullOrEmpty(nhVal))
                    {
                        filters.Add(string.Format("[Năm Học] = '{0}'", nhVal.Replace("'", "''")));
                    }
                }

                // 3. Lọc theo Từ khóa kiếm tìm (Mã SV hoặc Tên)
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    string safeKeyword = keyword.Replace("'", "''");
                    filters.Add(string.Format("([Mã SV] LIKE '%{0}%' OR [Họ và Tên] LIKE '%{0}%')", safeKeyword));
                }

                // Áp dụng bộ lọc gộp vào DataGridView
                dt.DefaultView.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : string.Empty;
            }
        }

        private void LoadStudentByClassCombo(string maLopHP)
        {
            if (string.IsNullOrEmpty(maLopHP))
            {
                cboStudent.DataSource = null;
                return;
            }

            using (var conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // SỬA: Đổi từ dbo.Register sang dbo.DKMH cho đúng với cấu trúc Database của bạn
                string query = @"
                    SELECT s.* FROM dbo.Students s                    
                    INNER JOIN dbo.DKMH r ON s.MSSV = r.MSSV                    
                    WHERE r.MaLopHP = @MaLopHP";

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                {
                    cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                    using (Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt != null && dt.Rows.Count >= 0)
                        {
                            // Xác định chính xác tên cột MSSV trong bảng Students (Database của bạn đang định nghĩa viết HOA là MSSV)
                            string idColumn = dt.Columns.Contains("MSSV") ? "MSSV" : "Mssv";

                            // Thêm cột hiển thị ghép "Mã - Họ Tên" nếu chưa có
                            if (!dt.Columns.Contains("FullNameWithId"))
                            {
                                dt.Columns.Add("FullNameWithId", typeof(string), idColumn + " + ' - ' + LastName + ' ' + FirstName");
                            }

                            // Tạm khóa trigger sự kiện SelectedIndexChanged để tránh lỗi binding chéo chập dữ liệu
                            bool oldBinding = _isBinding;
                            _isBinding = true;

                            cboStudent.DataSource = dt;
                            cboStudent.DisplayMember = "FullNameWithId";
                            cboStudent.ValueMember = idColumn;

                            // Reset lại trạng thái binding
                            cboStudent.SelectedIndex = -1; // Đưa về mặc định chưa chọn để an toàn

                            _isBinding = oldBinding;
                        }
                    }
                }
            }
        }

        private void LoadCourseSectionCombo()
        {
            using (var conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // 1. Xây dựng câu truy vấn động dựa trên vai trò của User
                string query = @"SELECT cs.MaLopHP, cs.HocKy, cs.NamHoc, 
                                       (cs.MaLopHP + ' - ' + c.TenMH) AS DisplayText 
                                FROM dbo.CourseSection cs
                                JOIN dbo.Course c ON cs.MaMH = c.MaMH";

                // Nếu CHỈ là giảng viên (không phải Admin/Staff) thì lọc theo mã giảng viên của họ
                bool isOnlyTeacher = UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff;

                if (isOnlyTeacher)
                {
                    query += " WHERE cs.MSGV = @MSGV";
                }

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                {
                    if (isOnlyTeacher)
                    {
                        string teacherId = UserSession.TeacherId?.ToString()?.Trim();
                        cmd.Parameters.AddWithValue("@MSGV", string.IsNullOrEmpty(teacherId) ? (object)DBNull.Value : teacherId);
                    }

                    using (Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // 2. Nạp dữ liệu vào ComboBox Lớp học phần
                        cboCourse.DataSource = dt;
                        cboCourse.DisplayMember = "DisplayText";
                        cboCourse.ValueMember = "MaLopHP";

                        // 3. Nạp dữ liệu lọc cho ComboBox Học Kỳ
                        DataView viewHky = new DataView(dt);
                        DataTable dtHkyRaw = viewHky.ToTable(true, "HocKy");
                        DataTable dtHky = new DataTable();
                        dtHky.Columns.Add("HocKy", typeof(string));

                        DataRow rowHkAll = dtHky.NewRow();
                        rowHkAll["HocKy"] = "-- Tất cả --";
                        dtHky.Rows.Add(rowHkAll);

                        foreach (DataRow r in dtHkyRaw.Rows)
                        {
                            if (r["HocKy"] != DBNull.Value)
                            {
                                DataRow newRow = dtHky.NewRow();
                                newRow["HocKy"] = r["HocKy"].ToString();
                                dtHky.Rows.Add(newRow);
                            }
                        }
                        cboHocKy.DataSource = dtHky;
                        cboHocKy.DisplayMember = "HocKy";
                        cboHocKy.ValueMember = "HocKy";

                        // 4. Nạp dữ liệu lọc cho ComboBox Năm Học
                        DataView viewNamHoc = new DataView(dt);
                        DataTable dtNamHocRaw = viewNamHoc.ToTable(true, "NamHoc");
                        DataTable dtNamHoc = new DataTable();
                        dtNamHoc.Columns.Add("NamHoc", typeof(string));

                        DataRow rowNamAll = dtNamHoc.NewRow();
                        rowNamAll["NamHoc"] = "-- Tất cả --";
                        dtNamHoc.Rows.Add(rowNamAll);

                        foreach (DataRow r in dtNamHocRaw.Rows)
                        {
                            if (r["NamHoc"] != DBNull.Value)
                            {
                                DataRow newRow = dtNamHoc.NewRow();
                                newRow["NamHoc"] = r["NamHoc"].ToString();
                                dtNamHoc.Rows.Add(newRow);
                            }
                        }
                        cboNamHoc.DataSource = dtNamHoc;
                        cboNamHoc.DisplayMember = "NamHoc";
                        cboNamHoc.ValueMember = "NamHoc";
                    }
                }
            }
        }

        private void LoadGridData()
        {
            DataTable dtScores = _scoreRepo.GetScoreList();

            // Nếu là giảng viên và không phải Admin/Staff, tiến hành lọc dữ liệu trên Grid 
            if (UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff)
            {
                // Thu thập danh sách các Mã Lớp HP mà giảng viên này được phép dạy từ cboCourse
                List<string> validClassIds = new List<string>();
                if (cboCourse.DataSource is DataTable dtCourses)
                {
                    foreach (DataRow row in dtCourses.Rows)
                    {
                        validClassIds.Add($"'{row["MaLopHP"].ToString().Replace("'", "''")}'");
                    }
                }

                // Nếu giảng viên có lớp dạy, lọc bảng điểm chỉ hiển thị các lớp đó
                if (validClassIds.Count > 0)
                {
                    string filterExpression = $"[Mã Lớp HP] IN ({string.Join(",", validClassIds)})";

                    // Tạo bản sao DataTable đã lọc để gán vào Grid công khai
                    DataView dv = new DataView(dtScores);
                    dv.RowFilter = filterExpression;
                    dgvScores.DataSource = dv.ToTable();
                }
                else
                {
                    // Nếu không dạy lớp nào, cho bảng trống
                    dgvScores.DataSource = dtScores.Clone();
                }
            }
            else
            {
                // Admin hoặc Giáo vụ: xem toàn bộ dữ liệu
                dgvScores.DataSource = dtScores;
            }

            // Cấu hình UI Grid giữ nguyên
            if (dgvScores.Columns.Count > 0)
            {
                dgvScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvScores.AllowUserToAddRows = false;
                dgvScores.ReadOnly = true;
                dgvScores.ColumnHeadersVisible = true;
                if (dgvScores.ColumnHeadersHeight < 30)
                {
                    dgvScores.ColumnHeadersHeight = 40;
                }
            }
        }

        private void CalculateTotal(object sender, EventArgs e)
        {
            decimal qt = numQT.Value;
            decimal ck = numCK.Value;

            if (qt >= 0 && qt <= 10 && ck >= 0 && ck <= 10)
            {
                decimal tk = Math.Round(qt * 0.4m + ck * 0.6m, 2);
                txtTK.Text = tk.ToString("0.00");

                decimal he4 = ConvertToHe4(tk);
                txtDiemHe4.Text = he4.ToString("0.0");
                txtXepLoai.Text = GetXepLoai(tk);
            }
            else
            {
                txtTK.Text = "0.00";
                txtDiemHe4.Text = "0.0";
                txtXepLoai.Text = "Chưa xếp loại";
            }
        }

        private decimal ConvertToHe4(decimal gpa10)
        {
            if (gpa10 >= 8.5m) return 4.0m;
            if (gpa10 >= 8.0m) return 3.5m;
            if (gpa10 >= 7.0m) return 3.0m;
            if (gpa10 >= 6.5m) return 2.5m;
            if (gpa10 >= 5.5m) return 2.0m;
            if (gpa10 >= 5.0m) return 1.5m;
            if (gpa10 >= 4.0m) return 1.0m;
            return 0.0m;
        }

        private string GetXepLoai(decimal gpa10)
        {
            if (gpa10 >= 8.5m) return "Giỏi";
            if (gpa10 >= 7.0m) return "Khá";
            if (gpa10 >= 5.5m) return "Trung bình";
            if (gpa10 >= 4.0m) return "Yếu";
            return "Kém";
        }

        private void btnSaveScore_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null) return;

            string mssv = cboStudent.SelectedValue.ToString();
            string maLopHP = cboCourse.SelectedValue.ToString();

            // KIỂM TRA ĐĂNG KÝ: Sinh viên phải nằm trong danh sách đăng ký của lớp học phần này mới được vào điểm
            if (!_registerRepo.IsRegistered(mssv, maLopHP))
            {
                MessageBox.Show("Sinh viên này hiện không đăng ký học lớp học phần này! Thao tác nhập điểm bị từ chối.", "Sai dữ liệu học vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal qt = numQT.Value;
            decimal ck = numCK.Value;
            decimal tk = Math.Round(qt * 0.4m + ck * 0.6m, 2);
            string ghiChu = txtGhiChu.Text;

            try
            {
                if (_scoreRepo.SaveScore(mssv, maLopHP, qt, ck, tk, ghiChu))
                {
                    MessageBox.Show("Đồng bộ dữ liệu bảng điểm sinh viên lên hệ thống thành công!", "Thông báo học vụ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGridData();
                    LoadQuickStats();
                    btnClear_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Lưu điểm thất bại, vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kết nối cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvScores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvScores.Rows[e.RowIndex];

                _isBinding = true; // Chặn các sự kiện SelectedIndexChanged kích hoạt logic tìm kiếm thừa

                string idColumn = dgvScores.Columns.Contains("Mã SV") ? "Mã SV" : "MSSV";

                // 1. Đồng bộ theo cột "Mã Lớp HP" trước để nạp tập sinh viên phù hợp vào ComboBox
                if (row.Cells["Mã Lớp HP"].Value != null)
                {
                    string currentClass = row.Cells["Mã Lớp HP"].Value.ToString();
                    cboCourse.SelectedValue = currentClass;

                    // 🌟 BỔ SUNG: Nạp lại danh sách sinh viên của lớp vừa được click
                    LoadStudentByClassCombo(currentClass);
                    UpdateSemesterAndYearInfo();
                }

                // 2. Đồng bộ Sinh viên
                if (row.Cells[idColumn].Value != null)
                    cboStudent.SelectedValue = row.Cells[idColumn].Value.ToString();

                // 3. Đồng bộ Điểm Quá trình
                if (decimal.TryParse(row.Cells["Điểm QT (40%)"].Value?.ToString(), out decimal qt))
                    numQT.Value = qt;
                else
                    numQT.Value = 0;

                // 4. Đồng bộ Điểm Cuối kỳ
                if (decimal.TryParse(row.Cells["Điểm CK (60%)"].Value?.ToString(), out decimal ck))
                    numCK.Value = ck;
                else
                    numCK.Value = 0;

                // 5. Đồng bộ Ghi chú
                txtGhiChu.Text = row.Cells["Ghi Chú"].Value?.ToString() ?? "";

                _isBinding = false;
            }
        }

        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isBinding) return;

            // 🌟 ĐÃ ĐỒNG BỘ: Mỗi khi chuyển lớp, cboStudent lập tức cập nhật danh sách sinh viên tương ứng
            if (cboCourse.SelectedValue != null)
            {
                LoadStudentByClassCombo(cboCourse.SelectedValue.ToString());
            }

            UpdateSemesterAndYearInfo();
            HienThiDiemTheoCapHocVienMonHoc();
        }

        private void UpdateSemesterAndYearInfo()
        {
            if (cboCourse.SelectedValue != null && cboCourse.DataSource is DataTable dt)
            {
                string selectedMaLopHP = cboCourse.SelectedValue.ToString();
                DataRow[] rows = dt.Select($"MaLopHP = '{selectedMaLopHP}'");
                if (rows.Length > 0)
                {
                    // Tạm khóa trigger lọc DataGridView khi đang gán text thủ công
                    bool oldBinding = _isBinding;
                    _isBinding = true;

                    cboHocKy.Text = rows[0]["HocKy"]?.ToString() ?? "";
                    cboNamHoc.Text = rows[0]["NamHoc"]?.ToString() ?? "";

                    _isBinding = oldBinding;
                }
            }
        }

        private void btnDeleteScore_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn bản ghi điểm cần xóa trên bảng hiển thị!", "Yêu cầu thao tác", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mssv = cboStudent.SelectedValue.ToString();
            string maLopHP = cboCourse.SelectedValue.ToString();

            DialogResult result = MessageBox.Show($"Xác nhận loại bỏ điểm số của lớp học phần [{maLopHP}] thuộc sinh viên [{mssv}] ra khỏi hệ thống?", "Xác nhận xóa biên bản điểm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_scoreRepo.DeleteScore(mssv, maLopHP))
                    {
                        MessageBox.Show("Xóa bản ghi điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGridData();
                        LoadQuickStats();
                        btnClear_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu điểm phù hợp hoặc xóa thất bại!", "Lỗi thực thi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _isBinding = true;

            numQT.Value = 0;
            numCK.Value = 0;
            txtTK.Text = "0.00";
            txtDiemHe4.Text = "0.0";
            txtXepLoai.Text = "Chưa xếp loại";
            txtGhiChu.Clear();
            txtSearch.Clear();

            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;

            // 🌟 ĐÃ ĐỒNG BỘ: Sau khi reset môn học về index 0, phải nạp lại danh sách sinh viên của môn đó
            if (cboCourse.SelectedValue != null)
            {
                LoadStudentByClassCombo(cboCourse.SelectedValue.ToString());
            }
            if (cboStudent.Items.Count > 0) cboStudent.SelectedIndex = 0;

            UpdateSemesterAndYearInfo();

            _isBinding = false;

            if (dgvScores.DataSource is DataTable dt)
                dt.DefaultView.RowFilter = string.Empty;

            dgvScores.ClearSelection();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Báo cáo/In ấn đang chuẩn bị kết nối dữ liệu...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvScores.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu điểm nào để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Bang_Diem_Sinh_Vien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataTable dtSource = null;
                        if (dgvScores.DataSource is DataTable dt)
                        {
                            dtSource = dt.DefaultView.ToTable();
                        }

                        if (dtSource == null) return;

                        using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("BangDiem");

                            List<string> headers = new List<string>();
                            foreach (DataGridViewColumn col in dgvScores.Columns)
                            {
                                if (col.Visible) headers.Add(col.HeaderText);
                            }

                            // Đổ Header
                            for (int i = 0; i < headers.Count; i++)
                            {
                                var cell = ws.Cell(1, i + 1);
                                cell.Value = headers[i];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#0D6EFD");
                                cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                                cell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            }

                            // Đổ Data rows
                            int rIndex = 2;
                            foreach (DataRow row in dtSource.Rows)
                            {
                                int cIndex = 1;
                                foreach (DataGridViewColumn col in dgvScores.Columns)
                                {
                                    if (!col.Visible) continue;

                                    var val = row[col.DataPropertyName ?? col.Name];
                                    var currentCell = ws.Cell(rIndex, cIndex);

                                    if (val != null && decimal.TryParse(val.ToString(), out decimal numVal) &&
                                        (col.HeaderText.Contains("Điểm") || col.HeaderText.Contains("Hệ 4")))
                                    {
                                        currentCell.Value = numVal;
                                        currentCell.Style.NumberFormat.Format = "0.00";
                                        currentCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                                    }
                                    else
                                    {
                                        currentCell.Value = val?.ToString() ?? "";
                                        currentCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                                    }

                                    currentCell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                                    cIndex++;
                                }
                                rIndex++;
                            }

                            ws.Columns().AdjustToContents();
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất danh sách bảng điểm sinh viên ra file Excel thành công!", "Thông báo thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi tạo tệp Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ThucHienBoLocDGV(sender, e);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnSaveScore_Click(sender, e);
        }

        private void LoadQuickStats()
        {
            try
            {
                DataTable dt = _scoreRepo.GetQuickStats();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblTotalStudents.Text = string.Format("{0:#,##0}", row["TotalStudents"]);
                    lblTotalCourses.Text = string.Format("{0:#,##0}", row["TotalCourses"]);
                    lblTotalScores.Text = string.Format("{0:#,##0}", row["TotalScoresEntered"]);

                    decimal avgScore = Convert.ToDecimal(row["AverageSchoolScore"]);
                    lblAvgScore.Text = avgScore.ToString("0.00");
                }
            }
            catch
            {
                lblTotalStudents.Text = "0";
                lblTotalCourses.Text = "0";
                lblTotalScores.Text = "0";
                lblAvgScore.Text = "0.00";
            }
        }

        private void HienThiDiemTheoCapHocVienMonHoc()
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null) return;

            string selectedMssv = cboStudent.SelectedValue.ToString();
            string selectedMaLopHP = cboCourse.SelectedValue.ToString();

            if (dgvScores.DataSource is DataTable dt)
            {
                string idFilterColumn = dt.Columns.Contains("Mã SV") ? "Mã SV" : "MSSV";
                DataRow[] rows = dt.Select($"[{idFilterColumn}] = '{selectedMssv}' AND [Mã Lớp HP] = '{selectedMaLopHP}'");

                if (rows.Length > 0)
                {
                    DataRow row = rows[0];

                    if (decimal.TryParse(row["Điểm QT (40%)"]?.ToString(), out decimal qt))
                        numQT.Value = qt;
                    else
                        numQT.Value = 0;

                    if (decimal.TryParse(row["Điểm CK (60%)"]?.ToString(), out decimal ck))
                        numCK.Value = ck;
                    else
                        numCK.Value = 0;

                    txtGhiChu.Text = row["Ghi Chú"]?.ToString() ?? "";

                    // Focus dòng trên grid view một cách an toàn
                    foreach (DataGridViewRow dgvRow in dgvScores.Rows)
                    {
                        if (dgvRow.Cells[idFilterColumn].Value?.ToString() == selectedMssv &&
                            dgvRow.Cells["Mã Lớp HP"].Value?.ToString() == selectedMaLopHP)
                        {
                            dgvRow.Selected = true;
                            dgvScores.FirstDisplayedScrollingRowIndex = dgvRow.Index;
                            break;
                        }
                    }
                }
                else
                {
                    numQT.Value = 0;
                    numCK.Value = 0;
                    txtGhiChu.Clear();
                    txtTK.Text = "0.00";
                    txtDiemHe4.Text = "0.0";
                    txtXepLoai.Text = "Chưa xếp loại";
                }
            }
        }
        private void PhanQuyenGiaoDien()
        {
            // Kiểm tra nếu USER là Admin hoặc Giáo vụ ban đào tạo (Staff)
            // Hoặc kiểm tra: nếu KHÔNG PHẢI Giảng viên thì khóa tính năng nhập liệu
            if (UserSession.IsAdmin || UserSession.IsStaff)
            {
                // 1. Làm mờ / Vô hiệu hóa các control nhập liệu (Chỉ cho xem)
                numQT.Enabled = false;
                numCK.Enabled = false;
                txtGhiChu.ReadOnly = true;
                cboStudent.Enabled = false; // Khóa luôn chọn sinh viên lẻ nếu muốn họ chỉ xem trên Grid

                // 2. Ẩn hoặc làm mờ các nút chức năng can thiệp dữ liệu
                btnSaveScore.Enabled = false;  // Nút Lưu/Thêm
                btnUpdate.Enabled = false;     // Nút Cập nhật
                btnDeleteScore.Enabled = false; // Nút Xóa
                btnClear.Enabled = false;      // Nút Nhập lại

                // 3. Giữ lại các nút nghiệp vụ của Admin/Giáo vụ
                btnExport.Enabled = true;      // Xuất Excel (Báo cáo)
                txtSearch.Enabled = true;      // Ô tìm kiếm nhanh
                cboCourse.Enabled = true;      // Combo lọc lớp học phần
                cboHocKy.Enabled = true;
                cboNamHoc.Enabled = true;
            }
            else
            {
                // Nếu là Giảng viên: Mở khóa toàn bộ để họ làm việc
                numQT.Enabled = true;
                numCK.Enabled = true;
                txtGhiChu.ReadOnly = false;
                cboStudent.Enabled = true;

                btnSaveScore.Enabled = true;
                btnUpdate.Enabled = true;
                btnDeleteScore.Enabled = true;
                btnClear.Enabled = true;
            }
        }
    }
}