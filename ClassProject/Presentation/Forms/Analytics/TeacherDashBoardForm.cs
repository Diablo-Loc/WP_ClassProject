using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;

namespace ClassProject.Presentation.Forms.Analytics
{
    public partial class TeacherDashBoardForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly DashboardRepository _dashboardRepo;
        private Chart chartStudentsPerSection;
        private Chart chartPassFailRate;
        private string _teacherID;

        public TeacherDashBoardForm(string teacherID)
        {
            InitializeComponent();

            // 1. CẦN GÁN BIẾN NÀY ĐẦU TIÊN để bảo toàn dữ liệu truyền vào
            _teacherID = teacherID;

            // Nếu trong UserSession chưa có mã này, ta đồng bộ ngược lại để an toàn hệ thống
            if (UserSession.IsTeacher && string.IsNullOrEmpty(UserSession.TeacherId))
            {
                UserSession.UpdateTeacherId(teacherID);
            }

            // Khởi tạo repository quản lý dữ liệu Dashboard
            _dashboardRepo = new DashboardRepository(_db.GetConnection().ConnectionString);

            // Tự động nạp biểu đồ động vào các vùng Container của Guna Panel
            InitializeDynamicCharts();
            dgvAtRiskStudents.MouseEnter += (s, e) => { dgvAtRiskStudents.Focus(); };
            this.Shown += TeacherDashBoardForm_Shown;
        }

        private void InitializeDynamicCharts()
        {
            chartStudentsPerSection = new Chart { Dock = DockStyle.Fill };
            if (pnlLeftChartContainer != null)
                pnlLeftChartContainer.Controls.Add(chartStudentsPerSection);

            chartPassFailRate = new Chart { Dock = DockStyle.Fill };
            if (pnlRightChartContainer != null)
                pnlRightChartContainer.Controls.Add(chartPassFailRate);
        }

