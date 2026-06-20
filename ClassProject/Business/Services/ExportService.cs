using System;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using WpWord = DocumentFormat.OpenXml.Wordprocessing;

namespace ClassProject.Business.Services
{
    public static class ExportService
    {
        private static readonly string ArialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");

        public static void ToExcel(DataTable dt, string filePath, string reportTitle)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Báo cáo");

                // Tiêu đề lớn
                worksheet.Cell(1, 1).Value = reportTitle.ToUpper();
                worksheet.Range(1, 1, 1, dt.Columns.Count + 1).Merge();
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Header bảng
                worksheet.Cell(3, 1).Value = "STT";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cell(3, i + 2).Value = dt.Columns[i].ColumnName;
                }

                var headerRange = worksheet.Range(3, 1, 3, dt.Columns.Count + 1);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A");
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ Data
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    int currentRow = r + 4;
                    worksheet.Cell(currentRow, 1).Value = r + 1;
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        worksheet.Cell(currentRow, c + 2).Value = dt.Rows[r][c]?.ToString() ?? string.Empty;
                    }
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }

        public static void ToPdf(DataTable dt, string filePath, string reportTitle)
        {
            // Fix lỗi Ambiguous: Sử dụng đầy đủ namespace iTextSharp.text.Document
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 15f, 15f, 20f, 20f);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                iTextSharp.text.pdf.BaseFont bf = iTextSharp.text.pdf.BaseFont.CreateFont(ArialFontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);

                // Sửa đổi tận gốc: Ép chính xác class Font của iTextSharp
                iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(15, 23, 42));
                iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL);

                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph(reportTitle.ToUpper() + "\n\n", fontTitle)
                {
                    Alignment = iTextSharp.text.Element.ALIGN_CENTER
                };
                pdfDoc.Add(title);

                iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(dt.Columns.Count + 1) { WidthPercentage = 100 };

                pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("STT", fontHeader)) { BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42), HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                foreach (DataColumn col in dt.Columns)
                {
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(col.ColumnName, fontHeader)) { BackgroundColor = new iTextSharp.text.BaseColor(15, 23, 42), HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                }

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((r + 1).ToString(), fontBody)) { HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        pdfTable.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(dt.Rows[r][c]?.ToString() ?? string.Empty, fontBody)));
                    }
                }

                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
            }
        }

        public static void ToWord(DataTable dt, string filePath, string reportTitle)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new WpWord.Document();
                WpWord.Body body = mainPart.Document.AppendChild(new WpWord.Body());

                WpWord.Paragraph pTitle = body.AppendChild(new WpWord.Paragraph());
                WpWord.ParagraphProperties pPr = pTitle.AppendChild(new WpWord.ParagraphProperties());
                pPr.AppendChild(new WpWord.Justification() { Val = WpWord.JustificationValues.Center });
                WpWord.Run rTitle = pTitle.AppendChild(new WpWord.Run());

                // FIX LỖI CONVERT STRING TO FLOAT: Tạo đối tượng FontSize trống, sau đó gán giá trị chuỗi vào thuộc tính .Val một cách an toàn
                var runProperties = new WpWord.RunProperties();
                runProperties.AppendChild(new WpWord.Bold());

                var fontSize = new WpWord.FontSize();
                fontSize.Val = "32"; // Gán trực tiếp qua Property thay vì truyền vào Constructor giúp tương thích mọi phiên bản OpenXML SDK

                runProperties.AppendChild(fontSize);
                rTitle.AppendChild(runProperties);
                rTitle.AppendChild(new WpWord.Text(reportTitle.ToUpper()));

                body.AppendChild(new WpWord.Paragraph(new WpWord.Run(new WpWord.Break())));

                WpWord.Table table = new WpWord.Table();
                table.AppendChild(new WpWord.TableProperties(
                    new WpWord.TableBorders(
                        new WpWord.TopBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                        new WpWord.BottomBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                        new WpWord.LeftBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                        new WpWord.RightBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                        new WpWord.InsideHorizontalBorder() { Val = WpWord.BorderValues.Single, Size = 4 },
                        new WpWord.InsideVerticalBorder() { Val = WpWord.BorderValues.Single, Size = 4 }
                    )
                ));

                WpWord.TableRow headerRow = new WpWord.TableRow();
                headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text("STT")))));
                foreach (DataColumn col in dt.Columns)
                {
                    headerRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.RunProperties(new WpWord.Bold()), new WpWord.Text(col.ColumnName)))));
                }
                table.Append(headerRow);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    WpWord.TableRow dataRow = new WpWord.TableRow();
                    dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text((i + 1).ToString())))));
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellValue = dt.Rows[i][j]?.ToString() ?? string.Empty;
                        dataRow.Append(new WpWord.TableCell(new WpWord.Paragraph(new WpWord.Run(new WpWord.Text(cellValue)))));
                    }
                    table.Append(dataRow);
                }

                body.Append(table);
                mainPart.Document.Save();
            }
        }
    }
}