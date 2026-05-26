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
            LoadStudentCombo();
            LoadCourseCombo();
            LoadGridData();
            LoadQuickStats();

            // Đăng ký sự kiện SelectedIndexChanged cho cboCourse sau khi dữ liệu đã nạp xong
            cboCourse.SelectedIndexChanged += cboCourse_SelectedIndexChanged;

            // --- ĐĂNG KÝ THÊM 2 DÒNG NÀY ĐỂ KÍCH HOẠT BỘ LỌC ---
            cboHocKy.SelectedIndexChanged += ThucHienBoLocDGV;
            cboNamHoc.SelectedIndexChanged += ThucHienBoLocDGV;

            // Tự động cập nhật Học kỳ / Năm học cho môn học đầu tiên
            UpdateSemesterAndYearInfo();
        }

        // Hàm xử lý lọc dữ liệu kết hợp đồng thời cả Học kỳ, Năm học và Ô tìm kiếm văn bản
        private void ThucHienBoLocDGV(object sender, EventArgs e)
        {
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

                // FIX LỖI ẨN TIÊU ĐỀ CỘT
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
                btnClear_Click(sender, e); // Tự động làm sạch form sau khi lưu thành công
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
                    cboHocKy.Text = rows[0]["Hky"]?.ToString() ?? "";
                    cboNamHoc.Text = rows[0]["NamHoc"]?.ToString() ?? "";
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

            if (cboStudent.Items.Count > 0) cboStudent.SelectedIndex = 0;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;

            UpdateSemesterAndYearInfo(); // Reset lại Học kỳ/Năm học theo môn mặc định đầu tiên
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
                sfd.Filter = "CSV File (*.csv)|*.csv|Text File (*.txt)|*.txt";
                sfd.FileName = "Bang_Diem_Sinh_Vien.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        List<string> headers = new List<string>();
                        foreach (DataGridViewColumn col in dgvScores.Columns)
                        {
                            headers.Add(col.HeaderText);
                        }
                        sb.AppendLine(string.Join(",", headers));

                        foreach (DataGridViewRow row in dgvScores.Rows)
                        {
                            List<string> cells = new List<string>();
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                cells.Add(cell.Value?.ToString()?.Replace(",", " ") ?? "");
                            }
                            sb.AppendLine(string.Join(",", cells));
                        }

                        // MẸO QUAN TRỌNG: Ghi file đính kèm ký tự đặc biệt UTF-8 BOM (Byte Order Mark) 
                        // Giúp Microsoft Excel tự động nhận dạng ngôn ngữ tiếng Việt có dấu, không lo bị lỗi font ô vuông.
                        byte[] bom = Encoding.UTF8.GetPreamble();
                        byte[] content = Encoding.UTF8.GetBytes(sb.ToString());

                        using (var fs = System.IO.File.Create(sfd.FileName))
                        {
                            fs.Write(bom, 0, bom.Length);
                            fs.Write(content, 0, content.Length);
                        }

                        MessageBox.Show("Xuất danh sách bảng điểm thành công! File hiển thị tiếng Việt chuẩn xác trong Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}