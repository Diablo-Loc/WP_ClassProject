using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class StatisticsForm : Form
    {
        private readonly StatisticRepository _statRepo;
        private readonly My_DB _db = new My_DB();
        private LiveCharts.WinForms.PieChart livePieChart;
        private bool _isDataLoading = false;

        // Lưu thông tin phân quyền của người dùng hiện tại
        private readonly string _currentUserRole;
        private readonly int _currentUserId;

        /// <summary>
        /// Constructor nhận thông tin phân quyền từ Form chính (MainForm)
        /// </summary>
        /// <param name="role">"Admin", "HR", hoặc "Giảng viên"</param>
        /// <param name="userId">ID của người dùng đăng nhập (đặc biệt cần cho Giảng viên)</param>
        public StatisticsForm(string role, int userId = 0)
        {
            InitializeComponent();
            _currentUserRole = role;
            _currentUserId = userId;

            _statRepo = new StatisticRepository(_db.GetConnection().ConnectionString);
        }

        private async void StatisticsForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(241, 245, 249);

            InitializeLiveChart();
            StyleDashboardGrid();

            // Áp dụng thiết lập UI theo vai trò trước khi nạp dữ liệu
            ApplyRoleBasedUI();

            await RefreshDashboardDataAsync();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshDashboardDataAsync();
        }

        /// <summary>
        /// Ẩn/Hiện các thành phần giao diện dựa trên vai trò người dùng
        /// </summary>
        private void ApplyRoleBasedUI()
        {
            if (_currentUserRole == "HR")
            {
                // Ví dụ: HR chỉ cần xem biểu đồ tỉ lệ học lực, không cần xem danh sách chi tiết Top 10
                if (dgvTopRanking != null) dgvTopRanking.Visible = false;

                // Thay đổi tiêu đề form hoặc panel để HR biết phạm vi quyền hạn của mình
                this.Text = "Thống Kê Phân Tích Dành Cho Nhân Sự (HR)";
            }
            else if (_currentUserRole == "Giảng viên")
            {
                this.Text = "Thống Kê Lớp Học & Điểm Số - Giảng Viên";
                // Giảng viên xem được cả bảng điểm lẫn biểu đồ nhưng chỉ thuộc phạm vi lớp họ dạy
            }
            else // Admin
            {
                this.Text = "Bảng Điều Khiển Quản Trị Hệ Thống (Admin)";
            }
        }

        /// <summary>
        /// Xử lý nghiệp vụ chính: Đọc, gộp và tải dữ liệu theo phân quyền của từng Role
        /// </summary>
        private async Task RefreshDashboardDataAsync()
        {
            if (_isDataLoading) return;

            try
            {
                _isDataLoading = true;

                if (btnRefresh != null) btnRefresh.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                // Khởi tạo các Task rỗng để tránh lỗi chưa gán giá trị
                Task<DataRow> cardMetricsTask = null;
                Task<DataTable> topStudentsTask = Task.FromResult(new DataTable());
                Task<DataTable> academicRankingTask = Task.FromResult(new DataTable());

                // PHÂN LUỒNG TRUY VẤN THEO VAI TRÒ
                if (_currentUserRole == "Giảng viên")
                {
                    cardMetricsTask = _statRepo.GetDashboardCardMetricsByInstructorAsync(_currentUserId.ToString());
                    topStudentsTask = Task.Run(() => _statRepo.GetTopStudentsByInstructor(_currentUserId.ToString()));
                    academicRankingTask = Task.Run(() => _statRepo.GetAcademicRankingStatsByInstructor(_currentUserId.ToString()));
                }
                else if (_currentUserRole == "HR")
                {
                    // HR xem toàn trường giống Admin nhưng không cần nạp Task danh sách sinh viên
                    cardMetricsTask = _statRepo.GetDashboardCardMetricsAsync();
                    academicRankingTask = Task.Run(() => _statRepo.GetAcademicRankingStats());
                }
                else // Admin hoặc các quyền cao nhất
                {
                    cardMetricsTask = _statRepo.GetDashboardCardMetricsAsync();
                    topStudentsTask = Task.Run(() => _statRepo.GetTopStudents());
                    academicRankingTask = Task.Run(() => _statRepo.GetAcademicRankingStats());
                }

                // Đợi các luồng được chỉ định hoàn thành
                await Task.WhenAll(cardMetricsTask, topStudentsTask, academicRankingTask);

                // 1. CẬP NHẬT 3 THẺ KPI CARDS
                DataRow drMetrics = cardMetricsTask.Result;
                if (drMetrics != null)
                {
                    int totalStudents = drMetrics["TotalStudents"] != DBNull.Value ? Convert.ToInt32(drMetrics["TotalStudents"]) : 0;
                    double avgGpa = drMetrics["AvgGPA"] != DBNull.Value ? Convert.ToDouble(drMetrics["AvgGPA"]) : 0.0;
                    double excellentRate = drMetrics["ExcellentRate"] != DBNull.Value ? Convert.ToDouble(drMetrics["ExcellentRate"]) : 0.0;

                    if (lblTotalStudentsValue != null) lblTotalStudentsValue.Text = totalStudents.ToString("N0");
                    if (lblAvgGPAValue != null) lblAvgGPAValue.Text = avgGpa.ToString("0.00");
                    if (lblExcellentRateValue != null) lblExcellentRateValue.Text = Math.Round(excellentRate, 1).ToString("0.0") + "%";
                }

                // 2. ĐỔ DỮ LIỆU VÀO BẢNG XẾP HẠNG (Chỉ thực hiện nếu không phải HR)
                if (_currentUserRole != "HR" && dgvTopRanking != null)
                {
                    dgvTopRanking.DataSource = topStudentsTask.Result;
                    FormatGridColumns();
                }

                // 3. VẼ BIỂU ĐỒ TRÒN PHÂN PHỐI HỌC LỰC
                SetupLivePieChart(academicRankingTask.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi tải dữ liệu phân tích Dashboard: {ex.Message}",
                                "Lỗi Truy Vấn Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isDataLoading = false;
                if (btnRefresh != null) btnRefresh.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void InitializeLiveChart()
        {
            if (pnlChartContainer == null) return;

            livePieChart = new LiveCharts.WinForms.PieChart();
            livePieChart.Dock = DockStyle.Fill;
            livePieChart.LegendLocation = LegendLocation.Bottom;

            livePieChart.DefaultLegend.Foreground = System.Windows.Media.Brushes.DarkSlateGray;
            livePieChart.DefaultLegend.FontSize = 13;

            pnlChartContainer.Controls.Clear();
            pnlChartContainer.Controls.Add(livePieChart);
        }

        private void StyleDashboardGrid()
        {
            if (dgvTopRanking == null) return;

            dgvTopRanking.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTopRanking.AllowUserToAddRows = false;
            dgvTopRanking.EnableHeadersVisualStyles = false;
            dgvTopRanking.RowTemplate.Height = 35;
            dgvTopRanking.GridColor = Color.FromArgb(241, 245, 249);
            dgvTopRanking.BackgroundColor = Color.White;
            dgvTopRanking.BorderStyle = BorderStyle.None;
            dgvTopRanking.ReadOnly = true;

            dgvTopRanking.RowHeadersVisible = true;
            dgvTopRanking.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvTopRanking.RowHeadersWidth = 50;

            dgvTopRanking.ColumnHeadersVisible = true;
            dgvTopRanking.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTopRanking.ColumnHeadersHeight = 35;

            dgvTopRanking.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvTopRanking.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTopRanking.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvTopRanking.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvTopRanking.RowsDefaultCellStyle.BackColor = Color.White;
            dgvTopRanking.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvTopRanking.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvTopRanking.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);

            dgvTopRanking.RowPostPaint -= dgvTopRanking_RowPostPaint;
            dgvTopRanking.RowPostPaint += dgvTopRanking_RowPostPaint;
        }

        private void dgvTopRanking_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null) return;
            string rankText = (e.RowIndex + 1).ToString();

            using (Brush brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                var centerFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rankText, new Font("Segoe UI", 9, FontStyle.Bold), brush, headerBounds, centerFormat);
            }
        }

        private void FormatGridColumns()
        {
            dgvTopRanking.ColumnHeadersVisible = true;

            if (dgvTopRanking.Columns.Count > 0)
            {
                if (dgvTopRanking.Columns.Contains("MSSV")) dgvTopRanking.Columns["MSSV"].HeaderText = "MSSV";
                if (dgvTopRanking.Columns.Contains("FullName")) dgvTopRanking.Columns["FullName"].HeaderText = "Họ tên Sinh viên";
                if (dgvTopRanking.Columns.Contains("GPA")) dgvTopRanking.Columns["GPA"].HeaderText = "Điểm GPA";
                if (dgvTopRanking.Columns.Contains("Classification")) dgvTopRanking.Columns["Classification"].HeaderText = "Xếp loại";

                dgvTopRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvTopRanking.Columns.Contains("MSSV")) dgvTopRanking.Columns["MSSV"].FillWeight = 70;
                if (dgvTopRanking.Columns.Contains("FullName")) dgvTopRanking.Columns["FullName"].FillWeight = 140;
                if (dgvTopRanking.Columns.Contains("GPA")) dgvTopRanking.Columns["GPA"].FillWeight = 60;
                if (dgvTopRanking.Columns.Contains("Classification")) dgvTopRanking.Columns["Classification"].FillWeight = 70;
            }
        }

        private void SetupLivePieChart(DataTable dt)
        {
            if (livePieChart == null) return;

            SeriesCollection seriesCollection = new SeriesCollection();

            System.Windows.Media.Color[] colors = new System.Windows.Media.Color[] {
                System.Windows.Media.Color.FromRgb(16, 185, 129),  // Xuất sắc
                System.Windows.Media.Color.FromRgb(59, 130, 246),  // Giỏi
                System.Windows.Media.Color.FromRgb(245, 158, 11),  // Khá
                System.Windows.Media.Color.FromRgb(107, 114, 128), // Trung bình
                System.Windows.Media.Color.FromRgb(239, 68, 68)    // Yếu
            };

            int idx = 0;
            foreach (DataRow row in dt.Rows)
            {
                string title = row["RankingGroup"] != DBNull.Value ? row["RankingGroup"].ToString() : "Chưa phân loại";
                int count = row["StudentCount"] != DBNull.Value ? Convert.ToInt32(row["StudentCount"]) : 0;

                var fillBrush = new System.Windows.Media.SolidColorBrush(colors[idx % colors.Length]);
                fillBrush.Freeze();

                var pieSeries = new PieSeries
                {
                    Title = title,
                    Values = new ChartValues<int> { count },
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0} SV ({1:P1})", chartPoint.Y, chartPoint.Participation),
                    Fill = fillBrush,
                    PushOut = 2,
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeThickness = 1.5
                };

                seriesCollection.Add(pieSeries);
                idx++;
            }

            livePieChart.Series = seriesCollection;
        }
    }
}