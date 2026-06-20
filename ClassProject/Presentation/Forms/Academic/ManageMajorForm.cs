using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Data;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ManageMajorForm : Form
    {
        private readonly MajorRepository _majorRepository;
        private bool _isEditMode = false;

        public ManageMajorForm()
        {
            InitializeComponent();
            _majorRepository = new MajorRepository();
        }

        private void ManageMajorForm_Load(object sender, EventArgs e)
        {
            // 🌟 CẬP NHẬT PHÂN QUYỀN CHUẨN RBAC: Chỉ Admin hoặc Giáo vụ mới có quyền quản lý danh mục ngành học toàn trường
            if (!UserSession.IsLoggedIn || (!UserSession.IsAdmin && !UserSession.IsStaff))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Tài khoản của bạn không có thẩm quyền cấu hình danh mục ngành học toàn trường.",
                                "Cảnh Báo An Ninh", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Đóng form an toàn thông qua BeginInvoke để tránh xung đột luồng UI
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            // PHÒNG THỦ TEST CASE (DATA OVERFLOW): Giới hạn độ dài chuỗi nhập liệu khớp DB
            txtMajorCode.MaxLength = 10;  // Khớp chính xác kiểu dữ liệu CHAR(10) hoặc VARCHAR(10)
            txtMajorName.MaxLength = 100; // Khớp chính xác kiểu dữ liệu NVARCHAR(100)
            if (txtSearch != null) txtSearch.MaxLength = 100;

            LoadMajorData();
            ResetForm();
        }

        /// <summary>
        /// Nạp toàn bộ danh sách ngành học lên Guna2DataGridView
        /// </summary>
        private void LoadMajorData(string keyword = "")
        {
            try
            {
                DataTable dt = _majorRepository.GetMajors(keyword);
                dgvMajors.DataSource = dt;
                lblTotalMajors.Text = $"Tổng số ngành học: {dt?.Rows.Count ?? 0}";

                // Tối ưu hóa giao diện hiển thị các cột dữ liệu
                if (dgvMajors.Columns.Count > 0)
                {
                    if (dgvMajors.Columns["STT"] != null)
                        dgvMajors.Columns["STT"].Width = 50;

                    if (dgvMajors.Columns["Mã Ngành"] != null)
                        dgvMajors.Columns["Mã Ngành"].Width = 120;

                    if (dgvMajors.Columns["Tên Ngành Học"] != null)
                        dgvMajors.Columns["Tên Ngành Học"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi LoadMajorData: " + ex.Message);
                MessageBox.Show("Đã xảy ra sự cố khi kết nối dữ liệu ngành học. Vui lòng thử lại hoặc liên hệ quản trị viên!",
                                "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện bấm nút Lưu (Tích hợp thông minh cả Thêm mới & Cập nhật)
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Loại bỏ khoảng trắng thừa (Trim) và tự động viết hoa mã ngành (ToUpper)
            string maNganh = txtMajorCode.Text.Trim().ToUpper();
            string tenNganh = txtMajorName.Text.Trim();

            // Kiểm tra tính hợp lệ dữ liệu đầu vào (Validation)
            if (string.IsNullOrEmpty(maNganh) || string.IsNullOrEmpty(tenNganh))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã ngành và Tên ngành học!", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Khởi tạo đối tượng Model từ dữ liệu đã chuẩn hóa
            MajorModel major = new MajorModel(maNganh, tenNganh);

            try
            {
                if (!_isEditMode) // CHẾ ĐỘ THÊM MỚI
                {
                    // Kiểm tra trùng khóa chính (Primary Key) trước khi thực hiện câu lệnh Insert
                    if (_majorRepository.IsMaNganhExists(maNganh))
                    {
                        MessageBox.Show("Mã ngành học này đã tồn tại trên hệ thống! Vui lòng kiểm tra lại.", "Trùng Lặp Khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (_majorRepository.Insert(major))
                    {
                        MessageBox.Show("Thêm mới ngành học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else // CHẾ ĐỘ CẬP NHẬT
                {
                    if (_majorRepository.Update(major))
                    {
                        MessageBox.Show("Cập nhật thông tin ngành học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadMajorData();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Thực thi nghiệp vụ thất bại: {ex.Message}", "Lỗi Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện click chọn một dòng trên Guna2DataGridView đổ dữ liệu ngược lại Form để sửa/xóa
        /// </summary>
        private void dgvMajors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMajors.CurrentRow != null)
            {
                DataGridViewRow row = dgvMajors.Rows[e.RowIndex];

                // Đổ dữ liệu từ bảng lên các TextBox nhập liệu
                txtMajorCode.Text = row.Cells["Mã Ngành"].Value?.ToString();
                txtMajorName.Text = row.Cells["Tên Ngành Học"].Value?.ToString();

                // Chuyển Form sang trạng thái Chỉnh sửa (Edit Mode)
                _isEditMode = true;
                txtMajorCode.ReadOnly = true; // KHÓA trường khóa chính không cho phép chỉnh sửa bừa bãi

                btnSave.Text = "💾 Cập Nhật Ngành";
                btnSave.FillColor = System.Drawing.Color.FromArgb(79, 70, 229); // Màu Indigo hiện đại

                // 🌟 KIỂM SOÁT QUYỀN NÚT XÓA: Chỉ Admin tối cao mới được phép bấm xóa, Giáo vụ chỉ được sửa
                if (UserSession.IsAdmin)
                {
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false; // Khóa chặt nút xóa nếu là Giáo vụ (IsStaff)
                }
            }
        }

        /// <summary>
        /// Xử lý xóa ngành học ra khỏi hệ thống
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Chốt chặn bảo mật tầng 2: Phòng hờ trường hợp nút bấm bị bypass cố ý
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("Tài khoản của bạn không có thẩm quyền xóa dữ liệu ngành học khỏi hệ thống!", "Hạn Chế Quyền Hạn", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string maNganh = txtMajorCode.Text.Trim();
            if (string.IsNullOrEmpty(maNganh)) return;

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa ngành [{maNganh}] khỏi hệ thống?\nHành động này có thể ảnh hưởng đến dữ liệu Lớp học và Sinh viên liên quan!",
                "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_majorRepository.Delete(maNganh))
                    {
                        MessageBox.Show("Đã xóa ngành học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMajorData();
                        ResetForm();
                    }
                }
                catch (Exception)
                {
                    // Bọc ngoại lệ bắt lỗi ràng buộc toàn vẹn dữ liệu (Foreign Key Constraint) 
                    MessageBox.Show("Không thể xóa ngành học này vì đang có dữ liệu danh sách Lớp học hoặc Sinh viên tham chiếu thuộc về ngành này!",
                                    "Lỗi Ràng Buộc Khóa Ngoại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Tìm kiếm thời gian thực (Real-time Search) khi người dùng gõ chữ đến đâu lọc dữ liệu đến đó
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadMajorData(txtSearch.Text);
        }

        /// <summary>
        /// Sự kiện nút Làm mới (Clear)
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        /// <summary>
        /// Đưa giao diện form và các biến trạng thái về mặc định ban đầu
        /// </summary>
        private void ResetForm()
        {
            txtMajorCode.Clear();
            txtMajorName.Clear();
            txtMajorCode.ReadOnly = false; // Mở lại quyền nhập mã cho trạng thái thêm mới

            _isEditMode = false;
            btnSave.Text = "(+) Thêm Ngành Học";
            btnSave.FillColor = System.Drawing.Color.FromArgb(16, 124, 65); // Tông xanh lá đặc trưng
            btnDelete.Enabled = false; // Vô hiệu hóa nút xóa khi form trống
            txtMajorCode.Focus();
        }
    }
}