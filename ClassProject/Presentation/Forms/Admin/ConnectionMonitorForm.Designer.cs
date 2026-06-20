namespace ClassProject.Presentation.Forms.Admin
{
    partial class ConnectionMonitorForm
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblTitle = new Label();
            pnlCardsContainer = new TableLayoutPanel();
            pnlCardActive = new Panel();
            lblActiveCount = new Label();
            lblActiveTitle = new Label();
            pnlCardLeak = new Panel();
            lblLeakCount = new Label();
            lblLeakTitle = new Label();
            pnlMainLayout = new TableLayoutPanel();
            dgvLeaks = new DataGridView();
            pnlAiReport = new Panel();
            txtAiRecommendation = new RichTextBox();
            pnlSimulatorButtons = new Panel();
            btnSimulateLeak = new Button();
            btnSimulateSafe = new Button();
            lblAiHeader = new Label();
            tmrScan = new System.Windows.Forms.Timer(components);
            pnlHeader.SuspendLayout();
            pnlCardsContainer.SuspendLayout();
            pnlCardActive.SuspendLayout();
            pnlCardLeak.SuspendLayout();
            pnlMainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLeaks).BeginInit();
            pnlAiReport.SuspendLayout();
            pnlSimulatorButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(26, 32, 46);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(3, 4, 3, 4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1143, 80);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 210, 255);
            lblTitle.Location = new Point(18, 21);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(498, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🤖 AI CONNECTION OPERATIONS CENTER";
            // 
            // pnlCardsContainer
            // 
            pnlCardsContainer.ColumnCount = 2;
            pnlCardsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlCardsContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlCardsContainer.Controls.Add(pnlCardActive, 0, 0);
            pnlCardsContainer.Controls.Add(pnlCardLeak, 1, 0);
            pnlCardsContainer.Dock = DockStyle.Top;
            pnlCardsContainer.Location = new Point(0, 80);
            pnlCardsContainer.Margin = new Padding(3, 4, 3, 4);
            pnlCardsContainer.Name = "pnlCardsContainer";
            pnlCardsContainer.Padding = new Padding(11, 13, 11, 13);
            pnlCardsContainer.RowCount = 1;
            pnlCardsContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlCardsContainer.Size = new Size(1143, 133);
            pnlCardsContainer.TabIndex = 1;
            // 
            // pnlCardActive
            // 
            pnlCardActive.BackColor = Color.FromArgb(33, 41, 54);
            pnlCardActive.BorderStyle = BorderStyle.FixedSingle;
            pnlCardActive.Controls.Add(lblActiveCount);
            pnlCardActive.Controls.Add(lblActiveTitle);
            pnlCardActive.Dock = DockStyle.Fill;
            pnlCardActive.Location = new Point(17, 20);
            pnlCardActive.Margin = new Padding(6, 7, 6, 7);
            pnlCardActive.Name = "pnlCardActive";
            pnlCardActive.Size = new Size(548, 93);
            pnlCardActive.TabIndex = 0;
            // 
            // lblActiveCount
            // 
            lblActiveCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblActiveCount.ForeColor = Color.FromArgb(46, 204, 113);
            lblActiveCount.Location = new Point(183, 0);
            lblActiveCount.Name = "lblActiveCount";
            lblActiveCount.Size = new Size(337, 91);
            lblActiveCount.TabIndex = 0;
            lblActiveCount.Text = "0";
            lblActiveCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblActiveTitle
            // 
            lblActiveTitle.AutoSize = true;
            lblActiveTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblActiveTitle.ForeColor = Color.DarkGray;
            lblActiveTitle.Location = new Point(17, 33);
            lblActiveTitle.Name = "lblActiveTitle";
            lblActiveTitle.Size = new Size(165, 23);
            lblActiveTitle.TabIndex = 1;
            lblActiveTitle.Text = "KẾT NỐI ĐANG MỞ";
            // 
            // pnlCardLeak
            // 
            pnlCardLeak.BackColor = Color.FromArgb(33, 41, 54);
            pnlCardLeak.BorderStyle = BorderStyle.FixedSingle;
            pnlCardLeak.Controls.Add(lblLeakCount);
            pnlCardLeak.Controls.Add(lblLeakTitle);
            pnlCardLeak.Dock = DockStyle.Fill;
            pnlCardLeak.Location = new Point(577, 20);
            pnlCardLeak.Margin = new Padding(6, 7, 6, 7);
            pnlCardLeak.Name = "pnlCardLeak";
            pnlCardLeak.Size = new Size(549, 93);
            pnlCardLeak.TabIndex = 1;
            // 
            // lblLeakCount
            // 
            lblLeakCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblLeakCount.ForeColor = Color.FromArgb(231, 76, 60);
            lblLeakCount.Location = new Point(194, 0);
            lblLeakCount.Name = "lblLeakCount";
            lblLeakCount.Size = new Size(326, 91);
            lblLeakCount.TabIndex = 0;
            lblLeakCount.Text = "0";
            lblLeakCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblLeakTitle
            // 
            lblLeakTitle.AutoSize = true;
            lblLeakTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLeakTitle.ForeColor = Color.DarkGray;
            lblLeakTitle.Location = new Point(17, 33);
            lblLeakTitle.Name = "lblLeakTitle";
            lblLeakTitle.Size = new Size(187, 23);
            lblLeakTitle.TabIndex = 1;
            lblLeakTitle.Text = "SỰ CỐ KHÓA KẾT NỐI";
            // 
            // pnlMainLayout
            // 
            pnlMainLayout.ColumnCount = 2;
            pnlMainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            pnlMainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            pnlMainLayout.Controls.Add(dgvLeaks, 0, 0);
            pnlMainLayout.Controls.Add(pnlAiReport, 1, 0);
            pnlMainLayout.Dock = DockStyle.Fill;
            pnlMainLayout.Location = new Point(0, 213);
            pnlMainLayout.Margin = new Padding(3, 4, 3, 4);
            pnlMainLayout.Name = "pnlMainLayout";
            pnlMainLayout.Padding = new Padding(11, 13, 11, 13);
            pnlMainLayout.RowCount = 1;
            pnlMainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlMainLayout.Size = new Size(1143, 587);
            pnlMainLayout.TabIndex = 2;
            // 
            // dgvLeaks
            // 
            this.dgvLeaks.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(46)))));
            this.dgvLeaks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLeaks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvLeaks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            // Cấu hình Header (Tiêu đề cột) - Chữ xanh neon nổi bật
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(41)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(41)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeaks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLeaks.ColumnHeadersHeight = 35;

            // Cấu hình các ô dữ liệu mặc định - Đổi ForeColor thành WHITE (Trắng tinh) để cực kỳ dễ đọc
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(46)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White; // 🌟 SỬA TỪ LightGray THÀNH White để không bị chìm
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLeaks.DefaultCellStyle = dataGridViewCellStyle2;

            this.dgvLeaks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLeaks.EnableHeadersVisualStyles = false;
            this.dgvLeaks.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(48)))), ((int)(((byte)(66)))));
            this.dgvLeaks.Location = new System.Drawing.Point(17, 20);
            this.dgvLeaks.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.dgvLeaks.Name = "dgvLeaks";
            this.dgvLeaks.ReadOnly = true;
            this.dgvLeaks.RowHeadersVisible = false;
            this.dgvLeaks.RowHeadersWidth = 51;

            // Đảm bảo các dòng mặc định ăn theo màu nền tối và chữ trắng tinh
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(46)))));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White; // 🌟 Thêm dòng này để ép chữ luôn màu trắng
            this.dgvLeaks.RowsDefaultCellStyle = dataGridViewCellStyle3;

            this.dgvLeaks.RowTemplate.Height = 30;
            this.dgvLeaks.Size = new System.Drawing.Size(604, 547);
            this.dgvLeaks.TabIndex = 0;
            // 
            // pnlAiReport
            // 
            pnlAiReport.BackColor = Color.FromArgb(33, 41, 54);
            pnlAiReport.BorderStyle = BorderStyle.FixedSingle;
            pnlAiReport.Controls.Add(txtAiRecommendation);
            pnlAiReport.Controls.Add(pnlSimulatorButtons);
            pnlAiReport.Controls.Add(lblAiHeader);
            pnlAiReport.Dock = DockStyle.Fill;
            pnlAiReport.Location = new Point(633, 20);
            pnlAiReport.Margin = new Padding(6, 7, 6, 7);
            pnlAiReport.Name = "pnlAiReport";
            pnlAiReport.Padding = new Padding(11, 13, 11, 13);
            pnlAiReport.Size = new Size(493, 547);
            pnlAiReport.TabIndex = 1;
            // 
            // txtAiRecommendation
            // 
            txtAiRecommendation.BackColor = Color.FromArgb(21, 27, 38);
            txtAiRecommendation.BorderStyle = BorderStyle.None;
            txtAiRecommendation.Dock = DockStyle.Fill;
            txtAiRecommendation.Font = new Font("Consolas", 10F);
            txtAiRecommendation.ForeColor = Color.FromArgb(241, 196, 15);
            txtAiRecommendation.Location = new Point(11, 53);
            txtAiRecommendation.Margin = new Padding(3, 4, 3, 4);
            txtAiRecommendation.Name = "txtAiRecommendation";
            txtAiRecommendation.ReadOnly = true;
            txtAiRecommendation.Size = new Size(469, 418);
            txtAiRecommendation.TabIndex = 0;
            txtAiRecommendation.Text = "// Hãy chọn một dòng rò rỉ bên trái để AI tiến hành phân tích mã nguồn...";
            // 
            // pnlSimulatorButtons
            // 
            pnlSimulatorButtons.Controls.Add(btnSimulateLeak);
            pnlSimulatorButtons.Controls.Add(btnSimulateSafe);
            pnlSimulatorButtons.Dock = DockStyle.Bottom;
            pnlSimulatorButtons.Location = new Point(11, 471);
            pnlSimulatorButtons.Name = "pnlSimulatorButtons";
            pnlSimulatorButtons.Size = new Size(469, 61);
            pnlSimulatorButtons.TabIndex = 2;
            // 
            // btnSimulateLeak
            // 
            btnSimulateLeak.BackColor = Color.FromArgb(231, 76, 60);
            btnSimulateLeak.FlatStyle = FlatStyle.Flat;
            btnSimulateLeak.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSimulateLeak.ForeColor = Color.White;
            btnSimulateLeak.Location = new Point(205, 15);
            btnSimulateLeak.Name = "btnSimulateLeak";
            btnSimulateLeak.Size = new Size(190, 35);
            btnSimulateLeak.TabIndex = 1;
            btnSimulateLeak.Text = "⚠️ Thử Kích Hoạt Leak";
            btnSimulateLeak.UseVisualStyleBackColor = false;
            btnSimulateLeak.Click += btnSimulateLeak_Click;
            // 
            // btnSimulateSafe
            // 
            btnSimulateSafe.BackColor = Color.FromArgb(46, 204, 113);
            btnSimulateSafe.FlatStyle = FlatStyle.Flat;
            btnSimulateSafe.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSimulateSafe.ForeColor = Color.White;
            btnSimulateSafe.Location = new Point(0, 15);
            btnSimulateSafe.Name = "btnSimulateSafe";
            btnSimulateSafe.Size = new Size(190, 35);
            btnSimulateSafe.TabIndex = 0;
            btnSimulateSafe.Text = "\U0001f7e2 Thử Kết Nối An Toàn";
            btnSimulateSafe.UseVisualStyleBackColor = false;
            btnSimulateSafe.Click += btnSimulateSafe_Click;
            // 
            // lblAiHeader
            // 
            lblAiHeader.Dock = DockStyle.Top;
            lblAiHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAiHeader.ForeColor = Color.White;
            lblAiHeader.Location = new Point(11, 13);
            lblAiHeader.Name = "lblAiHeader";
            lblAiHeader.Size = new Size(469, 40);
            lblAiHeader.TabIndex = 1;
            lblAiHeader.Text = "\U0001f9e0 CHẨN ĐOÁN && KHUYẾN NGHỊ SỬA CODE TỪ AI";
            // 
            // tmrScan
            // 
            tmrScan.Enabled = true;
            tmrScan.Interval = 1000;
            tmrScan.Tick += tmrScan_Tick;
            // 
            // ConnectionMonitorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(21, 27, 38);
            ClientSize = new Size(1143, 800);
            Controls.Add(pnlMainLayout);
            Controls.Add(pnlCardsContainer);
            Controls.Add(pnlHeader);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1026, 718);
            Name = "ConnectionMonitorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI Connection Monitor";
            FormClosing += ConnectionMonitorForm_FormClosing;
            Load += ConnectionMonitorForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlCardsContainer.ResumeLayout(false);
            pnlCardActive.ResumeLayout(false);
            pnlCardActive.PerformLayout();
            pnlCardLeak.ResumeLayout(false);
            pnlCardLeak.PerformLayout();
            pnlMainLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLeaks).EndInit();
            pnlAiReport.ResumeLayout(false);
            pnlSimulatorButtons.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel pnlCardsContainer;
        private System.Windows.Forms.Panel pnlCardActive;
        private System.Windows.Forms.Label lblActiveCount;
        private System.Windows.Forms.Label lblActiveTitle;
        private System.Windows.Forms.Panel pnlCardLeak;
        private System.Windows.Forms.Label lblLeakCount;
        private System.Windows.Forms.Label lblLeakTitle;
        private System.Windows.Forms.TableLayoutPanel pnlMainLayout;
        private System.Windows.Forms.DataGridView dgvLeaks;
        private System.Windows.Forms.Panel pnlAiReport;
        private System.Windows.Forms.RichTextBox txtAiRecommendation;
        private System.Windows.Forms.Label lblAiHeader;
        private System.Windows.Forms.Timer tmrScan;
        private System.Windows.Forms.Panel pnlSimulatorButtons;
        private System.Windows.Forms.Button btnSimulateSafe;
        private System.Windows.Forms.Button btnSimulateLeak;
    }
}