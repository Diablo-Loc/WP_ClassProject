namespace ClassProject.Presentation.Forms.Main
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            studentToolStripMenuItem = new ToolStripMenuItem();
            addStudentToolStripMenuItem = new ToolStripMenuItem();
            listStudentToolStripMenuItem = new ToolStripMenuItem();
            adminToolStripMenuItem = new ToolStripMenuItem();
            pheDuyetYeuCauToolStripMenuItem = new ToolStripMenuItem();
            guiYeuCauHoTroToolStripMenuItem = new ToolStripMenuItem();
            đăngKýMônToolStripMenuItem = new ToolStripMenuItem();
            quảnLýĐiểmToolStripMenuItem = new ToolStripMenuItem();
            quảnLýLớpToolStripMenuItem = new ToolStripMenuItem();
            pnlTotal = new Panel();
            lblTotalStudents = new Label();
            pnlMale = new Panel();
            lblMaleStudents = new Label();
            pnlFemale = new Panel();
            lblFemaleStudents = new Label();
            lblRole = new Label();
            picChart = new PictureBox();
            dgvPendingUsers = new DataGridView();
            colAccept = new DataGridViewButtonColumn();
            colDelete = new DataGridViewButtonColumn();
            txtSearchPending = new TextBox();
            btnBulkAccept = new Button();
            btnBulkDelete = new Button();
            chkSelectAll = new CheckBox();
            menuStrip1.SuspendLayout();
            pnlTotal.SuspendLayout();
            pnlMale.SuspendLayout();
            pnlFemale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picChart).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { studentToolStripMenuItem, adminToolStripMenuItem, pheDuyetYeuCauToolStripMenuItem, guiYeuCauHoTroToolStripMenuItem, đăngKýMônToolStripMenuItem, quảnLýĐiểmToolStripMenuItem, quảnLýLớpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(875, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // studentToolStripMenuItem
            // 
            studentToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addStudentToolStripMenuItem, listStudentToolStripMenuItem });
            studentToolStripMenuItem.Name = "studentToolStripMenuItem";
            studentToolStripMenuItem.Size = new Size(74, 24);
            studentToolStripMenuItem.Text = "Student";
            // 
            // addStudentToolStripMenuItem
            // 
            addStudentToolStripMenuItem.Name = "addStudentToolStripMenuItem";
            addStudentToolStripMenuItem.Size = new Size(175, 26);
            addStudentToolStripMenuItem.Text = "Add Student";
            addStudentToolStripMenuItem.Click += addStudentToolStripMenuItem_Click;
            // 
            // listStudentToolStripMenuItem
            // 
            listStudentToolStripMenuItem.Name = "listStudentToolStripMenuItem";
            listStudentToolStripMenuItem.Size = new Size(175, 26);
            listStudentToolStripMenuItem.Text = "List Student";
            listStudentToolStripMenuItem.Click += listStudentToolStripMenuItem_Click;
            // 
            // adminToolStripMenuItem
            // 
            adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            adminToolStripMenuItem.Size = new Size(67, 24);
            adminToolStripMenuItem.Text = "Admin";
            // 
            // pheDuyetYeuCauToolStripMenuItem
            // 
            pheDuyetYeuCauToolStripMenuItem.Name = "pheDuyetYeuCauToolStripMenuItem";
            pheDuyetYeuCauToolStripMenuItem.Size = new Size(146, 24);
            pheDuyetYeuCauToolStripMenuItem.Text = "Phê Duyệt Yêu Cầu";
            pheDuyetYeuCauToolStripMenuItem.Click += pheDuyetYeuCauToolStripMenuItem_Click;
            // 
            // guiYeuCauHoTroToolStripMenuItem
            // 
            guiYeuCauHoTroToolStripMenuItem.BackgroundImageLayout = ImageLayout.Stretch;
            guiYeuCauHoTroToolStripMenuItem.Name = "guiYeuCauHoTroToolStripMenuItem";
            guiYeuCauHoTroToolStripMenuItem.Size = new Size(151, 24);
            guiYeuCauHoTroToolStripMenuItem.Text = "Gửi Yêu Cầu Hỗ Trợ";
            guiYeuCauHoTroToolStripMenuItem.Click += guiYeuCauHoTroToolStripMenuItem_Click;
            // 
            // đăngKýMônToolStripMenuItem
            // 
            đăngKýMônToolStripMenuItem.Name = "đăngKýMônToolStripMenuItem";
            đăngKýMônToolStripMenuItem.Size = new Size(111, 24);
            đăngKýMônToolStripMenuItem.Text = "Đăng ký môn";
            đăngKýMônToolStripMenuItem.Click += đăngKýMônToolStripMenuItem_Click;
            // 
            // quảnLýĐiểmToolStripMenuItem
            // 
            quảnLýĐiểmToolStripMenuItem.Name = "quảnLýĐiểmToolStripMenuItem";
            quảnLýĐiểmToolStripMenuItem.Size = new Size(111, 24);
            quảnLýĐiểmToolStripMenuItem.Text = "Quản lý điểm";
            quảnLýĐiểmToolStripMenuItem.Click += quảnLýĐiểmToolStripMenuItem_Click;
            // 
            // quảnLýLớpToolStripMenuItem
            // 
            quảnLýLớpToolStripMenuItem.Name = "quảnLýLớpToolStripMenuItem";
            quảnLýLớpToolStripMenuItem.Size = new Size(104, 24);
            quảnLýLớpToolStripMenuItem.Text = "Quản Lý Lớp";
            quảnLýLớpToolStripMenuItem.Click += quảnLýLớpToolStripMenuItem_Click;
            // 
            // pnlTotal
            // 
            pnlTotal.BackColor = Color.FromArgb(192, 255, 192);
            pnlTotal.Controls.Add(lblTotalStudents);
            pnlTotal.Location = new Point(12, 38);
            pnlTotal.Name = "pnlTotal";
            pnlTotal.Size = new Size(250, 105);
            pnlTotal.TabIndex = 1;
            // 
            // lblTotalStudents
            // 
            lblTotalStudents.AutoSize = true;
            lblTotalStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTotalStudents.Location = new Point(22, 37);
            lblTotalStudents.Name = "lblTotalStudents";
            lblTotalStudents.Size = new Size(152, 31);
            lblTotalStudents.TabIndex = 4;
            lblTotalStudents.Text = "TotalStudents";
            // 
            // pnlMale
            // 
            pnlMale.Anchor = AnchorStyles.Top;
            pnlMale.BackColor = Color.Cyan;
            pnlMale.Controls.Add(lblMaleStudents);
            pnlMale.Location = new Point(312, 40);
            pnlMale.Name = "pnlMale";
            pnlMale.Size = new Size(250, 105);
            pnlMale.TabIndex = 2;
            // 
            // lblMaleStudents
            // 
            lblMaleStudents.AutoSize = true;
            lblMaleStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblMaleStudents.Location = new Point(30, 37);
            lblMaleStudents.Name = "lblMaleStudents";
            lblMaleStudents.Size = new Size(154, 31);
            lblMaleStudents.TabIndex = 5;
            lblMaleStudents.Text = "MaleStudents";
            // 
            // pnlFemale
            // 
            pnlFemale.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlFemale.BackColor = Color.FromArgb(255, 192, 255);
            pnlFemale.Controls.Add(lblFemaleStudents);
            pnlFemale.Location = new Point(618, 40);
            pnlFemale.Name = "pnlFemale";
            pnlFemale.Size = new Size(250, 105);
            pnlFemale.TabIndex = 3;
            // 
            // lblFemaleStudents
            // 
            lblFemaleStudents.AutoSize = true;
            lblFemaleStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFemaleStudents.Location = new Point(27, 37);
            lblFemaleStudents.Name = "lblFemaleStudents";
            lblFemaleStudents.Size = new Size(176, 31);
            lblFemaleStudents.TabIndex = 6;
            lblFemaleStudents.Text = "FemaleStudents";
            // 
            // lblRole
            // 
            lblRole.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblRole.AutoSize = true;
            lblRole.Location = new Point(782, 771);
            lblRole.Name = "lblRole";
            lblRole.RightToLeft = RightToLeft.No;
            lblRole.Size = new Size(39, 20);
            lblRole.TabIndex = 4;
            lblRole.Text = "Role";
            lblRole.TextAlign = ContentAlignment.MiddleRight;
            // 
            // picChart
            // 
            picChart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picChart.Location = new Point(137, 153);
            picChart.Name = "picChart";
            picChart.Size = new Size(643, 608);
            picChart.TabIndex = 5;
            picChart.TabStop = false;
            // 
            // dgvPendingUsers
            // 
            dgvPendingUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPendingUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPendingUsers.Columns.AddRange(new DataGridViewColumn[] { colAccept, colDelete });
            dgvPendingUsers.Location = new Point(12, 186);
            dgvPendingUsers.Name = "dgvPendingUsers";
            dgvPendingUsers.RowHeadersWidth = 51;
            dgvPendingUsers.Size = new Size(851, 575);
            dgvPendingUsers.TabIndex = 6;
            // 
            // colAccept
            // 
            colAccept.HeaderText = "Accept";
            colAccept.MinimumWidth = 6;
            colAccept.Name = "colAccept";
            colAccept.Resizable = DataGridViewTriState.True;
            colAccept.SortMode = DataGridViewColumnSortMode.Automatic;
            colAccept.Text = "✔";
            colAccept.UseColumnTextForButtonValue = true;
            colAccept.Width = 125;
            // 
            // colDelete
            // 
            colDelete.HeaderText = "Delete";
            colDelete.MinimumWidth = 6;
            colDelete.Name = "colDelete";
            colDelete.Text = "✖";
            colDelete.UseColumnTextForButtonValue = true;
            colDelete.Width = 125;
            // 
            // txtSearchPending
            // 
            txtSearchPending.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearchPending.Location = new Point(696, 153);
            txtSearchPending.Name = "txtSearchPending";
            txtSearchPending.PlaceholderText = "Nhập tìm kiếm...";
            txtSearchPending.Size = new Size(172, 27);
            txtSearchPending.TabIndex = 7;
            // 
            // btnBulkAccept
            // 
            btnBulkAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBulkAccept.Location = new Point(27, 767);
            btnBulkAccept.Name = "btnBulkAccept";
            btnBulkAccept.Size = new Size(205, 29);
            btnBulkAccept.TabIndex = 8;
            btnBulkAccept.Text = "✔ Duyệt các mục đã chọn";
            btnBulkAccept.UseVisualStyleBackColor = true;
            // 
            // btnBulkDelete
            // 
            btnBulkDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBulkDelete.Location = new Point(264, 767);
            btnBulkDelete.Name = "btnBulkDelete";
            btnBulkDelete.Size = new Size(205, 29);
            btnBulkDelete.TabIndex = 9;
            btnBulkDelete.Text = "✖ Từ chối các mục đã chọn";
            btnBulkDelete.UseVisualStyleBackColor = true;
            // 
            // chkSelectAll
            // 
            chkSelectAll.AutoSize = true;
            chkSelectAll.Location = new Point(20, 193);
            chkSelectAll.Name = "chkSelectAll";
            chkSelectAll.Size = new Size(18, 17);
            chkSelectAll.TabIndex = 10;
            chkSelectAll.UseVisualStyleBackColor = true;
            chkSelectAll.CheckedChanged += chkSelectAll_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(875, 801);
            Controls.Add(chkSelectAll);
            Controls.Add(btnBulkDelete);
            Controls.Add(btnBulkAccept);
            Controls.Add(txtSearchPending);
            Controls.Add(dgvPendingUsers);
            Controls.Add(picChart);
            Controls.Add(lblRole);
            Controls.Add(pnlFemale);
            Controls.Add(pnlMale);
            Controls.Add(pnlTotal);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            pnlTotal.ResumeLayout(false);
            pnlTotal.PerformLayout();
            pnlMale.ResumeLayout(false);
            pnlMale.PerformLayout();
            pnlFemale.ResumeLayout(false);
            pnlFemale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picChart).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem studentToolStripMenuItem;
        private ToolStripMenuItem listStudentToolStripMenuItem;
        private ToolStripMenuItem adminToolStripMenuItem;
        private ToolStripMenuItem addStudentToolStripMenuItem;
        private Panel pnlTotal;
        private Label lblTotalStudents;
        private Panel pnlMale;
        private Label lblMaleStudents;
        private Panel pnlFemale;
        private Label lblFemaleStudents;
        private Label lblRole;
        private PictureBox picChart;
        private DataGridView dgvPendingUsers;
        private DataGridViewButtonColumn colAccept;
        private DataGridViewButtonColumn colDelete;
        private TextBox txtSearchPending;
        private Button btnBulkAccept;
        private Button btnBulkDelete;
        private CheckBox chkSelectAll;
        private ToolStripMenuItem pheDuyetYeuCauToolStripMenuItem;
        private ToolStripMenuItem guiYeuCauHoTroToolStripMenuItem;
        private ToolStripMenuItem đăngKýMônToolStripMenuItem;
        private ToolStripMenuItem quảnLýĐiểmToolStripMenuItem;
        private ToolStripMenuItem quảnLýLớpToolStripMenuItem;
    }
}