using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace ClassProject.Services
{
    public class AzureOcrResult
    {
        public string Name { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
    }

    public class AzureOcrService
    {
        // Điền Endpoint và Key lấy từ Resource gói F0 của bạn vào đây
        private const string Endpoint = "https://YOUR_AZURE_REGION.api.cognitive.microsoft.com/";
        private const string ApiKey = "YOUR_AZURE_COGNITIVE_KEY";

        private readonly DocumentAnalysisClient _client;

        public AzureOcrService()
        {
            // Khởi tạo client kết nối an toàn bằng Key
            var credential = new AzureKeyCredential(ApiKey);
            _client = new DocumentAnalysisClient(new Uri(Endpoint), credential);
        }

        public async Task<AzureOcrResult> ScanCardAsync(string imagePath)
        {
            var result = new AzureOcrResult();
            try
            {
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    // Sử dụng Model chuyên dụng cho giấy tờ tùy thân (CCCD) của Azure
                    AnalyzeDocumentOperation operation = await _client.AnalyzeDocumentAsync(
                        WaitUntil.Completed,
                        "prebuilt-idDocument",
                        stream
                    );

                    AnalyzeResult analyzeResult = operation.Value;

                    // Nếu Azure nhận diện được cấu trúc thẻ (Thường là CCCD)
                    if (analyzeResult.Documents.Count > 0)
                    {
                        var document = analyzeResult.Documents[0];

                        // 1. Trích xuất Họ Tên (Ghép Họ + Tên đệm và Tên)
                        string firstName = document.Fields.TryGetValue("FirstName", out DocumentField firstNameField) ? firstNameField.Value.AsString() : string.Empty;
                        string lastName = document.Fields.TryGetValue("LastName", out DocumentField lastNameField) ? lastNameField.Value.AsString() : string.Empty;
                        result.Name = $"{lastName} {firstName}".Trim();

                        // 2. Trích xuất Ngày sinh
                        if (document.Fields.TryGetValue("DateOfBirth", out DocumentField dobField))
                        {
                            result.BirthDate = dobField.Value.AsDate().ToString("dd/MM/yyyy");
                        }

                        // 3. Trích xuất Số CCCD / Số ID
                        if (document.Fields.TryGetValue("DocumentNumber", out DocumentField idField))
                        {
                            result.StudentId = idField.Value.AsString();
                        }
                    }

                    // DỰ PHÒNG: Nếu quét THẺ SINH VIÊN (Azure không nhận diện theo form CCCD chuẩn)
                    // Hệ thống sẽ tự động quét chữ thô toàn bài (Read Model) và dùng toán tử Regex tìm kiếm như cũ
                    if (string.IsNullOrEmpty(result.StudentId))
                    {
                        string fullText = "";
                        foreach (var page in analyzeResult.Pages)
                        {
                            foreach (var line in page.Lines)
                            {
                                fullText += line.Content + "\n";
                            }
                        }

                        // Quét tìm chuỗi 8 số liên tiếp (MSSV của bạn)
                        var match = System.Text.RegularExpressions.Regex.Match(fullText, @"\b\d{8}\b");
                        if (match.Success) result.StudentId = match.Value;

                        // Quét tìm ngày sinh dạng dd/mm/yyyy trong văn bản thô nếu có
                        var dateMatch = System.Text.RegularExpressions.Regex.Match(fullText, @"\b\d{2}/\d{2}/\d{4}\b");
                        if (dateMatch.Success) result.BirthDate = dateMatch.Value;
                    }
                }
            }
            catch (RequestFailedException ex) when (ex.Status == 429)
            {
                // Xử lý khi vượt hạn mức gói F0 (Ví dụ bấm liên tục > 15 lần/phút)
                throw new InvalidOperationException("Hệ thống Free đang bận (Quá số lượt yêu cầu/phút). Vui lòng đợi vài giây rồi thử lại!", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi kết nối AI Cloud: " + ex.Message, ex);
            }

            return result;
        }
    }
}