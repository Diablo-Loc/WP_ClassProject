using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace ClassProject
{
    internal static class Program
    {
        private static readonly string FlagFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "installed.flag");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Cấu hình WinForms chuẩn của Microsoft bắt buộc phải đặt ở ĐẦU HÀM MAIN
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if NET5_0_OR_GREATER
            ApplicationConfiguration.Initialize();
#endif

            // Gọi hàm kiểm tra Database và cấu hình hệ thống
            EnsureDatabaseExists();

            // Khởi chạy Form chính của bạn
            Application.Run(new LoginForm());
        }

        private static void EnsureDatabaseExists()
        {
            // 1. Kiểm tra file flag trước. Nếu đã tích "Không hiện lại" từ trước -> Vào thẳng App luôn
            if (File.Exists(FlagFilePath))
            {
                return;
            }

            // 2. Kiểm tra ngầm xem thực tế máy đã có Database 'LoginDB' chưa
            string checkConnectionString = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
            bool dbExists = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(checkConnectionString))
                {
                    conn.Open();
                    string query = "SELECT DB_ID('LoginDB')";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            dbExists = true; // Đã có DB thực tế
                        }
                    }
                }
            }
            catch
            {
                dbExists = false; // Không kết nối được hoặc chưa có DB
            }

            // 3. Nếu CHƯA CÓ database thực tế -> Mới hiện bảng hỏi
            if (!dbExists)
            {
                DialogResult userChoice = DialogResult.No;
                bool doNotShowAgain = false;

                // Tạo giao diện hộp thoại bằng code trực quan 
                using (Form prompt = new Form())
                {
                    prompt.Width = 600;
                    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                    prompt.Text = "Cấu hình Hệ thống & Cơ sở dữ liệu - UTEID";
                    prompt.StartPosition = FormStartPosition.CenterScreen;
                    prompt.MaximizeBox = false;
                    prompt.MinimizeBox = false;

                    // DÙNG RICHTEXTBOX THAY LABEL ĐỂ CHỐNG TRÀN CHỮ KHI ĐỔI MÁY / ĐỔI ĐỘ PHÂN GIẢI
                    RichTextBox textMessage = new RichTextBox()
                    {
                        Left = 20,
                        Top = 20,
                        Width = 540,
                        Height = 120, // Tăng hẳn chiều cao vùng chứa chữ
                        ReadOnly = true,
                        BorderStyle = BorderStyle.None,
                        BackColor = prompt.BackColor, // Trộn màu nền tệp với Form
                        ScrollBars = RichTextBoxScrollBars.None,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                        Text = "📌 Hệ thống phát hiện bạn chưa khởi tạo Cơ sở dữ liệu 'LoginDB'.\n\n" +
                               "1. Bạn có muốn chạy kịch bản Setup tự động ngay bây giờ không?\n\n" +
                               "2. Lưu ý quan trọng: Hãy đảm bảo bạn đã cấu hình chính xác API Key trong tệp 'secret.config' để có thể sử dụng toàn bộ tính năng trợ lý AI của ứng dụng."
                    };
                    prompt.Controls.Add(textMessage);

                    // Đặt vị trí CheckBox dựa trên đáy của hộp chữ
                    CheckBox chkDontShow = new CheckBox()
                    {
                        Left = 20,
                        Top = textMessage.Bottom + 10,
                        Width = 350,
                        Text = "Không hiển thị lại thông báo này lần sau",
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
                    };
                    prompt.Controls.Add(chkDontShow);

                    // Đặt vị trí các nút bấm
                    int btnTop = chkDontShow.Bottom + 15;
                    Button btnYes = new Button() { Text = "Có (Yes)", Left = 320, Width = 90, Top = btnTop, DialogResult = DialogResult.Yes };
                    Button btnNo = new Button() { Text = "Không (No)", Left = 420, Width = 90, Top = btnTop, DialogResult = DialogResult.No };

                    prompt.Controls.Add(btnYes);
                    prompt.Controls.Add(btnNo);

                    prompt.AcceptButton = btnYes;
                    prompt.CancelButton = btnNo;

                    // Tự động tính chiều cao Form ôm khít vừa vặn các nút bấm
                    prompt.Height = btnNo.Bottom + 55;

                    // Chờ người dùng click chọn nút
                    userChoice = prompt.ShowDialog();
                    doNotShowAgain = chkDontShow.Checked;
                }

                // 4. XỬ LÝ LỰA CHỌN CỦA NGƯỜI DÙNG (Đặt sau khi Form đã đóng hoàn toàn)
                if (userChoice == DialogResult.Yes)
                {
                    string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Run_SetupDB.ps1");

                    if (File.Exists(scriptPath))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "powershell.exe";
                        psi.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
                        psi.UseShellExecute = true;
                        psi.Verb = "runas";

                        try
                        {
                            Process proc = Process.Start(psi);
                            proc?.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Không thể chạy script tự động: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tệp kịch bản 'Run_SetupDB.ps1'!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // CHỈ TẠO FILE FLAG SAU KHI ĐÃ XỬ LÝ XONG LUỒNG CHẠY POWERSHELL
                if (doNotShowAgain)
                {
                    try
                    {
                        File.WriteAllText(FlagFilePath, "User skipped prompt.");
                    }
                    catch { /* Tránh crash do quyền ghi file */ }
                }
            }
        }
    }
}