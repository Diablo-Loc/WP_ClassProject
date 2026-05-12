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
            lbMSSV = new Label();
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
            lbEmail = new Label();
            lbAddress = new Label();
            txtHometown = new TextBox();
            pnlBackground = new Panel();
            picLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picStudent).BeginInit();
            pnlBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // picStudent
            // 
            picStudent.BorderStyle = BorderStyle.FixedSingle;
            picStudent.Location = new Point(35, 68);
            picStudent.Name = "picStudent";
            picStudent.Size = new Size(164, 156);
            picStudent.SizeMode = PictureBoxSizeMode.StretchImage;
            picStudent.TabIndex = 0;
            picStudent.TabStop = false;
            // 
            // btnChooseImage
            // 
            btnChooseImage.Location = new Point(35, 243);
            btnChooseImage.Name = "btnChooseImage";
            btnChooseImage.Size = new Size(164, 30);
            btnChooseImage.TabIndex = 1;
            btnChooseImage.Text = "ChooseImage";
            btnChooseImage.UseVisualStyleBackColor = true;
            btnChooseImage.Click += btnChooseImage_Click;
            // 
            // lbMSSV
            // 
            lbMSSV.AutoSize = true;
            lbMSSV.BackColor = Color.White;
            lbMSSV.Font = new Font("Segoe UI", 12F);
            lbMSSV.Location = new Point(418, 64);
            lbMSSV.Name = "lbMSSV";
            lbMSSV.Size = new Size(52, 21);
            lbMSSV.TabIndex = 2;
            lbMSSV.Text = "MSSV";
            // 
            // lbFirstName
            // 
            lbFirstName.AutoSize = true;
            lbFirstName.BackColor = Color.White;
            lbFirstName.Font = new Font("Segoe UI", 12F);
            lbFirstName.Location = new Point(419, 101);
            lbFirstName.Name = "lbFirstName";
            lbFirstName.Size = new Size(111, 21);
            lbFirstName.TabIndex = 3;
            lbFirstName.Text = "Họ và tên đệm";
            // 
            // lbLastName
            // 
            lbLastName.AutoSize = true;
            lbLastName.Font = new Font("Segoe UI", 12F);
            lbLastName.Location = new Point(419, 136);
            lbLastName.Name = "lbLastName";
            lbLastName.Size = new Size(33, 21);
            lbLastName.TabIndex = 4;
            lbLastName.Text = "Tên";
            // 
            // lbDateOfBirth
            // 
            lbDateOfBirth.AutoSize = true;
            lbDateOfBirth.Font = new Font("Segoe UI", 12F);
            lbDateOfBirth.Location = new Point(419, 173);
            lbDateOfBirth.Name = "lbDateOfBirth";
            lbDateOfBirth.Size = new Size(80, 21);
            lbDateOfBirth.TabIndex = 5;
            lbDateOfBirth.Text = "Ngày sinh";
            // 
            // lbPhone
            // 
            lbPhone.AutoSize = true;
            lbPhone.Font = new Font("Segoe UI", 12F);
            lbPhone.Location = new Point(419, 214);
            lbPhone.Name = "lbPhone";
            lbPhone.Size = new Size(101, 21);
            lbPhone.TabIndex = 6;
            lbPhone.Text = "Số điện thoại";
            // 
            // lbGender
            // 
            lbGender.AutoSize = true;
            lbGender.Font = new Font("Segoe UI", 12F);
            lbGender.Location = new Point(420, 308);
            lbGender.Name = "lbGender";
            lbGender.Size = new Size(70, 21);
            lbGender.TabIndex = 8;
            lbGender.Text = "Giới tính";
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(245, 371);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(115, 40);
            btnAdd.TabIndex = 9;
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(419, 371);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(115, 40);
            btnClear.TabIndex = 10;
            btnClear.Text = "CLEAR";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // txtMSSV
            // 
            txtMSSV.Location = new Point(553, 66);
            txtMSSV.Name = "txtMSSV";
            txtMSSV.Size = new Size(200, 23);
            txtMSSV.TabIndex = 11;
            // 
            // txtFirstName
            // 
            txtFirstName.Location = new Point(553, 101);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(200, 23);
            txtFirstName.TabIndex = 12;
            // 
            // txtLastName
            // 
            txtLastName.Location = new Point(553, 136);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(200, 23);
            txtLastName.TabIndex = 13;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(553, 206);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(200, 23);
            txtPhone.TabIndex = 14;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(553, 244);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(200, 23);
            txtAddress.TabIndex = 16;
            // 
            // dtpDateOfBirth
            // 
            dtpDateOfBirth.Location = new Point(553, 173);
            dtpDateOfBirth.Name = "dtpDateOfBirth";
            dtpDateOfBirth.Size = new Size(200, 23);
            dtpDateOfBirth.TabIndex = 18;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(553, 340);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(200, 23);
            txtEmail.TabIndex = 19;
            // 
            // cboGender
            // 
            cboGender.FormattingEnabled = true;
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGender.Location = new Point(553, 305);
            cboGender.Name = "cboGender";
            cboGender.Size = new Size(198, 23);
            cboGender.TabIndex = 20;
            // 
            // lblHometown
            // 
            lblHometown.AutoSize = true;
            lblHometown.Font = new Font("Segoe UI", 12F);
            lblHometown.Location = new Point(418, 278);
            lblHometown.Name = "lblHometown";
            lblHometown.Size = new Size(88, 21);
            lblHometown.TabIndex = 21;
            lblHometown.Text = "HomeTown";
            // 
            // lbEmail
            // 
            lbEmail.AutoSize = true;
            lbEmail.BackColor = Color.White;
            lbEmail.Font = new Font("Segoe UI", 12F);
            lbEmail.Location = new Point(420, 340);
            lbEmail.Name = "lbEmail";
            lbEmail.Size = new Size(48, 21);
            lbEmail.TabIndex = 17;
            lbEmail.Text = "Email";
            // 
            // lbAddress
            // 
            lbAddress.AutoSize = true;
            lbAddress.BackColor = Color.White;
            lbAddress.Font = new Font("Segoe UI", 12F);
            lbAddress.Location = new Point(418, 246);
            lbAddress.Name = "lbAddress";
            lbAddress.Size = new Size(61, 21);
            lbAddress.TabIndex = 7;
            lbAddress.Text = "Địa chỉ ";
            // 
            // txtHometown
            // 
            txtHometown.Location = new Point(553, 276);
            txtHometown.Name = "txtHometown";
            txtHometown.Size = new Size(200, 23);
            txtHometown.TabIndex = 22;
            // 
            // pnlBackground
            // 
            pnlBackground.BackColor = Color.FromArgb(90, 0, 0, 0);
            pnlBackground.Controls.Add(picLogo);
            pnlBackground.Controls.Add(txtHometown);
            pnlBackground.Controls.Add(lbAddress);
            pnlBackground.Controls.Add(lbEmail);
            pnlBackground.Controls.Add(lbMSSV);
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.Size = new Size(800, 450);
            pnlBackground.TabIndex = 23;
            pnlBackground.Paint += panel1_Paint;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.FromArgb(100, 0, 0, 0);
            picLogo.BackgroundImage = Properties.Resources.Login_ico;
            picLogo.BackgroundImageLayout = ImageLayout.Stretch;
            picLogo.Location = new Point(60, 291);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(117, 120);
            picLogo.TabIndex = 23;
            picLogo.TabStop = false;
            // 
            // AddStudentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.createSV;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(lblHometown);
            Controls.Add(cboGender);
            Controls.Add(txtEmail);
            Controls.Add(dtpDateOfBirth);
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
            Controls.Add(pnlBackground);
            Name = "AddStudentForm";
            Text = "AddStudentForm";
            Load += AddStudentForm_Load;
            ((System.ComponentModel.ISupportInitialize)picStudent).EndInit();
            pnlBackground.ResumeLayout(false);
            pnlBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picStudent;
        private Button btnChooseImage;
        private Label lbMSSV;
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
        private Label lbEmail;
        private Label lbAddress;
        private TextBox txtHometown;
        private Panel pnlBackground;
        private PictureBox picLogo;
    }
}