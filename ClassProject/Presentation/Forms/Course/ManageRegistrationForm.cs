using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;

namespace ClassProject.Presentation.Forms.Course
{
    public partial class ManageRegistrationForm : Form
    {
        private readonly RegisterRepository _registerRepo;
        private readonly CourseSectionRepository _sectionRepo;
        private readonly StudentRepository _studentRepo;
        private readonly ScoreRepository _scoreRepo;

        // Cờ kiểm soát trạng thái Binding dữ liệu và trạng thái xử lý tiến trình
        private bool _isInitialBinding = false;
        private bool _isProcessing = false;

        // Chuỗi định danh MSSV được phân tách cho tiến trình xử lý UI hiện tại
        private string _resolvedMssv = string.Empty;

        /// <summary>
        /// Constructor mặc định không tham số - Đạt chuẩn đóng gói Global State
        /// </summary>
        public ManageRegistrationForm()
        {
            InitializeComponent();
            _registerRepo = new RegisterRepository();
            _sectionRepo = new CourseSectionRepository();
            _studentRepo = new StudentRepository();
            _scoreRepo = new ScoreRepository();
        }

        private async void RegisterCourseForm_Load(object sender, EventArgs e)
        {
            // 🌟 CHỐT CHẶN BẢO MẬT: Chỉ cho phép Sinh viên, Admin, hoặc HR/Staff (Giáo vụ) truy cập phân hệ này
            if (!UserSession.IsLoggedIn || (!UserSession.IsStudent && !UserSession.IsAdmin && !UserSession.IsStaff))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chức năng đăng ký học phần chỉ dành cho Sinh viên hoặc Ban quản lý đào tạo.",
                                "Cảnh Báo Bảo Mật", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            await ExecuteSecureOperationAsync(async () =>
            {
                _isInitialBinding = true;

                // 1. Tải danh sách cấu hình học phần đang mở trước
                await LoadCourseSectionsAsync();

                // 2. Kiểm tra phân quyền thực tế dựa trên Global Passport (UserSession)
                if (UserSession.IsStudent) // Vai trò Sinh viên (Student)
                {
                    // Lấy thẳng MSSV từ phiên làm việc toàn cục đã được f_main đồng bộ từ trước
                    _resolvedMssv = UserSession.MSSV;

                    // Khóa cứng tính năng chọn sinh viên khác để bảo mật dữ liệu tuyệt đối (Chống Hack ID)
                    cboStudent.Enabled = false;

                    // Hiển thị nhãn chào mừng định danh cá nhân trực quan
                    lblStudentWelcome.Text = $"Sinh viên: {UserSession.Username} | MSSV: {_resolvedMssv} (Cổng đăng ký trực tuyến)";
                    lblStudentWelcome.ForeColor = Color.DarkGreen;

                    // Gán nguồn dữ liệu ảo giả lập để không lộ danh sách toàn trường ra UI
                    DataTable dtSingle = new DataTable();
                    dtSingle.Columns.Add("Mssv");
                    dtSingle.Columns.Add("FullNameWithId");
                    dtSingle.Rows.Add(_resolvedMssv, $"{_resolvedMssv} - Tài khoản cá nhân");

                    cboStudent.DataSource = dtSingle;
                    cboStudent.DisplayMember = "FullNameWithId";
                    cboStudent.ValueMember = "Mssv";
                }
                else // Vai trò Quản trị viên / Giáo vụ (Admin / HR / Staff)
                {
                    cboStudent.Enabled = true;
                    lblStudentWelcome.Text = $"Chế độ: Quản lý / Đăng ký hộ ({UserSession.RoleName})";
                    lblStudentWelcome.ForeColor = Color.DarkRed;

                    await LoadAllStudentsComboAsync();
                }

                _isInitialBinding = false;

                // 3. Đồng bộ nạp dữ liệu lưới hiển thị và chỉ số thống kê học vụ lần đầu
                await RefreshFormCoreDataAsync();
            }, "Khởi tạo phân hệ đăng ký học phần học kỳ");
        }

        #region Các Hàm Tải Dữ Liệu Bất Đồng Bộ (Async Data Loaders)

        private async Task LoadAllStudentsComboAsync()
        {
            DataTable dt = await Task.Run(() => _studentRepo.SearchStudents("", "Tất cả"));

            string idColumn = dt.Columns.Contains("Mssv") ? "Mssv" : "MSSV";
            if (!dt.Columns.Contains("FullNameWithId"))
            {
                dt.Columns.Add("FullNameWithId", typeof(string), idColumn + " + ' - ' + LastName + ' ' + FirstName");
            }

            cboStudent.DataSource = dt;
            cboStudent.DisplayMember = "FullNameWithId";
            cboStudent.ValueMember = idColumn;
        }

