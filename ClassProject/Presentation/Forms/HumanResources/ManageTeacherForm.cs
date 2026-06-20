using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using ClassProject.DataAccess.Repositories.Interfaces;
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ManageTeacherForm : Form
    {
        private readonly ITeacherRepository _teacherRepo;
        private bool _isEditMode = false;
        private int _selectedTeacherId = -1;

        // Định nghĩa cấu trúc Email Doanh nghiệp dành riêng cho giảng viên
        private const string TEACHER_EMAIL_SUBDOMAIN = "@teacher.";
        private const string REQUIRED_EMAIL_SUFFIX = ".edu.vn";

        public ManageTeacherForm()
        {
            InitializeComponent();
            _teacherRepo = new TeacherRepository();

            if (txtPhone != null) txtPhone.KeyPress += TxtPhone_KeyPress;

            // Đăng ký sự kiện tự động tạo Email/Username khi Admin nhập chữ vào ô Họ & Tên
            txtLastName.TextChanged += AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged += AutoGenerateEmail_TextChanged;
        }

        private void ManageTeacherForm_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn || !UserSession.IsAdmin)
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chỉ tài khoản Quản trị viên mới được quyền truy cập.",
                                "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            StyleGrid();
            ResetFormToInsertMode();
            LoadTeacherGrid();
        }

        private void StyleGrid()
        {
            if (dgvTeachers == null) return;
            dgvTeachers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTeachers.AllowUserToAddRows = false;
            dgvTeachers.MultiSelect = false;
            dgvTeachers.RowTemplate.Height = 35;
            dgvTeachers.BackgroundColor = Color.White;
            dgvTeachers.BorderStyle = BorderStyle.None;
            dgvTeachers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvTeachers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTeachers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvTeachers.RowHeadersVisible = false;

            if (!dgvTeachers.Columns.Contains("btnViewAccount"))
            {
                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.Name = "btnViewAccount";
                btnColumn.HeaderText = "Tài khoản";
                btnColumn.Text = "👁️ Xem";
                btnColumn.UseColumnTextForButtonValue = true;
                btnColumn.Width = 80;
                btnColumn.FlatStyle = FlatStyle.Flat;
                dgvTeachers.Columns.Add(btnColumn);
            }
        }

        private void FormatGridColumns()
        {
            if (dgvTeachers.Columns.Count == 0) return;
            if (dgvTeachers.Columns.Contains("Id")) dgvTeachers.Columns["Id"].Visible = false;
            if (dgvTeachers.Columns.Contains("UserId")) dgvTeachers.Columns["UserId"].Visible = false;
            if (dgvTeachers.Columns.Contains("Status")) dgvTeachers.Columns["Status"].Visible = false;
            if (dgvTeachers.Columns.Contains("FirstName")) dgvTeachers.Columns["FirstName"].Visible = false;
            if (dgvTeachers.Columns.Contains("LastName")) dgvTeachers.Columns["LastName"].Visible = false;

            dgvTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dgvTeachers.Columns.Contains("btnViewAccount"))
            {
                dgvTeachers.Columns["btnViewAccount"].DisplayIndex = dgvTeachers.Columns.Count - 1;
            }
        }

        private void LoadTeacherGrid()
        {
            try
            {
                dgvTeachers.CellClick -= dgvTeachers_CellClick;
                DataTable dt = _teacherRepo.GetAllTeachers();
                dgvTeachers.DataSource = dt;
                FormatGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp danh sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dgvTeachers.CellClick += dgvTeachers_CellClick;
            }
        }

        #region 💡 TỰ ĐỘNG SINH THÔNG TIN DOANH NGHIỆP DÀNH CHO GIẢNG VIÊN
        private void AutoGenerateEmail_TextChanged(object sender, EventArgs e)
        {
            if (!_isEditMode)
            {
                string fullNameStr = $"{txtLastName.Text.Trim()} {txtFirstName.Text.Trim()}";
                if (string.IsNullOrWhiteSpace(fullNameStr))
                {
                    txtEmail.Clear();
                    return;
                }

                // Loại bỏ dấu tiếng Việt, viết thường liền nhau (Ví dụ: tranvanb)
                string unsignedName = RemoveSignForVietnamese(fullNameStr).Replace(" ", "").ToLower();

                // Định dạng đầu ra gợi ý: tranvanb@teacher.school.edu.vn
                txtEmail.Text = unsignedName + TEACHER_EMAIL_SUBDOMAIN + "school" + REQUIRED_EMAIL_SUFFIX;
            }
        }

        private string GenerateUsernameFromEmail(string email)
        {
            string localPart = email.Split('@')[0];
            return $"teacher_{localPart.ToLower().Trim()}"; // Định dạng: teacher_tranvanb
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return res.ToString();
        }

        private string RemoveSignForVietnamese(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ" };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y" };
            for (int i = 0; i < arr1.Length; i++)
            {
                str = str.Replace(arr1[i], arr2[i]);
                str = str.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return str;
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msgv = txtTeacherCode.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string gender = cboGender.SelectedItem?.ToString() ?? "Nam";
            string rank = cboRank.SelectedItem?.ToString() ?? "Thạc sĩ";
            DateTime birth = dtpBirthDate.Value;

            if (string.IsNullOrWhiteSpace(msgv) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Các trường thông tin thiết yếu không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra định dạng Email giảng viên
            if (!email.Contains(TEACHER_EMAIL_SUBDOMAIN, StringComparison.OrdinalIgnoreCase) || !email.EndsWith(REQUIRED_EMAIL_SUFFIX, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"Email giảng viên bắt buộc phải chứa '{TEACHER_EMAIL_SUBDOMAIN}' và đuôi '{REQUIRED_EMAIL_SUFFIX}'!", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? excludeId = _isEditMode ? (int?)_selectedTeacherId : null;
            if (_teacherRepo.IsDuplicateCheck(msgv, phone, email, excludeId))
            {
                MessageBox.Show("Mã số giảng viên hoặc Email này đã tồn tại trên hệ thống!", "Trùng dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isEditMode)
            {
                int status = chkActive.Checked ? 1 : 0;
                if (_teacherRepo.UpdateTeacher(_selectedTeacherId, firstName, lastName, birth, gender, phone, email, rank, status))
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
            }
            else
            {
                // ✨ SINH TÀI KHOẢN VÀ MẬT KHẨU TỰ ĐỘNG THEO FORM MỚI
                string username = GenerateUsernameFromEmail(email);
                string rawPassword = GenerateRandomPassword(8); // Sinh chuỗi mật khẩu 8 ký tự ngẫu nhiên bảo mật cao

                if (_teacherRepo.InsertTeacherWithAccount(msgv, firstName, lastName, birth, gender, phone, email, rank, username, rawPassword, out string dbError))
                {
                    string msgSuccess = $"🎉 THÊM GIẢNG VIÊN VÀ CẤP TÀI KHOẢN THÀNH CÔNG!\n\n" +
                                        $"• Username: {username}\n" +
                                        $"• Mật khẩu tạm: {rawPassword}\n\n" +
                                        $"Hãy lưu lại thông tin này để cấp phát cho giảng viên!";
                    MessageBox.Show(msgSuccess, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
                else
                {
                    MessageBox.Show($"Lỗi hệ thống: {dbError}", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ✨ BỔ SUNG SỰ KIỆN: XỬ LÝ KHI NHẤN NÚT XÓA GIẢNG VIÊN
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedTeacherId == -1)
            {
                MessageBox.Show("Vui lòng chọn một giảng viên từ danh sách để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa giảng viên này không?\nHành động này không thể hoàn tác!",
                                                 "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                if (_teacherRepo.DeleteTeacher(_selectedTeacherId))
                {
                    MessageBox.Show("Xóa hồ sơ giảng viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormToInsertMode();
                    LoadTeacherGrid();
                }
                else
                {
                    MessageBox.Show("Không thể xóa giảng viên này. Có thể hồ sơ đang được liên kết dữ liệu ở bảng khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ✨ BỔ SUNG SỰ KIỆN: LÀM TRỐNG FORM (HỦY CHẾ ĐỘ SỬA)
        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetFormToInsertMode();
        }

        // ✨ BỔ SUNG SỰ KIỆN: TÌM KIẾM THEO TỪ KHÓA REAL-TIME
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(keyword))
                {
                    LoadTeacherGrid();
                }
                else
                {
                    dgvTeachers.CellClick -= dgvTeachers_CellClick;
                    DataTable dt = _teacherRepo.SearchTeachers(keyword);
                    dgvTeachers.DataSource = dt;
                    FormatGridColumns();
                    dgvTeachers.CellClick += dgvTeachers_CellClick;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void dgvTeachers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvTeachers.CurrentRow == null) return;
            DataGridViewRow row = dgvTeachers.Rows[e.RowIndex];

            if (dgvTeachers.Columns[e.ColumnIndex].Name == "btnViewAccount")
            {
                if (row.Cells["Id"].Value != null && row.Cells["Id"].Value != DBNull.Value)
                {
                    int teacherId = Convert.ToInt32(row.Cells["Id"].Value);
                    DataRow accInfo = _teacherRepo.GetAccountInfoByTeacherId(teacherId);
                    if (accInfo != null)
                    {
                        var diag = new AccountSecurityInfoDialog(teacherId, accInfo["MSGV"].ToString(), accInfo["Username"].ToString(), accInfo["Email"].ToString(), accInfo["FullName"].ToString(), 2);
                        diag.ShowDialog();
                    }
                }
                return;
            }

            // Gán ngược dữ liệu lên TextBox để phục vụ sửa đổi
            txtLastName.TextChanged -= AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged -= AutoGenerateEmail_TextChanged;

            _selectedTeacherId = Convert.ToInt32(row.Cells["Id"].Value);
            txtTeacherCode.Text = row.Cells["MSGV"].Value?.ToString();
            txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
            txtLastName.Text = row.Cells["LastName"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();

            if (row.Cells["Gender"].Value != null) cboGender.SelectedItem = row.Cells["Gender"].Value.ToString();
            if (row.Cells["AcademicRank"].Value != null) cboRank.SelectedItem = row.Cells["AcademicRank"].Value.ToString();
            if (row.Cells["DateOfBirth"].Value != DBNull.Value && row.Cells["DateOfBirth"].Value != null)
                dtpBirthDate.Value = Convert.ToDateTime(row.Cells["DateOfBirth"].Value);

            txtTeacherCode.Enabled = false;
            txtEmail.ReadOnly = true; // Khóa không cho sửa Email ở mode cập nhật để bảo toàn định dạng Auth
            SwitchMode(editMode: true);

            if (row.Cells["Status"].Value != DBNull.Value && row.Cells["Status"].Value != null)
            {
                if (chkActive != null) chkActive.Checked = Convert.ToInt32(row.Cells["Status"].Value) == 1;
            }

            txtLastName.TextChanged += AutoGenerateEmail_TextChanged;
            txtFirstName.TextChanged += AutoGenerateEmail_TextChanged;
        }

        private void SwitchMode(bool editMode)
        {
            _isEditMode = editMode;
            if (_isEditMode)
            {
                btnSave.Text = "💾 Cập nhật";
                if (chkActive != null) chkActive.Visible = true;
            }
            else
            {
                btnSave.Text = "(+) Thêm Giảng Viên";
                if (chkActive != null) chkActive.Visible = false;
                txtTeacherCode.Enabled = true;
                txtEmail.ReadOnly = false;
            }
        }

        private void ResetFormToInsertMode()
        {
            txtTeacherCode.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            _selectedTeacherId = -1;
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;
            if (cboRank.Items.Count > 0) cboRank.SelectedIndex = 0;
            dtpBirthDate.Value = DateTime.Now.AddYears(-25); // Giá trị mặc định hợp lý cho giảng viên
            SwitchMode(editMode: false);
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8) e.Handled = true;
        }
    }
}