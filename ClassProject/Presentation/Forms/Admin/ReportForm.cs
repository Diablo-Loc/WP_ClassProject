using ClassProject.DataAccess.Db; // Đảm bảo gọi đúng Namespace chứa class kết nối My_DB của bạn
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Data;
using Microsoft.Data.SqlClient; // Sử dụng thư viện kết nối SQL Server hiện đại cho .NET
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;

// Đặt Alias rút gọn cho OpenXml Wordprocessing để tuyệt đối không bị xung đột với iTextSharp và WinForms
using WpWord = DocumentFormat.OpenXml.Wordprocessing;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ReportForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly string _connString;

        public ReportForm()
        {
            InitializeComponent();
            // Lấy chuỗi kết nối trực tiếp từ đối tượng kết nối DB của bạn
            _connString = _db.GetConnection().ConnectionString;
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
            LoadComboboxData();
            LoadReportData(); // Mặc định nạp toàn bộ danh sách điểm lên bảng để xem trước
        }

        // ⭐ LÀM ĐẸP GRID THEO STYLE SLATE CAO CẤP + BẬT LỀ TRÁI CHO STT
        private void StyleGrid()
        {
            if (dgvReport == null) return;

            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.EnableHeadersVisualStyles = false;
            dgvReport.RowTemplate.Height = 35;
            dgvReport.GridColor = Color.FromArgb(241, 245, 249);
            dgvReport.BackgroundColor = Color.White;
            dgvReport.BorderStyle = BorderStyle.None;

            // Header tiêu đề
            dgvReport.ColumnHeadersHeight = 35;
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // Màu Slate tối
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold);

            // Dòng xen kẽ
            dgvReport.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            // Hiệu ứng chọn dòng
            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvReport.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);

            // Bật hàng tiêu đề trái để làm cột số thứ tự tự động
            dgvReport.RowHeadersVisible = true;
            dgvReport.RowHeadersWidth = 45;
            dgvReport.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        // Tự động vẽ số thứ tự vào lề trái chuẩn xác
        private void dgvReport_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string stt = (e.RowIndex + 1).ToString();
            System.Drawing.Font rFont = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold);
            Brush rBrush = new SolidBrush(Color.FromArgb(100, 116, 139));

            float x = e.RowBounds.Location.X + (dgvReport.RowHeadersWidth - e.Graphics.MeasureString(stt, rFont).Width) / 2;
            float y = e.RowBounds.Location.Y + (e.RowBounds.Height - rFont.Height) / 2;

            e.Graphics.DrawString(stt, rFont, rBrush, x, y);
        }

        // NẠP DỮ LIỆU VÀO COMBOBOX BỘ LỌC TỪ DATABASE CỦA BẠN
        private void LoadComboboxData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    // Nạp ComboBox Môn học (Từ bảng Course: MaMH, TenMH)
                    SqlDataAdapter daCourse = new SqlDataAdapter("SELECT MaMH, TenMH FROM Course", conn);
                    DataTable dtCourse = new DataTable();
                    daCourse.Fill(dtCourse);

                    DataRow rowCourse = dtCourse.NewRow();
                    rowCourse["MaMH"] = DBNull.Value;
                    rowCourse["TenMH"] = "-- Tất cả môn học --";
                    dtCourse.Rows.InsertAt(rowCourse, 0);

                    cbMonHoc.DataSource = dtCourse;
                    cbMonHoc.DisplayMember = "TenMH";
                    cbMonHoc.ValueMember = "MaMH";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách bộ lọc: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TẢI DỮ LIỆU BÁO CÁO ĐIỂM SỐ TỪ BA BẢNG: Score, Students, Course
        private void LoadReportData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    // Truy vấn liên kết bảng theo đúng cấu trúc dữ liệu của bạn
                    string query = @"SELECT s.MSSV, (s.FirstName + ' ' + s.LastName) AS HoTen, 
                                            c.MaMH, c.TenMH, sc.DiemQT, sc.DiemCK, sc.DiemTK
                                     FROM Score sc
                                     INNER JOIN Students s ON sc.MSSV = s.MSSV
                                     INNER JOIN Course c ON sc.MaMH = c.MaMH
                                     WHERE (1=1)";

                    SqlCommand cmd = new SqlCommand("", conn);

                    // Thêm điều kiện lọc nếu người dùng chọn môn học cụ thể
                    if (cbMonHoc.SelectedValue != null && cbMonHoc.SelectedValue != DBNull.Value)
                    {
                        query += " AND sc.MaMH = @MaMH";
                        cmd.Parameters.AddWithValue("@MaMH", cbMonHoc.SelectedValue.ToString());
                    }

                    cmd.CommandText = query;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvReport.DataSource = dt;

                    // Định dạng tên cột Tiếng Việt hiển thị trên GridView
                    if (dgvReport.Columns.Count > 0)
                    {
                        if (dgvReport.Columns.Contains("MSSV")) dgvReport.Columns["MSSV"].HeaderText = "MSSV";
                        if (dgvReport.Columns.Contains("HoTen")) dgvReport.Columns["HoTen"].HeaderText = "Họ và Tên";
                        if (dgvReport.Columns.Contains("MaMH")) dgvReport.Columns["MaMH"].HeaderText = "Mã Môn";
                        if (dgvReport.Columns.Contains("TenMH")) dgvReport.Columns["TenMH"].HeaderText = "Tên Môn Học";
                        if (dgvReport.Columns.Contains("DiemQT")) dgvReport.Columns["DiemQT"].HeaderText = "Điểm QT";
                        if (dgvReport.Columns.Contains("DiemCK")) dgvReport.Columns["DiemCK"].HeaderText = "Điểm CK";
                        if (dgvReport.Columns.Contains("DiemTK")) dgvReport.Columns["DiemTK"].HeaderText = "Điểm TK";

                        dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải bảng báo cáo: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        // ==========================================
        // ⭐ LOGIC XUẤT EXCEL (SỬ DỤNG EPPLUS)
        // ==========================================
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "BaoCao_DiemSo_" + DateTime.Now.ToString("yyyyMMdd")
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Sử dụng XLWorkbook của ClosedXML (Giống hệt thư viện bạn đang dùng để Import)
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Báo cáo điểm số");

                        // 1. Tiêu đề báo cáo lớn nằm ở hàng 1
                        worksheet.Cell(1, 1).Value = "BÁO CÁO DANH SÁCH ĐIỂM SỐ SINH VIÊN";
                        int totalColumns = dgvReport.Columns.Count + 1; // Cộng thêm 1 cột STT
                        worksheet.Range(1, 1, 1, totalColumns).Merge(); // Trộn các ô tiêu đề
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // 2. Tạo tiêu đề cột tại hàng 3 (Thêm cột STT đầu tiên)
                        worksheet.Cell(3, 1).Value = "STT";
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            worksheet.Cell(3, i + 2).Value = dgvReport.Columns[i].HeaderText;
                        }

                        // Đổ màu nền Slate tối + chữ trắng cho thanh tiêu đề (Đồng bộ giao diện)
                        var headerRange = worksheet.Range(3, 1, 3, totalColumns);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A"); // Màu Slate tối
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // 3. Đổ dữ liệu từ lưới DataGridView vào các ô Excel từ hàng 4
                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            int currentRow = r + 4;
                            worksheet.Cell(currentRow, 1).Value = r + 1; // Ghi Số thứ tự (STT)

                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                string cellValue = dgvReport.Rows[r].Cells[c].Value?.ToString() ?? "";
                                worksheet.Cell(currentRow, c + 2).Value = cellValue;
                            }
                        }

                        // Tự động căn chỉnh độ rộng các cột cho vừa vặn chữ, không bị che khuất
                        worksheet.Columns().AdjustToContents();

                        // Lưu file Excel xuống ổ đĩa
                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Xuất file Excel báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Excel bằng ClosedXML: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // ⭐ LOGIC XUẤT PDF (SỬ DỤNG ITEXTSHARP VÀ ĐÃ FIX XUNG ĐỘT)
        // ==========================================
        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = "BaoCao_DiemSo_" + DateTime.Now.ToString("yyyyMMdd")
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Cấu hình Font chữ hệ thống (Arial) để hiển thị đầy đủ dấu Tiếng Việt
                    string sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                    BaseFont bf = BaseFont.CreateFont(sysFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);

                    // Sử dụng định danh tường minh iTextSharp.text để tránh xung đột với WinForms/OpenXML
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);

                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        // Tiêu đề văn bản PDF
                        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("BÁO CÁO DANH SÁCH ĐIỂM SỐ SINH VIÊN\n\n", fontTitle)
                        {
                            Alignment = iTextSharp.text.Element.ALIGN_CENTER
                        };
                        pdfDoc.Add(title);

                        // Tạo bảng PDF (+1 cột STT)
                        PdfPTable pdfTable = new PdfPTable(dgvReport.Columns.Count + 1)
                        {
                            WidthPercentage = 100
                        };

                        // Khởi tạo các ô Tiêu đề cột
                        pdfTable.AddCell(new PdfPCell(new iTextSharp.text.Phrase("STT", fontHeader)) { BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42) });
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            PdfPCell cell = new PdfPCell(new iTextSharp.text.Phrase(dgvReport.Columns[i].HeaderText, fontHeader))
                            {
                                BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42)
                            };
                            pdfTable.AddCell(cell);
                        }

                        // Duyệt chèn dữ liệu
                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            pdfTable.AddCell(new iTextSharp.text.Phrase((r + 1).ToString(), fontBody));
                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                pdfTable.AddCell(new iTextSharp.text.Phrase(dgvReport.Rows[r].Cells[c].Value?.ToString(), fontBody));
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                    }
                    MessageBox.Show("Xuất file PDF báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // ⭐ LOGIC XUẤT WORD CHUẨN ĐỊNH DẠNG .DOCX (SỬ DỤNG ALIAS WPWORD)
        // ==========================================
        private void btnExportWord_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = "BaoCao_DiemSo_" + DateTime.Now.ToString("yyyyMMdd")
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new WpWord.Document();
                        WpWord.Body body = mainPart.Document.AppendChild(new WpWord.Body());

                        // 1. Tiêu đề văn bản Word lớn nằm giữa
                        WpWord.Paragraph pTitle = body.AppendChild(new WpWord.Paragraph());
                        WpWord.ParagraphProperties pPr = pTitle.AppendChild(new WpWord.ParagraphProperties());
                        pPr.AppendChild(new WpWord.Justification() { Val = WpWord.JustificationValues.Center });

                        WpWord.Run rTitle = pTitle.AppendChild(new WpWord.Run());
                        WpWord.RunProperties rPr = rTitle.AppendChild(new WpWord.RunProperties());
                        rPr.AppendChild(new WpWord.Bold());
                        rPr.AppendChild(new WpWord.FontSize() { Val = "32" }); // Cỡ chữ 16pt trong OpenXML
                        rTitle.AppendChild(new WpWord.Text("BÁO CÁO DANH SÁCH ĐIỂM SỐ SINH VIÊN"));

                        body.AppendChild(new WpWord.Paragraph(new WpWord.Run(new WpWord.Break()))); // Tạo khoảng cách cách dòng

                        // 2. Cấu hình bảng dữ liệu và đường viền bao quanh các ô
                        WpWord.Table table = new WpWord.Table();
                        WpWord.TableProperties tblProp = new WpWord.TableProperties(
                            new WpWord.TableBorders(
                                new WpWord.TopBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                                new WpWord.BottomBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                                new WpWord.LeftBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                                new WpWord.RightBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                                new WpWord.InsideHorizontalBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                                new WpWord.InsideVerticalBorder() { Val = WpWord.BorderValues.Single, Size = 4 }
                            )
                        );
                        table.AppendChild(tblProp);

                        // 3. Tạo dòng tiêu đề cho bảng Word
                        WpWord.TableRow headerRow = new WpWord.TableRow();
                        headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text("STT")))));
                        foreach (DataGridViewColumn col in dgvReport.Columns)
                        {
                            headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text(col.HeaderText)))));
                        }
                        table.Append(headerRow);

                        // 4. Đọc dữ liệu ghi vào từng dòng trong bảng Word
                        for (int i = 0; i < dgvReport.Rows.Count; i++)
                        {
                            WpWord.TableRow dataRow = new WpWord.TableRow();
                            dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text((i + 1).ToString()))))); // Ghi Số thứ tự

                            for (int j = 0; j < dgvReport.Columns.Count; j++)
                            {
                                string cellValue = dgvReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                                dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text(cellValue)))));
                            }
                            table.Append(dataRow);
                        }

                        body.Append(table);
                        mainPart.Document.Save();
                    }

                    MessageBox.Show("Xuất file Word báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Word: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}