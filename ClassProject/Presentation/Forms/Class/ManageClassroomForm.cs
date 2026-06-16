using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using ClassProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassProject.Presentation.Forms.Admin
{
    public partial class ManageClassroomForm : Form
    {
        private readonly ClassRoomRepository _classRepo;
        private bool isEditMode = false;

        public ManageClassroomForm()
        {
            InitializeComponent();
            _classRepo = new ClassRoomRepository();
        }

        // CHUYỂN ĐỔI ASYNC: Kích hoạt khi Form bắt đầu được load lên màn hình
        private async void ManageClassroomForm_Load(object sender, EventArgs e)
        {
            // 🌟 CHỐT CHẶN BẢO MẬT TẦNG 1: Kiểm tra quyền truy cập của Admin (Role 0) hoặc Giáo vụ/HR (Role 2)
            if (!UserSession.IsLoggedIn || (!UserSession.IsAdmin && !UserSession.IsStaff))
            {
                MessageBox.Show("Quyền truy cập bị từ chối! Chức năng quản lý lớp hành chính chỉ dành cho Ban quản trị hoặc phòng Giáo vụ/HR.",
                                "Cảnh Báo An Ninh", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Đóng form an toàn thông qua BeginInvoke tránh xung đột UI Thread khi form đang load
                this.BeginInvoke(new MethodInvoker(this.Close));
                return;
            }

            // Cấu hình giới hạn ký tự nhập liệu trực tiếp trên UI phòng thủ tràn dữ liệu
            txtMaLop.MaxLength = 20;
            txtTenLop.MaxLength = 100;
            if (txtSearch != null) txtSearch.MaxLength = 100;
            numSiSo.Enabled = false;

            ConfigureGridStyle();
            await LoadInitialDataAsync();
        }

        #region --- KHỞI TẠO DỮ LIỆU VÀ CẤU HÌNH UI ---

        private void ConfigureGridStyle()
        {
            dgvClassroom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClassroom.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClassroom.AllowUserToAddRows = false;
            dgvClassroom.ReadOnly = true;
            dgvClassroom.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvClassroom.ColumnHeadersHeight = 35;
        }

        // CHUYỂN ĐỔI ASYNC: Tải dữ liệu ban đầu không gây nghẽn luồng UI
        private async Task LoadInitialDataAsync()
        {
            try
            {
                // 1. Tải danh sách Giảng viên chủ nhiệm lên cboGVCN (Bất đồng bộ)
                DataTable dtTeachers = await _classRepo.GetActiveTeachersAsync();
                cboGVCN.DataSource = dtTeachers;
                cboGVCN.DisplayMember = "Username";
                cboGVCN.ValueMember = "Username";
                cboGVCN.SelectedIndex = -1;

                // 2. Tải danh mục ngành học lên cboNganhHoc (Bất đồng bộ)
                DataTable dtMajors = await _classRepo.GetAllMajorsAsync();
                cboNganhHoc.DataSource = dtMajors;
                cboNganhHoc.DisplayMember = "TenNganh";
                cboNganhHoc.ValueMember = "MaNganh";
                cboNganhHoc.SelectedIndex = -1;

                // 3. Đổ dữ liệu lên bảng hiển thị
                await LoadClassroomGridAsync();
                SwitchMode(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo cấu hình dữ liệu ban đầu: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SwitchMode(bool editMode)
        {
            isEditMode = editMode;

            if (isEditMode)
            {
                btnInsert.Text = "💾 Cập nhật";
                txtMaLop.ReadOnly = true;
                txtTenLop.Focus();
            }
            else
            {
                btnInsert.Text = "➕ Lưu lớp học";
                txtMaLop.ReadOnly = false;
                txtMaLop.Focus();
            }
        }

        private void BindGrid(DataTable dt)
        {
            if (dt == null) return;

            // Tạo cột STT tự động nếu CSDL không trả về sẵn
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

            // Việt hóa tiêu đề các cột trên GridView
            foreach (DataGridViewColumn col in dgvClassroom.Columns)
            {
                string fieldName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;

                switch (fieldName)
                {
                    case "STT": col.HeaderText = "STT"; col.Width = 60; col.DisplayIndex = 0; break;
                    case "MaLop": col.HeaderText = "Mã lớp học"; col.DisplayIndex = 1; break;
                    case "TenLop": col.HeaderText = "Tên lớp học"; col.DisplayIndex = 2; break;
                    case "SiSo": col.HeaderText = "Sĩ số hiện tại"; col.DisplayIndex = 3; break;
                    case "GVCN": col.HeaderText = "Giảng viên chủ nhiệm"; col.DisplayIndex = 4; break;
                    case "TenNganh": col.HeaderText = "Chuyên ngành"; col.DisplayIndex = 5; break;
                    case "MaNganh": col.Visible = false; break;
                    case "Status": col.Visible = false; break;
                }
            }

            if (dgvClassroom.Columns.Contains("btnEditColumn"))
                dgvClassroom.Columns["btnEditColumn"].DisplayIndex = 6;
            if (dgvClassroom.Columns.Contains("btnDeleteColumn"))
                dgvClassroom.Columns["btnDeleteColumn"].DisplayIndex = 7;

            lblTotalClasses.Text = $"Tổng số lớp hành chính: {dt.Rows.Count}";
        }

        #endregion

        #region --- XỬ LÝ LOGIC NGHIỆP VỤ ---

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text) || string.IsNullOrWhiteSpace(txtTenLop.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc: Mã lớp và Tên lớp!", "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboNganhHoc.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn một ngành học cụ thể từ danh mục!", "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Classroom GetClassroomFromFields() => new Classroom
        {
            MaLop = txtMaLop.Text.Trim().ToUpper(), // Tự động viết hoa mã lớp
            TenLop = txtTenLop.Text.Trim(),
            GVCN = cboGVCN.SelectedIndex != -1 ? cboGVCN.SelectedValue?.ToString()?.Trim() : null,
            MaNganh = cboNganhHoc.SelectedIndex != -1 ? cboNganhHoc.SelectedValue?.ToString()?.Trim() : null
        };

        private void ClearInputs()
        {
            txtMaLop.Clear();
            txtTenLop.Clear();
            numSiSo.Value = 0;
            cboGVCN.SelectedIndex = -1;
            cboNganhHoc.SelectedIndex = -1;
            txtSearch.Clear();

            SwitchMode(false);
        }

        // CHUYỂN ĐỔI ASYNC: Đọc danh sách lưới bất đồng bộ
        private async Task LoadClassroomGridAsync()
        {
            try
            {
                DataTable dt = await _classRepo.GetClassroomsAsync();
                BindGrid(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách lớp học: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsDataColumn(DataGridViewColumn col)
        {
            return col.Visible &&
                   !(col is DataGridViewButtonColumn) &&
                   col.HeaderText != "Thao tác" &&
                   col.HeaderText != "Xóa" &&
                   !col.Name.Contains("Sua") &&
                   !col.Name.Contains("Xoa");
        }

        #endregion

        #region --- SỰ KIỆN ĐIỀU KHIỂN (CONTROL EVENTS) ---

        // CHUYỂN ĐỔI ASYNC: Nút Lưu lớp học xử lý đa luồng an toàn
        private async void btnInsert_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Classroom classroom = GetClassroomFromFields();

                if (isEditMode)
                {
                    // Gọi hàm cập nhật Async
                    if (await _classRepo.UpdateClassroomAsync(classroom))
                    {
                        MessageBox.Show("Cập nhật thông tin lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs();
                        await LoadClassroomGridAsync();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại. Lớp học có thể đã bị xóa bởi một phiên làm việc khác!", "Lỗi Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Gọi hàm thêm mới Async
                    if (await _classRepo.AddClassroomAsync(classroom))
                    {
                        MessageBox.Show("Thêm mới lớp học vào hệ thống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs();
                        await LoadClassroomGridAsync();
                    }
                    else
                    {
                        MessageBox.Show("Thêm mới thất bại. Mã lớp học này đã tồn tại trên hệ thống!", "Lỗi Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cảnh Báo Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CHUYỂN ĐỔI ASYNC: Tìm kiếm theo nút bấm
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = await _classRepo.SearchClassroomsAsync(txtSearch.Text.Trim());
                BindGrid(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực thi tìm kiếm: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
            await LoadClassroomGridAsync();
        }

        // CHUYỂN ĐỔI ASYNC: Click nút chức năng Sửa/Xóa tích hợp trực tiếp trên Cell Grid
        private async void dgvClassroom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                var column = dgvClassroom.Columns[e.ColumnIndex];
                string maLop = dgvClassroom.Rows[e.RowIndex].Cells["MaLop"].Value?.ToString()?.Trim();

                if (string.IsNullOrEmpty(maLop)) return;

                // THÀNH PHẦN 1: KHI CLICK VÀO NÚT "SỬA" -> Cả Admin và Giáo vụ (HR) đều làm được
                if (column.Name == "btnEditColumn")
                {
                    var row = dgvClassroom.Rows[e.RowIndex];
                    txtMaLop.Text = maLop;
                    txtTenLop.Text = row.Cells["TenLop"].Value?.ToString();

                    if (int.TryParse(row.Cells["SiSo"].Value?.ToString(), out int siso))
                        numSiSo.Value = siso;
                    else
                        numSiSo.Value = 0;

                    if (row.Cells["GVCN"].Value != null && !string.IsNullOrEmpty(row.Cells["GVCN"].Value.ToString()))
                    {
                        cboGVCN.SelectedValue = row.Cells["GVCN"].Value.ToString().Trim();
                    }
                    else
                    {
                        cboGVCN.SelectedIndex = -1;
                    }

                    if (row.Cells["MaNganh"].Value != null && !string.IsNullOrEmpty(row.Cells["MaNganh"].Value.ToString()))
                    {
                        cboNganhHoc.SelectedValue = row.Cells["MaNganh"].Value.ToString().Trim();
                    }
                    else
                    {
                        cboNganhHoc.SelectedIndex = -1;
                    }

                    SwitchMode(true);
                }
                // THÀNH PHẦN 2: KHI CLICK VÀO NÚT "XÓA" -> Phân quyền chặt, chặn Giáo vụ tại chỗ
                else if (column.Name == "btnDeleteColumn")
                {
                    // 🌟 CHỐT CHẶN PHÂN QUYỀN VÒNG 2: Chỉ Admin tối cao mới được quyền xóa lớp
                    if (!UserSession.IsAdmin)
                    {
                        MessageBox.Show("Quyền hạn bị hạn chế! Tài khoản phòng Giáo vụ/HR không được phép xóa lớp học hành chính nhằm bảo toàn dữ liệu sinh viên.",
                                        "Từ Chối Thực Thi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }

                    var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa lớp học [{maLop}] ra khỏi hệ thống?\nHành động này chỉ thành công nếu lớp không có sinh viên học tập.",
                                                 "Xác nhận xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        if (await _classRepo.DeleteClassroomAsync(maLop))
                        {
                            MessageBox.Show("Xóa bản ghi lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadClassroomGridAsync();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại. Bản ghi lớp học không còn tồn tại!", "Lỗi Nghiệp Vụ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Ràng Buộc Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CHUYỂN ĐỔI ASYNC: Viết File xuất dữ liệu dạng IO Bất đồng bộ giúp giảm áp lực luồng đĩa cứng
        private async void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvClassroom.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu trên lưới hiển thị để thực hiện xuất tập tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        await sw.WriteLineAsync(string.Join(",", headers));

                        for (int i = 0; i < dgvClassroom.Rows.Count; i++)
                        {
                            if (dgvClassroom.Rows[i].IsNewRow) continue;

                            List<string> cells = new List<string>();
                            for (int j = 0; j < dgvClassroom.Columns.Count; j++)
                            {
                                if (IsDataColumn(dgvClassroom.Columns[j]))
                                {
                                    string val = dgvClassroom.Rows[i].Cells[j].Value?.ToString() ?? string.Empty;
                                    val = val.Replace("\"", "\"\"").Replace("\r", "").Replace("\n", " ");
                                    cells.Add($"\"{val}\"");
                                }
                            }
                            await sw.WriteLineAsync(string.Join(",", cells));
                        }
                    }
                    MessageBox.Show("Xuất tập tin định dạng .csv thành công! Bạn có thể sử dụng Microsoft Excel để kiểm tra.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi trong quá trình xuất luồng dữ liệu file: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // CHUYỂN ĐỔI ASYNC: Live Search tức thì khi người dùng gõ phím
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                if (keyword.Length >= 2)
                {
                    DataTable dt = await _classRepo.SearchClassroomsAsync(keyword);
                    BindGrid(dt);
                }
                else if (keyword.Length == 0)
                {
                    ClearInputs();
                    await LoadClassroomGridAsync();
                }
            }
            catch { }
        }
        #endregion
    }
}