        private async void TeacherDashBoardForm_Shown(object sender, EventArgs e)
        {
            // Kiểm tra bảo mật ngay khi form hiển thị công khai
            if (!UserSession.IsTeacher)
            {
                MessageBox.Show("Chức năng này được bảo mật và chỉ dành riêng cho tài khoản Giảng viên!",
                                "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }

            // KIỂM TRA PHÒNG NGỪA: Nếu cả biến cục bộ và Session đều trống
            if (string.IsNullOrEmpty(_teacherID))
            {
                MessageBox.Show("Lỗi hệ thống: Mã giảng viên không được phép để trống!",
                                "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                SetupChartStyles();

                // ĐÃ SỬA: Truyền chính xác biến _teacherID của Form thay vì gọi UserSession ngẫu nhiên
                DataSet dashboardData = await _dashboardRepo.GetTeacherDashboardDataSetAsync(_teacherID);

                this.SuspendLayout();

                if (dashboardData != null && dashboardData.Tables.Count >= 4)
                {
                    // Table 0: Thông tin bộ đếm các thẻ số (Summary Cards)
                    if (dashboardData.Tables[0].Rows.Count > 0)
                        BindSummaryCards(dashboardData.Tables[0].Rows[0]);

                    // Table 1: Biểu đồ số lượng sinh viên theo từng lớp học phần phụ trách
                    BindBarChart(dashboardData.Tables[1]);

                    // Table 2: Biểu đồ hình bánh biểu diễn tỷ lệ Đạt/Trượt môn
                    BindPieChart(dashboardData.Tables[2]);

                    // Table 3: Danh sách sinh viên có điểm quá trình thấp cần cảnh báo
                    BindAtRiskGrid(dashboardData.Tables[3]);
                }

                // Hiển thị thông tin định danh cá nhân lên Guna Header
                string tenGiangVien = !string.IsNullOrEmpty(UserSession.FullName) ? UserSession.FullName : "Giảng viên";
                lblWelcomeTeacher.Text = $"Xin chào Thầy/Cô: {tenGiangVien} (MSGV: {_teacherID})";

                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi tải thông tin Dashboard: {ex.Message}",
                                "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void SetupChartStyles()
        {
            // Cấu hình style tối giản hiện đại cho biểu đồ cột
            chartStudentsPerSection.ChartAreas.Clear();
            ChartArea areaBar = new ChartArea("MainArea") { BackColor = Color.White };
            areaBar.AxisX.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
            areaBar.AxisY.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
            areaBar.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            areaBar.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            chartStudentsPerSection.ChartAreas.Add(areaBar);

            // Cấu hình style cho biểu đồ tròn tỷ lệ kết quả học tập
            chartPassFailRate.ChartAreas.Clear();
            ChartArea areaPie = new ChartArea("MainArea") { BackColor = Color.White };
            chartPassFailRate.ChartAreas.Add(areaPie);

            chartPassFailRate.Legends.Clear();
            Legend mainLegend = new Legend("MainLegend")
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                Font = new Font("Segoe UI", 8.5f),
                BackColor = Color.Transparent
            };
            chartPassFailRate.Legends.Add(mainLegend);
        }

        private void BindSummaryCards(DataRow row)
        {
            if (row == null) return;

            lblTotalClassesVal.Text = (row["TotalClasses"] != DBNull.Value ? row["TotalClasses"].ToString() : "0") + " Lớp";
            lblTotalStudentsVal.Text = (row["TotalStudents"] != DBNull.Value ? Convert.ToInt32(row["TotalStudents"]).ToString("N0") : "0") + " SV";
            lblPassRateVal.Text = (row["AvgScore"] != DBNull.Value ? Convert.ToDouble(row["AvgScore"]).ToString("F1") : "0.0") + " Đ";
            lblPendingGradesVal.Text = (row["PendingScores"] != DBNull.Value ? row["PendingScores"].ToString() : "0") + " Đầu điểm";
        }

        private void BindBarChart(DataTable dt)
        {
            if (dt == null) return;

            chartStudentsPerSection.Series.Clear();
            Series series = new Series("Sinh Viên")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(59, 130, 246),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            chartStudentsPerSection.Series.Add(series);

            foreach (DataRow row in dt.Rows)
            {
                series.Points.AddXY(row["MaLopHP"].ToString(), Convert.ToInt32(row["StudentCount"]));
            }

            chartStudentsPerSection.Series["Sinh Viên"]["PixelPointWidth"] = "25";
        }

        private void BindPieChart(DataTable dt)
        {
            if (dt == null) return;

            chartPassFailRate.Series.Clear();
            Series series = new Series("PassRate")
            {
                ChartType = SeriesChartType.Pie
            };
            series["PieLabelStyle"] = "Inside";
            chartPassFailRate.Series.Add(series);

            foreach (DataRow row in dt.Rows)
            {
                string status = row["StatusGroup"].ToString();
                int qty = Convert.ToInt32(row["Quantity"]);

                DataPoint p = new DataPoint();
                p.SetValueY(qty);
                p.AxisLabel = status;
                p.LegendText = $"{status} ({qty} SV)";

                if (status.Contains("Đạt"))
                    p.Color = Color.FromArgb(16, 185, 129);
                else if (status.Contains("Trượt"))
                    p.Color = Color.FromArgb(239, 68, 68);
                else
                    p.Color = Color.FromArgb(148, 163, 184);

                series.Points.Add(p);
            }
            chartPassFailRate.Series["PassRate"].Label = "#PERCENT{P0}";
            chartPassFailRate.Series["PassRate"].Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            chartPassFailRate.Series["PassRate"].LabelForeColor = Color.White;
        }

        private void BindAtRiskGrid(DataTable dt)
        {
            if (dgvAtRiskStudents == null || dt == null) return;
            dgvAtRiskStudents.DataSource = dt;
            dgvAtRiskStudents.ScrollBars = ScrollBars.Both;
            dgvAtRiskStudents.AllowUserToResizeRows = true;
            dgvAtRiskStudents.Enabled = true;
            if (dgvAtRiskStudents.Columns["MSSV"] != null) dgvAtRiskStudents.Columns["MSSV"].HeaderText = "Mã Số SV";
            if (dgvAtRiskStudents.Columns["FullName"] != null) dgvAtRiskStudents.Columns["FullName"].HeaderText = "Họ và Tên Sinh Viên";
            if (dgvAtRiskStudents.Columns["MaLopHP"] != null) dgvAtRiskStudents.Columns["MaLopHP"].HeaderText = "Mã Lớp Học Phần";
            if (dgvAtRiskStudents.Columns["DiemQT"] != null) dgvAtRiskStudents.Columns["DiemQT"].HeaderText = "Điểm Quá Trình";
            if (dgvAtRiskStudents.Columns["WarningReason"] != null) dgvAtRiskStudents.Columns["WarningReason"].HeaderText = "Lý Do Cảnh Báo";
            dgvAtRiskStudents.Refresh();
        }
    }
}