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
            btnToggleChat = new Button();
            pnlChatbot = new Panel();
            rtbChatLog = new RichTextBox();
            txtChatInput = new TextBox();
            btnSendChat = new Button();
            pnlCreateAccount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlChatbot.SuspendLayout();
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
            pnlCreateAccount.Location = new Point(202, 13);
            pnlCreateAccount.Name = "pnlCreateAccount";
            pnlCreateAccount.Size = new Size(361, 495);
            pnlCreateAccount.TabIndex = 13;
            // 
            // lblBacktoLogin
            // 
            lblBacktoLogin.AutoSize = true;
            lblBacktoLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            lblBacktoLogin.Location = new Point(131, 453);
            lblBacktoLogin.Name = "lblBacktoLogin";
            lblBacktoLogin.Size = new Size(99, 20);
            lblBacktoLogin.TabIndex = 18;
            lblBacktoLogin.Text = "Back to Login";
            lblBacktoLogin.Click += lblBacktoLogin_Click;
            // 
            // btnReset
            // 
            btnReset.BackColor = SystemColors.ActiveCaption;
            btnReset.Location = new Point(37, 367);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(281, 40);
            btnReset.TabIndex = 17;
            btnReset.Text = "Update Password";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // btnSendOTP
            // 
            btnSendOTP.BackColor = SystemColors.ActiveCaption;
            btnSendOTP.Location = new Point(37, 171);
            btnSendOTP.Name = "btnSendOTP";
            btnSendOTP.Size = new Size(281, 37);
            btnSendOTP.TabIndex = 16;
            btnSendOTP.Text = "Send OTP Verification";
            btnSendOTP.UseVisualStyleBackColor = false;
            btnSendOTP.Click += btnSendOTP_Click;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.AutoSize = true;
            lblConfirmPassword.Location = new Point(35, 291);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(127, 20);
            lblConfirmPassword.TabIndex = 15;
            lblConfirmPassword.Text = "Confirm Password";
            // 
            // lblNewPassword
            // 
            lblNewPassword.AutoSize = true;
            lblNewPassword.Location = new Point(35, 224);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(104, 20);
            lblNewPassword.TabIndex = 14;
            lblNewPassword.Text = "New Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(35, 109);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(103, 20);
            lblEmail.TabIndex = 13;
            lblEmail.Text = "Email Address";
            // 
            // lblOTP
            // 
            lblOTP.AutoSize = true;
            lblOTP.Location = new Point(0, 0);
            lblOTP.Name = "lblOTP";
            lblOTP.Size = new Size(35, 20);
            lblOTP.TabIndex = 12;
            lblOTP.Text = "OTP";
            lblOTP.Visible = false;
            // 
            // lblResetPassword
            // 
            lblResetPassword.AutoSize = true;
            lblResetPassword.BackColor = Color.Transparent;
            lblResetPassword.Font = new Font("Segoe UI", 19.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblResetPassword.ForeColor = Color.Black;
            lblResetPassword.Location = new Point(53, 69);
            lblResetPassword.Margin = new Padding(2, 0, 2, 0);
            lblResetPassword.Name = "lblResetPassword";
            lblResetPassword.Size = new Size(265, 46);
            lblResetPassword.TabIndex = 2;
            lblResetPassword.Text = "Reset Password";
            lblResetPassword.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtNewPassword
            // 
            txtNewPassword.Location = new Point(37, 248);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PlaceholderText = "New Password";
            txtNewPassword.Size = new Size(281, 27);
            txtNewPassword.TabIndex = 7;
            txtNewPassword.UseSystemPasswordChar = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(147, 3);
            pictureBox1.Margin = new Padding(2, 3, 2, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(74, 67);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // txtOTP
            // 
            txtOTP.Location = new Point(0, 0);
            txtOTP.Margin = new Padding(3, 4, 3, 4);
            txtOTP.Name = "txtOTP";
            txtOTP.Size = new Size(114, 27);
            txtOTP.TabIndex = 5;
            txtOTP.Visible = false;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(37, 133);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Enter your email";
            txtEmail.Size = new Size(281, 27);
            txtEmail.TabIndex = 6;
            // 
            // txtConfirm
            // 
            txtConfirm.Location = new Point(37, 315);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.PlaceholderText = "Confirm Password";
            txtConfirm.Size = new Size(281, 27);
            txtConfirm.TabIndex = 8;
            txtConfirm.UseSystemPasswordChar = true;
            // 
            // btnToggleChat
            // 
            btnToggleChat.BackColor = SystemColors.ActiveCaption;
            btnToggleChat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnToggleChat.Location = new Point(569, 468);
            btnToggleChat.Name = "btnToggleChat";
            btnToggleChat.Size = new Size(182, 40);
            btnToggleChat.TabIndex = 19;
            btnToggleChat.Text = "💬 Trợ lý AI";
            btnToggleChat.UseVisualStyleBackColor = false;
            btnToggleChat.Click += btnToggleChat_Click;
            // 
            // pnlChatbot
            // 
            pnlChatbot.BackColor = Color.White;
            pnlChatbot.BorderStyle = BorderStyle.FixedSingle;
            pnlChatbot.Controls.Add(rtbChatLog);
            pnlChatbot.Controls.Add(txtChatInput);
            pnlChatbot.Controls.Add(btnSendChat);
            pnlChatbot.Location = new Point(569, 13);
            pnlChatbot.Name = "pnlChatbot";
            pnlChatbot.Size = new Size(182, 449);
            pnlChatbot.TabIndex = 20;
            pnlChatbot.Visible = false;
            // 
            // rtbChatLog
            // 
            rtbChatLog.BackColor = Color.FromArgb(248, 250, 252);
            rtbChatLog.BorderStyle = BorderStyle.None;
            rtbChatLog.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rtbChatLog.Location = new Point(3, 3);
            rtbChatLog.Name = "rtbChatLog";
            rtbChatLog.ReadOnly = true;
            rtbChatLog.Size = new Size(174, 395);
            rtbChatLog.TabIndex = 0;
            rtbChatLog.Text = "";
            // 
            // txtChatInput
            // 
            txtChatInput.Location = new Point(3, 413);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.PlaceholderText = "Hỏi AI...";
            txtChatInput.Size = new Size(130, 27);
            txtChatInput.TabIndex = 1;
            txtChatInput.KeyDown += txtChatInput_KeyDown;
            // 
            // btnSendChat
            // 
            btnSendChat.BackColor = SystemColors.ButtonFace;
            btnSendChat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSendChat.Location = new Point(139, 412);
            btnSendChat.Name = "btnSendChat";
            btnSendChat.Size = new Size(38, 29);
            btnSendChat.TabIndex = 2;
            btnSendChat.Text = "➔";
            btnSendChat.UseVisualStyleBackColor = false;
            btnSendChat.Click += btnSendChat_Click;
            // 
            // ForgetPassForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(763, 520);
            Controls.Add(pnlChatbot);
            Controls.Add(btnToggleChat);
            Controls.Add(pnlCreateAccount);
            MaximizeBox = false;
            Name = "ForgetPassForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Forget Password";
            pnlCreateAccount.ResumeLayout(false);
            pnlCreateAccount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlChatbot.ResumeLayout(false);
            pnlChatbot.PerformLayout();
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
        private Button btnToggleChat;
        private Panel pnlChatbot;
        private RichTextBox rtbChatLog;
        private TextBox txtChatInput;
        private Button btnSendChat;
    }
}