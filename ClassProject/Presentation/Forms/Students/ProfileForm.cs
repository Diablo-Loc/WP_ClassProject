using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
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

        // Cờ xác định chế độ giao diện (True: Giảng viên, False: Sinh viên)
        private bool _isTeacherMode = false;

        private Student _currentStudent;
        private Teacher _currentTeacher;
        private byte[] _selectedAvatarBytes = null;

        public ProfileForm()
        {
            InitializeComponent();
            _studentRepository = new StudentRepository();
            _teacherRepository = new TeacherRepository();

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

            // Phân luồng giao diện ngay từ khi nạp Form dựa vào Session quyền hành
            if (UserSession.IsTeacher)
            {
                _isTeacherMode = true;
                AdaptUiForTeacher(); // Cấu hình ẩn/hiện cấu phần chuyên biệt theo đúng DB Giáo viên
            }
            else if (UserSession.IsStudent) 
            {
                if (string.IsNullOrEmpty(UserSession.MSSV))
                {
                    MessageBox.Show("Không thể xác định MSSV của sinh viên hiện tại!", "Lỗi Đồng Bộ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.BeginInvoke(new Action(this.Close));
                    return;
                }
            }

            // 2. Đổ dữ liệu từ SQL Server lên các Control tương ứng
            LoadProfileData();
        }

        /// Tự động tinh chỉnh giao diện Guna/WinForms: Ẩn cấu phần ảnh, thông tin lớp/ngành và nhóm địa chỉ
        private void AdaptUiForTeacher()
        {
            // Thay đổi thông tin Tiêu đề Form và Tiêu đề Thẻ chính
            this.Text = "Hồ sơ thông tin cá nhân Giảng viên";
            if (lblMainTitle != null) lblMainTitle.Text = "Hồ sơ Giảng viên chi tiết";

            // 1. XỬ LÝ KHỐI CARD BÊN TRÁI (pnlLeftCard)
            if (picAvatar != null) picAvatar.Visible = false;                 // Giáo viên không có ảnh -> Ẩn khung tròn avatar
            if (btnChooseAvatar != null) btnChooseAvatar.Visible = false;     // Ẩn nút chọn ảnh
            if (lblSideMssv != null) lblSideMssv.Text = "Giảng viên cơ hữu";    // Thay đổi dòng chữ phụ bên trái
            if (lblBadgeStatus != null)
            {
                lblBadgeStatus.Text = "● Đang công tác";
                lblBadgeStatus.BackColor = Color.FromArgb(240, 253, 244);     // Màu nền xanh lục nhạt
                lblBadgeStatus.ForeColor = Color.FromArgb(22, 163, 74);       // Chữ xanh lục đậm
            }

            // 2. XỬ LÝ KHỐI HỌC VỤ & ĐĂNG NHẬP (grpAcademic)
            if (grpAcademic != null) grpAcademic.Text = "1. THÔNG TIN TÀI KHOẢN && ĐỊNH DANH GIẢNG VIÊN";
            if (lblStudentId != null) lblStudentId.Text = "Mã số giảng viên (MSGV)"; // Đổi nhãn tương thích mẫu Designer của bạn

            // Ẩn hoàn toàn các thuộc tính Lớp và Ngành (Vì bảng Giáo viên không có các trường này)
            if (lblClass != null) lblClass.Visible = false;
            if (txtClass != null) txtClass.Visible = false;
            if (lblMajor != null) lblMajor.Visible = false;
            if (txtMajor != null) txtMajor.Visible = false;

            // Co ngắn GroupBox Học vụ lại cho cân đối layout do đã ẩn 2 hàng dữ liệu bên dưới
            if (grpAcademic != null) grpAcademic.Size = new Size(grpAcademic.Width, 105);

            // Đẩy GroupBox thông tin cá nhân (grpPersonal) lên trên để bù đắp khoảng trống
            if (grpPersonal != null) grpPersonal.Location = new Point(grpPersonal.Location.X, 185);

            // 3. XỬ LÝ KHỐI ĐỊA CHỈ (grpAddress)
            // Vì schema table dbo.Teachers không có Address và Hometown, tiến hành ẩn luôn nhóm này
            if (grpAddress != null) grpAddress.Visible = false;

            // Đẩy nút Cập nhật (btnUpdate) lên vị trí cao hơn để Form nhìn gọn gàng và chuyên nghiệp
            if (btnUpdate != null) btnUpdate.Location = new Point(btnUpdate.Location.X, 410);
        }

        /// Truy vấn dữ liệu từ lớp Repository đổ trực tiếp lên UI dựa trên chế độ quyền
        private void LoadProfileData()
        {
            if (_isTeacherMode)
            {
                // 1. Gọi hàm nạp dữ liệu từ bảng dbo.Teachers
                _currentTeacher = _teacherRepository.GetTeacherByUserId(UserSession.UserId);

                if (_currentTeacher == null)
                {
                    // CHUẨN DOANH NGHIỆP: Cơ chế Lazy Initialization (Khởi tạo muộn nếu tài khoản chưa có hồ sơ)
                    _currentTeacher = new Teacher
                    {
                        UserId = UserSession.UserId,
                        Email = UserSession.Email ?? "chuyenviên@gmail.com", // Lấy email từ session đăng ký
                        FirstName = "Thành viên",
                        LastName = "Mới",
                        Status = 1,
                        // Tự sinh mã tạm thời dựa trên ID tài khoản để tránh trùng lặp UNIQUE constraint
                        MSGV = "TEMP_GV_" + UserSession.UserId
                    };

                    // Ghi nhận ngầm hồ sơ rỗng này vào Database trước
                    bool isInserted = _teacherRepository.InsertTeacherProfile(_currentTeacher);

                    if (!isInserted)
                    {
                        MessageBox.Show("Không thể tự động khởi tạo hồ sơ hệ thống cho tài khoản này!", "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }
                }

                // 2. Gán dữ liệu lên các Control tương ứng trên UI để người dùng sửa đổi
                txtStudentId.Text = _currentTeacher.MSGV;
                txtUsername.Text = UserSession.UserId.ToString();

                // Nếu là tài khoản mới sinh ngầm, hiển thị chuỗi rỗng để họ tự điền họ tên mới
                if (_currentTeacher.MSGV.StartsWith("TEMP_GV_"))
                {
                    txtName.Text = "";
                    lblSideFullName.Text = "Hồ sơ mới chưa cập nhật";
                    txtStudentId.ReadOnly = false; // Cho phép điền MSGV chính thức 1 lần duy nhất nếu cần
                }
                else
                {
                    txtName.Text = $"{_currentTeacher.LastName} {_currentTeacher.FirstName}".Trim();
                    lblSideFullName.Text = $"{_currentTeacher.LastName} {_currentTeacher.FirstName}".Trim();
                    txtStudentId.ReadOnly = true;  // Tài khoản chuẩn thì khóa lại không cho sửa mã số
                }

                txtEmail.Text = _currentTeacher.Email;
                txtPhone.Text = _currentTeacher.Phone;
            }
            else
            {
                // Gọi hàm nạp dữ liệu từ bảng dbo.Students
                _currentStudent = _studentRepository.GetStudentByMssv(UserSession.MSSV);

                if (_currentStudent == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin Sinh viên trên hệ thống học vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                txtStudentId.Text = _currentStudent.Mssv;
                txtUsername.Text = _currentStudent.UserId.ToString();
                txtName.Text = $"{_currentStudent.LastName} {_currentStudent.FirstName}".Trim();
                txtEmail.Text = _currentStudent.Email;
                txtPhone.Text = _currentStudent.Phone;

                // Các thuộc tính hành chính mặc định của Sinh viên
                txtClass.Text = "CNTT K16-A";
                txtMajor.Text = "Công nghệ thông tin";
                txtHometown.Text = _currentStudent.Hometown ?? "";
                txtAddress.Text = _currentStudent.Address ?? "";

                // Gán nhãn họ tên thẻ trái
                lblSideFullName.Text = $"{_currentStudent.LastName} {_currentStudent.FirstName}".Trim();
                lblSideMssv.Text = $"MSSV: {_currentStudent.Mssv}";

                // Chuyển mảng nhị phân ảnh đổ lên khung Guna2CirclePictureBox
                BindStudentAvatar(_currentStudent.Picture);
            }
        }

        /// Tạo luồng đọc bộ nhớ Stream để hiển thị Avatar nhị phân (Chỉ dành cho sinh viên)
        private void BindStudentAvatar(byte[] imgBytes)
        {
            if (imgBytes != null && imgBytes.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    picAvatar.Image = Image.FromStream(ms);
                }
                _selectedAvatarBytes = imgBytes;
            }
            else
            {
                picAvatar.Image = null;
                _selectedAvatarBytes = null;
            }
        }

        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            // Hàm chỉ hoạt động ở chế độ sinh viên (Vì chế độ giảng viên nút này đã bị ẩn hoàn toàn)
            if (ofdAvatar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    picAvatar.Image = Image.FromFile(ofdAvatar.FileName);
                    _selectedAvatarBytes = File.ReadAllBytes(ofdAvatar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tải tệp tin hình ảnh này lên hệ thống: " + ex.Message, "Lỗi Tải File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra tính toàn vẹn dữ liệu nhập (Validation)
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Họ và tên không được phép để trống!", "Nhắc nhở nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Định dạng thư điện tử (Email) không đúng tiêu chuẩn hệ thống!", "Nhắc nhở nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Thuật toán phân rã chuỗi Họ và Tên tự động
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
                firstName = fullName; // Trường hợp tên viết liền không chứa khoảng trống dấu cách
            }

            bool isSuccess = false;

            // 3. Thực thi lưu trữ phân luồng theo quyền hạn tài khoản
            if (_isTeacherMode)
            {
                // Nếu trước đó là mã tạm và người dùng đã nhập mã mới, lấy mã mới
                if (_currentTeacher.MSGV.StartsWith("TEMP_GV_"))
                {
                    if (string.IsNullOrWhiteSpace(txtStudentId.Text) || txtStudentId.Text.StartsWith("TEMP_GV_"))
                    {
                        MessageBox.Show("Vui lòng nhập Mã số định danh (MSGV) chính thức của bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    _currentTeacher.MSGV = txtStudentId.Text.Trim();
                }

                _currentTeacher.FirstName = firstName;
                _currentTeacher.LastName = lastName;
                _currentTeacher.Email = txtEmail.Text.Trim();
                _currentTeacher.Phone = txtPhone.Text.Trim();

                // Thực hiện cập nhật lại toàn bộ hồ sơ
                isSuccess = _teacherRepository.UpdateTeacherProfile(_currentTeacher);
            }
            else
            {
                _currentStudent.FirstName = firstName;
                _currentStudent.LastName = lastName;
                _currentStudent.Email = txtEmail.Text.Trim();
                _currentStudent.Phone = txtPhone.Text.Trim();
                _currentStudent.Hometown = txtHometown.Text.Trim();
                _currentStudent.Address = txtAddress.Text.Trim();
                _currentStudent.Picture = _selectedAvatarBytes; // Đính kèm mảng byte ảnh của Sinh viên

                isSuccess = _studentRepository.UpdateStudent(_currentStudent);
            }

            // 4. Phản hồi kết quả nghiệp vụ tới người dùng
            if (isSuccess)
            {
                MessageBox.Show("Cập nhật thông tin hồ sơ cá nhân thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProfileData(); // Nạp lại dữ liệu mới nhất để đồng bộ hiển thị lên UI
            }
            else
            {
                MessageBox.Show("Cập nhật thông tin thất bại. Vui lòng kiểm tra lại đường truyền kết nối CSDL!", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}