using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Presentation.Forms.Admin;
using ClassProject.Presentation.Forms.Course;
using ClassProject.Presentation.Forms.Students;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Runtime.InteropServices; // Để dùng API ẩn scrollbar
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class f_main : Form
    {
        private readonly int roleId;
        private readonly int userId;
        private string loggedInMSSV = "";
        private Form currentForm;

        private readonly My_DB db = new My_DB();
        private StudentRepository studentRepo;

        // 🌟 API Windows ẩn thanh cuộn nhưng giữ tính năng cuộn bằng code
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_HORZ = 0; // Thanh cuộn ngang

        public f_main(int roleId, int userId)
        {
            InitializeComponent();

            this.roleId = roleId;
            this.userId = userId;

            string connString = db.GetConnection().ConnectionString;
            studentRepo = new StudentRepository(connString);

            GetStudentMSSV();
        }

        private void f_main_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // Nền tổng xám nhạt hiện đại
            this.BackColor = Color.FromArgb(246, 248, 251);

            LoadRoleInfo();
            LoadMenu();

            // Gán sự kiện click cho 2 nút điều hướng
            btnScrollLeft.Click += BtnScrollLeft_Click;
            btnScrollRight.Click += BtnScrollRight_Click;

            timerClock.Start();
        }

        // 🌟 Hàm thực hiện ẩn thanh cuộn ngang thô kệch
        private void HideFlowMenuScrollbar()
        {
            flowMenu.AutoScroll = true;
            flowMenu.PerformLayout(); // Ép layout tính toán lại trước khi ẩn
            ShowScrollBar(flowMenu.Handle, SB_HORZ, false);
        }

        // 🌟 Hàm cốt lõi: Tự động ẩn/hiện nút Trái/Phải dựa trên vị trí cuộn thực tế
        private void UpdateButtonVisibility()
        {
            // 1. Kiểm tra vị trí bên trái (Nếu vị trí cuộn <= tối thiểu -> Ẩn nút Trái)
            if (flowMenu.HorizontalScroll.Value <= flowMenu.HorizontalScroll.Minimum)
            {
                btnScrollLeft.Visible = false;
            }
            else
            {
                btnScrollLeft.Visible = true;
            }

            // 2. Tính toán xem tổng độ dài các menu có tràn ra ngoài vùng hiển thị của flowMenu hay không
            int totalWidth = 0;
            foreach (Control ctrl in flowMenu.Controls)
            {
                if (ctrl.Visible)
                {
                    totalWidth += ctrl.Width + ctrl.Margin.Left + ctrl.Margin.Right;
                }
            }

            // 3. Kiểm tra vị trí bên phải
            // Lưu ý: Thêm sai số 5-10 pixel chống đứng hình trên một số độ phân giải màn hình
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

            // Thay đổi AutoScrollPosition bằng Point (X phải là số dương, WinForms tự chuyển thành âm nội bộ)
            flowMenu.AutoScrollPosition = new Point(newScrollPosition, 0);

            // Ép hệ thống ẩn lại thanh cuộn và cập nhật nút
            HideFlowMenuScrollbar();
            UpdateButtonVisibility();
        }

        private void BtnScrollRight_Click(object sender, EventArgs e)
        {
            int newScrollPosition = flowMenu.HorizontalScroll.Value + 150;

            if (newScrollPosition > flowMenu.HorizontalScroll.Maximum)
                newScrollPosition = flowMenu.HorizontalScroll.Maximum;

            // Thay đổi AutoScrollPosition bằng Point để tránh lỗi WinForms nuốt lệnh cuộn
            flowMenu.AutoScrollPosition = new Point(newScrollPosition, 0);

            // Ép hệ thống ẩn lại thanh cuộn và cập nhật nút
            HideFlowMenuScrollbar();
            UpdateButtonVisibility();
        }

        #endregion

        #region UserInfo

        private void LoadRoleInfo()
        {
            switch (roleId)
            {
                case 0: lblRole.Text = "Administrator"; break;
                case 1: lblRole.Text = "Student"; break;
                case 2: lblRole.Text = "HR"; break;
            }
        }

        private void GetStudentMSSV()
        {
            if (roleId != 1) return;
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    string query = @"
                    SELECT s.MSSV FROM Students s
                    INNER JOIN Users u ON s.UserId = u.Id
                    WHERE u.Id = @UserId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        loggedInMSSV = result.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region ChildForm

        private void OpenChildForm(Form child)
        {
            if (currentForm != null)
                currentForm.Close();

            currentForm = child;
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

        #region Modern Guna Menu

        private Guna2Button CreateMenuButton(string text, EventHandler click)
        {
            var btn = new Guna2Button();
            btn.Text = text;

            btn.AutoSize = true;
            btn.Height = 48;
            btn.MinimumSize = new Size(0, 48);
            btn.MaximumSize = new Size(0, 48);
            btn.Padding = new Padding(18, 0, 18, 0);
            btn.TextAlign = HorizontalAlignment.Center;
            btn.Margin = new Padding(2, 0, 2, 0);

            // MÀU NỀN TRẮNG mặc định
            btn.FillColor = Color.FromArgb(255, 255, 255);
            btn.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btn.ForeColor = Color.FromArgb(100, 116, 139);

            btn.CustomBorderThickness = new Padding(0, 0, 0, 3);
            btn.CustomBorderColor = Color.Transparent;

            btn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;

            // Trạng thái được chọn (Checked)
            btn.CheckedState.ForeColor = Color.FromArgb(37, 99, 235);
            btn.CheckedState.FillColor = Color.FromArgb(239, 246, 255);
            btn.CheckedState.CustomBorderColor = Color.FromArgb(37, 99, 235);

            // Trạng thái Hover
            btn.HoverState.FillColor = Color.FromArgb(241, 245, 249);
            btn.HoverState.ForeColor = Color.FromArgb(15, 23, 42);

            btn.BorderRadius = 4;
            btn.Cursor = Cursors.Hand;
            btn.Click += click;

            // Mỗi lần kích thước thay đổi, đảm bảo giấu thanh cuộn đi
            btn.SizeChanged += (s, e) => HideFlowMenuScrollbar();

            return btn;
        }

        private void LoadMenu()
        {
            flowMenu.Controls.Clear();

            // Màn hình trang chủ tổng quan (Mọi chức vụ đều tiếp cận khi đăng nhập thành công)
            Guna2Button btnDashboard = CreateMenuButton("Dashboard", Dashboard_Click);
            flowMenu.Controls.Add(btnDashboard);

            btnDashboard.Checked = true;
            Dashboard_Click(btnDashboard, EventArgs.Empty);

            switch (roleId)
            {
                // =========================================================
                // CASE 0: ADMIN TỐI CAO (POSITION = 0) - TOÀN QUYỀN HỆ THỐNG
                // =========================================================
                case 0:
                    // --- Quản trị tài khoản & Thực thể ---
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý tài khoản", Account_Click));     // UC-04
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý giảng viên", Contact_Click));    // UC-12 / Danh bạ
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý sinh viên", Student_Click));      // UC-05, 06, 07

                    // --- Quản lý đào tạo ---
                    flowMenu.Controls.Add(CreateMenuButton("Danh mục môn học", Course_Click));       // UC-10, 11, 12
                    flowMenu.Controls.Add(CreateMenuButton("Danh mục lớp học", Classroom_Click));    // Quản lý lớp
                    //flowMenu.Controls.Add(CreateMenuButton("Đăng ký môn học", RegisterCourse_Click));// UC-13 (Admin cấu hình hộ)

                    // --- Nghiệp vụ vận hành ---
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý điểm số", Score_Click));         // UC-14, 15
                    flowMenu.Controls.Add(CreateMenuButton("Phê duyệt yêu cầu", Request_Click));    // Duyệt đơn từ / Tài khoản mới

                    // --- Trung tâm dữ liệu & Kết xuất ---
                    flowMenu.Controls.Add(CreateMenuButton("Thống kê", Statistic_Click));            // UC-09, 17
                    flowMenu.Controls.Add(CreateMenuButton("Báo cáo", Report_Click));               // UC-20 (Xuất PDF/Excel/Word)
                    break;

                // =========================================================
                // CASE 1: STUDENT PORTAL (POSITION = 1) - CỔNG THÔNG TIN SINH VIÊN
                // =========================================================
                case 1:
                    //flowMenu.Controls.Add(CreateMenuButton("Thông tin cá nhân", Profile_Click));    // UC-21 (Hồ sơ cá nhân)
                    //flowMenu.Controls.Add(CreateMenuButton("Môn học của tôi", MyCourse_Click));     // Xem danh sách môn đang học
                    //flowMenu.Controls.Add(CreateMenuButton("Điểm của tôi", MyScore_Click));         // UC-16 (Xem điểm cá nhân)
                    flowMenu.Controls.Add(CreateMenuButton("Yêu cầu", Request_Click));               // Gửi đơn hỗ trợ, phúc khảo điểm
                    break;

                // =========================================================
                // CASE 2: HR / PHÒNG ĐÀO TẠO (POSITION = 2) - QUẢN LÝ NGHIỆP VỤ CA BIÊN
                // =========================================================
                case 2:
                    // HR quản lý dữ liệu đối tượng và lớp học theo phân quyền được giao
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý sinh viên", Student_Click));      // UC-05, 06, 08
                    flowMenu.Controls.Add(CreateMenuButton("Danh mục lớp học", Classroom_Click));
                    flowMenu.Controls.Add(CreateMenuButton("Danh mục môn học", Course_Click));
                    flowMenu.Controls.Add(CreateMenuButton("Quản lý điểm số", ScoreSv_Click));         // UC-14 (HR nhập điểm từ hội đồng thi)
                    flowMenu.Controls.Add(CreateMenuButton("Thống kê", Statistic_Click));            // UC-09, 17 (Thống kê điểm TB, giới tính)
                    break;
            }

            // Ép WinForms tính toán lại giao diện thực tế để ẩn scrollbar thô và hiển thị phím điều hướng hợp lý
            this.BeginInvoke((MethodInvoker)delegate
            {
                HideFlowMenuScrollbar();
                UpdateButtonVisibility();
            });
        }

        #endregion

        #region MenuEvents

        private void Dashboard_Click(object sender, EventArgs e)
        {
            // OpenChildForm(new DashboardForm());
        }
        
        
        //1.Admin: Quản lý tài khoản, quản lý giảng viên, quản lý sinh viên, danh mục môn học, danh mục lớp học, phân công giảng dạy, giám sát điểm số, phê duyệt yêu cầu, thống kê, báo cáo
        private void Account_Click(object sender, EventArgs e) => OpenChildForm(new AccountManageForm());
        private void Contact_Click(object sender, EventArgs e) => OpenChildForm(new ContactForm());
        private void Student_Click(object sender, EventArgs e) => OpenChildForm(new ListStudentForm(roleId));
        private void Course_Click(object sender, EventArgs e)
        {
            if (roleId == 1) OpenChildForm(new ManageCourseForm());
            else OpenChildForm(new Course.ManageCourseForm());
        }
        private void Classroom_Click(object sender, EventArgs e) => OpenChildForm(new ClassroomForm());
        private void Assign_Click(object sender, EventArgs e) => OpenChildForm(new TeachingAssignmentForm());
        private void Score_Click(object sender, EventArgs e) => OpenChildForm(new ManageScoreForm());
        private void Request_Click(object sender, EventArgs e)
        {
            if (roleId == 0)
                OpenChildForm(new Admin.f_main());
            else if (roleId == 1)
            {
                if (string.IsNullOrEmpty(loggedInMSSV))
                {
                    MessageBox.Show("Không tìm thấy thông tin Mã số sinh viên (MSSV) liên kết với tài khoản này.\nVui lòng kiểm tra lại bảng Students trong Database!",
                                    "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenChildForm(new StudentRequestForm(loggedInMSSV));
            }
        }
        private void Statistic_Click(object sender, EventArgs e) => OpenChildForm(new StatisticsForm());
        private void Report_Click(object sender, EventArgs e) => OpenChildForm(new ReportFormHR());

        //2.Student: Thông tin, môn học, đăng ký môn, điểm
        /*
        private void Profile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển");
        }

        private void MyCourse_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển");
        }

        private void MyScore_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển");
        }

        private void RegisterCourse_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển");
        }
         */
        //3.hr
        private void ScoreSv_Click(object sender, EventArgs e) => OpenChildForm(new StudentScoreForm());

        #endregion

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}