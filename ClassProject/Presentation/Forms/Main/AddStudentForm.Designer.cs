namespace ClassProject
{
    partial class AddStudentForm
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
            picStudent = new PictureBox();
            btnChooseImage = new Button();
            lbFirstName = new Label();
            lbLastName = new Label();
            lbDateOfBirth = new Label();
            lbPhone = new Label();
            lbGender = new Label();
            btnAdd = new Button();
            btnClear = new Button();
            txtMSSV = new TextBox();
            txtFirstName = new TextBox();
            txtLastName = new TextBox();
            txtPhone = new TextBox();
            txtAddress = new TextBox();
            dtpDateOfBirth = new DateTimePicker();
            txtEmail = new TextBox();
            cboGender = new ComboBox();
            lblHometown = new Label();
            lbMSSV = new Label();
            lbEmail = new Label();
            lbAddress = new Label();
            txtHometown = new TextBox();
            picLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picStudent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // picStudent
            // 
            picStudent.BorderStyle = BorderStyle.FixedSingle;
            picStudent.Location = new Point(40, 91);
            picStudent.Margin = new Padding(3, 4, 3, 4);
            picStudent.Name = "picStudent";
            picStudent.Size = new Size(187, 207);
            picStudent.SizeMode = PictureBoxSizeMode.StretchImage;
            picStudent.TabIndex = 0;
            picStudent.TabStop = false;
            // 
            // btnChooseImage
            // 
            btnChooseImage.Location = new Point(40, 324);
            btnChooseImage.Margin = new Padding(3, 4, 3, 4);
            btnChooseImage.Name = "btnChooseImage";
            btnChooseImage.Size = new Size(187, 40);
            btnChooseImage.TabIndex = 1;
            btnChooseImage.Text = "ChooseImage";
            btnChooseImage.UseVisualStyleBackColor = true;
            btnChooseImage.Click += btnChooseImage_Click;
            // 
            // lbFirstName
            // 
            lbFirstName.AutoSize = true;
            lbFirstName.BackColor = Color.White;
            lbFirstName.Font = new Font("Segoe UI", 12F);
            lbFirstName.Location = new Point(479, 135);
            lbFirstName.Name = "lbFirstName";
            lbFirstName.Size = new Size(140, 28);
            lbFirstName.TabIndex = 3;
            lbFirstName.Text = "Họ và tên đệm";
            // 
            // lbLastName
            // 
            lbLastName.AutoSize = true;
            lbLastName.Font = new Font("Segoe UI", 12F);
            lbLastName.Location = new Point(478, 181);
            lbLastName.Name = "lbLastName";
            lbLastName.Size = new Size(41, 28);
            lbLastName.TabIndex = 4;
            lbLastName.Text = "Tên";
            // 
            // lbDateOfBirth
            // 
            lbDateOfBirth.AutoSize = true;
            lbDateOfBirth.Font = new Font("Segoe UI", 12F);
            lbDateOfBirth.Location = new Point(478, 233);
            lbDateOfBirth.Name = "lbDateOfBirth";
            lbDateOfBirth.Size = new Size(99, 28);
            lbDateOfBirth.TabIndex = 5;
            lbDateOfBirth.Text = "Ngày sinh";
            // 
            // lbPhone
            // 
            lbPhone.AutoSize = true;
            lbPhone.Font = new Font("Segoe UI", 12F);
            lbPhone.Location = new Point(478, 277);
            lbPhone.Name = "lbPhone";
            lbPhone.Size = new Size(128, 28);
            lbPhone.TabIndex = 6;
            lbPhone.Text = "Số điện thoại";
            // 
            // lbGender
            // 
            lbGender.AutoSize = true;
            lbGender.Font = new Font("Segoe UI", 12F);
            lbGender.Location = new Point(480, 416);
            lbGender.Name = "lbGender";
            lbGender.Size = new Size(87, 28);
            lbGender.TabIndex = 8;
            lbGender.Text = "Giới tính";
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(280, 495);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(131, 53);
            btnAdd.TabIndex = 9;
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(479, 495);
            btnClear.Margin = new Padding(3, 4, 3, 4);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(131, 53);
            btnClear.TabIndex = 10;
            btnClear.Text = "CLEAR";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // txtMSSV
            // 
            txtMSSV.Location = new Point(632, 88);
            txtMSSV.Margin = new Padding(3, 4, 3, 4);
            txtMSSV.Name = "txtMSSV";
            txtMSSV.Size = new Size(228, 27);
            txtMSSV.TabIndex = 11;
            // 
            // txtFirstName
            // 
            txtFirstName.Location = new Point(632, 135);
            txtFirstName.Margin = new Padding(3, 4, 3, 4);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(228, 27);
            txtFirstName.TabIndex = 12;
            // 
            // txtLastName
            // 
            txtLastName.Location = new Point(632, 181);
            txtLastName.Margin = new Padding(3, 4, 3, 4);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(228, 27);
            txtLastName.TabIndex = 13;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(632, 275);
            txtPhone.Margin = new Padding(3, 4, 3, 4);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(228, 27);
            txtPhone.TabIndex = 14;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(632, 325);
            txtAddress.Margin = new Padding(3, 4, 3, 4);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(228, 27);
            txtAddress.TabIndex = 16;
            // 
            // dtpDateOfBirth
            // 
            dtpDateOfBirth.Location = new Point(632, 231);
            dtpDateOfBirth.Margin = new Padding(3, 4, 3, 4);
            dtpDateOfBirth.Name = "dtpDateOfBirth";
            dtpDateOfBirth.Size = new Size(228, 27);
            dtpDateOfBirth.TabIndex = 18;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(632, 456);
            txtEmail.Margin = new Padding(3, 4, 3, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(228, 27);
            txtEmail.TabIndex = 19;
            // 
            // cboGender
            // 
            cboGender.FormattingEnabled = true;
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGender.Location = new Point(632, 413);
            cboGender.Margin = new Padding(3, 4, 3, 4);
            cboGender.Name = "cboGender";
            cboGender.Size = new Size(226, 28);
            cboGender.TabIndex = 20;
            // 
            // lblHometown
            // 
            lblHometown.AutoSize = true;
            lblHometown.Font = new Font("Segoe UI", 12F);
            lblHometown.Location = new Point(478, 371);
            lblHometown.Name = "lblHometown";
            lblHometown.Size = new Size(110, 28);
            lblHometown.TabIndex = 21;
            lblHometown.Text = "HomeTown";
            // 
            // lbMSSV
            // 
            lbMSSV.AutoSize = true;
            lbMSSV.BackColor = Color.White;
            lbMSSV.Font = new Font("Segoe UI", 12F);
            lbMSSV.Location = new Point(478, 91);
            lbMSSV.Name = "lbMSSV";
            lbMSSV.Size = new Size(64, 28);
            lbMSSV.TabIndex = 2;
            lbMSSV.Text = "MSSV";
            // 
            // lbEmail
            // 
            lbEmail.AutoSize = true;
            lbEmail.BackColor = Color.White;
            lbEmail.Font = new Font("Segoe UI", 12F);
            lbEmail.Location = new Point(480, 456);
            lbEmail.Name = "lbEmail";
            lbEmail.Size = new Size(59, 28);
            lbEmail.TabIndex = 17;
            lbEmail.Text = "Email";
            // 
            // lbAddress
            // 
            lbAddress.AutoSize = true;
            lbAddress.BackColor = Color.White;
            lbAddress.Font = new Font("Segoe UI", 12F);
            lbAddress.Location = new Point(478, 328);
            lbAddress.Name = "lbAddress";
            lbAddress.Size = new Size(76, 28);
            lbAddress.TabIndex = 7;
            lbAddress.Text = "Địa chỉ ";
            // 
            // txtHometown
            // 
            txtHometown.Location = new Point(632, 368);
            txtHometown.Margin = new Padding(3, 4, 3, 4);
            txtHometown.Name = "txtHometown";
            txtHometown.Size = new Size(228, 27);
            txtHometown.TabIndex = 22;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.FromArgb(100, 0, 0, 0);
            picLogo.BackgroundImage = Properties.Resources.Login_ico;
            picLogo.BackgroundImageLayout = ImageLayout.Stretch;
            picLogo.Location = new Point(70, 388);
            picLogo.Margin = new Padding(3, 4, 3, 4);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(134, 160);
            picLogo.TabIndex = 23;
            picLogo.TabStop = false;
            // 
            // AddStudentForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.createSV;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(914, 600);
            Controls.Add(txtHometown);
            Controls.Add(picLogo);
            Controls.Add(lbEmail);
            Controls.Add(lblHometown);
            Controls.Add(cboGender);
            Controls.Add(lbAddress);
            Controls.Add(txtEmail);
            Controls.Add(dtpDateOfBirth);
            Controls.Add(lbMSSV);
            Controls.Add(txtAddress);
            Controls.Add(txtPhone);
            Controls.Add(txtLastName);
            Controls.Add(txtFirstName);
            Controls.Add(txtMSSV);
            Controls.Add(btnClear);
            Controls.Add(btnAdd);
            Controls.Add(lbGender);
            Controls.Add(lbPhone);
            Controls.Add(lbDateOfBirth);
            Controls.Add(lbLastName);
            Controls.Add(lbFirstName);
            Controls.Add(btnChooseImage);
            Controls.Add(picStudent);
            Margin = new Padding(3, 4, 3, 4);
            Name = "AddStudentForm";
            Text = "AddStudentForm";
            Load += AddStudentForm_Load;
            ((System.ComponentModel.ISupportInitialize)picStudent).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picStudent;
        private Button btnChooseImage;
        private Label lbFirstName;
        private Label lbLastName;
        private Label lbDateOfBirth;
        private Label lbPhone;
        private Label lbGender;
        private Button btnAdd;
        private Button btnClear;
        private TextBox txtMSSV;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhone;
        private TextBox textBox5;
        private TextBox txtAddress;
        private DateTimePicker dtpDateOfBirth;
        private TextBox txtEmail;
        private ComboBox cboGender;
        private Label lblHometown;
        private Label lbMSSV;
        private Label lbEmail;
        private Label lbAddress;
        private TextBox txtHometown;
        private PictureBox picLogo;
    }
}