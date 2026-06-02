namespace ClassProject.Presentation.Forms.Students
{
    partial class RegisterCourse
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnSendRequest = new Button();
            dgvMyRequests = new DataGridView();
            lblTitle = new Label();
            lblSub = new Label();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            btnClear = new Button();
            txtRequestContent = new Guna.UI2.WinForms.Guna2TextBox();
            guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            cboType = new Guna.UI2.WinForms.Guna2ComboBox();
            label1 = new Label();
            lblLoaiYeuCau = new Label();
            lblNoiDungYeuCau = new Label();
            guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            lblTotal = new Label();
            label3 = new Label();
            btnSearch = new Guna.UI2.WinForms.Guna2Button();
            txtSearchRequests = new Guna.UI2.WinForms.Guna2TextBox();
            label2 = new Label();
            guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)dgvMyRequests).BeginInit();
            guna2Panel1.SuspendLayout();
            guna2Panel3.SuspendLayout();
            SuspendLayout();
            // 
            // btnSendRequest
            // 
            btnSendRequest.Location = new Point(830, 310);
            btnSendRequest.Name = "btnSendRequest";
            btnSendRequest.Size = new Size(96, 38);
            btnSendRequest.TabIndex = 1;
            btnSendRequest.Text = "Gửi yêu cầu";
            btnSendRequest.UseVisualStyleBackColor = true;
            btnSendRequest.Click += btnSendRequest_Click;
            // 
            // dgvMyRequests
            // 
            dgvMyRequests.BackgroundColor = SystemColors.ButtonFace;
            dgvMyRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMyRequests.Location = new Point(24, 106);
            dgvMyRequests.Name = "dgvMyRequests";
            dgvMyRequests.RowHeadersWidth = 51;
            dgvMyRequests.Size = new Size(1049, 163);
            dgvMyRequests.TabIndex = 2;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(20, 40, 80);
            lblTitle.Location = new Point(59, 21);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(330, 41);
            lblTitle.TabIndex = 3;
            lblTitle.Text = "GỬI YÊU CẦU HỖ TRỢ";
            // 
            // lblSub
            // 
            lblSub.AutoSize = true;
            lblSub.Location = new Point(64, 71);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(280, 20);
            lblSub.TabIndex = 4;
            lblSub.Text = "Sinh viên gửi yêu cầu đến phòng đào tạo";
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.White;
            guna2Panel1.BorderRadius = 12;
            guna2Panel1.Controls.Add(btnClear);
            guna2Panel1.Controls.Add(txtRequestContent);
            guna2Panel1.Controls.Add(guna2Panel2);
            guna2Panel1.Controls.Add(cboType);
            guna2Panel1.Controls.Add(label1);
            guna2Panel1.Controls.Add(btnSendRequest);
            guna2Panel1.Controls.Add(lblLoaiYeuCau);
            guna2Panel1.Controls.Add(lblNoiDungYeuCau);
            guna2Panel1.CustomizableEdges = customizableEdges7;
            guna2Panel1.Location = new Point(12, 111);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges8;
            guna2Panel1.Size = new Size(1092, 361);
            guna2Panel1.TabIndex = 32;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(945, 310);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(96, 38);
            btnClear.TabIndex = 51;
            btnClear.Text = "Làm mới";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnReset_Click;
            // 
            // txtRequestContent
            // 
            txtRequestContent.BorderRadius = 6;
            txtRequestContent.CustomizableEdges = customizableEdges1;
            txtRequestContent.DefaultText = "";
            txtRequestContent.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtRequestContent.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtRequestContent.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtRequestContent.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtRequestContent.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtRequestContent.Font = new Font("Segoe UI", 9F);
            txtRequestContent.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtRequestContent.Location = new Point(165, 186);
            txtRequestContent.Margin = new Padding(3, 4, 3, 4);
            txtRequestContent.Multiline = true;
            txtRequestContent.Name = "txtRequestContent";
            txtRequestContent.PlaceholderText = "Nhập nội dung yêu cầu của bạn tại đây...";
            txtRequestContent.ScrollBars = ScrollBars.Vertical;
            txtRequestContent.SelectedText = "";
            txtRequestContent.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtRequestContent.Size = new Size(885, 106);
            txtRequestContent.TabIndex = 50;
            // 
            // guna2Panel2
            // 
            guna2Panel2.CustomizableEdges = customizableEdges3;
            guna2Panel2.Location = new Point(165, 155);
            guna2Panel2.Name = "guna2Panel2";
            guna2Panel2.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2Panel2.Size = new Size(885, 34);
            guna2Panel2.TabIndex = 49;
            // 
            // cboType
            // 
            cboType.BackColor = Color.Transparent;
            cboType.BorderRadius = 6;
            cboType.CustomizableEdges = customizableEdges5;
            cboType.DrawMode = DrawMode.OwnerDrawFixed;
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboType.FocusedColor = Color.FromArgb(94, 148, 255);
            cboType.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cboType.Font = new Font("Segoe UI", 10F);
            cboType.ForeColor = Color.FromArgb(68, 88, 112);
            cboType.ItemHeight = 30;
            cboType.Location = new Point(170, 62);
            cboType.Name = "cboType";
            cboType.ShadowDecoration.CustomizableEdges = customizableEdges6;
            cboType.Size = new Size(244, 36);
            cboType.TabIndex = 48;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(0, 192, 192);
            label1.Location = new Point(14, 13);
            label1.Name = "label1";
            label1.Size = new Size(160, 20);
            label1.TabIndex = 47;
            label1.Text = "THÔNG TIN YÊU CẦU";
            // 
            // lblLoaiYeuCau
            // 
            lblLoaiYeuCau.AutoSize = true;
            lblLoaiYeuCau.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLoaiYeuCau.ForeColor = Color.Black;
            lblLoaiYeuCau.Location = new Point(24, 68);
            lblLoaiYeuCau.Name = "lblLoaiYeuCau";
            lblLoaiYeuCau.Size = new Size(99, 20);
            lblLoaiYeuCau.TabIndex = 11;
            lblLoaiYeuCau.Text = "Loại yêu cầu:";
            // 
            // lblNoiDungYeuCau
            // 
            lblNoiDungYeuCau.AutoSize = true;
            lblNoiDungYeuCau.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNoiDungYeuCau.ForeColor = Color.Black;
            lblNoiDungYeuCau.Location = new Point(24, 155);
            lblNoiDungYeuCau.Name = "lblNoiDungYeuCau";
            lblNoiDungYeuCau.Size = new Size(135, 20);
            lblNoiDungYeuCau.TabIndex = 12;
            lblNoiDungYeuCau.Text = "Nội dung yêu cầu:";
            // 
            // guna2Panel3
            // 
            guna2Panel3.BackColor = Color.White;
            guna2Panel3.BorderRadius = 12;
            guna2Panel3.Controls.Add(lblTotal);
            guna2Panel3.Controls.Add(label3);
            guna2Panel3.Controls.Add(btnSearch);
            guna2Panel3.Controls.Add(txtSearchRequests);
            guna2Panel3.Controls.Add(label2);
            guna2Panel3.Controls.Add(dgvMyRequests);
            guna2Panel3.Controls.Add(guna2Button1);
            guna2Panel3.CustomizableEdges = customizableEdges15;
            guna2Panel3.Location = new Point(12, 489);
            guna2Panel3.Name = "guna2Panel3";
            guna2Panel3.ShadowDecoration.CustomizableEdges = customizableEdges16;
            guna2Panel3.Size = new Size(1092, 303);
            guna2Panel3.TabIndex = 52;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.ForeColor = Color.Black;
            lblTotal.Location = new Point(24, 275);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(65, 20);
            lblTotal.TabIndex = 53;
            lblTotal.Text = "Tổng: ...";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Black;
            label3.Location = new Point(14, 62);
            label3.Name = "label3";
            label3.Size = new Size(78, 20);
            label3.TabIndex = 52;
            label3.Text = "Tìm kiếm:";
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnSearch.BorderRadius = 8;
            btnSearch.CustomizableEdges = customizableEdges9;
            btnSearch.DisabledState.BorderColor = Color.DarkGray;
            btnSearch.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSearch.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSearch.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSearch.FillColor = Color.FromArgb(40, 167, 69);
            btnSearch.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(427, 47);
            btnSearch.Margin = new Padding(20, 20, 30, 20);
            btnSearch.Name = "btnSearch";
            btnSearch.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnSearch.Size = new Size(73, 50);
            btnSearch.TabIndex = 49;
            btnSearch.Text = "Tìm";
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearchRequests
            // 
            txtSearchRequests.BorderRadius = 8;
            txtSearchRequests.CustomizableEdges = customizableEdges11;
            txtSearchRequests.DefaultText = "";
            txtSearchRequests.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtSearchRequests.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtSearchRequests.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtSearchRequests.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtSearchRequests.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchRequests.Font = new Font("Segoe UI", 9F);
            txtSearchRequests.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearchRequests.Location = new Point(111, 47);
            txtSearchRequests.Margin = new Padding(3, 4, 3, 4);
            txtSearchRequests.Name = "txtSearchRequests";
            txtSearchRequests.PlaceholderForeColor = Color.FromArgb(170, 180, 190);
            txtSearchRequests.PlaceholderText = "Tìm kiếm yêu cầu,. ...";
            txtSearchRequests.SelectedText = "";
            txtSearchRequests.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtSearchRequests.Size = new Size(282, 46);
            txtSearchRequests.TabIndex = 50;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 192, 192);
            label2.Location = new Point(14, 13);
            label2.Name = "label2";
            label2.Size = new Size(133, 20);
            label2.TabIndex = 47;
            label2.Text = "LỊCH SỬ YÊU CẦU";
            // 
            // guna2Button1
            // 
            guna2Button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            guna2Button1.BorderColor = SystemColors.ScrollBar;
            guna2Button1.BorderRadius = 8;
            guna2Button1.BorderThickness = 1;
            guna2Button1.CustomizableEdges = customizableEdges13;
            guna2Button1.DisabledState.BorderColor = Color.DarkGray;
            guna2Button1.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2Button1.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2Button1.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2Button1.FillColor = Color.White;
            guna2Button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2Button1.ForeColor = SystemColors.WindowFrame;
            guna2Button1.Location = new Point(2437, 224);
            guna2Button1.Margin = new Padding(30, 20, 20, 20);
            guna2Button1.Name = "guna2Button1";
            guna2Button1.ShadowDecoration.CustomizableEdges = customizableEdges14;
            guna2Button1.Size = new Size(164, 503);
            guna2Button1.TabIndex = 46;
            guna2Button1.Text = "🔄 Làm mới";
            // 
            // StudentRequestForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1138, 800);
            Controls.Add(guna2Panel3);
            Controls.Add(guna2Panel1);
            Controls.Add(lblSub);
            Controls.Add(lblTitle);
            ForeColor = Color.DarkGray;
            Name = "StudentRequestForm";
            Text = "StudentRequestForm";
            Load += StudentRequestForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMyRequests).EndInit();
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            guna2Panel3.ResumeLayout(false);
            guna2Panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSendRequest;
        private DataGridView dgvMyRequests;
        private Label lblTitle;
        private Label lblSub;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Label lblLoaiYeuCau;
        private Label lblNoiDungYeuCau;
        private Label label1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2ComboBox cboType;
        private Button btnClear;
        private Guna.UI2.WinForms.Guna2TextBox txtRequestContent;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Label label2;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button btnSearch;
        private Guna.UI2.WinForms.Guna2TextBox txtSearchRequests;
        private Label label3;
        private Label lblTotal;
    }
}