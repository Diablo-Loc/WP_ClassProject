using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Course
{
    public partial class ManageScoreForm : Form
    {
        private ScoreRepository _scoreRepo;
        private RegisterRepository _registerRepo;
        private StudentRepository _studentRepo;
        private My_DB _db = new My_DB();
        private bool _isBinding = false;

        public ManageScoreForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;

            // Khởi tạo các tầng Repository tương ứng
            _scoreRepo = new ScoreRepository(connString);
            _registerRepo = new RegisterRepository(connString);
            _studentRepo = new StudentRepository(connString);

            // Guna2NumericUpDown sử dụng sự kiện ValueChanged thay cho TextChanged
            numQT.ValueChanged += CalculateTotal;
            numCK.ValueChanged += CalculateTotal;

            // Cấu hình giới hạn điểm từ 0 -> 10 và lấy 2 chữ số thập phân cho Guna Numeric
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
            _isBinding = true;
            LoadStudentCombo();
            LoadCourseCombo();
            LoadGridData();
            LoadQuickStats();

            // Đăng ký sự kiện SelectedIndexChanged cho cboCourse sau khi dữ liệu đã nạp xong
            cboCourse.SelectedIndexChanged += cboCourse_SelectedIndexChanged;
            cboHocKy.SelectedIndexChanged += ThucHienBoLocDGV;
            cboNamHoc.SelectedIndexChanged += ThucHienBoLocDGV;

            // Tự động cập nhật Học kỳ / Năm học cho môn học đầu tiên
            UpdateSemesterAndYearInfo();
        }

        private void cboStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            HienThiDiemTheoCapHocVienMonHoc();
        }

        // Hàm xử lý lọc dữ liệu kết hợp đồng thời cả Học kỳ, Năm học và Ô tìm kiếm văn bản
        private void ThucHienBoLocDGV(object sender, EventArgs e)
        {
            if (_isBinding) return;

            if (dgvScores.DataSource is DataTable dt)
            {
                List<string> filters = new List<string>();

                // 1. Lọc theo Học Kỳ (Bỏ qua nếu chọn giá trị trống/Tất cả)
                if (cboHocKy.SelectedValue != null &&
                    cboHocKy.SelectedValue.ToString() != "-- Tất cả --" &&
                    !string.IsNullOrEmpty(cboHocKy.SelectedValue.ToString()))
                {
                    filters.Add(string.Format("CONVERT([Học Kỳ], System.String) = '{0}'", cboHocKy.SelectedValue.ToString()));
                }
                // 2. Lọc theo Năm Học (Bỏ qua nếu chọn "-- Tất cả --")
                if (cboNamHoc.SelectedValue != null && cboNamHoc.SelectedValue.ToString() != "-- Tất cả --")
                {
                    filters.Add(string.Format("[Năm Học] = '{0}'", cboNamHoc.SelectedValue.ToString().Replace("'", "''")));
                }

                // 3. Lọc theo Từ khóa tìm kiếm trong ô Textbox (Nếu có gõ chữ)
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    string safeKeyword = keyword.Replace("'", "''");
                    filters.Add(string.Format("([Mã SV] LIKE '%{0}%' OR [Họ và Tên] LIKE '%{0}%')", safeKeyword));
                }

                // Tiến hành áp dụng bộ lọc gộp vào DataGridView thông qua DefaultView.RowFilter
                if (filters.Count > 0)
                {
                    dt.DefaultView.RowFilter = string.Join(" AND ", filters);
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Empty; // Hiển thị toàn bộ bảng nếu không chọn bộ lọc nào
                }
            }
        }

        private void LoadStudentCombo()
        {
            try
            {
                DataTable dt = _studentRepo.SearchStudents("", "Tất cả");
                if (dt.Columns.Contains("MSSV") && !dt.Columns.Contains("FullNameWithId"))
                {
                    dt.Columns.Add("FullNameWithId", typeof(string), "MSSV + ' - ' + LastName + ' ' + FirstName");
                }
                cboStudent.DataSource = dt;
                cboStudent.DisplayMember = "FullNameWithId";
                cboStudent.ValueMember = "MSSV";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách SV: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadCourseCombo()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // Lấy toàn bộ danh sách môn học để xử lý
                    string query = "SELECT MaMH, TenMH, Hky, NamHoc, (MaMH + ' - ' + TenMH) AS DisplayText FROM dbo.Course";
                    using (Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // 1. Nạp dữ liệu vào ComboBox Môn học (Phần nhập điểm)
                        cboCourse.DataSource = dt;
                        cboCourse.DisplayMember = "DisplayText";
                        cboCourse.ValueMember = "MaMH";

                        // 2. TỰ ĐỘNG LỌC VÀ NẠP DỮ LIỆU CHO COMBOBOX HỌC KỲ (ĐÃ SỬA CHỮ "-- Tất cả --")
                        DataView viewHky = new DataView(dt);
                        DataTable dtHkyRaw = viewHky.ToTable(true, "Hky"); // Lọc trùng các học kỳ gốc

                        // Tạo một bảng mới clone cấu trúc nhưng ép cột Học Kỳ sang kiểu String để chứa được chữ tiếng Việt
                        DataTable dtHky = new DataTable();
                        dtHky.Columns.Add("Hky", typeof(string));

                        // Chèn dòng chữ "-- Tất cả --" lên đầu bảng
                        DataRow rowHkAll = dtHky.NewRow();
                        rowHkAll["Hky"] = "-- Tất cả --";
                        dtHky.Rows.Add(rowHkAll);

                        // Đổ các học kỳ số từ database vào bảng chuỗi này
                        foreach (DataRow r in dtHkyRaw.Rows)
                        {
                            if (r["Hky"] != DBNull.Value)
                            {
                                DataRow newRow = dtHky.NewRow();
                                newRow["Hky"] = r["Hky"].ToString();
                                dtHky.Rows.Add(newRow);
                            }
                        }

                        cboHocKy.DataSource = dtHky;
                        cboHocKy.DisplayMember = "Hky";
                        cboHocKy.ValueMember = "Hky";
                        if (cboHocKy.Items.Count > 0) cboHocKy.SelectedIndex = 0;


                        // 3. TỰ ĐỘNG LỌC VÀ NẠP DỮ LIỆU CHO COMBOBOX NĂM HỌC
                        DataView viewNamHoc = new DataView(dt);
                        DataTable dtNamHoc = viewNamHoc.ToTable(true, "NamHoc");

                        DataRow rowNamAll = dtNamHoc.NewRow();
                        rowNamAll["NamHoc"] = "-- Tất cả --";
                        dtNamHoc.Rows.InsertAt(rowNamAll, 0);

                        cboNamHoc.DataSource = dtNamHoc;
                        cboNamHoc.DisplayMember = "NamHoc";
                        cboNamHoc.ValueMember = "NamHoc";
                        if (cboNamHoc.Items.Count > 0) cboNamHoc.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấu hình môn học, học kỳ, năm học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGridData()
        {
            dgvScores.DataSource = _scoreRepo.GetScoreList();
            if (dgvScores.Columns.Count > 0)
            {
                dgvScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvScores.AllowUserToAddRows = false;
                dgvScores.ReadOnly = true;
                dgvScores.ColumnHeadersVisible = true;
                if (dgvScores.ColumnHeadersHeight < 30)
                {
                    dgvScores.ColumnHeadersHeight = 40; // Đảm bảo header đủ cao để hiển thị chữ
                }
            }
        }

        // Tự động tính điểm tổng kết Real-time dựa trên thuộc tính Value của Guna Numeric
        private void CalculateTotal(object sender, EventArgs e)
        {
            decimal qt = numQT.Value;
            decimal ck = numCK.Value;

            if (qt >= 0 && qt <= 10 && ck >= 0 && ck <= 10)
            {
                // Tính điểm hệ 10
                decimal tk = Math.Round(qt * 0.4m + ck * 0.6m, 2);
                txtTK.Text = tk.ToString("0.00");

                // Tính điểm hệ 4 và Xếp loại
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
            if (gpa10 >= 8.5m) return 4.0m; // A
            if (gpa10 >= 8.0m) return 3.5m; // B+
            if (gpa10 >= 7.0m) return 3.0m; // B
            if (gpa10 >= 6.5m) return 2.5m; // C+
            if (gpa10 >= 5.5m) return 2.0m; // C
            if (gpa10 >= 5.0m) return 1.5m; // D+
            if (gpa10 >= 4.0m) return 1.0m; // D
            return 0.0m;                    // F
        }

        private string GetXepLoai(decimal gpa10)
        {
            if (gpa10 >= 8.5m) return "Giỏi";
            if (gpa10 >= 7.0m) return "Khá";
            if (gpa10 >= 5.5m) return "Trung bình";
            if (gpa10 >= 4.0m) return "Yếu";
            return "Kém";
        }

        // Sự kiện click nút Lưu Điểm
        private void btnSaveScore_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null) return;

            string mssv = cboStudent.SelectedValue.ToString();
            string maMH = cboCourse.SelectedValue.ToString();

            // KIỂM TRA: Sinh viên phải đăng ký môn học này trước thì mới cho nhập điểm
            if (!_registerRepo.IsRegistered(mssv, maMH))
            {
                MessageBox.Show("Sinh viên này chưa đăng ký học môn này! Không thể nhập điểm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal qt = numQT.Value;
            decimal ck = numCK.Value;

            // Vì dùng GunaNumericUpDown đã giới hạn Max/Min trực tiếp trên UI nên không sợ bị tràn lướt quá 10 điểm nữa
            decimal tk = Math.Round(qt * 0.4m + ck * 0.6m, 2);
            string ghiChu = txtGhiChu.Text;

            if (_scoreRepo.SaveScore(mssv, maMH, qt, ck, tk, ghiChu))
            {
                MessageBox.Show("Lưu bảng điểm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGridData(); // Refresh lại DataGridView điểm số
                LoadQuickStats();
                //btnClear_Click(sender, e); // Tự động làm sạch form sau khi lưu thành công
            }
            else
            {
                MessageBox.Show("Lưu điểm thất bại, vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvScores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvScores.Rows[e.RowIndex];

                // 1. Đồng bộ Sinh viên
                if (row.Cells["Mã SV"].Value != null)
                    cboStudent.SelectedValue = row.Cells["Mã SV"].Value.ToString();

                // 2. Đồng bộ Môn học 
                if (row.Cells["Mã MH"].Value != null)
                {
                    cboCourse.SelectedValue = row.Cells["Mã MH"].Value.ToString();

                    // ĐÃ ĐỔI: Sau khi gán giá trị cho cboCourse, ta chủ động gọi hàm nạp thông tin Học kỳ, Năm học
                    // để tránh việc đọc trực tiếp cột không tồn tại từ DataGridView gây crash phần mềm.
                    UpdateSemesterAndYearInfo();
                }

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
            }
        }

        // XỬ LÝ SỰ KIỆN KHI THAY ĐỔI MÔN HỌC TRÊN COMBOBOX
        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSemesterAndYearInfo();
            HienThiDiemTheoCapHocVienMonHoc();
        }

        // Hàm hỗ trợ tự động tìm và cập nhật thông tin Học kỳ / Năm học theo môn học được lựa chọn
        private void UpdateSemesterAndYearInfo()
        {
            if (cboCourse.SelectedValue != null && cboCourse.DataSource is DataTable dt)
            {
                string selectedMaMH = cboCourse.SelectedValue.ToString();
                DataRow[] rows = dt.Select($"MaMH = '{selectedMaMH}'");
                if (rows.Length > 0)
                {
                    _isBinding = true;
                    cboHocKy.Text = rows[0]["Hky"]?.ToString() ?? "";
                    cboNamHoc.Text = rows[0]["NamHoc"]?.ToString() ?? "";
                    _isBinding = false;
                }
            }
        }

        // XỬ LÝ NÚT XÓA ĐIỂM
        private void btnDeleteScore_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn dòng điểm cần xóa dưới bảng lưới hoặc chọn Sinh viên và Môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mssv = cboStudent.SelectedValue.ToString();
            string maMH = cboCourse.SelectedValue.ToString();

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa điểm môn học {maMH} của sinh viên {mssv} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (_scoreRepo.DeleteScore(mssv, maMH))
                {
                    MessageBox.Show("Xóa điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnClear_Click(sender, e);
                    LoadQuickStats();
                    LoadGridData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu điểm phù hợp hoặc xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // XỬ LÝ NÚT LÀM MỚI (RESET) - ĐÃ TỐI ƯU
        private void btnClear_Click(object sender, EventArgs e)
        {
            numQT.Value = 0;
            numCK.Value = 0;
            txtTK.Text = "0.00";
            txtDiemHe4.Text = "0.0";
            txtXepLoai.Text = "Chưa xếp loại";
            txtGhiChu.Clear();
            txtSearch.Clear(); // Làm trống cả thanh tìm kiếm để reset bộ lọc DataGridView

            _isBinding = true;
            if (cboStudent.Items.Count > 0) cboStudent.SelectedIndex = 0;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;

            UpdateSemesterAndYearInfo(); // Reset lại Học kỳ/Năm học theo môn mặc định đầu tiên
            _isBinding = false;
            if (dgvScores.DataSource is DataTable dt) dt.DefaultView.RowFilter = string.Empty;
            dgvScores.ClearSelection();
        }

        // XỬ LÝ NÚT IN BẢNG ĐIỂM
        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Print/Báo cáo đang kết nối tới Driver máy in mặc định hệ thống. Đang gửi dữ liệu hiển thị...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // XỬ LÝ NÚT XUẤT FILE (EXPORT EXCEL/CSV) - FIX LỖI FONT TIẾNG VIỆT
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvScores.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu điểm nào để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                // Thay đổi bộ lọc sang đuôi mở rộng .xlsx của Excel thay vì file .csv dễ lỗi font
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Bang_Diem_Sinh_Vien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Trích xuất an toàn dữ liệu từ Grid (Hỗ trợ cả trường hợp đang dùng bộ lọc DefaultView)
                        DataView currentView = dgvScores.DataSource as DataView;
                        DataTable dtSource;

                        if (currentView != null)
                        {
                            dtSource = currentView.ToTable();
                        }
                        else if (dgvScores.DataSource is DataTable dt)
                        {
                            dtSource = dt.DefaultView.ToTable();
                        }
                        else
                        {
                            // Trường hợp bất khả kháng: Tự dựng bảng từ các dòng hiện thị trên DataGridView
                            dtSource = new DataTable();
                            foreach (DataGridViewColumn col in dgvScores.Columns)
                            {
                                dtSource.Columns.Add(col.Name);
                            }
                            foreach (DataGridViewRow row in dgvScores.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    DataRow dr = dtSource.NewRow();
                                    foreach (DataGridViewColumn col in dgvScores.Columns)
                                    {
                                        dr[col.Name] = row.Cells[col.Name].Value;
                                    }
                                    dtSource.Rows.Add(dr);
                                }
                            }
                        }

                        // Tiến hành khởi tạo Workbook bằng ClosedXML
                        using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("BangDiem");

                            // Khởi tạo danh sách cấu trúc tiêu đề cột hiển thị ra Excel dựa trên Grid hiện tại
                            List<string> headers = new List<string>();
                            foreach (DataGridViewColumn col in dgvScores.Columns)
                            {
                                if (col.Visible) // Chỉ lấy các cột đang hiện trên màn hình công cụ
                                {
                                    headers.Add(col.HeaderText);
                                }
                            }

                            // 1. Đổ hàng tiêu đề (Header) với phong cách giao diện phẳng màu xanh chủ đạo
                            for (int i = 0; i < headers.Count; i++)
                            {
                                var cell = ws.Cell(1, i + 1);
                                cell.Value = headers[i];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#0D6EFD"); // Màu xanh thương hiệu
                                cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                                cell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                            }

                            // 2. Đổ dữ liệu chi tiết điểm số của từng sinh viên vào các dòng kế tiếp
                            int rIndex = 2;
                            foreach (DataGridViewRow row in dgvScores.Rows)
                            {
                                if (row.IsNewRow) continue;

                                int cIndex = 1;
                                foreach (DataGridViewColumn col in dgvScores.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        var val = row.Cells[col.Index].Value;
                                        var currentCell = ws.Cell(rIndex, cIndex);

                                        // Kiểm tra và định dạng kiểu dữ liệu số (Điểm số) để Excel tính toán được
                                        if (val != null && decimal.TryParse(val.ToString(), out decimal numVal) &&
                                            (col.HeaderText.Contains("Điểm") || col.HeaderText.Contains("Hệ 4")))
                                        {
                                            currentCell.Value = numVal;
                                            currentCell.Style.NumberFormat.Format = "0.00";
                                            currentCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                                        }
                                        else
                                        {
                                            // Dữ liệu chữ text thông thường (Mã SV, Họ tên, X xếp loại...)
                                            currentCell.Value = val?.ToString() ?? "";
                                            currentCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                                        }

                                        // Tạo đường viền mảnh xung quanh ô dữ liệu cho dễ nhìn
                                        currentCell.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                                        cIndex++;
                                    }
                                }
                                rIndex++;
                            }

                            // Tự động căn chỉnh độ rộng tất cả các cột Excel cho vừa vặn, không bị che khuất chữ
                            ws.Columns().AdjustToContents();

                            // Thực hiện lưu tệp xuống đĩa cứng
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất danh sách bảng điểm sinh viên ra file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi tạo tệp Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // SỰ KIỆN GÕ CHỮ VÀO Ô TÌM KIẾM (TEXTCHANGED) - TÌM KIẾM NÂNG CAO ĐA NĂNG
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ThucHienBoLocDGV(sender, e);
        }

        // Sự kiện click nút Cập nhật điểm
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn dòng điểm cần cập nhật trên lưới, hoặc chọn đầy đủ Sinh viên và Môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mssv = cboStudent.SelectedValue.ToString();
            string maMH = cboCourse.SelectedValue.ToString();
            decimal qt = numQT.Value;
            decimal ck = numCK.Value;
            decimal tk = Math.Round(qt * 0.4m + ck * 0.6m, 2);
            string ghiChu = txtGhiChu.Text;

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn cập nhật điểm môn {maMH} cho sinh viên {mssv} không?", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (_scoreRepo.SaveScore(mssv, maMH, qt, ck, tk, ghiChu))
                {
                    MessageBox.Show("Cập nhật điểm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGridData();
                    LoadQuickStats();
                    btnClear_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Vui lòng kiểm tra lại kết nối cơ sở dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadQuickStats()
        {
            try
            {
                DataTable dt = _scoreRepo.GetQuickStats();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Nạp số liệu vào các nhãn hiển thị (Định dạng chuỗi số cho đẹp)
                    lblTotalStudents.Text = string.Format("{0:#,##0}", row["TotalStudents"]);
                    lblTotalCourses.Text = string.Format("{0:#,##0}", row["TotalCourses"]);
                    lblTotalScores.Text = string.Format("{0:#,##0}", row["TotalScoresEntered"]);

                    decimal avgScore = Convert.ToDecimal(row["AverageSchoolScore"]);
                    lblAvgScore.Text = avgScore.ToString("0.00");
                }
            }
            catch
            {
                // Tránh crash ứng dụng nếu lỗi kết nối ngầm
                lblTotalStudents.Text = "0";
                lblTotalCourses.Text = "0";
                lblTotalScores.Text = "0";
                lblAvgScore.Text = "0.00";
            }
        }
        private void HienThiDiemTheoCapHocVienMonHoc()
        {
            // Kiểm tra nếu chưa chọn đủ Sinh viên và Môn học thì bỏ qua
            if (cboStudent.SelectedValue == null || cboCourse.SelectedValue == null) return;

            string selectedMssv = cboStudent.SelectedValue.ToString();
            string selectedMaMH = cboCourse.SelectedValue.ToString();

            if (dgvScores.DataSource is DataTable dt)
            {
                // Tìm kiếm trong DataTable xem đã tồn tại dòng điểm của Sinh viên + Môn học này chưa
                // Lưu ý: Tên cột phải trùng khít với tên cột trong database/GetScoreList() của bạn (ví dụ: "Mã SV", "Mã MH")
                DataRow[] rows = dt.Select($"[Mã SV] = '{selectedMssv}' AND [Mã MH] = '{selectedMaMH}'");

                if (rows.Length > 0)
                {
                    DataRow row = rows[0];

                    // 1. Đồng bộ Điểm Quá trình
                    if (decimal.TryParse(row["Điểm QT (40%)"]?.ToString(), out decimal qt))
                        numQT.Value = qt;
                    else
                        numQT.Value = 0;

                    // 2. Đồng bộ Điểm Cuối kỳ
                    if (decimal.TryParse(row["Điểm CK (60%)"]?.ToString(), out decimal ck))
                        numCK.Value = ck;
                    else
                        numCK.Value = 0;

                    // 3. Đồng bộ Ghi chú
                    txtGhiChu.Text = row["Ghi Chú"]?.ToString() ?? "";

                    // (Tùy chọn) Chọn luôn dòng đó dưới DataGridView cho người dùng thấy trực quan
                    foreach (DataGridViewRow dgvRow in dgvScores.Rows)
                    {
                        if (dgvRow.Cells["Mã SV"].Value?.ToString() == selectedMssv &&
                            dgvRow.Cells["Mã MH"].Value?.ToString() == selectedMaMH)
                        {
                            dgvRow.Selected = true;
                            dgvScores.FirstDisplayedScrollingRowIndex = dgvRow.Index; // Cuộn tới dòng đó
                            break;
                        }
                    }
                }
                else
                {
                    // Nếu chưa có điểm (cặp SV - MH này mới tinh), trả các ô nhập liệu về mặc định
                    numQT.Value = 0;
                    numCK.Value = 0;
                    txtGhiChu.Clear();
                    txtTK.Text = "0.00";
                    txtDiemHe4.Text = "0.0";
                    txtXepLoai.Text = "Chưa xếp loại";
                }
            }
        }
    }
}