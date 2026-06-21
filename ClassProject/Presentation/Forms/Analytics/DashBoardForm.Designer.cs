using System.Windows.Forms;
using System.Drawing;

namespace ClassProject.Presentation.Forms.Admin
{
    partial class DashBoardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            tlpMainLayout = new TableLayoutPanel();
            tlpCardsGrid = new TableLayoutPanel();
            pnlCard1 = new Guna.UI2.WinForms.Guna2Panel();
            lblTotalStudents = new Label();
            lblTotalStudentsVal = new Label();
            pnlCard2 = new Guna.UI2.WinForms.Guna2Panel();
            lblNewAdmissions = new Label();
            lblNewAdmissionsVal = new Label();
            pnlCard3 = new Guna.UI2.WinForms.Guna2Panel();
            lblAttendanceRate = new Label();
            lblAttendanceRateVal = new Label();
            pnlCard4 = new Guna.UI2.WinForms.Guna2Panel();
            lblPassRate = new Label();
            lblPassRateVal = new Label();
            tlpChartsGrid = new TableLayoutPanel();
            pnlPieChart = new Guna.UI2.WinForms.Guna2Panel();
            pnlPieChartContainer = new Panel();
            lblPieTitle = new Label();
            pnlLineChart = new Guna.UI2.WinForms.Guna2Panel();
            pnlLineChartContainer = new Panel();
            lblLineTitle = new Label();
            pnlDoughnutChart = new Guna.UI2.WinForms.Guna2Panel();
            pnlDoughnutChartContainer = new Panel();
            lblDoughnutTitle = new Label();
            pnlTopCourses = new Guna.UI2.WinForms.Guna2Panel();
            dgvTopCourses = new DataGridView();
            lblTopCoursesTitle = new Label();
            tlpMainLayout.SuspendLayout();
            tlpCardsGrid.SuspendLayout();
            pnlCard1.SuspendLayout();
            pnlCard2.SuspendLayout();
            pnlCard3.SuspendLayout();
            pnlCard4.SuspendLayout();
            tlpChartsGrid.SuspendLayout();
            pnlPieChart.SuspendLayout();
            pnlLineChart.SuspendLayout();
            pnlDoughnutChart.SuspendLayout();
            pnlTopCourses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopCourses).BeginInit();
            SuspendLayout();
            // 
            // tlpMainLayout
            // 
            tlpMainLayout.ColumnCount = 1;
            tlpMainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMainLayout.Controls.Add(tlpCardsGrid, 0, 0);
            tlpMainLayout.Controls.Add(tlpChartsGrid, 0, 1);
            tlpMainLayout.Dock = DockStyle.Fill;
            tlpMainLayout.Location = new Point(0, 0);
            tlpMainLayout.Name = "tlpMainLayout";
            tlpMainLayout.Padding = new Padding(20);
            tlpMainLayout.RowCount = 2;
            tlpMainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tlpMainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            tlpMainLayout.Size = new Size(1400, 850);
            tlpMainLayout.TabIndex = 0;
            // 
            // tlpCardsGrid
            // 
            tlpCardsGrid.ColumnCount = 4;
            tlpCardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpCardsGrid.Controls.Add(pnlCard1, 0, 0);
            tlpCardsGrid.Controls.Add(pnlCard2, 1, 0);
            tlpCardsGrid.Controls.Add(pnlCard3, 2, 0);
            tlpCardsGrid.Controls.Add(pnlCard4, 3, 0);
            tlpCardsGrid.Dock = DockStyle.Fill;
            tlpCardsGrid.Location = new Point(20, 20);
            tlpCardsGrid.Margin = new Padding(0, 0, 0, 15);
            tlpCardsGrid.Name = "tlpCardsGrid";
            tlpCardsGrid.RowCount = 1;
            tlpCardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpCardsGrid.Size = new Size(1360, 106);
            tlpCardsGrid.TabIndex = 0;
            // 
            // pnlCard1
            // 
            pnlCard1.BackColor = Color.Transparent;
            pnlCard1.BorderColor = Color.FromArgb(226, 232, 240);
            pnlCard1.BorderRadius = 10;
            pnlCard1.BorderThickness = 1;
            pnlCard1.Controls.Add(lblTotalStudents);
            pnlCard1.Controls.Add(lblTotalStudentsVal);
            pnlCard1.CustomizableEdges = customizableEdges1;
            pnlCard1.Dock = DockStyle.Fill;
            pnlCard1.FillColor = Color.White;
            pnlCard1.Location = new Point(10, 10);
            pnlCard1.Margin = new Padding(10);
            pnlCard1.Name = "pnlCard1";
            pnlCard1.Padding = new Padding(20);
            pnlCard1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlCard1.Size = new Size(320, 86);
            pnlCard1.TabIndex = 0;
            // 
            // lblTotalStudents
            // 
            lblTotalStudents.AutoSize = true;
            lblTotalStudents.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblTotalStudents.ForeColor = Color.FromArgb(148, 163, 184);
            lblTotalStudents.Location = new Point(20, 16);
            lblTotalStudents.Name = "lblTotalStudents";
            lblTotalStudents.Size = new Size(119, 21);
            lblTotalStudents.TabIndex = 0;
            lblTotalStudents.Text = "Tổng Sinh Viên";
            // 
            // lblTotalStudentsVal
            // 
            lblTotalStudentsVal.AutoSize = true;
            lblTotalStudentsVal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTotalStudentsVal.ForeColor = Color.FromArgb(15, 23, 42);
            lblTotalStudentsVal.Location = new Point(16, 38);
            lblTotalStudentsVal.Name = "lblTotalStudentsVal";
            lblTotalStudentsVal.Size = new Size(43, 50);
            lblTotalStudentsVal.TabIndex = 1;
            lblTotalStudentsVal.Text = "0";
            // 
            // pnlCard2
            // 
            pnlCard2.BackColor = Color.Transparent;
            pnlCard2.BorderColor = Color.FromArgb(226, 232, 240);
            pnlCard2.BorderRadius = 10;
            pnlCard2.BorderThickness = 1;
            pnlCard2.Controls.Add(lblNewAdmissions);
            pnlCard2.Controls.Add(lblNewAdmissionsVal);
            pnlCard2.CustomizableEdges = customizableEdges3;
            pnlCard2.Dock = DockStyle.Fill;
            pnlCard2.FillColor = Color.White;
            pnlCard2.Location = new Point(350, 10);
            pnlCard2.Margin = new Padding(10);
            pnlCard2.Name = "pnlCard2";
            pnlCard2.Padding = new Padding(20);
            pnlCard2.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlCard2.Size = new Size(320, 86);
            pnlCard2.TabIndex = 1;
            // 
            // lblNewAdmissions
            // 
            lblNewAdmissions.AutoSize = true;
            lblNewAdmissions.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblNewAdmissions.ForeColor = Color.FromArgb(148, 163, 184);
            lblNewAdmissions.Location = new Point(20, 16);
            lblNewAdmissions.Name = "lblNewAdmissions";
            lblNewAdmissions.Size = new Size(116, 21);
            lblNewAdmissions.TabIndex = 0;
            lblNewAdmissions.Text = "Nhập Học Mới";
            // 
            // lblNewAdmissionsVal
            // 
            lblNewAdmissionsVal.AutoSize = true;
            lblNewAdmissionsVal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblNewAdmissionsVal.ForeColor = Color.FromArgb(37, 99, 235);
            lblNewAdmissionsVal.Location = new Point(16, 38);
            lblNewAdmissionsVal.Name = "lblNewAdmissionsVal";
            lblNewAdmissionsVal.Size = new Size(43, 50);
            lblNewAdmissionsVal.TabIndex = 1;
            lblNewAdmissionsVal.Text = "0";
            // 
            // pnlCard3
            // 
            pnlCard3.BackColor = Color.Transparent;
            pnlCard3.BorderColor = Color.FromArgb(226, 232, 240);
            pnlCard3.BorderRadius = 10;
            pnlCard3.BorderThickness = 1;
            pnlCard3.Controls.Add(lblAttendanceRate);
            pnlCard3.Controls.Add(lblAttendanceRateVal);
            pnlCard3.CustomizableEdges = customizableEdges5;
            pnlCard3.Dock = DockStyle.Fill;
            pnlCard3.FillColor = Color.White;
            pnlCard3.Location = new Point(690, 10);
            pnlCard3.Margin = new Padding(10);
            pnlCard3.Name = "pnlCard3";
            pnlCard3.Padding = new Padding(20);
            pnlCard3.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlCard3.Size = new Size(320, 86);
            pnlCard3.TabIndex = 2;
            // 
            // lblAttendanceRate
            // 
            lblAttendanceRate.AutoSize = true;
            lblAttendanceRate.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblAttendanceRate.ForeColor = Color.FromArgb(148, 163, 184);
            lblAttendanceRate.Location = new Point(20, 16);
            lblAttendanceRate.Name = "lblAttendanceRate";
            lblAttendanceRate.Size = new Size(125, 21);
            lblAttendanceRate.TabIndex = 0;
            lblAttendanceRate.Text = "Tỷ Lệ Xử Lý Đơn";
            // 
            // lblAttendanceRateVal
            // 
            lblAttendanceRateVal.AutoSize = true;
            lblAttendanceRateVal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblAttendanceRateVal.ForeColor = Color.FromArgb(16, 185, 129);
            lblAttendanceRateVal.Location = new Point(16, 38);
            lblAttendanceRateVal.Name = "lblAttendanceRateVal";
            lblAttendanceRateVal.Size = new Size(106, 50);
            lblAttendanceRateVal.TabIndex = 1;
            lblAttendanceRateVal.Text = "0.0%";
            // 
            // pnlCard4
            // 
            pnlCard4.BackColor = Color.Transparent;
            pnlCard4.BorderColor = Color.FromArgb(226, 232, 240);
            pnlCard4.BorderRadius = 10;
            pnlCard4.BorderThickness = 1;
            pnlCard4.Controls.Add(lblPassRate);
            pnlCard4.Controls.Add(lblPassRateVal);
            pnlCard4.CustomizableEdges = customizableEdges7;
            pnlCard4.Dock = DockStyle.Fill;
            pnlCard4.FillColor = Color.White;
            pnlCard4.Location = new Point(1030, 10);
            pnlCard4.Margin = new Padding(10);
            pnlCard4.Name = "pnlCard4";
            pnlCard4.Padding = new Padding(20);
            pnlCard4.ShadowDecoration.CustomizableEdges = customizableEdges8;
            pnlCard4.Size = new Size(320, 86);
            pnlCard4.TabIndex = 3;
            // 
            // lblPassRate
            // 
            lblPassRate.AutoSize = true;
            lblPassRate.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblPassRate.ForeColor = Color.FromArgb(148, 163, 184);
            lblPassRate.Location = new Point(20, 16);
            lblPassRate.Name = "lblPassRate";
            lblPassRate.Size = new Size(119, 21);
            lblPassRate.TabIndex = 0;
            lblPassRate.Text = "Tỷ Lệ Qua Môn";
            // 
            // lblPassRateVal
            // 
            lblPassRateVal.AutoSize = true;
            lblPassRateVal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblPassRateVal.ForeColor = Color.FromArgb(245, 158, 11);
            lblPassRateVal.Location = new Point(16, 38);
            lblPassRateVal.Name = "lblPassRateVal";
            lblPassRateVal.Size = new Size(106, 50);
            lblPassRateVal.TabIndex = 1;
            lblPassRateVal.Text = "0.0%";
            // 
            // tlpChartsGrid
            // 
            tlpChartsGrid.ColumnCount = 2;
            tlpChartsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpChartsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpChartsGrid.Controls.Add(pnlPieChart, 0, 0);
            tlpChartsGrid.Controls.Add(pnlLineChart, 1, 0);
            tlpChartsGrid.Controls.Add(pnlDoughnutChart, 0, 1);
            tlpChartsGrid.Controls.Add(pnlTopCourses, 1, 1);
            tlpChartsGrid.Dock = DockStyle.Fill;
            tlpChartsGrid.Location = new Point(23, 144);
            tlpChartsGrid.Name = "tlpChartsGrid";
            tlpChartsGrid.RowCount = 2;
            tlpChartsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpChartsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpChartsGrid.Size = new Size(1354, 683);
            tlpChartsGrid.TabIndex = 1;
            // 
            // pnlPieChart
            // 
            pnlPieChart.BackColor = Color.Transparent;
            pnlPieChart.BorderColor = Color.FromArgb(226, 232, 240);
            pnlPieChart.BorderRadius = 10;
            pnlPieChart.BorderThickness = 1;
            pnlPieChart.Controls.Add(pnlPieChartContainer);
            pnlPieChart.Controls.Add(lblPieTitle);
            pnlPieChart.CustomizableEdges = customizableEdges9;
            pnlPieChart.Dock = DockStyle.Fill;
            pnlPieChart.FillColor = Color.White;
            pnlPieChart.Location = new Point(10, 10);
            pnlPieChart.Margin = new Padding(10);
            pnlPieChart.Name = "pnlPieChart";
            pnlPieChart.Padding = new Padding(20, 15, 20, 15);
            pnlPieChart.ShadowDecoration.CustomizableEdges = customizableEdges10;
            pnlPieChart.Size = new Size(657, 321);
            pnlPieChart.TabIndex = 0;
            // 
            // pnlPieChartContainer
            // 
            pnlPieChartContainer.Location = new Point(17, 56);
            pnlPieChartContainer.Name = "pnlPieChartContainer";
            pnlPieChartContainer.Size = new Size(617, 247);
            pnlPieChartContainer.TabIndex = 1;
            // 
            // lblPieTitle
            // 
            lblPieTitle.AutoSize = true;
            lblPieTitle.Dock = DockStyle.Top;
            lblPieTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblPieTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblPieTitle.Location = new Point(20, 15);
            lblPieTitle.Name = "lblPieTitle";
            lblPieTitle.Padding = new Padding(0, 0, 0, 10);
            lblPieTitle.Size = new Size(330, 38);
            lblPieTitle.TabIndex = 0;
            lblPieTitle.Text = "Cơ Cấu Sinh Viên Theo Ngành Học";
            // 
            // pnlLineChart
            // 
            pnlLineChart.BackColor = Color.Transparent;
            pnlLineChart.BorderColor = Color.FromArgb(226, 232, 240);
            pnlLineChart.BorderRadius = 10;
            pnlLineChart.BorderThickness = 1;
            pnlLineChart.Controls.Add(pnlLineChartContainer);
            pnlLineChart.Controls.Add(lblLineTitle);
            pnlLineChart.CustomizableEdges = customizableEdges11;
            pnlLineChart.Dock = DockStyle.Fill;
            pnlLineChart.FillColor = Color.White;
            pnlLineChart.Location = new Point(687, 10);
            pnlLineChart.Margin = new Padding(10);
            pnlLineChart.Name = "pnlLineChart";
            pnlLineChart.Padding = new Padding(20, 15, 20, 15);
            pnlLineChart.ShadowDecoration.CustomizableEdges = customizableEdges12;
            pnlLineChart.Size = new Size(657, 321);
            pnlLineChart.TabIndex = 1;
            // 
            // pnlLineChartContainer
            // 
            pnlLineChartContainer.Location = new Point(20, 56);
            pnlLineChartContainer.Name = "pnlLineChartContainer";
            pnlLineChartContainer.Size = new Size(617, 247);
            pnlLineChartContainer.TabIndex = 2;
            // 
            // lblLineTitle
            // 
            lblLineTitle.AutoSize = true;
            lblLineTitle.Dock = DockStyle.Top;
            lblLineTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblLineTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblLineTitle.Location = new Point(20, 15);
            lblLineTitle.Name = "lblLineTitle";
            lblLineTitle.Padding = new Padding(0, 0, 0, 10);
            lblLineTitle.Size = new Size(264, 38);
            lblLineTitle.TabIndex = 0;
            lblLineTitle.Text = "Biểu Đồ Xu Hướng Đào Tạo";
            // 
            // pnlDoughnutChart
            // 
            pnlDoughnutChart.BackColor = Color.Transparent;
            pnlDoughnutChart.BorderColor = Color.FromArgb(226, 232, 240);
            pnlDoughnutChart.BorderRadius = 10;
            pnlDoughnutChart.BorderThickness = 1;
            pnlDoughnutChart.Controls.Add(pnlDoughnutChartContainer);
            pnlDoughnutChart.Controls.Add(lblDoughnutTitle);
            pnlDoughnutChart.CustomizableEdges = customizableEdges13;
            pnlDoughnutChart.Dock = DockStyle.Fill;
            pnlDoughnutChart.FillColor = Color.White;
            pnlDoughnutChart.Location = new Point(10, 351);
            pnlDoughnutChart.Margin = new Padding(10);
            pnlDoughnutChart.Name = "pnlDoughnutChart";
            pnlDoughnutChart.Padding = new Padding(20, 15, 20, 15);
            pnlDoughnutChart.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlDoughnutChart.Size = new Size(657, 322);
            pnlDoughnutChart.TabIndex = 2;
            // 
            // pnlDoughnutChartContainer
            // 
            pnlDoughnutChartContainer.Location = new Point(17, 53);
            pnlDoughnutChartContainer.Name = "pnlDoughnutChartContainer";
            pnlDoughnutChartContainer.Size = new Size(617, 247);
            pnlDoughnutChartContainer.TabIndex = 2;
            // 
            // lblDoughnutTitle
            // 
            lblDoughnutTitle.AutoSize = true;
            lblDoughnutTitle.Dock = DockStyle.Top;
            lblDoughnutTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblDoughnutTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblDoughnutTitle.Location = new Point(20, 15);
            lblDoughnutTitle.Name = "lblDoughnutTitle";
            lblDoughnutTitle.Padding = new Padding(0, 0, 0, 10);
            lblDoughnutTitle.Size = new Size(321, 38);
            lblDoughnutTitle.TabIndex = 0;
            lblDoughnutTitle.Text = "Tình Trạng Xử Lý Đơn Thư Học Vụ";
            // 
            // pnlTopCourses
            // 
            pnlTopCourses.BackColor = Color.Transparent;
            pnlTopCourses.BorderColor = Color.FromArgb(226, 232, 240);
            pnlTopCourses.BorderRadius = 10;
            pnlTopCourses.BorderThickness = 1;
            pnlTopCourses.Controls.Add(dgvTopCourses);
            pnlTopCourses.Controls.Add(lblTopCoursesTitle);
            pnlTopCourses.CustomizableEdges = customizableEdges15;
            pnlTopCourses.Dock = DockStyle.Fill;
            pnlTopCourses.FillColor = Color.White;
            pnlTopCourses.Location = new Point(687, 351);
            pnlTopCourses.Margin = new Padding(10);
            pnlTopCourses.Name = "pnlTopCourses";
            pnlTopCourses.Padding = new Padding(20, 15, 20, 15);
            pnlTopCourses.ShadowDecoration.CustomizableEdges = customizableEdges16;
            pnlTopCourses.Size = new Size(657, 322);
            pnlTopCourses.TabIndex = 3;
            // 
            // dgvTopCourses
            // 
            dgvTopCourses.AllowUserToAddRows = false;
            dgvTopCourses.AllowUserToDeleteRows = false;
            dgvTopCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTopCourses.BackgroundColor = Color.White;
            dgvTopCourses.BorderStyle = BorderStyle.None;
            dgvTopCourses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTopCourses.Dock = DockStyle.Fill;
            dgvTopCourses.Location = new Point(20, 53);
            dgvTopCourses.Name = "dgvTopCourses";
            dgvTopCourses.ReadOnly = true;
            dgvTopCourses.RowHeadersVisible = false;
            dgvTopCourses.RowHeadersWidth = 51;
            dgvTopCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTopCourses.Size = new Size(617, 254);
            dgvTopCourses.TabIndex = 0;
            // 
            // lblTopCoursesTitle
            // 
            lblTopCoursesTitle.AutoSize = true;
            lblTopCoursesTitle.Dock = DockStyle.Top;
            lblTopCoursesTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblTopCoursesTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblTopCoursesTitle.Location = new Point(20, 15);
            lblTopCoursesTitle.Name = "lblTopCoursesTitle";
            lblTopCoursesTitle.Padding = new Padding(0, 0, 0, 10);
            lblTopCoursesTitle.Size = new Size(262, 38);
            lblTopCoursesTitle.TabIndex = 1;
            lblTopCoursesTitle.Text = "Kết Quả Học Phần Xuất Sắc";
            // 
            // DashBoardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1400, 850);
            Controls.Add(tlpMainLayout);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DashBoardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hệ Thống Thống Kê Giám Sát";
            tlpMainLayout.ResumeLayout(false);
            tlpCardsGrid.ResumeLayout(false);
            pnlCard1.ResumeLayout(false);
            pnlCard1.PerformLayout();
            pnlCard2.ResumeLayout(false);
            pnlCard2.PerformLayout();
            pnlCard3.ResumeLayout(false);
            pnlCard3.PerformLayout();
            pnlCard4.ResumeLayout(false);
            pnlCard4.PerformLayout();
            tlpChartsGrid.ResumeLayout(false);
            pnlPieChart.ResumeLayout(false);
            pnlPieChart.PerformLayout();
            pnlLineChart.ResumeLayout(false);
            pnlLineChart.PerformLayout();
            pnlDoughnutChart.ResumeLayout(false);
            pnlDoughnutChart.PerformLayout();
            pnlTopCourses.ResumeLayout(false);
            pnlTopCourses.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopCourses).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMainLayout;
        private System.Windows.Forms.TableLayoutPanel tlpCardsGrid;
        private System.Windows.Forms.TableLayoutPanel tlpChartsGrid;
        private Guna.UI2.WinForms.Guna2Panel pnlCard1;
        private System.Windows.Forms.Label lblTotalStudentsVal;
        private System.Windows.Forms.Label lblTotalStudents;
        private Guna.UI2.WinForms.Guna2Panel pnlCard2;
        private System.Windows.Forms.Label lblNewAdmissionsVal;
        private System.Windows.Forms.Label lblNewAdmissions;
        private Guna.UI2.WinForms.Guna2Panel pnlCard3;
        private System.Windows.Forms.Label lblAttendanceRateVal;
        private System.Windows.Forms.Label lblAttendanceRate;
        private Guna.UI2.WinForms.Guna2Panel pnlCard4;
        private System.Windows.Forms.Label lblPassRateVal;
        private System.Windows.Forms.Label lblPassRate;
        private Guna.UI2.WinForms.Guna2Panel pnlPieChart;
        private System.Windows.Forms.Label lblPieTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlLineChart;
        private System.Windows.Forms.Label lblLineTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlDoughnutChart;
        private System.Windows.Forms.Label lblDoughnutTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlTopCourses;
        private System.Windows.Forms.Label lblTopCoursesTitle;
        private System.Windows.Forms.DataGridView dgvTopCourses;
        private Panel pnlPieChartContainer;
        private Panel pnlLineChartContainer;
        private Panel pnlDoughnutChartContainer;
    }
}