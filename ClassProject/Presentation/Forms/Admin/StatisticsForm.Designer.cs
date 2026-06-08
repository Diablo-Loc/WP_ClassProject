namespace ClassProject.Presentation.Forms.Admin
{
    partial class StatisticsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();

            // Khai báo thêm các Panel quản lý layout cấp cao (Bổ sung để chia tỉ lệ responsive)
            pnlHeader = new Panel();
            tblCardsLayout = new TableLayoutPanel();

            lblTitle = new Label();
            label1 = new Label();
            txtLamMoi = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            lblTotalStudentsValue = new Label();
            label2 = new Label();
            guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            lblAvgGPAValue = new Label();
            label4 = new Label();
            guna2CircleButton2 = new Guna.UI2.WinForms.Guna2CircleButton();
            guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            lblExcellentRateValue = new Label();
            label6 = new Label();
            guna2CircleButton3 = new Guna.UI2.WinForms.Guna2CircleButton();
            pnlChartContainer = new Guna.UI2.WinForms.Guna2Panel();
            lblPhanBoHocLuc = new Label();
            guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            dgvTopRanking = new Guna.UI2.WinForms.Guna2DataGridView();
            label3 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();

            pnlHeader.SuspendLayout();
            tblCardsLayout.SuspendLayout();
            guna2Panel1.SuspendLayout();
            guna2Panel2.SuspendLayout();
            guna2Panel3.SuspendLayout();
            pnlChartContainer.SuspendLayout();
            guna2Panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopRanking).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(label1);
            pnlHeader.Controls.Add(txtLamMoi);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1136, 115);
            pnlHeader.TabIndex = 62;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(34, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(334, 41);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "THỐNG KÊ HỆ THỐNG";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(37, 73);
            label1.Name = "label1";
            label1.Size = new Size(268, 20);
            label1.TabIndex = 52;
            label1.Text = "Tổng quan và phân tích dữ liệu học tập";
            // 
            // txtLamMoi
            // 
            txtLamMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtLamMoi.BorderColor = Color.FromArgb(111, 66, 193);
            txtLamMoi.BorderRadius = 6;
            txtLamMoi.CustomizableEdges = customizableEdges1;
            txtLamMoi.DisabledState.BorderColor = Color.DarkGray;
            txtLamMoi.DisabledState.CustomBorderColor = Color.DarkGray;
            txtLamMoi.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            txtLamMoi.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            txtLamMoi.FillColor = Color.FromArgb(26, 115, 232);
            txtLamMoi.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtLamMoi.ForeColor = Color.White;
            txtLamMoi.Location = new Point(998, 35);
            txtLamMoi.Name = "txtLamMoi";
            txtLamMoi.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtLamMoi.Size = new Size(103, 44);
            txtLamMoi.TabIndex = 53;
            txtLamMoi.Text = "Làm mới";
            txtLamMoi.Click += BtnRefresh_Click;
            // 
            // tblCardsLayout
            // 
            tblCardsLayout.ColumnCount = 3;
            tblCardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tblCardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tblCardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tblCardsLayout.Controls.Add(guna2Panel1, 0, 0);
            tblCardsLayout.Controls.Add(guna2Panel2, 1, 0);
            tblCardsLayout.Controls.Add(guna2Panel3, 2, 0);
            tblCardsLayout.Dock = DockStyle.Top;
            tblCardsLayout.Location = new Point(0, 115);
            tblCardsLayout.Name = "tblCardsLayout";
            tblCardsLayout.Padding = new Padding(24, 0, 24, 0);
            tblCardsLayout.RowCount = 1;
            tblCardsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblCardsLayout.Size = new Size(1136, 125);
            tblCardsLayout.TabIndex = 63;
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.Transparent;
            guna2Panel1.BorderRadius = 10;
            guna2Panel1.Controls.Add(lblTotalStudentsValue);
            guna2Panel1.Controls.Add(label2);
            guna2Panel1.Controls.Add(guna2CircleButton1);
            guna2Panel1.CustomizableEdges = customizableEdges4;
            guna2Panel1.Dock = DockStyle.Fill;
            guna2Panel1.FillColor = Color.White;
            guna2Panel1.Location = new Point(34, 10);
            guna2Panel1.Margin = new Padding(10);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.BorderRadius = 10;
            guna2Panel1.ShadowDecoration.Color = Color.FromArgb(230, 235, 245);
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges5;
            guna2Panel1.ShadowDecoration.Enabled = true;
            guna2Panel1.Size = new Size(342, 105);
            guna2Panel1.TabIndex = 54;
            // 
            // lblTotalStudentsValue
            // 
            lblTotalStudentsValue.AutoSize = true;
            lblTotalStudentsValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalStudentsValue.ForeColor = Color.FromArgb(0, 90, 156);
            lblTotalStudentsValue.Location = new Point(105, 14);
            lblTotalStudentsValue.Name = "lblTotalStudentsValue";
            lblTotalStudentsValue.Size = new Size(42, 41);
            lblTotalStudentsValue.TabIndex = 57;
            lblTotalStudentsValue.Text = "...";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(105, 60);
            label2.Name = "label2";
            label2.Size = new Size(146, 20);
            label2.TabIndex = 56;
            label2.Text = "TỔNG SỐ SINH VIÊN";
            // 
            // guna2CircleButton1
            // 
            guna2CircleButton1.DisabledState.BorderColor = Color.DarkGray;
            guna2CircleButton1.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2CircleButton1.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2CircleButton1.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2CircleButton1.FillColor = Color.FromArgb(235, 243, 255);
            guna2CircleButton1.Font = new Font("Segoe UI", 9F);
            guna2CircleButton1.ForeColor = Color.White;
            guna2CircleButton1.Location = new Point(20, 19);
            guna2CircleButton1.Name = "guna2CircleButton1";
            guna2CircleButton1.ShadowDecoration.CustomizableEdges = customizableEdges3;
            guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CircleButton1.Size = new Size(65, 65);
            guna2CircleButton1.TabIndex = 55;
            guna2CircleButton1.Text = "";
            // 
            // guna2Panel2
            // 
            guna2Panel2.BackColor = Color.Transparent;
            guna2Panel2.BorderRadius = 10;
            guna2Panel2.Controls.Add(lblAvgGPAValue);
            guna2Panel2.Controls.Add(label4);
            guna2Panel2.Controls.Add(guna2CircleButton2);
            guna2Panel2.CustomizableEdges = customizableEdges7;
            guna2Panel2.Dock = DockStyle.Fill;
            guna2Panel2.FillColor = Color.White;
            guna2Panel2.Location = new Point(396, 10);
            guna2Panel2.Margin = new Padding(10);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.BorderRadius = 10;
            guna2Panel2.ShadowDecoration.Color = Color.FromArgb(230, 235, 245);
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges8;
            guna2Panel2.ShadowDecoration.Enabled = true;
            guna2Panel2.Size = new Size(342, 105);
            guna2Panel2.TabIndex = 58;
            // 
            // lblAvgGPAValue
            // 
            lblAvgGPAValue.AutoSize = true;
            lblAvgGPAValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAvgGPAValue.ForeColor = Color.FromArgb(0, 90, 156);
            lblAvgGPAValue.Location = new Point(105, 14);
            lblAvgGPAValue.Name = "lblAvgGPAValue";
            lblAvgGPAValue.Size = new Size(42, 41);
            lblAvgGPAValue.TabIndex = 57;
            lblAvgGPAValue.Text = "...";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Gray;
            label4.Location = new Point(105, 60);
            label4.Name = "label4";
            label4.Size = new Size(180, 20);
            label4.TabIndex = 56;
            label4.Text = "GPA TRUNG BÌNH CHUNG";
            // 
            // guna2CircleButton2
            // 
            guna2CircleButton2.DisabledState.BorderColor = Color.DarkGray;
            guna2CircleButton2.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2CircleButton2.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2CircleButton2.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2CircleButton2.FillColor = Color.FromArgb(235, 243, 255);
            guna2CircleButton2.Font = new Font("Segoe UI", 9F);
            guna2CircleButton2.ForeColor = Color.White;
            guna2CircleButton2.Location = new Point(20, 19);
            guna2CircleButton2.Name = "guna2CircleButton2";
            guna2CircleButton2.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2CircleButton2.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CircleButton2.Size = new Size(65, 65);
            guna2CircleButton2.TabIndex = 55;
            guna2CircleButton2.Text = "";
            // 
            // guna2Panel3
            // 
            guna2Panel3.BackColor = Color.Transparent;
            guna2Panel3.BorderRadius = 10;
            guna2Panel3.Controls.Add(lblExcellentRateValue);
            guna2Panel3.Controls.Add(label6);
            guna2Panel3.Controls.Add(guna2CircleButton3);
            guna2Panel3.CustomizableEdges = customizableEdges10;
            guna2Panel3.Dock = DockStyle.Fill;
            guna2Panel3.FillColor = Color.White;
            guna2Panel3.Location = new Point(758, 10);
            guna2Panel3.Margin = new Padding(10);
            guna2Panel3.Name = "guna2Panel3";
            guna2Panel3.ShadowDecoration.BorderRadius = 10;
            guna2Panel3.ShadowDecoration.Color = Color.FromArgb(230, 235, 245);
            guna2Panel3.ShadowDecoration.CustomizableEdges = customizableEdges11;
            guna2Panel3.ShadowDecoration.Enabled = true;
            guna2Panel3.Size = new Size(344, 105);
            guna2Panel3.TabIndex = 58;
            // 
            // lblExcellentRateValue
            // 
            lblExcellentRateValue.AutoSize = true;
            lblExcellentRateValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExcellentRateValue.ForeColor = Color.FromArgb(0, 90, 156);
            lblExcellentRateValue.Location = new Point(105, 14);
            lblExcellentRateValue.Name = "lblExcellentRateValue";
            lblExcellentRateValue.Size = new Size(42, 41);
            lblExcellentRateValue.TabIndex = 57;
            lblExcellentRateValue.Text = "...";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Gray;
            label6.Location = new Point(105, 60);
            label6.Name = "label6";
            label6.Size = new Size(191, 20);
            label6.TabIndex = 56;
            label6.Text = "TỶ LỆ SINH VIÊN XUẤT XẮC";
            // 
            // guna2CircleButton3
            // 
            guna2CircleButton3.DisabledState.BorderColor = Color.DarkGray;
            guna2CircleButton3.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2CircleButton3.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2CircleButton3.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2CircleButton3.FillColor = Color.FromArgb(235, 243, 255);
            guna2CircleButton3.Font = new Font("Segoe UI", 9F);
            guna2CircleButton3.ForeColor = Color.White;
            guna2CircleButton3.Location = new Point(20, 19);
            guna2CircleButton3.Name = "guna2CircleButton3";
            guna2CircleButton3.ShadowDecoration.CustomizableEdges = customizableEdges9;
            guna2CircleButton3.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CircleButton3.Size = new Size(65, 65);
            guna2CircleButton3.TabIndex = 55;
            guna2CircleButton3.Text = "";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(guna2Panel4, 1, 0);
            tableLayoutPanel1.Controls.Add(pnlChartContainer, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 240);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(24, 15, 24, 20);
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1136, 360);
            tableLayoutPanel1.TabIndex = 61;
            // 
            // pnlChartContainer
            // 
            pnlChartContainer.BackColor = Color.Transparent;
            pnlChartContainer.BorderRadius = 10;
            pnlChartContainer.Controls.Add(lblPhanBoHocLuc);
            pnlChartContainer.CustomizableEdges = customizableEdges12;
            pnlChartContainer.Dock = DockStyle.Fill;
            pnlChartContainer.FillColor = Color.White;
            pnlChartContainer.Location = new Point(34, 25);
            pnlChartContainer.Margin = new Padding(10);
            pnlChartContainer.Name = "pnlChartContainer";
            pnlChartContainer.ShadowDecoration.CustomizableEdges = customizableEdges13;
            pnlChartContainer.ShadowDecoration.Enabled = true;
            pnlChartContainer.Size = new Size(524, 295);
            pnlChartContainer.TabIndex = 59;
            // 
            // lblPhanBoHocLuc
            // 
            lblPhanBoHocLuc.AutoSize = true;
            lblPhanBoHocLuc.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPhanBoHocLuc.ForeColor = Color.FromArgb(26, 115, 232);
            lblPhanBoHocLuc.Location = new Point(16, 16);
            lblPhanBoHocLuc.Name = "lblPhanBoHocLuc";
            lblPhanBoHocLuc.Size = new Size(215, 20);
            lblPhanBoHocLuc.TabIndex = 43;
            lblPhanBoHocLuc.Text = "PHÂN BỐ XẾP LOẠI HỌC LỰC";
            // 
            // guna2Panel4
            // 
            guna2Panel4.BackColor = Color.Transparent;
            guna2Panel4.BorderRadius = 10;
            guna2Panel4.Controls.Add(dgvTopRanking);
            guna2Panel4.Controls.Add(label3);
            guna2Panel4.CustomizableEdges = customizableEdges14;
            guna2Panel4.Dock = DockStyle.Fill;
            guna2Panel4.FillColor = Color.White;
            guna2Panel4.Location = new Point(578, 25);
            guna2Panel4.Margin = new Padding(10);
            guna2Panel4.Name = "guna2Panel4";
            guna2Panel4.ShadowDecoration.CustomizableEdges = customizableEdges15;
            guna2Panel4.ShadowDecoration.Enabled = true;
            guna2Panel4.Size = new Size(524, 295);
            guna2Panel4.TabIndex = 60;
            // 
            // dgvTopRanking
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvTopRanking.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvTopRanking.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvTopRanking.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvTopRanking.ColumnHeadersHeight = 30;
            dgvTopRanking.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvTopRanking.DefaultCellStyle = dataGridViewCellStyle3;
            dgvTopRanking.GridColor = Color.FromArgb(231, 229, 255);
            dgvTopRanking.Location = new Point(16, 48);
            dgvTopRanking.Name = "dgvTopRanking";
            dgvTopRanking.RowHeadersVisible = false;
            dgvTopRanking.RowHeadersWidth = 51;
            dgvTopRanking.Size = new Size(492, 230);
            dgvTopRanking.TabIndex = 44;
            dgvTopRanking.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvTopRanking.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvTopRanking.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvTopRanking.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvTopRanking.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvTopRanking.ThemeStyle.BackColor = Color.White;
            dgvTopRanking.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvTopRanking.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvTopRanking.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTopRanking.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvTopRanking.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvTopRanking.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvTopRanking.ThemeStyle.HeaderStyle.Height = 30;
            dgvTopRanking.ThemeStyle.ReadOnly = false;
            dgvTopRanking.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvTopRanking.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTopRanking.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvTopRanking.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvTopRanking.ThemeStyle.RowsStyle.Height = 29;
            dgvTopRanking.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvTopRanking.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(26, 115, 232);
            label3.Location = new Point(16, 16);
            label3.Name = "label3";
            label3.Size = new Size(278, 20);
            label3.TabIndex = 43;
            label3.Text = "TOP 10 SINH VIÊN CÓ GPA CAO NHẤT";
            // 
            // StatisticsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1136, 600);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(tblCardsLayout);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(800, 550);
            Name = "StatisticsForm";
            Text = "StatisticsForm";
            Load += StatisticsForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tblCardsLayout.ResumeLayout(false);
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            guna2Panel3.ResumeLayout(false);
            guna2Panel3.PerformLayout();
            pnlChartContainer.ResumeLayout(false);
            pnlChartContainer.PerformLayout();
            guna2Panel4.ResumeLayout(false);
            guna2Panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopRanking).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // Giữ nguyên các biến cũ mà không thay đổi logic
        private Label lblTitle;
        private Label label1;
        private Guna.UI2.WinForms.Guna2Button txtLamMoi;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        private Label lblTotalStudentsValue;
        private Label label2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Label lblAvgGPAValue;
        private Label label4;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Label lblExcellentRateValue;
        private Label label6;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton3;
        private Guna.UI2.WinForms.Guna2Panel pnlChartContainer;
        private Label lblPhanBoHocLuc;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2DataGridView dgvTopRanking;
        private Label label3;
        private TableLayoutPanel tableLayoutPanel1;

        // Bổ sung các Panel Layout mới (Không ảnh hưởng đến logic của code cũ)
        private Panel pnlHeader;
        private TableLayoutPanel tblCardsLayout;
    }
}