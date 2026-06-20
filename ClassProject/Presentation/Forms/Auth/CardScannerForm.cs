using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tesseract;

namespace ClassProject.Presentation.Forms.Auth
{
    // Định nghĩa 2 chế độ hoạt động của Form quét giống hệt bản Azure cũ
    public enum ScannerMode
    {
        OnlyMSSV,     // Chỉ lấy MSSV (Dùng cho Register)
        FullInfo      // Lấy đầy đủ MSSV, Họ tên, Ngày sinh (Dùng cho Manage)
    }

    public partial class CardScannerForm : Form
    {
        private readonly ScannerMode _currentMode;
        private readonly string _tessdataPath;

        // Các thuộc tính công khai để Form gốc (Register/Manage) đọc kết quả
        public string DetectedMSSV { get; private set; } = string.Empty;
        public string DetectedName { get; private set; } = string.Empty;
        public string DetectedDOB { get; private set; } = string.Empty;

        public CardScannerForm(ScannerMode mode = ScannerMode.FullInfo)
        {
            InitializeComponent();
            _currentMode = mode;

            // Đường dẫn đến thư mục chứa file dữ liệu eng.traineddata
            _tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            btnConfirm.Enabled = false;
            ApplyInterfaceMode(); // Tự động căn chỉnh giao diện theo cấu hình Mode
        }

        /// <summary>
        /// Tự động ẩn hoặc hiện các ô nhập liệu tùy theo chế độ được gọi
        /// </summary>
        private void ApplyInterfaceMode()
        {
            if (_currentMode == ScannerMode.OnlyMSSV)
            {
                if (txtDetectedName != null) txtDetectedName.Visible = false;
                if (txtDetectedDOB != null) txtDetectedDOB.Visible = false;
                if (lblFieldName != null) lblFieldName.Visible = false;
                if (lblFieldDOB != null) lblFieldDOB.Visible = false;

                this.Text = "Quét thẻ - Nhận diện MSSV (Offline)";
            }
            else
            {
                if (txtDetectedName != null) txtDetectedName.Visible = true;
                if (txtDetectedDOB != null) txtDetectedDOB.Visible = true;
                if (lblFieldName != null) lblFieldName.Visible = true;
                if (lblFieldDOB != null) lblFieldDOB.Visible = true;

                this.Text = "Quét thẻ - Nhận diện thông tin sinh viên (Offline)";
            }
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
                ofd.Title = "Chọn ảnh thẻ Sinh viên để quét";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxCard.Image = Image.FromFile(ofd.FileName);
                    pictureBoxCard.SizeMode = PictureBoxSizeMode.Zoom;

                    this.Cursor = Cursors.WaitCursor;
                    btnSelectImage.Enabled = false;

                    try
                    {
                        // Chạy quét Text toàn diện bằng Tesseract
                        ExtractInfoFromImage(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        ClearFields();
                        MessageBox.Show($"Lỗi xử lý OCR: {ex.Message}\n\nHệ thống cho phép bạn tự điền tay vào ô.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                        btnSelectImage.Enabled = true;
                    }
                }
            }
        }

