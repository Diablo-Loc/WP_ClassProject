using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class AccountDetailForm : Form
    {
        private string username;
        private My_DB db = new My_DB();

        private Label lblUsername, lblRole, lblEmail, lblPhone, lblCreatedAt, lblStatus;
        private Button btnClose;

        public AccountDetailForm(string username)
        {

            this.username = username;
            BuildDynamicUI();
        }

        private void AccountDetailForm_Load(object sender, EventArgs e)
        {
            LoadUserDetail();
        }

        private void BuildDynamicUI()
        {
            // 1. Cấu hình Form chính
            this.Text = $"Thông tin chi tiết: {username}";
            this.Size = new Size(460, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // 2. Tạo Panel Header làm điểm nhấn thanh lịch
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 55;
            pnlHeader.BackColor = Color.FromArgb(245, 247, 250);
            this.Controls.Add(pnlHeader);

            Label lblTitle = new Label();
            lblTitle.Text = "HỒ SƠ TÀI KHOẢN CHI TIẾT";
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 16);
            lblTitle.AutoSize = true;
            pnlHeader.Controls.Add(lblTitle);

            // 3. Hàm phụ trợ tạo nhanh các hàng thông tin xếp dọc tự động
            int startY = 80;
            int rowHeight = 40;

            lblUsername = CreateInfoRow("Tên tài khoản (Username):", startY);
            lblRole = CreateInfoRow("Chức vụ / Quyền hạn:", startY += rowHeight);
            lblEmail = CreateInfoRow("Địa chỉ Email:", startY += rowHeight);
            lblPhone = CreateInfoRow("Số điện thoại:", startY += rowHeight);
            lblCreatedAt = CreateInfoRow("Ngày đăng ký:", startY += rowHeight);
            lblStatus = CreateInfoRow("Trạng thái hệ thống:", startY += rowHeight);

            // Tinh chỉnh riêng cho nhãn trạng thái đậm lên
            lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // 4. Khởi tạo nút Đóng thiết kế bo phẳng hiện đại
            btnClose = new Button();
            btnClose.Text = "Đóng cửa sổ";
            btnClose.Size = new Size(120, 36);
            btnClose.Location = new Point(300, 325);
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.FlatAppearance.BorderColor = Color.DarkGray;
            btnClose.BackColor = Color.FromArgb(240, 240, 240);
            btnClose.Click += btnClose_Click;
            this.Controls.Add(btnClose);

            // Đăng ký sự kiện Load form
            this.Load += AccountDetailForm_Load;
        }

        // Hàm vẽ tự động các tiêu đề bên trái và ô chứa giá trị bên phải hàng loạt
        private Label CreateInfoRow(string labelText, int yPosition)
        {
            // Vẽ tiêu đề (Ví dụ: Địa chỉ Email:)
            Label lblHeader = new Label();
            lblHeader.Text = labelText;
            lblHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblHeader.ForeColor = Color.DimGray;
            lblHeader.Location = new Point(25, yPosition);
            lblHeader.Size = new Size(180, 25);
            lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblHeader);

            // Vẽ ô chứa nội dung trả về từ Database bên phải
            Label lblValue = new Label();
            lblValue.Text = "Đang tải...";
            lblValue.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblValue.ForeColor = Color.Black;
            lblValue.Location = new Point(210, yPosition);
            lblValue.Size = new Size(220, 25);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblValue);

            return lblValue; // Trả về đối tượng để gán dữ liệu SQL vào sau này
        }

        private void LoadUserDetail()
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    // Sử dụng LEFT JOIN để lấy Email, Phone từ bảng Students (nếu có) 
                    // và sửa u.CreatedAt thành u.Created_At theo đúng file SQL
                    string query = @"SELECT u.Username, 
                                    r.RoleName, 
                                    u.Status, 
                                    ISNULL(s.Email, u.Email) AS Email, 
                                    s.Phone, 
                                    u.Created_At
                             FROM dbo.Users u
                             INNER JOIN dbo.Roles r ON u.RoleId = r.Id
                             LEFT JOIN dbo.Students s ON u.Id = s.UserId
                             WHERE u.Username = @user";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];

                            lblUsername.Text = row["Username"].ToString();
                            lblRole.Text = row["RoleName"].ToString();
                            lblEmail.Text = row["Email"] != DBNull.Value && !string.IsNullOrEmpty(row["Email"].ToString()) ? row["Email"].ToString() : "Chưa cập nhật";
                            lblPhone.Text = row["Phone"] != DBNull.Value && !string.IsNullOrEmpty(row["Phone"].ToString()) ? row["Phone"].ToString() : "Chưa cập nhật";

                            if (row["Created_At"] != DBNull.Value)
                                lblCreatedAt.Text = Convert.ToDateTime(row["Created_At"]).ToString("dd/MM/yyyy HH:mm");
                            else
                                lblCreatedAt.Text = "Không rõ";

                            int status = Convert.ToInt32(row["Status"]);
                            SetStatusLabel(status);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết tài khoản từ Database: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetStatusLabel(int status)
        {
            if (status == 0) { lblStatus.Text = "⏳ Chờ phê duyệt"; lblStatus.ForeColor = Color.Orange; }
            else if (status == 1) { lblStatus.Text = "🟢 Đang hoạt động"; lblStatus.ForeColor = Color.Green; }
            else if (status == 2) { lblStatus.Text = "🔴 Đã khóa"; lblStatus.ForeColor = Color.Red; }
            else if (status == 3) { lblStatus.Text = "❌ Đã từ chối"; lblStatus.ForeColor = Color.Gray; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}