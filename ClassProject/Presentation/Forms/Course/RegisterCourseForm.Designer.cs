namespace ClassProject.Presentation.Forms.Course
{
    partial class RegisterCourseForm
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
            lblTitle = new Label();
            lblSinhVien = new Label();
            lblMonHoc = new Label();
            cboStudent = new ComboBox();
            cboCourse = new ComboBox();
            btnRegister = new Button();
            btnCancelRegister = new Button();
            lblDanhSach = new Label();
            btnLoad = new Button();
            dgvRegisterCourse = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvRegisterCourse).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(238, 29);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(295, 41);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ĐĂNG KÝ MÔN HỌC";
            // 
            // lblSinhVien
            // 
            lblSinhVien.AutoSize = true;
            lblSinhVien.Location = new Point(95, 98);
            lblSinhVien.Name = "lblSinhVien";
            lblSinhVien.Size = new Size(71, 20);
            lblSinhVien.TabIndex = 1;
            lblSinhVien.Text = "Sinh viên:";
            // 
            // lblMonHoc
            // 
            lblMonHoc.AutoSize = true;
            lblMonHoc.Location = new Point(453, 98);
            lblMonHoc.Name = "lblMonHoc";
            lblMonHoc.Size = new Size(70, 20);
            lblMonHoc.TabIndex = 2;
            lblMonHoc.Text = "Môn học:";
            // 
            // cboStudent
            // 
            cboStudent.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStudent.FormattingEnabled = true;
            cboStudent.Location = new Point(190, 95);
            cboStudent.Name = "cboStudent";
            cboStudent.Size = new Size(225, 28);
            cboStudent.TabIndex = 3;
            // 
            // cboCourse
            // 
            cboCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCourse.FormattingEnabled = true;
            cboCourse.Location = new Point(550, 98);
            cboCourse.Name = "cboCourse";
            cboCourse.Size = new Size(165, 28);
            cboCourse.TabIndex = 4;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(277, 188);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(120, 52);
            btnRegister.TabIndex = 5;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // btnCancelRegister
            // 
            btnCancelRegister.Location = new Point(453, 188);
            btnCancelRegister.Name = "btnCancelRegister";
            btnCancelRegister.Size = new Size(120, 52);
            btnCancelRegister.TabIndex = 6;
            btnCancelRegister.Text = "Cancel";
            btnCancelRegister.UseVisualStyleBackColor = true;
            btnCancelRegister.Click += btnCancelRegister_Click;
            // 
            // lblDanhSach
            // 
            lblDanhSach.AutoSize = true;
            lblDanhSach.Location = new Point(95, 255);
            lblDanhSach.Name = "lblDanhSach";
            lblDanhSach.Size = new Size(219, 20);
            lblDanhSach.TabIndex = 7;
            lblDanhSach.Text = "Danh sách môn học đã đăng ký:";
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(621, 251);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(94, 29);
            btnLoad.TabIndex = 8;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // dgvRegisterCourse
            // 
            dgvRegisterCourse.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRegisterCourse.Location = new Point(66, 298);
            dgvRegisterCourse.Name = "dgvRegisterCourse";
            dgvRegisterCourse.RowHeadersWidth = 51;
            dgvRegisterCourse.Size = new Size(662, 140);
            dgvRegisterCourse.TabIndex = 9;
            // 
            // RegisterCourseForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvRegisterCourse);
            Controls.Add(btnLoad);
            Controls.Add(lblDanhSach);
            Controls.Add(btnCancelRegister);
            Controls.Add(btnRegister);
            Controls.Add(cboCourse);
            Controls.Add(cboStudent);
            Controls.Add(lblMonHoc);
            Controls.Add(lblSinhVien);
            Controls.Add(lblTitle);
            Name = "RegisterCourseForm";
            Text = "RegisterCourseForm";
            Load += RegisterCourseForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRegisterCourse).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblSinhVien;
        private Label lblMonHoc;
        private ComboBox cboStudent;
        private ComboBox cboCourse;
        private Button btnRegister;
        private Button btnCancelRegister;
        private Label lblDanhSach;
        private Button btnLoad;
        private DataGridView dgvRegisterCourse;
    }
}