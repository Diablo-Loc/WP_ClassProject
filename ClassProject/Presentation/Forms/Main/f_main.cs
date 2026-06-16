using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using ClassProject.Presentation.Forms.Admin;
using ClassProject.Presentation.Forms.Course;
using ClassProject.Presentation.Forms.Students;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class f_main : Form
    {
        private Form _currentForm;
        private readonly My_DB _db = new My_DB();
        private readonly StudentRepository _studentRepo;

        // API Windows ẩn thanh cuộn nhưng giữ tính năng cuộn bằng code
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_HORZ = 0; // Thanh cuộn ngang

        public f_main()
        {
            InitializeComponent();
            _studentRepo = new StudentRepository();

            // Tự động kiểm tra quyền từ bộ nhớ Global thông qua Helper hướng đối tượng
            if (UserSession.IsStudent)
            {
                SyncStudentMssv();
            }
        }

        private void f_main_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // Nền tổng xám nhạt hiện đại theo chuẩn UI/UX quốc tế
            this.BackColor = Color.FromArgb(246, 248, 251);

            // Tự động đọc cấu hình trực tiếp từ UserSession toàn cục
            LoadRoleInfo();
            LoadMenu();

            // Gán sự kiện click cho 2 nút điều hướng menu ngang
            btnScrollLeft.Click += BtnScrollLeft_Click;
            btnScrollRight.Click += BtnScrollRight_Click;

            timerClock.Start();
        }

        private void HideFlowMenuScrollbar()
        {
            flowMenu.AutoScroll = true;
            flowMenu.PerformLayout();
            ShowScrollBar(flowMenu.Handle, SB_HORZ, false);
        }

        private void UpdateButtonVisibility()
        {
            if (flowMenu.HorizontalScroll.Value <= flowMenu.HorizontalScroll.Minimum)
            {
                btnScrollLeft.Visible = false;
            }
            else
            {
                btnScrollLeft.Visible = true;
            }

            int totalWidth = 0;
            foreach (Control ctrl in flowMenu.Controls)
            {
                if (ctrl.Visible)
                {
                    totalWidth += ctrl.Width + ctrl.Margin.Left + ctrl.Margin.Right;
                }
            }

            bool isAtEnd = (flowMenu.HorizontalScroll.Value + flowMenu.Width) >= (totalWidth - 5);

            if (totalWidth <= flowMenu.Width || isAtEnd)
            {
                btnScrollRight.Visible = false;
            }
            else
            {
                btnScrollRight.Visible = true;
            }
        }

        #region Mũi Tên Điều Hướng Menu (Scroll Logic)

        private void BtnScrollLeft_Click(object sender, EventArgs e)
        {
            int newScrollPosition = flowMenu.HorizontalScroll.Value - 150;
            if (newScrollPosition < flowMenu.HorizontalScroll.Minimum)
                newScrollPosition = flowMenu.HorizontalScroll.Minimum;

            flowMenu.AutoScrollPosition = new Point(newScrollPosition, 0);
            HideFlowMenuScrollbar();
            UpdateButtonVisibility();
        }

        private void BtnScrollRight_Click(object sender, EventArgs e)
        {
            int newScrollPosition = flowMenu.HorizontalScroll.Value + 150;
            if (newScrollPosition > flowMenu.HorizontalScroll.Maximum)
                newScrollPosition = flowMenu.HorizontalScroll.Maximum;

            flowMenu.AutoScrollPosition = new Point(newScrollPosition, 0);
            HideFlowMenuScrollbar();
            UpdateButtonVisibility();
        }

        #endregion

        #region User Identity Sync (Đồng bộ danh tính)

        private void LoadRoleInfo()
        {
            lblRole.Text = UserSession.RoleName;
        }

        private void SyncStudentMssv()
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    string query = @"
                        SELECT s.MSSV FROM Students s
                        INNER JOIN Users u ON s.UserId = u.Id
                        WHERE u.Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", UserSession.UserId);

                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            UserSession.UpdateStudentMssv(result.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL Error tại f_main.SyncStudentMssv: " + ex.ToString());

                MessageBox.Show("Hệ thống không thể xác định Mã số sinh viên định danh liên kết.\nVui lòng liên hệ Phòng đào tạo để đồng bộ tài khoản!",
                                "Lỗi đồng bộ dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ChildForm Manager

        private void OpenChildForm(Form child)
        {
            if (_currentForm != null)
            {
                _currentForm.Close();
                _currentForm.Dispose();
            }

            _currentForm = child;
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            pnlContainer.Controls.Clear();
            pnlContainer.Controls.Add(child);
            pnlContainer.Tag = child;
            child.BringToFront();
            child.Show();
        }

        #endregion

        #region Modern Guna Menu Builder

        private Guna2Button CreateMenuButton(string text, EventHandler click)
        {
            var btn = new Guna2Button
            {
                Text = text,
                AutoSize = true,
                Height = 48,
                MinimumSize = new Size(0, 48),
                MaximumSize = new Size(0, 48),
                Padding = new Padding(18, 0, 18, 0),
                TextAlign = HorizontalAlignment.Center,
                Margin = new Padding(2, 0, 2, 0),
                FillColor = Color.FromArgb(255, 255, 255),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                CustomBorderThickness = new Padding(0, 0, 0, 3),
                CustomBorderColor = Color.Transparent,
                ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton
            };

            btn.CheckedState.ForeColor = Color.FromArgb(37, 99, 235);
            btn.CheckedState.FillColor = Color.FromArgb(239, 246, 255);
            btn.CheckedState.CustomBorderColor = Color.FromArgb(37, 99, 235);

            btn.HoverState.FillColor = Color.FromArgb(241, 245, 249);
            btn.HoverState.ForeColor = Color.FromArgb(15, 23, 42);

            btn.BorderRadius = 4;
            btn.Cursor = Cursors.Hand;
            btn.Click += click;

            btn.SizeChanged += (s, e) => HideFlowMenuScrollbar();

            return btn;
        }

        private void LoadMenu()
        {
            flowMenu.Controls.Clear();

            // Dashboard mặc định khả dụng với tất cả các vị trí
            Guna2Button btnDashboard = CreateMenuButton("Dashboard", Dashboard_Click);
            flowMenu.Controls.Add(btnDashboard);

            btnDashboard.Checked = true;
            Dashboard_Click(btnDashboard, EventArgs.Empty);

            // ===================================================================
            // 🌟 CẤU TRÚC PHÂN QUYỀN CHUẨN DOANH NGHIỆP (RBAC) - 4 NHÓM VAI TRÒ
            // ===================================================================

            if (UserSession.IsAdmin)
            {
                // Admin tối cao: Full quyền toàn bộ các cấu phần hệ thống
                flowMenu.Controls.Add(CreateMenuButton("Quản lý tài khoản", Account_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý giảng viên", Teacher_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý sinh viên", Student_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý ngành học", Major_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý lớp học", Classroom_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý môn học", Course_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý lớp học phần", CourseSection_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý đăng ký học phần", Registration_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý điểm số", Score_Click));
                flowMenu.Controls.Add(CreateMenuButton("Yêu cầu sinh viên", Request_Click));
                flowMenu.Controls.Add(CreateMenuButton("Thống kê tổng quan", Statistic_Click));
                flowMenu.Controls.Add(CreateMenuButton("Báo cáo hệ thống", Report_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý Danh bạ", Contact_Click));
            }
            else if (UserSession.IsStaff)
            {
                // Giáo vụ / Phòng đào tạo: Quản lý vận hành đào tạo toàn trường (Không được sửa tài khoản/giảng viên)
                flowMenu.Controls.Add(CreateMenuButton("Quản lý sinh viên", Student_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý ngành học", Major_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý lớp học", Classroom_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý môn học", Course_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý lớp học phần", CourseSection_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý đăng ký học phần", Registration_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý điểm số", Score_Click));
                flowMenu.Controls.Add(CreateMenuButton("Thống kê đào tạo", Statistic_Click));
                flowMenu.Controls.Add(CreateMenuButton("Báo cáo đào tạo", Report_Click));
            }
            else if (UserSession.IsTeacher)
            {
                // Giảng viên: Tập trung hoàn toàn vào lớp phụ trách và nghiệp vụ giảng dạy
                flowMenu.Controls.Add(CreateMenuButton("Thông tin cá nhân", Profile_Click));
                flowMenu.Controls.Add(CreateMenuButton("Lớp học phần của tôi", CourseSection_Click)); // Tái sử dụng form, tự lọc theo mã GV
                flowMenu.Controls.Add(CreateMenuButton("Sinh viên lớp tôi phụ trách", Student_Click)); // Tái sử dụng form, tự lọc danh sách sinh viên học lớp phần của GV
                flowMenu.Controls.Add(CreateMenuButton("Nhập & Sửa điểm số", Score_Click));         // Tái sử dụng form, khóa chỉ cho sửa lớp mình dạy
            }
            else if (UserSession.IsStudent)
            {
                // Sinh viên: Học tập và tra cứu cá nhân
                flowMenu.Controls.Add(CreateMenuButton("Thông tin cá nhân", Profile_Click));
                flowMenu.Controls.Add(CreateMenuButton("Đăng ký học phần", StudentRegisterCourse_Click));
                flowMenu.Controls.Add(CreateMenuButton("Học phần đã đăng ký", MyCourses_Click));
                flowMenu.Controls.Add(CreateMenuButton("Bảng điểm cá nhân", StudentScores_Click));
                flowMenu.Controls.Add(CreateMenuButton("Yêu cầu hỗ trợ", StudentRequests_Click));
            }

            this.BeginInvoke((MethodInvoker)delegate
            {
                HideFlowMenuScrollbar();
                UpdateButtonVisibility();
            });
        }

        #endregion

        #region Menu Click Route Events (Định tuyến sự kiện sạch)

        private void Dashboard_Click(object sender, EventArgs e) => OpenChildForm(new DashBoardForm());
        private void Account_Click(object sender, EventArgs e) => OpenChildForm(new AccountManageForm());
        private void Teacher_Click(object sender, EventArgs e) => OpenChildForm(new ManageTeacherForm());
        private void Student_Click(object sender, EventArgs e) => OpenChildForm(new ManageStudentForm());
        private void Major_Click(object sender, EventArgs e) => OpenChildForm(new ManageMajorForm());
        private void Classroom_Click(object sender, EventArgs e) => OpenChildForm(new ManageClassroomForm());
        private void Course_Click(object sender, EventArgs e) => OpenChildForm(new ManageCourseForm());
        private void CourseSection_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ManageCourseSectionForm());
        }
        private void Score_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng hiện tại có thuộc 1 trong 3 nhóm quyền hợp lệ không
            if (UserSession.IsLoggedIn && (UserSession.IsAdmin || UserSession.IsStaff || UserSession.IsTeacher))
            {
                OpenChildForm(new ManageScoreForm());
            }
            else
            {
                MessageBox.Show("Tài khoản của bạn không có quyền truy cập vào phân hệ Quản lý điểm số!",
                                "Quyền Truy Cập Bị Từ Chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void Registration_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Phiên làm việc hệ thống không hợp lệ hoặc đã hết hạn!", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new Course.ManageRegistrationForm());
        }

        private void Request_Click(object sender, EventArgs e) => OpenChildForm(new Admin.AdminApproveRequestForm());
        private void Statistic_Click(object sender, EventArgs e)
        {
            // 1. Xác định chuỗi Role đại diện từ UserSession toàn cục
            string role = "Student"; // Giá trị mặc định an toàn
            int referenceId = 0;

            if (UserSession.IsAdmin)
            {
                role = "Admin";
            }
            else if (UserSession.IsStaff)
            {
                role = "HR"; // Bản đồ ánh xạ Staff sang quyền HR ở form thống kê
            }

            // 2. Khởi tạo Form với các tham số phân quyền và đẩy vào vùng hiển thị ChildForm
            OpenChildForm(new StatisticsForm(role, referenceId));
        }
        private void Report_Click(object sender, EventArgs e) => OpenChildForm(new ReportFormHR());
        private void Contact_Click(object sender, EventArgs e) => OpenChildForm(new ContactForm());

        // --- ĐIỀU HƯỚNG SINH VIÊN & THÔNG TIN CÁ NHÂN ---
        private void Profile_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra trạng thái đăng nhập hệ thống trước
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập hệ thống để sử dụng chức năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra quyền hạn tài khoản (Chỉ cho phép Sinh viên hoặc Giảng viên)
            if (!UserSession.IsStudent && !UserSession.IsTeacher)
            {
                MessageBox.Show("Chức năng chỉ áp dụng cho tài khoản định danh Sinh viên hoặc Giảng viên!", "Quyền Truy Cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // 3. Khởi tạo và mở Form Hồ sơ cá nhân
            ProfileForm profileForm = new ProfileForm();
            OpenChildForm(profileForm);
        }

        private void StudentRegisterCourse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Hệ tokens chưa đồng bộ được Mã số sinh viên cá nhân của bạn!", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new Course.ManageRegistrationForm());
        }

        private void MyCourses_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("Phiên làm việc đã hết hạn!", "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new Presentation.Forms.Students.MyCoursesForm());
        }

        private void StudentScores_Click(object sender, EventArgs e) => OpenChildForm(new TranscriptForm());

        private void StudentRequests_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Không tìm thấy thông tin Mã số sinh viên (MSSV) liên kết.", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new StudentRequestForm());
        }

        // --- ĐĂNG XUẤT ---
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UserSession.Clear();
                this.Close();
            }
        }

        #endregion

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}