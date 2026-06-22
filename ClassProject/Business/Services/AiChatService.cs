using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassProject.Business.Services
{
    public class AiChatService
    {
        // 1. Duy trì HttpClient duy nhất cho toàn hệ thống
        private static readonly HttpClient _httpClient = new HttpClient();

        // Cấu hình Endpoint ổn định v1beta
        private readonly string _geminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
        private readonly string _apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];

        // 2. TẠO HÀM DỰNG TĨNH ĐỂ CẤU HÌNH HTTPCLIENT ĐÚNG 1 LẦN DUY NHẤT
        static AiChatService()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(12); // Giới hạn timeout 12s chống treo luồng
        }

        /// <summary>
        /// Hàm xử lý Chat chính thức gửi lên Cloud AI Gemini sau khi đã lọc các dữ liệu rác
        /// </summary>
        public async Task<string> FetchAiResponseAsync(string question, bool isForgetScreen = false)
        {
            // 1. Kiểm tra các điều kiện chặn Spam cơ bản
            if (string.IsNullOrWhiteSpace(question))
                return "🤖 Tôi không nhận được nội dung câu hỏi. Hãy nhập gì đó nhé!";

            if (question.Length > 250)
                return "⚠️ Câu hỏi quá dài! Vui lòng tóm tắt ngắn gọn dưới 250 ký tự để tôi hỗ trợ tốt nhất.";

            string cleanQuestion = question.Trim();
            if (Regex.IsMatch(cleanQuestion, @"^[a-zA-Z0-9\s]+$") && double.TryParse(cleanQuestion, out _))
                return "🤖 Hệ thống không hiểu chuỗi số này. Vui lòng hỏi rõ nhu cầu của bạn.";

            if (Regex.IsMatch(cleanQuestion, @"^[^a-zA-Z0-9\s]+$"))
                return "🤖 Vui lòng không gửi các ký tự đặc biệt vô nghĩa.";

            // 2. Nếu là màn hình quên mật khẩu cố định (Bối cảnh đặc biệt)
            if (isForgetScreen)
            {
                return "🤖 Tại màn hình này, tôi chỉ hỗ trợ các vấn đề liên quan đến: Quy trình lấy lại mật khẩu, lỗi nhận mã OTP, và tiêu chuẩn an toàn mật khẩu. Vui lòng đặt câu hỏi liên quan.";
            }

            // 3. BẮN THẲNG LÊN GOOGLE GEMINI KHÔNG QUA BỘ LỌC CHỮ LOCAL NÀO NỮA
            return await CallCloudAiApiAsync(cleanQuestion);
        }

        /// <summary>
        /// Hàm kiểm tra phản hồi tĩnh local (Quy trình, OTP, Mật khẩu...) giúp phản hồi ngay lập tức
        /// </summary>
        public string CheckLocalStaticResponse(string question)
        {
            string rawQuery = question.Trim().ToLower();
            string cleanQuery = RemoveSignForVietnameseString(rawQuery);

            // Sử dụng Regex khớp chính xác toàn từ (\b) để không bị bắt nhầm khi từ khóa nằm trong câu dài
            if (Regex.IsMatch(cleanQuery, @"\b(quy trinh|cac buoc|lam the nao de lay lai mat khau)\b"))
                return "📋 Quy trình khôi phục mật khẩu gồm 3 bước:\n1. Nhập chính xác Email của bạn và nhấn 'Send OTP Verification'.\n2. Kiểm tra hộp thư, lấy mã nhập vào hộp thoại xác thực.\n3. Sau khi xác thực thành công, hệ thống mở khóa các ô nhập mật khẩu mới ngay trên màn hình.";

            if (Regex.IsMatch(cleanQuery, @"\b(otp|ma xac thuc|khong nhan duoc ma|loi gui mail|chua co ma)\b"))
                return "📧 Nếu bạn không nhận được mã OTP, vui lòng:\n• Kiểm tra kỹ xem địa chỉ Email nhập chính xác chưa.\n• Kiểm tra trong mục 'Thư rác' (Spam) hoặc 'Quảng cáo'.\n• Đảm bảo khoảng cách giữa các lần bấm gửi tối thiểu là 30 giây.";

            if (Regex.IsMatch(cleanQuery, @"\b(mat khau manh|do dai mat khau|tieu chuan mat khau)\b"))
                return "🛡️ Để đảm bảo an toàn, mật khẩu mới của bạn phải đạt tiêu chuẩn:\n• Tối thiểu từ 8 ký tự trở lên.\n• Chứa ít nhất 1 chữ cái viết HOA (A-Z).\n• Chứa ít nhất 1 chữ số (0-9).\n• Chứa ít nhất 1 ký tự đặc biệt (ví dụ: @, $, !, %).";

            if (Regex.IsMatch(cleanQuery, @"\b(bi khoa|tam khoa|tai khoan khoa|lockout)\b"))
                return "🔒 Nếu hệ thống báo tài khoản đang bị tạm khóa tự động, nghĩa là bạn đã nhập sai thông tin quá 5 lần. Hệ thống sẽ tự động mở khóa sau thời gian hiển thị trên màn hình.";

            if (Regex.IsMatch(cleanQuery, @"\b(cam on|thank|tot qua|xin chao|hello|hi)\b") && cleanQuery.Length <= 10)
                return "😊 Xin chào! Tôi có thể hỗ trợ gì cho bạn về hệ thống quản lý UTEID?";

            return null;
        }

        /// <summary>
        /// Hàm phân tích ý định điều hướng mở Form con - Đã sửa lỗi bắt nhầm chuỗi con vô nghĩa
        /// </summary>
        public (string FormName, string ResponseMessage) AnalyzeNavigationIntent(string question)
        {
            if (string.IsNullOrWhiteSpace(question) || question.Length > 100)
                return ("", "");

            string cleanQuery = RemoveSignForVietnameseString(question.Trim().ToLower());

            var formIntents = new Dictionary<string, string[]>
{
                // Bổ sung thêm từ "diem so" vào danh sách bên dưới
                { "ManageScoreForm", new[] { "diem", "diem so", "xem diem", "ket qua", "bang diem", "diem thi", "quan ly diem" } },

                { "ManageStudentForm", new[] { "sinh vien", "danh sach sv", "ho so", "thong tin sv", "sv" } },
                { "ManageCourseForm", new[] { "mon hoc", "tin chi", "hoc phan", "chuong trinh hoc", "quan ly mon" } },
                { "ManageClassroomForm", new[] { "lop hoc", "quan ly lop", "lop nien che" } },
                { "AccountManageForm", new[] { "tai khoan", "user", "admin", "cap tai khoan" } },
                { "StatisticsForm", new[] { "thong ke", "bieu do", "bao cao", "statistic" } },
                { "TranscriptForm", new[] { "bang diem ca nhan", "diem cua toi", "diem sv" } },
                { "ProfileForm", new[] { "thong tin ca nhan", "ho so cua toi", "profile" } },
                { "StudentRequestForm", new[] { "yeu cau ho tro", "gui yeu cau", "xin giay" } }
            };

            foreach (var intent in formIntents)
            {
                foreach (var keyword in intent.Value)
                {
                    // Tạo khuôn Regex khớp chính xác cụm từ hệ thống quy định độc lập trong câu
                    string pattern = $@"\b{Regex.Escape(keyword)}\b";

                    if (Regex.IsMatch(cleanQuery, pattern) || ComputeLevenshteinDistance(cleanQuery, keyword) <= 1)
                    {
                        return (intent.Key, $"🤖 AI: Nhận lệnh. Đang điều hướng bạn đến màn hình chức năng phù hợp...");
                    }
                }
            }
            return ("", "");
        }

        #region Thuật Toán Tối Ưu Tốc Độ Local
        private int ComputeLevenshteinDistance(string s, string t)
        {
            if (Math.Abs(s.Length - t.Length) > 2) return 99;
            int n = s.Length, m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) return m; if (m == 0) return n;
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        private string RemoveSignForVietnameseString(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ" };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "do", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y" };
            for (int i = 0; i < arr1.Length; i++)
            {
                str = str.Replace(arr1[i], arr2[i]);
                str = str.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return str;
        }

        private async Task<string> CallCloudAiApiAsync(string userQuestion)
        {
            try
            {
                string requestUrl = $"{_geminiUrl}?key={_apiKey}";
                // Thay thế đoạn cấu hình systemPrompt cũ bằng đoạn rõ ràng này:
                string systemPrompt = "Bạn là trợ lý AI phân tích dữ liệu học vụ có quyền truy cập cơ sở dữ liệu SQL Server của phần mềm UTEID.\n" +
                                      "Dưới đây là cấu trúc các bảng dữ liệu trong hệ thống của chúng tôi:\n" +
                                      "- Bảng SinhVien(MaSV VARCHAR(10) PRIMARY KEY, HoTen NVARCHAR(100), Lop NVARCHAR(20), NgaySinh DATE, GioiTinh NVARCHAR(10))\n" +
                                      "- Bảng Diem(MaSV VARCHAR(10), MaMon VARCHAR(10), DiemQuaTrinh FLOAT, DiemThi FLOAT, DiemTongKet FLOAT, PRIMARY KEY(MaSV, MaMon))\n" +
                                      "- Bảng MonHoc(MaMon VARCHAR(10) PRIMARY KEY, TenMon NVARCHAR(100), SoTinChi INT)\n\n" +
                                      "QUY ĐỊNH TRẢ LỜI QUAN TRỌNG:\n" +
                                      "1. Khi người dùng yêu cầu thống kê, lọc, tìm kiếm danh sách hoặc so sánh số liệu (Ví dụ có chứa các từ hoặc ký tự như: 'điểm trung bình < 8', 'top 5', 'dưới 5', 'qua môn'), " +
                                      "bạn BẮT BUỘC phải chuyển câu hỏi đó thành một câu lệnh SQL Server chính xác để truy vấn dữ liệu.\n" +
                                      "Không được phép từ chối, không được chê câu hỏi quá dài hay quá ngắn.\n" +
                                      "Định dạng kết quả trả về bắt buộc phải là: EXECUTE_SQL:[Câu_lệnh_SQL]. Không giải thích gì thêm.\n" +
                                      "Ví dụ: EXECUTE_SQL:SELECT TOP 5 MaSV, DiemTongKet FROM Diem WHERE DiemTongKet < 8 ORDER BY DiemTongKet DESC\n\n" +
                                      "2. Nếu người dùng ra lệnh mở màn hình trống (Ví dụ: 'mở form sinh viên'), trả về: OPEN_FORM:[Tên_Form].\n" +
                                      "3. Nếu là câu hỏi trò chuyện bình thường không liên quan đến dữ liệu, hãy trả lời ngắn gọn dưới 3 câu.";

                var requestData = new GeminiPayload
                {
                    Contents = new[] { new GeminiContentItem { Parts = new[] { new GeminiPartItem { Text = userQuestion } } } },
                    SystemInstruction = new GeminiSystemInstructionItem { Parts = new[] { new GeminiPartItem { Text = systemPrompt } } },
                    // Tăng nhẹ Temperature từ 0.2 lên 0.3 để tránh việc AI bị bó hẹp tư duy khi gặp ký tự toán học <, >
                    GenerationConfig = new GeminiGenerationConfig { Temperature = 0.3, MaxOutputTokens = 350 }
                };

                string jsonPayload = JsonConvert.SerializeObject(requestData);

                using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string rawResponse = await response.Content.ReadAsStringAsync();
                            dynamic jsonResult = JsonConvert.DeserializeObject(rawResponse);
                            string aiText = jsonResult.candidates[0].content.parts[0].text;
                            aiText = aiText.Trim();
                            return aiText.StartsWith("🤖") ? aiText : "🤖 AI: " + aiText;
                        }
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return $"❌ Lỗi API Google ({response.StatusCode}): {errorContent}";
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return "⏱️ Thời gian phản hồi từ AI vượt quá giới hạn an toàn. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                return $"⚠️ Lỗi kết nối hệ thống: {ex.Message}";
            }
        }
           
        public class GeminiPayload
        {
            [JsonProperty("contents")] public GeminiContentItem[] Contents { get; set; }
            [JsonProperty("system_instruction")] public GeminiSystemInstructionItem SystemInstruction { get; set; }
            [JsonProperty("generationConfig")] public GeminiGenerationConfig GenerationConfig { get; set; }
        }
        public class GeminiContentItem { [JsonProperty("parts")] public GeminiPartItem[] Parts { get; set; } }
        public class GeminiSystemInstructionItem { [JsonProperty("parts")] public GeminiPartItem[] Parts { get; set; } }
        public class GeminiPartItem { [JsonProperty("text")] public string Text { get; set; } }
        public class GeminiGenerationConfig { [JsonProperty("temperature")] public double Temperature { get; set; } [JsonProperty("maxOutputTokens")] public int MaxOutputTokens { get; set; } }
        #endregion
    }
}