namespace ClassProject.Presentation.Forms.Auth
{
    partial class CardScannerForm
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
            this.pictureBoxCard = new System.Windows.Forms.PictureBox();
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.txtDetectedResult = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblResultHint = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlControlRight = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCard)).BeginInit();
            this.pnlControlRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxCard
            // 
            this.pictureBoxCard.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBoxCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCard.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxCard.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCard.Margin = new System.Windows.Forms.Padding(10);
            this.pictureBoxCard.Name = "pictureBoxCard";
            this.pictureBoxCard.Size = new System.Drawing.Size(460, 411);
            this.pictureBoxCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCard.TabIndex = 0;
            this.pictureBoxCard.TabStop = false;
            // 
            // btnSelectImage
            // 
            this.btnSelectImage.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSelectImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectImage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSelectImage.ForeColor = System.Drawing.Color.White;
            this.btnSelectImage.Location = new System.Drawing.Point(15, 60);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(225, 45);
            this.btnSelectImage.TabIndex = 1;
            this.btnSelectImage.Text = "📷 TẢI ẢNH THẺ SV";
            this.btnSelectImage.UseVisualStyleBackColor = false;
            this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
            // 
            // txtDetectedResult
            // 
            this.txtDetectedResult.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtDetectedResult.ForeColor = System.Drawing.Color.Crimson;
            this.txtDetectedResult.Location = new System.Drawing.Point(15, 160);
            this.txtDetectedResult.Name = "txtDetectedResult";
            this.txtDetectedResult.Size = new System.Drawing.Size(225, 32);
            this.txtDetectedResult.TabIndex = 2;
            this.txtDetectedResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(188, 21);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Trích Xuất Thẻ Tự Động";
            // 
            // lblResultHint
            // 
            this.lblResultHint.AutoSize = true;
            this.lblResultHint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResultHint.ForeColor = System.Drawing.Color.Gray;
            this.lblResultHint.Location = new System.Drawing.Point(15, 135);
            this.lblResultHint.Name = "lblResultHint";
            this.lblResultHint.Size = new System.Drawing.Size(142, 15);
            this.lblResultHint.TabIndex = 4;
            this.lblResultHint.Text = "Kết quả nhận diện MSSV:";
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.ForestGreen;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(15, 290);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(225, 40);
            this.btnConfirm.TabIndex = 5;
            this.btnConfirm.Text = "✓ XÁC NHẬN && ĐIỀN";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(15, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(225, 40);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlControlRight
            // 
            this.pnlControlRight.Controls.Add(this.lblTitle);
            this.pnlControlRight.Controls.Add(this.btnCancel);
            this.pnlControlRight.Controls.Add(this.btnSelectImage);
            this.pnlControlRight.Controls.Add(this.btnConfirm);
            this.pnlControlRight.Controls.Add(this.txtDetectedResult);
            this.pnlControlRight.Controls.Add(this.lblResultHint);
            this.pnlControlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControlRight.Location = new System.Drawing.Point(460, 0);
            this.pnlControlRight.Name = "pnlControlRight";
            this.pnlControlRight.Size = new System.Drawing.Size(254, 411);
            this.pnlControlRight.TabIndex = 7;
            // 
            // CardScannerForm
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(714, 411);
            this.Controls.Add(this.pnlControlRight);
            this.Controls.Add(this.pictureBoxCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CardScannerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trợ Lý Số: Quét Thẻ Sinh Viên Bằng AI";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCard)).EndInit();
            this.pnlControlRight.ResumeLayout(false);
            this.pnlControlRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCard;
        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.TextBox txtDetectedResult;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblResultHint;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlControlRight;
    }
}