using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class DashBoardForm : Form
    {
        private readonly DataAccess.Db.My_DB db = new DataAccess.Db.My_DB();

        public DashBoardForm()
        {
            InitializeComponent();
        }

        private void DashBoardForm_Load(object sender, EventArgs e)
        {
            InitializeDynamicCharts();
            LoadPieChartData();
            LoadLineChartData();
            LoadTopStudentsGridData();
            LoadSummaryCardsData();
        }

        private void InitializeDynamicCharts()
        {
            // 1. Khởi tạo Biểu đồ tròn (Xếp loại học lực) gắn vào pnlPieChart
            if (chartStudentsByProgram == null && pnlPieChart != null)
            {
                chartStudentsByProgram = new Chart();
                chartStudentsByProgram.Dock = DockStyle.Fill;
                chartStudentsByProgram.BackColor = Color.White;

                // Căn lề trên (Top = 45) giúp chừa không gian hiển thị cho lblPieTitle
                chartStudentsByProgram.Padding = new Padding(10, 45, 10, 10);

                ChartArea area = new ChartArea("MainArea");
                chartStudentsByProgram.ChartAreas.Add(area);

                Series series = new Series("Programs");
                series.ChartType = SeriesChartType.Pie;
                chartStudentsByProgram.Series.Add(series);

                Legend legend = new Legend("MainLegend");
                legend.Docking = Docking.Bottom;
                chartStudentsByProgram.Legends.Add(legend);

                pnlPieChart.Controls.Add(chartStudentsByProgram);
                chartStudentsByProgram.BringToFront();
                if (lblPieTitle != null) lblPieTitle.BringToFront();
            }

            // 2. Khởi tạo Biểu đồ đường (Xu hướng nhập học) gắn vào pnlLineChart
            if (chartEnrollmentTrend == null && pnlLineChart != null)
            {
                chartEnrollmentTrend = new Chart();
                chartEnrollmentTrend.Dock = DockStyle.Fill;
                chartEnrollmentTrend.BackColor = Color.White;

                // Căn lề trên (Top = 45) giúp chừa không gian hiển thị cho lblLineTitle
                chartEnrollmentTrend.Padding = new Padding(10, 45, 10, 10);

                ChartArea area = new ChartArea("MainArea");
                // Định dạng lưới mờ chuẩn Dashboard hiện đại
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(241, 245, 249);
                chartEnrollmentTrend.ChartAreas.Add(area);

                Series series = new Series("Enrollments");
                series.ChartType = SeriesChartType.Line;
                chartEnrollmentTrend.Series.Add(series);

                pnlLineChart.Controls.Add(chartEnrollmentTrend);
                chartEnrollmentTrend.BringToFront();
                if (lblLineTitle != null) lblLineTitle.BringToFront();
            }
        }

        // 1. BIỂU ĐỒ TRÒN: ĐỒNG BỘ THEO CỘT [RankingGroup] TỪ FILE SQL
        private void LoadPieChartData()
        {
            if (chartStudentsByProgram == null) return;
            chartStudentsByProgram.Series["Programs"].Points.Clear();

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand("proc_GetAcademicRankingStatistics", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Color[] colors = {
                            Color.FromArgb(34, 197, 94),   // Xuất sắc (Xanh lá)
                            Color.FromArgb(59, 130, 246),  // Giỏi (Xanh dương)
                            Color.FromArgb(234, 179, 8),   // Khá (Vàng)
                            Color.FromArgb(168, 85, 247),  // Trung bình (Tím)
                            Color.FromArgb(239, 68, 68)    // Yếu/Kém (Đỏ)
                        };
                        int colorIndex = 0;

                        while (reader.Read())
                        {
                            //  Sử dụng "RankingGroup" thay vì "XepLoai" để khớp 100% kết quả từ Procedure SQL
                            string rankingGroup = reader["RankingGroup"].ToString();
                            int studentCount = Convert.ToInt32(reader["StudentCount"]);

                            DataPoint point = new DataPoint();
                            point.SetValueY(studentCount);
                            point.AxisLabel = rankingGroup;
                            point.LegendText = $"{rankingGroup} ({studentCount})";
                            point.Color = colors[colorIndex % colors.Length];
                            colorIndex++;

                            chartStudentsByProgram.Series["Programs"].Points.Add(point);
                        }
                    }
                }

                chartStudentsByProgram.Series["Programs"].Label = "#PERCENT{P0}";
                chartStudentsByProgram.Series["Programs"].Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải biểu đồ xếp loại: " + ex.Message, "Hệ thống thống kê", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. BIỂU ĐỒ ĐƯỜNG: ĐỒNG BỘ THEO FILE TĂNG TRƯỞNG 005_dashboard_trends.sql
        private void LoadLineChartData()
        {
            if (chartEnrollmentTrend == null) return;
            chartEnrollmentTrend.Series["Enrollments"].Points.Clear();

            chartEnrollmentTrend.Series["Enrollments"].MarkerStyle = MarkerStyle.Circle;
            chartEnrollmentTrend.Series["Enrollments"].MarkerSize = 8;
            chartEnrollmentTrend.Series["Enrollments"].Color = Color.FromArgb(59, 130, 246);
            chartEnrollmentTrend.Series["Enrollments"].BorderWidth = 3;

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand("proc_GetEnrollmentTrendStatistics", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string monthYear = reader["MonthYear"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            chartEnrollmentTrend.Series["Enrollments"].Points.AddXY(monthYear, total);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải xu hướng sinh viên: " + ex.Message, "Hệ thống thống kê", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. DATAGRIDVIEW: HIỂN THỊ DANH SÁCH TOP 10 SINH VIÊN XUẤT SẮC
        private void LoadTopStudentsGridData()
        {
            if (dgvTopCourses == null) return;
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand("proc_GetTopStudentsRanking", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        adapter.Fill(dt);
                    }
                }

                dgvTopCourses.DataSource = dt;

                // Việt hóa tiêu đề lưới hiển thị tương ứng các cột MSSV, FullName, GPA của Procedure
                if (dgvTopCourses.Columns["MSSV"] != null) dgvTopCourses.Columns["MSSV"].HeaderText = "Mã số SV";
                if (dgvTopCourses.Columns["FullName"] != null) dgvTopCourses.Columns["FullName"].HeaderText = "Họ và Tên Sinh Viên";
                if (dgvTopCourses.Columns["GPA"] != null) dgvTopCourses.Columns["GPA"].HeaderText = "Điểm GPA Tổng Kết";

                // Kiểu dáng phẳng thanh lịch (Flat Web UI)
                dgvTopCourses.EnableHeadersVisualStyles = false;
                dgvTopCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
                dgvTopCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
                dgvTopCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvTopCourses.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgvTopCourses.RowTemplate.Height = 32;
                dgvTopCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị bảng xếp hạng điểm: " + ex.Message, "Hệ thống thống kê", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. THẺ THỐNG KÊ CARDS: CẬP NHẬT THỜI GIAN THỰC TỪ SQL TỔNG HỢP
        private void LoadSummaryCardsData()
        {
            if (lblTotalStudentsVal == null || lblNewAdmissionsVal == null || lblPassRateVal == null) return;

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand("proc_GetDashboardSummaryCards", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalStudents = reader["TotalStudents"] != DBNull.Value ? Convert.ToInt32(reader["TotalStudents"]) : 0;
                            int newAdmissions = reader["NewAdmissions"] != DBNull.Value ? Convert.ToInt32(reader["NewAdmissions"]) : 0;
                            double passRate = reader["PassRate"] != DBNull.Value ? Convert.ToDouble(reader["PassRate"]) : 0.0;

                            lblTotalStudentsVal.Text = totalStudents.ToString("N0");
                            lblNewAdmissionsVal.Text = newAdmissions.ToString("N0");
                            lblPassRateVal.Text = passRate.ToString("F1") + "%";

                            if (lblAttendanceRateVal != null)
                            {
                                lblAttendanceRateVal.Text = "100%";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải các thẻ thông số tổng quan: " + ex.Message, "Hệ thống thống kê", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}