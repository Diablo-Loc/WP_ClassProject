using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Main
{
    public partial class ClassroomForm : Form
    {
        private readonly ClassRoomRepository _classRepo;
        private readonly My_DB _db = new My_DB();
        private bool isEditMode = false;

        public ClassroomForm()
        {
            InitializeComponent();
            string connString = _db.GetConnection().ConnectionString;
            _classRepo = new ClassRoomRepository(connString);
        }

        private void f_Classroom_Load(object sender, EventArgs e)
        {
            txtMaLop.MaxLength = 20;
            ConfigureGridStyle();
            LoadClassroomGrid();
            SwitchMode(false);
        }

        #region --- HÀM TRỢ GIÚP (HELPERS) ---

        private void ConfigureGridStyle()
        {
            dgvClassroom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClassroom.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClassroom.AllowUserToAddRows = false;
            dgvClassroom.ColumnHeadersVisible = true;
            dgvClassroom.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvClassroom.ColumnHeadersHeight = 35;
        }

        // ⭐ HÀM CHUYỂN ĐỔI TRẠNG THÁI THÊM / SỬA LINH HOẠT
        private void SwitchMode(bool editMode)
        {
            isEditMode = editMode;

            if (isEditMode)
            {
                btnInsert.Text = "💾 Cập nhật";
                // Có thể đổi màu nút thành màu cam/vàng để cảnh báo đang sửa (tùy chọn)
                // btnInsert.BackColor = System.Drawing.Color.Orange; 
                txtMaLop.ReadOnly = true; // Khóa khóa chính không cho phá dữ liệu
            }
            else
            {
                btnInsert.Text = "(+) Thêm lớp";
                // btnInsert.BackColor = System.Drawing.Color.DodgerBlue;
                txtMaLop.ReadOnly = false; // Mở khóa để nhập mã mới
            }
        }

        private void BindGrid(DataTable dt)
        {
            if (dt == null) return;

            if (!dt.Columns.Contains("STT"))
            {
                DataColumn colSTT = new DataColumn("STT", typeof(int));
                dt.Columns.Add(colSTT);
                colSTT.SetOrdinal(0);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["STT"] = i + 1;
            }

            dgvClassroom.DataSource = null;
            dgvClassroom.DataSource = dt;

            foreach (DataGridViewColumn col in dgvClassroom.Columns)
            {
                string fieldName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;

                switch (fieldName)
                {
                    case "STT": col.HeaderText = "STT"; col.DisplayIndex = 0; break;
                    case "MaLop": col.HeaderText = "Mã lớp học"; col.DisplayIndex = 1; break;
                    case "TenLop": col.HeaderText = "Tên lớp học"; col.DisplayIndex = 2; break;
                    case "SiSo": col.HeaderText = "Sĩ số"; col.DisplayIndex = 3; break;
                    case "GVCN": col.HeaderText = "Giáo viên chủ nhiệm"; col.DisplayIndex = 4; break;
                }
            }

            if (dgvClassroom.Columns.Contains("btnEditColumn"))
                dgvClassroom.Columns["btnEditColumn"].DisplayIndex = 5;
            if (dgvClassroom.Columns.Contains("btnDeleteColumn"))
                dgvClassroom.Columns["btnDeleteColumn"].DisplayIndex = 6;

            lblTotalClasses.Text = $"Tổng số lớp: {dt.Rows.Count}";
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text) || string.IsNullOrWhiteSpace(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã lớp và Tên lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Classroom GetClassroomFromFields() => new Classroom
        {
            MaLop = txtMaLop.Text.Trim(),
            TenLop = txtTenLop.Text.Trim(),
            SiSo = (int)numSiSo.Value,
            GVCN = txtGVCN.Text.Trim()
        };

        private void ClearInputs()
        {
            txtMaLop.Clear();
            txtTenLop.Clear();
            numSiSo.Value = 0;
            txtGVCN.Clear();
            txtSearch.Clear();

            // ⭐ Khi xóa dữ liệu nhập, tự động trả về chế độ Thêm mới gỡ rối cho người dùng
            SwitchMode(false);
            txtMaLop.Focus();
        }

        private bool IsDataColumn(DataGridViewColumn col)
        {
            return col.Visible &&
                   !(col is DataGridViewButtonColumn) &&
                   col.HeaderText != "Thao tác" &&
                   !col.Name.Contains("Sua") &&
                   !col.Name.Contains("Xoa");
        }

        #endregion

        #region --- XỬ LÝ SỰ KIỆN (EVENTS) ---

        private void LoadClassroomGrid()
        {
            try
            {
                BindGrid(_classRepo.GetClassrooms());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị danh sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ⭐ NÚT GỘP CHỨC NĂNG: ĐÓNG VAI TRÒ CẢ THÊM LẪN CẬP NHẬT
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            if (isEditMode)
            {
                if (_classRepo.UpdateClassroom(GetClassroomFromFields()))
                {
                    MessageBox.Show("Cập nhật thông tin lớp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs(); // Hàm này tự đưa SwitchMode về false
                    LoadClassroomGrid();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (_classRepo.AddClassroom(GetClassroomFromFields()))
                {
                    MessageBox.Show("Thêm mới lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadClassroomGrid();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại! Mã lớp học có thể đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(_classRepo.SearchByTenLop(txtSearch.Text.Trim()));
        }

        // Nút làm mới: Vừa xóa chữ vừa giải thoát khỏi chế độ Sửa, quay về Thêm mới
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            LoadClassroomGrid();
        }

        private void dgvClassroom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var column = dgvClassroom.Columns[e.ColumnIndex];
            string maLop = dgvClassroom.Rows[e.RowIndex].Cells["MaLop"].Value?.ToString()?.Trim();

            if (string.IsNullOrEmpty(maLop)) return;

            // KHI BẤM NÚT SỬA TAY TRÊN GRID
            if (column.Name == "btnEditColumn")
            {
                var row = dgvClassroom.Rows[e.RowIndex];
                txtMaLop.Text = maLop;
                txtTenLop.Text = row.Cells["TenLop"].Value?.ToString();

                if (double.TryParse(row.Cells["SiSo"].Value?.ToString(), out double siso))
                    numSiSo.Value = (decimal)siso;
                else
                    numSiSo.Value = 0;

                txtGVCN.Text = row.Cells["GVCN"].Value?.ToString();

                // ⭐ Bật chế độ Sửa: Nút thêm biến thành nút Cập nhật, Khóa Mã lớp
                SwitchMode(true);

                MessageBox.Show($"Đã tải lớp [{maLop}] lên form. Chỉnh sửa xong bấm nút 'Cập nhật', nếu muốn thoát để thêm mới hãy bấm nút 'Làm mới'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // KHI BẤM NÚT XÓA TAY TRÊN GRID
            else if (column.Name == "btnDeleteColumn")
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa lớp học [{maLop}] không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (_classRepo.DeleteClassroom(maLop))
                    {
                        MessageBox.Show("Xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadClassroomGrid();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            // Đã gộp logic vào btnInsert_Click, không cần dùng nút này nữa
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            // Giữ nguyên logic xuất file CSV an toàn của bạn cũ...
            if (dgvClassroom.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trên bảng để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV File (*.csv)|*.csv", FileName = "Danh_Sach_Lop_Hoc.csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true)))
                    {
                        List<string> headers = new List<string>();
                        for (int i = 0; i < dgvClassroom.Columns.Count; i++)
                        {
                            if (IsDataColumn(dgvClassroom.Columns[i]))
                            {
                                headers.Add($"\"{dgvClassroom.Columns[i].HeaderText}\"");
                            }
                        }
                        sw.WriteLine(string.Join(",", headers));

                        for (int i = 0; i < dgvClassroom.Rows.Count; i++)
                        {
                            if (dgvClassroom.Rows[i].IsNewRow) continue;

                            List<string> cells = new List<string>();
                            for (int j = 0; j < dgvClassroom.Columns.Count; j++)
                            {
                                if (IsDataColumn(dgvClassroom.Columns[j]))
                                {
                                    string val = dgvClassroom.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    val = val.Replace("\"", "\"\"").Replace("\r", "").Replace("\n", " ");
                                    cells.Add($"\"{val}\"");
                                }
                            }
                            sw.WriteLine(string.Join(",", cells));
                        }
                    }
                    MessageBox.Show("Xuất dữ liệu thành công! Click đúp để mở file bằng Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}