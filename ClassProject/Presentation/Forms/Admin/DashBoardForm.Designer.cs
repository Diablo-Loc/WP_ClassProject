using System.Windows.Forms;
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
            pnlCard1 = new Panel();
            lblTotalStudentsVal = new Label();
            lblTotalStudents = new Label();
            pnlCard2 = new Panel();
            lblNewAdmissionsVal = new Label();
            lblNewAdmissions = new Label();
            pnlCard3 = new Panel();
            lblAttendanceRateVal = new Label();
            lblAttendanceRate = new Label();
            pnlCard4 = new Panel();
            lblPassRateVal = new Label();
            lblPassRate = new Label();
            pnlPieChart = new Panel();
            lblPieTitle = new Label();
            pnlLineChart = new Panel();
            lblLineTitle = new Label();
            pnlDoughnutChart = new Panel();
            lblDoughnutTitle = new Label();
            pnlTopCourses = new Panel();
            lblTopCoursesTitle = new Label();
            dgvTopCourses = new DataGridView();
            pnlCard1.SuspendLayout();
            pnlCard2.SuspendLayout();
            pnlCard3.SuspendLayout();
            pnlCard4.SuspendLayout();
            pnlPieChart.SuspendLayout();
            pnlLineChart.SuspendLayout();
            pnlDoughnutChart.SuspendLayout();
            pnlTopCourses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopCourses).BeginInit();
            SuspendLayout();
            // 
            // pnlCard1
            // 
            pnlCard1.BackColor = Color.White;
            pnlCard1.BorderStyle = BorderStyle.FixedSingle;
            pnlCard1.Controls.Add(lblTotalStudentsVal);
            pnlCard1.Controls.Add(lblTotalStudents);
            pnlCard1.Location = new Point(33, 119);
            pnlCard1.Margin = new Padding(5, 4, 5, 4);
            pnlCard1.Name = "pnlCard1";
            pnlCard1.Size = new Size(332, 138);
            pnlCard1.TabIndex = 1;
            // 
            // lblTotalStudentsVal
            // 
            lblTotalStudentsVal.AutoSize = true;
            lblTotalStudentsVal.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTotalStudentsVal.ForeColor = Color.FromArgb(15, 23, 42);
            lblTotalStudentsVal.Location = new Point(21, 55);
            lblTotalStudentsVal.Margin = new Padding(5, 0, 5, 0);
            lblTotalStudentsVal.Name = "lblTotalStudentsVal";
            lblTotalStudentsVal.Size = new Size(109, 46);
            lblTotalStudentsVal.TabIndex = 1;
            lblTotalStudentsVal.Text = "2,350";
            // 
            // lblTotalStudents
            // 
            lblTotalStudents.AutoSize = true;
            lblTotalStudents.Font = new Font("Segoe UI", 10F);
            lblTotalStudents.ForeColor = Color.Gray;
            lblTotalStudents.Location = new Point(21, 19);
            lblTotalStudents.Margin = new Padding(5, 0, 5, 0);
            lblTotalStudents.Name = "lblTotalStudents";
            lblTotalStudents.Size = new Size(117, 23);
            lblTotalStudents.TabIndex = 0;
            lblTotalStudents.Text = "Total Students";
            // 
            // pnlCard2
            // 
            pnlCard2.BackColor = Color.White;
            pnlCard2.BorderStyle = BorderStyle.FixedSingle;
            pnlCard2.Controls.Add(lblNewAdmissionsVal);
            pnlCard2.Controls.Add(lblNewAdmissions);
            pnlCard2.Location = new Point(400, 119);
            pnlCard2.Margin = new Padding(5, 4, 5, 4);
            pnlCard2.Name = "pnlCard2";
            pnlCard2.Size = new Size(332, 138);
            pnlCard2.TabIndex = 2;
            // 
            // lblNewAdmissionsVal
            // 
            lblNewAdmissionsVal.AutoSize = true;
            lblNewAdmissionsVal.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblNewAdmissionsVal.ForeColor = Color.FromArgb(15, 23, 42);
            lblNewAdmissionsVal.Location = new Point(21, 55);
            lblNewAdmissionsVal.Margin = new Padding(5, 0, 5, 0);
            lblNewAdmissionsVal.Name = "lblNewAdmissionsVal";
            lblNewAdmissionsVal.Size = new Size(80, 46);
            lblNewAdmissionsVal.TabIndex = 1;
            lblNewAdmissionsVal.Text = "320";
            // 
            // lblNewAdmissions
            // 
            lblNewAdmissions.AutoSize = true;
            lblNewAdmissions.Font = new Font("Segoe UI", 10F);
            lblNewAdmissions.ForeColor = Color.Gray;
            lblNewAdmissions.Location = new Point(21, 19);
            lblNewAdmissions.Margin = new Padding(5, 0, 5, 0);
            lblNewAdmissions.Name = "lblNewAdmissions";
            lblNewAdmissions.Size = new Size(134, 23);
            lblNewAdmissions.TabIndex = 0;
            lblNewAdmissions.Text = "New Admissions";
            // 
            // pnlCard3
            // 
            pnlCard3.BackColor = Color.White;
            pnlCard3.BorderStyle = BorderStyle.FixedSingle;
            pnlCard3.Controls.Add(lblAttendanceRateVal);
            pnlCard3.Controls.Add(lblAttendanceRate);
            pnlCard3.Location = new Point(767, 119);
            pnlCard3.Margin = new Padding(5, 4, 5, 4);
            pnlCard3.Name = "pnlCard3";
            pnlCard3.Size = new Size(332, 138);
            pnlCard3.TabIndex = 2;
            // 
            // lblAttendanceRateVal
            // 
            lblAttendanceRateVal.AutoSize = true;
            lblAttendanceRateVal.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblAttendanceRateVal.ForeColor = Color.FromArgb(15, 23, 42);
            lblAttendanceRateVal.Location = new Point(21, 55);
            lblAttendanceRateVal.Margin = new Padding(5, 0, 5, 0);
            lblAttendanceRateVal.Name = "lblAttendanceRateVal";
            lblAttendanceRateVal.Size = new Size(118, 46);
            lblAttendanceRateVal.TabIndex = 1;
            lblAttendanceRateVal.Text = "91.6%";
            // 
            // lblAttendanceRate
            // 
            lblAttendanceRate.AutoSize = true;
            lblAttendanceRate.Font = new Font("Segoe UI", 10F);
            lblAttendanceRate.ForeColor = Color.Gray;
            lblAttendanceRate.Location = new Point(21, 19);
            lblAttendanceRate.Margin = new Padding(5, 0, 5, 0);
            lblAttendanceRate.Name = "lblAttendanceRate";
            lblAttendanceRate.Size = new Size(137, 23);
            lblAttendanceRate.TabIndex = 0;
            lblAttendanceRate.Text = "Attendance Rate";
            // 
            // pnlCard4
            // 
            pnlCard4.BackColor = Color.White;
            pnlCard4.BorderStyle = BorderStyle.FixedSingle;
            pnlCard4.Controls.Add(lblPassRateVal);
            pnlCard4.Controls.Add(lblPassRate);
            pnlCard4.Location = new Point(1134, 119);
            pnlCard4.Margin = new Padding(5, 4, 5, 4);
            pnlCard4.Name = "pnlCard4";
            pnlCard4.Size = new Size(365, 138);
            pnlCard4.TabIndex = 2;
            // 
            // lblPassRateVal
            // 
            lblPassRateVal.AutoSize = true;
            lblPassRateVal.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblPassRateVal.ForeColor = Color.FromArgb(15, 23, 42);
            lblPassRateVal.Location = new Point(21, 55);
            lblPassRateVal.Margin = new Padding(5, 0, 5, 0);
            lblPassRateVal.Name = "lblPassRateVal";
            lblPassRateVal.Size = new Size(118, 46);
            lblPassRateVal.TabIndex = 1;
            lblPassRateVal.Text = "89.4%";
            // 
            // lblPassRate
            // 
            lblPassRate.AutoSize = true;
            lblPassRate.Font = new Font("Segoe UI", 10F);
            lblPassRate.ForeColor = Color.Gray;
            lblPassRate.Location = new Point(21, 19);
            lblPassRate.Margin = new Padding(5, 0, 5, 0);
            lblPassRate.Name = "lblPassRate";
            lblPassRate.Size = new Size(81, 23);
            lblPassRate.TabIndex = 0;
            lblPassRate.Text = "Pass Rate";
            // 
            // pnlPieChart
            // 
            pnlPieChart.BackColor = Color.White;
            pnlPieChart.BorderStyle = BorderStyle.FixedSingle;
            pnlPieChart.Controls.Add(lblPieTitle);
            pnlPieChart.Location = new Point(33, 284);
            pnlPieChart.Margin = new Padding(5, 4, 5, 4);
            pnlPieChart.Name = "pnlPieChart";
            pnlPieChart.Size = new Size(699, 491);
            pnlPieChart.TabIndex = 3;
            // 
            // lblPieTitle
            // 
            lblPieTitle.AutoSize = true;
            lblPieTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblPieTitle.Location = new Point(21, 12);
            lblPieTitle.Margin = new Padding(5, 0, 5, 0);
            lblPieTitle.Name = "lblPieTitle";
            lblPieTitle.Size = new Size(211, 28);
            lblPieTitle.TabIndex = 1;
            lblPieTitle.Text = "Students by Program";
            // 
            // pnlLineChart
            // 
            pnlLineChart.BackColor = Color.White;
            pnlLineChart.BorderStyle = BorderStyle.FixedSingle;
            pnlLineChart.Controls.Add(lblLineTitle);
            pnlLineChart.Location = new Point(767, 284);
            pnlLineChart.Margin = new Padding(5, 4, 5, 4);
            pnlLineChart.Name = "pnlLineChart";
            pnlLineChart.Size = new Size(732, 491);
            pnlLineChart.TabIndex = 4;
            // 
            // lblLineTitle
            // 
            lblLineTitle.AutoSize = true;
            lblLineTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblLineTitle.Location = new Point(21, 12);
            lblLineTitle.Margin = new Padding(5, 0, 5, 0);
            lblLineTitle.Name = "lblLineTitle";
            lblLineTitle.Size = new Size(175, 28);
            lblLineTitle.TabIndex = 1;
            lblLineTitle.Text = "Enrollment Trend";
            // 
            // pnlDoughnutChart
            // 
            pnlDoughnutChart.BackColor = Color.White;
            pnlDoughnutChart.BorderStyle = BorderStyle.FixedSingle;
            pnlDoughnutChart.Controls.Add(lblDoughnutTitle);
            pnlDoughnutChart.Location = new Point(33, 808);
            pnlDoughnutChart.Margin = new Padding(5, 4, 5, 4);
            pnlDoughnutChart.Name = "pnlDoughnutChart";
            pnlDoughnutChart.Size = new Size(699, 491);
            pnlDoughnutChart.TabIndex = 5;
            // 
            // lblDoughnutTitle
            // 
            lblDoughnutTitle.AutoSize = true;
            lblDoughnutTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblDoughnutTitle.Location = new Point(21, 12);
            lblDoughnutTitle.Margin = new Padding(5, 0, 5, 0);
            lblDoughnutTitle.Name = "lblDoughnutTitle";
            lblDoughnutTitle.Size = new Size(217, 28);
            lblDoughnutTitle.TabIndex = 1;
            lblDoughnutTitle.Text = "Attendance Overview";
            // 
            // pnlTopCourses
            // 
            pnlTopCourses.BackColor = Color.White;
            pnlTopCourses.BorderStyle = BorderStyle.FixedSingle;
            pnlTopCourses.Controls.Add(lblTopCoursesTitle);
            pnlTopCourses.Controls.Add(dgvTopCourses);
            pnlTopCourses.Location = new Point(767, 808);
            pnlTopCourses.Margin = new Padding(5, 4, 5, 4);
            pnlTopCourses.Name = "pnlTopCourses";
            pnlTopCourses.Size = new Size(732, 491);
            pnlTopCourses.TabIndex = 6;
            // 
            // lblTopCoursesTitle
            // 
            lblTopCoursesTitle.AutoSize = true;
            lblTopCoursesTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTopCoursesTitle.Location = new Point(21, 12);
            lblTopCoursesTitle.Margin = new Padding(5, 0, 5, 0);
            lblTopCoursesTitle.Name = "lblTopCoursesTitle";
            lblTopCoursesTitle.Size = new Size(237, 28);
            lblTopCoursesTitle.TabIndex = 1;
            lblTopCoursesTitle.Text = "Top Performing Courses";
            // 
            // dgvTopCourses
            // 
            dgvTopCourses.AllowUserToAddRows = false;
            dgvTopCourses.AllowUserToDeleteRows = false;
            dgvTopCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTopCourses.BackgroundColor = Color.White;
            dgvTopCourses.BorderStyle = BorderStyle.None;
            dgvTopCourses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTopCourses.Location = new Point(21, 65);
            dgvTopCourses.Margin = new Padding(5, 4, 5, 4);
            dgvTopCourses.Name = "dgvTopCourses";
            dgvTopCourses.ReadOnly = true;
            dgvTopCourses.RowHeadersVisible = false;
            dgvTopCourses.RowHeadersWidth = 51;
            dgvTopCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTopCourses.Size = new Size(687, 400);
            dgvTopCourses.TabIndex = 0;
            // 
            // DashBoardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1534, 1222);
            Controls.Add(pnlTopCourses);
            Controls.Add(pnlDoughnutChart);
            Controls.Add(pnlLineChart);
            Controls.Add(pnlPieChart);
            Controls.Add(pnlCard4);
            Controls.Add(pnlCard3);
            Controls.Add(pnlCard2);
            Controls.Add(pnlCard1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5, 4, 5, 4);
            Name = "DashBoardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "StatisticsForm";
            Load += DashBoardForm_Load;
            pnlCard1.ResumeLayout(false);
            pnlCard1.PerformLayout();
            pnlCard2.ResumeLayout(false);
            pnlCard2.PerformLayout();
            pnlCard3.ResumeLayout(false);
            pnlCard3.PerformLayout();
            pnlCard4.ResumeLayout(false);
            pnlCard4.PerformLayout();
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
        private System.Windows.Forms.Panel pnlCard1;
        private System.Windows.Forms.Label lblTotalStudentsVal;
        private System.Windows.Forms.Label lblTotalStudents;
        private System.Windows.Forms.Panel pnlCard2;
        private System.Windows.Forms.Label lblNewAdmissionsVal;
        private System.Windows.Forms.Label lblNewAdmissions;
        private System.Windows.Forms.Panel pnlCard3;
        private System.Windows.Forms.Label lblAttendanceRateVal;
        private System.Windows.Forms.Label lblAttendanceRate;
        private System.Windows.Forms.Panel pnlCard4;
        private System.Windows.Forms.Label lblPassRateVal;
        private System.Windows.Forms.Label lblPassRate;
        private System.Windows.Forms.Panel pnlPieChart;
        private System.Windows.Forms.Label lblPieTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartStudentsByProgram;
        private System.Windows.Forms.Panel pnlLineChart;
        private System.Windows.Forms.Label lblLineTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartEnrollmentTrend;
        private System.Windows.Forms.Panel pnlDoughnutChart;
        private System.Windows.Forms.Label lblDoughnutTitle;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartAttendanceOverview;
        private System.Windows.Forms.Panel pnlTopCourses;
        private System.Windows.Forms.Label lblTopCoursesTitle;
        private System.Windows.Forms.DataGridView dgvTopCourses;
    }
}