        private void ExtractInfoFromImage(string imagePath)
        {
            if (!Directory.Exists(_tessdataPath))
            {
                throw new DirectoryNotFoundException($"Không tìm thấy thư mục cấu hình tại: {_tessdataPath}. Vui lòng tạo thư mục này và bỏ file 'eng.traineddata' vào.");
            }

            using (var engine = new TesseractEngine(_tessdataPath, "vie", EngineMode.Default))
            {
                using (var bitmap = new Bitmap(imagePath))
                {
                    // Nếu ảnh bị ngược thì mở dòng này, nếu ảnh đã xuôi sẵn thì hãy comment (//) nó lại nhé
                    // bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                        using (var img = Pix.LoadFromMemory(stream.ToArray()))
                        {
                            using (var page = engine.Process(img))
                            {
                                // Lấy TOÀN BỘ dữ liệu chữ thô mà AI quét được trên thẻ
                                string fullText = page.GetText();
                                System.Diagnostics.Debug.WriteLine($"--- RAW TEXT TESSERACT ---\n{fullText}");

                                // 1. Trích xuất thử bằng Regex để gợi ý điền vào ô trước
                                string rawMssv = ParseMSSV(fullText);
                                string rawName = (_currentMode == ScannerMode.FullInfo) ? ParseFullNameVietnamese(fullText) : "";
                                string rawDob = (_currentMode == ScannerMode.FullInfo) ? ParseBirthDate(fullText) : "";

                                // 2. HIỂN THỊ HỘP THOẠI CHỨA TOÀN BỘ CHỮ THÔ CỦA THẺ
                                string reviewMessage = "=== TOÀN BỘ DỮ LIỆU CHỮ THÔ AI QUÉT ĐƯỢC ===\n\n" +
                                                       (string.IsNullOrEmpty(fullText.Trim()) ? "(Không quét được chữ nào, ảnh có thể bị mờ hoặc ngược!)" : fullText) +
                                                       "\n==========================================\n\n" +
                                                       "Bấm OK để đổ các trường gợi ý lên Form và tự điều chỉnh lại!";

                                // Hiện bảng thông báo chứa "Raw Text" cho bạn xem trước
                                //MessageBox.Show(reviewMessage, "Kết Quả Quét Chữ Thô Từ Thẻ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // 3. ĐỔ DỮ LIỆU LÊN FORM
                                txtDetectedResult.Text = rawMssv;

                                if (_currentMode == ScannerMode.FullInfo)
                                {
                                    txtDetectedName.Text = rawName;
                                    txtDetectedDOB.Text = rawDob;
                                }

                                // Bật nút xác nhận nếu ô kết quả MSSV có chữ
                                btnConfirm.Enabled = !string.IsNullOrEmpty(txtDetectedResult.Text);
                            }
                        }
                    }
                }
            }
        }

        #region Các hàm lọc Text bằng Regex

        private string ParseMSSV(string text)
        {
            // Regex 1: Tìm chữ "ID" kèm dãy 8 số phía sau
            var match = Regex.Match(text, @"ID[\s/:]*(\d{8})", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;

            // Regex 2: Dự phòng tìm dãy số dạng 24xxxxxx hoặc bị nhận nhầm thành 54xxxxxx
            var backupMatch = Regex.Match(text, @"\b[25]4\d{6}\b");
            if (backupMatch.Success)
            {
                string result = backupMatch.Value;
                if (result.StartsWith("54")) result = "24" + result.Substring(2); // Nắn lại số nhiễu
                return result;
            }
            return string.Empty;
        }

        private string ParseFullNameVietnamese(string text)
        {
            // Tách văn bản thành từng dòng
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Danh sách từ khóa hệ thống cần loại bỏ hoàn toàn
            string blacklist = "THẺ|SINH|VIÊN|CARD|STUDENT|UNIVERSITY|FACULTY|HIỆU|LỰC|NGÀNH|HỌC|CÔNG|NGHỆ|THÔNG|TIN";

            foreach (string line in lines)
            {
                string cleanLine = line.Trim();

                // Kiểm tra dòng chữ viết hoa toàn bộ (Có hỗ trợ đầy đủ bộ dấu Tiếng Việt)
                if (Regex.IsMatch(cleanLine, @"^[A-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠỨỨỬỰỂỆỞỘỚỜỞỊỊÝỲỸỶỸ\s]+$"))
                {
                    // Nếu độ dài quá ngắn hoặc chứa từ khóa nằm trong danh sách đen thì bỏ qua
                    if (cleanLine.Length < 5 || Regex.IsMatch(cleanLine, blacklist, RegexOptions.IgnoreCase))
                    {
                        continue;
                    }

                    return cleanLine; // Trả về dòng họ tên chuẩn đầu tiên tìm thấy
                }
            }
            return string.Empty;
        }

        private string ParseBirthDate(string text)
        {
            // Tìm định dạng ngày sinh chuẩn phổ biến: dd/mm/yyyy hoặc dd-mm-yyyy
            var match = Regex.Match(text, @"\b\d{2}[/\-]\d{2}[/\-]\d{4}\b");
            return match.Success ? match.Value : "";
        }

        #endregion

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Ép text box lưu ngược lại biến toàn cục để Form cha bên ngoài lấy sử dụng
            this.DetectedMSSV = txtDetectedResult.Text.Trim();

            if (_currentMode == ScannerMode.FullInfo)
            {
                this.DetectedName = txtDetectedName.Text.Trim();
                this.DetectedDOB = txtDetectedDOB.Text.Trim();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ClearFields()
        {
            txtDetectedResult.Clear();
            if (txtDetectedName != null) txtDetectedName.Clear();
            if (txtDetectedDOB != null) txtDetectedDOB.Clear();
            btnConfirm.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}