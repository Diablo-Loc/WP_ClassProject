namespace ClassProject.Presentation.Forms.Main
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            studentToolStripMenuItem = new ToolStripMenuItem();
            addStudentToolStripMenuItem = new ToolStripMenuItem();
            listStudentToolStripMenuItem = new ToolStripMenuItem();
            adminToolStripMenuItem = new ToolStripMenuItem();
            pnlTotal = new Panel();
            lblTotalStudents = new Label();
            pnlMale = new Panel();
            lblMaleStudents = new Label();
            pnlFemale = new Panel();
            lblFemaleStudents = new Label();
            lblRole = new Label();
            menuStrip1.SuspendLayout();
            pnlTotal.SuspendLayout();
            pnlMale.SuspendLayout();
            pnlFemale.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { studentToolStripMenuItem, adminToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // studentToolStripMenuItem
            // 
            studentToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addStudentToolStripMenuItem, listStudentToolStripMenuItem });
            studentToolStripMenuItem.Name = "studentToolStripMenuItem";
            studentToolStripMenuItem.Size = new Size(74, 24);
            studentToolStripMenuItem.Text = "Student";
            // 
            // addStudentToolStripMenuItem
            // 
            addStudentToolStripMenuItem.Name = "addStudentToolStripMenuItem";
            addStudentToolStripMenuItem.Size = new Size(175, 26);
            addStudentToolStripMenuItem.Text = "Add Student";
            addStudentToolStripMenuItem.Click += addStudentToolStripMenuItem_Click;
            // 
            // listStudentToolStripMenuItem
            // 
            listStudentToolStripMenuItem.Name = "listStudentToolStripMenuItem";
            listStudentToolStripMenuItem.Size = new Size(175, 26);
            listStudentToolStripMenuItem.Text = "List Student";
            listStudentToolStripMenuItem.Click += listStudentToolStripMenuItem_Click;
            // 
            // adminToolStripMenuItem
            // 
            adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            adminToolStripMenuItem.Size = new Size(67, 24);
            adminToolStripMenuItem.Text = "Admin";
            // 
            // pnlTotal
            // 
            pnlTotal.BackColor = Color.FromArgb(192, 255, 192);
            pnlTotal.Controls.Add(lblTotalStudents);
            pnlTotal.Location = new Point(0, 74);
            pnlTotal.Name = "pnlTotal";
            pnlTotal.Size = new Size(250, 125);
            pnlTotal.TabIndex = 1;
            // 
            // lblTotalStudents
            // 
            lblTotalStudents.AutoSize = true;
            lblTotalStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTotalStudents.Location = new Point(41, 49);
            lblTotalStudents.Name = "lblTotalStudents";
            lblTotalStudents.Size = new Size(152, 31);
            lblTotalStudents.TabIndex = 4;
            lblTotalStudents.Text = "TotalStudents";
            // 
            // pnlMale
            // 
            pnlMale.BackColor = Color.Cyan;
            pnlMale.Controls.Add(lblMaleStudents);
            pnlMale.Location = new Point(270, 74);
            pnlMale.Name = "pnlMale";
            pnlMale.Size = new Size(250, 125);
            pnlMale.TabIndex = 2;
            // 
            // lblMaleStudents
            // 
            lblMaleStudents.AutoSize = true;
            lblMaleStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblMaleStudents.Location = new Point(51, 49);
            lblMaleStudents.Name = "lblMaleStudents";
            lblMaleStudents.Size = new Size(154, 31);
            lblMaleStudents.TabIndex = 5;
            lblMaleStudents.Text = "MaleStudents";
            // 
            // pnlFemale
            // 
            pnlFemale.BackColor = Color.FromArgb(255, 192, 255);
            pnlFemale.Controls.Add(lblFemaleStudents);
            pnlFemale.Location = new Point(538, 74);
            pnlFemale.Name = "pnlFemale";
            pnlFemale.Size = new Size(250, 125);
            pnlFemale.TabIndex = 3;
            // 
            // lblFemaleStudents
            // 
            lblFemaleStudents.AutoSize = true;
            lblFemaleStudents.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFemaleStudents.Location = new Point(36, 49);
            lblFemaleStudents.Name = "lblFemaleStudents";
            lblFemaleStudents.Size = new Size(176, 31);
            lblFemaleStudents.TabIndex = 6;
            lblFemaleStudents.Text = "FemaleStudents";
            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Location = new Point(693, 376);
            lblRole.Name = "lblRole";
            lblRole.RightToLeft = RightToLeft.No;
            lblRole.Size = new Size(39, 20);
            lblRole.TabIndex = 4;
            lblRole.Text = "Role";
            lblRole.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblRole);
            Controls.Add(pnlFemale);
            Controls.Add(pnlMale);
            Controls.Add(pnlTotal);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            pnlTotal.ResumeLayout(false);
            pnlTotal.PerformLayout();
            pnlMale.ResumeLayout(false);
            pnlMale.PerformLayout();
            pnlFemale.ResumeLayout(false);
            pnlFemale.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem studentToolStripMenuItem;
        private ToolStripMenuItem listStudentToolStripMenuItem;
        private ToolStripMenuItem adminToolStripMenuItem;
        private ToolStripMenuItem addStudentToolStripMenuItem;
        private Panel pnlTotal;
        private Label lblTotalStudents;
        private Panel pnlMale;
        private Label lblMaleStudents;
        private Panel pnlFemale;
        private Label lblFemaleStudents;
        private Label lblRole;
    }
}