using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace ClassProject.Business.Services // Thay đổi namespace cho đúng project của bạn
{
    public class FaceIdService : IDisposable
    {
        private EigenFaceRecognizer _recognizer = null;
        private CascadeClassifier _faceDetector = null;
        private List<Image<Gray, byte>> _trainedFaces = new List<Image<Gray, byte>>();
        private List<string> _trainedLabels = new List<string>();
        private List<int> _trainedIntLabels = new List<int>();

        public FaceIdService()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string xmlPath = Path.Combine(exeDir, "haarcascade_frontalface_default.xml");

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException($"Không tìm thấy file cấu hình AI: {xmlPath}");

            _faceDetector = new CascadeClassifier(xmlPath);
        }

        // Kiểm tra khuôn mặt trong khung hình
        public Rectangle[] DetectFaces(Image<Gray, byte> grayFrame)
        {
            return _faceDetector.DetectMultiScale(grayFrame, 1.2, 10, new Size(50, 50));
        }

        // Huấn luyện AI từ thư mục ảnh mẫu
        public bool InitializeAI(string loginUsername, Action<string> UIWarningNotifier)
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string trainedPath = Path.Combine(exeDir, "TrainedFaces");

                if (!Directory.Exists(trainedPath)) Directory.CreateDirectory(trainedPath);

                string[] files = Directory.GetFiles(trainedPath, "*.jpg");

                List<string> userFiles = new List<string>();
                List<string> negativeFiles = new List<string>(); // 🌟 THÊM: Chứa ảnh của người khác làm đối chứng

                foreach (string file in files)
                {
                    string rawName = Path.GetFileNameWithoutExtension(file);
                    if (rawName.Contains("_")) rawName = rawName.Split('_')[0];

                    if (rawName.Equals(loginUsername, StringComparison.OrdinalIgnoreCase))
                    {
                        userFiles.Add(file);
                    }
                    else
                    {
                        negativeFiles.Add(file); // Gom các ảnh của tài khoản khác lại
                    }
                }

                if (userFiles.Count == 0)
                {
                    UIWarningNotifier?.Invoke($"Tài khoản [{loginUsername}] chưa từng thiết lập dữ liệu Face ID!\r\n\r\nVui lòng đăng nhập bằng mật khẩu thường để cài đặt.");
                    return false;
                }

                _trainedFaces.Clear();
                _trainedLabels.Clear();
                _trainedIntLabels.Clear();

                int currentId = 0;

                // 1. Nạp ảnh của chính User cần đăng nhập (Nhãn: 0)
                foreach (string file in userFiles)
                {
                    try
                    {
                        Image<Gray, byte> faceImg = new Image<Gray, byte>(file).Resize(200, 200, Inter.Cubic);
                        _trainedFaces.Add(faceImg);
                        _trainedLabels.Add(loginUsername); // Nhãn chuỗi thật
                        _trainedIntLabels.Add(0);          // Mã hóa số nguyên cố định cho user này
                    }
                    catch { continue; }
                }

                // 2. QUAN TRỌNG: Lấy tối đa 5 tấm ảnh của người khác làm mẫu đối chứng (Nhãn: 1)
                int negativeCount = 0;
                foreach (string file in negativeFiles)
                {
                    if (negativeCount >= 5) break; // Chỉ cần tối đa 5 tấm để AI làm mốc phân biệt
                    try
                    {
                        Image<Gray, byte> faceImg = new Image<Gray, byte>(file).Resize(200, 200, Inter.Cubic);
                        _trainedFaces.Add(faceImg);
                        _trainedLabels.Add("UNKNOWN_FACE"); // Nhãn người lạ
                        _trainedIntLabels.Add(1);           // Mã hóa số nguyên cố định cho tập đối chứng
                        negativeCount++;
                    }
                    catch { continue; }
                }

                if (_trainedFaces.Count > 0)
                {
                    // Khởi tạo bộ nhận diện
                    _recognizer = new EigenFaceRecognizer();
                    using (VectorOfMat vectorOfMat = new VectorOfMat())
                    using (VectorOfInt vectorOfIds = new VectorOfInt())
                    {
                        foreach (var faceImg in _trainedFaces) { vectorOfMat.Push(faceImg.Mat); }
                        vectorOfIds.Push(_trainedIntLabels.ToArray());
                        _recognizer.Train(vectorOfMat, vectorOfIds);
                    }
                    return true;
                }

                UIWarningNotifier?.Invoke("Các file ảnh mẫu hiện tại bị lỗi cấu trúc dữ liệu!");
                return false;
            }
            catch (Exception ex)
            {
                UIWarningNotifier?.Invoke("Lỗi cấu hình AI: " + ex.Message);
                return false;
            }
        }

        // Dự đoán xem mặt này là của ai
        public string PredictOwner(Image<Gray, byte> faceResult, double maxDistance = 2800)
        {
            if (_recognizer == null || _trainedLabels.Count == 0) return null;

            try
            {
                var result = _recognizer.Predict(faceResult);

                // Nếu tìm thấy nhãn hợp lệ, khoảng cách nằm trong ngưỡng an toàn 
                // VÀ nhãn đó index trỏ tới không phải là người lạ (UNKNOWN_FACE)
                if (result.Label >= 0 && result.Label < _trainedLabels.Count && result.Distance < maxDistance)
                {
                    string predictionName = _trainedLabels[result.Label];

                    if (predictionName.Equals("UNKNOWN_FACE", StringComparison.OrdinalIgnoreCase))
                    {
                        return null; // Nhận diện ra là người lạ -> Trả về null chặn đăng nhập công cụ
                    }

                    return predictionName;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        // Lưu ảnh đăng ký mới mã hóa an toàn bằng Bitmap chuẩn
        public void SaveRegisterFace(Image<Gray, byte> grayFrame, Rectangle faceRect, string username)
        {
            using (Image<Gray, byte> faceCrop = grayFrame.Copy(faceRect).Resize(200, 200, Inter.Cubic))
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string trainedPath = Path.Combine(exeDir, "TrainedFaces");

                if (!Directory.Exists(trainedPath)) Directory.CreateDirectory(trainedPath);

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string savePath = Path.Combine(trainedPath, $"{username}_{timestamp}.jpg");

                using (Bitmap bmpSave = faceCrop.ToBitmap())
                {
                    bmpSave.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        public void Dispose()
        {
            _faceDetector?.Dispose();
            _recognizer?.Dispose();
            foreach (var face in _trainedFaces) face.Dispose();
        }
    }
}