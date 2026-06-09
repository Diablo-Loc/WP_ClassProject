using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ContactForm : Form
    {
        private readonly ContactRepository _contactRepo;
        private readonly My_DB _db = new My_DB();
        private bool isEditMode = false;
        private int selectedContactId = -1;

        public ContactForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;
            _contactRepo = new ContactRepository(connString);

            this.txtPhone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPhone_KeyPress);
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(241, 245, 249);
            StyleGrid();
            LoadContactGrid(); // Hàm này chạy xong sẽ tự kích hoạt dòng đầu và cấu hình trạng thái
        }

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

            dgvContacts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvContacts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvContacts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvContacts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvContacts.RowsDefaultCellStyle.BackColor = Color.White;
            dgvContacts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvContacts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgvContacts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);

            dgvContacts.RowHeadersVisible = true;
            dgvContacts.RowHeadersWidth = 40;
            dgvContacts.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvContacts.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvContacts.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
        }

        private void FormatGridColumns()
        {
            if (dgvContacts.Columns.Count > 0)
            {
                if (dgvContacts.Columns.Contains("ContactID")) dgvContacts.Columns["ContactID"].Visible = false;
                if (dgvContacts.Columns.Contains("Name")) dgvContacts.Columns["Name"].HeaderText = "Họ và tên / Phòng ban";
                if (dgvContacts.Columns.Contains("Phone")) dgvContacts.Columns["Phone"].HeaderText = "Số điện thoại";
                if (dgvContacts.Columns.Contains("Email")) dgvContacts.Columns["Email"].HeaderText = "Địa chỉ Email";

                dgvContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void SwitchMode(bool editMode)
        {
            isEditMode = editMode;

            if (isEditMode)
            {
                btnInsert.Text = "💾 Cập nhật";
                btnInsert.BackColor = Color.FromArgb(245, 158, 11);
            }
            else
            {
                btnInsert.Text = "(+) Thêm liên hệ";
                btnInsert.BackColor = Color.FromArgb(37, 99, 235);
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

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người hoặc phòng ban liên hệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            string phoneInput = txtPhone.Text.Trim();
            string emailInput = txtEmail.Text.Trim();

            string phonePattern = @"^(03|05|07|08|09)[0-9]{8}$";
            if (!Regex.IsMatch(phoneInput, phonePattern))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!\nVui lòng nhập đúng 10 chữ số và bắt đầu bằng các đầu số hợp lệ (03, 05, 07, 08, 09).",
                                "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhone.Focus();
                return;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(emailInput, emailPattern))
            {
                MessageBox.Show("Địa chỉ Email không đúng cấu trúc quy định!\nVí dụ hợp lệ: contact@hcmute.edu.vn hoặc user@gmail.com",
                                "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            Contact contact = new Contact
            {
                ContactID = selectedContactId,
                Name = txtName.Text.Trim(),
                Phone = phoneInput,
                Email = emailInput
            };

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

        // ⭐ ĐÃ FIX TRIỆT ĐỂ: Cơ chế phòng thủ kép lấy trực tiếp ID từ Grid nếu biến nhớ bị ghi đè
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvContacts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng click chọn một liên hệ trên bảng trước khi bấm xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvContacts.SelectedRows[0];

            // Nếu biến selectedContactId bị reset về -1 lỗi, lấy lại trực tiếp từ dòng đang chọn
            if (selectedContactId == -1 && row.Cells["ContactID"].Value != null)
            {
                selectedContactId = Convert.ToInt32(row.Cells["ContactID"].Value);
            }

            if (selectedContactId == -1)
            {
                MessageBox.Show("Không tìm thấy mã định danh (ID) của liên hệ cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string currentName = row.Cells["Name"].Value?.ToString();
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

                if (dt != null)
                {
                    lblTotalContacts.Text = $"Kết quả tìm kiếm: {dt.Rows.Count}";
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            LoadContactGrid();
        }

        private void dgvContacts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvContacts.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvContacts.SelectedRows[0];

                if (row.Cells["ContactID"].Value != null && row.Cells["ContactID"].Value != DBNull.Value)
                {
                    selectedContactId = Convert.ToInt32(row.Cells["ContactID"].Value);

                    txtName.Text = row.Cells["Name"].Value?.ToString();
                    txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                    txtEmail.Text = row.Cells["Email"].Value?.ToString();

                    SwitchMode(true);
                }
            }
            else
            {
                selectedContactId = -1;
            }
        }

        private void dgvContacts_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            string stt = (e.RowIndex + 1).ToString();
            Font rFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Brush rBrush = new SolidBrush(Color.FromArgb(100, 116, 139));

            var grid = (DataGridView)sender;
            float x = e.RowBounds.Location.X + (grid.RowHeadersWidth - e.Graphics.MeasureString(stt, rFont).Width) / 2;
            float y = e.RowBounds.Location.Y + (e.RowBounds.Height - rFont.Height) / 2;

            e.Graphics.DrawString(stt, rFont, rBrush, x, y);
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}