using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ClassProject.Presentation.Forms.Analytics
{
    public partial class StudentDashboardForm : Form
    {
        private readonly DashboardRepository _dashboardRepo;
        private readonly RegisterRepository _registerRepo;
        private readonly RequestRepository _requestRepo;

        private readonly string _currentMssv = UserSession.MSSV;
        private Chart chartAcademicRanking;

        /// <summary>
        /// CONSTRUCTOR: Nhận các thực thể layer dữ liệu được inject từ bên ngoài vào
        /// </summary>
        public StudentDashboardForm(
            DashboardRepository dashboardRepo,
            RegisterRepository registerRepo,
            RequestRepository requestRepo)
        {
            InitializeComponent();

            _dashboardRepo = dashboardRepo;
            _registerRepo = registerRepo;
            _requestRepo = requestRepo;

            // Khởi tạo an toàn biểu đồ tròn
            InitializeOptionalPieChart();
            InitializeProgressChart();

            this.Load += StudentDashboardForm_Load;
        }

        private void InitializeOptionalPieChart()
        {
            // Kiểm tra tránh lỗi Null nếu Panel chưa được kéo thả hoặc đặt tên sai ở Designer
            if (pnlRightOptionalWrapper == null) return;

            chartAcademicRanking = new Chart { Dock = DockStyle.Fill };

            ChartArea chartArea = new ChartArea("RankingArea");
            chartAcademicRanking.ChartAreas.Add(chartArea);

            Legend legend = new Legend("RankingLegend") { Docking = Docking.Bottom };
            chartAcademicRanking.Legends.Add(legend);

            Series series = new Series("XepLoai")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelFormat = "#,##0"
            };
            chartAcademicRanking.Series.Add(series);

            pnlRightOptionalWrapper.Controls.Add(chartAcademicRanking);
            chartAcademicRanking.BringToFront();

            if (lblRightOptionalTitle != null)
            {
                lblRightOptionalTitle.Dock = DockStyle.Top;
                lblRightOptionalTitle.Padding = new Padding(5);
            }
        }
        private void InitializeProgressChart()
        {
            // Kiểm tra nếu Panel bọc ngoài biểu đồ xu hướng bị null thì dừng
            if (pnlChartWrapper == null) return;

            // Nếu ở Designer chưa khởi tạo, ta tự khởi tạo bằng code
            if (chartProgress == null)
            {
                chartProgress = new System.Windows.Forms.DataVisualization.Charting.Chart { Dock = DockStyle.Fill };

                // Thêm vào Panel bọc ngoài và đẩy lên phía trước tiêu đề để không bị đè
                pnlChartWrapper.Controls.Add(chartProgress);
                chartProgress.BringToFront();

                // Đẩy label tiêu đề "Xu hướng điểm số học kỳ" lên trên cùng
                if (lblChartTitle != null)
                {
                    lblChartTitle.Dock = DockStyle.Top;
                    lblChartTitle.Padding = new Padding(5);
                }
            }

            // Tự động thêm vùng vẽ đồ thị (ChartArea) nếu chưa có
            if (chartProgress.ChartAreas.Count == 0)
            {
                chartProgress.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea("DefaultArea"));
            }
        }
        private async void StudentDashboardForm_Load(object sender, EventArgs e)
        {
            await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var summaryTask = _dashboardRepo.GetDashboardSummaryCardsAsync(_currentMssv);
                var rankingTask = _dashboardRepo.GetAcademicRankingStatisticsAsync(_currentMssv);
                var trendTask = _dashboardRepo.GetEnrollmentTrendStatisticsAsync(_currentMssv);
                var requestsTask = Task.Run(() => _requestRepo.GetRequestsByStudent(_currentMssv));

                var registeredCoursesTask = Task.Run(() => _registerRepo.GetRegistrationList(_currentMssv));
                var dailyScheduleTask = Task.Run(() => _registerRepo.GetStudentDailySchedule(_currentMssv));

                // Đưa cả 2 vào WhenAll để chạy song song ngầm
                await Task.WhenAll(summaryTask, rankingTask, trendTask, requestsTask, registeredCoursesTask, dailyScheduleTask);

                await Task.WhenAll(summaryTask, rankingTask, trendTask, requestsTask, registeredCoursesTask);

                // ----------------------------------------------------------------------------
                // 1. ĐỔ DATA VÀO 4 THẺ KPI CARDS (Bọc lót chống DBNull / Null)
                // ----------------------------------------------------------------------------
                DataRow summaryCards = await summaryTask;
                if (summaryCards != null)
                {
                    // Tín chỉ tích lũy
                    string tcToken = (summaryCards["TotalStudents"] != DBNull.Value) ? summaryCards["TotalStudents"]?.ToString() : "0";
                    if (lblTinChiVal != null) lblTinChiVal.Text = $"{tcToken} / 120";

                    // Điểm số GPA
                    if (lblGpaVal != null)
                    {
                        lblGpaVal.Text = (summaryCards["PassRate"] != DBNull.Value && double.TryParse(summaryCards["PassRate"]?.ToString(), out double gpa))
                            ? gpa.ToString("0.00")
                            : "0.00";
                    }

                    // Số môn học
                    if (lblMonHocVal != null)
                    {
                        lblMonHocVal.Text = (summaryCards["NewAdmissions"] != DBNull.Value) ? summaryCards["NewAdmissions"]?.ToString() : "0";
                    }

                    // Học phí công nợ
                    if (lblHocPhiVal != null)
                    {
                        lblHocPhiVal.Text = (summaryCards["CustomHocPhi"] != DBNull.Value && decimal.TryParse(summaryCards["CustomHocPhi"]?.ToString(), out decimal hocPhi))
                            ? hocPhi.ToString("#,##0") + " đ"
                            : "0 đ";
                    }
                }
                // ----------------------------------------------------------------------------
                // 2.0. ĐỒNG BỘ THỜI KHÓA BIỂU SINH VIÊN (Bảng TRÊN - Lịch thời khóa biểu trong ngày)
                // ----------------------------------------------------------------------------
                DataTable dtDailySchedule = await dailyScheduleTask; // ĐỔI THÀNH THU THẬP TỪ dailyScheduleTask
                if (dtDailySchedule != null && dgvStudentSchedule != null)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        dgvStudentSchedule.SuspendLayout();
                        dgvStudentSchedule.DataSource = null;
                        dgvStudentSchedule.AutoGenerateColumns = true;

                        dgvStudentSchedule.DataSource = dtDailySchedule; // Đổ dữ liệu lịch chi tiết (Thứ, Ca, Giờ)
                        dgvStudentSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        if (dgvStudentSchedule.Columns.Contains("MaLopHP")) dgvStudentSchedule.Columns["MaLopHP"].HeaderText = "Mã Lớp HP";
                        if (dgvStudentSchedule.Columns.Contains("TenMH")) dgvStudentSchedule.Columns["TenMH"].HeaderText = "Tên Môn Học";
                        if (dgvStudentSchedule.Columns.Contains("PhongHoc")) dgvStudentSchedule.Columns["PhongHoc"].HeaderText = "Phòng Học";
                        if (dgvStudentSchedule.Columns.Contains("ThuHoc")) dgvStudentSchedule.Columns["ThuHoc"].HeaderText = "Thứ";
                        if (dgvStudentSchedule.Columns.Contains("CaHoc")) dgvStudentSchedule.Columns["CaHoc"].HeaderText = "Ca Học";
                        if (dgvStudentSchedule.Columns.Contains("ThoiGian")) dgvStudentSchedule.Columns["ThoiGian"].HeaderText = "Thời Gian Học";
                        if (dgvStudentSchedule.Columns.Contains("MSSV")) dgvStudentSchedule.Columns["MSSV"].Visible = false;

                        dgvStudentSchedule.ResumeLayout();
                        dgvStudentSchedule.Visible = true;
                        dgvStudentSchedule.BringToFront();
                    });
                }

                // ----------------------------------------------------------------------------
                // 2.1. DANH SÁCH LỚP HỌC PHẦN ĐÃ ĐĂNG KÝ (Bảng DƯỚI - Danh sách lớp học phần hiện tại)
                // ----------------------------------------------------------------------------
                DataTable dtClassList = await registeredCoursesTask; // ĐỔI THÀNH THU THẬP TỪ registeredCoursesTask
                if (dtClassList != null && dgvSchedule != null)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        dgvSchedule.DataSource = null;
                        dgvSchedule.AutoGenerateColumns = true;

                        dgvSchedule.DataSource = dtClassList; // Đổ dữ liệu danh sách tổng quan (Mã HP, Tên MH, Số TC, Giảng viên)
                        dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        if (dgvSchedule.Columns.Contains("MaLopHP")) dgvSchedule.Columns["MaLopHP"].HeaderText = "Mã Lớp HP";
                        if (dgvSchedule.Columns.Contains("TenMH")) dgvSchedule.Columns["TenMH"].HeaderText = "Tên Môn Học";
                        if (dgvSchedule.Columns.Contains("SoTC")) dgvSchedule.Columns["SoTC"].HeaderText = "Số TC";
                        if (dgvSchedule.Columns.Contains("TenGiangVien")) dgvSchedule.Columns["TenGiangVien"].HeaderText = "Giảng Viên";
                        if (dgvSchedule.Columns.Contains("PhongHoc")) dgvSchedule.Columns["PhongHoc"].HeaderText = "Phòng";

                        if (dgvSchedule.Columns.Contains("STT")) dgvSchedule.Columns["STT"].Visible = true; // Hiện cột STT nếu muốn giống ảnh
                        if (dgvSchedule.Columns.Contains("RegistrationDate")) dgvSchedule.Columns["RegistrationDate"].Visible = false;
                    });
                }

                // ----------------------------------------------------------------------------
                // 3. ĐỒNG BỘ BẢNG YÊU CẦU & PHẢN HỒI
                // ----------------------------------------------------------------------------
                DataTable dtRequests = await requestsTask;
                if (dtRequests != null && dgvRequest != null)
                {
                    dgvRequest.DataSource = dtRequests;
                    dgvRequest.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (dgvRequest.Columns.Contains("RequestContent")) dgvRequest.Columns["RequestContent"].HeaderText = "Nội dung yêu cầu";
                    if (dgvRequest.Columns.Contains("Status")) dgvRequest.Columns["Status"].HeaderText = "Trạng thái";
                    if (dgvRequest.Columns.Contains("AdminComment")) dgvRequest.Columns["AdminComment"].HeaderText = "Phản hồi từ Admin";
                    if (dgvRequest.Columns.Contains("Created_At")) dgvRequest.Columns["Created_At"].HeaderText = "Ngày gửi";

                    if (dgvRequest.Columns.Contains("Id")) dgvRequest.Columns["Id"].Visible = false;
                }

                // ----------------------------------------------------------------------------
                // 4. BIỂU ĐỒ TRÒN: THỐNG KÊ XẾP LOẠI HỌC LỰC (Bọc lót thông minh)
                // ----------------------------------------------------------------------------
                DataTable dtRanking = await rankingTask;
                if (dtRanking != null && chartAcademicRanking != null)
                {
                    chartAcademicRanking.Series["XepLoai"].Points.Clear();

                    string rankCol = dtRanking.Columns.Contains("AcademicRanking") ? "AcademicRanking" : (dtRanking.Columns.Count > 0 ? dtRanking.Columns[0].ColumnName : "");
                    string countCol = dtRanking.Columns.Contains("StudentCount") ? "StudentCount" : (dtRanking.Columns.Count > 1 ? dtRanking.Columns[1].ColumnName : "");

                    if (!string.IsNullOrEmpty(rankCol) && !string.IsNullOrEmpty(countCol) && dtRanking.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtRanking.Rows)
                        {
                            string rankingName = row[rankCol]?.ToString() ?? "Khác";
                            if (row[countCol] != DBNull.Value && double.TryParse(row[countCol]?.ToString(), out double count))
                            {
                                chartAcademicRanking.Series["XepLoai"].Points.AddXY(rankingName, count);
                            }
                        }
                    }
                    else
                    {
                        chartAcademicRanking.Series["XepLoai"].Points.AddXY("Chưa có dữ liệu", 100);
                    }
                }

                // ----------------------------------------------------------------------------
                // 5. BIỂU ĐỒ TUYẾN TÍNH: XU HƯỚNG ĐIỂM SỐ HỌC KỲ
                // ----------------------------------------------------------------------------
                DataTable dtTrend = await trendTask;
                if (dtTrend != null && chartProgress != null)
                {
                    // Xóa tiêu đề cũ nếu có
                    chartProgress.Titles.Clear();

                    if (chartProgress.ChartAreas.Count == 0)
                    {
                        chartProgress.ChartAreas.Add(new ChartArea("DefaultArea"));
                    }

                    Series tuyenDoSeries = chartProgress.Series.FindByName("Tiến độ");
                    if (tuyenDoSeries == null)
                    {
                        tuyenDoSeries = new Series("Tiến độ");
                        chartProgress.Series.Add(tuyenDoSeries);
                    }

                    tuyenDoSeries.Points.Clear();

                    string hocKyCol = dtTrend.Columns.Contains("MonthYear") ? "MonthYear" : (dtTrend.Columns.Count > 0 ? dtTrend.Columns[0].ColumnName : "");
                    string diemCol = dtTrend.Columns.Contains("Total") ? "Total" : (dtTrend.Columns.Count > 1 ? dtTrend.Columns[1].ColumnName : "");

                    if (!string.IsNullOrEmpty(hocKyCol) && !string.IsNullOrEmpty(diemCol) && dtTrend.Rows.Count > 0)
                    {
                        // Kích hoạt lại trục tọa độ khi có dữ liệu
                        chartProgress.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
                        chartProgress.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;

                        tuyenDoSeries.ChartType = SeriesChartType.Line;
                        tuyenDoSeries.IsValueShownAsLabel = true;
                        tuyenDoSeries.MarkerStyle = MarkerStyle.Circle;
                        tuyenDoSeries.MarkerSize = 8;
                        tuyenDoSeries.Color = Color.FromArgb(41, 128, 185);

                        foreach (DataRow row in dtTrend.Rows)
                        {
                            string hocKy = row[hocKyCol]?.ToString() ?? "";
                            if (row[diemCol] != DBNull.Value && double.TryParse(row[diemCol]?.ToString(), out double gpaMoc))
                            {
                                tuyenDoSeries.Points.AddXY(hocKy, gpaMoc);
                            }
                        }
                    }
                    else
                    {
                        // 🛠️ KHI KHÔNG CÓ DỮ LIỆU: Ẩn hoàn toàn các trục tọa độ (Không vẽ đường, không hiện mốc số)
                        chartProgress.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                        chartProgress.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;

                        // Thêm chữ thông báo lớn, căn giữa biểu đồ thay thế
                        Title emptyTitle = new Title
                        {
                            Text = "CHƯA CÓ DỮ LIỆU ĐIỂM HỌC KỲ\n(Dữ liệu sẽ hiển thị sau khi giảng viên nhập điểm)",
                            Font = new Font("Segoe UI", 11, FontStyle.Bold),
                            ForeColor = Color.Gray,
                            Alignment = ContentAlignment.MiddleCenter,
                            Docking = Docking.Top // Đặt ở trên hoặc tự căn giữa khu vực chart
                        };
                        chartProgress.Titles.Add(emptyTitle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình nạp dữ liệu giao diện sinh viên:\n{ex.Message}\nChi tiết: {ex.StackTrace}",
                                "Thông Báo Hệ Thống",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                if (this.IsHandleCreated)
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
    }
}