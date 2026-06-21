using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms
{
    partial class ForgetPassForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            ElipseForm = new Guna.UI2.WinForms.Guna2Elipse(components);
            pnlBackground = new Guna.UI2.WinForms.Guna2GradientPanel();
            btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            lblBacktoLogin = new Label();
            lblOr = new Label();
            btnReset = new Guna.UI2.WinForms.Guna2Button();
            lblConfirmPassword = new Label();
            txtConfirm = new Guna.UI2.WinForms.Guna2TextBox();
            lblNewPassword = new Label();
            txtNewPassword = new Guna.UI2.WinForms.Guna2TextBox();
            btnSendOTP = new Guna.UI2.WinForms.Guna2Button();
            txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            lblSubTitle = new Label();
            lblTitle = new Label();
            picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            pnlChatbot = new Guna.UI2.WinForms.Guna2Panel();
            rtbChatLog = new RichTextBox();
            txtChatInput = new Guna.UI2.WinForms.Guna2TextBox();
            btnSendChat = new Guna.UI2.WinForms.Guna2Button();
            btnToggleChat = new Guna.UI2.WinForms.Guna2Button();
            ElipseCard = new Guna.UI2.WinForms.Guna2Elipse(components);
            ElipseChat = new Guna.UI2.WinForms.Guna2Elipse(components);
            pnlBackground.SuspendLayout();
            pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            pnlChatbot.SuspendLayout();
            SuspendLayout();
            // 
            // ElipseForm
            // 
            ElipseForm.BorderRadius = 15;
            ElipseForm.TargetControl = this;
            // 
            // pnlBackground
            // 
            pnlBackground.Controls.Add(btnClose);
            pnlBackground.Controls.Add(pnlCard);
            pnlBackground.Controls.Add(pnlChatbot);
            pnlBackground.Controls.Add(btnToggleChat);
            pnlBackground.CustomizableEdges = customizableEdges21;
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.FillColor = Color.FromArgb(37, 120, 141);
            pnlBackground.FillColor2 = Color.FromArgb(44, 74, 92);
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Margin = new Padding(3, 4, 3, 4);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.ShadowDecoration.CustomizableEdges = customizableEdges22;
            pnlBackground.Size = new Size(1314, 960);
            pnlBackground.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.Transparent;
            btnClose.CustomizableEdges = customizableEdges1;
            btnClose.FillColor = Color.Transparent;
            btnClose.IconColor = Color.White;
            btnClose.Location = new Point(1263, 16);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnClose.Size = new Size(38, 39);
            btnClose.TabIndex = 1;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.Controls.Add(lblBacktoLogin);
            pnlCard.Controls.Add(lblOr);
            pnlCard.Controls.Add(btnReset);
            pnlCard.Controls.Add(lblConfirmPassword);
            pnlCard.Controls.Add(txtConfirm);
            pnlCard.Controls.Add(lblNewPassword);
            pnlCard.Controls.Add(txtNewPassword);
            pnlCard.Controls.Add(btnSendOTP);
            pnlCard.Controls.Add(txtEmail);
            pnlCard.Controls.Add(lblSubTitle);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(picLogo);
            pnlCard.CustomizableEdges = customizableEdges15;
            pnlCard.Location = new Point(171, 40);
            pnlCard.Margin = new Padding(3, 4, 3, 4);
            pnlCard.Name = "pnlCard";
            pnlCard.ShadowDecoration.CustomizableEdges = customizableEdges16;
            pnlCard.Size = new Size(526, 788);
            pnlCard.TabIndex = 0;
            // 
            // lblBacktoLogin
            // 
            lblBacktoLogin.Cursor = Cursors.Hand;
            lblBacktoLogin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblBacktoLogin.ForeColor = Color.FromArgb(54, 102, 240);
            lblBacktoLogin.Location = new Point(46, 716);
            lblBacktoLogin.Name = "lblBacktoLogin";
            lblBacktoLogin.Size = new Size(434, 33);
            lblBacktoLogin.TabIndex = 10;
            lblBacktoLogin.Text = "← Back to Login";
            lblBacktoLogin.TextAlign = ContentAlignment.MiddleCenter;
            lblBacktoLogin.Click += lblBacktoLogin_Click;
            // 
            // lblOr
            // 
            lblOr.Font = new Font("Segoe UI", 9F);
            lblOr.ForeColor = Color.FromArgb(170, 170, 170);
            lblOr.Location = new Point(46, 685);
            lblOr.Name = "lblOr";
            lblOr.Size = new Size(434, 20);
            lblOr.TabIndex = 9;
            lblOr.Text = "or";
            lblOr.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnReset
            // 
            btnReset.BorderRadius = 6;
            btnReset.CustomizableEdges = customizableEdges3;
            btnReset.FillColor = Color.FromArgb(28, 111, 238);
            btnReset.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(46, 609);
            btnReset.Margin = new Padding(3, 4, 3, 4);
            btnReset.Name = "btnReset";
            btnReset.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnReset.Size = new Size(434, 56);
            btnReset.TabIndex = 8;
            btnReset.Text = "Reset Password";
            btnReset.Click += btnReset_Click;
            // 
            // lblConfirmPassword
            // 
            lblConfirmPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblConfirmPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblConfirmPassword.Location = new Point(46, 498);
            lblConfirmPassword.Name = "lblConfirmPassword";
            lblConfirmPassword.Size = new Size(229, 24);
            lblConfirmPassword.TabIndex = 13;
            lblConfirmPassword.Text = "Nhập lại mật khẩu";
            // 
            // txtConfirm
            // 
            txtConfirm.BorderRadius = 6;
            txtConfirm.Cursor = Cursors.IBeam;
            txtConfirm.CustomizableEdges = customizableEdges5;
            txtConfirm.DefaultText = "";
            txtConfirm.Font = new Font("Segoe UI", 9.5F);
            txtConfirm.ForeColor = Color.Black;
            txtConfirm.Location = new Point(46, 526);
            txtConfirm.Margin = new Padding(3, 5, 3, 5);
            txtConfirm.Name = "txtConfirm";
            txtConfirm.PasswordChar = '●';
            txtConfirm.PlaceholderText = "Nhập lại mật khẩu...";
            txtConfirm.SelectedText = "";
            txtConfirm.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtConfirm.Size = new Size(434, 53);
            txtConfirm.TabIndex = 7;
            txtConfirm.UseSystemPasswordChar = true;
            // 
            // lblNewPassword
            // 
            lblNewPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNewPassword.ForeColor = Color.FromArgb(64, 64, 64);
            lblNewPassword.Location = new Point(46, 392);
            lblNewPassword.Name = "lblNewPassword";
            lblNewPassword.Size = new Size(229, 24);
            lblNewPassword.TabIndex = 12;
            lblNewPassword.Text = "Mật khẩu mới";
            // 
            // txtNewPassword
            // 
            txtNewPassword.BorderRadius = 6;
            txtNewPassword.Cursor = Cursors.IBeam;
            txtNewPassword.CustomizableEdges = customizableEdges7;
            txtNewPassword.DefaultText = "";
            txtNewPassword.Font = new Font("Segoe UI", 9.5F);
            txtNewPassword.ForeColor = Color.Black;
            txtNewPassword.Location = new Point(46, 420);
            txtNewPassword.Margin = new Padding(3, 5, 3, 5);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PasswordChar = '●';
            txtNewPassword.PlaceholderText = "At least 8 characters";
            txtNewPassword.SelectedText = "";
            txtNewPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtNewPassword.Size = new Size(434, 53);
            txtNewPassword.TabIndex = 6;
            txtNewPassword.UseSystemPasswordChar = true;
            // 
            // btnSendOTP
            // 
            btnSendOTP.BorderRadius = 6;
            btnSendOTP.CustomizableEdges = customizableEdges9;
            btnSendOTP.FillColor = Color.FromArgb(28, 111, 238);
            btnSendOTP.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSendOTP.ForeColor = Color.White;
            btnSendOTP.Location = new Point(46, 296);
            btnSendOTP.Margin = new Padding(3, 4, 3, 4);
            btnSendOTP.Name = "btnSendOTP";
            btnSendOTP.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnSendOTP.Size = new Size(434, 56);
            btnSendOTP.TabIndex = 4;
            btnSendOTP.Text = "Gửi OTP";
            btnSendOTP.Click += btnSendOTP_Click;
            // 
            // txtEmail
            // 
            txtEmail.BorderRadius = 6;
            txtEmail.Cursor = Cursors.IBeam;
            txtEmail.CustomizableEdges = customizableEdges11;
            txtEmail.DefaultText = "";
            txtEmail.Font = new Font("Segoe UI", 9.5F);
            txtEmail.ForeColor = Color.Black;
            txtEmail.Location = new Point(46, 220);
            txtEmail.Margin = new Padding(3, 5, 3, 5);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email Address";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtEmail.Size = new Size(434, 53);
            txtEmail.TabIndex = 3;
            // 
            // lblSubTitle
            // 
            lblSubTitle.Font = new Font("Segoe UI", 9F);
            lblSubTitle.ForeColor = Color.FromArgb(128, 137, 149);
            lblSubTitle.Location = new Point(34, 168);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(457, 47);
            lblSubTitle.TabIndex = 2;
            lblSubTitle.Text = "Nhập email đã đăng ký của bạn để nhận mã xác minh.";
            lblSubTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(26, 37, 48);
            lblTitle.Location = new Point(34, 117);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(457, 51);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "QUÊN MẬT KHẨU";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.CustomizableEdges = customizableEdges13;
            picLogo.Image = Properties.Resources.Login_ico;
            picLogo.ImageRotate = 0F;
            picLogo.Location = new Point(223, 20);
            picLogo.Margin = new Padding(3, 4, 3, 4);
            picLogo.Name = "picLogo";
            picLogo.ShadowDecoration.CustomizableEdges = customizableEdges14;
            picLogo.Size = new Size(80, 93);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // pnlChatbot
            // 
            pnlChatbot.BackColor = Color.White;
            pnlChatbot.Controls.Add(rtbChatLog);
            pnlChatbot.Controls.Add(txtChatInput);
            pnlChatbot.Controls.Add(btnSendChat);
            pnlChatbot.CustomizableEdges = customizableEdges19;
            pnlChatbot.Location = new Point(720, 40);
            pnlChatbot.Margin = new Padding(3, 4, 3, 4);
            pnlChatbot.Name = "pnlChatbot";
            pnlChatbot.ShadowDecoration.CustomizableEdges = customizableEdges19;
            pnlChatbot.Size = new Size(400, 705);
            pnlChatbot.TabIndex = 12;
            pnlChatbot.Visible = false;
            // 
            // rtbChatLog
            // 
            rtbChatLog.BackColor = Color.FromArgb(248, 250, 252);
            rtbChatLog.BorderStyle = BorderStyle.None;
            rtbChatLog.Font = new Font("Segoe UI", 9.5F);
            rtbChatLog.Location = new Point(17, 20);
            rtbChatLog.Margin = new Padding(3, 4, 3, 4);
            rtbChatLog.Name = "rtbChatLog";
            rtbChatLog.ReadOnly = true;
            rtbChatLog.Size = new Size(366, 592);
            rtbChatLog.TabIndex = 0;
            rtbChatLog.Text = "";
            // 
            // txtChatInput
            // 
            txtChatInput.BorderRadius = 6;
            txtChatInput.Cursor = Cursors.IBeam;
            txtChatInput.CustomizableEdges = customizableEdges17;
            txtChatInput.DefaultText = "";
            txtChatInput.Font = new Font("Segoe UI", 9.5F);
            txtChatInput.ForeColor = Color.Black;
            txtChatInput.Location = new Point(17, 632);
            txtChatInput.Margin = new Padding(3, 5, 3, 5);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.PlaceholderText = "Nhập câu hỏi tại đây...";
            txtChatInput.SelectedText = "";
            txtChatInput.ShadowDecoration.CustomizableEdges = customizableEdges17;
            txtChatInput.Size = new Size(297, 53);
            txtChatInput.TabIndex = 1;
            txtChatInput.KeyDown += txtChatInput_KeyDown;
            // 
            // btnSendChat
            // 
            btnSendChat.BorderRadius = 6;
            btnSendChat.CustomizableEdges = customizableEdges18;
            btnSendChat.FillColor = Color.FromArgb(28, 111, 238);
            btnSendChat.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSendChat.ForeColor = Color.White;
            btnSendChat.Location = new Point(326, 632);
            btnSendChat.Margin = new Padding(3, 4, 3, 4);
            btnSendChat.Name = "btnSendChat";
            btnSendChat.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btnSendChat.Size = new Size(57, 53);
            btnSendChat.TabIndex = 2;
            btnSendChat.Text = "➔";
            btnSendChat.Click += btnSendChat_Click;
            // 
            // btnToggleChat
            // 
            btnToggleChat.BorderRadius = 20;
            btnToggleChat.CustomizableEdges = customizableEdges20;
            btnToggleChat.FillColor = Color.FromArgb(16, 185, 129);
            btnToggleChat.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnToggleChat.ForeColor = Color.White;
            btnToggleChat.Location = new Point(720, 768);
            btnToggleChat.Margin = new Padding(3, 4, 3, 4);
            btnToggleChat.Name = "btnToggleChat";
            btnToggleChat.ShadowDecoration.CustomizableEdges = customizableEdges20;
            btnToggleChat.Size = new Size(171, 60);
            btnToggleChat.TabIndex = 11;
            btnToggleChat.Text = "💬 Trợ lý AI";
            btnToggleChat.Click += btnToggleChat_Click;
            // 
            // ElipseCard
            // 
            ElipseCard.BorderRadius = 20;
            ElipseCard.TargetControl = pnlCard;
            // 
            // ElipseChat
            // 
            ElipseChat.BorderRadius = 20;
            ElipseChat.TargetControl = pnlChatbot;
            // 
            // ForgetPassForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1314, 960);
            Controls.Add(pnlBackground);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "ForgetPassForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Forget Password";
            pnlBackground.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            pnlChatbot.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse ElipseForm;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlBackground;
        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private Guna.UI2.WinForms.Guna2Elipse ElipseCard;
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2Button btnSendOTP;
        private System.Windows.Forms.Label lblNewPassword;
        private Guna.UI2.WinForms.Guna2TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private Guna.UI2.WinForms.Guna2TextBox txtConfirm;
        private Guna.UI2.WinForms.Guna2Button btnReset;
        private System.Windows.Forms.Label lblOr;
        private System.Windows.Forms.Label lblBacktoLogin;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;

        // Các biến thành phần Chatbot Guna2 được khai báo mới
        private Guna.UI2.WinForms.Guna2Button btnToggleChat;
        private Guna.UI2.WinForms.Guna2Panel pnlChatbot;
        private System.Windows.Forms.RichTextBox rtbChatLog;
        private Guna.UI2.WinForms.Guna2TextBox txtChatInput;
        private Guna.UI2.WinForms.Guna2Button btnSendChat;
        private Guna.UI2.WinForms.Guna2Elipse ElipseChat;
    }
}