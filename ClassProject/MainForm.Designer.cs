namespace ClassProject
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
            btnViewStudent = new Button();
            btnAddStudent = new Button();
            SuspendLayout();
            // 
            // btnViewStudent
            // 
            btnViewStudent.Location = new Point(227, 164);
            btnViewStudent.Name = "btnViewStudent";
            btnViewStudent.Size = new Size(192, 29);
            btnViewStudent.TabIndex = 0;
            btnViewStudent.Text = "Danh sách sinh viên";
            btnViewStudent.UseVisualStyleBackColor = true;
            btnViewStudent.Click += btnViewStudent_Click;
            // 
            // btnAddStudent
            // 
            btnAddStudent.Location = new Point(485, 164);
            btnAddStudent.Name = "btnAddStudent";
            btnAddStudent.Size = new Size(259, 29);
            btnAddStudent.TabIndex = 1;
            btnAddStudent.Text = "Thêm sinh viên mới";
            btnAddStudent.UseVisualStyleBackColor = true;
            btnAddStudent.Click += btnAddStudent_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(879, 484);
            Controls.Add(btnAddStudent);
            Controls.Add(btnViewStudent);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnViewStudent;
        private Button btnAddStudent;
    }
}