namespace ClassProject.Presentation.Forms
{
    partial class ForgetPassForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForgetPassForm));
            pnlCreateAccount = new Panel();
            lblBacktoLogin = new Label();
            btnReset = new Button();
            btnSendOTP = new Button();
            lblConfirmPassword = new Label();
            lblNewPassword = new Label();
            lblEmail = new Label();
            lblOTP = new Label();
            lblResetPassword = new Label();
            txtNewPassword = new TextBox();
            pictureBox1 = new PictureBox();
            txtOTP = new TextBox();
            txtEmail = new TextBox();
            txtConfirm = new TextBox();
            pnlCreateAccount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pnlCreateAccount
            // 
            pnlCreateAccount.BorderStyle = BorderStyle.FixedSingle;
            pnlCreateAccount.Controls.Add(lblBacktoLogin);
            pnlCreateAccount.Controls.Add(btnReset);
            pnlCreateAccount.Controls.Add(btnSendOTP);
            pnlCreateAccount.Controls.Add(lblConfirmPassword);
            pnlCreateAccount.Controls.Add(lblNewPassword);
            pnlCreateAccount.Controls.Add(lblEmail);
            pnlCreateAccount.Controls.Add(lblOTP);
            pnlCreateAccount.Controls.Add(lblResetPassword);
            pnlCreateAccount.Controls.Add(txtNewPassword);
            pnlCreateAccount.Controls.Add(pictureBox1);
            pnlCreateAccount.Controls.Add(txtOTP);
            pnlCreateAccount.Controls.Add(txtEmail);
            pnlCreateAccount.Controls.Add(txtConfirm);
            pnlCreateAccount.Location = new Point(177, 10);
            pnlCreateAccount.Margin = new Padding(3, 2, 3, 2);
            pnlCreateAccount.Name = "pnlCreateAccount";
            pnlCreateAccount.Size = new Size(316, 372);
            pnlCreateAccount.TabIndex = 13;
            // 
            // lblBacktoLogin
            // 
            lblBacktoLogin.AutoSize = true;
            lblBacktoLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            lblBacktoLogin.Location = new Point(110, 340);
            lblBacktoLogin.Name = "lblBacktoLogin";
            lblBacktoLogin.Size = new Size(79, 15);
            lblBacktoLogin.TabIndex = 18;
            lblBacktoLogin.Text = "Back to Login";
            lblBacktoLogin.Click += lblBacktoLogin_Click;
            // 
            // btnReset
            // 
            btnReset.BackColor = SystemColors.ActiveCaption;
            btnReset.Location = new Point(32, 303);
            btnReset.Margin = new Padding(3, 2, 3, 2);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(246, 27);
            btnReset.TabIndex = 17;
            btnReset.Text = "Reset Password";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // btnSendOTP
            // 
            btnSendOTP.BackColor = SystemColors.ActiveCaption;
            btnSendOTP.Location = new Point(34, 128);
            btnSendOTP.Margin = new Padding(3, 2, 3, 2);
            btnSendOTP.Name = "btnSendOTP";
            btnSendOTP.Size = new Size(246, 26);
            btnSendOTP.TabIndex = 16;
            btnSendOTP.Text = "Send OTP";
            btnSendOTP.UseVisualStyleBackColor = false;
            btnSendOTP.Click += btnSendOTP_Click;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(32, 254);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(104, 15);
            lblConfirmPassword.TabIndex = 15;
            lblConfirmPassword.Text = "Confirm Password";
            // 
            // lblNewPassword
            // 
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new Point(33, 206);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(84, 15);
            lblNewPassword.TabIndex = 14;
            lblNewPassword.Text = "New Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(31, 82);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(36, 15);
            lblEmail.TabIndex = 13;
            lblEmail.Text = "Email";
            // 
            // lblOTP
            // 
            lblOTP.AutoSize = true;
            lblOTP.Location = new Point(32, 157);
            lblOTP.Name = "lblOTP";
            lblOTP.Size = new Size(28, 15);
            lblOTP.TabIndex = 12;
            lblOTP.Text = "OTP";
            // 
            // lblResetPassword
            // 
            lblResetPassword.AutoSize = true;
            lblResetPassword.BackColor = Color.Transparent;
            lblResetPassword.Font = new Font("Segoe UI", 19.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblResetPassword.ForeColor = Color.Black;
            lblResetPassword.Location = new Point(46, 52);
            lblResetPassword.Margin = new Padding(2, 0, 2, 0);
            lblResetPassword.Name = "lblResetPassword";
            lblResetPassword.Size = new Size(216, 37);
            lblResetPassword.TabIndex = 2;
            lblResetPassword.Text = "Reset Password";
            lblResetPassword.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtNewPassword
            // 
            txtNewPassword.Location = new Point(34, 224);
            txtNewPassword.Margin = new Padding(3, 2, 3, 2);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PlaceholderText = "New Password";
            txtNewPassword.Size = new Size(245, 23);
            txtNewPassword.TabIndex = 7;
            txtNewPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(129, 2);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 50);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // txtOTP
            // 
            txtOTP.Location = new Point(34, 175);
            txtOTP.Margin = new Padding(3, 2, 3, 2);
            txtOTP.Name = "txtOTP";
            txtOTP.PlaceholderText = "Enter OTP";
            txtOTP.Size = new Size(246, 23);
            txtOTP.TabIndex = 5;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(32, 100);
            txtEmail.Margin = new Padding(3, 2, 3, 2);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.Size = new Size(246, 23);
            txtEmail.TabIndex = 6;
            // 
            // txtConfirm
            // 
            txtConfirm.Location = new Point(34, 272);
            txtConfirm.Margin = new Padding(3, 2, 3, 2);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.PlaceholderText = "Confirm Password";
            txtConfirm.Size = new Size(245, 23);
            txtConfirm.TabIndex = 8;
            txtConfirm.UseSystemPasswordChar = true;
            // 
            // ForgetPassForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(668, 390);
            Controls.Add(pnlCreateAccount);
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "ForgetPassForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ForgetPass";
            pnlCreateAccount.ResumeLayout(false);
            pnlCreateAccount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlCreateAccount;
        private Label lblConfirmPassword;
        private Label lblNewPassword;
        private Label lblEmail;
        private Label lblOTP;
        private Label lblResetPassword;
        private TextBox txtNewPassword;
        private PictureBox pictureBox1;
        private TextBox txtOTP;
        private TextBox txtEmail;
        private TextBox txtConfirm;
        private Button btnSendOTP;
        private Label lblBacktoLogin;
        private Button btnReset;
    }
}