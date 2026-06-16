using ClassProject.Services;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ReportForm : Form
    {
        private readonly IReportRepository _reportRepo;
        private DataTable _currentReportTable;
        private bool _isLoading = false;

        // CHUẨN THỰC TẾ: Nên truyền Repository qua Constructor (Dependency Injection)
        // Nếu trường học chưa áp dụng DI Container, có thể giữ overload mặc định nhưng khuyến khích dùng cách này
        public ReportForm(IReportRepository reportRepo)
        {
            InitializeComponent();
            _reportRepo = reportRepo ?? throw new ArgumentNullException(nameof(reportRepo));
        }

        public ReportForm() : this(new ReportRepository()) // Constructor dự phòng cho designer
        {
        }

        private async void ReportForm_Load(object sender, EventArgs e)
        {
            // Bảo vệ sự kiện vẽ STT, tránh việc đăng ký lặp khi Form load lại
            dgvReport.RowPostPaint -= dgvReport_RowPostPaint;
            dgvReport.RowPostPaint += dgvReport_RowPostPaint;

            StyleGrid();

            // Nạp dữ liệu bộ lọc trước
            await LoadComboboxDataAsync();

            // Tự động nạp dữ liệu báo cáo lần đầu
            await LoadReportDataAsync();
        }

        private void StyleGrid()
        {
            if (dgvReport == null) return;

            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.EnableHeadersVisualStyles = false;
            dgvReport.ReadOnly = true;
            dgvReport.RowTemplate.Height = 35;
            dgvReport.GridColor = Color.FromArgb(241, 245, 249);
            dgvReport.BackgroundColor = Color.White;
            dgvReport.BorderStyle = BorderStyle.None;

            dgvReport.ColumnHeadersHeight = 38;
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvReport.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvReport.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);

            dgvReport.RowHeadersVisible = true;
            dgvReport.RowHeadersWidth = 45;
            dgvReport.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void dgvReport_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string stt = (e.RowIndex + 1).ToString();
            using (Font rFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (Brush rBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                float x = e.RowBounds.Location.X + (dgvReport.RowHeadersWidth - e.Graphics.MeasureString(stt, rFont).Width) / 2;
                float y = e.RowBounds.Location.Y + (e.RowBounds.Height - rFont.Height) / 2;
                e.Graphics.DrawString(stt, rFont, rBrush, x, y);
            }
        }

        private async Task LoadComboboxDataAsync()
        {
            try
            {
                cbMonHoc.SelectedIndexChanged -= CbMonHoc_SelectedIndexChanged;

                DataTable dtCourse = await _reportRepo.GetCoursesAsync();

                DataRow rowCourse = dtCourse.NewRow();
                rowCourse["MaMH"] = DBNull.Value;
                rowCourse["TenMH"] = "-- Tất cả môn học --";
                dtCourse.Rows.InsertAt(rowCourse, 0);

                cbMonHoc.DataSource = dtCourse;
                cbMonHoc.DisplayMember = "TenMH";
                cbMonHoc.ValueMember = "MaMH";

                cbMonHoc.SelectedIndexChanged += CbMonHoc_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách bộ lọc: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadReportDataAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                btnXemBaoCao.Enabled = false;
                this.Cursor = Cursors.WaitCursor; // UX: Đổi con trỏ chuột sang trạng thái chờ

                // CHUẨN HÓA: Lấy SelectedValue an toàn, ngắn gọn hơn code cũ
                string selectedMaMH = cbMonHoc.SelectedValue is string mamh ? mamh : null;
                if (string.IsNullOrWhiteSpace(selectedMaMH)) selectedMaMH = null;

                _currentReportTable = await _reportRepo.GetScoreReportDataAsync(selectedMaMH);

                // Clone cấu trúc và đổi tên cột hiển thị thân thiện với người dùng
                DataTable displayTable = _currentReportTable.Copy();

                string[] sourceCols = { "MSSV", "HoTen", "MaMH", "TenMH", "DiemQT", "DiemCK", "DiemTK" };
                string[] destCols = { "MSSV", "Họ và Tên", "Mã Môn", "Tên Môn Học", "Điểm QT", "Điểm CK", "Điểm TK" };

                for (int i = 0; i < sourceCols.Length; i++)
                {
                    if (displayTable.Columns.Contains(sourceCols[i]))
                        displayTable.Columns[sourceCols[i]].ColumnName = destCols[i];
                }

                dgvReport.DataSource = displayTable;
                dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải bảng báo cáo: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
                btnXemBaoCao.Enabled = true;
                this.Cursor = Cursors.Default; // Trả con trỏ chuột về mặc định
            }
        }

        private async void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
        }

        private async void CbMonHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            await LoadReportDataAsync();
        }

        // =========================================================================
        // XỬ LÝ XUẤT FILE BẤT ĐỒNG BỘ (ASYNC) - GIÚP KHÔNG BỊ TREO GIAO DIỆN KHI FILE NẶNG
        // =========================================================================

        private async void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (!ValidateDataBeforeExport()) return;

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook (*.xlsx)|*.xlsx", FileName = $"BaoCao_DiemSo_{DateTime.Now:yyyyMMdd}" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        // Đẩy tác vụ ghi file xuống luồng ngầm (Background Thread) bằng Task.Run
                        await Task.Run(() => ExportService.ToExcel(_currentReportTable, sfd.FileName, "Báo cáo danh sách điểm số sinh viên"));

                        MessageBox.Show("Xuất file Excel báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi hệ thống khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (!ValidateDataBeforeExport()) return;

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf", FileName = $"BaoCao_DiemSo_{DateTime.Now:yyyyMMdd}" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        // Chạy ngầm tránh block UI Thread
                        await Task.Run(() => ExportService.ToPdf(_currentReportTable, sfd.FileName, "Báo cáo danh sách điểm số sinh viên"));

                        MessageBox.Show("Xuất file PDF báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi hệ thống khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void btnExportWord_Click(object sender, EventArgs e)
        {
            if (!ValidateDataBeforeExport()) return;

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Word Document (*.docx)|*.docx", FileName = $"BaoCao_DiemSo_{DateTime.Now:yyyyMMdd}" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        // Chạy ngầm tránh block UI Thread
                        await Task.Run(() => ExportService.ToWord(_currentReportTable, sfd.FileName, "Báo cáo danh sách điểm số sinh viên"));

                        MessageBox.Show("Xuất file Word báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi hệ thống khi xuất Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        // Hàm tiện ích kiểm tra dữ liệu trước khi xuất (Gom nhóm tránh lặp code - DRY Principle)
        private bool ValidateDataBeforeExport()
        {
            if (_currentReportTable == null || _currentReportTable.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu hợp lệ trên lưới để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}