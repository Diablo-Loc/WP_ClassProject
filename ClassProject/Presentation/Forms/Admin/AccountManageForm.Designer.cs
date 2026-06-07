namespace ClassProject.Presentation.Forms.Admin
{
    partial class AccountManageForm
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
            dgvPendingUsers = new DataGridView();
            colAccept = new DataGridViewButtonColumn();
            colDelete = new DataGridViewButtonColumn();
            txtSearchPending = new TextBox();
            btnBulkDelete = new Button();
            btnBulkAccept = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).BeginInit();
            SuspendLayout();
            // 
            // dgvPendingUsers
            // 
            dgvPendingUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPendingUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPendingUsers.Columns.AddRange(new DataGridViewColumn[] { colAccept, colDelete });
            dgvPendingUsers.Location = new Point(12, 62);
            dgvPendingUsers.Name = "dgvPendingUsers";
            dgvPendingUsers.RowHeadersWidth = 51;
            dgvPendingUsers.Size = new Size(997, 435);
            dgvPendingUsers.TabIndex = 7;
            // 
            // colAccept
            // 
            colAccept.HeaderText = "Accept";
            colAccept.MinimumWidth = 6;
            colAccept.Name = "colAccept";
            colAccept.Resizable = DataGridViewTriState.True;
            colAccept.SortMode = DataGridViewColumnSortMode.Automatic;
            colAccept.Text = "✔";
            colAccept.UseColumnTextForButtonValue = true;
            colAccept.Width = 125;
            // 
            // colDelete
            // 
            colDelete.HeaderText = "Delete";
            colDelete.MinimumWidth = 6;
            colDelete.Name = "colDelete";
            colDelete.Text = "✖";
            colDelete.UseColumnTextForButtonValue = true;
            colDelete.Width = 125;
            // 
            // txtSearchPending
            // 
            txtSearchPending.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearchPending.Location = new Point(837, 12);
            txtSearchPending.Name = "txtSearchPending";
            txtSearchPending.PlaceholderText = "Nhập tìm kiếm...";
            txtSearchPending.Size = new Size(172, 27);
            txtSearchPending.TabIndex = 8;
            txtSearchPending.TextChanged += TxtSearchPending_TextChanged;
            // 
            // btnBulkDelete
            // 
            btnBulkDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBulkDelete.Location = new Point(248, 503);
            btnBulkDelete.Name = "btnBulkDelete";
            btnBulkDelete.Size = new Size(205, 29);
            btnBulkDelete.TabIndex = 11;
            btnBulkDelete.Text = "✖ Từ chối các mục đã chọn";
            btnBulkDelete.UseVisualStyleBackColor = true;
            // 
            // btnBulkAccept
            // 
            btnBulkAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnBulkAccept.Location = new Point(11, 503);
            btnBulkAccept.Name = "btnBulkAccept";
            btnBulkAccept.Size = new Size(205, 29);
            btnBulkAccept.TabIndex = 10;
            btnBulkAccept.Text = "✔ Duyệt các mục đã chọn";
            btnBulkAccept.UseVisualStyleBackColor = true;
            // 
            // AccountManageForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1030, 536);
            Controls.Add(btnBulkDelete);
            Controls.Add(btnBulkAccept);
            Controls.Add(txtSearchPending);
            Controls.Add(dgvPendingUsers);
            Name = "AccountManageForm";
            Text = "AccountManageForm";
            Load += AccountManageForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPendingUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvPendingUsers;
        private DataGridViewButtonColumn colAccept;
        private DataGridViewButtonColumn colDelete;
        private TextBox txtSearchPending;
        private Button btnBulkDelete;
        private Button btnBulkAccept;
    }
}