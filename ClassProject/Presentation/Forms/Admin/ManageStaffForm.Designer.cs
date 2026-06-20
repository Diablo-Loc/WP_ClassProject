using System.Drawing;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    partial class ManageStaffForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges25 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges26 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges23 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges24 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            pnlInputCard = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Label();
            txtMSNV = new Guna.UI2.WinForms.Guna2TextBox();
            txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            txtPhone = new Guna.UI2.WinForms.Guna2TextBox();
            txtDepartment = new Guna.UI2.WinForms.Guna2TextBox();
            btnInsert = new Guna.UI2.WinForms.Guna2Button();
            btnUpdate = new Guna.UI2.WinForms.Guna2Button();
            btnDelete = new Guna.UI2.WinForms.Guna2Button();
            btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            pnlDataCard = new Guna.UI2.WinForms.Guna2Panel();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            dgvStaffs = new Guna.UI2.WinForms.Guna2DataGridView();
            pnlInputCard.SuspendLayout();
            pnlDataCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStaffs).BeginInit();
            SuspendLayout();
            // 
            // pnlInputCard
            // 
            pnlInputCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pnlInputCard.BorderRadius = 16;
            pnlInputCard.Controls.Add(lblTitle);
            pnlInputCard.Controls.Add(txtMSNV);
            pnlInputCard.Controls.Add(txtLastName);
            pnlInputCard.Controls.Add(txtFirstName);
            pnlInputCard.Controls.Add(txtEmail);
            pnlInputCard.Controls.Add(txtPhone);
            pnlInputCard.Controls.Add(txtDepartment);
            pnlInputCard.Controls.Add(btnInsert);
            pnlInputCard.Controls.Add(btnUpdate);
            pnlInputCard.Controls.Add(btnDelete);
            pnlInputCard.Controls.Add(btnRefresh);
            pnlInputCard.CustomizableEdges = customizableEdges21;
            pnlInputCard.FillColor = Color.White;
            pnlInputCard.Location = new Point(15, 15);
            pnlInputCard.Name = "pnlInputCard";
            pnlInputCard.ShadowDecoration.CustomizableEdges = customizableEdges22;
            pnlInputCard.Size = new Size(340, 690);
            pnlInputCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(236, 35);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "QUẢN LÝ GIÁO VỤ";
            // 
            // txtMSNV
            // 
            txtMSNV.BorderRadius = 8;
            txtMSNV.Cursor = Cursors.IBeam;
            txtMSNV.CustomizableEdges = customizableEdges1;
            txtMSNV.DefaultText = "";
            txtMSNV.Font = new Font("Segoe UI", 10F);
            txtMSNV.ForeColor = Color.FromArgb(64, 64, 64);
            txtMSNV.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtMSNV.Location = new Point(20, 80);
            txtMSNV.Margin = new Padding(3, 4, 3, 4);
            txtMSNV.Name = "txtMSNV";
            txtMSNV.PlaceholderForeColor = Color.Gray;
            txtMSNV.PlaceholderText = "Mã số nhân viên (MSNV) *";
            txtMSNV.SelectedText = "";
            txtMSNV.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtMSNV.Size = new Size(300, 42);
            txtMSNV.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtMSNV.TabIndex = 1;
            // 
            // txtLastName
            // 
            txtLastName.BorderRadius = 8;
            txtLastName.Cursor = Cursors.IBeam;
            txtLastName.CustomizableEdges = customizableEdges3;
            txtLastName.DefaultText = "";
            txtLastName.Font = new Font("Segoe UI", 10F);
            txtLastName.ForeColor = Color.FromArgb(64, 64, 64);
            txtLastName.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtLastName.Location = new Point(20, 145);
            txtLastName.Margin = new Padding(3, 4, 3, 4);
            txtLastName.Name = "txtLastName";
            txtLastName.PlaceholderForeColor = Color.Gray;
            txtLastName.PlaceholderText = "Họ và chữ lót *";
            txtLastName.SelectedText = "";
            txtLastName.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtLastName.Size = new Size(300, 42);
            txtLastName.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            txtFirstName.BorderRadius = 8;
            txtFirstName.Cursor = Cursors.IBeam;
            txtFirstName.CustomizableEdges = customizableEdges5;
            txtFirstName.DefaultText = "";
            txtFirstName.Font = new Font("Segoe UI", 10F);
            txtFirstName.ForeColor = Color.FromArgb(64, 64, 64);
            txtFirstName.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtFirstName.Location = new Point(20, 210);
            txtFirstName.Margin = new Padding(3, 4, 3, 4);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.PlaceholderForeColor = Color.Gray;
            txtFirstName.PlaceholderText = "Tên giáo vụ *";
            txtFirstName.SelectedText = "";
            txtFirstName.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtFirstName.Size = new Size(300, 42);
            txtFirstName.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtFirstName.TabIndex = 3;
            // 
            // txtEmail
            // 
            txtEmail.BorderRadius = 8;
            txtEmail.Cursor = Cursors.IBeam;
            txtEmail.CustomizableEdges = customizableEdges7;
            txtEmail.DefaultText = "";
            txtEmail.Font = new Font("Segoe UI", 10F);
            txtEmail.ForeColor = Color.FromArgb(64, 64, 64);
            txtEmail.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtEmail.Location = new Point(20, 275);
            txtEmail.Margin = new Padding(3, 4, 3, 4);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderForeColor = Color.Gray;
            txtEmail.PlaceholderText = "Địa chỉ Email trường cấp *";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtEmail.Size = new Size(300, 42);
            txtEmail.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtEmail.TabIndex = 4;
            // 
            // txtPhone
            // 
            txtPhone.BorderRadius = 8;
            txtPhone.Cursor = Cursors.IBeam;
            txtPhone.CustomizableEdges = customizableEdges9;
            txtPhone.DefaultText = "";
            txtPhone.Font = new Font("Segoe UI", 10F);
            txtPhone.ForeColor = Color.FromArgb(64, 64, 64);
            txtPhone.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtPhone.Location = new Point(20, 340);
            txtPhone.Margin = new Padding(3, 4, 3, 4);
            txtPhone.Name = "txtPhone";
            txtPhone.PlaceholderForeColor = Color.Gray;
            txtPhone.PlaceholderText = "Số điện thoại";
            txtPhone.SelectedText = "";
            txtPhone.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtPhone.Size = new Size(300, 42);
            txtPhone.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtPhone.TabIndex = 5;
            // 
            // txtDepartment
            // 
            txtDepartment.BorderRadius = 8;
            txtDepartment.Cursor = Cursors.IBeam;
            txtDepartment.CustomizableEdges = customizableEdges11;
            txtDepartment.DefaultText = "Phòng Giáo vụ";
            txtDepartment.Font = new Font("Segoe UI", 10F);
            txtDepartment.ForeColor = Color.FromArgb(64, 64, 64);
            txtDepartment.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtDepartment.Location = new Point(20, 405);
            txtDepartment.Margin = new Padding(3, 4, 3, 4);
            txtDepartment.Name = "txtDepartment";
            txtDepartment.PlaceholderForeColor = Color.Gray;
            txtDepartment.PlaceholderText = "Phòng ban đảm nhiệm";
            txtDepartment.SelectedText = "";
            txtDepartment.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtDepartment.Size = new Size(300, 42);
            txtDepartment.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtDepartment.TabIndex = 6;
            // 
            // btnInsert
            // 
            btnInsert.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnInsert.BorderRadius = 8;
            btnInsert.CustomizableEdges = customizableEdges13;
            btnInsert.FillColor = Color.FromArgb(0, 114, 198);
            btnInsert.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnInsert.ForeColor = Color.White;
            btnInsert.HoverState.FillColor = Color.FromArgb(0, 90, 160);
            btnInsert.Location = new Point(20, 500);
            btnInsert.Name = "btnInsert";
            btnInsert.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btnInsert.Size = new Size(300, 48);
            btnInsert.TabIndex = 7;
            btnInsert.Text = "CẤP TÀI KHOẢN TỰ ĐỘNG";
            btnInsert.Click += btnInsert_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUpdate.BorderRadius = 8;
            btnUpdate.CustomizableEdges = customizableEdges15;
            btnUpdate.FillColor = Color.FromArgb(40, 167, 69);
            btnUpdate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnUpdate.ForeColor = Color.White;
            btnUpdate.HoverState.FillColor = Color.FromArgb(34, 139, 58);
            btnUpdate.Location = new Point(20, 560);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnUpdate.Size = new Size(145, 45);
            btnUpdate.TabIndex = 8;
            btnUpdate.Text = "CẬP NHẬT";
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDelete.BorderRadius = 8;
            btnDelete.CustomizableEdges = customizableEdges17;
            btnDelete.FillColor = Color.FromArgb(220, 53, 69);
            btnDelete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.HoverState.FillColor = Color.FromArgb(180, 40, 50);
            btnDelete.Location = new Point(175, 560);
            btnDelete.Name = "btnDelete";
            btnDelete.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btnDelete.Size = new Size(145, 45);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "VÔ HIỆU HÓA";
            btnDelete.Click += btnDelete_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefresh.BorderRadius = 8;
            btnRefresh.CustomizableEdges = customizableEdges19;
            btnRefresh.FillColor = Color.FromArgb(241, 245, 249);
            btnRefresh.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRefresh.ForeColor = Color.FromArgb(100, 116, 139);
            btnRefresh.HoverState.FillColor = Color.FromArgb(226, 232, 240);
            btnRefresh.Location = new Point(20, 625);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.ShadowDecoration.CustomizableEdges = customizableEdges20;
            btnRefresh.Size = new Size(300, 40);
            btnRefresh.TabIndex = 10;
            btnRefresh.Text = "LÀM MỚI FORM";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // pnlDataCard
            // 
            pnlDataCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlDataCard.BorderRadius = 16;
            pnlDataCard.Controls.Add(txtSearch);
            pnlDataCard.Controls.Add(dgvStaffs);
            pnlDataCard.CustomizableEdges = customizableEdges25;
            pnlDataCard.FillColor = Color.White;
            pnlDataCard.Location = new Point(370, 15);
            pnlDataCard.Name = "pnlDataCard";
            pnlDataCard.ShadowDecoration.CustomizableEdges = customizableEdges26;
            pnlDataCard.Size = new Size(715, 690);
            pnlDataCard.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BorderRadius = 20;
            txtSearch.CustomizableEdges = customizableEdges23;
            txtSearch.DefaultText = "";
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.HoverState.BorderColor = Color.FromArgb(0, 114, 198);
            txtSearch.Location = new Point(20, 20);
            txtSearch.Margin = new Padding(3, 4, 3, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "🔍 Tìm nhanh giáo vụ theo Mã số, Họ tên hoặc Email...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges24;
            txtSearch.Size = new Size(675, 40);
            txtSearch.TabIndex = 0;
            // 
            // dgvStaffs
            // 
            dgvStaffs.AllowUserToAddRows = false;
            dgvStaffs.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvStaffs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvStaffs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(0, 114, 198);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dgvStaffs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvStaffs.ColumnHeadersHeight = 40;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvStaffs.DefaultCellStyle = dataGridViewCellStyle3;
            dgvStaffs.GridColor = Color.FromArgb(231, 229, 255);
            dgvStaffs.Location = new Point(20, 80);
            dgvStaffs.Name = "dgvStaffs";
            dgvStaffs.ReadOnly = true;
            dgvStaffs.RowHeadersVisible = false;
            dgvStaffs.RowHeadersWidth = 51;
            dgvStaffs.RowTemplate.Height = 35;
            dgvStaffs.Size = new Size(675, 590);
            dgvStaffs.TabIndex = 1;
            dgvStaffs.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvStaffs.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvStaffs.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvStaffs.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvStaffs.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvStaffs.ThemeStyle.BackColor = Color.White;
            dgvStaffs.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvStaffs.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(0, 114, 198);
            dgvStaffs.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStaffs.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dgvStaffs.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvStaffs.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvStaffs.ThemeStyle.HeaderStyle.Height = 40;
            dgvStaffs.ThemeStyle.ReadOnly = true;
            dgvStaffs.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvStaffs.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvStaffs.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvStaffs.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvStaffs.ThemeStyle.RowsStyle.Height = 35;
            dgvStaffs.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvStaffs.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dgvStaffs.CellClick += dgvStaffs_CellClick;
            // ManageStaffForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1100, 720);
            Controls.Add(pnlDataCard);
            Controls.Add(pnlInputCard);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ManageStaffForm";
            Text = "Hệ thống Cấp phát Tài khoản Giáo vụ";
            Load += ManageStaffForm_Load;
            pnlInputCard.ResumeLayout(false);
            pnlInputCard.PerformLayout();
            pnlDataCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvStaffs).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlInputCard;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtMSNV;
        private Guna.UI2.WinForms.Guna2TextBox txtFirstName;
        private Guna.UI2.WinForms.Guna2TextBox txtLastName;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2TextBox txtPhone;
        private Guna.UI2.WinForms.Guna2TextBox txtDepartment;
        private Guna.UI2.WinForms.Guna2Button btnInsert;
        private Guna.UI2.WinForms.Guna2Button btnUpdate;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;

        private Guna.UI2.WinForms.Guna2Panel pnlDataCard;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2DataGridView dgvStaffs;
    }
}