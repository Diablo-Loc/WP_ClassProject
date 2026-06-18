using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ManageStaffForm : Form
    {
        private readonly StaffRepository _staffRepo;
        private readonly My_DB _db = new My_DB();
        private bool _isEditMode = false;

        private const string STAFF_EMAIL_SUBDOMAIN = "@giaovu.";
        private const string REQUIRED_EMAIL_SUFFIX = ".edu.vn";

        public ManageStaffForm()
        {
            InitializeComponent();
            _staffRepo = new StaffRepository();

            // Đăng ký sự kiện để tự động sinh Email khi gõ Họ hoặc Tên
            txtLastName.TextChanged += AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged += AutoGenerateEmail_TextChanged;
        }

        private void ManageStaffForm_Load(object sender, EventArgs e)
        {
            StyleDataGridView();
            LoadStaffList();
            SwitchMode(false);
        }

        private void StyleDataGridView()
        {
            dgvStaffs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStaffs.AllowUserToAddRows = false;
            dgvStaffs.EnableHeadersVisualStyles = false;
            dgvStaffs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 114, 198);
            dgvStaffs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStaffs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvStaffs.RowTemplate.Height = 32;
        }

        private void LoadStaffList()
        {
            try
            {
                if (dgvStaffs.Columns.Contains("btnActionView"))
                {
                    dgvStaffs.Columns.Remove("btnActionView");
                }

                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = @"SELECT s.Id, s.MSNV, s.LastName, s.FirstName, s.Email, s.Phone, s.Department, u.Username 
                                     FROM LoginDB.dbo.Staffs s 
                                     INNER JOIN LoginDB.dbo.Users u ON s.UserId = u.Id 
                                     WHERE s.Status = 1";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvStaffs.DataSource = dt;

                    if (dgvStaffs.Columns.Contains("Id")) dgvStaffs.Columns["Id"].Visible = false;

                    if (dgvStaffs.Columns.Contains("LastName")) dgvStaffs.Columns["LastName"].HeaderText = "Họ & Chữ lót";
                    if (dgvStaffs.Columns.Contains("FirstName")) dgvStaffs.Columns["FirstName"].HeaderText = "Tên";
                }

                // 👁️ THÊM TỰ ĐỘNG CỘT "CON MẮT" XEM THÔNG TIN VÀO CUỐI BẢNG
                DataGridViewButtonColumn viewColumn = new DataGridViewButtonColumn();
                viewColumn.Name = "btnActionView";
                viewColumn.HeaderText = "Bảo Mật";
                viewColumn.Text = "👁️ Xem";
                viewColumn.UseColumnTextForButtonValue = true;
                viewColumn.Width = 80;
                viewColumn.FlatStyle = FlatStyle.Flat;
                viewColumn.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                viewColumn.DefaultCellStyle.ForeColor = Color.FromArgb(0, 114, 198);
                viewColumn.DefaultCellStyle.SelectionForeColor = Color.FromArgb(0, 114, 198);

                dgvStaffs.Columns.Add(viewColumn);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách giáo vụ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SwitchMode(bool isEdit)
        {
            _isEditMode = isEdit;

            if (isEdit)
            {
                lblTitle.Text = "CẬP NHẬT GIÁO VỤ";
                txtMSNV.ReadOnly = true;
                txtEmail.ReadOnly = true; // Chế độ sửa không cho sửa Email tránh lỗi đồng bộ Auth

                btnInsert.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnRefresh.Text = "HỦY CHỌN (TẠO MỚI)";
            }
            else
            {
                lblTitle.Text = "THÊM GIÁO VỤ MỚI";
                txtMSNV.ReadOnly = false;
                txtEmail.ReadOnly = false;

                btnInsert.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                btnRefresh.Text = "LÀM MỚI FORM";
            }
        }

        #region 💡 TỰ ĐỘNG SINH EMAIL & USERNAME CHUẨN DOANH NGHIỆP

        private void AutoGenerateEmail_TextChanged(object sender, EventArgs e)
        {
            // Chỉ tự động điền gợi ý khi đang ở chế độ THÊM MỚI
            if (!_isEditMode)
            {
                string fullNameStr = $"{txtLastName.Text.Trim()} {txtFirstName.Text.Trim()}";
                if (string.IsNullOrWhiteSpace(fullNameStr))
                {
                    txtEmail.Clear();
                    return;
                }

                // Chuyển chữ có dấu thành không dấu, viết liền, chữ thường (Ví dụ: nguyenvana)
                string unsignedName = RemoveSignForVietnamese(fullNameStr).Replace(" ", "").ToLower();

                // Định dạng chuẩn động sinh ra: nguyenvana@giaovu.school.edu.vn
                txtEmail.Text = unsignedName + STAFF_EMAIL_SUBDOMAIN + "school" + REQUIRED_EMAIL_SUFFIX;
            }
        }

        private string RemoveSignForVietnamese(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ", "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ", "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự", "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d", "e","e","e","e","e","e","e","e","e","e","e", "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u", "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                str = str.Replace(arr1[i], arr2[i]);
                str = str.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return str;
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return res.ToString();
        }

        private string GenerateUsernameFromEmail(string email)
        {
            // Lấy phần trước ký tự '@' làm tên gốc
            string localPart = email.Split('@')[0];
            return $"giaovu_{localPart.ToLower().Trim()}";
        }
        #endregion

        #region 🛡️ VALIDATION CHECKS (TEST CASES CHUẨN DOANH NGHIỆP)
        private bool ValidateStaffData(bool isUpdateMode)
        {
            // 1. Kiểm tra rỗng các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtMSNV.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc!", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Validate Họ và Tên không chứa số hoặc ký tự đặc biệt
            string namePattern = @"^[\p{L}\s]+$";
            if (!Regex.IsMatch(txtLastName.Text.Trim(), namePattern) || !Regex.IsMatch(txtFirstName.Text.Trim(), namePattern))
            {
                MessageBox.Show("Họ và Tên chỉ được phép chứa ký tự chữ!", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 3. 🎯 ĐÃ SỬA LỖI REGEX: Cập nhật Regex chuẩn tương thích với hệ thống Subdomain đa cấp
            string emailInput = txtEmail.Text.Trim();
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(emailInput, emailPattern))
            {
                MessageBox.Show("Địa chỉ Email không đúng định dạng cấu trúc tiêu chuẩn!", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 4. Đảm bảo kiểm tra đúng sự tồn tại của cụm '@giaovu.' và đuôi '.edu.vn'
            if (!emailInput.Contains(STAFF_EMAIL_SUBDOMAIN, StringComparison.OrdinalIgnoreCase) ||
                !emailInput.EndsWith(REQUIRED_EMAIL_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"Email dành cho giáo vụ bắt buộc phải chứa miền định danh '{STAFF_EMAIL_SUBDOMAIN}' và kết thúc bằng đuôi '{REQUIRED_EMAIL_SUFFIX}'!\n\nVí dụ hợp lệ: nguyenvana@giaovu.school.edu.vn", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 5. Validate Số điện thoại chuẩn 10 số Việt Nam
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string phonePattern = @"^(03|05|07|08|09)\d{8}$";
                if (!Regex.IsMatch(txtPhone.Text.Trim(), phonePattern))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! Phải có đúng 10 chữ số và thuộc các đầu số VN (03, 05, 07, 08, 09).", "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }
        #endregion

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffData(isUpdateMode: false)) return;

            string email = txtEmail.Text.Trim();
            string username = GenerateUsernameFromEmail(email);
            string rawPassword = GenerateRandomPassword(8);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            string errorMsg;
            bool success = _staffRepo.CreateStaff(
                username, email, passwordHash,
                txtMSNV.Text.Trim(), txtFirstName.Text.Trim(), txtLastName.Text.Trim(),
                txtPhone.Text.Trim(), txtDepartment.Text.Trim(), out errorMsg
            );

            if (success)
            {
                string successMessage = $"🎉 CẤP TÀI KHOẢN THÀNH CÔNG!\n\n" +
                                        $"• Username: {username}\n" +
                                        $"• Mật khẩu tạm: {rawPassword}\n\n" +
                                        $"Hãy bàn giao thông tin này bảo mật!";
                MessageBox.Show(successMessage, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
                LoadStaffList();
            }
            else
            {
                MessageBox.Show($"Thêm thất bại:\n{errorMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffData(isUpdateMode: true)) return;

            string errorMsg;
            bool success = _staffRepo.UpdateStaff(
                txtMSNV.Text.Trim(), txtFirstName.Text.Trim(), txtLastName.Text.Trim(),
                txtPhone.Text.Trim(), txtDepartment.Text.Trim(), out errorMsg
            );

            if (success)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadStaffList();
            }
            else
            {
                MessageBox.Show($"Lỗi: {errorMsg}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn vô hiệu hóa MSNV: {txtMSNV.Text}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                string errorMsg;
                bool success = _staffRepo.DeleteStaff(txtMSNV.Text.Trim(), out errorMsg);

                if (success)
                {
                    MessageBox.Show("Vô hiệu hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadStaffList();
                }
                else
                {
                    MessageBox.Show($"Lỗi: {errorMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvStaffs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvStaffs.Rows[e.RowIndex];

            if (dgvStaffs.Columns[e.ColumnIndex].Name == "btnActionView")
            {
                if (dgvStaffs.Columns.Contains("Id") && row.Cells["Id"].Value != null)
                {
                    string idRaw = row.Cells["Id"].Value.ToString();
                    if (int.TryParse(idRaw, out int staffId))
                    {
                        ShowSecurityInfoByProc(staffId);
                        return;
                    }
                }
                MessageBox.Show("Không thể tìm thấy giá trị cột ID trên lưới dữ liệu!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtLastName.TextChanged -= AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged -= AutoGenerateEmail_TextChanged;

            txtMSNV.Text = row.Cells["MSNV"].Value?.ToString()?.Trim();
            txtLastName.Text = row.Cells["LastName"].Value?.ToString()?.Trim();
            txtFirstName.Text = row.Cells["FirstName"].Value?.ToString()?.Trim();
            txtEmail.Text = row.Cells["Email"].Value?.ToString()?.Trim();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString()?.Trim();
            txtDepartment.Text = row.Cells["Department"].Value?.ToString()?.Trim();

            SwitchMode(true);

            txtLastName.TextChanged += AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged += AutoGenerateEmail_TextChanged;
        }

        private void ShowSecurityInfoByProc(int staffId)
        {
            DataRow accInfo = _staffRepo.GetAccountInfoById(staffId);

            if (accInfo != null)
            {
                string msnv = accInfo["MSNV"].ToString();
                string username = accInfo["Username"].ToString();
                string email = accInfo["Email"].ToString();
                string fullName = accInfo["FullName"].ToString();

                using (var dialog = new AccountSecurityInfoDialog(staffId, msnv, username, email, fullName, 3))
                {
                    dialog.ShowDialog(this);
                }
            }
            else
            {
                MessageBox.Show($"Không tìm thấy dữ liệu giáo vụ có ID: {staffId}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearFields();
            if (_isEditMode) LoadStaffList();
        }

        private void ClearFields()
        {
            txtLastName.TextChanged -= AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged -= AutoGenerateEmail_TextChanged;

            txtMSNV.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtDepartment.Text = "Phòng Giáo vụ";

            SwitchMode(false);

            txtLastName.TextChanged += AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged += AutoGenerateEmail_TextChanged;
        }
    }
}