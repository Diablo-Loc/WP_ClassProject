using ClassProject.DataAccess.Db;
using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using Xceed.Document.NET;
using Xceed.Words.NET;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class TranscriptForm : Form
    {
        private readonly ScoreRepository _scoreRepository;
        private string _currentMssv;
        private bool _isProcessing = false;

        public TranscriptForm()
        {
            InitializeComponent();
            _scoreRepository = new ScoreRepository();
        }

        private async void TranscriptForm_Load(object sender, EventArgs e)
        {
            ResetSummary();

            // 1. Kiểm tra an toàn trạng thái đăng nhập của Session trước khi xử lý
            if (!UserSession.IsLoggedIn || string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Phiên làm việc đã hết hạn hoặc bạn chưa đăng nhập hệ thống!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // 2. Đồng bộ MSSV từ UserSession vào biến nội bộ của Form
            _currentMssv = UserSession.MSSV;

            if (txtMSSV != null)
            {
                txtMSSV.Text = _currentMssv;
            }

            // 3. Tự động tải dữ liệu bảng điểm ngay khi mở màn hình
            await LoadStudentTranscriptAsync();
        }

        /// TỰ ĐỘNG TẢI VÀ ĐỒNG BỘ BẢNG ĐIỂM SINH VIÊN
        private async Task LoadStudentTranscriptAsync()
        {
            await ExecuteSecureOperationAsync(async () =>
            {
                // Truy vấn dữ liệu bất đồng bộ từ tầng Repository (Luồng nền Task.Run)
                DataTable dt = await Task.Run(() => _scoreRepository.GetStudentTranscripts(_currentMssv));

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Hiện tại bạn chưa có dữ liệu điểm môn học nào trên hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvScores.DataSource = null;
                    ResetSummary();
                    return;
                }

                // Gán dữ liệu lên bảng hiển thị trực quan
                dgvScores.DataSource = dt;

                // Trích xuất thông tin cá nhân an toàn từ dòng dữ liệu đầu tiên thu được
                DataRow firstRow = dt.Rows[0];
                if (txtName != null)
                    txtName.Text = dt.Columns.Contains("Họ Tên") ? firstRow["Họ Tên"]?.ToString() ?? "Sinh Viên" : "Sinh Viên";

                if (txtClass != null)
                    txtClass.Text = dt.Columns.Contains("Tên Lớp") ? firstRow["Tên Lớp"]?.ToString() ?? "Chưa xếp lớp" : "Chưa xếp lớp";

                // Tính toán các chỉ số tích lũy học tập
                CalculateAndDisplayMetrics(dt);
            }, "Tải bảng điểm cá nhân");
        }

        /// TÍNH ĐIỂM TRUNG BÌNH TÍCH LŨY VÀ PHÂN LOẠI HỌC LỰC
        private void CalculateAndDisplayMetrics(DataTable dt)
        {
            int tongSoTC = 0;
            double tongDiemTichLuy = 0;

            foreach (DataRow row in dt.Rows)
            {
                // CHECKLIST: Kiểm tra DBNull an toàn hệ thống trước khi xử lý dữ liệu
                if (row["Số Tín Chỉ"] != DBNull.Value && row["Điểm TK"] != DBNull.Value)
                {
                    // CHECKLIST: Sử dụng TryParse thay vì Parse trực tiếp để chống crash ứng dụng
                    if (int.TryParse(row["Số Tín Chỉ"].ToString(), out int soTC) &&
                        double.TryParse(row["Điểm TK"].ToString(), out double diemTK))
                    {
                        tongDiemTichLuy += (diemTK * soTC);
                        tongSoTC += soTC;
                    }
                }
            }

            // CHECKLIST: Phòng chống triệt để lỗi chia cho số 0 (Division by Zero)
            if (tongSoTC == 0)
            {
                ResetSummary();
                return;
            }

            double diemTB = Math.Round(tongDiemTichLuy / tongSoTC, 2);

            string xepLoai = "Yếu";
            if (diemTB >= 9.0) xepLoai = "Xuất Sắc";
            else if (diemTB >= 8.0) xepLoai = "Giỏi";
            else if (diemTB >= 6.5) xepLoai = "Khá";
            else if (diemTB >= 5.0) xepLoai = "Trung Bình";

            // Cập nhật kết quả đồng bộ lên giao diện chính
            label5.Text = $"Tổng Số Tín Chỉ\n{tongSoTC}";
            label4.Text = $"Điểm Trung Bình\n{diemTB:0.00}";
            label6.Text = $"Xếp Loại\n{xepLoai}";
        }

        /// KÍCH HOẠT XUẤT PHIẾU ĐIỂM WORD (.DOCX)
        private async void BtnExportWord_Click(object sender, EventArgs e)
        {
            // CHECKLIST: Kiểm tra DataGridView rỗng trước khi thao tác xuất file
            if (dgvScores.DataSource == null || dgvScores.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu bảng điểm để kết xuất báo cáo Word!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Word Document (*.docx)|*.docx";
                sfd.FileName = $"BangDiem_CaNhan_{_currentMssv}.docx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;

                    // ĐỒNG BỘ UI: Thu thập dữ liệu UI an toàn trên luồng chính (Main Thread) trước khi đẩy vào Task.Run
                    string displayTC = label5.Text.Contains("\n") ? label5.Text.Split('\n')[1] : "0";
                    string displayTB = label4.Text.Contains("\n") ? label4.Text.Split('\n')[1] : "0.00";
                    string displayXL = label6.Text.Contains("\n") ? label6.Text.Split('\n')[1] : "Chưa xếp loại";
                    string studentName = txtName?.Text ?? "Sinh Viên";
                    string studentClass = txtClass?.Text ?? "Chưa xếp lớp";

                    // Bản sao cấu trúc DataTable để truyền luồng an toàn (tránh cross-thread với DataGridView)
                    DataTable dataCopy = ((DataTable)dgvScores.DataSource).Copy();

                    await ExecuteSecureOperationAsync(async () =>
                    {
                        // Chạy tác vụ I/O ghi file nặng trên luồng nền tránh đơ UI
                        await Task.Run(() => ExportTranscriptToDocx(filePath, dataCopy, studentName, studentClass, displayTC, displayTB, displayXL));

                        MessageBox.Show("Xuất file Word kết quả học tập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Mở file ngay sau khi xuất thành công
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                    }, "Kết xuất báo cáo Word");
                }
            }
        }

        /// LUỒNG NỀN XỬ LÝ ĐỘC LẬP TẠO FILE DOCX CHUẨN ĐỊNH DẠNG HÀNH CHÍNH
        private void ExportTranscriptToDocx(string filePath, DataTable dt, string name, string className, string tc, string tb, string xl)
        {
            using (DocX document = DocX.Create(filePath))
            {
                string fontName = "Times New Roman";

                // 1. Tiêu ngữ hành chính
                Table headerTable = document.AddTable(2, 2);
                headerTable.Alignment = Alignment.center;
                headerTable.Rows[0].Cells[0].Width = 260; headerTable.Rows[0].Cells[1].Width = 300;
                headerTable.Rows[0].Cells[0].Paragraphs[0].Append("BỘ GIÁO DỤC VÀ ĐÀO TẠO\nTRƯỜNG ĐẠI HỌC CÔNG NGHỆ\n").Bold().Font(fontName).FontSize(10).Alignment = Alignment.center;
                headerTable.Rows[0].Cells[1].Paragraphs[0].Append("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập - Tự do - Hạnh phúc\n").Bold().Font(fontName).FontSize(10).Alignment = Alignment.center;

                headerTable.Design = TableDesign.None; // Ẩn đường viền bảng tiêu ngữ
                document.InsertTable(headerTable);

                // 2. Tiêu đề văn bản
                var pTitle = document.InsertParagraph();
                pTitle.AppendLine("\nPHIẾU KẾT QUẢ HỌC TẬP CÁ NHÂN").Bold().FontSize(14).Alignment = Alignment.center;
                pTitle.Font(fontName);

                // 3. Phần hiển thị thông tin chi tiết sinh viên
                var pInfo = document.InsertParagraph();
                pInfo.AppendLine($"Mã số sinh viên: {_currentMssv}").Font(fontName).FontSize(11);
                pInfo.AppendLine($"Họ và tên: {name}").Font(fontName).FontSize(11);
                pInfo.AppendLine($"Lớp sinh hoạt: {className}\n").Font(fontName).FontSize(11);

                // 4. Cấu trúc bảng điểm môn học (Lấy từ DataTable đã copy thay vì dgvScores trực tiếp để đồng bộ luồng)
                int dataRowCount = dt.Rows.Count;
                Table table = document.AddTable(dataRowCount + 1, 6);
                table.Alignment = Alignment.center;
                table.Design = TableDesign.TableGrid; // Hiện lưới bảng sắc nét

                string[] headers = { "Mã Lớp HP", "Mã Môn", "Tên Môn Học", "Số TC", "Học Kỳ", "Điểm TK" };
                for (int i = 0; i < headers.Length; i++)
                {
                    table.Rows[0].Cells[i].Paragraphs[0].Append(headers[i]).Bold().Font(fontName).FontSize(11).Alignment = Alignment.center;
                }

                int wordRowIndex = 1;
                foreach (DataRow row in dt.Rows)
                {
                    // CHECKLIST: Sử dụng toán tử ?. và ?? "" để tránh NullReferenceException
                    table.Rows[wordRowIndex].Cells[0].Paragraphs[0].Append(row["Mã Lớp HP"]?.ToString() ?? "").Alignment = Alignment.center;
                    table.Rows[wordRowIndex].Cells[1].Paragraphs[0].Append(row["Mã Môn"]?.ToString() ?? "").Alignment = Alignment.center;
                    table.Rows[wordRowIndex].Cells[2].Paragraphs[0].Append(row["Tên Môn Học"]?.ToString() ?? "");
                    table.Rows[wordRowIndex].Cells[3].Paragraphs[0].Append(row["Số Tín Chỉ"]?.ToString() ?? "").Alignment = Alignment.center;
                    table.Rows[wordRowIndex].Cells[4].Paragraphs[0].Append(row["Học Kỳ"]?.ToString() ?? "").Alignment = Alignment.center;
                    table.Rows[wordRowIndex].Cells[5].Paragraphs[0].Append(row["Điểm TK"]?.ToString() ?? "").Alignment = Alignment.center;

                    for (int i = 0; i < 6; i++)
                    {
                        table.Rows[wordRowIndex].Cells[i].Paragraphs[0].Font(fontName).FontSize(10);
                    }
                    wordRowIndex++;
                }
                document.InsertTable(table);

                // 5. Báo cáo tổng hợp chân trang
                var pSummary = document.InsertParagraph();
                pSummary.AppendLine($"\nTổng số tín chỉ tích lũy: {tc}");
                pSummary.AppendLine($"Điểm trung bình tích lũy: {tb}");
                pSummary.AppendLine($"Xếp loại học lực tích lũy: {xl}").Bold();
                pSummary.Font(fontName).FontSize(11);

                document.Save();
            }
        }

        private void ResetSummary()
        {
            label5.Text = "Tổng Số Tín Chỉ\n0";
            label4.Text = "Điểm Trung Bình\n0.00";
            label6.Text = "Xếp Loại\nChưa xếp loại";
        }

        private async Task ExecuteSecureOperationAsync(Func<Task> businessLogic, string operationName)
        {
            if (_isProcessing) return;
            try
            {
                _isProcessing = true;
                this.UseWaitCursor = true;
                await businessLogic();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi thực hiện [{operationName}]: {ex.Message}", "Hệ thống lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.UseWaitCursor = false;
                _isProcessing = false;
            }
        }
    }
}