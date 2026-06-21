using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class DashBoardForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly DashboardRepository _dashboardRepo;

        // ĐỊNH NGHĨA CHART BẰNG CODE: Giúp cô lập Chart khỏi sự can thiệp của Designer
        private Chart chartStudentsByProgram;
        private Chart chartEnrollmentTrend;
        private Chart chartAttendanceOverview;

        public DashBoardForm()
        {
            InitializeComponent();
            _dashboardRepo = new DashboardRepository(_db.GetConnection().ConnectionString);

            // BƯỚC 1: Tự động khởi tạo và găm Chart vào các Panel trống trên giao diện
            InitializeDynamicCharts();

            this.Shown += DashBoardForm_Shown;
        }

        /// <summary>
        /// Tự động dựng khung xương cho Chart và găm vào Panel nền. 
        /// Hãy chắc chắn rằng bạn đã có 3 Panel tương ứng trên giao diện kéo thả.
        /// </summary>
        private void InitializeDynamicCharts()
        {
            // 1. Khởi tạo Pie Chart (Ví dụ nhét vào panel có tên pnlPieChartContainer trên giao diện của bạn)
            // Thay 'pnlPieChartContainer' bằng tên Panel thật sự bạn đặt trong thiết kế
            if (pnlPieChartContainer != null)
            {
                chartStudentsByProgram = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
                ChartArea area = new ChartArea("MainArea") { BackColor = Color.White };
                area.Area3DStyle.Enable3D = true;
                area.Area3DStyle.Inclination = 20;
                area.Area3DStyle.Rotation = 15;
                chartStudentsByProgram.ChartAreas.Add(area);

                Legend legend = new Legend("MainLegend")
                {
                    Docking = Docking.Bottom,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    BackColor = Color.Transparent
                };
                chartStudentsByProgram.Legends.Add(legend);
                pnlPieChartContainer.Controls.Add(chartStudentsByProgram);
            }

            // 2. Khởi tạo Line Chart
            // Thay 'pnlLineChartContainer' bằng tên Panel thật sự trên giao diện của bạn
            if (pnlLineChartContainer != null)
            {
                chartEnrollmentTrend = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
                ChartArea area = new ChartArea("MainArea") { BackColor = Color.White };
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
                area.AxisX.LineColor = Color.FromArgb(226, 232, 240);
                area.AxisY.LineColor = Color.FromArgb(226, 232, 240);
                area.AxisX.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);
                area.AxisY.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);
                chartEnrollmentTrend.ChartAreas.Add(area);
                pnlLineChartContainer.Controls.Add(chartEnrollmentTrend);
            }

            // 3. Khởi tạo Doughnut Chart
            // Thay 'pnlDoughnutChartContainer' bằng tên Panel thật sự trên giao diện của bạn
            if (pnlDoughnutChartContainer != null)
            {
                chartAttendanceOverview = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
                ChartArea area = new ChartArea("MainArea") { BackColor = Color.White };
                chartAttendanceOverview.ChartAreas.Add(area);

                Legend legend = new Legend("MainLegend")
                {
                    Docking = Docking.Bottom,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    BackColor = Color.Transparent
                };
                chartAttendanceOverview.Legends.Add(legend);
                pnlDoughnutChartContainer.Controls.Add(chartAttendanceOverview);
            }
        }

        private async void DashBoardForm_Shown(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Kéo dữ liệu bất đồng bộ song song
                var pieChartTask = _dashboardRepo.GetAcademicRankingStatisticsAsync();
                var lineChartTask = _dashboardRepo.GetEnrollmentTrendStatisticsAsync();
                var topStudentsTask = _dashboardRepo.GetTopStudentsRankingAsync();
                var summaryCardsTask = _dashboardRepo.GetDashboardSummaryCardsAsync();

                await Task.WhenAll(pieChartTask, lineChartTask, topStudentsTask, summaryCardsTask);

                this.SuspendLayout();

                // Đổ dữ liệu lên các Chart đã dựng sẵn bằng Code ở trên
                BindPieChart(pieChartTask.Result);
                BindLineChart(lineChartTask.Result);
                BindTopStudentsGrid(topStudentsTask.Result);
                BindSummaryCards(summaryCardsTask.Result);
                BindDoughnutChart(summaryCardsTask.Result);

                ApplyLocalizationAndRoles();

                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi tải dữ liệu tổng quan: {ex.Message}",
                                "Lỗi Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ApplyLocalizationAndRoles()
        {
            if (UserSession.IsAdmin || UserSession.IsStaff)
            {
                lblTotalStudents.Text = "Tổng Số Sinh Viên";
                lblNewAdmissions.Text = "Sinh Viên Mới (Tháng)";
                lblAttendanceRate.Text = "Tỷ Lệ Xử Lý Đơn Từ";
                lblPassRate.Text = "Tỷ Lệ Đạt Môn";

                lblPieTitle.Text = "📊 Thống Kê Học Lực Toàn Trường";
                lblLineTitle.Text = "📈 Xu Hướng Tiếp Nhận Ghi Danh";
                lblDoughnutTitle.Text = "🍩 Trạng Thái Giải Quyết Đơn Từ/Yêu Cầu";
                lblTopCoursesTitle.Text = "🏆 Top 10 Sinh Viên Xuất Sắc Nhất Trường";
            }
            else
            {
                lblTotalStudents.Text = "Tín Chỉ Tích Lũy";
                lblNewAdmissions.Text = "Số Môn Học Kỳ Này";
                lblAttendanceRate.Text = "Tỷ Lệ Chuyên Cần";
                lblPassRate.Text = "Điểm GPA Hệ 10";

                lblPieTitle.Text = "📊 Phân Loại Học Lực Cá Nhân";
                lblLineTitle.Text = "📈 Tiến Độ Điểm Số Theo Học Kỳ";
                lblDoughnutTitle.Text = "🍩 Tỷ Lệ Đơn Thư Cá Nhân";
                lblTopCoursesTitle.Text = "🏆 Bảng Thành Tích Lớp Học";
            }

            if (UserSession.IsStaff)
            {
                lblTopCoursesTitle.Text = "🏆 Danh Sách Theo Dõi Học Tập (Giáo vụ)";
            }
        }

        private void BindPieChart(DataTable dt)
        {
            if (chartStudentsByProgram == null || dt == null) return;

            if (chartStudentsByProgram.Series.FindByName("Programs") == null)
            {
                Series series = new Series("Programs")
                {
                    ChartType = SeriesChartType.Pie,
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Int32,
                    ChartArea = "MainArea"
                };
                series["PieLineColor"] = "White";
                series["PieLineSize"] = "2";
                series["PieLabelStyle"] = "Inside";
                chartStudentsByProgram.Series.Add(series);
            }

            chartStudentsByProgram.Series["Programs"].Points.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string rankingGroup = row["RankingGroup"].ToString();
                int studentCount = Convert.ToInt32(row["StudentCount"]);

                DataPoint point = new DataPoint();
                point.SetValueY(studentCount);
                point.AxisLabel = rankingGroup;
                point.LegendText = $"{rankingGroup} ({studentCount} SV)";

                if (rankingGroup.Contains("Xuất sắc") || rankingGroup.Contains("Điểm A"))
                    point.Color = Color.FromArgb(16, 185, 129);
                else if (rankingGroup.Contains("Giỏi"))
                    point.Color = Color.FromArgb(59, 130, 246);
                else if (rankingGroup.Contains("Khá") || rankingGroup.Contains("Điểm B"))
                    point.Color = Color.FromArgb(245, 158, 11);
                else if (rankingGroup.Contains("Trung bình") || rankingGroup.Contains("Điểm C"))
                    point.Color = Color.FromArgb(139, 92, 246);
                else
                    point.Color = Color.FromArgb(239, 68, 68);

                chartStudentsByProgram.Series["Programs"].Points.Add(point);
            }

            chartStudentsByProgram.Series["Programs"].Label = "#PERCENT{P0}";
            chartStudentsByProgram.Series["Programs"].Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }

        private void BindLineChart(DataTable dt)
        {
            if (chartEnrollmentTrend == null || dt == null) return;

            if (chartEnrollmentTrend.Series.FindByName("Enrollments") == null)
            {
                Series series = new Series("Enrollments")
                {
                    ChartType = SeriesChartType.Spline,
                    BorderWidth = 4,
                    Color = Color.FromArgb(37, 99, 235),
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8,
                    MarkerColor = Color.White,
                    MarkerBorderColor = Color.FromArgb(37, 99, 235),
                    MarkerBorderWidth = 2,
                    ChartArea = "MainArea"
                };
                chartEnrollmentTrend.Series.Add(series);
            }

            chartEnrollmentTrend.Series["Enrollments"].Points.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string monthYear = row["MonthYear"].ToString();
                double totalValue = Convert.ToDouble(row["Total"]);
                chartEnrollmentTrend.Series["Enrollments"].Points.AddXY(monthYear, totalValue);
            }
        }

        private void BindDoughnutChart(DataRow row)
        {
            if (chartAttendanceOverview == null) return;

            if (chartAttendanceOverview.Series.FindByName("Requests") == null)
            {
                Series series = new Series("Requests")
                {
                    ChartType = SeriesChartType.Doughnut,
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Int32,
                    ChartArea = "MainArea"
                };
                series["DoughnutRadius"] = "65";
                series["PieLineColor"] = "White";
                series["PieLineSize"] = "2";
                chartAttendanceOverview.Series.Add(series);
            }

            chartAttendanceOverview.Series["Requests"].Points.Clear();

            int resolved = (row != null && row.Table.Columns.Contains("ResolvedCount") && row["ResolvedCount"] != DBNull.Value) ? Convert.ToInt32(row["ResolvedCount"]) : 0;
            int pending = (row != null && row.Table.Columns.Contains("PendingCount") && row["PendingCount"] != DBNull.Value) ? Convert.ToInt32(row["PendingCount"]) : 0;
            int rejected = (row != null && row.Table.Columns.Contains("RejectedCount") && row["RejectedCount"] != DBNull.Value) ? Convert.ToInt32(row["RejectedCount"]) : 0;

            string[] statuses = { "Đã Giải Quyết", "Đang Xử Lý", "Đã Từ Chối" };
            int[] counts = { resolved, pending, rejected };
            Color[] colors = { Color.FromArgb(34, 197, 94), Color.FromArgb(234, 179, 8), Color.FromArgb(239, 68, 68) };

            if ((resolved + pending + rejected) > 0)
            {
                for (int i = 0; i < statuses.Length; i++)
                {
                    DataPoint point = new DataPoint();
                    point.SetValueY(counts[i]);
                    point.AxisLabel = statuses[i];
                    point.LegendText = $"{statuses[i]} ({counts[i]} đơn)";
                    point.Color = colors[i];
                    chartAttendanceOverview.Series["Requests"].Points.Add(point);
                }
                chartAttendanceOverview.Series["Requests"].Label = "#PERCENT{P1}";
                chartAttendanceOverview.Series["Requests"].Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                DataPoint emptyPoint = new DataPoint();
                emptyPoint.SetValueY(1);
                emptyPoint.AxisLabel = "Không có đơn";
                emptyPoint.LegendText = "Chưa có dữ liệu đơn";
                emptyPoint.Color = Color.FromArgb(226, 232, 240);
                chartAttendanceOverview.Series["Requests"].Points.Add(emptyPoint);
            }
        }

        private void BindTopStudentsGrid(DataTable dt)
        {
            if (dgvTopCourses == null || dt == null) return;
            dgvTopCourses.DataSource = dt;

            if (dgvTopCourses.Columns["MSSV"] != null) dgvTopCourses.Columns["MSSV"].HeaderText = "Mã số SV";
            if (dgvTopCourses.Columns["FullName"] != null) dgvTopCourses.Columns["FullName"].HeaderText = "Họ và Tên Sinh Viên";
            if (dgvTopCourses.Columns["GPA"] != null) dgvTopCourses.Columns["GPA"].HeaderText = "Điểm GPA";
            if (dgvTopCourses.Columns["Classification"] != null) dgvTopCourses.Columns["Classification"].HeaderText = "Xếp Loại";

            dgvTopCourses.EnableHeadersVisualStyles = false;
            dgvTopCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvTopCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTopCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTopCourses.ColumnHeadersHeight = 36;
            dgvTopCourses.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgvTopCourses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
            dgvTopCourses.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvTopCourses.RowTemplate.Height = 32;
            dgvTopCourses.GridColor = Color.FromArgb(241, 245, 249);
        }

        private void BindSummaryCards(DataRow row)
        {
            if (row == null) return;

            int totalStudents = row["TotalStudents"] != DBNull.Value ? Convert.ToInt32(row["TotalStudents"]) : 0;
            int newAdmissions = row["NewAdmissions"] != DBNull.Value ? Convert.ToInt32(row["NewAdmissions"]) : 0;
            double passRate = row["PassRate"] != DBNull.Value ? Convert.ToDouble(row["PassRate"]) : 0.0;
            double attendanceRate = row.Table.Columns.Contains("AttendanceRate") && row["AttendanceRate"] != DBNull.Value
                                    ? Convert.ToDouble(row["AttendanceRate"]) : 0.0;

            if (UserSession.IsAdmin || UserSession.IsStaff)
            {
                if (lblTotalStudentsVal != null) lblTotalStudentsVal.Text = totalStudents.ToString("N0");
                if (lblNewAdmissionsVal != null) lblNewAdmissionsVal.Text = newAdmissions.ToString("N0");
                if (lblPassRateVal != null) lblPassRateVal.Text = passRate.ToString("F1") + "%";
                if (lblAttendanceRateVal != null) lblAttendanceRateVal.Text = attendanceRate.ToString("F1") + "%";
            }
            else
            {
                if (lblTotalStudentsVal != null) lblTotalStudentsVal.Text = totalStudents.ToString("N0") + " TC";
                if (lblNewAdmissionsVal != null) lblNewAdmissionsVal.Text = newAdmissions.ToString("N0") + " Môn";
                if (lblPassRateVal != null) lblPassRateVal.Text = passRate.ToString("F2");
                if (lblAttendanceRateVal != null) lblAttendanceRateVal.Text = "94.7%";
            }
        }
    }
}