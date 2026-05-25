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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            
            picStudent = new PictureBox();
            btnChooseImage = new Button();
            lbFirstName = new Label();
            lbLastName = new Label();
            lbDateOfBirth = new Label();
            lbPhone = new Label();
            lbGender = new Label();
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
            lbLastName.Location = new Point(418, 136);
            lbLastName.Name = "lbLastName";
            lbLastName.Size = new Size(33, 21);
            lbLastName.TabIndex = 4;
            lbLastName.Text = "Tên";
            // 
            // lbDateOfBirth
            // 
            lbDateOfBirth.AutoSize = true;
            lbDateOfBirth.Font = new Font("Segoe UI", 12F);
            lbDateOfBirth.Location = new Point(418, 175);
            lbDateOfBirth.Name = "lbDateOfBirth";
            lbDateOfBirth.Size = new Size(80, 21);
            lbDateOfBirth.TabIndex = 5;
            lbDateOfBirth.Text = "Ngày sinh";
            // 
            // lbPhone
            // 
            lbPhone.AutoSize = true;
            lbPhone.Font = new Font("Segoe UI", 12F);
            lbPhone.Location = new Point(418, 208);
            lbPhone.Name = "lbPhone";
            lbPhone.Size = new Size(101, 21);
            lbPhone.TabIndex = 6;
            lbPhone.Text = "Số điện thoại";
            // 
            // lbGender
            // 
            lbGender.AutoSize = true;
            lbGender.Font = new Font("Segoe UI", 12F);
            lbGender.Location = new Point(420, 312);
            lbGender.Name = "lbGender";
            lbGender.Size = new Size(70, 21);
            lbGender.TabIndex = 8;
            lbGender.Text = "Giới tính";
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
            txtEmail.Location = new Point(553, 342);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(200, 23);
            txtEmail.TabIndex = 19;
            // 
            // cboGender
            // 
            cboGender.FormattingEnabled = true;
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            cboGender.Location = new Point(553, 310);
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
            // lbMSSV
            // 
            lbMSSV.AutoSize = true;
            lbMSSV.BackColor = Color.White;
            lbMSSV.Font = new Font("Segoe UI", 12F);
            lbMSSV.Location = new Point(418, 68);
            lbMSSV.Name = "lbMSSV";
            lbMSSV.Size = new Size(52, 21);
            lbMSSV.TabIndex = 2;
            lbMSSV.Text = "MSSV";
            // 
            // lbEmail
            // 
            lbEmail.AutoSize = true;
            lbEmail.BackColor = Color.White;
            lbEmail.Font = new Font("Segoe UI", 12F);
            lbEmail.Location = new Point(420, 342);
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
            // picLogo
            // 
            picLogo.BackColor = Color.FromArgb(100, 0, 0, 0);
            picLogo.BackgroundImage = Properties.Resources.Login_ico;
            picLogo.BackgroundImageLayout = ImageLayout.Stretch;
            picLogo.Location = new Point(61, 291);
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
            Controls.Add(lbGender);
            Controls.Add(lbPhone);
            Controls.Add(lbDateOfBirth);
            Controls.Add(lbLastName);
            Controls.Add(lbFirstName);
            Controls.Add(btnChooseImage);
            Controls.Add(picStudent);
            Name = "AddStudentForm";
            Text = "AddStudentForm";
            Load += AddStudentForm_Load;
            ((System.ComponentModel.ISupportInitialize)picStudent).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox picStudent;
        private System.Windows.Forms.Button btnChooseImage;
        private System.Windows.Forms.Label lbFirstName;
        private System.Windows.Forms.Label lbLastName;
        private System.Windows.Forms.Label lbDateOfBirth;
        private System.Windows.Forms.Label lbPhone;
        private System.Windows.Forms.Label lbGender;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.DateTimePicker dtpDateOfBirth;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.ComboBox cboGender;
        private System.Windows.Forms.Label lblHometown;
        private System.Windows.Forms.Label lbMSSV;
        private System.Windows.Forms.TextBox txtMSSV;
        private System.Windows.Forms.Label lbEmail;
        private System.Windows.Forms.Label lbAddress;
        private System.Windows.Forms.TextBox txtHometown;
        private System.Windows.Forms.PictureBox picLogo;
    }
}