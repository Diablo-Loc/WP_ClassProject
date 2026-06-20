using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using ClassProject.Business.Services; // Namespace tầng dịch vụ AI và An ninh của bạn
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Auth
{
    public partial class FaceLoginForm : Form
    {
        private readonly My_DB _db = new My_DB();
        private FaceIdService _faceService = null;
        private VideoCapture _capture = null;

        // 🌟 THÊM: Khởi tạo dịch vụ kiểm toán an ninh
        private readonly SecurityMonitoringService _securityService = new SecurityMonitoringService();
        // 🌟 THÊM: Biến lưu trữ email của user đăng nhập phục vụ gửi alert ngầm nhanh chóng
        private string _cachedUserEmail = string.Empty;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsRegisterMode { get; set; } = false;
        private string _currentUsername = ""; // Dùng cho đăng ký
        private string _loginUsername = "";   // Dùng cho đăng nhập (Xác thực 1:1)

        private DateTime _lastPredictTime = DateTime.MinValue;
        private bool _isProcessingFrame = false;

        // Constructor dùng chung linh hoạt
        public FaceLoginForm(string username, bool isRegisterMode)
        {
            InitializeComponent();
            this.IsRegisterMode = isRegisterMode;

            if (isRegisterMode) this._currentUsername = username;
            else this._loginUsername = username;

            this.Load += FaceLoginForm_Load;
            this.FormClosing += (s, e) => StopCamera();
            btnCancel.Click += (s, e) => this.Close();
            btnRegisterFace.Click += BtnRegisterFace_Click;
        }

        private async void FaceLoginForm_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Đang khởi tạo hệ thống sinh trắc học...";
            Application.DoEvents();

            // Nếu ở chế độ đăng nhập, lấy sẵn email của user từ DB để nạp vào cache
            if (!IsRegisterMode)
            {
                _cachedUserEmail = await Task.Run(() => GetUserEmailFromDB(_loginUsername));
            }

            try
            {
                // Gọi Service xử lý tầng AI ngầm
                await Task.Run(() => { _faceService = new FaceIdService(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (IsRegisterMode)
            {
                btnRegisterFace.Visible = true;
                lblTitle.Text = "ĐĂNG KÝ SINH TRẮC HỌC FACE ID";
                lblStatus.Text = "Vui lòng nhìn thẳng vào camera và nhấn nút 'Chụp & Đăng ký'.";
                await Task.Run(() => StartCameraNgam());
            }
            else
            {
                btnRegisterFace.Visible = false;
                lblStatus.Text = "Đang đối chiếu dữ liệu nhận diện khuôn mặt...";

                // Truyền thêm '_loginUsername' vào tầng Service xử lý kiểm tra & nạp mẫu
                bool isSuccess = await Task.Run(() => _faceService.InitializeAI(_loginUsername, msg =>
                {
                    this.Invoke(new Action(() => MessageBox.Show(msg, "Chưa đăng ký Face ID", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                }));

                if (isSuccess)
                {
                    lblStatus.Text = $"Hệ thống đang xác thực khuôn mặt cho tài khoản: {_loginUsername}";
                    await Task.Run(() => StartCameraNgam());
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void StartCameraNgam()
        {
            try
            {
                _capture = new VideoCapture(0);
                _capture.Set(CapProp.FrameWidth, 640);
                _capture.Set(CapProp.FrameHeight, 480);
                _capture.ImageGrabbed += ProcessFrame;
                _capture.Start();
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Không kết nối được Webcam: " + ex.Message, "Lỗi phần cứng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }));
            }
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (_capture == null || _isProcessingFrame) return;
            _isProcessingFrame = true;

            using (Mat frame = new Mat())
            {
                _capture.Retrieve(frame);
                if (frame.IsEmpty) { _isProcessingFrame = false; return; }

                using (Image<Bgr, byte> currentFrame = frame.ToImage<Bgr, byte>())
                using (Image<Gray, byte> grayFrame = currentFrame.Convert<Gray, byte>())
                {
                    Rectangle[] facesDetected = _faceService.DetectFaces(grayFrame);

                    foreach (Rectangle faceRect in facesDetected)
                    {
                        currentFrame.Draw(faceRect, new Bgr(Color.LimeGreen), 2);

                        // Thực hiện quét so khớp 1:1 sau mỗi 400ms
                        if (!IsRegisterMode && (DateTime.Now - _lastPredictTime).TotalMilliseconds > 400)
                        {
                            _lastPredictTime = DateTime.Now;

                            using (Image<Gray, byte> trainedFaceResult = grayFrame.Copy(faceRect).Resize(200, 200, Inter.Cubic))
                            {
                                string matchedUsername = _faceService.PredictOwner(trainedFaceResult);

                                // TRƯỜNG HỢP 1: ĐỐI CHIẾU KHỚP KHUÔN MẶT
                                if (matchedUsername != null && matchedUsername.Equals(_loginUsername, StringComparison.OrdinalIgnoreCase))
                                {
                                    // KIỂM TOÁN NGẦM: Ghi nhận đăng nhập Face ID thành công & Quét kiểm tra giờ lạ
                                    _securityService.ProcessSecurityAudit(_loginUsername, isSuccess: true, method: "FACE_ID", userEmail: _cachedUserEmail);

                                    this.Invoke(new Action(() =>
                                    {
                                        StopCamera();
                                        ProcLoginWithFace(matchedUsername);
                                    }));
                                    _isProcessingFrame = false;
                                    return;
                                }
                                // TRƯỜNG HỢP 2: SAI KHUÔN MẶT (Có người lạ đứng trước camera hoặc sai tài khoản)
                                else
                                {
                                    // KIỂM TOÁN NGẦM: Ghi nhận thất bại Face ID, AI bắt đầu tính tần suất dò mặt liên tiếp trong 5 phút
                                    _securityService.ProcessSecurityAudit(_loginUsername, isSuccess: false, method: "FACE_ID", userEmail: _cachedUserEmail, failureReason: "Guong mat khong trung khop");

                                    this.BeginInvoke(new Action(() =>
                                    {
                                        lblStatus.Text = "Cảnh báo: Khuôn mặt không khớp với tài khoản yêu cầu!";
                                    }));
                                }
                            }
                        }
                    }

                    // Đẩy hình lên PictureBox
                    if (picCamera.IsHandleCreated)
                    {
                        Bitmap bmp = currentFrame.ToBitmap();
                        this.BeginInvoke(new Action(() =>
                        {
                            if (picCamera.Image != null)
                            {
                                var oldImg = picCamera.Image;
                                picCamera.Image = bmp;
                                oldImg.Dispose();
                            }
                            else picCamera.Image = bmp;
                        }));
                    }
                }
            }
            _isProcessingFrame = false;
        }

        // Các hàm BtnRegisterFace_Click, ProcLoginWithFace, StopCamera giữ nguyên...
        private void BtnRegisterFace_Click(object sender, EventArgs e)
        {
            if (_capture == null) return;

            using (Mat frame = new Mat())
            {
                _capture.Retrieve(frame);
                if (frame.IsEmpty) return;

                using (Image<Bgr, byte> currentFrame = frame.ToImage<Bgr, byte>())
                using (Image<Gray, byte> grayFrame = currentFrame.Convert<Gray, byte>())
                {
                    Rectangle[] facesDetected = _faceService.DetectFaces(grayFrame);

                    if (facesDetected.Length == 0)
                    {
                        MessageBox.Show("Không thấy khuôn mặt chính diện!", "Thử lại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        _faceService.SaveRegisterFace(grayFrame, facesDetected[0], _currentUsername);

                        StopCamera();
                        MessageBox.Show($"Đăng ký Face ID cho [{_currentUsername}] thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi lưu ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ProcLoginWithFace(string username)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT u.Id, u.RoleId, u.Email, ISNULL(u.Valid, 0) AS Valid, ISNULL(u.Status, 0) AS Status,
                               ISNULL(s.LastName + ' ' + s.FirstName, 
                               ISNULL(t.LastName + ' ' + t.FirstName, 
                               ISNULL(st.LastName + ' ' + st.FirstName, N'Hệ thống Administrator'))) AS FullName
                        FROM Users u
                        LEFT JOIN Students s ON u.Id = s.UserId
                        LEFT JOIN Teachers t ON u.Id = t.UserId
                        LEFT JOIN Staffs st ON u.Id = st.UserId
                        WHERE u.Username = @user AND ISNULL(u.Status, 0) != -1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["Id"]);
                                int roleId = Convert.ToInt32(reader["RoleId"]);
                                int valid = Convert.ToInt32(reader["Valid"]);
                                int status = Convert.ToInt32(reader["Status"]);
                                string email = reader["Email"].ToString();
                                string fullName = reader["FullName"].ToString();

                                if (roleId != 0)
                                {
                                    if (valid == 0) { MessageBox.Show("Tài khoản đang chờ duyệt!"); this.Close(); return; }
                                    if (status == 1 || status == 2) { MessageBox.Show("Tài khoản đang bị khóa!"); this.Close(); return; }
                                }

                                UserSession.Initialize(userId, username, roleId, email, fullName, "", "");
                                MessageBox.Show($"[FACE ID] Xin chào {UserSession.FullName}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi CSDL: " + ex.Message);
                    this.Close();
                }
            }
        }

        private void StopCamera()
        {
            if (_capture != null)
            {
                _capture.Stop();
                _capture.ImageGrabbed -= ProcessFrame;
                _capture.Dispose();
                _capture = null;
            }
            _faceService?.Dispose();
        }

        // Hàm bổ trợ đọc nhanh Email lúc load Form nhằm tăng hiệu năng tối đa
        private string GetUserEmailFromDB(string username)
        {
            string email = string.Empty;
            string query = "SELECT Email FROM Users WHERE Username = @user";
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            email = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi lấy Email cache: " + ex.Message);
            }
            return email;
        }

        private async void btnTestSecurityAlert_Click(object sender, EventArgs e)
        {
            // 1. TỰ ĐỘNG LẤY TÀI KHOẢN ĐANG ĐĂNG NHẬP
            string usernameTest = _loginUsername;

            // Nếu chạy Form độc lập mà chưa truyền Username từ Form Login chính sang, lấy mặc định để không bị lỗi
            if (string.IsNullOrWhiteSpace(usernameTest))
            {
                usernameTest = "User_Test_AI";
            }

            // 2. TỰ ĐỘNG KIỂM TRA EMAIL TRONG DB (Đồng bộ cache như lúc load Form thật)
            lblStatus.Text = "Đang kiểm tra thông tin tài khoản từ Cơ sở dữ liệu...";
            string targetEmail = _cachedUserEmail;

            if (string.IsNullOrWhiteSpace(targetEmail))
            {
                // Nếu lúc load form chưa kịp lấy hoặc chạy độc lập, tiến hành đọc CSDL thời gian thực luôn
                targetEmail = await Task.Run(() => GetUserEmailFromDB(usernameTest));
            }

            // Biện pháp an toàn: Nếu tài khoản test này trong DB bị bỏ trống Email hoặc điền mail giả không gửi được, 
            // Hệ thống sẽ thông báo nhắc nhở lập trình viên thay vì chạy lỗi.
            if (string.IsNullOrWhiteSpace(targetEmail) || !targetEmail.Contains("@"))
            {
                MessageBox.Show($"Tài khoản [{usernameTest}] hiện chưa có Email hợp lệ trong Cơ sở dữ liệu!\n" +
                                "Vui lòng cập nhật Email thật trong bảng Users trước khi chạy tính năng này.",
                                "Thông báo cấu hình DB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatus.Text = "Sẵn sàng.";
                return;
            }

            // 3. ĐỒNG BỘ LOGIC PHÂN TÍCH HÀNH VI TỰ ĐỘNG (Kiểm tra thời gian thực tế thay vì hardcode chuỗi)
            lblStatus.Text = "Hệ thống AI đang phân tích rủi ro môi trường đăng nhập...";
            btnTestSecurityAlert.Enabled = false;

            // Tự động quét kiểm tra xem giờ bấm nút có nằm ngoài khung giờ làm việc hành chính (7h - 18h) hay không
            int currentHour = DateTime.Now.Hour;
            bool isAnomalousTime = (currentHour < 7 || currentHour >= 18);

            // Tạo chuỗi lý do động dựa trên trạng thái thời gian thực tế của máy tính bạn
            string dynamicReasons = string.Empty;
            if (isAnomalousTime)
            {
                dynamicReasons += $"* CẢNH BÁO: Hệ thống phát hiện lượt truy cập vào khung giờ ngoài hành chính ({DateTime.Now:HH:mm:ss} PM/AM).\n";
            }
            else
            {
                dynamicReasons += $"* GIẢ LẬP: Phát hiện đăng nhập khung giờ hành chính ({DateTime.Now:HH:mm:ss}) nhưng kích hoạt chế độ kiểm tra cưỡng bức.\n";
            }
            dynamicReasons += "* NGUY CƠ: Ghi nhận chuỗi hành vi dò quét Face ID thất bại liên tiếp (Vượt ngưỡng 5 lần/đoạn ngắn).";

            try
            {
                // 4. KHỞI TẠO VÀ KÍCH HOẠT TIẾN TRÌNH GỬI THƯ NGẦM (Đọc từ App.config tập trung)
                EmailAlertService alertService = new EmailAlertService();

                // Gửi trực tiếp đến Email thật của User được cấu hình trong bảng dữ liệu SQL
                await alertService.SendSecurityAlertAsync(targetEmail, usernameTest, dynamicReasons);

                // 5. THÔNG BÁO THÀNH CÔNG VỚI THÔNG TIN THỰC TẾ
                MessageBox.Show($"[REAL-TIME AI MOCK] Hệ thống phân tích hành vi đã kích hoạt chốt chặn an ninh thành công!\n\n" +
                                $"📌 Tài khoản: {usernameTest}\n" +
                                $"📩 Email nhận cảnh báo (Lấy từ DB): {targetEmail}\n" +
                                $"🕒 Trạng thái thời gian: {(isAnomalousTime ? "Giờ lạ (Cảnh báo đỏ)" : "Giờ bình thường (Chế độ Test)")}",
                                "Xác thực kiểm toán an ninh thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi luồng gửi Mail Cảnh báo: " + ex.Message, "Lỗi kết nối SMTP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Hoàn trả trạng thái ban đầu cho giao diện WinForms
                lblStatus.Text = "Sẵn sàng.";
                btnTestSecurityAlert.Enabled = true;
            }
        }
    }
}