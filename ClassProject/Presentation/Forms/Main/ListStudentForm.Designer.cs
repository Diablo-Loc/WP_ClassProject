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
            btnDelete = new Button();
            btnInsert = new Button();
            btnUpdate = new Button();
            btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            pnl.SuspendLayout();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(694, 2);
            btnRefresh.Margin = new Padding(2);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(94, 32);
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
            txtDanhsachsinhvien.Location = new Point(18, 30);
            txtDanhsachsinhvien.Margin = new Padding(2, 0, 2, 0);
            txtDanhsachsinhvien.Name = "txtDanhsachsinhvien";
            txtDanhsachsinhvien.Size = new Size(319, 37);
            txtDanhsachsinhvien.TabIndex = 0;
            txtDanhsachsinhvien.Text = "DANH SÁCH SINH VIÊN";
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.ForeColor = SystemColors.WindowFrame;
            txtSearch.Location = new Point(391, 36);
            txtSearch.Margin = new Padding(2);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(289, 23);
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
            dgvStudents.Location = new Point(0, 70);
            dgvStudents.Margin = new Padding(2);
            dgvStudents.Name = "dgvStudents";
            dgvStudents.ReadOnly = true;
            dgvStudents.RowHeadersWidth = 62;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.Size = new Size(799, 336);
            dgvStudents.TabIndex = 3;
            // 
            // pnl
            // 
            pnl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnl.Controls.Add(btnUpdate);
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnRefresh);
            pnl.Controls.Add(btnInsert);
            pnl.Location = new Point(0, 419);
            pnl.Margin = new Padding(2);
            pnl.Name = "pnl";
            pnl.Size = new Size(799, 38);
            pnl.TabIndex = 4;
            // 
            // btnDelete
            // 
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(228, 2);
            btnDelete.Margin = new Padding(2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(94, 32);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnInsert
            // 
            btnInsert.FlatStyle = FlatStyle.Flat;
            btnInsert.Location = new Point(18, 2);
            btnInsert.Margin = new Padding(2);
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(94, 32);
            btnInsert.TabIndex = 0;
            btnInsert.Text = "Thêm mới";
            btnInsert.UseVisualStyleBackColor = true;
            btnInsert.Click += btnInsert_Click_1;
            // 
            // btnUpdate
            // 
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Location = new Point(123, 2);
            btnUpdate.Margin = new Padding(2);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(94, 32);
            btnUpdate.TabIndex = 3;
            btnUpdate.Text = "Sửa";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // ListStudentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(799, 458);
            Controls.Add(pnl);
            Controls.Add(dgvStudents);
            Controls.Add(txtSearch);
            Controls.Add(txtDanhsachsinhvien);
            Margin = new Padding(2);
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
    }
}
