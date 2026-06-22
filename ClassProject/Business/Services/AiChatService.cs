using ClassProject.DataAccess.Entities;
using Emgu.CV.ML;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
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
        public async Task<string> FetchAiResponseAsync(string question, bool isForgetScreen = false, bool isSystemInternalCall = false)
        {
            if (string.IsNullOrWhiteSpace(question))
                return "🤖 Tôi không nhận được nội dung câu hỏi. Hãy nhập gì đó nhé!";

            // NẾU KHÔNG PHẢI CUỘC GỌI NỘI BỘ THÌ MỚI CHECK SPAM NGƯỜI DÙNG
            if (!isSystemInternalCall)
            {
                if (question.Length > 250)
                    return "⚠️ Câu hỏi quá dài! Vui lòng tóm tắt ngắn gọn dưới 250 ký tự để tôi hỗ trợ tốt nhất.";

                string cleanQuestion = question.Trim();
                if (Regex.IsMatch(cleanQuestion, @"^[a-zA-Z0-9\s]+$") && double.TryParse(cleanQuestion, out _))
                    return "🤖 Hệ thống không hiểu chuỗi số này. Vui lòng hỏi rõ nhu cầu của bạn.";

                if (Regex.IsMatch(cleanQuestion, @"^[^a-zA-Z0-9\s]+$"))
                    return "🤖 Vui lòng không gửi các ký tự đặc biệt vô nghĩa.";

                if (isForgetScreen)
                {
                    return "🤖 Tại màn hình này, tôi chỉ hỗ trợ các vấn đề liên quan đến: Quy trình lấy lại mật khẩu, lỗi nhận mã OTP, và tiêu chuẩn an toàn mật khẩu. Vui lòng đặt câu hỏi liên quan.";
                }
            }

            // BẮN THẲNG LÊN GOOGLE GEMINI KHÔNG QUA BỘ LỌC KHÁC
            return await CallCloudAiApiAsync(question.Trim());
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

            // Định nghĩa các từ khóa kích hoạt hành động mở form
            string[] actionPrefixes = { "mo", "vao", "chuyen den", "den", "open", "go to" };

            foreach (var intent in formIntents)
            {
                foreach (var keyword in intent.Value)
                {
                    // Kiểm tra 1: Khớp chuẩn xác và tuyệt đối (Ví dụ gõ đúng chữ "sinh vien" hoặc "sinh vien ")
                    if (cleanQuery == keyword || ComputeLevenshteinDistance(cleanQuery, keyword) <= 1)
                    {
                        return (intent.Key, $"🤖 AI: Nhận lệnh. Đang điều hướng bạn đến màn hình chức năng phù hợp...");
                    }

                    // Kiểm tra 2: Có chứa từ khóa hành động đi kèm (Ví dụ: "mo form sinh vien", "vao muc diem")
                    foreach (var prefix in actionPrefixes)
                    {
                        if (cleanQuery.StartsWith(prefix + " " + keyword) || cleanQuery.Contains(" " + prefix + " " + keyword))
                        {
                            return (intent.Key, $"🤖 AI: Nhận lệnh hành động. Đang mở màn hình...");
                        }
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

                // ============================================================================
                // TÁI CẤU TRÚC SYSTEM PROMPT CHUẨN XÁC THEO SCRIPT DATABASE THỰC TẾ (LOGINDB)
                // ============================================================================
                string systemPrompt =
                    "Bạn là trợ lý AI phân tích dữ liệu học vụ trường HCMUTE, kết nối trực tiếp với SQL Server (Database: LoginDB).\n\n" +
                    "Dưới đây là cấu trúc bảng CHÍNH XÁC 100% trong hệ thống, bắt buộc phải viết đúng tên bảng và tên cột khi sinh câu lệnh SQL:\n" +
                    "- Bảng sinh viên: dbo.Students (Id, UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Email, MaLop, MaNganh)\n" +
                    "- Bảng điểm HP: dbo.Score (MSSV, MaLopHP, DiemQT, DiemCK, DiemTK, Mota)\n" +
                    "- Bảng môn học: dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota)\n" +
                    "- Bảng lớp học phần: dbo.CourseSection (MaLopHP, MaMH, HocKy, NamHoc, MSGV, PhongHoc, MaxStudents, ThuHoc, CaHoc)\n" +
                    "- Bảng lớp hành chính: dbo.Classroom (MaLop, TenLop, SiSo, GVCN, MaNganh)\n\n" +
                    "CÁC VIEW CÓ SẴN (Ưu tiên SELECT từ đây nếu câu hỏi cần thông tin tổng hợp điểm, lịch học):\n" +
                    "* View bảng điểm đầy đủ: dbo.vw_StudentTranscript (MSSV, StudentName, MaLopHP, MaMH, TenMH, SoTC, DiemQT, DiemCK, DiemTK, NamHoc, HocKy)\n" +
                    "* View lịch học hằng ngày: dbo.vw_StudentDailySchedule (MSSV, MaLopHP, TenMH, PhongHoc, ThuHoc, CaHoc, ThoiGian)\n\n" +
                    "⚠️ QUY TẮC SẢN SINH SQL BẮT BUỘC (QUAN TRỌNG):\n" +
                    "1. Tuyệt đối KHÔNG ĐƯỢC dùng các tên bảng tự đoán như 'SinhVien', 'Diem', 'MonHoc'. Phải dùng chính xác tiền tố dbo. và tên bảng tiếng Anh ở trên.\n" +
                    "2. Cột 'MaMH' và 'MaNganh' dùng kiểu CHAR(10) nên chứa khoảng trắng thừa ở cuối. Khi lọc theo 2 cột này, BẮT BUỘC phải dùng hàm TRIM() hoặc RTRIM(). Ví dụ: WHERE TRIM(MaNganh) = 'CNTT' hoặc WHERE TRIM(MaMH) = 'ANM004'.\n" +
                    "3. Khi tìm theo tên Sinh viên, hãy dùng phép cộng chuỗi: (FirstName + ' ' + LastName) LIKE N'%Tên_Cần_Tìm%'.\n\n" +
                    "QUY ĐỊNH TRẢ LỜI:\n" +
                    "- Nếu người dùng yêu cầu thống kê, lọc, tra cứu, hoặc đếm dữ liệu, bạn CHỈ ĐƯỢC PHÉP trả về chuỗi có định dạng duy nhất: EXECUTE_SQL:[Câu_lệnh_SQL_Server_ở_đây]\n" +
                    "- Tuyệt đối không bọc câu lệnh trong ký tự markdown như ```sql ... ```. Không giải thích dông dài.\n" +
                    "- Nếu là câu hỏi chào hỏi xã giao bình thường không liên quan đến dữ liệu, trả lời ngắn gọn không quá 2 câu." +
                    "4.Khi viết các câu lệnh đếm dữ liệu(COUNT), tính trung bình(AVG), hoặc tính tổng(SUM), BẮT BUỘC phải đặt tên định danh dễ hiểu cho cột bằng từ khóa 'As'. Ví dụ đúng: SELECT COUNT(*) As[Số lượng sinh viên] FROM dbo.Students";
                var requestData = new GeminiPayload
                {
                    Contents = new[] { new GeminiContentItem { Parts = new[] { new GeminiPartItem { Text = userQuestion } } } },
                    SystemInstruction = new GeminiSystemInstructionItem { Parts = new[] { new GeminiPartItem { Text = systemPrompt } } },
                    GenerationConfig = new GeminiGenerationConfig { Temperature = 0.2, MaxOutputTokens = 350 }
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

                            // Để đồng bộ với tầng WinForms xử lý cắt chuỗi EXECUTE_SQL:, ta loại bỏ tiền tố tự thêm ngẫu nhiên
                            if (aiText.StartsWith("🤖 AI: "))
                            {
                                aiText = aiText.Substring("🤖 AI: ".Length).Trim();
                            }
                            else if (aiText.StartsWith("🤖"))
                            {
                                aiText = aiText.Substring(1).Trim();
                            }

                            return aiText;
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