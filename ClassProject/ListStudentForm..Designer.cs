namespace ListStudentForm
{
    partial class Form1
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
            dataGridView1 = new DataGridView();
            MaSV = new DataGridViewTextBoxColumn();
            Ho = new DataGridViewTextBoxColumn();
            Ten = new DataGridViewTextBoxColumn();
            NgaySinh = new DataGridViewTextBoxColumn();
            GioiTinh = new DataGridViewTextBoxColumn();
            pnl = new Panel();
            btnInsert = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            pnl.SuspendLayout();
            SuspendLayout();
            // 
            // txtDanhsachsinhvien
            // 
            txtDanhsachsinhvien.AutoSize = true;
            txtDanhsachsinhvien.Cursor = Cursors.No;
            txtDanhsachsinhvien.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtDanhsachsinhvien.Location = new Point(25, 50);
            txtDanhsachsinhvien.Name = "txtDanhsachsinhvien";
            txtDanhsachsinhvien.Size = new Size(473, 54);
            txtDanhsachsinhvien.TabIndex = 0;
            txtDanhsachsinhvien.Text = "DANH SÁCH SINH VIÊN";
            txtDanhsachsinhvien.Click += label1_Click;
            // 
            // txtSearch
            // 
            txtSearch.ForeColor = SystemColors.WindowFrame;
            txtSearch.Location = new Point(559, 60);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(412, 31);
            txtSearch.TabIndex = 2;
            txtSearch.Text = "Nhập mã SV, họ hoặc tên để tìm...";
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.Enter += txtSearch_Enter;
            txtSearch.Leave += txtSearch_Leave;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { MaSV, Ho, Ten, NgaySinh, GioiTinh });
            dataGridView1.Location = new Point(0, 118);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1141, 560);
            dataGridView1.TabIndex = 3;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick_1;
            // 
            // MaSV
            // 
            MaSV.HeaderText = "Mã SV";
            MaSV.MinimumWidth = 8;
            MaSV.Name = "MaSV";
            // 
            // Ho
            // 
            Ho.HeaderText = "Họ";
            Ho.MinimumWidth = 8;
            Ho.Name = "Ho";
            // 
            // Ten
            // 
            Ten.HeaderText = "Tên";
            Ten.MinimumWidth = 8;
            Ten.Name = "Ten";
            // 
            // NgaySinh
            // 
            NgaySinh.HeaderText = "Ngày Sinh";
            NgaySinh.MinimumWidth = 8;
            NgaySinh.Name = "NgaySinh";
            // 
            // GioiTinh
            // 
            GioiTinh.HeaderText = "Giới Tính";
            GioiTinh.MinimumWidth = 8;
            GioiTinh.Name = "GioiTinh";
            // 
            // pnl
            // 
            pnl.Controls.Add(btnDelete);
            pnl.Controls.Add(btnUpdate);
            pnl.Controls.Add(btnInsert);
            pnl.Dock = DockStyle.Bottom;
            pnl.Location = new Point(0, 699);
            pnl.Name = "pnl";
            pnl.Size = new Size(1141, 64);
            pnl.TabIndex = 4;
            // 
            // btnInsert
            // 
            btnInsert.FlatStyle = FlatStyle.Flat;
            btnInsert.Location = new Point(25, 3);
            btnInsert.Name = "btnInsert";
            btnInsert.Size = new Size(135, 53);
            btnInsert.TabIndex = 0;
            btnInsert.Text = "Thêm mới";
            btnInsert.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.Location = new Point(175, 3);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(135, 53);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "Sửa";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(325, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(135, 53);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1141, 763);
            Controls.Add(pnl);
            Controls.Add(dataGridView1);
            Controls.Add(txtSearch);
            Controls.Add(txtDanhsachsinhvien);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            pnl.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label txtDanhsachsinhvien;
        private TextBox txtSearch;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn MaSV;
        private DataGridViewTextBoxColumn Ho;
        private DataGridViewTextBoxColumn Ten;
        private DataGridViewTextBoxColumn NgaySinh;
        private DataGridViewTextBoxColumn GioiTinh;
        private Panel pnl;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnInsert;
    }
}
