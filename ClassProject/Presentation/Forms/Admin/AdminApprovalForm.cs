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
    public partial class f_main : Form
    {
        private readonly RequestRepository _requestRepo;

        public f_main()
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
            dgvPendingRequests.RowPostPaint += dgvPendingRequests_RowPostPaint;
        }

        private void dgvPendingRequests_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString(); // Số thứ tự bắt đầu từ 1

            using (Brush brush = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                // Canh lề chữ số thứ tự nằm giữa vùng Row Header bên trái
                var centerFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                Rectangle headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, grid.Font, brush, headerBounds, centerFormat);
            }
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

                // TỰ ĐỘNG CHỌN VÀ HIỂN THỊ DÒNG ĐẦU TIÊN
                SelectFirstRowAndDisplay();
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
                // 💡 LỰA CHỌN HIỂN THỊ MÃ YÊU CẦU (ID)
                // Nếu bạn muốn hiển thị luôn cột Mã Yêu Cầu (Id) từ SQL, hãy đổi .Visible = true và đặt Header Text
                if (dgvPendingRequests.Columns.Contains("Id"))
                {
                    dgvPendingRequests.Columns["Id"].Visible = true; // Hiện cột mã yêu cầu lên lưới
                    dgvPendingRequests.Columns["Id"].HeaderText = "Mã YC";
                }

                if (dgvPendingRequests.Columns.Contains("Created_At")) dgvPendingRequests.Columns["Created_At"].Visible = false;

                if (dgvPendingRequests.Columns.Contains("MSSV"))
                    dgvPendingRequests.Columns["MSSV"].HeaderText = "MSSV";

                if (dgvPendingRequests.Columns.Contains("FullName"))
                    dgvPendingRequests.Columns["FullName"].HeaderText = "Họ tên";

                if (dgvPendingRequests.Columns.Contains("RequestContent"))
                    dgvPendingRequests.Columns["RequestContent"].HeaderText = "Nội dung";

                if (dgvPendingRequests.Columns.Contains("Status"))
                    dgvPendingRequests.Columns["Status"].HeaderText = "Status";

                // Cấu hình co giãn tự động toàn màn hình
                dgvPendingRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Mở rộng cột RowHeaders một chút để hiển thị số thứ tự không bị che khuất
                dgvPendingRequests.RowHeadersWidth = 45;

                // Phân bổ tỷ lệ độ rộng (FillWeight) bao gồm cả cột Mã YC (Id) nếu hiển thị
                if (dgvPendingRequests.Columns.Contains("Id")) dgvPendingRequests.Columns["Id"].FillWeight = 45;
                if (dgvPendingRequests.Columns.Contains("MSSV")) dgvPendingRequests.Columns["MSSV"].FillWeight = 50;
                if (dgvPendingRequests.Columns.Contains("FullName")) dgvPendingRequests.Columns["FullName"].FillWeight = 90;
                if (dgvPendingRequests.Columns.Contains("RequestContent")) dgvPendingRequests.Columns["RequestContent"].FillWeight = 160;
                if (dgvPendingRequests.Columns.Contains("Status")) dgvPendingRequests.Columns["Status"].FillWeight = 50;
            }
        }

        // Tự động chọn hàng đầu tiên và kích hoạt đổ dữ liệu lên TextBox
        private void SelectFirstRowAndDisplay()
        {
            if (dgvPendingRequests.Rows.Count > 0)
            {
                dgvPendingRequests.ClearSelection();

                dgvPendingRequests.Rows[0].Selected = true;
                dgvPendingRequests.CurrentCell = dgvPendingRequests.Rows[0].Cells[GetFirstVisibleColumnIndex()];

                DisplayRowData(dgvPendingRequests.Rows[0]);
            }
            else
            {
                ClearDetailFields();
            }
        }

        // Hàm bổ trợ lấy cột đang hiển thị đầu tiên (Tránh lỗi Cell ẩn)
        private int GetFirstVisibleColumnIndex()
        {
            foreach (DataGridViewColumn col in dgvPendingRequests.Columns)
            {
                if (col.Visible) return col.Index;
            }
            return 0;
        }

        // Đổ dữ liệu ra hàm riêng để dùng chung cho cả sự kiện Click và tự động chọn
        private void DisplayRowData(DataGridViewRow row)
        {
            if (row == null) return;
            try
            {
                txtRequestId.Text = row.Cells["Id"].Value?.ToString();
                txtStudentMSSV.Text = row.Cells["MSSV"].Value?.ToString();
                txtRequestContentDetail.Text = row.Cells["RequestContent"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvPendingRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPendingRequests.CurrentRow == null || e.RowIndex < 0) return;
            DisplayRowData(dgvPendingRequests.CurrentRow);
        }

        // Sự kiện khi Admin gõ từ khóa
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
                SelectFirstRowAndDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện NÚT LÀM MỚI
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
                    btnRefresh_Click(null, null);
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