        private async Task LoadCourseSectionsAsync()
        {
            DataTable dt = await Task.Run(() => _sectionRepo.GetCourseSections());

            // Lọc an toàn các lớp học phần đang mở (Status = 1) tại bộ nhớ RAM qua DataView
            DataView dv = new DataView(dt);
            dv.RowFilter = "Status = 1";
            DataTable filteredDt = dv.ToTable();

            if (!filteredDt.Columns.Contains("DisplayText"))
            {
                filteredDt.Columns.Add("DisplayText", typeof(string), "MaLopHP + ' | ' + TenMH + ' (Còn: ' + (MaxStudents - SisoHienTai) + ' chỗ)'");
            }

            cboCourse.DataSource = filteredDt;
            cboCourse.DisplayMember = "DisplayText";
            cboCourse.ValueMember = "MaLopHP";
        }

        private async Task RefreshFormCoreDataAsync()
        {
            if (_isInitialBinding) return;

            // Nếu không phải sinh viên, đọc mã định danh sinh viên được chọn trên ComboBox UI
            if (!UserSession.IsStudent && cboStudent.SelectedValue != null)
            {
                _resolvedMssv = cboStudent.SelectedValue.ToString();
            }

            if (string.IsNullOrEmpty(_resolvedMssv) || _resolvedMssv.Contains("System.Data.DataRowView"))
            {
                dgvRegisterCourse.DataSource = null;
                return;
            }

            // Gọi đồng thời DataGrid và Thống kê qua cơ chế xử lý song song để đạt hiệu năng tối đa
            Task<DataTable> gridTask = Task.Run(() => _registerRepo.GetRegistrationList(_resolvedMssv));
            Task<int> totalCoursesTask = Task.Run(() => _registerRepo.GetTotalCoursesRegistered(_resolvedMssv));
            Task<int> totalCreditsTask = Task.Run(() => _registerRepo.GetTotalCreditsRegistered(_resolvedMssv));

            await Task.WhenAll(gridTask, totalCoursesTask, totalCreditsTask);

            // Gán dữ liệu trực quan lên DataGridView
            dgvRegisterCourse.DataSource = null;
            dgvRegisterCourse.Columns.Clear();
            dgvRegisterCourse.AutoGenerateColumns = true;
            dgvRegisterCourse.DataSource = gridTask.Result;

            ConfigureDataGridViewFormat();

            // Cập nhật bảng chỉ số thống kê nhanh
            lblTotalCourses.Text = totalCoursesTask.Result.ToString();
            lblTotalCredits.Text = totalCreditsTask.Result.ToString();
        }

        private void ConfigureDataGridViewFormat()
        {
            if (dgvRegisterCourse.Columns.Count == 0) return;

            dgvRegisterCourse.ColumnHeadersVisible = true;
            dgvRegisterCourse.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            if (dgvRegisterCourse.Columns.Contains("STT")) dgvRegisterCourse.Columns["STT"].Width = 50;
            if (dgvRegisterCourse.Columns.Contains("MaLopHP")) dgvRegisterCourse.Columns["MaLopHP"].HeaderText = "Mã Lớp Học Phần";
            if (dgvRegisterCourse.Columns.Contains("TenMH")) dgvRegisterCourse.Columns["TenMH"].HeaderText = "Tên Môn Học";
            if (dgvRegisterCourse.Columns.Contains("SoTC")) dgvRegisterCourse.Columns["SoTC"].HeaderText = "Số Tín Chỉ";

            if (dgvRegisterCourse.Columns.Contains("MSGV")) dgvRegisterCourse.Columns["MSGV"].Visible = false;

            if (dgvRegisterCourse.Columns.Contains("TenGiangVien"))
                dgvRegisterCourse.Columns["TenGiangVien"].HeaderText = "Giảng Viên Đứng Lớp";
            else if (dgvRegisterCourse.Columns.Contains("GiangVien"))
                dgvRegisterCourse.Columns["GiangVien"].HeaderText = "Giảng Viên Đứng Lớp";

            if (dgvRegisterCourse.Columns.Contains("PhongHoc")) dgvRegisterCourse.Columns["PhongHoc"].HeaderText = "Phòng Học";
            if (dgvRegisterCourse.Columns.Contains("RegistrationDate")) dgvRegisterCourse.Columns["RegistrationDate"].HeaderText = "Ngày Đăng Ký";

            dgvRegisterCourse.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRegisterCourse.AllowUserToAddRows = false;
            dgvRegisterCourse.ReadOnly = true;
            dgvRegisterCourse.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private async void cboStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitialBinding || UserSession.IsStudent) return;

            await ExecuteSecureOperationAsync(async () =>
            {
                await RefreshFormCoreDataAsync();
            }, "Đổi đối tượng sinh viên quản lý");
        }

        #endregion

