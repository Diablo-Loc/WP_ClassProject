using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static ClassProject.Models.Session;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class MainForm : Form
    {
        private int roleId;
        private int userId;
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();

        public MainForm(int roleId, int userId)
        {
            InitializeComponent();

            this.roleId = roleId;
            this.userId = userId;
            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //1. HIỂN THỊ ROLE LÊN GIAO DIỆN
            if (roleId == 0)
            {
                lblRole.Text = "Quyền: ADMIN";
                lblRole.ForeColor = Color.Red;
            }
            else if (roleId == 1)
            {
                lblRole.Text = "Quyền: STUDENT";
                lblRole.ForeColor = Color.Gray;
            }
            else
            {
                lblRole.Text = "Quyền: HR (GIẢNG VIÊN)";
                lblRole.ForeColor = Color.Orange;
            }

            // 2. ẨN MENU ADMIN THEO QUYỀN
            // Chỉ có Admin (0) mới được xem menu Admin. Student (1) và HR (2) đều bị ẩn!
            if (roleId != 0)
            {
                adminToolStripMenuItem.Visible = false;
            }

            // Nạp số liệu lên các ô màu Dashboard 
            LoadDashboard();
        }

        // TỐI ƯU SỰ KIỆN: Tự động tính toán lại số liệu sau khi đóng Form Thêm Mới
        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (roleId == 1)
            {
                MessageBox.Show("Tài khoản Sinh viên không có quyền thực hiện chức năng này!", "Bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            this.Hide();
            using (AddStudentForm f = new AddStudentForm(0))
            {
                f.ShowDialog();
            }
            // Ngay khi Form con được đóng lại, MainForm hiện lên và tự cập nhật lại số liệu mới!
            this.Show();
            LoadDashboard();
        }

        // Tự động tính toán lại số liệu sau khi đóng Form Danh Sách (Nơi diễn ra Sửa/Xóa)
        private void listStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (ListStudentForm f = new ListStudentForm(roleId))
            {
                f.ShowDialog();
            }
            this.Show();
            LoadDashboard();
        }

        // Hàm trung tâm tính toán số liệu đổ lên 3 khối Panel (Xanh dương, Xanh lá, Hồng)
        public void LoadDashboard()
        {
            try
            {
                int total = studentRepo.GetTotalStudentsCount();
                int male = studentRepo.GetTotalMaleStudentsCount();
                int female = studentRepo.GetTotalFemaleStudentsCount();

                // Đồng bộ nhãn hiển thị tương ứng như thiết kế đồ họa
                lblTotalStudents.Text = $"Tổng số SV: {total}";
                lblMaleStudents.Text = $"Nam: {male}";
                lblFemaleStudents.Text = $"Nữ: {female}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng điều khiển thống kê: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}