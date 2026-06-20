namespace ClassProject.Presentation.Forms.Auth
{
    partial class FaceLoginForm
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
            picCamera = new PictureBox();
            btnCancel = new Button();
            btnRegisterFace = new Button();
            lblTitle = new Label();
            lblStatus = new Label();
            btnTestSecurityAlert = new Button();
            ((System.ComponentModel.ISupportInitialize)picCamera).BeginInit();
            SuspendLayout();
            // 
            // picCamera
            // 
            picCamera.BackColor = Color.FromArgb(30, 30, 30);
            picCamera.BorderStyle = BorderStyle.FixedSingle;
            picCamera.Location = new Point(67, 123);
            picCamera.Margin = new Padding(4, 5, 4, 5);
            picCamera.Name = "picCamera";
            picCamera.Size = new Size(639, 553);
            picCamera.SizeMode = PictureBoxSizeMode.StretchImage;
            picCamera.TabIndex = 0;
            picCamera.TabStop = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(741, 502);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(253, 77);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Hủy quét & Quay lại";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnRegisterFace
            // 
            btnRegisterFace.BackColor = Color.FromArgb(46, 204, 113);
            btnRegisterFace.Cursor = Cursors.Hand;
            btnRegisterFace.FlatAppearance.BorderSize = 0;
            btnRegisterFace.FlatStyle = FlatStyle.Flat;
            btnRegisterFace.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRegisterFace.ForeColor = Color.White;
            btnRegisterFace.Location = new Point(741, 402);
            btnRegisterFace.Margin = new Padding(4, 5, 4, 5);
            btnRegisterFace.Name = "btnRegisterFace";
            btnRegisterFace.Size = new Size(253, 77);
            btnRegisterFace.TabIndex = 4;
            btnRegisterFace.Text = "Chụp & Đăng ký";
            btnRegisterFace.UseVisualStyleBackColor = false;
            btnRegisterFace.Visible = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(59, 38);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(529, 41);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "XÁC THỰC SINH TRẮC HỌC FACE ID";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(741, 123);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(267, 231);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Hướng dẫn:\r\n\r\nVui lòng nhìn thẳng vào webcam. Hệ thống AI đang tự động phân tích và nhận diện khuôn mặt để đăng nhập.";
            // 
            // btnTestSecurityAlert
            // 
            btnTestSecurityAlert.BackColor = Color.Yellow;
            btnTestSecurityAlert.Cursor = Cursors.Hand;
            btnTestSecurityAlert.FlatAppearance.BorderSize = 0;
            btnTestSecurityAlert.FlatStyle = FlatStyle.Flat;
            btnTestSecurityAlert.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnTestSecurityAlert.ForeColor = Color.Black;
            btnTestSecurityAlert.Location = new Point(741, 599);
            btnTestSecurityAlert.Margin = new Padding(4, 5, 4, 5);
            btnTestSecurityAlert.Name = "btnTestSecurityAlert";
            btnTestSecurityAlert.Size = new Size(253, 77);
            btnTestSecurityAlert.TabIndex = 5;
            btnTestSecurityAlert.Text = "Test Cảnh Báo AI";
            btnTestSecurityAlert.UseVisualStyleBackColor = false;
            btnTestSecurityAlert.Click += btnTestSecurityAlert_Click;
            // 
            // FaceLoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1045, 740);
            Controls.Add(btnTestSecurityAlert);
            Controls.Add(lblStatus);
            Controls.Add(lblTitle);
            Controls.Add(btnRegisterFace);
            Controls.Add(btnCancel);
            Controls.Add(picCamera);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FaceLoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hệ thống Đăng nhập FaceID";
            ((System.ComponentModel.ISupportInitialize)picCamera).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picCamera;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRegisterFace;
        private Button btnTestSecurityAlert;
    }
}