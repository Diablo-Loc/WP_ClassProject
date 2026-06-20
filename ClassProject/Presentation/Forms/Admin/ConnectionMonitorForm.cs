using System;
using System.Drawing;
using System.Windows.Forms;
using ClassProject.Business.Services;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ConnectionMonitorForm : Form
    {
        public ConnectionMonitorForm()
        {
            InitializeComponent();
            ConfigCustomUI();
        }

        private void ConnectionMonitorForm_Load(object sender, EventArgs e)
        {
            // Đăng ký sự kiện cập nhật dữ liệu tự động
            ConnectionMonitor.OnDataChanged += RefreshDashboardData;

            // Load dữ liệu ban đầu
            RefreshDashboardData();
        }

        private void ConfigCustomUI()
        {
            this.Text = "🤖 AI Connection Operations Center";
            this.Size = new Size(850, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Thiết kế Gridview chuẩn doanh nghiệp
            dgvLeaks.DataSource = ConnectionMonitor.LeakHistoryTable;
            dgvLeaks.AllowUserToAddRows = false;
            dgvLeaks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLeaks.RowHeadersVisible = false;
            dgvLeaks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Đăng ký sự kiện click dòng để AI phân tích khuyên dùng
            dgvLeaks.CellClick += dgvLeaks_CellClick;
        }

        private void RefreshDashboardData()
        {
            // Đảm bảo không bị crash luồng chéo (Cross-thread UI control access)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshDashboardData));
                return;
            }

            lblActiveCount.Text = ConnectionMonitor.ActiveConnections.Count.ToString();
            lblLeakCount.Text = ConnectionMonitor.LeakHistoryTable.Rows.Count.ToString();

            // Nếu có lỗi Leak thì đổi màu số đếm sang đỏ rực để cảnh báo, không thì để màu Gray mặc định
            if (ConnectionMonitor.LeakHistoryTable.Rows.Count > 0)
            {
                lblLeakCount.ForeColor = Color.FromArgb(231, 76, 60); // Màu đỏ Coral cực đẹp
            }
            else
            {
                lblLeakCount.ForeColor = Color.DarkGray;
            }
        }

        // Sự kiện của Timer chạy đều đặn mỗi giây để thúc ép AI quét lỗi
        private void tmrScan_Tick(object sender, EventArgs e)
        {
            ConnectionMonitor.CheckForLeaks();
        }

        // Click dòng nào, AI phân tích lỗi dòng đó ngay lập tức!
        private void dgvLeaks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvLeaks.Rows[e.RowIndex];
                string fileName = row.Cells["File"].Value?.ToString();
                string lineNum = row.Cells["Line"].Value?.ToString();
                string connId = row.Cells["ConnID"].Value?.ToString();

                // Tạo văn bản phân tích dựa trên Heuristic AI Engine
                txtAiRecommendation.Text =
                    $"[AI ENGINE ANALYSIS REPORT]{Environment.NewLine}" +
                    $"──────────────────────────────────────────{Environment.NewLine}" +
                    $"❌ Phát hiện vi phạm an toàn tài nguyên tại Kết nối mang mã định danh: #{connId}.{Environment.NewLine}" +
                    $"📍 Vị trí chính xác: File [{fileName}] -> Tại vị trí Dòng số: {lineNum}.{Environment.NewLine}" +
                    $"⚠️ Nguyên nhân: Lập trình viên gọi hàm Open() nhưng không thực hiện giải phóng (Dispose) hoặc đóng kết nối khiến bộ nhớ của SQL Connection Pool bị chiếm dụng lâu hơn thời gian quy định.{Environment.NewLine}{Environment.NewLine}" +
                    $"💡 KHUYẾN NGHỊ KHẮC PHỤC TỪ AI DOANH NGHIỆP:{Environment.NewLine}" +
                    $"Hãy bọc khối lệnh gọi kết nối tại dòng {lineNum} trong file {fileName} bằng cú pháp cấu trúc: 'using var conn = db.GetConnection();' hoặc cấu trúc khối 'using (SqlConnection conn = db.GetConnection()) {{ ... }}' để cơ chế tự giải phóng tự động kích hoạt khi có ngoại lệ xảy ra.";
            }
        }

        private void ConnectionMonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ cho chính Form
            ConnectionMonitor.OnDataChanged -= RefreshDashboardData;
        }
        // 🟢 Giả lập luồng kết nối ĐÚNG chuẩn doanh nghiệp
        private void btnSimulateSafe_Click(object sender, EventArgs e)
        {
            // Giả lập ID ngẫu nhiên cho kết nối
            string mockConnId = "Safe_" + Guid.NewGuid().ToString().Substring(0, 4);

            // 1. Khai báo Mở kết nối
            ConnectionMonitor.RegisterOpen(mockConnId, "StudentRepository.cs", 45);
            RefreshDashboardData();

            // 2. Tạo một Timer nhỏ để tự động Đóng sau 0.3 giây (mô phỏng truy vấn thực tế)
            var delayTimer = new System.Windows.Forms.Timer();
            delayTimer.Interval = 300;
            delayTimer.Tick += (s, args) =>
            {
                ConnectionMonitor.RegisterClose(mockConnId);
                RefreshDashboardData();
                delayTimer.Stop();
                delayTimer.Dispose();
            };
            delayTimer.Start();
        }

        // ❌ Giả lập luồng kết nối SAI (Gây rò rỉ - Connection Leak)
        private void btnSimulateLeak_Click(object sender, EventArgs e)
        {
            string mockConnId = "Leak_" + Guid.NewGuid().ToString().Substring(0, 4);

            // Khai báo mở kết nối tại dòng 112 file DashBoardForm.cs nhưng cố tình BỎ QUÊN không gọi Close
            ConnectionMonitor.RegisterOpen(mockConnId, "DashBoardForm.cs", 112);
            RefreshDashboardData();

            MessageBox.Show("Đã cố tình mở 1 kết nối ngầm và bỏ quên!\nAI đang tiến hành giám sát, vui lòng đợi 3 giây...",
                            "AI Simulator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}