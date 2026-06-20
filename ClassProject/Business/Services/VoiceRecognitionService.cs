using System;
using System.Speech.Recognition;
using System.Text.RegularExpressions;

namespace ClassProject.Services
{
    public class VoiceRecognitionService : IDisposable
    {
        private SpeechRecognitionEngine _recognizer;
        private bool _isListening = false;
        public bool IsRecognizerAvailable() => _recognizer != null;
        // Định nghĩa 1 Event để báo cho Form biết khi đã bóc tách xong dữ liệu độc lập
        public event Action<string, string> OnStudentDataParsed;

        // Định nghĩa Event để đồng bộ trạng thái giao diện (Đang nghe / Dừng)
        public event Action<bool> OnListeningStatusChanged;

        public bool IsListening => _isListening;

        public VoiceRecognitionService()
        {
            InitVoiceRecognition();
        }

        private void InitVoiceRecognition()
        {
            try
            {
                // 1. Kiểm tra xem máy tính có cài bộ nhận diện giọng nói nào không
                var installedRecognizers = SpeechRecognitionEngine.InstalledRecognizers();

                if (installedRecognizers.Count == 0)
                {
                    // Không có bộ nhận diện nào -> Hủy cấu hình engine, không ném lỗi làm sập app
                    _recognizer = null;
                    return;
                }

                // 2. Nếu có, tự động lấy bộ nhận diện đầu tiên tìm thấy (thường là tiếng Anh en-US)
                var defaultRecognizerInfo = installedRecognizers[0];
                _recognizer = new SpeechRecognitionEngine(defaultRecognizerInfo.Culture);

                // 3. Cấu hình thiết bị đầu vào (Mic)
                _recognizer.SetInputToDefaultAudioDevice();

                // 4. Nạp cấu ngữ pháp tự do
                DictationGrammar dictationGrammar = new DictationGrammar();
                _recognizer.LoadGrammar(dictationGrammar);

                // 5. Đăng ký sự kiện
                _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }
            catch (Exception ex)
            {
                // Bảo vệ app khỏi crash nếu lỗi phần cứng Mic
                _recognizer = null;
                System.Diagnostics.Debug.WriteLine("Voice Recognition Init Failed: " + ex.Message);
            }
        }

        public void ToggleListening()
        {
            if (_recognizer == null) return;

            if (!_isListening)
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                _isListening = true;
                OnListeningStatusChanged?.Invoke(true);
            }
            else
            {
                StopListening();
            }
        }

        public void StopListening()
        {
            if (_recognizer != null && _isListening)
            {
                _recognizer.RecognizeAsyncStop();
                _isListening = false;
                OnListeningStatusChanged?.Invoke(false);
            }
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speechText = e.Result.Text;
            if (string.IsNullOrWhiteSpace(speechText)) return;

            string lowerText = speechText.ToLower();
            string mssv = string.Empty;
            string hoTen = string.Empty;

            // 1. Tìm MSSV (8 chữ số liên tiếp)
            var mssvMatch = Regex.Match(speechText, @"\b\d{8}\b");
            if (mssvMatch.Success)
            {
                mssv = mssvMatch.Value.Trim();
            }

            // 2. Tìm Họ Tên
            string patternName = @"(?<=sinh vien |sinh viên |student )(.*?)(?= mssv|ma so|mã số|id|$)";
            var nameMatch = Regex.Match(lowerText, patternName);
            if (nameMatch.Success && !string.IsNullOrWhiteSpace(nameMatch.Value))
            {
                int startIdx = nameMatch.Index;
                int length = nameMatch.Length;
                hoTen = speechText.Substring(startIdx, length).Trim();
            }

            // Bắn dữ liệu đã bóc tách về Form qua Event công khai
            OnStudentDataParsed?.Invoke(hoTen, mssv);

            // Tự động tắt mic sau khi nhận diện thành công 1 câu lệnh
            StopListening();
        }

        public void Dispose()
        {
            if (_recognizer != null)
            {
                _recognizer.SpeechRecognized -= Recognizer_SpeechRecognized;
                _recognizer.Dispose();
            }
        }
    }
}