using Guna.UI2.WinForms;

namespace ClassProject.Presentation.Forms.Main
{
    partial class ClassroomForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblTitle = new Label();
            guna2Panel1 = new Guna2Panel();
            btnInsert = new Guna2Button();
            btnLoad = new Guna2Button();
            txtTenLop = new Guna2TextBox();
            txtMaLop = new Guna2TextBox();
            numSiSo = new Guna2NumericUpDown();
            lblMaLopHoc = new Label();
            lblTenLopHoc = new Label();
            lblSiSo = new Label();
            lblGiaoVienChuNhiem = new Label();
            guna2Panel2 = new Guna2Panel();
            lblTotalClasses = new Label();
            dgvClassroom = new Guna2DataGridView();
            btnEditColumn = new DataGridViewButtonColumn();
            btnDeleteColumn = new DataGridViewButtonColumn();
            btnExport = new Guna2Button();
            btnSearch = new Guna2Button();
            txtSearch = new Guna2TextBox();
            label1 = new Label();
            cboGVCN = new Guna2ComboBox();
            guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSiSo).BeginInit();
            guna2Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClassroom).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(25, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(285, 41);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "QUẢN LÝ LỚP HỌC";
            // 
            // guna2Panel1
            // 
            guna2Panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            guna2Panel1.BackColor = Color.White;
            guna2Panel1.BorderRadius = 12;
            guna2Panel1.Controls.Add(cboGVCN);
            guna2Panel1.Controls.Add(btnInsert);
            guna2Panel1.Controls.Add(btnLoad);
            guna2Panel1.Controls.Add(txtTenLop);
            guna2Panel1.Controls.Add(txtMaLop);
            guna2Panel1.Controls.Add(numSiSo);
            guna2Panel1.Controls.Add(lblMaLopHoc);
            guna2Panel1.Controls.Add(lblTenLopHoc);
            guna2Panel1.Controls.Add(lblSiSo);
            guna2Panel1.Controls.Add(lblGiaoVienChuNhiem);
            guna2Panel1.CustomizableEdges = customizableEdges13;
            guna2Panel1.Location = new Point(25, 86);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges14;
            guna2Panel1.Size = new Size(1098, 240); // Đã tối ưu chiều cao gọn gàng hơn
            guna2Panel1.TabIndex = 31;
            // 
            // txtMaLop
            // 
            txtMaLop.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtMaLop.BorderRadius = 8;
            txtMaLop.CustomizableEdges = customizableEdges9;
            txtMaLop.DefaultText = "";
            txtMaLop.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtMaLop.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMaLop.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtMaLop.Location = new Point(149, 30);
            txtMaLop.Margin = new Padding(3, 5, 3, 5);
            txtMaLop.Name = "txtMaLop";
            txtMaLop.PlaceholderText = "Nhập mã lớp học";
            txtMaLop.SelectedText = "";
            txtMaLop.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtMaLop.Size = new Size(318, 39);
            txtMaLop.TabIndex = 42;
            // 
            // txtTenLop
            // 
            txtTenLop.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtTenLop.BorderRadius = 8;
            txtTenLop.CustomizableEdges = customizableEdges7;
            txtTenLop.DefaultText = "";
            txtTenLop.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtTenLop.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTenLop.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtTenLop.Location = new Point(149, 100);
            txtTenLop.Margin = new Padding(3, 5, 3, 5);
            txtTenLop.Name = "txtTenLop";
            txtTenLop.PlaceholderText = "Nhập tên lớp học";
            txtTenLop.SelectedText = "";
            txtTenLop.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtTenLop.Size = new Size(318, 39);
            txtTenLop.TabIndex = 43;
            // 
            // numSiSo
            // 
            numSiSo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Tự dãn theo chiều ngang
            numSiSo.BackColor = Color.Transparent;
            numSiSo.BorderRadius = 8;
            numSiSo.CustomizableEdges = customizableEdges11;
            numSiSo.Font = new Font("Segoe UI", 9F);
            numSiSo.Location = new Point(709, 30);
            numSiSo.Margin = new Padding(3, 4, 3, 4);
            numSiSo.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSiSo.Name = "numSiSo";
            numSiSo.ShadowDecoration.CustomizableEdges = customizableEdges12;
            numSiSo.Size = new Size(351, 39);
            numSiSo.TabIndex = 34;
            // 
            // cboGVCN
            // 
            cboGVCN.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Tự dãn theo chiều ngang
            cboGVCN.BackColor = Color.Transparent;
            cboGVCN.BorderRadius = 6;
            cboGVCN.CustomizableEdges = customizableEdges1;
            cboGVCN.DrawMode = DrawMode.OwnerDrawFixed;
            cboGVCN.DropDownStyle = ComboBoxStyle.DropDownList;
            cboGVCN.FocusedColor = Color.FromArgb(0, 90, 156);
            cboGVCN.FocusedState.BorderColor = Color.FromArgb(0, 90, 156);
            cboGVCN.Font = new Font("Segoe UI", 10F);
            cboGVCN.ForeColor = Color.FromArgb(68, 88, 112);
            cboGVCN.ItemHeight = 30;
            cboGVCN.Location = new Point(709, 100);
            cboGVCN.Name = "cboGVCN";
            cboGVCN.ShadowDecoration.CustomizableEdges = customizableEdges2;
            cboGVCN.Size = new Size(351, 36);
            cboGVCN.TabIndex = 50;
            // 
            // lblMaLopHoc
            // 
            lblMaLopHoc.AutoSize = true;
            lblMaLopHoc.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMaLopHoc.Location = new Point(24, 39);
            lblMaLopHoc.Name = "lblMaLopHoc";
            lblMaLopHoc.Size = new Size(90, 20);
            lblMaLopHoc.TabIndex = 11;
            lblMaLopHoc.Text = "Mã lớp học:";
            // 
            // lblTenLopHoc
            // 
            lblTenLopHoc.AutoSize = true;
            lblTenLopHoc.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTenLopHoc.Location = new Point(24, 110);
            lblTenLopHoc.Name = "lblTenLopHoc";
            lblTenLopHoc.Size = new Size(99, 20);
            lblTenLopHoc.TabIndex = 12;
            lblTenLopHoc.Text = "Tên Lớp Học:";
            // 
            // lblSiSo
            // 
            lblSiSo.AutoSize = true;
            lblSiSo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSiSo.Location = new Point(530, 39);
            lblSiSo.Name = "lblSiSo";
            lblSiSo.Size = new Size(46, 20);
            lblSiSo.TabIndex = 20;
            lblSiSo.Text = "Sỉ Số:";
            // 
            // lblGiaoVienChuNhiem
            // 
            lblGiaoVienChuNhiem.AutoSize = true;
            lblGiaoVienChuNhiem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGiaoVienChuNhiem.Location = new Point(530, 110);
            lblGiaoVienChuNhiem.Name = "lblGiaoVienChuNhiem";
            lblGiaoVienChuNhiem.Size = new Size(78, 20);
            lblGiaoVienChuNhiem.TabIndex = 22;
            lblGiaoVienChuNhiem.Text = "Giáo viên:";
            // 
            // btnInsert
            // 
            btnInsert.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Neo chặt góc dưới bên trái panel
            btnInsert.BorderRadius = 8;
            btnInsert.CustomizableEdges = customizableEdges3;
            btnInsert.FillColor = Color.FromArgb(26, 115, 232);
            btnInsert.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnInsert.ForeColor = Color.White;
            btnInsert.Location = new Point(149, 175);
            btnInsert.Name = "btnInsert";
            btnInsert.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnInsert.Size = new Size(156, 39);
            btnInsert.TabIndex = 45;
            btnInsert.Text = "(+) Thêm lớp";
            btnInsert.Click += btnInsert_Click;
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Đặt cạnh nút thêm
            btnLoad.BorderColor = SystemColors.ScrollBar;
            btnLoad.BorderRadius = 8;
            btnLoad.BorderThickness = 1;
            btnLoad.CustomizableEdges = customizableEdges5;
            btnLoad.FillColor = Color.White;
            btnLoad.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoad.ForeColor = SystemColors.WindowFrame;
            btnLoad.Location = new Point(320, 175);
            btnLoad.Name = "btnLoad";
            btnLoad.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnLoad.Size = new Size(147, 39);
            btnLoad.TabIndex = 46;
            btnLoad.Text = "🔄 Làm mới";
            btnLoad.Click += btnClear_Click;
            // 
            // guna2Panel2
            // 
            guna2Panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Tự động co giãn 4 phía theo Form
            guna2Panel2.BorderColor = Color.FromArgb(120, 200, 150);
            guna2Panel2.BorderRadius = 10;
            guna2Panel2.BorderThickness = 1;
            guna2Panel2.Controls.Add(lblTotalClasses);
            guna2Panel2.Controls.Add(dgvClassroom);
            guna2Panel2.Controls.Add(btnExport);
            guna2Panel2.Controls.Add(btnSearch);
            guna2Panel2.Controls.Add(txtSearch);
            guna2Panel2.Controls.Add(label1);
            guna2Panel2.CustomizableEdges = customizableEdges21;
            guna2Panel2.FillColor = Color.White;
            guna2Panel2.Location = new Point(25, 350); // Đẩy sát lên để cân đối diện tích
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges22;
            guna2Panel2.Size = new Size(1098, 420);
            guna2Panel2.TabIndex = 32;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(40, 167, 69);
            label1.Location = new Point(20, 18);
            label1.Name = "label1";
            label1.Size = new Size(165, 20);
            label1.TabIndex = 32;
            label1.Text = "DANH SÁCH LỚP HỌC";
            // 
            // txtSearch
            // 
            txtSearch.BorderRadius = 8;
            txtSearch.CustomizableEdges = customizableEdges19;
            txtSearch.DefaultText = "";
            txtSearch.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Font = new Font("Segoe UI", 9F);
            txtSearch.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Location = new Point(24, 55);
            txtSearch.Margin = new Padding(3, 4, 3, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderForeColor = Color.FromArgb(170, 180, 190);
            txtSearch.PlaceholderText = "Tìm kiếm lớp học...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges20;
            txtSearch.Size = new Size(282, 39);
            txtSearch.TabIndex = 47;
            // 
            // btnSearch
            // 
            btnSearch.BorderRadius = 8;
            btnSearch.CustomizableEdges = customizableEdges17;
            btnSearch.FillColor = Color.FromArgb(40, 167, 69);
            btnSearch.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(320, 55);
            btnSearch.Name = "btnSearch";
            btnSearch.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btnSearch.Size = new Size(73, 39);
            btnSearch.TabIndex = 47;
            btnSearch.Text = "Tìm";
            btnSearch.Click += btnSearch_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Neo bên phải để luôn ở góc phải màn hình
            btnExport.BorderColor = Color.LightGray;
            btnExport.BorderRadius = 6;
            btnExport.BorderThickness = 1;
            btnExport.CustomizableEdges = customizableEdges15;
            btnExport.FillColor = Color.White;
            btnExport.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExport.ForeColor = Color.Black;
            btnExport.Location = new Point(941, 55);
            btnExport.Name = "btnExport";
            btnExport.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnExport.Size = new Size(119, 39);
            btnExport.TabIndex = 48;
            btnExport.Text = "Xuất Excel";
            btnExport.Click += btnExportExcel_Click;
            // 
            // dgvClassroom
            // 
            dgvClassroom.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(250, 252, 255);
            dgvClassroom.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvClassroom.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Co giãn phủ kín không gian thừa của panel 2
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(34, 103, 230);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvClassroom.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvClassroom.ColumnHeadersHeight = 35; // Tăng chiều cao Header để text không bị che khuất
            dgvClassroom.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvClassroom.Columns.AddRange(new DataGridViewColumn[] { btnEditColumn, btnDeleteColumn });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvClassroom.DefaultCellStyle = dataGridViewCellStyle3;
            dgvClassroom.GridColor = Color.FromArgb(240, 242, 245);
            dgvClassroom.Location = new Point(24, 115);
            dgvClassroom.Name = "dgvClassroom";
            dgvClassroom.RowHeadersVisible = false;
            dgvClassroom.RowHeadersWidth = 51;
            dgvClassroom.Size = new Size(1036, 250); // Chiều cao tự động dãn theo độ phóng to Form
            dgvClassroom.TabIndex = 49;
            dgvClassroom.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvClassroom.ThemeStyle.BackColor = Color.White;
            dgvClassroom.ThemeStyle.GridColor = Color.FromArgb(240, 242, 245);
            dgvClassroom.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(34, 103, 230);
            dgvClassroom.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvClassroom.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvClassroom.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvClassroom.ThemeStyle.HeaderStyle.Height = 35;
            dgvClassroom.ThemeStyle.ReadOnly = false;
            dgvClassroom.ThemeStyle.RowsStyle.Height = 29;
            dgvClassroom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Chia đều các cột tự động theo màn hình
            dgvClassroom.CellClick += dgvClassroom_CellClick;
            // 
            // btnEditColumn
            // 
            btnEditColumn.HeaderText = "Thao tác";
            btnEditColumn.MinimumWidth = 80;
            btnEditColumn.Name = "btnEditColumn";
            btnEditColumn.Text = "✏ Sửa";
            btnEditColumn.UseColumnTextForButtonValue = true;
            // 
            // btnDeleteColumn
            // 
            btnDeleteColumn.HeaderText = "Xóa";
            btnDeleteColumn.MinimumWidth = 80;
            btnDeleteColumn.Name = "btnDeleteColumn";
            btnDeleteColumn.Text = "❌ Xóa";
            btnDeleteColumn.UseColumnTextForButtonValue = true;
            // 
            // lblTotalClasses
            // 
            lblTotalClasses.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Ghim chặt dưới đáy lưới DataGridView
            lblTotalClasses.AutoSize = true;
            lblTotalClasses.BackColor = Color.Transparent;
            lblTotalClasses.Location = new Point(24, 385);
            lblTotalClasses.Name = "lblTotalClasses";
            lblTotalClasses.Size = new Size(104, 20);
            lblTotalClasses.TabIndex = 33;
            lblTotalClasses.Text = "Tổng số lớp: ...";
            // 
            // ClassroomForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font; // Cực kỳ quan trọng để tự động scale cỡ chữ theo Windows Scale %
            ClientSize = new Size(1148, 797);
            Controls.Add(guna2Panel2);
            Controls.Add(guna2Panel1);
            Controls.Add(lblTitle);
            MinimumSize = new Size(1000, 700); // Ngăn người dùng thu quá nhỏ làm vỡ Layout
            Name = "ClassroomForm";
            Text = "ClassroomForm";
            Load += f_Classroom_Load;
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSiSo).EndInit();
            guna2Panel2.ResumeLayout(false);
            guna2Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClassroom).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna2NumericUpDown numSiSo;
        private Guna.UI2.WinForms.Guna2TextBox txtMaLop;
        private Label lblMaLopHoc;
        private Label lblTenLopHoc;
        private Label lblSiSo;
        private Label lblGiaoVienChuNhiem;
        private Guna.UI2.WinForms.Guna2TextBox txtTenLop;
        private Guna.UI2.WinForms.Guna2Button btnInsert;
        private Guna.UI2.WinForms.Guna2Button btnLoad;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Label label1;
        private Guna.UI2.WinForms.Guna2Button btnSearch;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnExport;
        private Guna.UI2.WinForms.Guna2DataGridView dgvClassroom;
        private Label lblTotalClasses;
        private DataGridViewButtonColumn btnEditColumn;
        private DataGridViewButtonColumn btnDeleteColumn;
        private Guna2ComboBox cboGVCN;
    }
}