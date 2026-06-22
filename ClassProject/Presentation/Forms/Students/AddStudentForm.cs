using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using ClassProject.Presentation.Forms.Auth;
using ClassProject.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace ClassProject
{
    public partial class AddStudentForm : Form
    {
        private byte[] studentImage = null;
        private readonly string targetMssv;

        // Gọi các tầng Repository chuẩn
        private readonly StudentRepository studentRepo;
        private readonly MajorRepository majorRepo;
        private readonly ClassRoomRepository classroomRepo;
        private VoiceRecognitionService _voiceService;

        private bool isDataLoading = false;
        private bool isSaving = false;

        // Định nghĩa hằng số quy chuẩn cấu trúc miền cho Sinh viên
        private const string STUDENT_EMAIL_SUBDOMAIN = "@student.";
        private const string REQUIRED_EMAIL_SUFFIX = ".edu.vn";

        public AddStudentForm(string mssv)
        {
            InitializeComponent();
            InitializeVoiceService();
            targetMssv = mssv;
            studentRepo = new StudentRepository();
            majorRepo = new MajorRepository();
            classroomRepo = new ClassRoomRepository();

            txtMSSV.TextChanged += AutoGenerateStudentEmail_TextChanged;
        }

        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            isDataLoading = true;
            LoadMajorComboBox();

            cboMaNganh.SelectedIndexChanged -= CboMaNganh_SelectedIndexChanged;
            cboMaNganh.SelectedIndexChanged += CboMaNganh_SelectedIndexChanged;

            LoadClassroomComboBoxFiltered();
            isDataLoading = false;

            // CHẾ ĐỘ CHỈNH SỬA (EDIT)
            if (!string.IsNullOrEmpty(targetMssv))
            {
                // Tạm hủy bắt sự kiện sinh tự động để không ghi đè dữ liệu cũ từ DB
                txtMSSV.TextChanged -= AutoGenerateStudentEmail_TextChanged;

                this.Text = "Chỉnh sửa thông tin sinh viên";
                btnSave.Text = "Cập nhật";
                txtMSSV.Text = targetMssv;
                txtMSSV.ReadOnly = true;
                txtMSSV.BackColor = SystemColors.InactiveCaption;

                LoadStudentDataForEdit();
            }
        }

        #region 💡 TỰ ĐỘNG SINH EMAIL SINH VIÊN CHUẨN ĐỊNH DẠNG
        private void AutoGenerateStudentEmail_TextChanged(object sender, EventArgs e)
        {
            // Chỉ tự động gợi ý điền khi thêm mới
            if (string.IsNullOrEmpty(targetMssv))
            {
                string mssvRaw = txtMSSV.Text.Trim();
                if (string.IsNullOrWhiteSpace(mssvRaw))
                {
                    txtEmail.Clear();
                    return;
                }

                // Tự sinh cấu trúc: [mssv]@student.school.edu.vn (Sinh viên có thể tùy biến subdomain 'school')
                txtEmail.Text = mssvRaw.ToLower() + STUDENT_EMAIL_SUBDOMAIN + "school" + REQUIRED_EMAIL_SUFFIX;
            }
        }
        #endregion

        private void LoadMajorComboBox()
        {
            try
            {
                DataTable dt = majorRepo.GetAllMajorsForComboBox();
                DataRow row = dt.NewRow();
                row["MaNganh"] = ""; // Mã rỗng để validate bẫy lỗi
                row["TenNganh"] = "-- Vui lòng chọn chuyên ngành --";
                dt.Rows.InsertAt(row, 0);

                cboMaNganh.DataSource = dt;
                cboMaNganh.DisplayMember = "TenNganh";
                cboMaNganh.ValueMember = "MaNganh";
                cboMaNganh.SelectedIndex = 0;
            }
            catch (SqlException)
            {
                MessageBox.Show("Mất kết nối đến cơ sở dữ liệu khi tải danh mục Ngành!", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục Ngành ngoài dự kiến: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadClassroomComboBoxFiltered()
        {
            if (cboMaNganh.SelectedValue == null) return;
            string selectedMajor = cboMaNganh.SelectedValue.ToString();

            try
            {
                DataTable dt = classroomRepo.GetClassroomsByMajor(selectedMajor);
                if (string.IsNullOrEmpty(selectedMajor))
                {
                    dt = new DataTable();
                    dt.Columns.Add("MaLop");
                    dt.Columns.Add("TenLop");
                    dt.Rows.Add("", "-- Vui lòng chọn Ngành trước --");
                }
                else
                {
                    dt = classroomRepo.GetClassroomsByMajor(selectedMajor);
                    DataRow row = dt.NewRow();
                    row["MaLop"] = "";
                    row["TenLop"] = "-- Vui lòng chọn lớp hành chính --";
                    dt.Rows.InsertAt(row, 0);
                }

                cboMaLop.DataSource = dt;
                cboMaLop.DisplayMember = "TenLop";
                cboMaLop.ValueMember = "MaLop";
                cboMaLop.SelectedIndex = 0;
            }
            catch (SqlException)
            {
                MessageBox.Show("Mất kết nối đến cơ sở dữ liệu khi tải danh mục Lớp!", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục Lớp ngoài dự kiến: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboMaNganh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isDataLoading) LoadClassroomComboBoxFiltered();
        }

        private void LoadStudentDataForEdit()
        {
            try
            {
                Student sv = studentRepo.GetStudentByMssv(targetMssv);
                if (sv != null)
                {
                    txtFirstName.Text = sv.FirstName?.Trim() ?? "";
                    txtLastName.Text = sv.LastName?.Trim() ?? "";

                    if (sv.DateOfBirth.HasValue)
                    {
                        if (sv.DateOfBirth.Value >= dtpDateOfBirth.MinDate && sv.DateOfBirth.Value <= dtpDateOfBirth.MaxDate)
                            dtpDateOfBirth.Value = sv.DateOfBirth.Value;
                    }

                    cboGender.Text = string.IsNullOrWhiteSpace(sv.Gender) ? "Nam" : sv.Gender.Trim();
                    txtPhone.Text = sv.Phone?.Trim() ?? "";
                    txtAddress.Text = sv.Address?.Trim() ?? "";
                    txtHometown.Text = sv.Hometown?.Trim() ?? "";
                    txtEmail.Text = sv.Email?.Trim() ?? "";

                    if (sv.UserId != null)
                    {
                        txtEmail.ReadOnly = true;
                        txtEmail.BackColor = SystemColors.InactiveCaption;

                        ToolTip toolTip = new ToolTip();
                        toolTip.SetToolTip(txtEmail, "Trường dữ liệu này được cố định do tài khoản đang hoạt động.");
                    }

                    isDataLoading = true;

                    if (!string.IsNullOrEmpty(sv.MaNganh))
                    {
                        // Quét thủ công để loại bỏ lỗi khoảng trắng CHAR(10) của SQL
                        for (int i = 0; i < cboMaNganh.Items.Count; i++)
                        {
                            DataRowView row = cboMaNganh.Items[i] as DataRowView;
                            if (row != null && row["MaNganh"].ToString().Trim() == sv.MaNganh.Trim())
                            {
                                cboMaNganh.SelectedIndex = i;
                                break;
                            }
                        }

                        // Nạp danh sách Lớp theo Ngành vừa quét được
                        LoadClassroomComboBoxFiltered();
                    }

                    if (!string.IsNullOrEmpty(sv.MaLop))
                    {
                        // Quét thủ công để gán đúng lớp học
                        for (int i = 0; i < cboMaLop.Items.Count; i++)
                        {
                            DataRowView row = cboMaLop.Items[i] as DataRowView;
                            if (row != null && row["MaLop"].ToString().Trim() == sv.MaLop.Trim())
                            {
                                cboMaLop.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    isDataLoading = false;
                    if (sv.Picture != null && sv.Picture.Length > 0)
                    {
                        studentImage = sv.Picture;
                        using (MemoryStream ms = new MemoryStream(studentImage))
                        {
                            Image oldImg = picStudent.Image;
                            picStudent.Image = Image.FromStream(ms);
                            oldImg?.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị hồ sơ hệ thống: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";
                ofd.Title = "Chọn ảnh đại diện sinh viên";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo fInfo = new FileInfo(ofd.FileName);
                        if (fInfo.Length > 2 * 1024 * 1024)
                        {
                            MessageBox.Show("Dung lượng ảnh vượt quá giới hạn cho phép (Tối đa 2MB)!", "Xác thực tệp tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (fInfo.Length == 0)
                        {
                            MessageBox.Show("Tệp tin hình ảnh bị trống (0 byte)!", "Xác thực tệp tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (Image testImg = Image.FromStream(fs))
                            {
                                Image oldImg = picStudent.Image;
                                picStudent.Image = new Bitmap(testImg);
                                oldImg?.Dispose();

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    testImg.Save(ms, testImg.RawFormat);
                                    studentImage = ms.ToArray();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Tệp tin không đúng cấu trúc đồ họa chuẩn, nghi vấn giả mạo định dạng hoặc file hỏng!", "An Ninh Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        studentImage = null;
                        Image oldImg = picStudent.Image;
                        picStudent.Image = null;
                        oldImg?.Dispose();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isSaving) return;
            isSaving = true;

            try
            {
                string mssv = txtMSSV.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string firstName = txtFirstName.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower(); // Chuẩn hóa chữ thường
                string phone = txtPhone.Text.Trim();

                // [Các bước Validate 1, 2, 3, 4 giữ nguyên...]
                if (!Regex.IsMatch(mssv, @"^[A-Za-z0-9]{6,20}$"))
                {
                    MessageBox.Show("MSSV không hợp lệ! Độ dài yêu cầu từ 6-20 ký tự, không chứa khoảng trắng hoặc ký tự đặc biệt.", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMSSV.Focus(); return;
                }
                if (string.IsNullOrEmpty(lastName) || lastName.Length > 100)
                {
                    MessageBox.Show("Họ đệm không được trống và không vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLastName.Focus(); return;
                }
                if (string.IsNullOrEmpty(firstName) || firstName.Length > 100)
                {
                    MessageBox.Show("Tên sinh viên không được trống và không vượt quá 100 ký tự!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFirstName.Focus(); return;
                }
                int age = DateTime.Today.Year - dtpDateOfBirth.Value.Year;
                if (dtpDateOfBirth.Value.Date > DateTime.Today || age < 17 || age > 50)
                {
                    MessageBox.Show("Ngày sinh không hợp lệ!\nTheo quy chế, độ tuổi nhập học phải từ 17 đến 50 tuổi.", "Xác thực Nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtpDateOfBirth.Focus(); return;
                }

                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || email.Length > 100)
                {
                    MessageBox.Show("Địa chỉ Email không đúng định dạng tiêu chuẩn!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus(); return;
                }
                if (!email.StartsWith(mssv, StringComparison.OrdinalIgnoreCase) ||
                    !email.Contains(STUDENT_EMAIL_SUBDOMAIN, StringComparison.OrdinalIgnoreCase) ||
                    !email.EndsWith(REQUIRED_EMAIL_SUFFIX, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Email sinh viên không khớp quy chuẩn quản lý!\n\nCấu trúc bắt buộc: {mssv.ToLower()}{STUDENT_EMAIL_SUBDOMAIN}[tên_miền]{REQUIRED_EMAIL_SUFFIX}", "Xác thực Giáo vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus(); return;
                }
                if (!Regex.IsMatch(phone, @"^(03|05|07|08|09)\d{8}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! Yêu cầu sử dụng đầu số di động Việt Nam chuẩn và đủ 10 chữ số.", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus(); return;
                }
                if (cboMaNganh.SelectedValue == null || cboMaLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Chuyên ngành và Lớp hành chính!", "Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (studentImage == null || studentImage.Length == 0)
                {
                    MessageBox.Show("Sinh viên chưa được tải lên Ảnh hồ sơ đại diện!","Xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboMaNganh.SelectedValue == null || string.IsNullOrEmpty(cboMaNganh.SelectedValue.ToString()) ||
                    cboMaLop.SelectedValue == null || string.IsNullOrEmpty(cboMaLop.SelectedValue.ToString()))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Chuyên ngành và Lớp hành chính để phân bổ sinh viên!", "Xác thực Nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboMaNganh.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(targetMssv)) // Chế độ THÊM MỚI
                {
                    // 1. Kiểm tra trùng MSSV bằng hàm có sẵn hoặc gọi truy vấn nhanh
                    if (studentRepo.GetStudentByMssv(mssv) != null)
                    {
                        MessageBox.Show($"Mã số sinh viên '{mssv}' đã tồn tại trên hệ thống. Không thể thêm trùng lặp!", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMSSV.Focus(); return;
                    }

                    // 2. Kiểm tra trùng Email để triệt tiêu lỗi UNIQUE KEY 'UQ_Students_Email'
                    if (studentRepo.IsEmailExists(email, string.Empty))
                    {
                        MessageBox.Show($"Địa chỉ Email hệ thống '{email}' đã được cấp phát cho một sinh viên khác trước đó!\nVui lòng kiểm tra lại.", "Trùng lặp Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtEmail.Focus(); return;
                    }
                }
                else // Chế độ CẬP NHẬT (EDIT)
                {
                    // Nếu sửa thông tin mà đổi email (nếu được phép), phải chắc chắn email mới không đụng hàng với ai khác
                    if (studentRepo.IsEmailExists(email, targetMssv))
                    {
                        MessageBox.Show($"Không thể cập nhật! Email '{email}' đang được sử dụng bởi một sinh viên khác.", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtEmail.Focus(); return;
                    }
                }

                // Đóng gói Model dữ liệu
                Student sv = new Student
                {
                    Mssv = mssv,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dtpDateOfBirth.Value,
                    Gender = string.IsNullOrWhiteSpace(cboGender.Text) ? "Nam" : cboGender.Text.Trim(),
                    Phone = phone,
                    Address = txtAddress.Text.Trim(),
                    Hometown = txtHometown.Text.Trim(),
                    Email = email,
                    Picture = studentImage,
                    MaNganh = cboMaNganh.SelectedValue.ToString(),
                    MaLop = cboMaLop.SelectedValue.ToString()
                };

                // Thực thi xuống DB
                if (string.IsNullOrEmpty(targetMssv))
                {
                    if (studentRepo.AddStudent(sv))
                    {
                        MessageBox.Show("Thêm mới hồ sơ sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    if (studentRepo.UpdateStudent(sv))
                    {
                        MessageBox.Show("Cập nhật thông tin hồ sơ sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Ràng Buộc Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                // Bẫy phòng hờ tầng cuối nếu tầng kiểm tra trên RAM lọt lưới do hạ tầng mạng chậm trễ (Race Condition)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Lỗi đồng bộ: Email hoặc Mã định danh này vừa mới được đăng ký ngầm bởi một phiên làm việc khác!", "Dữ liệu trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Mất kết nối đột ngột với SQL Server Engine!", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ngoài dự phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isSaving = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Image oldImg = picStudent.Image;
            picStudent.Image = null;
            oldImg?.Dispose();
            this.Close();
        }

        private void ClearAllFields()
        {
            // 1. Chỉ xóa trắng MSSV và Email nếu đang ở chế độ THÊM MỚI
            if (string.IsNullOrEmpty(targetMssv))
            {
                txtMSSV.Clear();
                txtEmail.Clear();
            }

            // 2. Xóa các ô Textbox nhập liệu thông thường
            txtLastName.Clear();
            txtFirstName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtHometown.Clear();

            // 3. Reset Ngày sinh về mặc định chuẩn nghiệp vụ (Cách đây 18 năm)
            DateTime defaultDOB = DateTime.Today.AddYears(-18);
            if (defaultDOB >= dtpDateOfBirth.MinDate && defaultDOB <= dtpDateOfBirth.MaxDate)
            {
                dtpDateOfBirth.Value = defaultDOB;
            }

            // 4. Reset ComboBox về lựa chọn đầu tiên (tránh bị null)
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;
            if (cboMaNganh.Items.Count > 0) cboMaNganh.SelectedIndex = 0;
            // cboMaLop sẽ tự động cập nhật lại theo sự kiện SelectedIndexChanged của MaNganh

            // 5. Xóa ảnh đại diện an toàn (Sử dụng lại hàm chống Memory Leak đã viết)
            studentImage = null;
            if (picStudent.Image != null)
            {
                Image oldImg = picStudent.Image;
                picStudent.Image = null;
                oldImg?.Dispose();
            }

            // 6. Đưa con trỏ chuột về ô nhập liệu đầu tiên để người dùng gõ lại ngay
            if (string.IsNullOrEmpty(targetMssv))
                txtMSSV.Focus();
            else
                txtLastName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ dữ liệu đang nhập không?",
                                                 "Xác nhận làm mới",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearAllFields();
            }
        }
        private void InitializeVoiceService()
        {
            try
            {
                _voiceService = new VoiceRecognitionService();

                // Chỉ đăng ký duy nhất sự kiện trả dữ liệu cuối cùng khi nói xong
                _voiceService.OnStudentDataParsed += VoiceService_OnStudentDataParsed;
                _voiceService.OnListeningStatusChanged += VoiceService_OnListeningStatusChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n(Hệ thống vẫn hoạt động bằng cách nhập liệu thủ công bình thường)", "Thông báo thiết bị");
            }
        }

        private void btnVoiceInput_Click(object sender, EventArgs e)
        {
            if (_voiceService == null || !_voiceService.IsRecognizerAvailable())
            {
                MessageBox.Show("Máy tính của bạn chưa cài đặt tính năng Speech Recognition của Windows hoặc thiếu thiết bị thu âm (Mic).\n\nVui lòng nhập liệu bằng tay!", "Thông báo hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _voiceService.ToggleListening();
        }

        private void VoiceService_OnListeningStatusChanged(bool isListening)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VoiceService_OnListeningStatusChanged(isListening)));
                return;
            }

            if (isListening)
            {
                // 1. Đổi chữ nút bấm để bạn biết là mic đang bật
                btnVoiceInput.Text = "🛑 Đang nghe... Hãy nói";
                btnVoiceInput.FillColor = System.Drawing.Color.FromArgb(253, 237, 237);
                btnVoiceInput.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69);

                // 2. Hiện MessageBox báo hiệu cho bạn bắt đầu nói
                MessageBox.Show("Hệ thống đã bật Mic và đang lắng nghe!\nHãy nói rõ [Họ tên] và [Mã số sinh viên] của bạn.",
                                "Trạng thái Mic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Trả nút bấm về trạng thái cũ
                btnVoiceInput.Text = "🎙️ Nhập giọng nói";
                btnVoiceInput.FillColor = System.Drawing.Color.Transparent;
                btnVoiceInput.ForeColor = System.Drawing.Color.FromArgb(40, 167, 69);
            }
        }

        private void VoiceService_OnStudentDataParsed(string hoTen, string mssv)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => VoiceService_OnStudentDataParsed(hoTen, mssv)));
                return;
            }

            // Đổ dữ liệu xịn nhận từ Service vào TextBox trên UI
            if (!string.IsNullOrEmpty(hoTen))
            {
                hoTen = hoTen.Trim();
                int lastSpaceIndex = hoTen.LastIndexOf(' ');

                if (lastSpaceIndex != -1)
                {
                    txtLastName.Text = hoTen.Substring(0, lastSpaceIndex).Trim();
                    txtFirstName.Text = hoTen.Substring(lastSpaceIndex + 1).Trim();
                }
                else
                {
                    txtLastName.Text = string.Empty;
                    txtFirstName.Text = hoTen;
                }
            }
            if (!string.IsNullOrEmpty(mssv)) txtMSSV.Text = mssv;

            // Bật hiện thông báo kết quả cuối cùng thu được cho bạn check dữ liệu
            MessageBox.Show($"Hệ thống đã nhận diện xong!\n- Họ và tên: {hoTen}\n- MSSV: {mssv}",
                            "Kết quả nhận diện", MessageBoxButtons.OK, MessageBoxIcon.Information); // <-- Thay bằng Information
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _voiceService?.Dispose();
        }

        private void btnScanFullInfo_Click(object sender, EventArgs e)
        {
            // Gọi Form quét thẻ với chế độ FullInfo lấy đầy đủ thông tin
            using (var scannerForm = new CardScannerForm(ScannerMode.FullInfo))
            {
                if (scannerForm.ShowDialog() == DialogResult.OK)
                {
                    // 1. Điền MSSV bóc tách được từ Azure OCR
                    if (!string.IsNullOrEmpty(scannerForm.DetectedMSSV))
                    {
                        txtMSSV.Text = scannerForm.DetectedMSSV;
                    }

                    // 2. Điền Họ và Tên (Tách chuỗi dựa trên scannerForm.DetectedName)
                    string fullOcrName = scannerForm.DetectedName;
                    if (!string.IsNullOrEmpty(fullOcrName))
                    {
                        fullOcrName = fullOcrName.Trim();
                        int lastSpaceIndex = fullOcrName.LastIndexOf(' ');

                        if (lastSpaceIndex != -1)
                        {
                            // Lấy phần Họ đệm (Ví dụ: "Nguyễn Văn")
                            txtLastName.Text = fullOcrName.Substring(0, lastSpaceIndex).Trim();
                            // Lấy phần Tên chính (Ví dụ: "An")
                            txtFirstName.Text = fullOcrName.Substring(lastSpaceIndex + 1).Trim();
                        }
                        else
                        {
                            // Trường hợp đặc biệt nếu tên chỉ có duy nhất 1 từ
                            txtLastName.Text = string.Empty;
                            txtFirstName.Text = fullOcrName;
                        }
                    }

                    // 3. Xử lý và điền Ngày sinh vào DateTimePicker an toàn
                    string ocrDob = scannerForm.DetectedDOB; // Chuỗi định dạng dd/MM/yyyy từ Azure
                    if (!string.IsNullOrEmpty(ocrDob))
                    {
                        if (DateTime.TryParseExact(ocrDob, "dd/MM/yyyy",
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None,
                                                   out DateTime parsedDate))
                        {
                            // Kiểm tra xem ngày sinh nằm trong phạm vi cho phép của DateTimePicker không
                            if (parsedDate >= dtpDateOfBirth.MinDate && parsedDate <= dtpDateOfBirth.MaxDate)
                            {
                                dtpDateOfBirth.Value = parsedDate;
                            }
                        }
                        else
                        {
                            // Dự phòng nếu định dạng ngày từ Azure trả về cấu trúc khác chuẩn dd/MM/yyyy
                            try { dtpDateOfBirth.Text = ocrDob; } catch { }
                        }
                    }
                }
            }
        }
    }
}