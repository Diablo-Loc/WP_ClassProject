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
            btnOpenScanner = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlCreateAccount.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(133, 17);
            pictureBox1.Margin = new Padding(2, 3, 2, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(97, 92);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // lblCreatAcount
            // 
            lblCreatAcount.AutoSize = true;
            lblCreatAcount.BackColor = Color.Transparent;
            lblCreatAcount.Font = new Font("Segoe UI", 19.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCreatAcount.ForeColor = Color.Black;
            lblCreatAcount.Location = new Point(57, 111);
            lblCreatAcount.Margin = new Padding(2, 0, 2, 0);
            lblCreatAcount.Name = "lblCreatAcount";
            lblCreatAcount.Size = new Size(263, 46);
            lblCreatAcount.TabIndex = 2;
            lblCreatAcount.Text = "Create Account";
            lblCreatAcount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(39, 192);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Username";
            txtUsername.Size = new Size(281, 27);
            txtUsername.TabIndex = 5;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(39, 252);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.Size = new Size(281, 27);
            txtEmail.TabIndex = 6;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(39, 316);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Password";
            txtPassword.Size = new Size(127, 27);
            txtPassword.TabIndex = 7;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirm
            // 
            txtConfirm.Location = new Point(193, 316);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.PlaceholderText = "Confirm Password";
            txtConfirm.Size = new Size(127, 27);
            txtConfirm.TabIndex = 8;
            txtConfirm.UseSystemPasswordChar = true;
            // 
            // chkAgree
            // 
            chkAgree.AutoSize = true;
            chkAgree.BackColor = Color.Transparent;
            chkAgree.ForeColor = Color.Red;
            chkAgree.Location = new Point(193, 375);
            chkAgree.Name = "chkAgree";
            chkAgree.Size = new Size(136, 24);
            chkAgree.TabIndex = 9;
            chkAgree.Text = "I agree to terms";
            chkAgree.UseVisualStyleBackColor = false;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(40, 412);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(281, 29);
            btnRegister.TabIndex = 10;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // pnlCreateAccount
            // 
            pnlCreateAccount.BorderStyle = BorderStyle.FixedSingle;
            pnlCreateAccount.Controls.Add(btnOpenScanner);
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
            pnlCreateAccount.Location = new Point(206, 12);
            pnlCreateAccount.Name = "pnlCreateAccount";
            pnlCreateAccount.Size = new Size(361, 485);
            pnlCreateAccount.TabIndex = 12;
            // 
            // txtPosition
            // 
            txtPosition.AutoSize = true;
            txtPosition.Location = new Point(40, 349);
            txtPosition.Name = "txtPosition";
            txtPosition.Size = new Size(61, 20);
            txtPosition.TabIndex = 21;
            txtPosition.Text = "Position";
            // 
            // cboPosition
            // 
            cboPosition.FormattingEnabled = true;
            cboPosition.Items.AddRange(new object[] { "Student", "HR" });
            cboPosition.Location = new Point(40, 375);
            cboPosition.Margin = new Padding(3, 4, 3, 4);
            cboPosition.Name = "cboPosition";
            cboPosition.Size = new Size(129, 28);
            cboPosition.TabIndex = 20;
            // 
            // lblBacktoLogin
            // 
            lblBacktoLogin.AutoSize = true;
            lblBacktoLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            lblBacktoLogin.Location = new Point(37, 444);
            lblBacktoLogin.Name = "lblBacktoLogin";
            lblBacktoLogin.Size = new Size(99, 20);
            lblBacktoLogin.TabIndex = 19;
            lblBacktoLogin.Text = "Back to Login";
            lblBacktoLogin.Click += lblBacktoLogin_Click;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(191, 292);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(127, 20);
            lblConfirmPassword.TabIndex = 15;
            lblConfirmPassword.Text = "Confirm Password";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(38, 292);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(70, 20);
            lblPassword.TabIndex = 14;
            lblPassword.Text = "Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(37, 227);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(46, 20);
            lblEmail.TabIndex = 13;
            lblEmail.Text = "Email";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(37, 164);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(75, 20);
            lblUsername.TabIndex = 12;
            lblUsername.Text = "Username";
            // 
            // btnOpenScanner
            // 
            btnOpenScanner.Location = new Point(234, 447);
            btnOpenScanner.Name = "btnOpenScanner";
            btnOpenScanner.Size = new Size(95, 29);
            btnOpenScanner.TabIndex = 22;
            btnOpenScanner.Text = "📷 Quét thẻ SV để điền nhanh";
            btnOpenScanner.UseVisualStyleBackColor = true;
            btnOpenScanner.Click += btnOpenScanner_Click;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(763, 507);
            Controls.Add(pnlCreateAccount);
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
        private Button btnOpenScanner;
    }
}