using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject
{
    public partial class StudentListForm : Form
    {
        Student student = new Student();
        public StudentListForm()
        {
            InitializeComponent();
        }

        private void StudentListForm_Load(object sender, EventArgs e)
        {
            fillGrid();
        }

        // Viết một hàm riêng để có thể gọi lại khi cần làm mới (Refresh) dữ liệu
        public void fillGrid()
        {
            // 1. Tạo câu lệnh SQL
            SqlCommand command = new SqlCommand("SELECT mssv as 'MSSV', fname as 'Họ', lname as 'Tên', dob as 'Ngày sinh', gder as 'Giới tính', phone as 'SĐT', address as 'Địa chỉ', htown as 'Quê quán', email as 'Email', pture as 'Ảnh' FROM Student");

            // 2. Gọi hàm từ lớp Student để lấy DataTable
            dgvStudents.DataSource = student.getStudents(command);

            // 3. Xử lý hiển thị Ảnh (Nếu có)
            // DataGridView sẽ tự động hiển thị cột Image nếu kiểu dữ liệu là byte[]
            DataGridViewImageColumn picCol = new DataGridViewImageColumn();
            picCol = (DataGridViewImageColumn)dgvStudents.Columns[9]; // Cột thứ 10 là ảnh
            picCol.ImageLayout = DataGridViewImageCellLayout.Stretch; // Chỉnh ảnh vừa ô

            // 4. Tùy chỉnh độ rộng cột cho đẹp
            dgvStudents.RowTemplate.Height = 80; // Chỉnh dòng cao lên để thấy rõ ảnh
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            fillGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow == null) return;

            // Lấy MSSV từ dòng đang chọn trên GridView
            int mssv = Convert.ToInt32(dgvStudents.CurrentRow.Cells["MSSV"].Value);

            // Hiển thị Confirm xác nhận xóa (Requirement của Lộc)
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên MSSV: {mssv}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (student.deleteStudent(mssv))
                {
                    MessageBox.Show("Xóa thành công!");
                    fillGrid();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!");
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                fillGrid();
                return;
            }

            // Tìm theo MSSV hoặc Tên (Sử dụng LIKE)
            string query = "SELECT Mssv, FirstName, LastName, Phone FROM Students " +
                           "WHERE Mssv LIKE @key OR FirstName LIKE @key OR LastName LIKE @key";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@key", "%" + keyword + "%");

            dgvStudents.DataSource = student.getStudents(command);
        }
    }
}
