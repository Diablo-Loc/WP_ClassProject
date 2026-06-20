using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using MiniExcelLibs;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class MyCoursesForm : Form
    {
        private readonly RegisterRepository _registerRepo;
        private bool _isProcessing = false;

        /// <summary>
        /// Constructor mặc định không tham số - Đạt chuẩn đóng gói và giảm phụ thuộc (Decoupling)
        /// </summary>
        public MyCoursesForm()
        {
            InitializeComponent();
            _registerRepo = new RegisterRepository();
        }

        /// <summary>
        /// Sự kiện nạp Form - Tự động đồng bộ hóa tiêu đề và nạp dữ liệu phi đồng bộ dựa vào UserSession toàn cục
        /// </summary>
        private async void MyCoursesForm_Load(object sender, EventArgs e)
        {
            // Kiểm tra an toàn tính danh tính từ Session toàn cục
            if (string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Không thể xác định mã số sinh viên hợp lệ cho phiên làm việc này!\nVui lòng đăng nhập lại.",
                                "Lỗi Xác Thực Phiên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new Action(this.Close));
                return;
            }

            lblTitle.Text = $"THỜI KHÓA BIỂU CÁ NHÂN — SV: {UserSession.MSSV.ToUpper()}";
            await LoadMyScheduleAsync();
        }

        /// <summary>
        /// Core Engine: Tải dữ liệu từ SQL Server lên GridView qua luồng phụ để chống đơ UI (Non-blocking UI)
        /// </summary>
        private async Task LoadMyScheduleAsync()
        {
            if (_isProcessing) return; // Chống spam click hoặc nạp chồng (Re-entrancy Guard)

            try
            {
                _isProcessing = true;
                this.UseWaitCursor = true;
                dgvMyCourses.DataSource = null; // Reset trạng thái lưới dữ liệu

                // Truy vấn dữ liệu bất đồng bộ dựa trên MSSV của UserSession hiện tại
                DataTable dt = await Task.Run(() => _registerRepo.GetRegistrationList(UserSession.MSSV));

                if (dt == null)
                {
                    lblTotal.Text = "📊 Không tìm thấy dữ liệu đăng ký học phần.";
                    return;
                }

                // Gán dữ liệu lên UI Thread an toàn
                dgvMyCourses.DataSource = dt;

                // UX/UI: Chuẩn hóa toàn bộ tiêu đề cột và tỷ lệ hiển thị
                ConfigureDataGridViewLayout();

                lblTotal.Text = $"📊 Tổng số lớp học phần đã đăng ký thành công: {dt.Rows.Count} môn.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Lỗi Hệ Thống] Không thể kết nối cơ sở dữ liệu thời khóa biểu:\n{ex.Message}",
                                "Lỗi Vận Hành", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"Error loading schedule for {UserSession.MSSV}: {ex}");
            }
            finally
            {
                this.UseWaitCursor = false;
                _isProcessing = false;
            }
        }

        /// <summary>
        /// Hàm cấu hình giao diện GridView tùy biến sâu (Độc lập, dễ bảo trì)
        /// </summary>
        private void ConfigureDataGridViewLayout()
        {
            if (dgvMyCourses.Columns.Count == 0) return;

            if (dgvMyCourses.Columns.Contains("STT"))
            {
                dgvMyCourses.Columns["STT"].Width = 60;
                dgvMyCourses.Columns["STT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMyCourses.Columns.Contains("MaLopHP")) dgvMyCourses.Columns["MaLopHP"].HeaderText = "Mã Lớp Học Phần";
            if (dgvMyCourses.Columns.Contains("TenMH")) dgvMyCourses.Columns["TenMH"].HeaderText = "Tên Môn Học";
            if (dgvMyCourses.Columns.Contains("SoTC"))
            {
                dgvMyCourses.Columns["SoTC"].HeaderText = "Số Tín Chỉ";
                dgvMyCourses.Columns["SoTC"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dgvMyCourses.Columns.Contains("GiangVien")) dgvMyCourses.Columns["GiangVien"].HeaderText = "Giảng Viên Đứng Lớp";
            if (dgvMyCourses.Columns.Contains("PhongHoc")) dgvMyCourses.Columns["PhongHoc"].HeaderText = "Phòng Học";

            if (dgvMyCourses.Columns.Contains("RegistrationDate"))
            {
                dgvMyCourses.Columns["RegistrationDate"].HeaderText = "Ngày Đăng Ký";
                dgvMyCourses.Columns["RegistrationDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }

            dgvMyCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Nghiệp vụ Doanh nghiệp: Xuất file báo cáo sạch bằng MiniExcel (Giải phóng RAM ngay lập tức)
        /// </summary>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvMyCourses.DataSource is not DataTable currentTable || currentTable.Rows.Count == 0)
            {
                MessageBox.Show("Hiện tại hệ thống không ghi nhận lịch học nào để kết xuất báo cáo Excel!",
                                "Thông báo học vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = $"ThoiKhoaBieu_{UserSession.MSSV}_{DateTime.Now:yyyyMMdd}.xlsx";
                sfd.Title = "Chọn nơi lưu Biên nhận Thời khóa biểu";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Sử dụng khối lệnh 'using' để hủy vùng nhớ bản sao ngay sau khi xuất, tránh rò rỉ RAM (Memory Leak)
                    using (DataTable exportDt = currentTable.Copy())
                    {
                        try
                        {
                            // Đổi tên cột trực quan cho tệp Excel báo cáo đầu ra đạt chuẩn ISO
                            if (exportDt.Columns.Contains("STT")) exportDt.Columns["STT"].ColumnName = "STT";
                            if (exportDt.Columns.Contains("MaLopHP")) exportDt.Columns["MaLopHP"].ColumnName = "Mã Lớp Học Phần";
                            if (exportDt.Columns.Contains("TenMH")) exportDt.Columns["TenMH"].ColumnName = "Tên Môn Học";
                            if (exportDt.Columns.Contains("SoTC")) exportDt.Columns["SoTC"].ColumnName = "Số Tín Chỉ";
                            if (exportDt.Columns.Contains("GiangVien")) exportDt.Columns["GiangVien"].ColumnName = "Giảng Viên Đứng Lớp";
                            if (exportDt.Columns.Contains("PhongHoc")) exportDt.Columns["PhongHoc"].ColumnName = "Phòng Học";
                            if (exportDt.Columns.Contains("RegistrationDate")) exportDt.Columns["RegistrationDate"].ColumnName = "Ngày Đăng Ký";

                            // Ghi trực tiếp ra luồng lưu tệp tin của MiniExcel
                            MiniExcel.SaveAs(sfd.FileName, exportDt);

                            MessageBox.Show("Dữ liệu Thời khóa biểu điện tử đã được kết xuất thành công sang định dạng Excel!",
                                            "Xuất Báo Báo Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hệ thống từ chối ghi tệp tin do có ứng dụng Excel khác đang mở file này trùng tên!\nChi tiết: {ex.Message}",
                                            "Lỗi Xuất File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void Btn_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb(4, 120, 87); // Màu xanh Emerald đậm hơn khi di chuột qua
            }
        }

        private void Btn_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackColor = Color.FromArgb(5, 150, 105); // Trả lại màu xanh Emerald gốc của doanh nghiệp
            }
        }
    }
}