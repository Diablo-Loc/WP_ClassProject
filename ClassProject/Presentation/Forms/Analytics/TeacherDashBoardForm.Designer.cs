using System.Drawing;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Analytics
{
    partial class TeacherDashBoardForm
    {
        private System.ComponentModel.IContainer components = null;

        // Khai báo các Controls sử dụng Guna.UI2 nâng cao
        private Guna.UI2.WinForms.Guna2Panel pnlTopHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblWelcomeTeacher;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSubHeader;

        // Layouts tự động co giãn theo tỷ lệ %
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private System.Windows.Forms.TableLayoutPanel tlpCharts;

        // Các thẻ số liệu (Cards)
        private Guna.UI2.WinForms.Guna2Panel cardClasses;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalClassesVal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalClassesTitle;

        private Guna.UI2.WinForms.Guna2Panel cardStudents;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalStudentsVal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalStudentsTitle;

        private Guna.UI2.WinForms.Guna2Panel cardPassRate;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPassRateVal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPassRateTitle;

        private Guna.UI2.WinForms.Guna2Panel cardPending;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPendingGradesVal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPendingGradesTitle;

        // Khu vực đồ thị và bảng dữ liệu chuyên sâu
        private Guna.UI2.WinForms.Guna2Panel pnlMainBody;
        public Guna.UI2.WinForms.Guna2Panel pnlLeftChartContainer;
        public Guna.UI2.WinForms.Guna2Panel pnlRightChartContainer;
        private Guna.UI2.WinForms.Guna2Panel pnlGridContainer;
        public Guna.UI2.WinForms.Guna2DataGridView dgvAtRiskStudents;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGridTitle;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();

            this.pnlTopHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.lblWelcomeTeacher = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblSubHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();

            // Khởi tạo các TableLayoutPanel quản lý layout đàn hồi
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.tlpCharts = new System.Windows.Forms.TableLayoutPanel();

            this.cardClasses = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTotalClassesVal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTotalClassesTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.cardStudents = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTotalStudentsVal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTotalStudentsTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.cardPassRate = new Guna.UI2.WinForms.Guna2Panel();
            this.lblPassRateVal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPassRateTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.cardPending = new Guna.UI2.WinForms.Guna2Panel();
            this.lblPendingGradesVal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPendingGradesTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.pnlMainBody = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlLeftChartContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlRightChartContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlGridContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvAtRiskStudents = new Guna.UI2.WinForms.Guna2DataGridView();
            this.lblGridTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.pnlTopHeader.SuspendLayout();
            this.tlpCards.SuspendLayout();
            this.cardClasses.SuspendLayout();
            this.cardStudents.SuspendLayout();
            this.cardPassRate.SuspendLayout();
            this.cardPending.SuspendLayout();
            this.pnlMainBody.SuspendLayout();
            this.tlpCharts.SuspendLayout();
            this.pnlGridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAtRiskStudents)).BeginInit();
            this.SuspendLayout();

            // ========================================================
            // pnlTopHeader (Thanh tiêu đề cố định trên cùng)
            // ========================================================
            this.pnlTopHeader.Controls.Add(this.lblSubHeader);
            this.pnlTopHeader.Controls.Add(this.lblWelcomeTeacher);
            this.pnlTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopHeader.FillColor = System.Drawing.Color.White;
            this.pnlTopHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlTopHeader.Name = "pnlTopHeader";
            this.pnlTopHeader.Size = new System.Drawing.Size(1150, 75);
            this.pnlTopHeader.TabIndex = 0;

            this.lblWelcomeTeacher.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcomeTeacher.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblWelcomeTeacher.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblWelcomeTeacher.Location = new System.Drawing.Point(24, 13);
            this.lblWelcomeTeacher.Name = "lblWelcomeTeacher";
            this.lblWelcomeTeacher.Size = new System.Drawing.Size(320, 32);
            this.lblWelcomeTeacher.Text = "Xin chào Thầy/Cô: Đang cập nhật...";

            this.lblSubHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblSubHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubHeader.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblSubHeader.Location = new System.Drawing.Point(25, 46);
            this.lblSubHeader.Name = "lblSubHeader";
            this.lblSubHeader.Size = new System.Drawing.Size(298, 19);
            this.lblSubHeader.Text = "Hệ thống tổng quan hiệu suất giảng dạy học kỳ hiện tại";

            // ========================================================
            // tlpCards (TableLayout chia đều 4 thẻ số liệu, mỗi thẻ 25%)
            // ========================================================
            this.tlpCards.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.tlpCards.ColumnCount = 4;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.Controls.Add(this.cardClasses, 0, 0);
            this.tlpCards.Controls.Add(this.cardStudents, 1, 0);
            this.tlpCards.Controls.Add(this.cardPassRate, 2, 0);
            this.tlpCards.Controls.Add(this.cardPending, 3, 0);
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCards.Location = new System.Drawing.Point(0, 75);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.tlpCards.RowCount = 1;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCards.Size = new System.Drawing.Size(1150, 135);
            this.tlpCards.TabIndex = 1;

            // --- Card 1 (Lớp học) ---
            this.cardClasses.BorderRadius = 12;
            this.cardClasses.Controls.Add(this.lblTotalClassesVal);
            this.cardClasses.Controls.Add(this.lblTotalClassesTitle);
            this.cardClasses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardClasses.FillColor = System.Drawing.Color.White;
            this.cardClasses.Location = new System.Drawing.Point(20, 20);
            this.cardClasses.Margin = new System.Windows.Forms.Padding(5);
            this.cardClasses.Name = "cardClasses";
            this.cardClasses.Size = new System.Drawing.Size(255, 105);
            this.cardClasses.TabIndex = 0;

            this.lblTotalClassesTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalClassesTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalClassesTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblTotalClassesTitle.Location = new System.Drawing.Point(18, 16);
            this.lblTotalClassesTitle.Text = "🏫 LỚP HỌC KỲ NÀY";

            this.lblTotalClassesVal.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalClassesVal.Font = new System.Drawing.Font("Segoe UI Black", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotalClassesVal.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblTotalClassesVal.Location = new System.Drawing.Point(18, 42);
            this.lblTotalClassesVal.Text = "0 Lớp";

            // --- Card 2 (Sinh viên) ---
            this.cardStudents.BorderRadius = 12;
            this.cardStudents.Controls.Add(this.lblTotalStudentsVal);
            this.cardStudents.Controls.Add(this.lblTotalStudentsTitle);
            this.cardStudents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardStudents.FillColor = System.Drawing.Color.White;
            this.cardStudents.Location = new System.Drawing.Point(300, 20);
            this.cardStudents.Margin = new System.Windows.Forms.Padding(5);
            this.cardStudents.Name = "cardStudents";
            this.cardStudents.Size = new System.Drawing.Size(255, 105);
            this.cardStudents.TabIndex = 1;

            this.lblTotalStudentsTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalStudentsTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalStudentsTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblTotalStudentsTitle.Location = new System.Drawing.Point(18, 16);
            this.lblTotalStudentsTitle.Text = "👥 TỔNG SỐ SINH VIÊN";

            this.lblTotalStudentsVal.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalStudentsVal.Font = new System.Drawing.Font("Segoe UI Black", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotalStudentsVal.ForeColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.lblTotalStudentsVal.Location = new System.Drawing.Point(18, 42);
            this.lblTotalStudentsVal.Text = "0 SV";

            // --- Card 3 (Điểm TB) ---
            this.cardPassRate.BorderRadius = 12;
            this.cardPassRate.Controls.Add(this.lblPassRateVal);
            this.cardPassRate.Controls.Add(this.lblPassRateTitle);
            this.cardPassRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPassRate.FillColor = System.Drawing.Color.White;
            this.cardPassRate.Location = new System.Drawing.Point(580, 20);
            this.cardPassRate.Margin = new System.Windows.Forms.Padding(5);
            this.cardPassRate.Name = "cardPassRate";
            this.cardPassRate.Size = new System.Drawing.Size(255, 105);
            this.cardPassRate.TabIndex = 2;

            this.lblPassRateTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPassRateTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPassRateTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblPassRateTitle.Location = new System.Drawing.Point(18, 16);
            this.lblPassRateTitle.Text = "📊 ĐIỂM TRUNG BÌNH LỚP";

            this.lblPassRateVal.BackColor = System.Drawing.Color.Transparent;
            this.lblPassRateVal.Font = new System.Drawing.Font("Segoe UI Black", 20F, System.Drawing.FontStyle.Bold);
            this.lblPassRateVal.ForeColor = System.Drawing.Color.FromArgb(245, 158, 11);
            this.lblPassRateVal.Location = new System.Drawing.Point(18, 42);
            this.lblPassRateVal.Text = "0.0 Đ";

            // --- Card 4 (Thiếu điểm) ---
            this.cardPending.BorderRadius = 12;
            this.cardPending.Controls.Add(this.lblPendingGradesVal);
            this.cardPending.Controls.Add(this.lblPendingGradesTitle);
            this.cardPending.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPending.FillColor = System.Drawing.Color.White;
            this.cardPending.Location = new System.Drawing.Point(860, 20);
            this.cardPending.Margin = new System.Windows.Forms.Padding(5);
            this.cardPending.Name = "cardPending";
            this.cardPending.Size = new System.Drawing.Size(260, 105);
            this.cardPending.TabIndex = 3;

            this.lblPendingGradesTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblPendingGradesTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPendingGradesTitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblPendingGradesTitle.Location = new System.Drawing.Point(18, 16);
            this.lblPendingGradesTitle.Text = "⚠️ ĐẦU ĐIỂM CHƯA NHẬP";

            this.lblPendingGradesVal.BackColor = System.Drawing.Color.Transparent;
            this.lblPendingGradesVal.Font = new System.Drawing.Font("Segoe UI Black", 20F, System.Drawing.FontStyle.Bold);
            this.lblPendingGradesVal.ForeColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.lblPendingGradesVal.Location = new System.Drawing.Point(18, 42);
            this.lblPendingGradesVal.Text = "0 Đang thiếu";

            // ========================================================
            // pnlMainBody (Bao phủ và co giãn toàn bộ vùng trống còn lại)
            // ========================================================
            this.pnlMainBody.Controls.Add(this.pnlGridContainer);
            this.pnlMainBody.Controls.Add(this.tlpCharts);
            this.pnlMainBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainBody.FillColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlMainBody.Location = new System.Drawing.Point(0, 210);
            this.pnlMainBody.Name = "pnlMainBody";
            this.pnlMainBody.Padding = new System.Windows.Forms.Padding(20, 10, 20, 15);
            this.pnlMainBody.Size = new System.Drawing.Size(1150, 490);
            this.pnlMainBody.TabIndex = 2;

            // ========================================================
            // tlpCharts (TableLayout chia tỉ lệ co giãn động 60% - 40%)
            // ========================================================
            this.tlpCharts.BackColor = System.Drawing.Color.Transparent;
            this.tlpCharts.ColumnCount = 2;
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpCharts.Controls.Add(this.pnlLeftChartContainer, 0, 0);
            this.tlpCharts.Controls.Add(this.pnlRightChartContainer, 1, 0);
            this.tlpCharts.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCharts.Location = new System.Drawing.Point(20, 10);
            this.tlpCharts.Name = "tlpCharts";
            this.tlpCharts.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.tlpCharts.RowCount = 1;
            this.tlpCharts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCharts.Size = new System.Drawing.Size(1110, 252);
            this.tlpCharts.TabIndex = 0;

            // Biểu đồ trái (60%)
            this.pnlLeftChartContainer.BorderRadius = 12;
            this.pnlLeftChartContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeftChartContainer.FillColor = System.Drawing.Color.White;
            this.pnlLeftChartContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftChartContainer.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.pnlLeftChartContainer.Name = "pnlLeftChartContainer";
            this.pnlLeftChartContainer.Size = new System.Drawing.Size(658, 240);
            this.pnlLeftChartContainer.TabIndex = 0;

            // Biểu đồ phải (40%)
            this.pnlRightChartContainer.BorderRadius = 12;
            this.pnlRightChartContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightChartContainer.FillColor = System.Drawing.Color.White;
            this.pnlRightChartContainer.Location = new System.Drawing.Point(674, 0);
            this.pnlRightChartContainer.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.pnlRightChartContainer.Name = "pnlRightChartContainer";
            this.pnlRightChartContainer.Size = new System.Drawing.Size(436, 240);
            this.pnlRightChartContainer.TabIndex = 1;

            // ========================================================
            // pnlGridContainer (Khung chứa bảng danh sách cuối trang)
            // ========================================================
            this.pnlGridContainer.BorderRadius = 12;
            this.pnlGridContainer.Controls.Add(this.lblGridTitle);
            this.pnlGridContainer.Controls.Add(this.dgvAtRiskStudents);
            this.pnlGridContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridContainer.FillColor = System.Drawing.Color.White;
            this.pnlGridContainer.Location = new System.Drawing.Point(20, 262);
            this.pnlGridContainer.Name = "pnlGridContainer";
            this.pnlGridContainer.Padding = new System.Windows.Forms.Padding(15, 45, 15, 15); // Tạo khoảng trống trên đầu làm tiêu đề
            this.pnlGridContainer.Size = new System.Drawing.Size(1110, 213);
            this.pnlGridContainer.TabIndex = 1;

            this.lblGridTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblGridTitle.Location = new System.Drawing.Point(18, 14);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(515, 22);
            this.lblGridTitle.Text = "🚩 DANH SÁCH SINH VIÊN CÓ NGUY CƠ TRƯỢT MÔN (ĐIỂM QT < 4.0)";

            // --- dgvAtRiskStudents (Bảng dữ liệu Guna) ---
            this.dgvAtRiskStudents.AllowUserToAddRows = false;
            this.dgvAtRiskStudents.AllowUserToDeleteRows = false;
            this.dgvAtRiskStudents.ReadOnly = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvAtRiskStudents.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAtRiskStudents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAtRiskStudents.BackgroundColor = System.Drawing.Color.White;
            this.dgvAtRiskStudents.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAtRiskStudents.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvAtRiskStudents.Dock = System.Windows.Forms.DockStyle.Fill; // Tự động co giãn phủ kín lòng Panel và sinh scrollbar

            // Header Style
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            this.dgvAtRiskStudents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAtRiskStudents.ColumnHeadersHeight = 35;

            // Row Style
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.dgvAtRiskStudents.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAtRiskStudents.Location = new System.Drawing.Point(15, 45);
            this.dgvAtRiskStudents.Name = "dgvAtRiskStudents";
            this.dgvAtRiskStudents.RowHeadersVisible = false;
            this.dgvAtRiskStudents.RowTemplate.Height = 32;
            this.dgvAtRiskStudents.Size = new System.Drawing.Size(1080, 153);
            this.dgvAtRiskStudents.ThemeStyle.RowsStyle.Height = 32;

            // ========================================================
            // TeacherDashBoardForm (Cấu hình Form tổng)
            // ========================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.ClientSize = new System.Drawing.Size(1150, 720);

            // Thay đổi Dock của MainBody thành Top để giữ form ổn định khi cuộn
            this.pnlMainBody.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMainBody.Size = new System.Drawing.Size(1150, 500); // Chiều cao lý tưởng cho thân dưới

            this.Controls.Add(this.pnlMainBody);
            this.Controls.Add(this.tlpCards);
            this.Controls.Add(this.pnlTopHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TeacherDashBoardForm";
            this.Text = "Teacher Dashboard";

            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.tlpCards.ResumeLayout(false);
            this.cardClasses.ResumeLayout(false);
            this.cardClasses.PerformLayout();
            this.cardStudents.ResumeLayout(false);
            this.cardStudents.PerformLayout();
            this.cardPassRate.ResumeLayout(false);
            this.cardPassRate.PerformLayout();
            this.cardPending.ResumeLayout(false);
            this.cardPending.PerformLayout();
            this.pnlMainBody.ResumeLayout(false);
            this.tlpCharts.ResumeLayout(false);
            this.pnlGridContainer.ResumeLayout(false);
            this.pnlGridContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAtRiskStudents)).EndInit();

            // Kích hoạt thanh cuộn thông minh khi màn hình bị thu nhỏ
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(1120, 710); // Ngưỡng an toàn bắt đầu xuất hiện thanh cuộn

            this.ResumeLayout(false);
        }

        #endregion
    }
}