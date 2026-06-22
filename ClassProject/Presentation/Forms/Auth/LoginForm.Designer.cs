namespace ClassProject
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2GradientPanel panelBackground;
        private Guna.UI2.WinForms.Guna2ShadowPanel panelLoginCard;
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtUsername;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2CheckBox chkRememberMe;
        private System.Windows.Forms.Label lblForgetPassword;
        private Guna.UI2.WinForms.Guna2Button btnLogin;
        private Guna.UI2.WinForms.Guna2Button btnOpenFaceID;
        private System.Windows.Forms.Label lblRegister;
        private Guna.UI2.WinForms.Guna2ControlBox btnCloseForm;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelBackground = new Guna.UI2.WinForms.Guna2GradientPanel();
            btnCloseForm = new Guna.UI2.WinForms.Guna2ControlBox();
            panelLoginCard = new Guna.UI2.WinForms.Guna2ShadowPanel();
            chkShowPassword = new Guna.UI2.WinForms.Guna2CheckBox();
            pictureBox1 = new PictureBox();
            lblRegister = new Label();
            btnOpenFaceID = new Guna.UI2.WinForms.Guna2Button();
            btnLogin = new Guna.UI2.WinForms.Guna2Button();
            lblForgetPassword = new Label();
            chkRememberMe = new Guna.UI2.WinForms.Guna2CheckBox();
            txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            txtUsername = new Guna.UI2.WinForms.Guna2TextBox();
            lblSubTitle = new Label();
            lblHeaderTitle = new Label();
            picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            panelBackground.SuspendLayout();
            panelLoginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // panelBackground
            // 
            panelBackground.Controls.Add(btnCloseForm);
            panelBackground.Controls.Add(panelLoginCard);
            panelBackground.CustomizableEdges = customizableEdges13;
            panelBackground.Dock = DockStyle.Fill;
            panelBackground.FillColor = Color.FromArgb(44, 62, 80);
            panelBackground.FillColor2 = Color.FromArgb(76, 161, 175);
            panelBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            panelBackground.Location = new Point(0, 0);
            panelBackground.Name = "panelBackground";
            panelBackground.ShadowDecoration.CustomizableEdges = customizableEdges14;
            panelBackground.Size = new Size(1000, 650);
            panelBackground.TabIndex = 0;
            // 
            // btnCloseForm
            // 
            btnCloseForm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCloseForm.BackColor = Color.Transparent;
            btnCloseForm.CustomizableEdges = customizableEdges1;
            btnCloseForm.FillColor = Color.Transparent;
            btnCloseForm.HoverState.FillColor = Color.FromArgb(231, 76, 60);
            btnCloseForm.HoverState.IconColor = Color.White;
            btnCloseForm.IconColor = Color.White;
            btnCloseForm.Location = new Point(955, 12);
            btnCloseForm.Name = "btnCloseForm";
            btnCloseForm.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCloseForm.Size = new Size(33, 29);
            btnCloseForm.TabIndex = 1;
            // 
            // panelLoginCard
            // 
            panelLoginCard.Anchor = AnchorStyles.None;
            panelLoginCard.BackColor = Color.Transparent;
            panelLoginCard.Controls.Add(chkShowPassword);
            panelLoginCard.Controls.Add(pictureBox1);
            panelLoginCard.Controls.Add(lblRegister);
            panelLoginCard.Controls.Add(btnOpenFaceID);
            panelLoginCard.Controls.Add(btnLogin);
            panelLoginCard.Controls.Add(lblForgetPassword);
            panelLoginCard.Controls.Add(chkRememberMe);
            panelLoginCard.Controls.Add(txtPassword);
            panelLoginCard.Controls.Add(txtUsername);
            panelLoginCard.Controls.Add(lblSubTitle);
            panelLoginCard.Controls.Add(lblHeaderTitle);
            panelLoginCard.Controls.Add(picLogo);
            panelLoginCard.FillColor = Color.White;
            panelLoginCard.Location = new Point(290, 45);
            panelLoginCard.Name = "panelLoginCard";
            panelLoginCard.Radius = 15;
            panelLoginCard.ShadowColor = Color.Black;
            panelLoginCard.ShadowDepth = 150;
            panelLoginCard.ShadowShift = 10;
            panelLoginCard.Size = new Size(420, 560);
            panelLoginCard.TabIndex = 0;
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            chkShowPassword.CheckedState.BorderRadius = 0;
            chkShowPassword.CheckedState.BorderThickness = 0;
            chkShowPassword.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            chkShowPassword.Location = new Point(323, 274);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new Size(51, 19);
            chkShowPassword.TabIndex = 2;
            chkShowPassword.Text = "Hiện";
            chkShowPassword.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            chkShowPassword.UncheckedState.BorderRadius = 0;
            chkShowPassword.UncheckedState.BorderThickness = 0;
            chkShowPassword.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Login_ico;
            pictureBox1.Location = new Point(156, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(104, 100);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // lblRegister
            // 
            lblRegister.Cursor = Cursors.Hand;
            lblRegister.Font = new Font("Segoe UI", 9.5F);
            lblRegister.ForeColor = Color.FromArgb(100, 116, 139);
            lblRegister.Location = new Point(0, 495);
            lblRegister.Name = "lblRegister";
            lblRegister.Size = new Size(420, 25);
            lblRegister.TabIndex = 9;
            lblRegister.Text = "Chưa có tài khoản? Đăng ký ngay";
            lblRegister.TextAlign = ContentAlignment.MiddleCenter;
            lblRegister.Click += lblRegister_Click;
            // 
            // btnOpenFaceID
            // 
            btnOpenFaceID.BorderRadius = 8;
            btnOpenFaceID.Cursor = Cursors.Hand;
            btnOpenFaceID.CustomizableEdges = customizableEdges3;
            btnOpenFaceID.FillColor = Color.FromArgb(16, 185, 129);
            btnOpenFaceID.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnOpenFaceID.ForeColor = Color.White;
            btnOpenFaceID.Location = new Point(40, 425);
            btnOpenFaceID.Name = "btnOpenFaceID";
            btnOpenFaceID.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnOpenFaceID.Size = new Size(340, 40);
            btnOpenFaceID.TabIndex = 8;
            btnOpenFaceID.Text = "📸  ĐĂNG NHẬP BẰNG FACE ID";
            btnOpenFaceID.Click += btnOpenFaceID_Click;
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 8;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.CustomizableEdges = customizableEdges5;
            btnLogin.FillColor = Color.FromArgb(37, 99, 235);
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(40, 365);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnLogin.Size = new Size(340, 45);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "ĐĂNG NHẬP HỆ THỐNG";
            btnLogin.Click += btnLogin_Click;
            // 
            // lblForgetPassword
            // 
            lblForgetPassword.AutoSize = true;
            lblForgetPassword.Cursor = Cursors.Hand;
            lblForgetPassword.Font = new Font("Segoe UI", 9F);
            lblForgetPassword.ForeColor = Color.FromArgb(37, 99, 235);
            lblForgetPassword.Location = new Point(288, 322);
            lblForgetPassword.Name = "lblForgetPassword";
            lblForgetPassword.Size = new Size(94, 15);
            lblForgetPassword.TabIndex = 6;
            lblForgetPassword.Text = "Quên mật khẩu?";
            lblForgetPassword.Click += lblForgetPassword_Click;
            // 
            // chkRememberMe
            // 
            chkRememberMe.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            chkRememberMe.CheckedState.BorderRadius = 2;
            chkRememberMe.CheckedState.BorderThickness = 0;
            chkRememberMe.CheckedState.FillColor = Color.FromArgb(37, 99, 235);
            chkRememberMe.Font = new Font("Segoe UI", 9F);
            chkRememberMe.ForeColor = Color.FromArgb(100, 116, 139);
            chkRememberMe.Location = new Point(45, 320);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Size = new Size(130, 20);
            chkRememberMe.TabIndex = 5;
            chkRememberMe.Text = "Ghi nhớ đăng nhập";
            chkRememberMe.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            chkRememberMe.UncheckedState.BorderRadius = 2;
            chkRememberMe.UncheckedState.BorderThickness = 0;
            chkRememberMe.UncheckedState.FillColor = Color.FromArgb(226, 232, 240);
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 8;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.CustomizableEdges = customizableEdges7;
            txtPassword.DefaultText = "";
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.ForeColor = Color.Black;
            txtPassword.IconRightCursor = Cursors.Hand;
            txtPassword.IconRightSize = new Size(22, 22);
            txtPassword.Location = new Point(40, 260);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Mật khẩu / Password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtPassword.Size = new Size(340, 45);
            txtPassword.TabIndex = 4;
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 8;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.CustomizableEdges = customizableEdges9;
            txtUsername.DefaultText = "";
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.ForeColor = Color.Black;
            txtUsername.Location = new Point(40, 200);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Tên đăng nhập / Username";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtUsername.Size = new Size(340, 45);
            txtUsername.TabIndex = 3;
            // 
            // lblSubTitle
            // 
            lblSubTitle.Font = new Font("Segoe UI", 9.5F);
            lblSubTitle.ForeColor = Color.FromArgb(100, 116, 139);
            lblSubTitle.Location = new Point(0, 155);
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Size = new Size(420, 20);
            lblSubTitle.TabIndex = 2;
            lblSubTitle.Text = "Vui lòng đăng nhập tài khoản của bạn";
            lblSubTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblHeaderTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblHeaderTitle.Location = new Point(0, 120);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(420, 35);
            lblHeaderTitle.TabIndex = 1;
            lblHeaderTitle.Text = "QUẢN LÝ SINH VIÊN UTEID";
            lblHeaderTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picLogo
            // 
            picLogo.CustomizableEdges = customizableEdges11;
            picLogo.ImageRotate = 0F;
            picLogo.Location = new Point(170, 30);
            picLogo.Name = "picLogo";
            picLogo.ShadowDecoration.CustomizableEdges = customizableEdges12;
            picLogo.Size = new Size(80, 80);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // LoginForm
            // 
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 650);
            Controls.Add(panelBackground);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginForm";
            panelBackground.ResumeLayout(false);
            panelLoginCard.ResumeLayout(false);
            panelLoginCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }

        private PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2CheckBox chkShowPassword;
    }
}