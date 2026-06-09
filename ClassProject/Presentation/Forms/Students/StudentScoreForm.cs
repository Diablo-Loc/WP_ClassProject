using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

using Xceed.Document.NET;
using Xceed.Words.NET;
using Xceed.Drawing;

namespace ClassProject.Presentation.Forms.Students
{
    public partial class StudentScoreForm : Form
    {
        private ScoreRepository _scoreRepository;
        private string _connectionString;

        public StudentScoreForm()
        {
            InitializeComponent();

            _connectionString = new My_DB().GetConnection().ConnectionString;
            _scoreRepository = new ScoreRepository(_connectionString);
        }

        private void StudentScoreForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Bước A: Đưa giao diện tổng hợp về trạng thái reset ban đầu
                ResetSummary();

                // Bước B: Đổ dữ liệu Lớp học vào cboClass từ Database để người dùng sẵn sàng sử dụng
                LoadClassesToComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khởi tạo Form: " + ex.Message, "Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadClassesToComboBox()
        {
            string query = "SELECT DISTINCT Class FROM dbo.Students WHERE Class IS NOT NULL AND Class != ''";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            conn.Open();
                            da.Fill(dt);

                            cboClass.DataSource = dt;
                            cboClass.DisplayMember = "Class";
                            cboClass.ValueMember = "Class";

                            cboClass.SelectedIndex = -1;
                        }
                        catch { /* Xử lý ẩn hoặc bỏ qua nếu bảng Student chưa có cột Class */ }
                    }
                }
            }
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            string mssv = txtMSSV.Text.Trim();

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(mssv))
            {
                MessageBox.Show("Vui lòng nhập Mã số sinh viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetSummary();
                return;
            }

            try
            {
                // TỰ ĐỘNG TÌM KIẾM VÀ ĐIỀN TÊN SINH VIÊN
                string queryStudent = "SELECT FirstName, LastName FROM dbo.Students WHERE MSSV = @mssv";
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryStudent, conn))
                    {
                        cmd.Parameters.AddWithValue("@mssv", mssv);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["FirstName"]?.ToString() ?? "";
                                string lastName = reader["LastName"]?.ToString() ?? "";
                                txtName.Text = (firstName + " " + lastName).Trim();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin sinh viên mang mã số này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ResetSummary();
                                return;
                            }
                        }
                    }
                }

                // 2. TÁI SỬ DỤNG REPOSITORY: Gọi hàm vừa viết để lấy dữ liệu về
                DataTable dt = _scoreRepository.GetStudentTranscripts(mssv);

                // 3. Kiểm tra nếu không có dữ liệu điểm
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Sinh viên tồn tại nhưng chưa có dữ liệu điểm số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvScores.DataSource = null;

                    // Reset thông số điểm về 0 nhưng giữ nguyên tên sinh viên vừa tìm thấy
                    label5.Text = "Tổng Số Tín Chỉ\n0";
                    label4.Text = "Điểm Trung Bình\n0.00";
                    label6.Text = "Xếp Loại\nChưa có điểm";
                    return;
                }

                // 4. Đổ dữ liệu lên lưới hiển thị
                dgvScores.DataSource = dt;

                // 5. Thực hiện tính toán Điểm trung bình trọng số & Xếp loại học lực
                CalculateAndDisplayMetrics(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateAndDisplayMetrics(DataTable dt)
        {
            int tongSoTC = 0;
            double tongDiemTichLuy = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Số Tín Chỉ"] != DBNull.Value && row["Điểm TK"] != DBNull.Value)
                {
                    int soTC = Convert.ToInt32(row["Số Tín Chỉ"]);
                    double diemTK = Convert.ToDouble(row["Điểm TK"]);

                    tongDiemTichLuy += (diemTK * soTC);
                    tongSoTC += soTC;
                }
            }

            double diemTB = tongSoTC > 0 ? Math.Round(tongDiemTichLuy / tongSoTC, 2) : 0.0;

            string xepLoai = "Yếu";
            if (diemTB >= 9.0) xepLoai = "Xuất Sắc";
            else if (diemTB >= 8.0) xepLoai = "Giỏi";
            else if (diemTB >= 6.5) xepLoai = "Khá";
            else if (diemTB >= 5.0) xepLoai = "Trung Bình";

            label5.Text = $"Tổng Số Tín Chỉ\n{tongSoTC}";
            label4.Text = $"Điểm Trung Bình\n{diemTB:0.00}";
            label6.Text = $"Xếp Loại\n{xepLoai}";
        }

        private void ResetSummary()
        {
            label5.Text = "Tổng Số Tín Chỉ\n0";
            label4.Text = "Điểm Trung Bình\n0.00";
            label6.Text = "Xếp Loại\nChưa xếp loại";
            txtName.Text = "";
        }

        private void BtnExportWord_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu trên lưới Guna
            if (dgvScores.DataSource == null || dgvScores.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập MSSV và tìm kiếm dữ liệu trước khi xuất file Word!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Mở hộp thoại lưu file .docx
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Word Document (*.docx)|*.docx";
                sfd.FileName = $"BangDiem_{txtMSSV.Text.Trim()}.docx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 3. Tự khởi tạo một file Word mới tinh hoàn toàn bằng code (Không cần template)
                        using (DocX document = DocX.Create(sfd.FileName))
                        {
                            // ĐỊNH DẠNG FONT CHỮ CHUẨN
                            string fontName = "Times New Roman";

                            // --- PHẦN HEADER: TIÊU NGỮ (SỬ DỤNG BẢNG ẨN CHIA 2 CỘT SONG SONG) ---
                            Table headerTable = document.AddTable(2, 2);
                            headerTable.Alignment = Alignment.center;

                            // Thiết lập độ rộng thích hợp cho 2 cột để chữ không bị rớt dòng lỗi
                            headerTable.Rows[0].Cells[0].Width = 260;
                            headerTable.Rows[0].Cells[1].Width = 300;

                            // --- CỘT TRÁI: THÔNG TIN CƠ QUAN / TRƯỜNG BẢO CHỦ ---
                            var pLeft = headerTable.Rows[0].Cells[0].Paragraphs[0];
                            pLeft.Append("BỘ GIÁO DỤC VÀ ĐÀO TẠO\n").Font(fontName).FontSize(10).Alignment = Alignment.center;
                            pLeft.Append("TRƯỜNG ĐẠI HỌC CÔNG NGHỆ\nKỸ THUẬT TP.HCM\n").Bold().Font(fontName).FontSize(10).Alignment = Alignment.center;
                            pLeft.Append("---------------").Font(fontName).FontSize(10).Alignment = Alignment.center;

                            // --- CỘT PHẢI: QUỐC HIỆU ---
                            var pRight = headerTable.Rows[0].Cells[1].Paragraphs[0];
                            pRight.Append("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\n").Bold().Font(fontName).FontSize(10).Alignment = Alignment.center;
                            pRight.Append("Độc lập - Tự do - Hạnh phúc\n").Bold().Font(fontName).FontSize(11).Alignment = Alignment.center;
                            pRight.Append("-----------------------").Font(fontName).FontSize(10).Alignment = Alignment.center;

                            // --- HÀNG 2 CỘT PHẢI: ĐỊA DANH, NGÀY THÁNG NĂM ---
                            var pDate = headerTable.Rows[1].Cells[1].Paragraphs[0];
                            pDate.Append($"TP. HCM, Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}")
                                 .Italic().Font(fontName).FontSize(11).Alignment = Alignment.center;

                            // Note: skipping explicit border removal to avoid Xceed color/Border API mismatches

                            // Chèn bảng tiêu ngữ vào vị trí đầu tiên của tài liệu
                            document.InsertTable(headerTable);


                            // --- TIÊU ĐỀ PHIẾU ---
                            var pTitle = document.InsertParagraph();
                            pTitle.AppendLine("\nPHIẾU KẾT QUẢ HỌC TẬP").Bold().FontSize(16)
                                  .AppendLine("(TRANSCRIPT OF ACADEMIC RECORD)").Italic().FontSize(12)
                                  .Alignment = Alignment.center;
                            pTitle.Font(fontName);

                            // --- THÔNG TIN SINH VIÊN ---
                            var pInfo = document.InsertParagraph();
                            pInfo.AppendLine($"- Họ và tên (Full name): {txtName.Text.Trim()}")
                                 .AppendLine($"- Mã số sinh viên (Student ID): {txtMSSV.Text.Trim()}")
                                 .AppendLine($"- Lớp học (Class): {cboClass.Text.Trim()}")
                                 .AppendLine("- Bậc đào tạo (Level): Đại học chính quy")
                                 .Alignment = Alignment.left;
                            pInfo.Font(fontName).FontSize(12);

                            // --- PHẦN KẾT QUẢ CHI TIẾT ---
                            var pDetailText = document.InsertParagraph();
                            pDetailText.AppendLine("\nKẾT QUẢ CHI TIẾT (DETAILS)").Bold()
                                       .Alignment = Alignment.left;
                            pDetailText.Font(fontName).FontSize(12);


                            // --- 4. TỰ VẼ BẢNG ĐIỂM ĐỘNG ---
                            // Đếm số dòng dữ liệu thực tế (bỏ dòng mới cuối Grid nếu có)
                            int dataRowCount = 0;
                            foreach (DataGridViewRow r in dgvScores.Rows) if (!r.IsNewRow) dataRowCount++;

                            // Tạo bảng: số hàng = số dòng dữ liệu + 1 hàng tiêu đề; số cột = 7
                            Table table = document.AddTable(dataRowCount + 1, 7);
                            table.Alignment = Alignment.center;

                            // Định dạng tiêu đề cột cho bảng và đổ màu xám nhạt làm nổi bật thanh tiêu đề
                            string[] headers = { "Mã Môn", "Tên Môn Học", "Số TC", "Điểm QT", "Điểm CK", "Điểm TK", "Mô tả" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                table.Rows[0].Cells[i].Paragraphs[0].Append(headers[i]).Bold().Font(fontName).FontSize(11);
                                // Skipping FillColor assignment to avoid cross-type color conversion issues with Xceed
                            }

                            // Đổ dữ liệu từ DataGridView vào bảng Word từng dòng một
                            int wordRowIndex = 1;
                            foreach (DataGridViewRow row in dgvScores.Rows)
                            {
                                if (row.IsNewRow) continue;

                                table.Rows[wordRowIndex].Cells[0].Paragraphs[0].Append((row.Cells[0].Value ?? "").ToString().Trim()); // Mã Môn
                                table.Rows[wordRowIndex].Cells[1].Paragraphs[0].Append((row.Cells[1].Value ?? "").ToString().Trim()); // Tên Môn
                                table.Rows[wordRowIndex].Cells[2].Paragraphs[0].Append((row.Cells[2].Value ?? "").ToString().Trim()); // Số TC
                                table.Rows[wordRowIndex].Cells[3].Paragraphs[0].Append((row.Cells[4].Value ?? "").ToString().Trim()); // Điểm QT
                                table.Rows[wordRowIndex].Cells[4].Paragraphs[0].Append((row.Cells[5].Value ?? "").ToString().Trim()); // Điểm CK
                                table.Rows[wordRowIndex].Cells[5].Paragraphs[0].Append((row.Cells[6].Value ?? "").ToString().Trim()); // Điểm TK
                                table.Rows[wordRowIndex].Cells[6].Paragraphs[0].Append((row.Cells[7].Value ?? "").ToString().Trim()); // Mô tả

                                // Thiết lập font đồng nhất cho hàng dữ liệu
                                for (int i = 0; i < 7; i++)
                                {
                                    table.Rows[wordRowIndex].Cells[i].Paragraphs[0].Font(fontName).FontSize(11);
                                }
                                wordRowIndex++;
                            }
                            document.InsertTable(table); // Chèn bảng vào file Word


                            // --- PHẦN TỔNG KẾT ---
                            string diemTB = label4.Text.Contains("\n") ? label4.Text.Split('\n')[1] : "0.00";
                            string tongSoTC = label5.Text.Contains("\n") ? label5.Text.Split('\n')[1] : "0";
                            string xepLoai = label6.Text.Contains("\n") ? label6.Text.Split('\n')[1] : "Chưa xếp loại";

                            var pSummary = document.InsertParagraph();
                            pSummary.AppendLine("\nTỔNG KẾT (SUMMARY)").Bold()
                                    .AppendLine($"- Tổng số tín chỉ tích lũy (Total credits): {tongSoTC}")
                                    .AppendLine($"- Điểm trung bình tích lũy (GPA): {diemTB}")
                                    .AppendLine($"- Xếp loại học lực (Rank): {xepLoai}")
                                    .Alignment = Alignment.left;
                            pSummary.Font(fontName).FontSize(12);

                            // --- KÝ TÊN ---
                            var pSign = document.InsertParagraph();
                            pSign.AppendLine("\nTRƯỜNG PHÒNG ĐÀO TẠO").Bold()
                                 .AppendLine("(Mẫu ký điện tử phòng Đào tạo HCMUTE)").Italic()
                                 .Alignment = Alignment.right;
                            pSign.Font(fontName).FontSize(11);

                            // 5. Lưu file thực tế xuống ổ đĩa
                            document.Save();
                        }

                        MessageBox.Show("Xuất bảng điểm ra file Word thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Tự động mở file Word vừa xuất để xem kết quả trực tiếp
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tạo và xuất file Word: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}