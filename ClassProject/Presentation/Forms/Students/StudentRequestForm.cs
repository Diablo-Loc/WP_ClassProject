using ClassProject.DataAccess.Db;
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
    public partial class StudentRequestForm : Form
    {
        private readonly RequestRepository _requestRepo;

        /// <summary>
        /// Constructor duy nhất không tham số - Triệt tiêu Tight Coupling
        /// </summary>
        public StudentRequestForm()
        {
            InitializeComponent();

            My_DB db = new My_DB();
            _requestRepo = new RequestRepository(db.GetConnection().ConnectionString);
        }

        private void StudentRequestForm_Load(object sender, EventArgs e)
        {
            // BẪY PHÒNG THỦ: Kiểm tra session ngay khi form khởi chạy để tránh lỗi Runtime
            if (string.IsNullOrEmpty(UserSession.MSSV))
            {
                MessageBox.Show("Không thể xác định danh tính sinh viên cho phiên làm việc này!\nHệ thống tự động đóng chức năng.",
                                "Lỗi Xác Thực Phiên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new Action(this.Close));
                return;
            }

            if (cboType != null && cboType.Items.Count == 0)
            {
                cboType.Items.Add("Phúc khảo điểm");
                cboType.Items.Add("Chỉnh sửa thông tin cá nhân");
                cboType.Items.Add("Hỗ trợ đăng ký môn học");
                cboType.Items.Add("Yêu cầu khác");
                cboType.SelectedIndex = 0;
            }

            // Đồng bộ định dạng lưới ngay khi load form
            StyleDataGridView();
            LoadMyRequests();
        }

        // Hàm làm đẹp DataGridView đồng bộ màu với bảng ClassroomForm của bạn
        private void StyleDataGridView()
        {
            dgvMyRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyRequests.AllowUserToAddRows = false;
            dgvMyRequests.EnableHeadersVisualStyles = false; // Cho phép can thiệp màu header
        }

        private void LoadMyRequests()
        {
            try
            {
                if (_requestRepo == null) return;

                // Bốc trực tiếp MSSV an toàn từ UserSession toàn cục
                DataTable dt = _requestRepo.GetRequestsByStudent(UserSession.MSSV);
                dgvMyRequests.DataSource = dt;
                FormatGridColumns();

                // Cập nhật tổng số dòng lên Label
                if (lblTotal != null)
                {
                    lblTotal.Text = $"Tổng số yêu cầu: {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lịch sử yêu cầu: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tách hàm định dạng tiêu đề cột ra dùng chung cho cả Load dữ liệu lẫn Tìm kiếm
        private void FormatGridColumns()
        {
            if (dgvMyRequests.Columns.Count > 0)
            {
                if (dgvMyRequests.Columns.Contains("Id")) dgvMyRequests.Columns["Id"].HeaderText = "Mã YC";
                if (dgvMyRequests.Columns.Contains("RequestContent")) dgvMyRequests.Columns["RequestContent"].HeaderText = "Nội dung yêu cầu";
                if (dgvMyRequests.Columns.Contains("Status")) dgvMyRequests.Columns["Status"].HeaderText = "Trạng thái";
                if (dgvMyRequests.Columns.Contains("AdminComment")) dgvMyRequests.Columns["AdminComment"].HeaderText = "Phản hồi từ Admin";
                if (dgvMyRequests.Columns.Contains("Created_At")) dgvMyRequests.Columns["Created_At"].HeaderText = "Ngày gửi";

                dgvMyRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Mẹo nhỏ: Thu nhỏ cột ID lại vì nội dung chỉ có số ngắn
                if (dgvMyRequests.Columns.Contains("Id"))
                {
                    dgvMyRequests.Columns["Id"].FillWeight = 40;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearchRequests.Text.Trim();

                // Nếu để trống ô tìm kiếm thì tự động load lại toàn bộ danh sách
                if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm yêu cầu, ...")
                {
                    LoadMyRequests();
                    return;
                }

                // Sử dụng trực tiếp UserSession.MSSV tại đây thay vì biến nội bộ
                DataTable result = _requestRepo.SearchRequests(UserSession.MSSV, keyword);
                dgvMyRequests.DataSource = result;
                FormatGridColumns();

                if (lblTotal != null)
                {
                    lblTotal.Text = $"Tìm thấy: {result.Rows.Count} yêu cầu";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực hiện tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            string requestType = cboType.SelectedItem?.ToString() ?? "Yêu cầu khác";
            string textContent = txtRequestContent.Text.Trim();

            if (string.IsNullOrWhiteSpace(textContent))
            {
                MessageBox.Show("Vui lòng viết nội dung yêu cầu trước khi gửi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fullContent = $"[{requestType}] - {textContent}";

            // Gửi yêu cầu với Passport UserSession.MSSV
            if (_requestRepo.AddRequest(UserSession.MSSV, fullContent))
            {
                MessageBox.Show("Yêu cầu của bạn đã được gửi lên hệ thống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetForm();
                LoadMyRequests();
            }
            else
            {
                MessageBox.Show("Gửi yêu cầu thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
            LoadMyRequests(); // Load lại lưới sạch sẽ khi bấm làm mới
        }

        private void ResetForm()
        {
            txtRequestContent.Clear();
            if (txtSearchRequests != null) txtSearchRequests.Clear();
            if (cboType != null && cboType.Items.Count > 0)
            {
                cboType.SelectedIndex = 0;
            }
        }
    }
}