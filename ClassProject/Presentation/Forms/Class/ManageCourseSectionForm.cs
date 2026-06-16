using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using MiniExcelLibs;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class ManageCourseSectionForm : Form
    {
        private readonly CourseSectionRepository _sectionRepository;
        private readonly My_DB _db = new My_DB();

        private bool _isEditMode = false;
        private bool _isProcessing = false;
        private bool _isInitialLoading = true;

        public ManageCourseSectionForm()
        {
            InitializeComponent();
            _sectionRepository = new CourseSectionRepository();

            // Đăng ký sự kiện
            cboMonHoc.SelectedIndexChanged += cboMonHoc_SelectedIndexChanged;
            dgvCourseSections.CellClick += dgvCourseSections_CellClick;
        }

        private async void ManageCourseSectionForm_Load(object sender, EventArgs e)
        {
            // 🌟 CHỐT CHẶN BẢO MẬT TẦNG 1
            if (!UserSession.IsLoggedIn || (!UserSession.IsAdmin && !UserSession.IsStaff && !UserSession.IsTeacher))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chức năng này chỉ dành cho Ban quản trị, phòng Giáo vụ hoặc Giảng viên.",
                                "Cảnh Báo An Ninh", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            // Phòng thủ độ dài nhập liệu
            txtMaLopHP.MaxLength = 30;
            txtNamHoc.MaxLength = 15;
            txtSearch.MaxLength = 100;

            // Phân quyền giao diện cho Giảng viên
            if (UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff)
            {
                btnInsert.Enabled = false;
                //btnClear.Enabled = false;
            }

            await ExecuteSecureOperationAsync(async () =>
            {
                _isInitialLoading = true;

                // Tải dữ liệu song song
                Task comboTask = LoadComboBoxDataAsync();
                Task sectionsTask = LoadCourseSectionsAsync();

                await Task.WhenAll(comboTask, sectionsTask);

                _isInitialLoading = false;
                TriggerHocKyAutoSelection();
            }, "Khởi tạo dữ liệu hệ thống lớp học phần");
        }

        #region Các Hàm Tải Dữ Liệu Bất Đồng Bộ

        private async Task LoadComboBoxDataAsync()
        {
            if (cboHocKy.Items.Count > 0) cboHocKy.SelectedIndex = 0;

            // 1. Tải dữ liệu Môn học trên luồng nền
            DataTable dtMonHoc = await Task.Run(() =>
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = "SELECT MaMH, TenMH, Hky FROM dbo.Course ORDER BY TenMH ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                return dt;
            });

            // 2. Tải dữ liệu Giảng viên trên luồng nền
            DataTable dtGiangVien = await Task.Run(() =>
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = "SELECT MSGV, (LastName + ' ' + FirstName) AS TenGV FROM dbo.Teachers ORDER BY FirstName ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                return dt;
            });

            // 3. Cập nhật lên UI (Bắt buộc thực hiện trên UI Thread)
            cboMonHoc.DataSource = dtMonHoc;
            cboMonHoc.DisplayMember = "TenMH";
            cboMonHoc.ValueMember = "MaMH";

            cboGiangVien.DataSource = dtGiangVien;
            cboGiangVien.DisplayMember = "TenGV";
            cboGiangVien.ValueMember = "MSGV";
        }

        private void cboMonHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialLoading) return;
            TriggerHocKyAutoSelection();
        }

        private void TriggerHocKyAutoSelection()
        {
            if (cboMonHoc.SelectedValue != null && cboMonHoc.SelectedItem is DataRowView rowView)
            {
                if (rowView.Row.Table.Columns.Contains("Hky"))
                {
                    string hocKyMacDinh = rowView.Row["Hky"]?.ToString() ?? "1";

                    if (!_isEditMode)
                    {
                        if (hocKyMacDinh.Contains("1")) cboHocKy.SelectedIndex = 0;
                        else if (hocKyMacDinh.Contains("2")) cboHocKy.SelectedIndex = 1;
                        else if (hocKyMacDinh.Contains("3")) cboHocKy.SelectedIndex = 2;

                        cboHocKy.Enabled = false;
                    }
                    else
                    {
                        cboHocKy.Enabled = true;
                    }
                }
            }
        }

        private async Task LoadCourseSectionsAsync(string searchKeyword = "")
        {
            // Lấy dữ liệu từ Repository
            DataTable dt = await Task.Run(() => _sectionRepository.GetCourseSections());

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                // Sử dụng DataView an toàn, xử lý triệt để dấu nháy đơn chống lỗi cú pháp
                string safeKeyword = searchKeyword.Replace("'", "''");
                DataView dv = dt.DefaultView;
                dv.RowFilter = $"MaLopHP LIKE '%{safeKeyword}%' OR TenMH LIKE '%{safeKeyword}%' OR MSGV LIKE '%{safeKeyword}%' OR TenGiangVien LIKE '%{safeKeyword}%'";
                dt = dv.ToTable();
            }

            dgvCourseSections.DataSource = dt;
            ConfigureDataGridViewFormat();
            lblTotalSections.Text = $"Tổng số học phần đã mở: {dgvCourseSections.Rows.Count}";
        }

        private void ConfigureDataGridViewFormat()
        {
            if (dgvCourseSections.Columns.Count == 0) return;

            string[] hiddenColumns = { "MaMH", "PhongHoc", "Status" };
            foreach (var col in hiddenColumns)
            {
                if (dgvCourseSections.Columns.Contains(col)) dgvCourseSections.Columns[col].Visible = false;
            }

            // Đổi tên Header trực quan
            var headers = new (string Key, string Value)[]
            {
                ("MaLopHP", "Mã Lớp HP"), ("TenMH", "Tên Môn Học"), ("HocKy", "Học Kỳ"),
                ("NamHoc", "Năm Học"), ("MSGV", "Mã GV"), ("TenGiangVien", "Giảng Viên"),
                ("MaxStudents", "Sĩ Số Tối Đa"), ("SisoHienTai", "Đã Đăng Ký")
            };

            foreach (var header in headers)
            {
                if (dgvCourseSections.Columns.Contains(header.Key))
                    dgvCourseSections.Columns[header.Key].HeaderText = header.Value;
            }

            // Phân quyền hiển thị cột chức năng trên lưới
            if (UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff)
            {
                if (dgvCourseSections.Columns.Contains("btnEditColumn")) dgvCourseSections.Columns["btnEditColumn"].Visible = false;
                if (dgvCourseSections.Columns.Contains("btnDeleteColumn")) dgvCourseSections.Columns["btnDeleteColumn"].Visible = false;
            }
            else
            {
                if (dgvCourseSections.Columns.Contains("btnEditColumn"))
                    dgvCourseSections.Columns["btnEditColumn"].DisplayIndex = dgvCourseSections.Columns.Count - 2;
                if (dgvCourseSections.Columns.Contains("btnDeleteColumn"))
                    dgvCourseSections.Columns["btnDeleteColumn"].DisplayIndex = dgvCourseSections.Columns.Count - 1;
            }
        }

        #endregion

        #region Xử Lý Nghiệp Vụ CRUD 

        private async void btnInsert_Click(object sender, EventArgs e)
        {
            if (UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff)
            {
                MessageBox.Show("Quyền hạn bị từ chối! Vai trò Giảng viên không thể thực hiện Mở/Cập nhật lớp.",
                                "Từ Chối Thao Tác", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaLopHP.Text) || string.IsNullOrWhiteSpace(txtNamHoc.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ Mã lớp học phần và Năm học hiện hành!", "Dữ liệu trống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Môn học hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int hockyInt = 1;
            string selectedHocKy = cboHocKy.SelectedItem?.ToString() ?? "";
            if (selectedHocKy.Contains("2")) hockyInt = 2;
            else if (selectedHocKy.Contains("3")) hockyInt = 3;

            CourseSection section = new CourseSection
            {
                MaLopHP = txtMaLopHP.Text.Trim().ToUpper(),
                MaMH = cboMonHoc.SelectedValue.ToString(),
                HocKy = hockyInt,
                NamHoc = txtNamHoc.Text.Trim(),
                MSGV = cboGiangVien.SelectedValue?.ToString() ?? "",
                PhongHoc = "Phòng Lý Thuyết",
                MaxStudents = (int)numMaxStudents.Value,
                Status = 1
            };

            await ExecuteSecureOperationAsync(async () =>
            {
                bool success;
                if (!_isEditMode)
                {
                    success = await Task.Run(() => _sectionRepository.AddSection(section));
                    if (success) MessageBox.Show("Mở lớp học phần mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    success = await Task.Run(() => _sectionRepository.UpdateSection(section));
                    if (success) MessageBox.Show("Cập nhật thông tin lớp học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadCourseSectionsAsync();
                ResetInputFields();
            }, "Xử lý Lưu / Cập nhật lớp học phần");
        }

        private async void dgvCourseSections_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvCourseSections.Columns[e.ColumnIndex].Name;
            string maLopHP = dgvCourseSections.Rows[e.RowIndex].Cells["MaLopHP"].Value?.ToString() ?? "";

            if (columnName == "btnEditColumn")
            {
                if (UserSession.IsTeacher && !UserSession.IsAdmin && !UserSession.IsStaff)
                {
                    MessageBox.Show("Quyền hạn bị từ chối!", "Từ Chối Thao Tác", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                _isEditMode = true;
                txtMaLopHP.Text = maLopHP;
                txtMaLopHP.ReadOnly = true;
                txtMaLopHP.FillColor = Color.LightGray;

                if (dgvCourseSections.Columns.Contains("MaMH"))
                    cboMonHoc.SelectedValue = dgvCourseSections.Rows[e.RowIndex].Cells["MaMH"].Value;

                cboHocKy.Enabled = true;
                string hk = dgvCourseSections.Rows[e.RowIndex].Cells["HocKy"].Value?.ToString() ?? "1";
                cboHocKy.SelectedIndex = hk == "3" ? 2 : (hk == "2" ? 1 : 0);

                txtNamHoc.Text = dgvCourseSections.Rows[e.RowIndex].Cells["NamHoc"].Value?.ToString() ?? "";

                if (dgvCourseSections.Columns.Contains("MSGV"))
                    cboGiangVien.SelectedValue = dgvCourseSections.Rows[e.RowIndex].Cells["MSGV"].Value?.ToString();

                if (dgvCourseSections.Columns.Contains("MaxStudents"))
                    numMaxStudents.Value = Convert.ToDecimal(dgvCourseSections.Rows[e.RowIndex].Cells["MaxStudents"].Value);

                btnInsert.Text = "💾 Cập nhật";
                btnInsert.FillColor = Color.FromArgb(230, 126, 34);
            }
            else if (columnName == "btnDeleteColumn")
            {
                if (!UserSession.IsAdmin)
                {
                    MessageBox.Show("Chỉ có Admin cấp cao mới có quyền hủy hoặc xóa lớp học phần.", "Từ Chối Thao Tác", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa lớp [{maLopHP}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    await ExecuteSecureOperationAsync(async () =>
                    {
                        bool result = await Task.Run(() => _sectionRepository.DeleteSection(maLopHP));
                        if (result)
                        {
                            MessageBox.Show("Xóa lớp học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (_isEditMode && txtMaLopHP.Text == maLopHP) ResetInputFields();
                            await LoadCourseSectionsAsync();
                        }
                    }, "Xử lý xóa lớp học phần");
                }
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            await ExecuteSecureOperationAsync(async () =>
            {
                await LoadCourseSectionsAsync(txtSearch.Text.Trim());
            }, "Tìm kiếm thông tin học phần");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetInputFields();
        }

        private async void ResetInputFields()
        {
            _isEditMode = false;
            txtMaLopHP.Text = string.Empty;
            txtMaLopHP.ReadOnly = false;
            txtMaLopHP.FillColor = Color.White;
            txtNamHoc.Text = string.Empty;
            numMaxStudents.Value = 50;

            if (cboMonHoc.Items.Count > 0) cboMonHoc.SelectedIndex = 0;
            if (cboGiangVien.Items.Count > 0) cboGiangVien.SelectedIndex = 0;

            TriggerHocKyAutoSelection();
            txtSearch.Text = string.Empty;

            if (!UserSession.IsTeacher || UserSession.IsAdmin || UserSession.IsStaff)
            {
                btnInsert.Text = "🚀 Mở lớp";
                btnInsert.FillColor = Color.FromArgb(26, 115, 232);
            }

            // Gọi tải lại dữ liệu an toàn bọc trong cơ chế quản lý luồng
            await ExecuteSecureOperationAsync(async () => {
                await LoadCourseSectionsAsync();
            }, "Tải lại danh sách sau khi reset");
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            // Kiểm tra an toàn trước khi ép kiểu
            if (dgvCourseSections.DataSource == null || dgvCourseSections.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = $"DanhSach_LopHocPhan_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Sửa lỗi cast: Chuyển đổi an toàn bất kể DataSource là DataTable hay DataView
                        DataTable dtExport;
                        if (dgvCourseSections.DataSource is DataView dv)
                        {
                            dtExport = dv.ToTable().Copy();
                        }
                        else
                        {
                            dtExport = ((DataTable)dgvCourseSections.DataSource).Copy();
                        }

                        // Loại bỏ cột thừa
                        string[] redundantCols = { "MaMH", "PhongHoc", "Status" };
                        foreach (var col in redundantCols)
                        {
                            if (dtExport.Columns.Contains(col)) dtExport.Columns.Remove(col);
                        }

                        // Việt hóa cột Excel hiển thị sạch đẹp
                        dtExport.Columns["MaLopHP"].ColumnName = "Mã Lớp Học Phần";
                        dtExport.Columns["TenMH"].ColumnName = "Tên Môn Học";
                        dtExport.Columns["HocKy"].ColumnName = "Học Kỳ";
                        dtExport.Columns["NamHoc"].ColumnName = "Năm Học";
                        if (dtExport.Columns.Contains("MSGV")) dtExport.Columns["MSGV"].ColumnName = "Mã Số Giảng Viên";
                        if (dtExport.Columns.Contains("TenGiangVien")) dtExport.Columns["TenGiangVien"].ColumnName = "Tên Giảng Viên";
                        dtExport.Columns["MaxStudents"].ColumnName = "Sĩ Số Tối Đa";
                        dtExport.Columns["SisoHienTai"].ColumnName = "Số SV Đã Đăng Ký";

                        MiniExcel.SaveAs(sfd.FileName, dtExport);
                        MessageBox.Show("Xuất file Excel thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xuất dữ liệu Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Khung Bảo Vệ Đa Luồng Toàn Vẹn Dữ Liệu

        private async Task ExecuteSecureOperationAsync(Func<Task> businessLogic, string operationName)
        {
            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                this.UseWaitCursor = true;

                await businessLogic();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"[Lỗi SQL Server] Thao tác '{operationName}' thất bại.\nChi tiết: ({sqlEx.Number}) - {sqlEx.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException invEx)
            {
                MessageBox.Show(invEx.Message, "Cảnh báo Nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Lỗi Hệ Thống] Có lỗi xảy ra trong tiến trình: '{operationName}'.\nChi tiết: {ex.Message}", "Lỗi Thực Thi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.UseWaitCursor = false;
                _isProcessing = false;
            }
        }

        #endregion
    }
}