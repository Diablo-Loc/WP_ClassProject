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

                if (!Directory.Exists(trainedPath))
                {
                    Directory.CreateDirectory(trainedPath);
                }

                // 1. Lấy tất cả file ảnh đang có trong thư mục
                string[] files = Directory.GetFiles(trainedPath, "*.jpg");

                // 2. Lọc riêng ra những ảnh thuộc về Username đang chuẩn bị Đăng nhập
                List<string> userFiles = new List<string>();
                foreach (string file in files)
                {
                    string rawName = Path.GetFileNameWithoutExtension(file);
                    if (rawName.Contains("_")) rawName = rawName.Split('_')[0];

                    // So sánh không phân biệt hoa thường
                    if (rawName.Equals(loginUsername, StringComparison.OrdinalIgnoreCase))
                    {
                        userFiles.Add(file);
                    }
                }

                // 3. ĐẶC BIỆT: Nếu danh sách trống -> Student này chưa đăng ký Face ID -> Báo lỗi cụ thể ngay lập tức!
                if (userFiles.Count == 0)
                {
                    UIWarningNotifier?.Invoke($"Tài khoản [{loginUsername}] chưa từng thiết lập dữ liệu Face ID trên hệ thống!\r\n\r\nVui lòng đăng nhập bằng mật khẩu thường và vào phần cá nhân để cài đặt.");
                    return false;
                }

                _trainedFaces.Clear();
                _trainedLabels.Clear();
                _trainedIntLabels.Clear();
                int idCounter = 0;

                // 4. Thay vì train hết toàn trường, AI bây giờ chỉ nạp duy nhất tập ảnh của chính User này để so khớp 1:1
                foreach (string file in userFiles)
                {
                    try
                    {
                        if (new FileInfo(file).Length == 0) continue;

                        using (Mat testMat = CvInvoke.Imread(file, ImreadModes.Grayscale))
                        {
                            if (testMat.IsEmpty) continue;
                        }

                        Image<Gray, byte> faceImg = new Image<Gray, byte>(file).Resize(200, 200, Inter.Cubic);
                        _trainedFaces.Add(faceImg);

                        string rawName = Path.GetFileNameWithoutExtension(file);
                        if (rawName.Contains("_")) rawName = rawName.Split('_')[0];

                        _trainedLabels.Add(rawName);
                        _trainedIntLabels.Add(idCounter);
                        idCounter++;
                    }
                    catch { continue; }
                }

                if (_trainedFaces.Count > 0)
                {
                    _recognizer = new EigenFaceRecognizer(_trainedFaces.Count);
                    using (VectorOfMat vectorOfMat = new VectorOfMat())
                    using (VectorOfInt vectorOfIds = new VectorOfInt())
                    {
                        foreach (var faceImg in _trainedFaces) { vectorOfMat.Push(faceImg.Mat); }
                        vectorOfIds.Push(_trainedIntLabels.ToArray());
                        _recognizer.Train(vectorOfMat, vectorOfIds);
                    }
                    return true;
                }

                UIWarningNotifier?.Invoke("Các file ảnh mẫu hiện tại của bạn bị lỗi cấu trúc dữ liệu!");
                return false;
            }
            catch (Exception ex)
            {
                UIWarningNotifier?.Invoke("Lỗi cấu hình AI: " + ex.Message);
                return false;
            }
        }

        // Dự đoán xem mặt này là của ai
        public string PredictOwner(Image<Gray, byte> faceResult, double maxDistance = 3500)
        {
            if (_recognizer == null) return null;

            var result = _recognizer.Predict(faceResult);
            if (result.Label != -1 && result.Distance < maxDistance)
            {
                return _trainedLabels[result.Label];
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