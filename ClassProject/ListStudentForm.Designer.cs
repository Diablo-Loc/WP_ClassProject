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
            txtDanhsachsinhvien = new Label();
            txtSearch = new TextBox();
            dgvStudents = new DataGridView();
            pnl = new Panel();
            btnDelete = new Button();
            btnUpdate = new Button();
            btnInsert = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            pnl.SuspendLayout();
            SuspendLayout();
            // 
            // txtDanhsachsinhvien
            // 
            txtDanhsachsinhvien.AutoSize = true;
            txtDanhsachsinhvien.Cursor = Cursors.No;
            txtDanhsachsinhvien.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtDanhsachsinhvien.Location = new Point(20, 40);
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
            txtSearch.Margin = new Padding(2);
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
            dgvStudents.Location = new Point(0, 94);
            dgvStudents.Margin = new Padding(2);
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
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnUpdate);
            pnl.Controls.Add(btnInsert);
            pnl.Location = new Point(0, 559);
            pnl.Margin = new Padding(2);
            pnl.Name = "pnl";
            pnl.Size = new Size(913, 51);
            pnl.TabIndex = 4;
            // 
            // btnDelete
            // 
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(260, 2);
            btnDelete.Margin = new Padding(2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(108, 42);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Location = new Point(140, 2);
            btnUpdate.Margin = new Padding(2);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(108, 42);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "Sửa";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnInsert
            // 
            btnInsert.FlatStyle = FlatStyle.Flat;
            btnInsert.Location = new Point(20, 2);
            btnInsert.Margin = new Padding(2);
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(108, 42);
            btnInsert.TabIndex = 0;
            btnInsert.Text = "Thêm mới";
            btnInsert.UseVisualStyleBackColor = true;
            btnInsert.Click += btnInsert_Click_1;
            // 
            // ListStudentForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(913, 610);
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
        private Button btnUpdate;
        private Button btnInsert;
    }
}
