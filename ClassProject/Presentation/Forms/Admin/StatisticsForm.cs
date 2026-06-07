using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class StatisticsForm : Form
    {
        private readonly StatisticRepository _statRepo;
        private readonly My_DB db = new My_DB();
        private LiveCharts.WinForms.PieChart livePieChart;

        public StatisticsForm()
        {
            InitializeComponent();
            _statRepo = new StatisticRepository(db.GetConnection().ConnectionString);
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(241, 245, 249);

            InitializeLiveChart();

            StyleDashboardGrid();

            RefreshDashboardData();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDashboardData();
        }

        private void RefreshDashboardData()
        {
            this.Cursor = Cursors.WaitCursor; // Hiển thị con trỏ chuột chờ xử lý chuyên nghiệp
            try
            {
                LoadDashboardCards();
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi làm mới dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Cursor = Cursors.Default; // Trả con trỏ chuột về trạng thái bình thường
            }
        }

        private void InitializeLiveChart()
        {
            if (pnlChartContainer == null) return;

            livePieChart = new LiveCharts.WinForms.PieChart();
            livePieChart.Dock = DockStyle.Fill;
            livePieChart.LegendLocation = LegendLocation.Bottom; // Chú thích nằm dưới đáy biểu đồ

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

            dgvTopRanking.RowPostPaint -= dgvTopRanking_RowPostPaint; // Gỡ sự kiện cũ tránh trùng lặp
            dgvTopRanking.RowPostPaint += dgvTopRanking_RowPostPaint;
        }

        private void dgvTopRanking_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
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

        // GỘP VÀ CHUẨN HÓA TOÀN BỘ LOGIC ĐỌC CARD DỮ LIỆU TỪ DATABASE
        private void LoadDashboardCards()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    // 1. Đếm tổng số sinh viên toàn trường
                    SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Students", conn);
                    int totalStudents = Convert.ToInt32(cmd1.ExecuteScalar());
                    if (lblTotalStudentsValue != null) lblTotalStudentsValue.Text = totalStudents.ToString("N0");

                    // 2. Tính điểm GPA trung bình hệ thống từ cột DiemTK mới
                    SqlCommand cmd2 = new SqlCommand("SELECT AVG(DiemTK) FROM Score", conn);
                    object avgGpaObj = cmd2.ExecuteScalar();
                    double avgGpa = (avgGpaObj != DBNull.Value) ? Convert.ToDouble(avgGpaObj) : 0.0;

                    if (lblAvgGPAValue != null)
                        lblAvgGPAValue.Text = avgGpa.ToString("0.00");

                    // 3. Tính tỷ lệ sinh viên xuất sắc (DiemTK từ 9.0 trở lên)
                    SqlCommand cmd3 = new SqlCommand(@"
                        SELECT 
                            CASE WHEN COUNT(DISTINCT MSSV) = 0 THEN 0 
                            ELSE (COUNT(DISTINCT CASE WHEN DiemTK >= 9.0 THEN MSSV END) * 100.0 / COUNT(DISTINCT MSSV)) END
                        FROM Score", conn);
                    double excellentRate = Convert.ToDouble(cmd3.ExecuteScalar());

                    if (lblExcellentRateValue != null)
                        lblExcellentRateValue.Text = Math.Round(excellentRate, 1).ToString("0.0") + "%";
                }
            }
            catch
            {
                // Khóa an toàn chống sập ứng dụng khi database trống rỗng
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                // Tải dữ liệu bảng xếp hạng học lực Top 10 sinh viên
                DataTable dtTop = _statRepo.GetTopStudents();
                dgvTopRanking.DataSource = dtTop;
                FormatGridColumns();

                // Lấy dữ liệu phân loại học lực và kích hoạt biểu đồ tròn
                DataTable dtChart = _statRepo.GetAcademicRankingStats();
                SetupLivePieChart(dtChart);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xử lý dữ liệu thống kê: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGridColumns()
        {
            // Bảo hiểm: Bật hiển thị tên cột lên một lần nữa khi nạp nguồn dữ liệu mới
            dgvTopRanking.ColumnHeadersVisible = true;

            if (dgvTopRanking.Columns.Count > 0)
            {
                if (dgvTopRanking.Columns.Contains("MSSV")) dgvTopRanking.Columns["MSSV"].HeaderText = "MSSV";
                if (dgvTopRanking.Columns.Contains("FullName")) dgvTopRanking.Columns["FullName"].HeaderText = "Họ tên Sinh viên";
                if (dgvTopRanking.Columns.Contains("GPA")) dgvTopRanking.Columns["GPA"].HeaderText = "Điểm GPA";
                if (dgvTopRanking.Columns.Contains("Classification")) dgvTopRanking.Columns["Classification"].HeaderText = "Xếp loại";

                dgvTopRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Cân đối tỷ lệ các cột dữ liệu trên bảng xếp hạng Top 10
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
                System.Windows.Media.Color.FromRgb(16, 185, 129), // Xuất sắc
                System.Windows.Media.Color.FromRgb(59, 130, 246), // Giỏi
                System.Windows.Media.Color.FromRgb(245, 158, 11), // Khá
                System.Windows.Media.Color.FromRgb(235, 94, 40),  // Trung bình
                System.Windows.Media.Color.FromRgb(239, 68, 68)   // Yếu
            };

            livePieChart.Series = seriesCollection;

            int idx = 0;
            foreach (DataRow row in dt.Rows)
            {
                string title = row["RankingGroup"].ToString();
                int count = Convert.ToInt32(row["StudentCount"]);

                var fillBrush = new System.Windows.Media.SolidColorBrush(colors[idx % colors.Length]);
                fillBrush.Freeze();

                var pieSeries = new PieSeries
                {
                    Title = title,
                    Values = new ChartValues<int> { count },
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0} ({1:P1})", chartPoint.Y, chartPoint.Participation),
                    Fill = fillBrush,
                    PushOut = 2,
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeThickness = 1.5
                };

                seriesCollection.Add(pieSeries);
                idx++;
            }
        }
    }
}