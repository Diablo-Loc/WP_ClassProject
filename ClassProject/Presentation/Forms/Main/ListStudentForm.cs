using ClassProject.Models;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using ClosedXML.Excel; // Thư viện ClosedXML mới theo tài liệu

namespace ClassProject
{
    public partial class ListStudentForm : Form
    {
        private int roleId;
        private StudentRepository studentRepo;
        private My_DB db = new My_DB();

        public ListStudentForm(int roleId)
        {
            InitializeComponent();
            this.roleId = roleId;

            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);
        }

        private void ListStudentForm_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định cho ComboBox Lọc giới tính khi mở Form
            if (cboFilterGender.Items.Count > 0)
            {
                cboFilterGender.SelectedIndex = 0; // Chọn dòng "Tất cả" mặc định
            }
            fillGrid();

            if (roleId == 1) // Nếu là Sinh viên -> Khóa giao diện can thiệp dữ liệu
            {
                btnInsert.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        // 1. Hàm nạp danh sách sinh viên lên DataGridView (Bộ lọc kép trung tâm)
        public void fillGrid()
        {
            try
            {
                dgvStudents.AutoGenerateColumns = true;

                // Lấy từ khóa tìm kiếm realtime (Bỏ qua nếu là chữ mờ placeholder)
                string keyword = txtSearch.Text.Trim();
                if (keyword == "Nhập mã SV, họ hoặc tên để tìm...")
                {
                    keyword = "";
                }

                // Lấy giá trị giới tính đang chọn từ ComboBox
                string gender = cboFilterGender.Text;
                if (string.IsNullOrEmpty(gender))
                {
                    gender = "Tất cả";
                }

                // GỌI REPOSITORY: Truy vấn bộ lọc kép kết hợp cả Từ khóa + Giới tính
                DataTable dt = studentRepo.SearchStudents(keyword, gender);
                DataView dv = dt.DefaultView;

                if (cbSort != null && cbSort.SelectedIndex > 0)
                {
                    if (cbSort.Text == "Tên sinh viên")
                    {
                        // Sắp xếp theo Tên (FirstName) trước, nếu trùng thì xếp theo Họ (LastName)
                        dv.Sort = "FirstName ASC, LastName ASC";
                    }
                    else if (cbSort.Text == "Mã số sinh viên (MSSV)" || cbSort.Text == "MSSV")
                    {
                        // Sắp xếp theo MSSV tăng dần
                        dv.Sort = "MSSV ASC";
                    }
                }
                else
                {
                    // Mặc định không sắp xếp (giữ nguyên thứ tự từ SQL trả về)
                    dv.Sort = "";
                }
                dgvStudents.DataSource = dv;
                if (lblTotalFooter != null)
                {
                    lblTotalFooter.Text = $"Tổng số sinh viên trong danh sách: {dv.Count}";
                }
                // Định dạng lưới hiển thị
                FormatDataGridView();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillGrid(); // Người dùng đổi kiểu xếp -> tải lại lưới tự động đảo vị  
        }

        // Hàm phụ dùng chung để định dạng thẩm mỹ và Việt hóa tiêu đề lưới
        private void FormatDataGridView()
        {
            if (dgvStudents.Columns.Count > 0)
            {
                if (dgvStudents.Columns["UserId"] != null)
                    dgvStudents.Columns["UserId"].Visible = false;

                dgvStudents.Columns["Mssv"].HeaderText = "Mã SV";
                dgvStudents.Columns["FirstName"].HeaderText = "Tên";
                dgvStudents.Columns["LastName"].HeaderText = "Họ";
                dgvStudents.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
                dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
                dgvStudents.Columns["Phone"].HeaderText = "Điện thoại";
                dgvStudents.Columns["Address"].HeaderText = "Địa chỉ";
                dgvStudents.Columns["Hometown"].HeaderText = "Quê quán";
                dgvStudents.Columns["Email"].HeaderText = "Email";

                if (dgvStudents.Columns["Picture"] != null)
                {
                    dgvStudents.Columns["Picture"].HeaderText = "Hình ảnh";
                    if (dgvStudents.Columns["Picture"] is DataGridViewImageColumn picCol)
                    {
                        picCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    }
                }

                dgvStudents.RowTemplate.Height = 80;
                dgvStudents.AllowUserToAddRows = false;
                dgvStudents.ReadOnly = true;
                dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        // 2. Hàm tìm kiếm sinh viên realtime khi gõ phím
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        // Sự kiện khi người dùng thay đổi ComboBox giới tính
        private void cboFilterGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Nhập mã SV, họ hoặc tên để tìm...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập mã SV, họ hoặc tên để tìm...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        // HÀM REFRESH: Đưa toàn bộ các ô nhập liệu và bộ lọc về trạng thái ban đầu
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Nhập mã SV, họ hoặc tên để tìm...";
            txtSearch.ForeColor = Color.Gray;

            if (cboFilterGender.Items.Count > 0)
            {
                cboFilterGender.SelectedIndex = 0; // Trở về lọc "Tất cả"
            }

            fillGrid();
        }

        // 3. Hàm xử lý nút Xóa sinh viên, chặn quyền và bảo mật
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (roleId == 1)
            {
                MessageBox.Show("Bạn không có quyền thực hiện hành động xóa sinh viên!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (dgvStudents.CurrentRow == null) return;

            int mssv = Convert.ToInt32(dgvStudents.CurrentRow.Cells["Mssv"].Value);

            DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa SV mã {mssv}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (studentRepo.DeleteStudent(mssv))
                {
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fillGrid();
                }
            }
        }

        // 4. Nút mở Form Thêm Mới Sinh Viên
        private void btnInsert_Click_1(object sender, EventArgs e)
        {
            if (roleId == 1)
            {
                MessageBox.Show("Bạn không có quyền thêm mới sinh viên!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            using (AddStudentForm addStudentForm = new AddStudentForm(0))
            {
                addStudentForm.ShowDialog();
            }
            fillGrid();
        }

        // 5. Nút mở Form Sửa Thông Tin Sinh Viên
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (roleId == 1)
            {
                MessageBox.Show("Bạn không có quyền chỉnh sửa thông tin sinh viên!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (dgvStudents.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên từ danh sách để chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int mssv = Convert.ToInt32(dgvStudents.CurrentRow.Cells["Mssv"].Value);

                using (AddStudentForm editForm = new AddStudentForm(mssv))
                {
                    editForm.ShowDialog();
                }
                fillGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi mở form chỉnh sửa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 6. PHẦN 3 — EXPORT EXCEL CHUẨN CLOSEDXML (Theo tài liệu của bạn)
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvStudents.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Workbook|*.xlsx";
            sfd.FileName = "Students.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        // Tạo một bản sao DataTable từ DataGridView để xử lý
                        DataTable dt = ((DataTable)dgvStudents.DataSource).Copy();

                        // Loại bỏ cột hình ảnh nhị phân (Picture) nếu có trước khi add vào Excel để tránh crash file
                        if (dt.Columns.Contains("Picture"))
                        {
                            dt.Columns.Remove("Picture");
                        }

                        wb.Worksheets.Add(dt, "Students");
                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Export thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 7. PHẦN 4 & 5 — IMPORT EXCEL THẲNG DB NÂNG CAO (Dùng ClosedXML)
        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            // Phân quyền bảo mật: Sinh viên không được phép Import bậy dữ liệu
            if (roleId == 1)
            {
                MessageBox.Show("Bạn không có quyền thực hiện hành động này!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx";
            ofd.Title = "Chọn file Excel dữ liệu sinh viên để Import";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int successCount = 0;
                    int failCount = 0;

                    using (XLWorkbook wb = new XLWorkbook(ofd.FileName))
                    {
                        var ws = wb.Worksheet(1); // Lấy Sheet đầu tiên
                        var rows = ws.RowsUsed();

                        // Kiểm tra nếu file rỗng hoặc chỉ có tiêu đề
                        if (rows.Count() <= 1)
                        {
                            MessageBox.Show("File Excel trống hoặc không có dữ liệu sinh viên hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        bool isFirstRow = true;

                        foreach (var row in rows)
                        {
                            // Bỏ qua hàng tiêu đề đầu tiên (Mã SV, Tên, Họ...)
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                continue;
                            }

                            try
                            {
                                // Đọc giá trị an toàn từ các cột Excel bằng ClosedXML (Theo thứ tự cột từ 1 đến 9)
                                string sMssv = row.Cell(1).Value.ToString().Trim();
                                string firstName = row.Cell(2).Value.ToString().Trim();
                                string lastName = row.Cell(3).Value.ToString().Trim();
                                string sDob = row.Cell(4).Value.ToString().Trim();
                                string gender = row.Cell(5).Value.ToString().Trim();
                                string phone = row.Cell(6).Value.ToString().Trim();
                                string address = row.Cell(7).Value.ToString().Trim();
                                string hometown = row.Cell(8).Value.ToString().Trim();
                                string email = row.Cell(9).Value.ToString().Trim();

                                if (string.IsNullOrEmpty(sMssv) || string.IsNullOrEmpty(firstName))
                                {
                                    failCount++;
                                    continue;
                                }

                                int mssv = Convert.ToInt32(sMssv);

                                // KIỂM TRA ĐẶC BIỆT: Nếu trùng mã MSSV trong DB thì bỏ qua không Insert để tránh lỗi sập phần mềm
                                if (studentRepo.IsMssvExist(mssv))
                                {
                                    failCount++;
                                    continue;
                                }

                                // Tạo đối tượng Sinh viên mới và nạp dữ liệu từ dòng Excel vào
                                Student sv = new Student();
                                sv.Mssv = mssv;
                                sv.UserId = mssv; // Đồng bộ cấu trúc bảng Login
                                sv.FirstName = firstName;
                                sv.LastName = lastName;
                                sv.DateOfBirth = string.IsNullOrEmpty(sDob) ? DateTime.Now : Convert.ToDateTime(sDob);
                                sv.Gender = string.IsNullOrEmpty(gender) ? "Nam" : gender;
                                sv.Phone = phone;
                                sv.Address = address;
                                sv.Hometown = hometown;
                                sv.Email = email;
                                sv.Picture = null; // Excel không chứa ảnh trực tiếp dạng mảng byte, đặt mặc định null

                                // Gọi tầng Repository lưu trực tiếp và vĩnh viễn vào cơ sở dữ liệu SQL Server
                                if (studentRepo.AddStudent(sv))
                                {
                                    successCount++;
                                }
                                else
                                {
                                    failCount++;
                                }
                            }
                            catch
                            {
                                failCount++; // Nếu dòng bị lỗi định dạng (ví dụ sai ngày tháng) -> tính vào ca thất bại, chạy tiếp dòng sau
                            }
                        }
                    }

                    // Thông báo tổng kết kết quả cực chuyên nghiệp
                    MessageBox.Show($"Quá trình Import hoàn tất!\n- Lưu thành công vào DB: {successCount} SV\n- Thất bại hoặc trùng mã: {failCount} SV",
                                    "Kết quả cấu hình hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại dữ liệu mới nhất từ SQL Server lên lưới DataGridView để người dùng nhìn thấy luôn
                    fillGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong quá trình xử lý file Excel: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}