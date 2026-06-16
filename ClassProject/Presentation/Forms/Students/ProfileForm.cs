using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class ProfileForm : Form
    {
        private readonly StudentRepository _studentRepository;
        private readonly int _currentMssv; // MSSV của sinh viên đang đăng nhập
        private Student _currentStudent;   // Biến lưu trữ thông tin sinh viên hiện tại
        private byte[] _selectedAvatarBytes = null; // Lưu mảng byte của ảnh mới chọn
        // Constructor truyền vào chuỗi kết nối và MSSV sinh viên đang đăng nhập
        public ProfileForm(string connectionString, int loggedInMssv)
        {
            InitializeComponent();
            _studentRepository = new StudentRepository(connectionString);
            _currentMssv = loggedInMssv;

            // Gán sự kiện cho các nút bấm
            btnChooseAvatar.Click += BtnChooseAvatar_Click;
            btnUpdate.Click += BtnUpdate_Click;
            this.Load += ProfileForm_Load;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            LoadStudentProfile();
        }
        // Hàm tải dữ liệu sinh viên từ DB lên giao diện
        private void LoadStudentProfile()
        {
            _currentStudent = _studentRepository.GetStudentByMssv(_currentMssv);

            if (_currentStudent == null)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên trên hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // Đổ dữ liệu vào các Control
            txtStudentId.Text = _currentStudent.Mssv.ToString();
            txtUsername.Text = _currentStudent.UserId.ToString(); // Hoặc trường Username nếu bạn liên kết bảng Users
            txtName.Text = $"{_currentStudent.LastName} {_currentStudent.FirstName}".Trim();
            txtEmail.Text = _currentStudent.Email;
            txtPhone.Text = _currentStudent.Phone;

            // Giả lập dữ liệu Lớp và Chuyên ngành (Do database hiện tại chưa có 2 cột này)
            txtClass.Text = "CNTT K16-A";
            txtMajor.Text = "Công nghệ thông tin";

            // Xử lý hiển thị ảnh đại diện
            if (_currentStudent.Picture != null && _currentStudent.Picture.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(_currentStudent.Picture))
                {
                    picAvatar.Image = Image.FromStream(ms);
                }
                _selectedAvatarBytes = _currentStudent.Picture;
            }
            else
            {
                picAvatar.Image = null; // Nếu không có ảnh, hiển thị mặc định của Guna Circle PictureBox
            }
        }

        // Sự kiện khi bấm nút "Đổi ảnh"
        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            if (ofdAvatar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Hiển thị ảnh lên giao diện vòng tròn
                    picAvatar.Image = Image.FromFile(ofdAvatar.FileName);

                    // Chuyển đổi tệp ảnh thành mảng byte để chuẩn bị lưu vào SQL
                    _selectedAvatarBytes = File.ReadAllBytes(ofdAvatar.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Sự kiện khi bấm nút "Cập nhật thông tin"
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra tính hợp lệ cơ bản (Validation)
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng điền họ và tên!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Email không đúng định dạng!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Tách Họ và Tên từ một ô TextBox nhập liệu duy nhất
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
                firstName = fullName; // Trường hợp sinh viên chỉ nhập đúng 1 từ
            }

            // 3. Gán các giá trị mới cập nhật vào Object sinh viên hiện tại
            _currentStudent.FirstName = firstName;
            _currentStudent.LastName = lastName;
            _currentStudent.Email = txtEmail.Text.Trim();
            _currentStudent.Phone = txtPhone.Text.Trim();
            _currentStudent.Picture = _selectedAvatarBytes;

            // 4. Gọi hàm cập nhật xuống tầng DataAccess bằng Repository của bạn
            bool isSuccess = _studentRepository.UpdateStudent(_currentStudent);

            if (isSuccess)
            {
                MessageBox.Show("Cập nhật thông tin cá nhân thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudentProfile(); // Tải lại thông tin mới nhất
            }
            else
            {
                MessageBox.Show("Cập nhật thông tin thất bại. Vui lòng thử lại!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
