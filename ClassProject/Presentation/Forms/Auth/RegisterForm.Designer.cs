namespace ClassProject.Presentation.Forms
{
    partial class RegisterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForm));
            pictureBox1 = new PictureBox();
            lblCreatAcount = new Label();
            txtUsername = new TextBox();
            txtEmail = new TextBox();
            txtPassword = new TextBox();
            txtConfirm = new TextBox();
            chkAgree = new CheckBox();
            btnRegister = new Button();
            pnlCreateAccount = new Panel();
            txtPosition = new Label();
            cboPosition = new ComboBox();
            lblBacktoLogin = new Label();
            lblConfirmPassword = new Label();
            lblPassword = new Label();
            lblEmail = new Label();
            lblUsername = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlCreateAccount.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(116, 13);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(85, 69);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // lblCreatAcount
            // 
            lblCreatAcount.AutoSize = true;
            lblCreatAcount.BackColor = Color.Transparent;
            lblCreatAcount.Font = new Font("Segoe UI", 19.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCreatAcount.ForeColor = Color.Black;
            lblCreatAcount.Location = new Point(50, 83);
            lblCreatAcount.Margin = new Padding(2, 0, 2, 0);
            lblCreatAcount.Name = "lblCreatAcount";
            lblCreatAcount.Size = new Size(213, 37);
            lblCreatAcount.TabIndex = 2;
            lblCreatAcount.Text = "Create Account";
            lblCreatAcount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(34, 144);
            txtUsername.Margin = new Padding(3, 2, 3, 2);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Username";
            txtUsername.Size = new Size(246, 23);
            txtUsername.TabIndex = 5;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(34, 189);
            txtEmail.Margin = new Padding(3, 2, 3, 2);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.Size = new Size(246, 23);
            txtEmail.TabIndex = 6;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(34, 237);
            txtPassword.Margin = new Padding(3, 2, 3, 2);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Password";
            txtPassword.Size = new Size(112, 23);
            txtPassword.TabIndex = 7;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirm
            // 
            txtConfirm.Location = new Point(169, 237);
            txtConfirm.Margin = new Padding(3, 2, 3, 2);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.PlaceholderText = "Confirm Password";
            txtConfirm.Size = new Size(112, 23);
            txtConfirm.TabIndex = 8;
            txtConfirm.UseSystemPasswordChar = true;
            // 
            // chkAgree
            // 
            chkAgree.AutoSize = true;
            chkAgree.BackColor = Color.Transparent;
            chkAgree.ForeColor = Color.Red;
            chkAgree.Location = new Point(169, 281);
            chkAgree.Margin = new Padding(3, 2, 3, 2);
            chkAgree.Name = "chkAgree";
            chkAgree.Size = new Size(108, 19);
            chkAgree.TabIndex = 9;
            chkAgree.Text = "I agree to terms";
            chkAgree.UseVisualStyleBackColor = false;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(35, 309);
            btnRegister.Margin = new Padding(3, 2, 3, 2);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(246, 22);
            btnRegister.TabIndex = 10;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // pnlCreateAccount
            // 
            pnlCreateAccount.BorderStyle = BorderStyle.FixedSingle;
            pnlCreateAccount.Controls.Add(txtPosition);
            pnlCreateAccount.Controls.Add(chkAgree);
            pnlCreateAccount.Controls.Add(cboPosition);
            pnlCreateAccount.Controls.Add(lblBacktoLogin);
            pnlCreateAccount.Controls.Add(lblConfirmPassword);
            pnlCreateAccount.Controls.Add(lblPassword);
            pnlCreateAccount.Controls.Add(lblEmail);
            pnlCreateAccount.Controls.Add(lblUsername);
            pnlCreateAccount.Controls.Add(lblCreatAcount);
            pnlCreateAccount.Controls.Add(txtPassword);
            pnlCreateAccount.Controls.Add(pictureBox1);
            pnlCreateAccount.Controls.Add(btnRegister);
            pnlCreateAccount.Controls.Add(txtUsername);
            pnlCreateAccount.Controls.Add(txtEmail);
            pnlCreateAccount.Controls.Add(txtConfirm);
            pnlCreateAccount.Location = new Point(180, 9);
            pnlCreateAccount.Margin = new Padding(3, 2, 3, 2);
            pnlCreateAccount.Name = "pnlCreateAccount";
            pnlCreateAccount.Size = new Size(316, 364);
            pnlCreateAccount.TabIndex = 12;
            // 
            // txtPosition
            // 
            txtPosition.AutoSize = true;
            txtPosition.Location = new Point(35, 262);
            txtPosition.Name = "txtPosition";
            txtPosition.Size = new Size(50, 15);
            txtPosition.TabIndex = 21;
            txtPosition.Text = "Position";
            // 
            // cboPosition
            // 
            cboPosition.FormattingEnabled = true;
            cboPosition.Items.AddRange(new object[] { "Admin", "Student", "HR" });
            cboPosition.Location = new Point(35, 281);
            cboPosition.Name = "cboPosition";
            cboPosition.Size = new Size(113, 23);
            cboPosition.TabIndex = 20;
            // 
            // lblBacktoLogin
            // 
            lblBacktoLogin.AutoSize = true;
            lblBacktoLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            lblBacktoLogin.Location = new Point(32, 333);
            lblBacktoLogin.Name = "lblBacktoLogin";
            lblBacktoLogin.Size = new Size(79, 15);
            lblBacktoLogin.TabIndex = 19;
            lblBacktoLogin.Text = "Back to Login";
            lblBacktoLogin.Click += lblBacktoLogin_Click;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(167, 219);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(104, 15);
            lblConfirmPassword.TabIndex = 15;
            lblConfirmPassword.Text = "Confirm Password";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(33, 219);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(57, 15);
            lblPassword.TabIndex = 14;
            lblPassword.Text = "Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(32, 170);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(36, 15);
            lblEmail.TabIndex = 13;
            lblEmail.Text = "Email";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(32, 123);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(60, 15);
            lblUsername.TabIndex = 12;
            lblUsername.Text = "Username";
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(668, 380);
            Controls.Add(pnlCreateAccount);
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "RegisterForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Register";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlCreateAccount.ResumeLayout(false);
            pnlCreateAccount.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblCreatAcount;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private CheckBox chkAgree;
        private Button btnRegister;
        private Panel pnlCreateAccount;
        private Label lblConfirmPassword;
        private Label lblPassword;
        private Label lblEmail;
        private Label lblUsername;
        private Label lblBacktoLogin;
        private ComboBox cboPosition;
        private Label txtPosition;
    }
}