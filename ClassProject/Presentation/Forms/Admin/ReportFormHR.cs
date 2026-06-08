using ClassProject.DataAccess.Db;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using ClosedXML.Excel; // Đã đổi sang dùng ClosedXML thay cho OfficeOpenXml (EPPlus)
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

// Tránh xung đột thư viện giữa OpenXml (Word) và iTextSharp (PDF)
using WpWord = DocumentFormat.OpenXml.Wordprocessing;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ReportFormHR : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly string _connString;

        public ReportFormHR()
        {
            InitializeComponent();
            _connString = _db.GetConnection().ConnectionString;

            // ĐÃ XÓA dòng cấu hình bản quyền phức tạp của EPPlus cũ (Vì ClosedXML là mã nguồn mở hoàn toàn miễn phí)
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            StyleGrid();
            LoadFilterGiangVien();
            LoadFilterMonHoc();
            LoadReportData(); // Tải dữ liệu phân công gốc lên Grid
        }

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

            dgvReport.ColumnHeadersHeight = 35;
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // Slate tối màu Admin
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold);

            dgvReport.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        // 1. NẠP DỮ LIỆU BỘ LỌC GIẢNG VIÊN (cbGiangVien)
        private void LoadFilterGiangVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = "SELECT Id, Username FROM dbo.Users WHERE RoleId = 2"; // Role Giảng viên
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DataRow row = dt.NewRow();
                    row["Id"] = DBNull.Value;
                    row["Username"] = "-- Tất cả giảng viên --";
                    dt.Rows.InsertAt(row, 0);

                    cbGiangVien.DataSource = dt;
                    cbGiangVien.DisplayMember = "Username";
                    cbGiangVien.ValueMember = "Id";
                }
            }
            catch { }
        }

        // 2. NẠP DỮ LIỆU BỘ LỌC MÔN HỌC (cbMonHoc)
        private void LoadFilterMonHoc()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = "SELECT MaMH, TenMH FROM dbo.Course";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DataRow row = dt.NewRow();
                    row["MaMH"] = DBNull.Value;
                    row["TenMH"] = "-- Tất cả môn học --";
                    dt.Rows.InsertAt(row, 0);

                    cbMonHoc.DataSource = dt;
                    cbMonHoc.DisplayMember = "TenMH";
                    cbMonHoc.ValueMember = "MaMH";
                }
            }
            catch { }
        }

        // 3. LẤY DỮ LIỆU BÁO CÁO PHÂN CÔNG GIẢNG DẠY CHUẨN NGHIỆP VỤ HR
        private void LoadReportData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("proc_GetTeachingAssignments", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtRaw = new DataTable();
                    da.Fill(dtRaw);

                    string filterExpression = "1=1";

                    if (cbGiangVien.SelectedValue != null && cbGiangVien.SelectedValue != DBNull.Value)
                    {
                        filterExpression += $" AND HRID = {cbGiangVien.SelectedValue}";
                    }

                    if (cbMonHoc.SelectedValue != null && cbMonHoc.SelectedValue != DBNull.Value)
                    {
                        filterExpression += $" AND MaMH = '{cbMonHoc.SelectedValue}'";
                    }

                    DataRow[] filteredRows = dtRaw.Select(filterExpression);
                    DataTable dtResult = dtRaw.Clone();
                    foreach (DataRow r in filteredRows)
                    {
                        dtResult.ImportRow(r);
                    }

                    dgvReport.DataSource = dtResult;

                    if (dgvReport.Columns.Count > 0)
                    {
                        if (dgvReport.Columns.Contains("ID")) dgvReport.Columns["ID"].Visible = false; // Ẩn ID khóa chính
                        if (dgvReport.Columns.Contains("HRID")) dgvReport.Columns["HRID"].HeaderText = "Mã Nhân Sự (HRID)";
                        if (dgvReport.Columns.Contains("HRName")) dgvReport.Columns["HRName"].HeaderText = "Tên Giảng Viên";
                        if (dgvReport.Columns.Contains("MaMH")) dgvReport.Columns["MaMH"].HeaderText = "Mã Môn Học";
                        if (dgvReport.Columns.Contains("TenMH")) dgvReport.Columns["TenMH"].HeaderText = "Tên Môn Học Được Phân Công";

                        dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu phân công: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        // ============================================================================
        // ⭐ LOGIC XUẤT EXCEL CHUẨN ĐÃ ĐỔI SANG THƯ VIỆN CLOSEDXML (KHÔNG LỖI BẢN QUYỀN)
        // ============================================================================
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "BaoCao_PhanCong_" + DateTime.Now.ToString("yyyyMMdd")
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Khởi tạo bảng tính bằng ClosedXML (XLWorkbook)
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Thống kê giảng dạy");

                        // --- 1. Tạo Tiêu đề lớn của báo cáo ---
                        worksheet.Cell(1, 1).Value = "BÁO CÁO THỐNG KÊ PHÂN CÔNG GIẢNG DẠY - ADMIN HR";
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                        worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#0F172A"); // Màu Slate tối

                        // --- 2. Tạo Tiêu đề cột (Chỉ lấy các cột đang hiển thị hiển thị trên Grid) ---
                        worksheet.Cell(3, 1).Value = "STT"; // Tạo thêm cột Số thứ tự tự động

                        int excelColIdx = 2; // Cột Excel bắt đầu từ cột 2 (Cột B) cho dữ liệu Grid
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            if (dgvReport.Columns[i].Visible) // Bỏ qua cột ID bị ẩn
                            {
                                worksheet.Cell(3, excelColIdx).Value = dgvReport.Columns[i].HeaderText;
                                excelColIdx++;
                            }
                        }

                        // Định dạng thanh tiêu đề: Màu nền Slate tối (#0F172A), chữ trắng, in đậm
                        int totalUsedColumns = excelColIdx - 1;
                        var headerRange = worksheet.Range(3, 1, 3, totalUsedColumns);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A");
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Trộn ô tiêu đề lớn ở hàng 1 tương ứng với độ rộng bảng
                        worksheet.Range(1, 1, 1, totalUsedColumns).Merge();
                        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // --- 3. Đổ dữ liệu từ GridView vào file Excel ---
                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            int currentRow = r + 4; // Bắt đầu ghi từ hàng thứ 4
                            worksheet.Cell(currentRow, 1).Value = r + 1; // Số thứ tự dòng (STT)

                            int colIdx = 2;
                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                if (dgvReport.Columns[c].Visible) // Chỉ ghi những ô có cột hiển thị
                                {
                                    string cellValue = dgvReport.Rows[r].Cells[c].Value?.ToString() ?? "";
                                    worksheet.Cell(currentRow, colIdx).Value = cellValue;
                                    colIdx++;
                                }
                            }
                        }

                        // Tự động kéo dãn độ rộng các cột vừa khít chữ, không bị lỗi hiển thị ###
                        worksheet.Columns().AdjustToContents();

                        // Thực hiện lưu file
                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Xuất file Excel báo cáo phân công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi hệ thống khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu phân công để xuất báo cáo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = "BaoCao_PhanCong_GiangDay_" + DateTime.Now.ToString("yyyyMMdd")
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                    BaseFont bf = BaseFont.CreateFont(sysFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);

                    // Sử dụng khổ dọc A4 thông thường vì số cột vừa phải gọn gàng
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 20f, 20f);

                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("BÁO CÁO TỔNG HỢP PHÂN CÔNG GIẢNG DẠY (HR)\n\n", fontTitle)
                        {
                            Alignment = iTextSharp.text.Element.ALIGN_CENTER
                        };
                        pdfDoc.Add(title);

                        // Đếm số cột hiển thị (bỏ qua ID ẩn)
                        int visibleColumnsCount = 1; // +1 cho cột STT
                        for (int i = 0; i < dgvReport.Columns.Count; i++) if (dgvReport.Columns[i].Visible) visibleColumnsCount++;

                        PdfPTable pdfTable = new PdfPTable(visibleColumnsCount) { WidthPercentage = 100 };

                        // Ghi Header PDF
                        pdfTable.AddCell(new PdfPCell(new iTextSharp.text.Phrase("STT", fontHeader)) { BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42) });
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            if (dgvReport.Columns[i].Visible)
                            {
                                pdfTable.AddCell(new PdfPCell(new iTextSharp.text.Phrase(dgvReport.Columns[i].HeaderText, fontHeader)) { BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42) });
                            }
                        }

                        // Ghi nội dung dòng
                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            pdfTable.AddCell(new iTextSharp.text.Phrase((r + 1).ToString(), fontBody));
                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                if (dgvReport.Columns[c].Visible)
                                {
                                    pdfTable.AddCell(new iTextSharp.text.Phrase(dgvReport.Rows[r].Cells[c].Value?.ToString(), fontBody));
                                }
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                    }
                    MessageBox.Show("Xuất file PDF báo cáo nhân sự thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất PDF HR: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExportWord_Click(object sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu phân công để xuất báo cáo!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                FileName = "BaoCao_PhanCong_GiangDay_" + DateTime.Now.ToString("yyyyMMdd")
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

                        // Tiêu đề Word nằm giữa
                        WpWord.Paragraph pTitle = body.AppendChild(new WpWord.Paragraph());
                        WpWord.ParagraphProperties pPr = pTitle.AppendChild(new WpWord.ParagraphProperties());
                        pPr.AppendChild(new WpWord.Justification() { Val = WpWord.JustificationValues.Center });

                        WpWord.Run rTitle = pTitle.AppendChild(new WpWord.Run());
                        WpWord.RunProperties rPr = rTitle.AppendChild(new WpWord.RunProperties());
                        rPr.AppendChild(new WpWord.Bold());
                        rPr.AppendChild(new WpWord.FontSize() { Val = "28" }); // Cỡ chữ tiêu đề
                        rTitle.AppendChild(new WpWord.Text("THỐNG KÊ PHÂN CÔNG GIẢNG DẠY NHÂN VIÊN"));

                        body.AppendChild(new WpWord.Paragraph(new WpWord.Run(new WpWord.Break())));

                        // Cấu hình viền bảng Word
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

                        // Thiết lập dòng tiêu đề bảng
                        WpWord.TableRow headerRow = new WpWord.TableRow();
                        headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text("STT")))));
                        foreach (DataGridViewColumn col in dgvReport.Columns)
                        {
                            if (col.Visible)
                            {
                                headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text(col.HeaderText)))));
                            }
                        }
                        table.Append(headerRow);

                        // Đọc nạp dữ liệu phân công vào từng dòng
                        for (int i = 0; i < dgvReport.Rows.Count; i++)
                        {
                            WpWord.TableRow dataRow = new WpWord.TableRow();
                            dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text((i + 1).ToString())))));

                            for (int j = 0; j < dgvReport.Columns.Count; j++)
                            {
                                if (dgvReport.Columns[j].Visible)
                                {
                                    string cellValue = dgvReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text(cellValue)))));
                                }
                            }
                            table.Append(dataRow);
                        }

                        body.Append(table);
                        mainPart.Document.Save();
                    }

                    MessageBox.Show("Xuất file Word báo cáo nhân sự thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Word HR: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}