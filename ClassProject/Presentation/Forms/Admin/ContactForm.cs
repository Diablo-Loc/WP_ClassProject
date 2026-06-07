using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ContactForm : Form
    {
        private readonly ContactRepository _contactRepo;
        private readonly My_DB _db = new My_DB();
        private bool isEditMode = false;
        private int selectedContactId = -1; // Lưu lại ID khi chọn dòng cần sửa

        public ContactForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;
            _contactRepo = new ContactRepository(connString);
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(241, 245, 249);

            StyleGrid();
            LoadContactGrid();
            SwitchMode(false);
        }

        // Làm đẹp DataGridView dgvContacts theo style Slate cao cấp
        private void StyleGrid()
        {
            if (dgvContacts == null) return;

            dgvContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContacts.AllowUserToAddRows = false;
            dgvContacts.EnableHeadersVisualStyles = false;
            dgvContacts.RowTemplate.Height = 35;
            dgvContacts.GridColor = Color.FromArgb(241, 245, 249);
            dgvContacts.BackgroundColor = Color.White;
            dgvContacts.BorderStyle = BorderStyle.None;

            dgvContacts.ColumnHeadersVisible = true;
            dgvContacts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvContacts.ColumnHeadersHeight = 35;

            // Màu tiêu đề Slate tối
            dgvContacts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvContacts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvContacts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvContacts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Màu dòng xen kẽ
            dgvContacts.RowsDefaultCellStyle.BackColor = Color.White;
            dgvContacts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvContacts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvContacts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);

            // 🛠️ BẬT CỘT TIÊU ĐỀ HÀNG (LỀ TRÁI) ĐỂ CHỨA SỐ THỨ TỰ, TRÁNH BỊ ĐÈ CHỮ
            dgvContacts.RowHeadersVisible = true;
            dgvContacts.RowHeadersWidth = 40; // Độ rộng vừa đủ cho số thứ tự
            dgvContacts.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvContacts.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // Màu nền lề trái xám nhẹ mượt mà
            dgvContacts.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
        }

        // Đồng bộ hóa tên cột tiếng Việt trực quan
        private void FormatGridColumns()
        {
            if (dgvContacts.Columns.Count > 0)
            {
                if (dgvContacts.Columns.Contains("ContactID")) dgvContacts.Columns["ContactID"].Visible = false; // Ẩn khóa chính

                if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên / Phòng ban";
                if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại";
                if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";

                dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        // Chuyển đổi trạng thái Thêm / Sửa linh hoạt
        private void SwitchMode(bool editMode)
        {
            isEditMode = editMode;

            if (isEditMode)
            {
                btnInsert.Text = "💾 Cập nhật";
                btnInsert.BackColor = Color.FromArgb(245, 158, 11); // Đổi sang màu vàng cam cảnh báo đang sửa
            }
            else
            {
                btnInsert.Text = "(+) Thêm liên hệ";
                btnInsert.BackColor = Color.FromArgb(37, 99, 235); // Màu xanh dương thêm mới
                selectedContactId = -1;
            }
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtSearch.Clear();

            SwitchMode(false);
            txtName.Focus();
        }

        private void LoadContactGrid()
        {
            try
            {
                DataTable dt = _contactRepo.GetContacts();
                dgvContacts.DataSource = dt;
                FormatGridColumns();

                if (dt != null)
                {
                    lblTotalContacts.Text = $"Tổng số liên lạc: {dt.Rows.Count}";
                }
                else
                {
                    lblTotalContacts.Text = "Tổng số liên lạc: 0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị danh sách liên hệ: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút đa năng: Đóng vai trò thực hiện Thêm mới hoặc Cập nhật (Sửa dữ liệu cũ)
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người hoặc phòng ban liên hệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            Contact contact = new Contact
            {
                ContactID = selectedContactId,
                Name = txtName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            // ⭐ HÀM UPDATE: Nếu đang ở chế độ chỉnh sửa (isEditMode = true) thì tiến hành cập nhật dữ liệu mới
            if (isEditMode)
            {
                if (_contactRepo.UpdateContact(contact))
                {
                    MessageBox.Show("Cập nhật thông tin liên hệ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadContactGrid();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại, vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Ngược lại, nếu ở trạng thái bình thường thì thực hiện Thêm mới bản ghi
            else
            {
                if (_contactRepo.AddContact(contact))
                {
                    MessageBox.Show("Thêm mới liên hệ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadContactGrid();
                }
                else
                {
                    MessageBox.Show("Thêm liên hệ thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ⭐ ĐẨY DỮ LIỆU LÊN CHỖ ĐIỀN: Bấm chọn một dòng trên Grid để hiện thông tin lên các ô nhập và chuẩn bị sửa
        private void dgvContacts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvContacts.Rows[e.RowIndex];

            // Lấy ID ẩn của liên hệ được chọn
            selectedContactId = Convert.ToInt32(row.Cells["ContactID"].Value);

            // Đẩy dữ liệu lên các ô TextBox để người dùng chỉnh sửa
            txtName.Text = row.Cells["Name"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();

            // Kích hoạt trạng thái Sửa (Nút chuyển thành "Cập nhật" và đổi sang màu cam cảnh báo)
            SwitchMode(true);
        }

        // Xử lý sự kiện bấm nút Xóa bản ghi được chọn
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvContacts.CurrentRow == null || selectedContactId == -1)
            {
                MessageBox.Show("Vui lòng click chọn một liên hệ trên bảng trước khi bấm xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentName = dgvContacts.CurrentRow.Cells["Name"].Value?.ToString();
            DialogResult dialogResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa liên hệ [{currentName}] không?", "Xác nhận hành động", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                if (_contactRepo.DeleteContact(selectedContactId))
                {
                    MessageBox.Show("Đã xóa thông tin liên hệ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadContactGrid();
                }
                else
                {
                    MessageBox.Show("Xóa dữ liệu thất bại, vui lòng thử lại!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Tìm kiếm nhanh liên hệ
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadContactGrid();
            }
            else
            {
                DataTable dt = _contactRepo.SearchContacts(keyword);
                dgvContacts.DataSource = dt;
                FormatGridColumns();

                // Cập nhật số lượng theo kết quả tìm kiếm được
                if (dt != null)
                {
                    lblTotalContacts.Text = $"Kết quả tìm kiếm: {dt.Rows.Count}";
                }
            }
        }

        // Nút Làm mới dữ liệu và đưa Form về trạng thái thêm mới ban đầu
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            LoadContactGrid();
        }

        private void dgvContacts_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Tính toán số thứ tự dựa trên chỉ số dòng hiện tại (bắt đầu từ 1)
            string stt = (e.RowIndex + 1).ToString();

            // Cấu hình font chữ và màu sắc hiển thị cho số thứ tự (Màu Slate tối đồng bộ)
            Font rFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Brush rBrush = new SolidBrush(Color.FromArgb(100, 116, 139)); // Đổi sang xám Slate hiện đại, dịu mắt hơn

            // 🛠️ ĐỊNH VỊ CHÍNH XÁC VÙNG VẼ NẰM TRONG Ô TIÊU ĐỀ HÀNG TRÁI
            var grid = (DataGridView)sender;
            float x = e.RowBounds.Location.X + (grid.RowHeadersWidth - e.Graphics.MeasureString(stt, rFont).Width) / 2;
            float y = e.RowBounds.Location.Y + (e.RowBounds.Height - rFont.Height) / 2;

            // Tiến hành vẽ số thứ tự lên giao diện lề trái
            e.Graphics.DrawString(stt, rFont, rBrush, x, y);
        }
    }
}