        #region Xử Lý Nghiệp Vụ Chính (Business Logics)

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (cboCourse.SelectedValue == null || string.IsNullOrEmpty(_resolvedMssv))
            {
                MessageBox.Show("Vui lòng lựa chọn cấu hình lớp học phần hợp lệ trước khi đăng ký!", "Thông báo học vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLopHP = cboCourse.SelectedValue.ToString();

            await ExecuteSecureOperationAsync(async () =>
            {
                bool isAlreadyRegistered = await Task.Run(() => _registerRepo.IsRegistered(_resolvedMssv, maLopHP));
                if (isAlreadyRegistered)
                {
                    MessageBox.Show("Đăng ký không hợp lệ! Sinh viên này đã có tên trong danh sách lớp học phần này.", "Trùng lịch học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool isFull = await Task.Run(() => _registerRepo.IsSectionFull(maLopHP));
                if (isFull)
                {
                    MessageBox.Show("Lớp học phần lựa chọn đã đạt ngưỡng giới hạn số lượng sinh viên tối đa! Vui lòng chọn lớp học phần khác.", "Lớp học phần đã đầy", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                int sectionCredits = await Task.Run(() => _registerRepo.GetCreditsOfSection(maLopHP));
                int currentTotalCredits = await Task.Run(() => _registerRepo.GetTotalCreditsRegistered(_resolvedMssv));

                if (currentTotalCredits + sectionCredits > 24)
                {
                    MessageBox.Show($"Thao tác bị từ chối! Tổng số tín chỉ đăng ký tích lũy trong học kỳ hiện tại sẽ đạt ({currentTotalCredits + sectionCredits} TC), vượt quá giới hạn quy chế là 24 tín chỉ.", "Vượt trần hạn mức tín chỉ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool isSuccess = await Task.Run(() => _registerRepo.AddRegistration(_resolvedMssv, maLopHP));
                if (isSuccess)
                {
                    MessageBox.Show($"Xử lý đăng ký thành công lớp học phần [{maLopHP}] vào lịch trình học vụ!", "Ghi nhận thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _isInitialBinding = true;
                    await LoadCourseSectionsAsync();
                    _isInitialBinding = false;

                    await RefreshFormCoreDataAsync();
                }
            }, "Xử lý đăng ký lớp học phần trực tuyến");
        }

        private async void btnCancelRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_resolvedMssv)) return;

            string maLopHP = string.Empty;

            if (dgvRegisterCourse.CurrentRow != null && dgvRegisterCourse.CurrentRow.Cells["MaLopHP"].Value != null)
            {
                maLopHP = dgvRegisterCourse.CurrentRow.Cells["MaLopHP"].Value?.ToString().Trim() ?? "";
            }
            else if (cboCourse.SelectedValue != null)
            {
                maLopHP = cboCourse.SelectedValue.ToString().Trim();
            }

            if (string.IsNullOrEmpty(maLopHP))
            {
                MessageBox.Show("Vui lòng chọn dòng lớp học phần cần loại bỏ trên lưới dữ liệu!", "Yêu cầu thao tác", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string finalMaLopHP = maLopHP;

            await ExecuteSecureOperationAsync(async () =>
            {
                bool hasScoreRecord = await Task.Run(() => _scoreRepo.HasCourseScore(_resolvedMssv, finalMaLopHP));
                if (hasScoreRecord)
                {
                    MessageBox.Show("Hủy học phần thất bại! Lớp học phần này đã được Giảng viên đồng bộ điểm số lên hệ thống quản lý. Biên bản dữ liệu học vụ đã đóng.", "Hệ thống khóa lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                DialogResult confirm = MessageBox.Show($"Xác nhận loại bỏ lớp học phần [{finalMaLopHP}] ra khỏi lịch trình học kỳ hiện tại?",
                    "Xác nhận hủy đăng ký học vụ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    bool isCanceled = await Task.Run(() => _registerRepo.CancelRegistration(_resolvedMssv, finalMaLopHP));
                    if (isCanceled)
                    {
                        MessageBox.Show("Đã xóa bỏ hoàn toàn lớp học phần ra khỏi danh sách đăng ký.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        _isInitialBinding = true;
                        await LoadCourseSectionsAsync();
                        _isInitialBinding = false;

                        await RefreshFormCoreDataAsync();
                    }
                }
            }, "Xử lý hủy học phần tích lũy");
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            await ExecuteSecureOperationAsync(async () =>
            {
                _isInitialBinding = true;
                await LoadCourseSectionsAsync();
                _isInitialBinding = false;

                await RefreshFormCoreDataAsync();
            }, "Làm mới cấu trúc đồng bộ dữ liệu");
        }

        #endregion

        #region Khung Bao Bọc An Toàn Hệ Thống (Enterprise Core Framework)

        private async Task ExecuteSecureOperationAsync(Func<Task> businessLogic, string operationName)
        {
            if (_isProcessing) return;

            try
            {
                _isProcessing = true;
                this.UseWaitCursor = true;

                await businessLogic();
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                MessageBox.Show($"[Lỗi Cơ Sở Dữ Liệu] Thao tác '{operationName}' thất bại.\nChi tiết mã lỗi: {sqlEx.Number} - {sqlEx.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"SQL Critical Error in '{operationName}': {sqlEx}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Lỗi Hệ Thống] Đã xảy ra lỗi không mong muốn trong quá trình thực thi: '{operationName}'.\nChi tiết: {ex.Message}", "Lỗi Nghiệp Vụ Ứng Dụng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"System Crash Exception in '{operationName}': {ex}");
            }
            finally
            {
                this.UseWaitCursor = false;
                _isProcessing = false;
            }
        }

        #endregion
    }
}