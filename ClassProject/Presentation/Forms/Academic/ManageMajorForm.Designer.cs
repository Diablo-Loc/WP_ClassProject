namespace ClassProject.Presentation.Forms.Admin
{
    partial class ManageMajorForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlLeftContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.btnClear = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.txtMajorName = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTitleCode = new System.Windows.Forms.Label();
            this.txtMajorCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblHeaderLeft = new System.Windows.Forms.Label();
            this.pnlRightContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTotalMajors = new System.Windows.Forms.Label();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.dgvMajors = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlLeftContainer.SuspendLayout();
            this.pnlRightContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMajors)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLeftContainer
            // 
            this.pnlLeftContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlLeftContainer.Controls.Add(this.btnClear);
            this.pnlLeftContainer.Controls.Add(this.btnDelete);
            this.pnlLeftContainer.Controls.Add(this.btnSave);
            this.pnlLeftContainer.Controls.Add(this.lblTitleName);
            this.pnlLeftContainer.Controls.Add(this.txtMajorName);
            this.pnlLeftContainer.Controls.Add(this.lblTitleCode);
            this.pnlLeftContainer.Controls.Add(this.txtMajorCode);
            this.pnlLeftContainer.Controls.Add(this.lblHeaderLeft);
            this.pnlLeftContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeftContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftContainer.Name = "pnlLeftContainer";
            this.pnlLeftContainer.Size = new System.Drawing.Size(340, 600);
            this.pnlLeftContainer.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.BorderRadius = 6;
            this.btnClear.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClear.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClear.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClear.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnClear.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(25, 345);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(290, 40);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "🔄 Làm Mới";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BorderRadius = 6;
            this.btnDelete.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.btnDelete.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(29)))), ((int)(((byte)(72)))));
            this.btnDelete.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(25, 295);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(290, 40);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "❌ Xóa Ngành";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 6;
            this.btnSave.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSave.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(124)))), ((int)(((byte)(65)))));
            this.btnSave.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(25, 245);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(290, 40);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "(+) Thêm Ngành Học";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9F);
            this.lblTitleName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTitleName.Location = new System.Drawing.Point(22, 160);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(94, 15);
            this.lblTitleName.TabIndex = 4;
            this.lblTitleName.Text = "Tên Ngành Học *";
            // 
            // txtMajorName
            // 
            this.txtMajorName.BorderRadius = 4;
            this.txtMajorName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMajorName.DefaultText = "";
            this.txtMajorName.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMajorName.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.txtMajorName.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMajorName.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMajorName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMajorName.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.txtMajorName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtMajorName.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.txtMajorName.Location = new System.Drawing.Point(25, 180);
            this.txtMajorName.Name = "txtMajorName";
            this.txtMajorName.PasswordChar = '\0';
            this.txtMajorName.PlaceholderText = "Ví dụ: Công nghệ thông tin";
            this.txtMajorName.SelectedText = "";
            this.txtMajorName.Size = new System.Drawing.Size(290, 36);
            this.txtMajorName.TabIndex = 3;
            // 
            // lblTitleCode
            // 
            this.lblTitleCode.AutoSize = true;
            this.lblTitleCode.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9F);
            this.lblTitleCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblTitleCode.Location = new System.Drawing.Point(22, 85);
            this.lblTitleCode.Name = "lblTitleCode";
            this.lblTitleCode.Size = new System.Drawing.Size(71, 15);
            this.lblTitleCode.TabIndex = 2;
            this.lblTitleCode.Text = "Mã Ngành *";
            // 
            // txtMajorCode
            // 
            this.txtMajorCode.BorderRadius = 4;
            this.txtMajorCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMajorCode.DefaultText = "";
            this.txtMajorCode.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMajorCode.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.txtMajorCode.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.txtMajorCode.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMajorCode.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMajorCode.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.txtMajorCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtMajorCode.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.txtMajorCode.Location = new System.Drawing.Point(25, 105);
            this.txtMajorCode.Name = "txtMajorCode";
            this.txtMajorCode.PasswordChar = '\0';
            this.txtMajorCode.PlaceholderText = "Ví dụ: CNTT";
            this.txtMajorCode.SelectedText = "";
            this.txtMajorCode.Size = new System.Drawing.Size(290, 36);
            this.txtMajorCode.TabIndex = 1;
            // 
            // lblHeaderLeft
            // 
            this.lblHeaderLeft.AutoSize = true;
            this.lblHeaderLeft.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 13F, System.Drawing.FontStyle.Bold);
            this.lblHeaderLeft.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblHeaderLeft.Location = new System.Drawing.Point(20, 25);
            this.lblHeaderLeft.Name = "lblHeaderLeft";
            this.lblHeaderLeft.Size = new System.Drawing.Size(188, 25);
            this.lblHeaderLeft.TabIndex = 0;
            this.lblHeaderLeft.Text = "Thông Tin Ngành Học";
            // 
            // pnlRightContainer
            // 
            this.pnlRightContainer.BackColor = System.Drawing.Color.White;
            this.pnlRightContainer.Controls.Add(this.lblTotalMajors);
            this.pnlRightContainer.Controls.Add(this.txtSearch);
            this.pnlRightContainer.Controls.Add(this.dgvMajors);
            this.pnlRightContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightContainer.Location = new System.Drawing.Point(340, 0);
            this.pnlRightContainer.Name = "pnlRightContainer";
            this.pnlRightContainer.Padding = new System.Windows.Forms.Padding(20, 25, 20, 20);
            this.pnlRightContainer.Size = new System.Drawing.Size(660, 600);
            this.pnlRightContainer.TabIndex = 1;
            // 
            // lblTotalMajors
            // 
            this.lblTotalMajors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalMajors.AutoSize = true;
            this.lblTotalMajors.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9F, System.Drawing.FontStyle.Italic);
            this.lblTotalMajors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTotalMajors.Location = new System.Drawing.Point(20, 565);
            this.lblTotalMajors.Name = "lblTotalMajors";
            this.lblTotalMajors.Size = new System.Drawing.Size(127, 15);
            this.lblTotalMajors.TabIndex = 2;
            this.lblTotalMajors.Text = "Tổng số ngành học: 0";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BorderRadius = 4;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSearch.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.txtSearch.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtSearch.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.txtSearch.Location = new System.Drawing.Point(20, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.PlaceholderText = "🔍 Tìm kiếm theo mã hoặc tên ngành học...";
            this.txtSearch.SelectedText = "";
            this.txtSearch.Size = new System.Drawing.Size(620, 36);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // dgvMajors
            // 
            this.dgvMajors.AllowUserToAddRows = false;
            this.dgvMajors.AllowUserToDeleteRows = false;
            this.dgvMajors.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvMajors.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMajors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMajors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMajors.ColumnHeadersHeight = 35;
            this.dgvMajors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMajors.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMajors.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvMajors.Location = new System.Drawing.Point(20, 80);
            this.dgvMajors.MultiSelect = false;
            this.dgvMajors.Name = "dgvMajors";
            this.dgvMajors.ReadOnly = true;
            this.dgvMajors.RowHeadersVisible = false;
            this.dgvMajors.RowTemplate.Height = 30;
            this.dgvMajors.Size = NavSize;
            this.dgvMajors.TabIndex = 0;
            this.dgvMajors.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvMajors.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvMajors.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvMajors.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvMajors.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvMajors.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvMajors.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvMajors.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.dgvMajors.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F, System.Drawing.FontStyle.Bold);
            this.dgvMajors.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvMajors.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvMajors.ThemeStyle.HeaderStyle.Height = 35;
            this.dgvMajors.ThemeStyle.ReadOnly = true;
            this.dgvMajors.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvMajors.ThemeStyle.RowsStyle.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9.5F);
            this.dgvMajors.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.dgvMajors.ThemeStyle.RowsStyle.Height = 30;
            this.dgvMajors.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dgvMajors.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.dgvMajors.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMajors_CellClick);
            // 
            // ManageMajorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.pnlRightContainer);
            this.Controls.Add(this.pnlLeftContainer);
            this.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Segoe UI"), 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManageMajorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Ngành Học";
            this.Load += new System.EventHandler(this.ManageMajorForm_Load);
            this.pnlLeftContainer.ResumeLayout(false);
            this.pnlLeftContainer.PerformLayout();
            this.pnlRightContainer.ResumeLayout(false);
            this.pnlRightContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMajors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlLeftContainer;
        private Guna.UI2.WinForms.Guna2Panel pnlRightContainer;
        private System.Windows.Forms.Label lblHeaderLeft;
        private System.Windows.Forms.Label lblTitleCode;
        private Guna.UI2.WinForms.Guna2TextBox txtMajorCode;
        private System.Windows.Forms.Label lblTitleName;
        private Guna.UI2.WinForms.Guna2TextBox txtMajorName;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnClear;
        private Guna.UI2.WinForms.Guna2DataGridView dgvMajors;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private System.Windows.Forms.Label lblTotalMajors;

        // Định vị kích thước DataGridView động chống vỡ khung hình
        private System.Drawing.Size NavSize { get { return new System.Drawing.Size(620, 465); } }
    }
}