using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class DashBoardForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private readonly DashboardRepository _dashboardRepo;

        public DashBoardForm()
        {
            InitializeComponent();
            // Khởi tạo Repository bằng Connection String chuẩn doanh nghiệp
            _dashboardRepo = new DashboardRepository(_db.GetConnection().ConnectionString);
        }

        private async void DashBoardForm_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Khởi tạo khung biểu đồ trước
                InitializeDynamicCharts();

                // Kỹ thuật đỉnh cao doanh nghiệp: Chạy song song 4 luồng bốc dữ liệu từ SQL Server về cùng 1 lúc
                var pieChartTask = _dashboardRepo.GetAcademicRankingStatisticsAsync();
                var lineChartTask = _dashboardRepo.GetEnrollmentTrendStatisticsAsync();
                var topStudentsTask = _dashboardRepo.GetTopStudentsRankingAsync();
                var summaryCardsTask = _dashboardRepo.GetDashboardSummaryCardsAsync();

                // Đợi cả 4 luồng hoàn thành an toàn
                await Task.WhenAll(pieChartTask, lineChartTask, topStudentsTask, summaryCardsTask);

                // Đổ dữ liệu lên giao diện (Xử lý an toàn trên UI Thread)
                BindPieChart(pieChartTask.Result);
                BindLineChart(lineChartTask.Result);
                BindTopStudentsGrid(topStudentsTask.Result);
                BindSummaryCards(summaryCardsTask.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi tải dữ liệu tổng quan: {ex.Message}", "Lỗi Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void InitializeDynamicCharts()
        {
            // 1. KHỞI TẠO & LÀM ĐẸP BIỂU ĐỒ TRÒN (PIE CHART)
            if (chartStudentsByProgram == null && pnlPieChart != null)
            {
                chartStudentsByProgram = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
                chartStudentsByProgram.Padding = new Padding(15, 50, 15, 15);

                ChartArea area = new ChartArea("MainArea")
                {
                    BackColor = Color.White,
                    BackSecondaryColor = Color.White
                };
                // Kích hoạt hiệu ứng 3D nhẹ nhìn rất nổi khối và sang trọng
                area.Area3DStyle.Enable3D = true;
                area.Area3DStyle.Inclination = 40; // Góc nghiêng đổ bóng
                area.Area3DStyle.Rotation = 20;    // Góc xoay biểu đồ
                chartStudentsByProgram.ChartAreas.Add(area);

                Series series = new Series("Programs")
                {
                    ChartType = SeriesChartType.Pie,
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Int32
                };

                // Custom thuộc tính Pie chuẩn Web: Thêm nét cắt viền trắng giữa các miếng bánh
                series["PieLineColor"] = "White";
                series["PieLineSize"] = "2";
                series["PieLabelStyle"] = "Inside"; // Đẩy chữ vào trong nếu vừa, tránh bị tràn viền
                chartStudentsByProgram.Series.Add(series);

                Legend legend = new Legend("MainLegend")
                {
                    Docking = Docking.Bottom,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(71, 85, 105), // Màu chữ xám Slate thanh lịch
                    BackColor = Color.Transparent
                };
                chartStudentsByProgram.Legends.Add(legend);

                pnlPieChart.Controls.Add(chartStudentsByProgram);
                chartStudentsByProgram.BringToFront();
                if (lblPieTitle != null) lblPieTitle.BringToFront();
            }

            // 2. KHỞI TẠO & LÀM ĐẸP BIỂU ĐỒ ĐƯỜNG (LINE CHART)
            if (chartEnrollmentTrend == null && pnlLineChart != null)
            {
                chartEnrollmentTrend = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
                chartEnrollmentTrend.Padding = new Padding(15, 50, 15, 15);

                ChartArea area = new ChartArea("MainArea")
                {
                    BackColor = Color.White
                };

                // Làm mờ đường lưới trục X và Y để làm nổi bật đường xu hướng chính
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);

                // Định dạng trục tọa độ phẳng (Flat Axis)
                area.AxisX.LineColor = Color.FromArgb(203, 213, 225);
                area.AxisY.LineColor = Color.FromArgb(203, 213, 225);
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                area.AxisX.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);
                area.AxisY.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);

                chartEnrollmentTrend.ChartAreas.Add(area);

                Series series = new Series("Enrollments")
                {
                    ChartType = SeriesChartType.Spline, // CHUYỂN TỪ LINE SANG SPLINE: Giúp đường đồ thị uốn lượn mượt mà thay vì gấp khúc thô cứng
                    BorderWidth = 4,
                    Color = Color.FromArgb(37, 99, 235), // Màu xanh Royal của các Dashboard hiện đại
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 10,
                    MarkerColor = Color.White,
                    MarkerBorderColor = Color.FromArgb(37, 99, 235),
                    MarkerBorderWidth = 3
                };

                chartEnrollmentTrend.Series.Add(series);

                pnlLineChart.Controls.Add(chartEnrollmentTrend);
                chartEnrollmentTrend.BringToFront();
                if (lblLineTitle != null) lblLineTitle.BringToFront();
            }
        }

        private void BindPieChart(DataTable dt)
        {
            if (chartStudentsByProgram == null) return;
            chartStudentsByProgram.Series["Programs"].Points.Clear();

            Color[] colors = {
                Color.FromArgb(34, 197, 94),   // Xuất sắc (Xanh lá)
                Color.FromArgb(59, 130, 246),  // Giỏi (Xanh dương)
                Color.FromArgb(234, 179, 8),   // Khá (Vàng)
                Color.FromArgb(168, 85, 247),  // Trung bình (Tím)
                Color.FromArgb(239, 68, 68)    // Yếu/Kém (Đỏ)
            };
            int colorIndex = 0;

            foreach (DataRow row in dt.Rows)
            {
                string rankingGroup = row["RankingGroup"].ToString();
                int studentCount = Convert.ToInt32(row["StudentCount"]);

                DataPoint point = new DataPoint();
                point.SetValueY(studentCount);
                point.AxisLabel = rankingGroup;
                point.LegendText = $"{rankingGroup} ({studentCount})";
                point.Color = colors[colorIndex % colors.Length];
                colorIndex++;

                chartStudentsByProgram.Series["Programs"].Points.Add(point);
            }

            chartStudentsByProgram.Series["Programs"].Label = "#PERCENT{P0}";
            chartStudentsByProgram.Series["Programs"].Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }

        private void BindLineChart(DataTable dt)
        {
            if (chartEnrollmentTrend == null) return;
            chartEnrollmentTrend.Series["Enrollments"].Points.Clear();

            chartEnrollmentTrend.Series["Enrollments"].MarkerStyle = MarkerStyle.Circle;
            chartEnrollmentTrend.Series["Enrollments"].MarkerSize = 8;
            chartEnrollmentTrend.Series["Enrollments"].Color = Color.FromArgb(59, 130, 246);
            chartEnrollmentTrend.Series["Enrollments"].BorderWidth = 3;

            foreach (DataRow row in dt.Rows)
            {
                string monthYear = row["MonthYear"].ToString();
                int total = Convert.ToInt32(row["Total"]);
                chartEnrollmentTrend.Series["Enrollments"].Points.AddXY(monthYear, total);
            }
        }

        private void BindTopStudentsGrid(DataTable dt)
        {
            if (dgvTopCourses == null) return;

            dgvTopCourses.DataSource = dt;

            if (dgvTopCourses.Columns["MSSV"] != null) dgvTopCourses.Columns["MSSV"].HeaderText = "Mã số SV";
            if (dgvTopCourses.Columns["FullName"] != null) dgvTopCourses.Columns["FullName"].HeaderText = "Họ và Tên Sinh Viên";
            if (dgvTopCourses.Columns["GPA"] != null) dgvTopCourses.Columns["GPA"].HeaderText = "Điểm GPA Tổng Kết";
            if (dgvTopCourses.Columns["Classification"] != null) dgvTopCourses.Columns["Classification"].HeaderText = "Xếp Loại";

            dgvTopCourses.EnableHeadersVisualStyles = false;
            dgvTopCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // Thay màu tối sang trọng hơn màu xám cũ
            dgvTopCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTopCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTopCourses.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvTopCourses.RowTemplate.Height = 32;
            dgvTopCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void BindSummaryCards(DataRow row)
        {
            if (row == null || lblTotalStudentsVal == null || lblNewAdmissionsVal == null || lblPassRateVal == null) return;

            int totalStudents = row["TotalStudents"] != DBNull.Value ? Convert.ToInt32(row["TotalStudents"]) : 0;
            int newAdmissions = row["NewAdmissions"] != DBNull.Value ? Convert.ToInt32(row["NewAdmissions"]) : 0;
            double passRate = row["PassRate"] != DBNull.Value ? Convert.ToDouble(row["PassRate"]) : 0.0;

            lblTotalStudentsVal.Text = totalStudents.ToString("N0");
            lblNewAdmissionsVal.Text = newAdmissions.ToString("N0");
            lblPassRateVal.Text = passRate.ToString("F1") + "%";

            if (lblAttendanceRateVal != null)
            {
                lblAttendanceRateVal.Text = "100%"; // Giữ nguyên logic mặc định của bạn
            }
        }
    }
}