using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class ProfileForm : Form
    {
        private readonly StudentRepository _studentRepository;
        private readonly TeacherRepository _teacherRepository;

        private bool _isTeacherMode = false;
        private Student _currentStudent;
        private Teacher _currentTeacher;
        private byte[] _selectedAvatarBytes = null;

        public ProfileForm()
        {
            InitializeComponent();
            _studentRepository = new StudentRepository();
            _teacherRepository = new TeacherRepository();

            // Đăng ký sự kiện
            btnChooseAvatar.Click += BtnChooseAvatar_Click;
            btnUpdate.Click += BtnUpdate_Click;
            this.Load += ProfileForm_Load;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            // 1. KIỂM TRA BẢO MẬT: Đảm bảo phiên làm việc hợp lệ
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Phiên làm việc đã hết hạn hoặc không hợp lệ!", "Lỗi Xác Thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new Action(this.Close));
                return;
            }

            // 2. PHÂN LUỒNG GIAO DIỆN THEO QUYỀN HẠN SỐNG CỦA SESSION
            if (UserSession.IsTeacher)
            {
                _isTeacherMode = true;
                AdaptUiForTeacher();
            }
            else if (UserSession.IsStudent && string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Không thể xác định MSSV của sinh viên hiện tại!", "Lỗi Đồng Bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new Action(this.Close));
                return;
            }

            // 3. ĐỔ DỮ LIỆU LÊN UI
            LoadProfileData();
        }

        /// <summary>
        /// Tinh chỉnh cấu trúc hiển thị động cho Giảng viên dựa trên Entity thực tế
        /// </summary>
        private void AdaptUiForTeacher()
        {
            this.Text = "Hồ sơ thông tin cá nhân Giảng viên";
            if (lblMainTitle != null) lblMainTitle.Text = "Hồ sơ Giảng viên chi tiết";

            // 1. Cấu hình khối bên trái (Ẩn avatar theo logic của bạn)
            if (picAvatar != null) picAvatar.Visible = false;
            if (btnChooseAvatar != null) btnChooseAvatar.Visible = false;
            if (lblSideMssv != null) lblSideMssv.Text = "Giảng viên cơ hữu";

            if (lblBadgeStatus != null)
            {
                lblBadgeStatus.Text = "● Đang công tác";
                lblBadgeStatus.BackColor = Color.FromArgb(240, 253, 244);
                lblBadgeStatus.ForeColor = Color.FromArgb(22, 163, 74);
            }

            // 2. SỬA LỖI CỤT CHỮ: Bật AutoSize = true để nhãn tự giãn rộng ra, không bị giới hạn bởi pixel cũ
            if (lblStudentId != null)
            {
                lblStudentId.AutoSize = true;
                lblStudentId.Text = "Mã số giảng viên (MSGV)";
            }
            if (lblClass != null)
            {
                lblClass.AutoSize = true;
                lblClass.Text = "Học hàm / Học vị";
            }
            if (lblName != null)
            {
                lblName.AutoSize = true;
                lblName.Text = "Họ và tên giảng viên";
            }
            if (lblBirthDate != null)
            {
                lblBirthDate.AutoSize = true;
                lblBirthDate.Text = "Ngày sinh";
            }

            // 3. Ẩn cột Chuyên ngành học (bên phải hàng 2 của Khối 1)
            if (lblMajor != null) lblMajor.Visible = false;
            if (txtMajor != null) txtMajor.Visible = false;

            // 4. Mở khóa ô học hàm giảng viên
            if (txtClass != null)
            {
                txtClass.ReadOnly = false;
                txtClass.Visible = true;
            }

            // 5. Ẩn khối địa chỉ hành chính (Khối 3)
            if (grpAddress != null) grpAddress.Visible = false;

            // =========================================================================
            // 6. CÂN CHỈNH KHOẢNG CÁCH KHÔNG BỊ ĐÈ (Dựa trên thông số gốc Designer)
            // =========================================================================

            if (grpAcademic != null)
            {
                grpAcademic.Text = "1. THÔNG TIN ĐẠI DIỆN VÀ HỌC HÀM GIẢNG VIÊN";
                // Tăng nhẹ chiều cao Khối 1 từ 173 lên 190 để chứa vừa vặn ô nhập liệu không bị kích sát đáy
                grpAcademic.Size = new Size(grpAcademic.Width, 190);
            }

            if (grpPersonal != null)
            {
                // Đẩy Khối 2 xuống tọa độ Y = 275 (Gốc là 256) nhằm tạo khoảng cách an toàn 
                // với Khối 1 vừa được tăng chiều cao, tránh việc đè lên ô Tiến sĩ.
                grpPersonal.Location = new Point(grpPersonal.Location.X, 275);
            }

            if (btnUpdate != null)
            {
                // Đẩy nút cập nhật xuống dưới Khối 2 một chút (Y = 535) cho cân đối
                btnUpdate.Location = new Point(btnUpdate.Location.X, 535);
                btnUpdate.Visible = true;
                btnUpdate.BringToFront(); // Đưa lên lớp trên cùng để không bị che khuất
            }
        }
        /// <summary>
        /// Nạp dữ liệu từ tầng dữ liệu lên UI sạch sẽ, có xử lý lỗi ràng buộc Entity
        /// </summary>
        private void LoadProfileData()
        {
            try
            {
                if (_isTeacherMode)
                {
                    _currentTeacher = _teacherRepository.GetTeacherByUserId(UserSession.UserId);

                    if (_currentTeacher == null)
                    {
                        // Khởi tạo muộn (Lazy Initialization) nếu tài khoản giáo viên chưa có bản ghi thông tin
                        _currentTeacher = new Teacher
                        {
                            UserId = UserSession.UserId,
                            Email = UserSession.Email ?? "giangvien@classproject.edu.vn",
                            FirstName = "Thành viên",
                            LastName = "Mới",
                            Status = 1,
                            MSGV = "TEMP_GV_" + UserSession.UserId
                        };

                        bool isInserted = _teacherRepository.InsertTeacher(
                            _currentTeacher.UserId, _currentTeacher.MSGV, _currentTeacher.FirstName,
                            _currentTeacher.LastName, null, null, null, _currentTeacher.Email, null
                        );

                        if (!isInserted)
                        {
                            MessageBox.Show("Không thể tự động khởi tạo hồ sơ hệ thống!", "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                        }
                    }

                    // Gán dữ liệu Giáo viên lên control
                    txtStudentId.Text = _currentTeacher.MSGV;
                    txtStudentId.ReadOnly = !_currentTeacher.MSGV.StartsWith("TEMP_GV_");
                    txtName.Text = _currentTeacher.FullName;
                    // Sửa lỗi hiển thị Họ và Tên theo đúng Entity Teacher (FirstName + LastName)
                    lblSideFullName.Text = string.IsNullOrWhiteSpace(txtName.Text) ? "Hồ sơ mới chưa cập nhật" : txtName.Text;
                    
                    txtEmail.Text = _currentTeacher.Email;
                    txtPhone.Text = _currentTeacher.Phone;
                    txtClass.Text = _currentTeacher.AcademicRank ?? "Chưa cập nhật"; // Ánh xạ vào ô học hàm
                    txtClass.ReadOnly = false; // Cho phép sửa học hàm/học vị

                    if (_currentTeacher.DateOfBirth.HasValue) dtpBirthDate.Value = _currentTeacher.DateOfBirth.Value;
                    cboGender.Text = _currentTeacher.Gender ?? "Nam";
                }
                else
                {
                    // Luồng nạp dữ liệu cho Sinh viên
                    _currentStudent = _studentRepository.GetStudentByMssv(UserSession.MSSV);

                    if (_currentStudent == null)
                    {
                        MessageBox.Show("Không tìm thấy thông tin Sinh viên trên hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                        return;
                    }

                    txtStudentId.Text = _currentStudent.Mssv;
                    txtStudentId.ReadOnly = true;

                    // Đồng bộ Họ tên sinh viên (LastName + FirstName)
                    txtName.Text = _currentStudent.FullName;
                    lblSideFullName.Text = _currentStudent.FullName;
                    lblSideMssv.Text = $"MSSV: {_currentStudent.Mssv}";

                    txtEmail.Text = _currentStudent.Email;
                    txtPhone.Text = _currentStudent.Phone;

                    // Tận dụng thuộc tính mở rộng từ Entity thay vì Fix cứng chuỗi text rác
                    txtClass.Text = _currentStudent.TenLop ?? "Chưa xếp lớp";
                    txtMajor.Text = _currentStudent.TenNganh ?? "Chưa phân ngành";
                    txtClass.ReadOnly = true;

                    txtHometown.Text = _currentStudent.Hometown ?? "";
                    txtAddress.Text = _currentStudent.Address ?? "";

                    if (_currentStudent.DateOfBirth.HasValue) dtpBirthDate.Value = _currentStudent.DateOfBirth.Value;
                    cboGender.Text = _currentStudent.Gender ?? "Nam";

                    BindStudentAvatar(_currentStudent.Picture);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp dữ liệu hồ sơ: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindStudentAvatar(byte[] imgBytes)
        {
            if (imgBytes != null && imgBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        picAvatar.Image = Image.FromStream(ms);
                    }
                    _selectedAvatarBytes = imgBytes;
                }
                catch
                {
                    picAvatar.Image = null;
                    _selectedAvatarBytes = null;
                }
            }
            else
            {
                picAvatar.Image = null;
                _selectedAvatarBytes = null;
            }
        }

        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            if (ofdAvatar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Đọc file và kiểm tra dung lượng ngay tại UI (Chống phình DB trước khi ném vào Entity)
                    FileInfo fileInfo = new FileInfo(ofdAvatar.FileName);
                    if (fileInfo.Length > 2 * 1024 * 1024)
                    {
                        MessageBox.Show("Dung lượng hình ảnh vượt quá giới hạn 2MB cho phép!", "Ràng buộc dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    picAvatar.Image = Image.FromFile(ofdAvatar.FileName);
                    _selectedAvatarBytes = File.ReadAllBytes(ofdAvatar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tải hình ảnh: " + ex.Message, "Lỗi Tải File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // 1. CHUẨN HÓA DỮ LIỆU ĐẦU VÀO (Validation căn bản)
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Họ và tên không được phép để trống!", "Nhắc nhở nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. PHÂN RÃ HỌ VÀ TÊN TỰ ĐỘNG THEO TỪNG THIẾT KẾ ENTITY
            string fullName = txtName.Text.Trim();
            string firstName = "";
            string lastName = "";

            int lastSpaceIndex = fullName.LastIndexOf(' ');
            if (lastSpaceIndex != -1)
            {
                firstName = fullName.Substring(lastSpaceIndex + 1);
                lastName = fullName.Substring(0, lastSpaceIndex);
            }
            else
            {
                firstName = fullName;
                lastName = "";
            }

            bool isSuccess = false;

            try
            {
                // 3. THỰC THI THAY ĐỔI
                if (_isTeacherMode)
                {
                    if (_currentTeacher.MSGV.StartsWith("TEMP_GV_"))
                    {
                        if (string.IsNullOrWhiteSpace(txtStudentId.Text) || txtStudentId.Text.StartsWith("TEMP_GV_"))
                        {
                            MessageBox.Show("Vui lòng nhập Mã số giảng viên (MSGV) chính thức!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        _currentTeacher.MSGV = txtStudentId.Text.Trim();
                    }

                    // Khớp đúng quy tắc gán dữ liệu cho Teacher Entity
                    _currentTeacher.FirstName = firstName;
                    _currentTeacher.LastName = lastName;
                    _currentTeacher.Email = txtEmail.Text.Trim();
                    _currentTeacher.Phone = txtPhone.Text.Trim();
                    _currentTeacher.Gender = cboGender.Text;
                    _currentTeacher.DateOfBirth = dtpBirthDate.Value;
                    _currentTeacher.AcademicRank = txtClass.Text.Trim(); // Lưu học hàm từ TextBox được tái sử dụng
                    _currentTeacher.Updated_At = DateTime.Now;

                    isSuccess = _teacherRepository.UpdateTeacherProfile(_currentTeacher);
                }
                else
                {
                    // Gán và kích hoạt bộ Trigger tự bẫy lỗi (Validation Property) của Student Entity
                    _currentStudent.FirstName = firstName;
                    _currentStudent.LastName = lastName;
                    _currentStudent.Email = txtEmail.Text.Trim();
                    _currentStudent.Phone = txtPhone.Text.Trim();
                    _currentStudent.Gender = cboGender.Text;
                    _currentStudent.DateOfBirth = dtpBirthDate.Value;
                    _currentStudent.Hometown = txtHometown.Text.Trim();
                    _currentStudent.Address = txtAddress.Text.Trim();
                    _currentStudent.Picture = _selectedAvatarBytes;

                    isSuccess = _studentRepository.UpdateStudent(_currentStudent);
                }

                // 4. PHẢN HỒI KẾT QUẢ NGHIỆP VỤ
                if (isSuccess)
                {
                    MessageBox.Show("Cập nhật thông tin hồ sơ cá nhân thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProfileData(); // Nạp lại dữ liệu đồng bộ
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin thất bại. Vui lòng kiểm tra lại kết nối CSDL!", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Bắt gọn các ngoại lệ chống sập do luật ràng buộc ném ra từ Entity thiết kế sẵn
                MessageBox.Show(ex.Message, "Lỗi Ràng Buộc Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi không xác định: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}