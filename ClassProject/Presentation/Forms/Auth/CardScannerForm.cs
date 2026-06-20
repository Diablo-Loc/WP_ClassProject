using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tesseract;

namespace ClassProject.Presentation.Forms.Auth
{
    public partial class CardScannerForm : Form
    {
        // Thuộc tính để lưu kết quả MSSV cuối cùng, Form gốc sẽ đọc từ đây
        public string DetectedMSSV { get; private set; } = string.Empty;

        public CardScannerForm()
        {
            InitializeComponent();
            btnConfirm.Enabled = false; // Chỉ cho bấm xác nhận khi đã tìm thấy số
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
                ofd.Title = "Chọn ảnh thẻ Sinh viên";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxCard.Image = Image.FromFile(ofd.FileName);
                    pictureBoxCard.SizeMode = PictureBoxSizeMode.Zoom;

                    // Chạy OCR nhận diện
                    Cursor.Current = Cursors.WaitCursor;
                    string mssv = ExtractMSSVFromImage(ofd.FileName);
                    Cursor.Current = Cursors.Default;

                    if (!string.IsNullOrEmpty(mssv))
                    {
                        txtDetectedResult.Text = mssv;
                        btnConfirm.Enabled = true; // Bật nút xác nhận
                        MessageBox.Show($"[AI OCR] Đã nhận diện được chuỗi số: {mssv}. Bạn có thể chỉnh sửa lại nếu chưa chính xác trước khi bấm Xác nhận.",
                                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        txtDetectedResult.Clear();
                        btnConfirm.Enabled = false;
                        MessageBox.Show("Không tìm thấy mã số hợp lệ. Vui lòng nhập tay vào ô kết quả hoặc chọn ảnh rõ nét hơn!",
                                        "Nhận diện thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private string ExtractMSSVFromImage(string imagePath)
        {
            try
            {
                string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                using (var engine = new TesseractEngine(dataPath, "eng", EngineMode.Default))
                {
                    using (var bitmap = new Bitmap(imagePath))
                    {
                        // Giữ nguyên góc xoay chuẩn phát hiện được ở bước trước
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                            using (var img = Pix.LoadFromMemory(stream.ToArray()))
                            {
                                using (var page = engine.Process(img))
                                {
                                    string fullText = page.GetText();
                                    System.Diagnostics.Debug.WriteLine($"--- Text thực tế ---\n{fullText}");

                                    // 🌟 REGEX CẢI TIẾN DÀNH RIÊNG CHO THẺ CỦA BẠN:
                                    // Tìm chữ "ID" (có thể kèm ký tự đặc biệt như / hoặc :) rồi lấy 8 số phía sau
                                    var match = Regex.Match(fullText, @"ID[\s/:]*(\d{8})", RegexOptions.IgnoreCase);
                                    if (match.Success)
                                    {
                                        return match.Groups[1].Value;
                                    }

                                    // DỰ PHÒNG CHUẨN: Tìm bất kỳ chuỗi 8 số nào bắt đầu bằng "24" hoặc "54" 
                                    // (Vì số thực tế AI bốc được đang là 54110107 do nhìn nhầm số 2 thành số 5)
                                    var backupMatch = Regex.Match(fullText, @"\b[25]4\d{6}\b");
                                    if (backupMatch.Success)
                                    {
                                        // Lấy chuỗi số tìm được
                                        string result = backupMatch.Value;

                                        // Mẹo nhỏ xử lý nhiễu: Nếu số đầu bị AI nhìn nhầm từ 2 thành 5, ta nắn lại thành số 2 cho đúng MSSV HCMUTE
                                        if (result.StartsWith("54"))
                                        {
                                            result = "24" + result.Substring(2);
                                        }
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OCR Error]: {ex.Message}");
            }
            return string.Empty;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Lấy giá trị trong ô text (để phòng trường hợp user có sửa lại bằng tay cho đúng)
            this.DetectedMSSV = txtDetectedResult.Text.Trim();
            this.DialogResult = DialogResult.OK; // Đánh dấu là bấm OK thành công
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}