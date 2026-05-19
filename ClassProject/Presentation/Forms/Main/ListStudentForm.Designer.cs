namespace ClassProject
{
    partial class ListStudentForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Button btnRefresh;
            txtDanhsachsinhvien = new Label();
            txtSearch = new TextBox();
            dgvStudents = new DataGridView();
            pnl = new Panel();
            btnExportExcel = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnInsert = new Button();
            cboFilterGender = new ComboBox();
            btnImportExcel = new Button();
            btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            pnl.SuspendLayout();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(793, 3);
            btnRefresh.Margin = new Padding(2, 3, 2, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(107, 43);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Làm mới";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtDanhsachsinhvien
            // 
            txtDanhsachsinhvien.AutoSize = true;
            txtDanhsachsinhvien.Cursor = Cursors.No;
            txtDanhsachsinhvien.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtDanhsachsinhvien.Location = new Point(21, 40);
            txtDanhsachsinhvien.Margin = new Padding(2, 0, 2, 0);
            txtDanhsachsinhvien.Name = "txtDanhsachsinhvien";
            txtDanhsachsinhvien.Size = new Size(400, 46);
            txtDanhsachsinhvien.TabIndex = 0;
            txtDanhsachsinhvien.Text = "DANH SÁCH SINH VIÊN";
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.ForeColor = SystemColors.WindowFrame;
            txtSearch.Location = new Point(447, 48);
            txtSearch.Margin = new Padding(2, 3, 2, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(330, 27);
            txtSearch.TabIndex = 2;
            txtSearch.Text = "Nhập mã SV, họ hoặc tên để tìm...";
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.Enter += txtSearch_Enter;
            txtSearch.Leave += txtSearch_Leave;
            // 
            // dgvStudents
            // 
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.AllowUserToDeleteRows = false;
            dgvStudents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.BackgroundColor = Color.White;
            dgvStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStudents.Location = new Point(0, 93);
            dgvStudents.Margin = new Padding(2, 3, 2, 3);
            dgvStudents.Name = "dgvStudents";
            dgvStudents.ReadOnly = true;
            dgvStudents.RowHeadersWidth = 62;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.Size = new Size(913, 448);
            dgvStudents.TabIndex = 3;
            // 
            // pnl
            // 
            pnl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnl.Controls.Add(btnImportExcel);
            pnl.Controls.Add(btnExportExcel);
            pnl.Controls.Add(btnUpdate);
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh);
            pnl.Controls.Add(btnInsert);
            pnl.Location = new Point(0, 559);
            pnl.Margin = new Padding(2, 3, 2, 3);
            pnl.Name = "pnl";
            pnl.Size = new Size(913, 51);
            pnl.TabIndex = 4;
            pnl.Click += btnImportExcel_Click;
            // 
            // btnExportExcel
            // 
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.Location = new Point(674, 3);
            btnExportExcel.Margin = new Padding(2, 3, 2, 3);
            btnExportExcel.Name = "btnExportExcel";
            btnExportExcel.Size = new Size(107, 43);
            btnExportExcel.TabIndex = 4;
            btnExportExcel.Text = "ExportExcel";
            btnExportExcel.UseVisualStyleBackColor = true;
            btnExportExcel.Click += btnExportExcel_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Location = new Point(141, 3);
            btnUpdate.Margin = new Padding(2, 3, 2, 3);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(107, 43);
            btnUpdate.TabIndex = 3;
            btnUpdate.Text = "Sửa";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(261, 3);
            btnDelete.Margin = new Padding(2, 3, 2, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(107, 43);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnInsert
            // 
            btnInsert.FlatStyle = FlatStyle.Flat;
            btnInsert.Location = new Point(21, 3);
            btnInsert.Margin = new Padding(2, 3, 2, 3);
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(107, 43);
            btnInsert.TabIndex = 0;
            btnInsert.Text = "Thêm mới";
            btnInsert.UseVisualStyleBackColor = true;
            btnInsert.Click += btnInsert_Click_1;
            // 
            // cboFilterGender
            // 
            cboFilterGender.FormattingEnabled = true;
            cboFilterGender.Items.AddRange(new object[] { "Tất cả", "Nam", "Nữ" });
            cboFilterGender.Location = new Point(793, 48);
            cboFilterGender.Name = "cboFilterGender";
            cboFilterGender.Size = new Size(108, 28);
            cboFilterGender.TabIndex = 5;
            cboFilterGender.SelectedIndexChanged += cboFilterGender_SelectedIndexChanged;
            // 
            // btnImportExcel
            // 
            btnImportExcel.FlatStyle = FlatStyle.Flat;
            btnImportExcel.Location = new Point(552, 5);
            btnImportExcel.Margin = new Padding(2, 3, 2, 3);
            btnImportExcel.Name = "btnImportExcel";
            btnImportExcel.Size = new Size(107, 43);
            btnImportExcel.TabIndex = 5;
            btnImportExcel.Text = "ImportExcel";
            btnImportExcel.UseVisualStyleBackColor = true;
            // 
            // ListStudentForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(913, 611);
            Controls.Add(cboFilterGender);
            Controls.Add(pnl);
            Controls.Add(dgvStudents);
            Controls.Add(txtSearch);
            Controls.Add(txtDanhsachsinhvien);
            Margin = new Padding(2, 3, 2, 3);
            Name = "ListStudentForm";
            Text = "Form1";
            Load += ListStudentForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvStudents).EndInit();
            pnl.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label txtDanhsachsinhvien;
        private TextBox txtSearch;
        private DataGridView dgvStudents;
        private Panel pnl;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnInsert;
        private Button btnUpdate;
        private ComboBox cboFilterGender;
        private Button btnExportExcel;
        private Button btnImportExcel;
    }
}
