using ClassProject.Business.Services;
using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.DataAccess.Repositories.Implementations;
using ClassProject.Presentation.Forms.Admin;
using ClassProject.Presentation.Forms.Analytics;
using ClassProject.Presentation.Forms.Auth;
using ClassProject.Presentation.Forms.Course;
using ClassProject.Presentation.Forms.Score;
using ClassProject.Presentation.Forms.Students;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
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
        private bool _hasNewNotifications = false;
        private bool isDarkMode = false;
        private readonly AiChatService _aiService = new AiChatService();

        // Bảng màu cho Chế độ Sáng (Light Mode)
        private readonly Color LightBgColor = Color.FromArgb(246, 248, 251); // Màu nền tổng xám nhạt hiện đại của bạn
        private readonly Color LightHeaderColor = Color.White;
        private readonly Color LightTextColor = Color.FromArgb(30, 41, 59);

        // Bảng màu cho Chế độ Tối (Dark Mode)
        private readonly Color DarkBgColor = Color.FromArgb(15, 23, 42);     // Màu xanh đen Slate huyền bí (chuẩn Tailwind)
        private readonly Color DarkHeaderColor = Color.FromArgb(30, 41, 59); // Màu panel/header sáng hơn một chút
        private readonly Color DarkTextColor = Color.FromArgb(241, 245, 249);

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

            if (picAvatar != null)
            {
                picAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            this.BackColor = Color.FromArgb(246, 248, 251);

            LoadRoleInfo();
            LoadMenu();

            btnNotification.Paint += BtnNotification_Paint;
            btnScrollLeft.Click += BtnScrollLeft_Click;
            btnScrollRight.Click += BtnScrollRight_Click;

            CheckNewNotificationStatus();
            timerClock.Start();
            UserSession.RefreshActivity();
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
            // Lấy tên thật từ Session, nếu trống (như tài khoản Admin mới tạo chưa map profile) thì dùng Username làm fallback
            string displayName = !string.IsNullOrEmpty(UserSession.FullName) ? UserSession.FullName : UserSession.Username;

            lblUserName.Text = displayName;
            lblRole.Text = UserSession.RoleName.ToUpper();

            if (picAvatar != null)
            {
                // Vẽ avatar chữ động theo tên thật cực kỳ chuyên nghiệp
                picAvatar.Image = CreateAvatarImage(displayName);
            }
        }

        private Bitmap CreateAvatarImage(string name)
        {
            // Lấy kích thước thực tế từ control thay vì cố định số 42
            int size = picAvatar != null ? picAvatar.Width : 42;
            Bitmap bmp = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                string shortName = "U";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    string[] words = name.Trim().Split(' ');
                    if (words.Length > 0)
                    {
                        shortName = words[words.Length - 1].Substring(0, 1).ToUpper();
                    }
                }

                Color backColor = GetAcademicColor(shortName[0]);

                using (SolidBrush bgBrush = new SolidBrush(backColor))
                {
                    // Trừ đi 1 pixel giúp viền tròn phẳng mịn chuẩn UI
                    g.FillEllipse(bgBrush, 0, 0, size - 1, size - 1);
                }

                // Tự động căn chỉnh font size dựa theo kích thước avatar thực tế
                float fontSize = size * 0.28f;
                using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(shortName, font, textBrush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }
        private Color GetAcademicColor(char c)
        {
            int ascii = (int)c;
            switch (ascii % 5)
            {
                case 0: return Color.FromArgb(37, 99, 235);  // Royal Blue (Admin/Giáo vụ)
                case 1: return Color.FromArgb(13, 148, 136); // Teal 
                case 2: return Color.FromArgb(124, 58, 237); // Purple
                case 3: return Color.FromArgb(219, 39, 119); // Pink
                default: return Color.FromArgb(225, 29, 72); // Rose
            }
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
            child.Activated += (s, e) => UserSession.RefreshActivity();
            child.Click += (s, e) => UserSession.RefreshActivity();
            pnlContainer.Controls.Clear();
            pnlContainer.Controls.Add(child);
            pnlContainer.Tag = child;
            ApplyThemeToChildForm(child, isDarkMode);
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

            if (UserSession.IsAdmin)
            {
                // Admin tối cao: Full quyền toàn bộ các cấu phần hệ thống
                flowMenu.Controls.Add(CreateMenuButton("Quản lý tài khoản", Account_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý giáo vụ", Staff_Click));
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
                flowMenu.Controls.Add(CreateMenuButton("Trung Tâm Giám Sát Kết Nối AI", AIConnectivityMonitoringCenter_Click));
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
                flowMenu.Controls.Add(CreateMenuButton("Phân công giảng dạy", TeachingAssignment_Click));
                flowMenu.Controls.Add(CreateMenuButton("Quản lý điểm số", Score_Click));
                flowMenu.Controls.Add(CreateMenuButton("Xét duyệt yêu cầu SV", Request_Click));
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

        private void Dashboard_Click(object sender, EventArgs e)
        {
            // 1. Lấy chuỗi kết nối an toàn từ My_DB mà bạn đã khai báo ở đầu Main Form
            // (Giúp tái sử dụng kết nối, tránh hardcode chuỗi string)
            string connString = _db.GetConnection().ConnectionString;

            // 2. Định tuyến theo vai trò (Role-based Routing) và tiêm phụ thuộc (Dependency Injection)
            if (UserSession.IsStudent)
            {
                // Khởi tạo các nguyên liệu chuyên biệt cho Dashboard Sinh viên
                var dashboardRepo = new DashboardRepository(connString);
                var requestRepo = new RequestRepository(connString);
                var registerRepo = new RegisterRepository();

                // Mở Form StudentDashboardForm mới tinh đã dọn dẹp sạch layer
                OpenChildForm(new StudentDashboardForm(dashboardRepo, registerRepo, requestRepo));
            }
            else if (UserSession.IsAdmin || UserSession.IsStaff)
            {
                // Đối với Admin hoặc Giáo vụ, mở màn hình Dashboard tổng quan toàn trường
                var dashboardRepo = new DashboardRepository(connString);

                // Giả sử DashBoardForm tổng của bạn nhận vào dashboardRepo tổng quan:
                OpenChildForm(new DashBoardForm());
            }
            else if (UserSession.IsTeacher)
            {
                string maGV = UserSession.TeacherId;

                // Truyền mã này vào trong ngoặc khi khởi tạo Form con
                OpenChildForm(new TeacherDashBoardForm(maGV));
            }
        }
        private void Account_Click(object sender, EventArgs e) => OpenChildForm(new AccountManageForm());
        private void Staff_Click(object sender, EventArgs e) => OpenChildForm(new ManageStaffForm());
        private void Teacher_Click(object sender, EventArgs e) => OpenChildForm(new ManageTeacherForm());
        private void Student_Click(object sender, EventArgs e) => OpenChildForm(new ManageStudentForm());
        private void Major_Click(object sender, EventArgs e) => OpenChildForm(new ManageMajorForm());
        private void Classroom_Click(object sender, EventArgs e) => OpenChildForm(new ManageClassroomForm());
        private void Course_Click(object sender, EventArgs e) => OpenChildForm(new ManageCourseForm());
        private void CourseSection_Click(object sender, EventArgs e)
        {
            if (!UserSession.IsLoggedIn || (!UserSession.IsAdmin && !UserSession.IsStaff && !UserSession.IsTeacher))
            {
                MessageBox.Show("Bạn không có quyền truy cập vào chức năng quản lý lớp học phần!",
                                "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var frm = new ManageCourseSectionForm();
            frm.FormClosed += (s, args) => frm.Dispose();
            OpenChildForm(frm);
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
        private void Contact_Click(object sender, EventArgs e) => OpenChildForm(new ContactForm(new ContactRepository()));
        private void AIConnectivityMonitoringCenter_Click(object sender, EventArgs e) => OpenChildForm(new ConnectionMonitorForm());

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

        private void TeachingAssignment_Click(object sender, EventArgs e) => OpenChildForm(new TeachingAssignmentForm());
        // --- ĐĂNG XUẤT ---
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UserSession.Clear();

                // 🌟 THÊM DÒNG NÀY: Đánh dấu đây là hành động ĐĂNG XUẤT
                this.DialogResult = DialogResult.Retry;

                this.Close();
            }
        }
        private void Score1_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            ctxNotificationMenu.Items.Clear();
            ctxNotificationMenu.AutoSize = true;
            ctxNotificationMenu.RenderMode = ToolStripRenderMode.System;
            ctxNotificationMenu.BackColor = Color.White;

            var repo = new GetNotificationRepository();
            System.Data.DataTable dtNoti = repo.GetNotificationData();

            if (dtNoti == null || dtNoti.Rows.Count == 0)
            {
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("📭 Không có thông báo mới");
                emptyItem.Enabled = false;
                emptyItem.ForeColor = Color.DarkGray;
                emptyItem.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
                ctxNotificationMenu.Items.Add(emptyItem);
            }
            else
            {
                foreach (System.Data.DataRow row in dtNoti.Rows)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = row["Title"].ToString();
                    item.Tag = Convert.ToInt32(row["Id"]);
                    item.ForeColor = Color.FromArgb(31, 41, 55);
                    item.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    item.ImageScaling = ToolStripItemImageScaling.None;

                    item.Click += NotificationItem_Click;
                    ctxNotificationMenu.Items.Add(item);
                }

                // TÍNH NĂNG THỰC TẾ: Khi click mở chuông ra xem, ẩn dấu chấm đỏ đi ngay lập tức
                _hasNewNotifications = false;
                btnNotification.Invalidate(); // Vẽ lại nút để xóa chấm đỏ
            }

            ctxNotificationMenu.PerformLayout();
            int xOffset = btnNotification.Width - ctxNotificationMenu.PreferredSize.Width;
            ctxNotificationMenu.Show(btnNotification, new Point(xOffset, btnNotification.Height + 4));
        }

        // Sự kiện xử lý khi người dùng CLICK VÀO MỘT DÒNG thông báo cụ thể
        private void NotificationItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem?.Tag == null) return;

            int requestId = (int)clickedItem.Tag;

            // Nếu là thông báo hệ thống mặc định (Id = 0) thì dừng xử lý
            if (requestId == 0) return;

            // ĐIỀU HƯỚNG CHUẨN THỰC TẾ DOANH NGHIỆP
            if (UserSession.RoleId == 3) // VAI TRÒ: GIÁO VỤ
            {
                // Sử dụng hàm OpenChildForm có sẵn trong f_main của bạn để nạp Form quản lý duyệt
                AdminApproveRequestForm frm = new AdminApproveRequestForm();
                OpenChildForm(frm);

                // Mẹo nâng cao: Nếu form AdminApproveRequestForm của bạn có viết hàm hỗ trợ tự động tìm kiếm/lọc
                // ví dụ: frm.LocTheoMaYeuCau(requestId); thì bạn có thể gọi ở đây để focus thẳng vào hàng đó!
            }
            else if (UserSession.RoleId == 1) // VAI TRÒ: SINH VIÊN
            {
                // Tự động mở phân hệ Yêu cầu hỗ trợ của sinh viên để xem chi tiết phản hồi của Giáo vụ
                StudentRequestForm frm = new StudentRequestForm();
                OpenChildForm(frm);

                // Tương tự, nếu form StudentRequestForm có viết hàm focus dòng: 
                // frm.FocusToRequest(requestId); hãy gọi ở đây.
            }
        }
        private void CheckNewNotificationStatus()
        {
            var repo = new GetNotificationRepository();
            DataTable dtNoti = repo.GetNotificationData();

            // Nếu có hàng dữ liệu và nội dung không phải là chuỗi trống/mặc định bảo mật
            if (dtNoti != null && dtNoti.Rows.Count > 0 && Convert.ToInt32(dtNoti.Rows[0]["Id"]) != 0)
            {
                _hasNewNotifications = true;
            }
            else
            {
                _hasNewNotifications = false;
            }
            btnNotification.Invalidate(); // Ép nút vẽ lại giao diện để hiển thị hoặc ẩn chấm đỏ
        }
        private void BtnNotification_Paint(object sender, PaintEventArgs e)
        {
            if (_hasNewNotifications)
            {
                int circleSize = 8; // Độ rộng dấu chấm đỏ (pixel)
                                    // Đặt vị trí chấm đỏ ở góc trên cùng bên phải, cách lề một chút
                int x = btnNotification.Width - circleSize - 6;
                int y = 6;

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.Red))
                {
                    e.Graphics.FillEllipse(brush, x, y, circleSize, circleSize);
                }
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            ContextMenuStrip ctxSettingMenu = new ContextMenuStrip();
            ctxSettingMenu.ImageScalingSize = new Size(20, 20);
            ctxSettingMenu.BackColor = isDarkMode ? Color.FromArgb(30, 41, 59) : Color.White;
            ctxSettingMenu.ForeColor = isDarkMode ? Color.White : Color.Black;
            ctxSettingMenu.RenderMode = ToolStripRenderMode.System;

            // 1. Mục Đổi mật khẩu
            ToolStripMenuItem itemChangePassword = new ToolStripMenuItem("🔑 Đổi mật khẩu cá nhân");
            itemChangePassword.Click += (s, ev) =>
            {
                ChangePasswordForm frmChangePass = new ChangePasswordForm();
                ApplyThemeToChildForm(frmChangePass, isDarkMode);

                if (frmChangePass.ShowDialog() == DialogResult.OK)
                {
                    this.Hide();
                    LoginForm frmLogin = new LoginForm();
                    frmLogin.FormClosed += (senderForm, args) =>
                    {
                        this.Dispose();
                    };
                    this.Close();
                }
            };

            // 2. THÊM MỚI: Mục Đăng ký Face ID (Sinh trắc học)
            ToolStripMenuItem itemRegisterFace = new ToolStripMenuItem("📸 Thiết lập Face ID (Quét mặt)");
            itemRegisterFace.Click += (s, ev) =>
            {
                using (FaceLoginForm frmRegisterFace = new FaceLoginForm(UserSession.Username, true))
                {
                    ApplyThemeToChildForm(frmRegisterFace, isDarkMode);

                    frmRegisterFace.ShowDialog();
                }
            };

            // 3. Mục Dark Mode
            ToolStripMenuItem itemDarkMode = new ToolStripMenuItem("🌙 Chế độ nền tối (Dark Mode)");
            itemDarkMode.CheckOnClick = true;
            itemDarkMode.Checked = isDarkMode;
            itemDarkMode.Click += (s, ev) =>
            {
                isDarkMode = itemDarkMode.Checked;
                ApplyTheme(isDarkMode);
            };

            // 4. Mục Thông tin phần mềm
            ToolStripMenuItem itemAbout = new ToolStripMenuItem("ℹ️ Thông tin phần mềm");
            itemAbout.Click += (s, ev) =>
            {
                MessageBox.Show("Hệ thống Quản lý Đào tạo - Phiên bản 1.0.0\n© 2026 ClassProject Team.", "About");
            };

            // --- TIẾN HÀNH NẠP CÁC MỤC VÀO MENU THẢ XUỐNG ---
            ctxSettingMenu.Items.Add(itemChangePassword);
            ctxSettingMenu.Items.Add(itemRegisterFace); // Đẩy nút Đăng ký Face ID vào đây
            ctxSettingMenu.Items.Add(new ToolStripSeparator()); // Đường gạch ngang phân cách
            ctxSettingMenu.Items.Add(itemDarkMode);
            ctxSettingMenu.Items.Add(itemAbout);

            // Tính toán vị trí hiển thị menu
            int xOffset = btnSetting.Width - ctxSettingMenu.PreferredSize.Width;
            ctxSettingMenu.Show(btnSetting, new Point(xOffset, btnSetting.Height + 4));
        }

        private void ApplyTheme(bool darkMode)
        {
            // ==========================================
            // 1. ĐỊNH NGHĨA PALETTE MÀU (CHUẨN UI/UX)
            // ==========================================
            // Chế độ Sáng (Màu gốc hiện tại của bạn)
            Color lightBgColor = Color.FromArgb(243, 244, 246);
            Color lightPanelColor = Color.White;
            Color lightTextMain = Color.FromArgb(31, 41, 55);
            Color lightTextSub = Color.FromArgb(107, 114, 128);

            // Chế độ Tối (Xanh đen Slate cao cấp, dịu mắt)
            Color darkBgColor = Color.FromArgb(15, 23, 42);       // Nền form chính
            Color darkPanelColor = Color.FromArgb(30, 41, 59);   // Nền của panel Header/Nav/Status
            Color darkTextMain = Color.FromArgb(241, 245, 249);  // Chữ trắng sáng
            Color darkTextSub = Color.FromArgb(148, 163, 184);   // Chữ xám nhạt cho Sub-title

            // ==========================================
            // 2. ÁP DỤNG CHO MAIN FORM (f_main)
            // ==========================================
            this.BackColor = darkMode ? darkBgColor : lightBgColor;

            // --- Header Panel (guna2Panel1) ---
            guna2Panel1.FillColor = darkMode ? darkPanelColor : lightPanelColor;
            guna2Panel1.BackColor = darkMode ? darkPanelColor : lightPanelColor;
            lblTitle.ForeColor = darkMode ? Color.FromArgb(56, 189, 248) : Color.FromArgb(0, 120, 212); // Đổi xanh dương sang Cyan khi ở nền tối
            lblUserName.ForeColor = darkMode ? darkTextMain : lightTextMain;
            lblRole.ForeColor = darkMode ? darkTextSub : lightTextSub;

            // Đổi màu nền nút bấm trên Header để không bị lộ viền trắng
            btnSetting.FillColor = darkMode ? Color.FromArgb(51, 65, 85) : Color.FromArgb(243, 244, 246);
            btnSetting.ForeColor = darkMode ? darkTextMain : Color.FromArgb(75, 85, 99);
            btnNotification.FillColor = darkMode ? Color.FromArgb(51, 65, 85) : Color.FromArgb(243, 244, 246);
            btnNotification.ForeColor = darkMode ? darkTextMain : Color.FromArgb(75, 85, 99);

            // --- Navigation Panel (pnlNavigation & flowMenu) ---
            pnlNavigation.FillColor = darkMode ? darkPanelColor : lightPanelColor;
            flowMenu.BackColor = darkMode ? darkPanelColor : lightPanelColor;
            btnScrollLeft.BackColor = darkMode ? darkPanelColor : lightPanelColor;
            btnScrollRight.BackColor = darkMode ? darkPanelColor : lightPanelColor;

            // Đổi màu các nút chức năng trong thanh cuộn điều hướng (btn1, btn2, btn3, btn4...)
            foreach (Control ctrl in flowMenu.Controls)
            {
                if (ctrl is Guna.UI2.WinForms.Guna2Button btn)
                {
                    btn.ForeColor = darkMode ? darkTextSub : Color.FromArgb(75, 85, 99);
                    btn.HoverState.FillColor = darkMode ? Color.FromArgb(51, 65, 85) : Color.FromArgb(237, 245, 255);
                }
            }

            // --- Status Bar (pnlStatusBar) ---
            pnlStatusBar.FillColor = darkMode ? darkPanelColor : Color.FromArgb(243, 244, 246);
            pnlStatusBar.BackColor = darkMode ? darkPanelColor : Color.FromArgb(243, 244, 246);
            lblVersion.ForeColor = darkMode ? darkTextSub : lightTextSub;
            lblDateTime.ForeColor = darkMode ? darkTextSub : lightTextSub;
            lblReady.ForeColor = darkMode ? Color.FromArgb(74, 222, 128) : Color.FromArgb(16, 124, 65); // Xanh lá sáng hơn ở nền tối

            // --- Container Panel chứa Child Form (pnlContainer) ---
            // Giữ nguyên FillColor là màu nền chính để làm móng cho Form con đè lên
            pnlContainer.FillColor = darkMode ? darkBgColor : Color.White;

            // ==========================================
            // 3. ĐỔI MÀU CHO CÁC CHILD FORM ĐANG MỞ
            // ==========================================
            foreach (Control ctrl in pnlContainer.Controls)
            {
                if (ctrl is Form childForm)
                {
                    ApplyThemeToChildForm(childForm, darkMode);
                }
            }
        }

        private void ApplyThemeToChildForm(Form f, bool darkMode)
        {
            Color darkBgColor = Color.FromArgb(15, 23, 42);
            Color lightBgColor = Color.White;
            Color darkTextMain = Color.FromArgb(241, 245, 249);
            Color lightTextMain = Color.FromArgb(31, 41, 55);

            f.BackColor = darkMode ? darkBgColor : lightBgColor;

            // Sử dụng một hàm đệ quy nhỏ để nhuộm màu tất cả các cấp control bên trong
            NhuomMauControl(f, darkMode, darkBgColor, lightBgColor, darkTextMain, lightTextMain);

            f.Refresh();
        }

        // Hàm đệ quy duyệt toàn bộ cây Control (kể cả control nằm sâu trong Panel/TabControl)
        private void NhuomMauControl(Control parent, bool darkMode, Color darkBg, Color lightBg, Color darkText, Color lightText)
        {
            foreach (Control c in parent.Controls)
            {
                // 1. Xử lý các nhãn, checkbox, radio cơ bản
                if (c is Label || c is CheckBox || c is RadioButton || c is GroupBox)
                {
                    c.ForeColor = darkMode ? darkText : lightText;
                }

                // 2. Xử lý Guna2HtmlLabel hoặc Guna2Label bằng ép kiểu chuỗi
                if (c.GetType().Name.Contains("Guna2HtmlLabel") || c.GetType().Name.Contains("Guna2Label"))
                {
                    c.ForeColor = darkMode ? darkText : lightText;
                }

                // 3. Xử lý Guna2Panel, Guna2GroupBox (Cần đổi thuộc tính FillColor thay vì BackColor)
                if (c is Guna.UI2.WinForms.Guna2Panel g2p)
                {
                    g2p.FillColor = darkMode ? darkBg : lightBg;
                }
                if (c is Guna.UI2.WinForms.Guna2GroupBox g2gb)
                {
                    g2gb.FillColor = darkMode ? Color.FromArgb(30, 41, 59) : lightBg; // Hộp nhóm tối hơn chút
                    g2gb.CustomBorderColor = darkMode ? Color.FromArgb(51, 65, 85) : Color.FromArgb(213, 218, 223);
                    g2gb.ForeColor = darkMode ? darkText : lightText;
                }

                // 4. Xử lý các ô nhập liệu Guna2TextBox, Guna2ComboBox
                if (c is Guna.UI2.WinForms.Guna2TextBox g2txt)
                {
                    g2txt.FillColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
                    g2txt.ForeColor = darkMode ? darkText : lightText;
                    g2txt.BorderColor = darkMode ? Color.FromArgb(71, 85, 105) : Color.FromArgb(213, 218, 223);
                }
                if (c is Guna.UI2.WinForms.Guna2ComboBox g2cb)
                {
                    g2cb.FillColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
                    g2cb.ForeColor = darkMode ? darkText : lightText;
                    g2cb.BorderColor = darkMode ? Color.FromArgb(71, 85, 105) : Color.FromArgb(213, 218, 223);
                }

                // 5. Xử lý lưới dữ liệu Guna2DataGridView hoặc DataGridView tiêu chuẩn
                if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
                    dgv.GridColor = darkMode ? Color.FromArgb(51, 65, 85) : Color.FromArgb(226, 232, 240);
                    dgv.DefaultCellStyle.BackColor = darkMode ? Color.FromArgb(30, 41, 59) : Color.White;
                    dgv.DefaultCellStyle.ForeColor = darkMode ? darkText : lightText;
                }

                // ĐỆ QUY: Nếu control này chứa các control con khác (như Panel, TabPage, FlowLayoutPanel)
                if (c.Controls.Count > 0)
                {
                    NhuomMauControl(c, darkMode, darkBg, lightBg, darkText, lightText);
                }
            }
        }
        // THÀNH PHẦN AI AUTOLOGOUT: ĐÁNH CHẶN TƯƠNG TÁC ĐỂ GIA HẠN PHIÊN
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            UserSession.RefreshActivity();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UserSession.RefreshActivity();
        }

        // --- ĐIỀU KHIỂN WIDGET CHAT ---
        private void btnToggleAiChat_Click(object sender, EventArgs e)
        {
            bool isVisible = !pnlAiChatBox.Visible;
            pnlAiChatBox.Visible = isVisible;
            lblAiStatus.Visible = isVisible;

            if (isVisible)
            {
                txtAiSearch.Focus();
                pnlAiChatBox.BringToFront();
                btnToggleAiChat.BringToFront();
                lblAiStatus.BringToFront();
            }
        }

        private void btnCloseAiChat_Click(object sender, EventArgs e)
        {
            pnlAiChatBox.Visible = false;
        }

        // --- XỬ LÝ LỒNG GHÉP TIN NHẮN VÀ GỌI API (SỬA LỖI ĐƠ LUỒNG ĐIỀU HƯỚNG) --
        private async void txtAiSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Chặn tiếng bíp Windows

                string userQuery = txtAiSearch.Text.Trim();
                if (string.IsNullOrEmpty(userQuery)) return;

                // 1. Hiển thị câu hỏi của người dùng lên màn hình Chat
                AddMessageToHistory("You", userQuery);
                txtAiSearch.Clear(); // Xóa trống ô nhập liệu ngay cho mượt

                lblAiStatus.Text = "🤖 Hệ thống đang xử lý...";
                txtAiSearch.Enabled = false;

                try
                {
                    // === TẦNG 1: CHẠY HOÀN TOÀN LOCAL (TỐC ĐỘ < 10ms) ===
                    string localStaticResponse = _aiService.CheckLocalStaticResponse(userQuery);
                    if (!string.IsNullOrEmpty(localStaticResponse))
                    {
                        AddMessageToHistory("AI (Local)", localStaticResponse);
                        return;
                    }

                    var navIntent = _aiService.AnalyzeNavigationIntent(userQuery);
                    if (!string.IsNullOrEmpty(navIntent.FormName))
                    {
                        AddMessageToHistory("AI (Local)", navIntent.ResponseMessage);
                        OpenFormByName(navIntent.FormName);
                        return;
                    }

                    // === TẦNG 2: GỬI LÊN CLOUD AI GEMINI (GỌI ĐÚNG 1 LẦN) ===
                    lblAiStatus.Text = "🤖 Đang kết nối trí tuệ nhân tạo Cloud...";
                    string aiResponse = await _aiService.FetchAiResponseAsync(userQuery, isForgetScreen: false, isSystemInternalCall: false);

                    // Tình huống A: AI phân tích thấy cần truy vấn dữ liệu
                    if (aiResponse.Contains("EXECUTE_SQL:"))
                    {
                        int index = aiResponse.IndexOf("EXECUTE_SQL:");
                        string sqlQuery = aiResponse.Substring(index + "EXECUTE_SQL:".Length).Trim();

                        // Làm sạch câu lệnh phòng trường hợp AI bọc ký tự đặc biệt
                        sqlQuery = sqlQuery.Replace("```sql", "").Replace("```", "").Trim();

                        // Hiển thị câu lệnh ẩn hoặc hiện tùy bạn (để debug)
                        System.Diagnostics.Debug.WriteLine($"[AI_SQL_EXECUTE]: {sqlQuery}");

                        // GỌI THẲNG QUA REPOSITORY: Lấy dữ liệu thật từ kết nối chuẩn
                        string dataResultText = _studentRepo.ExecuteAiQueryToJson(sqlQuery);

                        // === XỬ LÝ KẾT QUẢ THÀNH CHỮ ĐẸP KHÔNG TỐN TOKEN GỌI AI LẦN 2 ===
                        if (dataResultText.StartsWith("[Lỗi"))
                        {
                            AddMessageToHistory("Hệ thống", $"❌ Không thể truy xuất dữ liệu: {dataResultText}");
                        }
                        else if (dataResultText == "[]")
                        {
                            AddMessageToHistory("AI", "📊 Không tìm thấy dữ liệu phù hợp trong hệ thống.");
                        }
                        else
                        {
                            // Chuyển đổi JSON thành chuỗi danh sách dễ đọc bằng hàm phụ bên dưới
                            string cleanReadableText = ConvertJsonToReadableText(dataResultText);
                            AddMessageToHistory("AI", cleanReadableText);
                        }
                    }
                    // Tình huống B: AI tự trò chuyện hoặc xử lý các câu hỏi mở rộng khác
                    else
                    {
                        AddMessageToHistory("AI", aiResponse);
                    }
                }
                catch (Exception ex)
                {
                    AddMessageToHistory("Hệ thống", $"⚠️ Sự cố: {ex.Message}");
                }
                finally
                {
                    txtAiSearch.Enabled = true;
                    txtAiSearch.Focus();
                    lblAiStatus.Text = "Hệ thống sẵn sàng.";
                }
            }
        }
        private string ConvertJsonToReadableText(string jsonString)
        {
            try
            {
                // Phân tích chuỗi JSON thành một mảng các đối tượng
                var jsonArray = Newtonsoft.Json.Linq.JArray.Parse(jsonString);
                System.Text.StringBuilder readableText = new System.Text.StringBuilder();

                readableText.AppendLine("📊 Kết quả tìm kiếm thực tế từ hệ thống:");

                int rowCount = 1;
                foreach (var item in jsonArray)
                {
                    // Tạo một dòng hiển thị thông tin dạng: 1. CộtA: GiáTrịA | CộtB: GiáTrịB
                    List<string> properties = new List<string>();
                    foreach (var prop in ((Newtonsoft.Json.Linq.JObject)item).Properties())
                    {
                        properties.Add($"{prop.Name}: {prop.Value}");
                    }

                    readableText.AppendLine($"{rowCount}. {string.Join(" | ", properties)}");
                    rowCount++;
                }

                return readableText.ToString();
            }
            catch
            {
                // Phòng hờ nếu có lỗi phân tích cú pháp thì trả về chuỗi JSON gốc
                return jsonString;
            }
        }

        // --- HÀM ĐIỀU HƯỚNG FORM CON TỰ ĐỘNG CHUẨN ĐÃ LOẠI BỎ LỖI LỆCH CHỮ ---
        private void OpenFormByName(string formName)
        {
            if (string.IsNullOrEmpty(formName)) return;

            Form targetForm = null;
            string safeFormName = formName.Trim().ToLower();

            switch (safeFormName)
            {
                case "managescoreform": targetForm = new ManageScoreForm(); break;
                case "managestudentform": targetForm = new ManageStudentForm(); break;
                case "managecourseform": targetForm = new ManageCourseForm(); break;
                case "manageclassroomform": targetForm = new ManageClassroomForm(); break;
                case "accountmanageform": targetForm = new AccountManageForm(); break;

                case "statisticsform":
                    string role = UserSession.IsAdmin ? "Admin" : (UserSession.IsStaff ? "HR" : "Student");
                    targetForm = new StatisticsForm(role, 0);
                    break;

                case "transcriptform": targetForm = new TranscriptForm(); break;
                case "profileform": targetForm = new ProfileForm(); break;
                case "studentrequestform": targetForm = new StudentRequestForm(); break;

                default:
                    System.Diagnostics.Debug.WriteLine($"[AI Warning] Không tìm thấy case map cho form: {formName}");
                    break;
            }

            if (targetForm != null)
            {
                // 1. Gọi hàm nạp form con lồng vào Panel Container của bạn
                OpenChildForm(targetForm);

                // CỐ ĐỊNH CHÍ PHÁP: ẨN KHUNG CHAT ĐỂ TRÁNH CHE KHUẤT GIAO DIỆN FORM MỚI
                pnlAiChatBox.Visible = false;
                lblAiStatus.Visible = false; // Nếu bạn dùng nhãn trạng thái riêng ngoài panel

                // Trả lại trạng thái chữ mặc định để lần sau mở chat lên trông gọn gàng
                lblAiStatus.Text = "Hệ thống sẵn sàng.";
                lblAiStatus.ForeColor = Color.DarkSlateBlue;
            }
        }

        // --- THIẾT KẾ BONG BÓNG CHAT DOANH NGHIỆP HIỂN THỊ TRONG FLOWLAYOUTPANEL ---
        private void AddMessageToHistory(string sender, string message)
        {
            Label lblBubble = new Label();
            lblBubble.Text = message; // Bỏ chữ tiền tố lặp lại để UI trông giống Telegram/Messenger hơn
            lblBubble.AutoSize = true;
            lblBubble.MaximumSize = new Size(flowChatHistory.Width - 35, 0); // Ép text tự động xuống dòng mượt mà
            lblBubble.Padding = new Padding(10, 8, 10, 8);
            lblBubble.Font = new Font("Segoe UI", 9.5F);

            // Cấu hình lề biên: Tránh dính các cục bong bóng lại với nhau
            lblBubble.Margin = new Padding(3, 5, 3, 5);

            // Phân biệt kiểu dáng thiết kế chuẩn UI/UX doanh nghiệp
            if (sender == "You")
            {
                // Người dùng: Nằm sát bên phải (hoặc căn lề phù hợp), nền xanh nhạt, chữ xanh dương đậm
                lblBubble.BackColor = Color.FromArgb(231, 243, 255);
                lblBubble.ForeColor = Color.FromArgb(0, 120, 212);
                // Tạo hiệu ứng bo góc nhẹ qua FlatStyle nếu cần, hoặc giữ nguyên kiểu chữ tinh tế
            }
            else if (sender == "Hệ thống")
            {
                // Cảnh báo lỗi hệ thống: Màu đỏ cảnh báo dịu
                lblBubble.BackColor = Color.FromArgb(254, 242, 242);
                lblBubble.ForeColor = Color.FromArgb(220, 38, 38);
            }
            else
            {
                // Phản hồi từ AI: Nền xám nhạt, chữ xám đen cực kỳ dịu mắt
                lblBubble.BackColor = Color.FromArgb(243, 244, 246);
                lblBubble.ForeColor = Color.FromArgb(31, 41, 55);
            }

            // Đẩy bong bóng vào khung cuộn lịch sử tin nhắn
            flowChatHistory.Controls.Add(lblBubble);

            // MẸO UX: Tự động cuộn thanh Scroll xuống đáy để luôn nhìn thấy câu trả lời mới nhất
            flowChatHistory.ScrollControlIntoView(lblBubble);
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            TestDatabaseConnection();
        }
        public void TestDatabaseConnection()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            report.AppendLine("=== KẾT QUẢ KIỂM TRA TOÀN DIỆN DATABASE THEO SCRIPT THỰC TẾ ===");
            report.AppendLine();

            // Sửa lại danh sách bảng CHÍNH XÁC theo file SQL của bạn (Không có chữ s ở cuối bảng điểm và môn học)
            string[] tables = { "dbo.Students", "dbo.Score", "dbo.Course", "dbo.CourseSection", "dbo.Classroom" };

            foreach (string tableName in tables)
            {
                report.AppendLine($"--------------------------------------------------");
                report.AppendLine($"📍 BẢNG THỰC TẾ: {tableName}");

                // 1. Lấy thông tin cột
                string schemaQuery = $@"
            SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = '{tableName.Replace("dbo.", "")}'";

                string schemaResult = _studentRepo.ExecuteAiQueryToJson(schemaQuery);
                report.AppendLine("1. Cấu trúc các cột (Schema):");
                report.AppendLine(schemaResult);
                report.AppendLine();

                // 2. Lấy dữ liệu mẫu
                string dataQuery = $"SELECT TOP 2 * FROM {tableName}";
                string dataResult = _studentRepo.ExecuteAiQueryToJson(dataQuery);
                report.AppendLine("2. Dữ liệu mẫu (Top 2 rows):");
                report.AppendLine(dataResult);
                report.AppendLine();
            }

            // ============================================================================
            // TỰ ĐỘNG SINH LUÔN PROMPT CHUẨN CHO AI (BẠN CHỈ CẦN COPY NÉM VÀO SYSTEM PROMPT)
            // ============================================================================
            report.AppendLine("==================================================");
            report.AppendLine("🤖 CHUỖI SYSTEM PROMPT CHUẨN ĐỂ DẠY AI CHAT:");
            report.AppendLine("Bạn là trợ lý AI phân tích dữ liệu học vụ của trường HCMUTE, cơ sở dữ liệu LoginDB.");
            report.AppendLine("Dưới đây là cấu trúc các bảng bắt buộc phải viết đúng tên bảng và tên cột khi sinh câu lệnh SQL:");
            report.AppendLine("- Bảng sinh viên: dbo.Students (Id, UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Email, MaLop, MaNganh)");
            report.AppendLine("- Bảng điểm HP: dbo.Score (MSSV, MaLopHP, DiemQT, DiemCK, DiemTK, Mota)");
            report.AppendLine("- Bảng môn học: dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc)");
            report.AppendLine("- Bảng lớp học phần: dbo.CourseSection (MaLopHP, MaMH, HocKy, NamHoc, MSGV, PhongHoc, MaxStudents, ThuHoc, CaHoc)");
            report.AppendLine("- Bảng lớp hành chính: dbo.Classroom (MaLop, TenLop, SiSo, GVCN, MaNganh)");
            report.AppendLine("Ngoài ra bạn có thể sử dụng các View có sẵn sau nếu cần truy vấn nhanh:");
            report.AppendLine("* View bảng điểm đầy đủ: dbo.vw_StudentTranscript (MSSV, StudentName, MaLopHP, MaMH, TenMH, SoTC, DiemQT, DiemCK, DiemTK, NamHoc, HocKy)");
            report.AppendLine("* View lịch học hằng ngày: dbo.vw_StudentDailySchedule (MSSV, MaLopHP, TenMH, PhongHoc, ThuHoc, CaHoc, ThoiGian)");
            report.AppendLine("\nQUY ĐỊNH TRẢ LỜI: Nếu người dùng yêu cầu thống kê/tra cứu, chỉ trả về dạng: EXECUTE_SQL:[Lệnh_SQL_Server]. Không bọc ký tự đặc biệt.");

            string finalReport = report.ToString();

            // In ra cửa sổ Output để bạn Ctrl + A -> Ctrl + C copy cho tiện
            System.Diagnostics.Debug.WriteLine(finalReport);

            // Hiển thị hộp thoại lớn trực tiếp trên giao diện phần mềm
            Form reportForm = new Form
            {
                Text = "Cấu trúc dữ liệu thực tế phục vụ AI",
                Width = 750,
                Height = 600,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox txtReport = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Text = finalReport,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10)
            };
            reportForm.Controls.Add(txtReport);
            reportForm.ShowDialog();
        }
    }
}