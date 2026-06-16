using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ClassProject
{
    public partial class ListStudentForm : Form
    {
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();

        public ListStudentForm()
        {
            InitializeComponent();
            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);
        }

        private void ListStudentForm_Load(object sender, EventArgs e)
        {
            if (cboFilterGender.Items.Count > 0) cboFilterGender.SelectedIndex = 0;
            if (cbSort.Items.Count > 0) cbSort.SelectedIndex = 0;

            fillGrid();

            // Phân quyền tài khoản bảo mật dữ liệu
            if (UserSession.RoleId == 1)
            {
                btnInsert.Enabled = false;
                btnImportExcel.Enabled = false;
            }
        }

        public void fillGrid()
        {
            try
            {
                dgvStudents.DataSource = null;

                string keyword = txtSearch.Text.Trim();
                string gender = cboFilterGender.Text;
                if (gender.Contains("Tất cả")) gender = "Tất cả";

                DataTable dt = studentRepo.SearchStudents(keyword, gender);
                DataView dv = dt.DefaultView;

                // Xử lý tiêu chí sắp xếp động linh hoạt dựa trên DataView
                if (cbSort.SelectedIndex > 0)
                {
                    if (cbSort.Text == "Tên sinh viên")
                    {
                        if (dt.Columns.Contains("LastName")) dv.Sort = "LastName ASC, FirstName ASC";
                        else if (dt.Columns.Contains("Tên")) dv.Sort = "Tên ASC, Họ ASC";
                    }
                    else if (cbSort.Text == "MSSV")
                    {
                        if (dt.Columns.Contains("Mssv")) dv.Sort = "Mssv ASC";
                        else if (dt.Columns.Contains("Mã SV")) dv.Sort = "[Mã SV] ASC";
                    }
                }

                dgvStudents.DataSource = dv;

                // Cập nhật số lượng đếm dữ liệu trực quan lên Widget Card xanh dương
                lblTotalCount.Text = dv.Count.ToString();
                lblPaginationInfo.Text = $"Hiển thị 1 đến {dv.Count} của {dv.Count} sinh viên";

                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị danh sách: " + ex.Message, "Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvStudents.Columns.Count == 0) return;

            // 1. Ẩn các cột ID hệ thống không cần thiết
            if (dgvStudents.Columns["UserId"] != null) dgvStudents.Columns["UserId"].Visible = false;
            if (dgvStudents.Columns["Id"] != null) dgvStudents.Columns["Id"].Visible = false;

            // 2. Định dạng các cột thông tin cơ bản
            string[] mssvKeys = { "Mssv", "Mã SV", "MSSV" };
            foreach (var key in mssvKeys) if (dgvStudents.Columns[key] != null) dgvStudents.Columns[key].HeaderText = "Mã SV";

            if (dgvStudents.Columns["DateOfBirth"] != null) dgvStudents.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
            if (dgvStudents.Columns["Gender"] != null) dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
            if (dgvStudents.Columns["Phone"] != null) dgvStudents.Columns["Phone"].HeaderText = "Điện thoại";
            if (dgvStudents.Columns["Address"] != null) dgvStudents.Columns["Address"].HeaderText = "Địa chỉ";
            if (dgvStudents.Columns["Hometown"] != null) dgvStudents.Columns["Hometown"].HeaderText = "Quê quán";
            if (dgvStudents.Columns["Email"] != null) dgvStudents.Columns["Email"].HeaderText = "Email";

            if (dgvStudents.Columns["Picture"] != null)
            {
                dgvStudents.Columns["Picture"].HeaderText = "Hình ảnh";
                if (dgvStudents.Columns["Picture"] is DataGridViewImageColumn picCol) picCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }

            // 3. ĐỊNH DẠNG CHÍNH XÁC HỌ VÀ TÊN (Sửa lỗi ngược dữ liệu)
            string keyHo = dgvStudents.Columns.Contains("LastName") ? "LastName" : (dgvStudents.Columns.Contains("Họ") ? "Họ" : null);
            string keyTen = dgvStudents.Columns.Contains("FirstName") ? "FirstName" : (dgvStudents.Columns.Contains("Tên") ? "Tên" : null);

            if (keyHo != null && dgvStudents.Columns[keyHo] != null)
            {
                dgvStudents.Columns[keyHo].HeaderText = "Họ và tên đệm";
            }

            if (keyTen != null && dgvStudents.Columns[keyTen] != null)
            {
                dgvStudents.Columns[keyTen].HeaderText = "Tên";
            }

            // 4. Sắp xếp thứ tự hiển thị các cột từ trái qua phải cho đẹp mắt
            int currentIndex = 0;

            if (dgvStudents.Columns["Mssv"] != null) dgvStudents.Columns["Mssv"].DisplayIndex = currentIndex++;
            else if (dgvStudents.Columns["Mã SV"] != null) dgvStudents.Columns["Mã SV"].DisplayIndex = currentIndex++;

            if (keyHo != null) dgvStudents.Columns[keyHo].DisplayIndex = currentIndex++;
            if (keyTen != null) dgvStudents.Columns[keyTen].DisplayIndex = currentIndex++;

            // Tự động kéo dãn các cột vừa khít giao diện phẳng
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) => fillGrid();
        private void cboFilterGender_SelectedIndexChanged(object sender, EventArgs e) => fillGrid();
        private void cbSort_SelectedIndexChanged(object sender, EventArgs e) => fillGrid();

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboFilterGender.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
            fillGrid();
        }

        private void dgvStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || UserSession.RoleId == 1) return;

            try
            {
                string mssvCol = dgvStudents.Columns.Contains("Mssv") ? "Mssv" : "Mã SV";
                int mssv = Convert.ToInt32(dgvStudents.Rows[e.RowIndex].Cells[mssvCol].Value);

                using (AddStudentForm editForm = new AddStudentForm(mssv))
                {
                    editForm.ShowDialog();
                }
                fillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở cửa sổ chỉnh sửa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            using (AddStudentForm addForm = new AddStudentForm(0))
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
                        // Lấy chính xác đối tượng DataView hiện tại từ Grid dữ liệu
                        DataView currentView = dgvStudents.DataSource as DataView;
                        DataTable dtSource;

                        if (currentView != null)
                        {
                            dtSource = currentView.ToTable();
                        }
                        else
                        {
                            dtSource = (DataTable)dgvStudents.DataSource;
                        }

                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("Students");

                            // ĐỒNG BỘ ÁNH XẠ: Đổi lại cột nguồn trong CSDL để khớp tiêu đề Excel
                            // - LastName trong DB của bạn chứa Họ (Nguyễn Quốc, Trần Thiên...) => Đi với tiêu đề "Họ và tên đệm"
                            // - FirstName trong DB của bạn chứa Tên riêng (An, Ân, Bảo...) => Đi với tiêu đề "Tên"
                            var mappingCols = new[]
                            {
                        new { Src = dtSource.Columns.Contains("Mssv") ? "Mssv" : "Mã SV", Target = "Mã SV" },
                        new { Src = dtSource.Columns.Contains("LastName") ? "LastName" : "Họ", Target = "Họ và tên đệm" },
                        new { Src = dtSource.Columns.Contains("FirstName") ? "FirstName" : "Tên", Target = "Tên" },
                        new { Src = "DateOfBirth", Target = "Ngày sinh" },
                        new { Src = "Gender", Target = "Giới tính" },
                        new { Src = "Phone", Target = "Điện thoại" },
                        new { Src = "Address", Target = "Địa chỉ" },
                        new { Src = "Hometown", Target = "Quê quán" },
                        new { Src = "Email", Target = "Email" }
                    };

                            // 1. Tạo hàng tiêu đề (Header) phẳng màu xanh lam chuyên nghiệp
                            for (int i = 0; i < mappingCols.Length; i++)
                            {
                                var cell = ws.Cell(1, i + 1);
                                cell.Value = mappingCols[i].Target;
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
                                cell.Style.Font.FontColor = XLColor.White;
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }

                            // 2. Đổ dữ liệu chi tiết của sinh viên
                            int rIndex = 2;
                            foreach (DataRow row in dtSource.Rows)
                            {
                                for (int cIndex = 0; cIndex < mappingCols.Length; cIndex++)
                                {
                                    string colName = mappingCols[cIndex].Src;
                                    if (dtSource.Columns.Contains(colName))
                                    {
                                        var val = row[colName];

                                        // Định dạng lại ngày tháng ngắn gọn
                                        if (val is DateTime dVal)
                                            ws.Cell(rIndex, cIndex + 1).Value = dVal.ToString("dd/MM/yyyy");
                                        else
                                            ws.Cell(rIndex, cIndex + 1).Value = val?.ToString() ?? "";
                                    }
                                }
                                rIndex++;
                            }

                            // Tự động căn chỉnh độ rộng các cột Excel vừa khít chữ
                            ws.Columns().AdjustToContents();

                            // Thực hiện lưu tệp
                            wb.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất dữ liệu ra file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi tạo tệp Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Files|*.xlsx" };
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
                            string rawMssv = row.Cell(1).Value.ToString().Trim();
                            if (string.IsNullOrEmpty(rawMssv)) continue;

                            int mssv = Convert.ToInt32(rawMssv);
                            if (studentRepo.IsMssvExist(mssv)) { skipped++; continue; }

                            Student student = new Student
                            {
                                Mssv = mssv,
                                UserId = mssv,
                                FirstName = row.Cell(2).Value.ToString().Trim(),
                                LastName = row.Cell(3).Value.ToString().Trim(),
                                DateOfBirth = row.Cell(4).DataType == XLDataType.DateTime ? row.Cell(4).GetDateTime() : DateTime.Parse(row.Cell(4).Value.ToString()),
                                Gender = row.Cell(5).Value.ToString().Trim(),
                                Phone = row.Cell(6).Value.ToString().Trim(),
                                Address = row.Cell(7).Value.ToString().Trim(),
                                Hometown = row.Cell(8).Value.ToString().Trim(),
                                Email = row.Cell(9).Value.ToString().Trim()
                            };

                            if (studentRepo.AddStudent(student)) added++; else skipped++;
                        }
                        catch { skipped++; }
                    }
                }
                MessageBox.Show($"Hoàn tất Nhập dữ liệu:\n- Thành công: {added}\n- Thất bại hoặc trùng mã: {skipped}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cấu trúc file excel không tương thích: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nếu lưới DataGridView không có dữ liệu thì không xử lý
            if (dgvStudents.Rows.Count == 0)
            {
                MessageBox.Show("Danh sách sinh viên đang trống, không thể xuất PDF!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Thiết lập hộp thoại lưu file PDF
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveFileDialog.FileName = "Danh_sach_sinh_vien_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Khởi tạo đối tượng Document với kích thước trang A4 nằm ngang (Landscape) để vừa vặn 7 cột dữ liệu
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 15f, 15f, 20f, 20f);

                try
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        // --- CẤU HÌNH PHÔNG CHỮ TIẾNG VIỆT UNICODE ---
                        string sysFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                        iTextSharp.text.pdf.BaseFont bf = iTextSharp.text.pdf.BaseFont.CreateFont(sysFontPath, iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);

                        iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 102, 204));
                        iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                        iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL);

                        // --- TẠO TIÊU ĐỀ FILE PDF ---
                        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("DANH SÁCH SINH VIÊN", fontTitle);
                        title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        title.SpacingAfter = 20f;
                        pdfDoc.Add(title);

                        // --- KHỞI TẠO BẢNG PDF CHUẨN ĐÚNG 7 CỘT MỚI ---
                        iTextSharp.text.pdf.PdfPTable pdfTable = new iTextSharp.text.pdf.PdfPTable(7);
                        pdfTable.WidthPercentage = 100;

                        // Thiết lập tỷ lệ co giãn tương ứng cho 7 cột
                        float[] widths = new float[] { 12f, 20f, 12f, 13f, 10f, 13f, 20f };
                        pdfTable.SetWidths(widths);

                        // --- TẠO TIÊU ĐỀ (HEADER) CHO BẢNG PDF (THÊM NGÀY SINH VÀ QUÊ QUÁN) ---
                        string[] headers = { "Mã SV", "Họ và tên đệm", "Tên", "Ngày sinh", "Giới tính", "Quê quán", "Email" };
                        foreach (string headerText in headers)
                        {
                            iTextSharp.text.pdf.PdfPCell headerCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, fontHeader));
                            headerCell.BackgroundColor = new iTextSharp.text.BaseColor(0, 102, 204);
                            headerCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                            headerCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            headerCell.Padding = 6f;
                            pdfTable.AddCell(headerCell);
                        }

                        // --- VÒNG LẶP ĐỌC DỮ LIỆU TỪ DATAGRIDVIEW VÀ ĐẨY VÀO PDF ---
                        foreach (DataGridViewRow row in dgvStudents.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string maSV = "";
                            string hoTenDem = "";
                            string ten = "";
                            string ngaySinhRaw = "";
                            string gioiTinh = "";
                            string queQuan = "";
                            string email = "";

                            // Duyệt động qua từng ô trong hàng để lấy dữ liệu theo đúng HeaderText hiển thị
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.OwningColumn.HeaderText == "Mã SV")
                                    maSV = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Họ và tên đệm")
                                    hoTenDem = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Tên")
                                    ten = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Ngày sinh")
                                    ngaySinhRaw = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Giới tính")
                                    gioiTinh = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Quê quán")
                                    queQuan = cell.Value?.ToString()?.Trim() ?? "";

                                else if (cell.OwningColumn.HeaderText == "Email")
                                    email = cell.Value?.ToString()?.Trim() ?? "";
                            }

                            // THỰC HIỆN ĐỊNH DẠNG LẠI NGÀY SINH: Cắt bỏ phần giờ (12:00:00 AM)
                            string ngaySinhFormatted = ngaySinhRaw;
                            if (DateTime.TryParse(ngaySinhRaw, out DateTime parsedDate))
                            {
                                ngaySinhFormatted = parsedDate.ToString("dd/MM/yyyy"); // Định dạng chuẩn ngày/tháng/năm
                            }

                            // --- TIẾN HÀNH ĐẨY DỮ LIỆU VÀO CÁC Ô THEO THỨ TỰ TIÊU ĐỀ MỚI ---

                            // 1. Ô Mã SV (Căn giữa)
                            iTextSharp.text.pdf.PdfPCell cell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(maSV, fontBody));
                            cell1.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                            cell1.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell1.Padding = 5f;
                            pdfTable.AddCell(cell1);

                            // 2. Ô Họ và tên đệm (Căn trái)
                            iTextSharp.text.pdf.PdfPCell cell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(hoTenDem, fontBody));
                            cell2.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell2.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell2.Padding = 5f;
                            pdfTable.AddCell(cell2);

                            // 3. Ô Tên (Căn trái)
                            iTextSharp.text.pdf.PdfPCell cell3 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(ten, fontBody));
                            cell3.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell3.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell3.Padding = 5f;
                            pdfTable.AddCell(cell3);

                            // 4. Ô Ngày sinh mới thêm (Căn giữa)
                            iTextSharp.text.pdf.PdfPCell cell4 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(ngaySinhFormatted, fontBody));
                            cell4.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                            cell4.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell4.Padding = 5f;
                            pdfTable.AddCell(cell4);

                            // 5. Ô Giới tính (Căn giữa)
                            iTextSharp.text.pdf.PdfPCell cell5 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(gioiTinh, fontBody));
                            cell5.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                            cell5.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell5.Padding = 5f;
                            pdfTable.AddCell(cell5);

                            // 6. Ô Quê quán mới thêm (Căn trái)
                            iTextSharp.text.pdf.PdfPCell cell6 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(queQuan, fontBody));
                            cell6.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell6.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell6.Padding = 5f;
                            pdfTable.AddCell(cell6);

                            // 7. Ô Email (Căn trái)
                            iTextSharp.text.pdf.PdfPCell cell7 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(email, fontBody));
                            cell7.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT;
                            cell7.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                            cell7.Padding = 5f;
                            pdfTable.AddCell(cell7);
                        }

                        // Thêm bảng hoàn chỉnh vào tài liệu và kết thúc luồng ghi file
                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        stream.Close();
                    }

                    MessageBox.Show("Xuất danh sách sinh viên ra file PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException)
                {
                    MessageBox.Show("Lỗi: Không thể ghi đè dữ liệu. Vui lòng kiểm tra và đóng file PDF nếu file đó đang được mở bởi một ứng dụng khác!", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi không mong muốn: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}