namespace ClassProject.Presentation.Forms.Admin
{
    partial class TeachingAssignmentForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            label1 = new Label();
            lblTitle = new Label();
            pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            btnLoad = new Guna.UI2.WinForms.Guna2Button();
            lblThongTinPhanCong = new Label();
            pnlInfoRequest = new Guna.UI2.WinForms.Guna2Panel();
            btnDelete = new Guna.UI2.WinForms.Guna2Button();
            btnAssign = new Guna.UI2.WinForms.Guna2Button();
            cboCourse = new Guna.UI2.WinForms.Guna2ComboBox();
            cboTeacher = new Guna.UI2.WinForms.Guna2ComboBox();
            lblGiangVien = new Label();
            lblMonHoc = new Label();
            pnlHistory = new Guna.UI2.WinForms.Guna2Panel();
            dgvAssignments = new Guna.UI2.WinForms.Guna2DataGridView();
            lblDSPhanCongGiangDay = new Label();
            pnlHeader.SuspendLayout();
            pnlInfoRequest.SuspendLayout();
            pnlHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAssignments).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 60);
            label1.Name = "label1";
            label1.Size = new Size(334, 20);
            label1.TabIndex = 54;
            label1.Text = "Quản lý việc phân công giảng dạy cho giảng viên";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(12, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(372, 41);
            lblTitle.TabIndex = 53;
            lblTitle.Text = "PHÂN CÔNG GIẢNG DẠY";
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.Transparent;
            pnlHeader.Controls.Add(btnLoad);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(label1);
            pnlHeader.CustomizableEdges = customizableEdges3;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlHeader.Size = new Size(1069, 97);
            pnlHeader.TabIndex = 56;
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Cố định góc trên bên phải để không bị nén chiều dọc khi phóng to
            btnLoad.BorderColor = SystemColors.ScrollBar;
            btnLoad.BorderRadius = 8;
            btnLoad.BorderThickness = 1;
            btnLoad.CustomizableEdges = customizableEdges1;
            btnLoad.DisabledState.BorderColor = Color.DarkGray;
            btnLoad.DisabledState.CustomBorderColor = Color.DarkGray;
            btnLoad.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnLoad.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnLoad.FillColor = Color.White;
            btnLoad.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoad.ForeColor = SystemColors.WindowFrame;
            btnLoad.Location = new Point(885, 29); // Căn lề phải hợp lý hơn
            btnLoad.Margin = new Padding(30, 20, 20, 20);
            btnLoad.Name = "btnLoad";
            btnLoad.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnLoad.Size = new Size(164, 39);
            btnLoad.TabIndex = 55;
            btnLoad.Text = "🔄 Làm mới";
            btnLoad.Click += btnRefresh_Click;
            // 
            // lblThongTinPhanCong
            // 
            lblThongTinPhanCong.AutoSize = true;
            lblThongTinPhanCong.BackColor = Color.Transparent;
            lblThongTinPhanCong.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblThongTinPhanCong.ForeColor = Color.FromArgb(26, 115, 232);
            lblThongTinPhanCong.Location = new Point(23, 20);
            lblThongTinPhanCong.Name = "lblThongTinPhanCong";
            lblThongTinPhanCong.Size = new Size(187, 20);
            lblThongTinPhanCong.TabIndex = 44;
            lblThongTinPhanCong.Text = "THÔNG TIN PHÂN CÔNG";
            // 
            // pnlInfoRequest
            // 
            pnlInfoRequest.BackColor = Color.FromArgb(180, 200, 230);
            pnlInfoRequest.BorderRadius = 8;
            pnlInfoRequest.BorderThickness = 1;
            pnlInfoRequest.Controls.Add(btnDelete);
            pnlInfoRequest.Controls.Add(btnAssign);
            pnlInfoRequest.Controls.Add(cboCourse);
            pnlInfoRequest.Controls.Add(cboTeacher);
            pnlInfoRequest.Controls.Add(lblGiangVien);
            pnlInfoRequest.Controls.Add(lblMonHoc);
            pnlInfoRequest.Controls.Add(lblThongTinPhanCong);
            pnlInfoRequest.CustomizableEdges = customizableEdges13;
            pnlInfoRequest.Dock = DockStyle.Top;
            pnlInfoRequest.FillColor = Color.White;
            pnlInfoRequest.Location = new Point(0, 97);
            pnlInfoRequest.Margin = new Padding(20);
            pnlInfoRequest.Name = "pnlInfoRequest";
            pnlInfoRequest.Padding = new Padding(20);
            pnlInfoRequest.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlInfoRequest.Size = new Size(1069, 166);
            pnlInfoRequest.TabIndex = 57;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Sửa lỗi kéo giãn chiều dọc thô kệch
            btnDelete.BorderColor = Color.FromArgb(220, 53, 69);
            btnDelete.BorderRadius = 8;
            btnDelete.CustomizableEdges = customizableEdges5;
            btnDelete.DisabledState.BorderColor = Color.DarkGray;
            btnDelete.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDelete.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDelete.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDelete.FillColor = Color.White;
            btnDelete.Font = new Font("Segoe UI", 9F);
            btnDelete.ForeColor = Color.FromArgb(220, 53, 69);
            btnDelete.Location = new Point(885, 100);
            btnDelete.Name = "btnDelete";
            btnDelete.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnDelete.Size = new Size(164, 43);
            btnDelete.TabIndex = 52;
            btnDelete.Text = "Xóa/Hủy phân công";
            btnDelete.Click += btnDelete_Click;
            // 
            // btnAssign
            // 
            btnAssign.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Sửa lỗi kéo giãn chiều dọc thô kệch
            btnAssign.BorderRadius = 8;
            btnAssign.CustomizableEdges = customizableEdges7;
            btnAssign.DisabledState.BorderColor = Color.DarkGray;
            btnAssign.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAssign.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAssign.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAssign.FillColor = Color.FromArgb(26, 115, 232);
            btnAssign.Font = new Font("Segoe UI", 9F);
            btnAssign.ForeColor = Color.White;
            btnAssign.Location = new Point(702, 100);
            btnAssign.Name = "btnAssign";
            btnAssign.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnAssign.Size = new Size(164, 43);
            btnAssign.TabIndex = 51;
            btnAssign.Text = "+ Phân công";
            btnAssign.Click += btnAssign_Click;
            // 
            // cboCourse
            // 
            cboCourse.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Co giãn độ dài theo Form
            cboCourse.BackColor = Color.Transparent;
            cboCourse.BorderRadius = 6;
            cboCourse.CustomizableEdges = customizableEdges9;
            cboCourse.DrawMode = DrawMode.OwnerDrawFixed;
            cboCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCourse.FocusedColor = Color.FromArgb(0, 90, 156);
            cboCourse.FocusedState.BorderColor = Color.FromArgb(0, 90, 156);
            cboCourse.Font = new Font("Segoe UI", 10F);
            cboCourse.ForeColor = Color.FromArgb(68, 88, 112);
            cboCourse.ItemHeight = 30;
            cboCourse.Location = new Point(361, 103);
            cboCourse.Name = "cboCourse";
            cboCourse.ShadowDecoration.CustomizableEdges = customizableEdges10;
            cboCourse.Size = new Size(320, 36); // Tăng kích thước tối đa để mượt mà hơn với nút nhấn bên cạnh
            cboCourse.TabIndex = 50;
            // 
            // cboTeacher
            // 
            cboTeacher.BackColor = Color.Transparent;
            cboTeacher.BorderRadius = 6;
            cboTeacher.CustomizableEdges = customizableEdges11;
            cboTeacher.DrawMode = DrawMode.OwnerDrawFixed;
            cboTeacher.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTeacher.FocusedColor = Color.FromArgb(0, 90, 156);
            cboTeacher.FocusedState.BorderColor = Color.FromArgb(0, 90, 156);
            cboTeacher.Font = new Font("Segoe UI", 10F);
            cboTeacher.ForeColor = Color.FromArgb(68, 88, 112);
            cboTeacher.ItemHeight = 30;
            cboTeacher.Location = new Point(23, 103);
            cboTeacher.Name = "cboTeacher";
            cboTeacher.ShadowDecoration.CustomizableEdges = customizableEdges12;
            cboTeacher.Size = new Size(312, 36);
            cboTeacher.TabIndex = 49;
            // 
            // lblGiangVien
            // 
            lblGiangVien.AutoSize = true;
            lblGiangVien.BackColor = Color.Transparent;
            lblGiangVien.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGiangVien.Location = new Point(23, 66);
            lblGiangVien.Name = "lblGiangVien";
            lblGiangVien.Size = new Size(83, 20);
            lblGiangVien.TabIndex = 45;
            lblGiangVien.Text = "Giảng viên";
            // 
            // lblMonHoc
            // 
            lblMonHoc.AutoSize = true;
            lblMonHoc.BackColor = Color.Transparent;
            lblMonHoc.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMonHoc.Location = new Point(361, 66);
            lblMonHoc.Name = "lblMonHoc";
            lblMonHoc.Size = new Size(70, 20);
            lblMonHoc.TabIndex = 46;
            lblMonHoc.Text = "Môn học";
            // 
            // pnlHistory
            // 
            pnlHistory.Controls.Add(dgvAssignments);
            pnlHistory.Controls.Add(lblDSPhanCongGiangDay);
            pnlHistory.CustomizableEdges = customizableEdges15;
            pnlHistory.Dock = DockStyle.Fill; // Đảm bảo chiếm trọn không gian trống còn lại ở phía dưới
            pnlHistory.Location = new Point(0, 263);
            pnlHistory.Name = "pnlHistory";
            pnlHistory.ShadowDecoration.CustomizableEdges = customizableEdges16;
            pnlHistory.Size = new Size(1069, 315);
            pnlHistory.TabIndex = 45;
            // 
            // dgvAssignments
            // 
            dgvAssignments.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(248, 250, 253);
            dgvAssignments.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvAssignments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // Tự động co giãn 4 góc hoàn hảo
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvAssignments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvAssignments.ColumnHeadersHeight = 35; // Tăng chiều cao Header để text không bị che khuất
            dgvAssignments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvAssignments.DefaultCellStyle = dataGridViewCellStyle3;
            dgvAssignments.GridColor = Color.FromArgb(230, 235, 245);
            dgvAssignments.Location = new Point(23, 63); // Căn lề trái đồng đều với pnlInfoRequest
            dgvAssignments.Name = "dgvAssignments";
            dgvAssignments.RowHeadersVisible = false;
            dgvAssignments.RowHeadersWidth = 51;
            dgvAssignments.Size = new Size(1026, 230); // Tối ưu kích thước khoảng cách lề phải
            dgvAssignments.TabIndex = 54;
            dgvAssignments.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvAssignments.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvAssignments.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvAssignments.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvAssignments.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvAssignments.ThemeStyle.BackColor = Color.White;
            dgvAssignments.ThemeStyle.GridColor = Color.FromArgb(230, 235, 245);
            dgvAssignments.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvAssignments.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvAssignments.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvAssignments.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvAssignments.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAssignments.ThemeStyle.HeaderStyle.Height = 35;
            dgvAssignments.ThemeStyle.ReadOnly = false;
            dgvAssignments.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvAssignments.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAssignments.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvAssignments.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvAssignments.ThemeStyle.RowsStyle.Height = 29;
            dgvAssignments.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvAssignments.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // QUAN TRỌNG: Giúp toàn bộ cột tự động chia đều và lấp đầy bảng dữ liệu khi phóng to
            // 
            // lblDSPhanCongGiangDay
            // 
            lblDSPhanCongGiangDay.AutoSize = true;
            lblDSPhanCongGiangDay.BackColor = Color.Transparent;
            lblDSPhanCongGiangDay.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDSPhanCongGiangDay.ForeColor = Color.FromArgb(26, 115, 232);
            lblDSPhanCongGiangDay.Location = new Point(23, 20);
            lblDSPhanCongGiangDay.Name = "lblDSPhanCongGiangDay";
            lblDSPhanCongGiangDay.Size = new Size(280, 20);
            lblDSPhanCongGiangDay.TabIndex = 53;
            lblDSPhanCongGiangDay.Text = "DANH SÁCH PHÂN CÔNG GIẢNG DẠY";
            // 
            // TeachingAssignmentForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 251);
            ClientSize = new Size(1069, 578);
            Controls.Add(pnlHistory);
            Controls.Add(pnlInfoRequest);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1087, 625); // Tạo kích thước giới hạn nhỏ nhất để giao diện không bị vỡ khi thu nhỏ quá đà
            Name = "TeachingAssignmentForm";
            StartPosition = FormStartPosition.CenterScreen; // Mở ứng dụng ngay giữa màn hình cho chuyên nghiệp
            Text = "TeachingAssignmentRepository";
            Load += TeachingAssignmentForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlInfoRequest.ResumeLayout(false);
            pnlInfoRequest.PerformLayout();
            pnlHistory.ResumeLayout(false);
            pnlHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAssignments).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label lblTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Label lblThongTinPhanCong;
        private Guna.UI2.WinForms.Guna2Panel pnlInfoRequest;
        private Guna.UI2.WinForms.Guna2Panel pnlHistory;
        private Guna.UI2.WinForms.Guna2ComboBox cboTeacher;
        private Label lblGiangVien;
        private Label lblMonHoc;
        private Guna.UI2.WinForms.Guna2ComboBox cboCourse;
        private Guna.UI2.WinForms.Guna2Button btnLoad;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnAssign;
        private Guna.UI2.WinForms.Guna2DataGridView dgvAssignments;
        private Label lblDSPhanCongGiangDay;
    }
}