using ClassProject.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassProject
{
    public partial class ListStudentForm : Form
    {
        Student student = new Student();
        public ListStudentForm()
        {
            InitializeComponent();
        }

        private void ListStudentForm_Load(object sender, EventArgs e)
        {
            fillGrid();
        }
        public void fillGrid()
        {
            try
            {
                dgvStudents.AutoGenerateColumns = true;

                string query = "SELECT Mssv, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture FROM Students";
                SqlCommand command = new SqlCommand(query);
                DataTable dt = student.getStudents(command);

                dgvStudents.DataSource = dt;

                if (dgvStudents.Columns.Count > 0)
                {
                    dgvStudents.Columns["Mssv"].HeaderText = "Mã SV";
                    dgvStudents.Columns["FirstName"].HeaderText = "Tên";
                    dgvStudents.Columns["LastName"].HeaderText = "Họ";
                    dgvStudents.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
                    dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
                    dgvStudents.Columns["Phone"].HeaderText = "Điện thoại";
                    dgvStudents.Columns["Address"].HeaderText = "Địa chỉ";
                    dgvStudents.Columns["Hometown"].HeaderText = "Quê quán";
                    dgvStudents.Columns["Picture"].HeaderText = "Hình ảnh";

                    if (dgvStudents.Columns["Picture"] is DataGridViewImageColumn picCol)
                    {
                        picCol.ImageLayout = DataGridViewImageCellLayout.Stretch;
                    }
                }

                dgvStudents.RowTemplate.Height = 80;
                dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị: " + ex.Message);
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            // Nếu để trống hoặc là placeholder thì hiện lại toàn bộ danh sách
            if (string.IsNullOrEmpty(keyword) || keyword == "Nhập mã SV, họ hoặc tên để tìm...")
            {
                fillGrid();
                return;
            }

            try
            {
                // 1. Dùng lại câu lệnh SELECT đầy đủ để không bị mất cột/mất định dạng
                string query = "SELECT Mssv, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture " +
                               "FROM Students " +
                               "WHERE Mssv LIKE @key OR FirstName LIKE @key OR LastName LIKE @key";

                SqlCommand command = new SqlCommand(query);
                command.Parameters.AddWithValue("@key", "%" + keyword + "%");

                DataTable dt = student.getStudents(command);

                // 2. Gán dữ liệu mới
                dgvStudents.DataSource = dt;

                // 3. Vì DataSource thay đổi, ta cần gọi lại các Header tiếng Việt (nên tách hàm này ra nếu dùng nhiều)
                if (dgvStudents.Columns.Count > 0)
                {
                    dgvStudents.Columns["Mssv"].HeaderText = "Mã SV";
                    dgvStudents.Columns["FirstName"].HeaderText = "Tên";
                    dgvStudents.Columns["LastName"].HeaderText = "Họ";
                    dgvStudents.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
                    dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
                    dgvStudents.Columns["Phone"].HeaderText = "Điện thoại";
                    dgvStudents.Columns["Address"].HeaderText = "Địa chỉ";
                    dgvStudents.Columns["Hometown"].HeaderText = "Quê quán";
                    dgvStudents.Columns["Picture"].HeaderText = "Hình ảnh";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Nhập mã SV, họ hoặc tên để tìm...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black; // Chuyển sang màu đen khi gõ
            }
        }
        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập mã SV, họ hoặc tên để tìm...";
                txtSearch.ForeColor = Color.Gray; // Hiện lại chữ mờ
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            fillGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow == null) return;

            // Tên cột bây giờ chính xác là "Mssv" (lấy từ SQL sang)
            int mssv = Convert.ToInt32(dgvStudents.CurrentRow.Cells["Mssv"].Value);

            DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa SV mã {mssv}?", "Xác nhận", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (student.deleteStudent(mssv))
                {
                    MessageBox.Show("Xóa xong!");
                    fillGrid();
                }
            }
        }

        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            AddStudentForm addStudentForm = new AddStudentForm(0);
            addStudentForm.Show();
            fillGrid();
        }
    }
}
