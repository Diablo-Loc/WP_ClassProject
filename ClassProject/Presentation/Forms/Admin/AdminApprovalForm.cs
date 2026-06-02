using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class AdminApprovalForm : Form
    {
        private readonly RequestRepository _requestRepo;

        public AdminApprovalForm()
        {
            InitializeComponent();
            My_DB db = new My_DB();
            _requestRepo = new RequestRepository(db.GetConnection().ConnectionString);
        }

        private void AdminApprovalForm_Load(object sender, EventArgs e)
        {
            StyleDataGridView();
            LoadPendingRequests();
        }

        // Định dạng lưới đồng bộ phong cách với ClassroomForm
        private void StyleDataGridView()
        {
            dgvPendingRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPendingRequests.AllowUserToAddRows = false;
            dgvPendingRequests.EnableHeadersVisualStyles = false;

            // Ép màu xanh dương chủ đạo UTE giống ClassroomForm
            dgvPendingRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPendingRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPendingRequests.RowTemplate.Height = 30;
        }

        // Tải danh sách các yêu cầu đang chờ duyệt
        private void LoadPendingRequests()
        {
            try
            {
                DataTable dt = _requestRepo.GetPendingRequests();
                dgvPendingRequests.DataSource = dt;
                FormatGridColumns();

                // Cập nhật nhãn "Tổng số yêu cầu chờ xử lý" ở góc trên bên phải UI
                if (lblTotalPending != null)
                {
                    lblTotalPending.Text = $"Tổng số yêu cầu chờ xử lý: {dt.Rows.Count}";
                }

                ClearDetailFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách chờ phê duyệt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm định dạng cột dùng chung
        private void FormatGridColumns()
        {
            if (dgvPendingRequests.Columns.Count > 0)
            {
                if (dgvPendingRequests.Columns.Contains("Id")) dgvPendingRequests.Columns["Id"].HeaderText = "Mã YC";
                if (dgvPendingRequests.Columns.Contains("MSSV")) dgvPendingRequests.Columns["MSSV"].HeaderText = "Mã số SV";
                if (dgvPendingRequests.Columns.Contains("RequestContent")) dgvPendingRequests.Columns["RequestContent"].HeaderText = "Nội dung yêu cầu";
                if (dgvPendingRequests.Columns.Contains("Created_At")) dgvPendingRequests.Columns["Created_At"].HeaderText = "Ngày gửi hỗ trợ";

                dgvPendingRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dgvPendingRequests.Columns.Contains("Id")) dgvPendingRequests.Columns["Id"].FillWeight = 40;
                if (dgvPendingRequests.Columns.Contains("MSSV")) dgvPendingRequests.Columns["MSSV"].FillWeight = 60;
            }
        }

        private void dgvPendingRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPendingRequests.CurrentRow == null || e.RowIndex < 0) return;

            try
            {
                // Đổ dữ liệu lên các ô TextBox ở nhóm "CHI TIẾT YÊU CẦU"
                txtRequestId.Text = dgvPendingRequests.CurrentRow.Cells["Id"].Value?.ToString();
                txtStudentMSSV.Text = dgvPendingRequests.CurrentRow.Cells["MSSV"].Value?.ToString();
                txtRequestContentDetail.Text = dgvPendingRequests.CurrentRow.Cells["RequestContent"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // Sự kiện khi Admin gõ từ khóa và bấm Enter hoặc hệ thống tự tìm (hoặc bạn có thể gọi qua nút Tìm kiếm)
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword) || keyword.StartsWith("Tìm kiếm theo"))
            {
                LoadPendingRequests();
                return;
            }

            try
            {
                DataTable result = _requestRepo.SearchPendingRequests(keyword);
                dgvPendingRequests.DataSource = result;
                FormatGridColumns();

                if (lblTotalPending != null)
                {
                    lblTotalPending.Text = $"Tìm thấy: {result.Rows.Count} yêu cầu";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện NÚT LÀM MỚI (Nút màu xanh lá trên giao diện của bạn)
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtAdminComment.Clear();
            LoadPendingRequests();
        }

         private void ProcessRequest(string targetStatus)
        {
            if (string.IsNullOrEmpty(txtRequestId.Text))
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu cụ thể từ danh sách trước khi xử lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int requestId = Convert.ToInt32(txtRequestId.Text);
            string mssv = txtStudentMSSV.Text;
            string comment = txtAdminComment.Text.Trim();

            string actionName = targetStatus == "Approved" ? "PHÊ DUYỆT" : "TỪ CHỐI";
            MessageBoxIcon icon = targetStatus == "Approved" ? MessageBoxIcon.Question : MessageBoxIcon.Warning;

            var confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn {actionName} yêu cầu của sinh viên [{mssv}] không?",
                                                "Xác nhận xử lý", MessageBoxButtons.YesNo, icon);

            if (confirmResult == DialogResult.Yes)
            {
                if (_requestRepo.UpdateRequestStatus(requestId, targetStatus, comment))
                {
                    MessageBox.Show($"Đã {actionName} thành công yêu cầu của SV {mssv}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnRefresh_Click(null, null); // Kích hoạt làm mới giao diện ngay lập tức
                }
                else
                {
                    MessageBox.Show("Cập nhật trạng thái yêu cầu thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            ProcessRequest("Approved");
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdminComment.Text))
            {
                MessageBox.Show("Khi bấm Từ chối, bạn bắt buộc phải nhập lý do/phản hồi vào ô bên dưới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAdminComment.Focus();
                return;
            }
            ProcessRequest("Declined");
        }

        private void ClearDetailFields()
        {
            txtRequestId.Clear();
            txtStudentMSSV.Clear();
            txtRequestContentDetail.Clear();
        }
    }
}