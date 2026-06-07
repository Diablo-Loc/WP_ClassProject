# 🏫 Project Công Nghệ Phần Mềm / Lập Trình Windows

Chào mừng bạn đến với kho lưu trữ mã nguồn của dự án **ClassProject**. Dưới đây là hướng dẫn chi tiết cách thiết lập môi trường và nạp Cơ sở dữ liệu (Database) tự động để chạy ứng dụng.

---

## 📁 Cấu trúc thư mục Database

Hệ thống quản lý cơ sở dữ liệu của dự án được tổ chức tách biệt và tự động hóa, tránh việc gộp file hay chỉnh sửa thủ công:

```text
├── ClassProject.slnx           # File Solution của dự án (.NET)
├── Click_Chay_DB.bat           # File mồi 1-Click để chạy hệ thống trên Windows
├── Run_SetupDB.ps1             # Script PowerShell xử lý nạp DB tuần tự
└── SQL/                        # Thư mục chứa các file SQL con độc lập
    ├── 001_init_schema.sql     # Khởi tạo Database, cấu trúc bảng & dữ liệu mẫu
    ├── Table_classroom.sql     # Quản lý cấu trúc phòng học
    ├── Teaching_Assignment.sql # Quản lý phân công giảng dạy
    └── Create_Procedures_Statistics.sql # Các Stored Procedure thống kê
```
🚀 Hướng dẫn Thiết lập Database (Chỉ với 1-Click)
Thay vì phải mở SSMS, tạo Database thủ công rồi copy-paste từng file SQL rất mất thời gian, bạn chỉ cần thực hiện theo các bước siêu đơn giản sau:

🌟 Bước 1: Khởi chạy công cụ tự động
Vào thư mục gốc dự án, tìm và nhấp đúp chuột (Double-click) vào file:
👉 Click_Chay_DB.bat

🌟 Bước 2: Điền thông tin SQL Server của máy bạn
Màn hình dòng lệnh (Command Prompt) màu đen sẽ hiện lên và đưa ra câu hỏi tương tác:

Plaintext


Nhap ten SQL Server cua ban va bam Enter.
Mac dinh la: .\SQLEXPRESS (Bam Enter neu dung luon):
Trường hợp 1: Nếu máy bạn dùng SQL Server Express mặc định (.\SQLEXPRESS), bạn chỉ cần bấm ENTER.

Trường hợp 2: Nếu máy bạn đặt tên Server khác (Ví dụ: ABC\SQLEXPRESS hoặc .\), hãy gõ/dán tên Server đó vào rồi bấm ENTER.

🌟 Bước 3: Nghiệm thu kết quả
Hệ thống sẽ tự động gọi tiện ích sqlcmd chính chủ của Microsoft để chạy tuần tự tất cả các file trong thư mục SQL/.

Khi màn hình xuất hiện thông báo màu xanh lá cây:
🎉 XONG: Database da duoc nap thanh cong!
Nghĩa là toàn bộ cơ sở dữ liệu LoginDB, các bảng, dữ liệu mẫu và các hàm Procedure đã sẵn sàng nạp vào máy của bạn!

⚠️ Lưu ý khi gặp lỗi (Troubleshooting)
Lỗi trùng lặp bảng (Object already named):

Triệu chứng: Hệ thống báo There is already an object named '...' in the database.

Cách sửa: Do bạn đã chạy script này trước đó nên database đã có sẵn bảng. Hãy mở SSMS ra, click chuột phải vào database LoginDB chọn Delete (nhớ tích chọn Close existing connections), sau đó chạy lại file .bat là xong.

Lỗi không nhận diện được lệnh sqlcmd:

Triệu chứng: Script báo lỗi hệ thống do máy bạn bị thiếu công cụ dòng lệnh của SQL Server.

Cách sửa: Mở Terminal/CMD trên máy tính của bạn lên, gõ lệnh sau để Windows tự động tải và cài đặt trong 5 giây:

Bash


winget install Microsoft.go-sqlcmd
Cài xong, tắt đi và nhấp đúp lại file .bat.
