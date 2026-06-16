using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassProject.BusinessLogic.Services; // Import tầng Service

// 1. Thư viện Excel (ClosedXML)
using ClosedXML.Excel;

// 2. Thư viện Word (OpenXml)
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using WpWord = DocumentFormat.OpenXml.Wordprocessing;

// 3. Thư viện PDF (iTextSharp)
using iTextText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ReportFormHR : Form
    {
        // Sử dụng Service thay vì My_DB thô
        private readonly TeachingAssignmentService _assignmentService;

        public ReportFormHR()
        {
            InitializeComponent();

            // Khởi tạo Service. 
            // Lưu ý: Bạn có thể truyền chuỗi kết nối lấy từ cấu hình hệ thống hoặc thông qua một Helper quản lý DB của dự án.
            string connString = new ClassProject.DataAccess.Db.My_DB().GetConnection().ConnectionString;
            _assignmentService = new TeachingAssignmentService(connString);
        }

        private async void ReportForm_Load(object sender, EventArgs e)
        {
            StyleGrid();

            // Chạy song song các tác vụ nạp bộ lọc để tối ưu hóa hiệu năng UI
            await Task.WhenAll(
                LoadFilterGiangVienAsync(),
                LoadFilterMonHocAsync()
            );

            await LoadReportDataAsync();
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
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);

            dgvReport.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private async Task LoadFilterGiangVienAsync()
        {
            try
            {
                DataTable dt = await _assignmentService.GetDropdownTeachersAsync();

                if (dt.Columns.Contains("Id"))
                {
                    dt.Columns["Id"].AllowDBNull = true;
                }

                DataRow row = dt.NewRow();
                row["Id"] = DBNull.Value; 
                row["Username"] = "-- Tất cả giảng viên --";
                dt.Rows.InsertAt(row, 0);

                cbGiangVien.DataSource = dt;
                cbGiangVien.DisplayMember = "Username";
                cbGiangVien.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách giảng viên: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFilterMonHocAsync()
        {
            try
            {
                DataTable dt = await _assignmentService.GetDropdownCoursesAsync();

                if (dt.Columns.Contains("MaMH"))
                {
                    dt.Columns["MaMH"].AllowDBNull = true;
                }

                DataRow row = dt.NewRow();
                row["MaMH"] = DBNull.Value;
                row["TenMH"] = "-- Tất cả môn học --";
                dt.Rows.InsertAt(row, 0);

                cbMonHoc.DataSource = dt;
                cbMonHoc.DisplayMember = "TenMH";
                cbMonHoc.ValueMember = "MaMH";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách môn học: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadReportDataAsync()
        {
            try
            {
                // Xác định giá trị bộ lọc từ UI
                int? teacherId = null;
                if (cbGiangVien.SelectedValue != null && cbGiangVien.SelectedValue != DBNull.Value)
                {
                    teacherId = Convert.ToInt32(cbGiangVien.SelectedValue);
                }

                string maMH = null;
                if (cbMonHoc.SelectedValue != null && cbMonHoc.SelectedValue != DBNull.Value)
                {
                    maMH = cbMonHoc.SelectedValue.ToString();
                }

                // Gọi dữ liệu thông qua tầng nghiệp vụ Service (Bất đồng bộ)
                DataTable dtResult = await _assignmentService.GetReportDataAsync(teacherId, maMH);
                dgvReport.DataSource = dtResult;

                if (dgvReport.Columns.Count > 0)
                {
                    if (dgvReport.Columns.Contains("ID")) dgvReport.Columns["ID"].Visible = false;
                    if (dgvReport.Columns.Contains("HRID")) dgvReport.Columns["HRID"].HeaderText = "Mã Nhân Sự (HRID)";
                    if (dgvReport.Columns.Contains("HRName")) dgvReport.Columns["HRName"].HeaderText = "Tên Giảng Viên";
                    if (dgvReport.Columns.Contains("MaMH")) dgvReport.Columns["MaMH"].HeaderText = "Mã Môn Học";
                    if (dgvReport.Columns.Contains("TenMH")) dgvReport.Columns["TenMH"].HeaderText = "Tên Môn Học Được Phân Công";

                    dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu phân công: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
        }

        // ⭐ XUẤT EXCEL - GIỮ NGUYÊN LOGIC GIAO DIỆN XUẤT SẮC CỦA BẠN
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
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Thống kê giảng dạy");
                        worksheet.ShowGridLines = true;

                        worksheet.Cell(1, 1).Value = "BÁO CÁO THỐNG KÊ PHÂN CÔNG GIẢNG DẠY - ADMIN HR";
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                        worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#0F172A");

                        worksheet.Cell(3, 1).Value = "STT";
                        int excelColIdx = 2;
                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            if (dgvReport.Columns[i].Visible)
                            {
                                worksheet.Cell(3, excelColIdx).Value = dgvReport.Columns[i].HeaderText;
                                excelColIdx++;
                            }
                        }

                        int totalUsedColumns = excelColIdx - 1;
                        var headerRange = worksheet.Range(3, 1, 3, totalUsedColumns);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A");
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#94A3B8");

                        worksheet.Range(1, 1, 1, totalUsedColumns).Merge();
                        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            int currentRow = r + 4;
                            var cellStt = worksheet.Cell(currentRow, 1);
                            cellStt.SetValue(r + 1);
                            cellStt.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            int colIdx = 2;
                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                if (dgvReport.Columns[c].Visible)
                                {
                                    string cellValue = dgvReport.Rows[r].Cells[c].Value?.ToString() ?? "";
                                    if (dgvReport.Columns[c].Name == "HRID" && int.TryParse(cellValue, out int numValue))
                                    {
                                        worksheet.Cell(currentRow, colIdx).SetValue(numValue);
                                    }
                                    else
                                    {
                                        worksheet.Cell(currentRow, colIdx).SetValue(cellValue);
                                    }
                                    colIdx++;
                                }
                            }

                            var dataRowRange = worksheet.Range(currentRow, 1, currentRow, totalUsedColumns);
                            dataRowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            dataRowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            dataRowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
                            dataRowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");
                        }

                        if (dgvReport.Rows.Count > 0)
                        {
                            var targetRange = worksheet.Range(4, 2, dgvReport.Rows.Count + 3, 2);
                            targetRange.AddConditionalFormat().DataBar(XLColor.SkyBlue);
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("Xuất báo cáo Excel thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi hệ thống khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                using (new DisposableCursor(Cursors.Default)) { }
            }
        }

        // ⭐ XUẤT PDF
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
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    string sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                    iTextPdf.BaseFont bf = iTextPdf.BaseFont.CreateFont(sysFontPath, iTextPdf.BaseFont.IDENTITY_H, iTextPdf.BaseFont.EMBEDDED);

                    iTextText.Font fontTitle = new iTextText.Font(bf, 14, iTextText.Font.BOLD);
                    iTextText.Font fontSub = new iTextText.Font(bf, 10, iTextText.Font.BOLD);
                    iTextText.Font fontBody = new iTextText.Font(bf, 9, iTextText.Font.NORMAL);
                    iTextText.Font fontHeader = new iTextText.Font(bf, 9, iTextText.Font.BOLD, iTextText.BaseColor.WHITE);

                    iTextText.Document pdfDoc = new iTextText.Document(iTextText.PageSize.A4, 30f, 30f, 40f, 40f);

                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        iTextPdf.PdfWriter writer = iTextPdf.PdfWriter.GetInstance(pdfDoc, stream);
                        writer.PageEvent = new PdfHeaderFooterEvent(bf);

                        pdfDoc.Open();

                        iTextPdf.PdfPTable headerTable = new iTextPdf.PdfPTable(1) { WidthPercentage = 100 };
                        iTextText.Paragraph pTitle = new iTextText.Paragraph();
                        pTitle.Add(new iTextText.Chunk("TRƯỜNG ĐẠI HỌC CÔNG NGHỆ KỸ THUẬT TPHCM\n", fontSub));
                        pTitle.Add(new iTextText.Chunk("BÁO CÁO TỔNG HỢP PHÂN CÔNG GIẢNG DẠY (HR)\n", fontTitle));
                        pTitle.Alignment = iTextText.Element.ALIGN_CENTER;

                        iTextPdf.PdfPCell titleCell = new iTextPdf.PdfPCell(pTitle) { Border = iTextText.Rectangle.NO_BORDER };
                        headerTable.AddCell(titleCell);
                        pdfDoc.Add(headerTable);
                        pdfDoc.Add(new iTextText.Paragraph("\n"));

                        int visibleColumnsCount = 1;
                        for (int i = 0; i < dgvReport.Columns.Count; i++) if (dgvReport.Columns[i].Visible) visibleColumnsCount++;

                        iTextPdf.PdfPTable pdfTable = new iTextPdf.PdfPTable(visibleColumnsCount) { WidthPercentage = 100 };

                        pdfTable.AddCell(new iTextPdf.PdfPCell(new iTextText.Phrase("STT", fontHeader))
                        {
                            BackgroundColor = new iTextText.BaseColor(15, 23, 42),
                            HorizontalAlignment = iTextText.Element.ALIGN_CENTER,
                            Padding = 6f
                        });

                        for (int i = 0; i < dgvReport.Columns.Count; i++)
                        {
                            if (dgvReport.Columns[i].Visible)
                            {
                                pdfTable.AddCell(new iTextPdf.PdfPCell(new iTextText.Phrase(dgvReport.Columns[i].HeaderText, fontHeader))
                                {
                                    BackgroundColor = new iTextText.BaseColor(15, 23, 42),
                                    HorizontalAlignment = iTextText.Element.ALIGN_CENTER,
                                    Padding = 6f
                                });
                            }
                        }

                        for (int r = 0; r < dgvReport.Rows.Count; r++)
                        {
                            var cellStt = new iTextPdf.PdfPCell(new iTextText.Phrase((r + 1).ToString(), fontBody)) { Padding = 5f, HorizontalAlignment = iTextText.Element.ALIGN_CENTER };
                            pdfTable.AddCell(cellStt);

                            for (int c = 0; c < dgvReport.Columns.Count; c++)
                            {
                                if (dgvReport.Columns[c].Visible)
                                {
                                    string cellVal = dgvReport.Rows[r].Cells[c].Value?.ToString() ?? "";
                                    var dataCell = new iTextPdf.PdfPCell(new iTextText.Phrase(cellVal, fontBody)) { Padding = 5f };
                                    pdfTable.AddCell(dataCell);
                                }
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                    }
                    MessageBox.Show("Xuất file PDF thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                using (new DisposableCursor(Cursors.Default)) { }
            }
        }

        // ⭐ XUẤT WORD
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
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(sfd.FileName, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new WpWord.Document();
                        WpWord.Body body = mainPart.Document.AppendChild(new WpWord.Body());

                        WpWord.Paragraph pTitle = body.AppendChild(new WpWord.Paragraph());
                        WpWord.ParagraphProperties pPr = pTitle.AppendChild(new WpWord.ParagraphProperties());
                        pPr.AppendChild(new WpWord.Justification() { Val = WpWord.JustificationValues.Center });

                        WpWord.Run rTitle = pTitle.AppendChild(new WpWord.Run());
                        WpWord.RunProperties rPr = rTitle.AppendChild(new WpWord.RunProperties());
                        rPr.AppendChild(new WpWord.Bold());
                        rPr.AppendChild(new WpWord.FontSize() { Val = "28" });
                        rTitle.AppendChild(new WpWord.Text("THỐNG KÊ PHÂN CÔNG GIẢNG DẠY NHÂN VIÊN"));

                        body.AppendChild(new WpWord.Paragraph(new WpWord.Run(new WpWord.Break())));

                        WpWord.Table table = new WpWord.Table();
                        WpWord.TableBorders borders = new WpWord.TableBorders(
                            new WpWord.TopBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                            new WpWord.BottomBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                            new WpWord.LeftBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                            new WpWord.RightBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "CBD5E1" },
                            new WpWord.InsideHorizontalBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "E2E8F0" },
                            new WpWord.InsideVerticalBorder() { Val = WpWord.BorderValues.Single, Size = 4, Color = "E2E8F0" }
                        );

                        WpWord.TableWidth tblWidth = new WpWord.TableWidth() { Width = "5000", Type = WpWord.TableWidthUnitValues.Pct };
                        WpWord.TableCellMargin margins = new WpWord.TableCellMargin(
                            new WpWord.TopMargin() { Width = "140", Type = WpWord.TableWidthUnitValues.Dxa },
                            new WpWord.BottomMargin() { Width = "140", Type = WpWord.TableWidthUnitValues.Dxa },
                            new WpWord.LeftMargin() { Width = "180", Type = WpWord.TableWidthUnitValues.Dxa },
                            new WpWord.RightMargin() { Width = "180", Type = WpWord.TableWidthUnitValues.Dxa }
                        );

                        WpWord.TableProperties tblProp = new WpWord.TableProperties(borders, tblWidth, margins);
                        table.AppendChild(tblProp);

                        WpWord.TableRow headerRow = new WpWord.TableRow();
                        Action<string> addHeaderCell = (text) => {
                            WpWord.TableCell cell = new WpWord.TableCell();
                            WpWord.TableCellProperties cellProp = new WpWord.TableCellProperties(
                                new WpWord.Shading() { Val = WpWord.ShadingPatternValues.Clear, Color = "auto", Fill = "0F172A" }
                            );
                            cell.AppendChild(cellProp);

                            WpWord.Paragraph p = new WpWord.Paragraph(new WpWord.Run(
                                new WpWord.RunProperties(new WpWord.Bold(), new WpWord.Color() { Val = "FFFFFF" }),
                                new WpWord.Text(text)
                            ));
                            cell.AppendChild(p);
                            headerRow.Append(cell);
                        };

                        addHeaderCell("STT");
                        foreach (DataGridViewColumn col in dgvReport.Columns)
                        {
                            if (col.Visible) addHeaderCell(col.HeaderText);
                        }
                        table.Append(headerRow);

                        for (int i = 0; i < dgvReport.Rows.Count; i++)
                        {
                            WpWord.TableRow dataRow = new WpWord.TableRow();

                            WpWord.TableCell cellStt = new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text((i + 1).ToString()))));
                            cellStt.AppendChild(new WpWord.TableCellProperties(new WpWord.TableCellWidth() { Type = WpWord.TableWidthUnitValues.Auto }));
                            dataRow.Append(cellStt);

                            for (int j = 0; j < dgvReport.Columns.Count; j++)
                            {
                                if (dgvReport.Columns[j].Visible)
                                {
                                    string cellValue = dgvReport.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    WpWord.TableCell dataCell = new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text(cellValue))));
                                    dataCell.AppendChild(new WpWord.TableCellProperties(new WpWord.TableCellWidth() { Type = WpWord.TableWidthUnitValues.Auto }));
                                    dataRow.Append(dataCell);
                                }
                            }
                            table.Append(dataRow);
                        }

                        body.Append(table);
                        mainPart.Document.Save();
                    }
                    MessageBox.Show("Xuất file Word thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất Word: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                using (new DisposableCursor(Cursors.Default)) { }
            }
        }

        // Subclass phục vụ Footer PDF
        public class PdfHeaderFooterEvent : iTextPdf.PdfPageEventHelper
        {
            private iTextText.Font _fontFooter;
            public PdfHeaderFooterEvent(iTextPdf.BaseFont bf)
            {
                _fontFooter = new iTextText.Font(bf, 8, iTextText.Font.ITALIC, iTextText.BaseColor.LIGHT_GRAY);
            }

            public override void OnEndPage(iTextPdf.PdfWriter writer, iTextText.Document document)
            {
                base.OnEndPage(writer, document);

                iTextPdf.PdfPTable footerTable = new iTextPdf.PdfPTable(2);
                footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                footerTable.LockedWidth = true;

                string dateText = $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}";
                iTextPdf.PdfPCell cellLeft = new iTextPdf.PdfPCell(new iTextText.Phrase(dateText, _fontFooter))
                {
                    Border = iTextText.Rectangle.NO_BORDER,
                    HorizontalAlignment = iTextText.Element.ALIGN_LEFT
                };
                footerTable.AddCell(cellLeft);

                string pageText = $"Trang {writer.PageNumber}";
                iTextPdf.PdfPCell cellRight = new iTextPdf.PdfPCell(new iTextText.Phrase(pageText, _fontFooter))
                {
                    Border = iTextText.Rectangle.NO_BORDER,
                    HorizontalAlignment = iTextText.Element.ALIGN_RIGHT
                };
                footerTable.AddCell(cellRight);

                footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
            }
        }

        private class DisposableCursor : IDisposable
        {
            public DisposableCursor(Cursor cursor) { Cursor.Current = cursor; }
            public void Dispose() { }
        }
    }
}