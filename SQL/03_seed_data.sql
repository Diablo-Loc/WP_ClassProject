-- ============================================================================
-- FILE 03 (ĐÃ SỬA LỖI): NẠP DỮ LIỆU TEST TOÀN DIỆN & PHONG PHÚ (ĐÃ ĐỒNG BỘ THỨ/CA)
-- MẬT KHẨU MẶC ĐỊNH CHO TẤT CẢ TÀI KHOẢN: 123
-- ============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- 1. NẠP DANH MỤC PHÂN QUYỀN
INSERT INTO dbo.Roles (Id, RoleName) VALUES  
(0, N'Admin'), (1, N'Student'), (2, N'Giảng viên'), (3, N'Giáo Vụ');
GO

-- 2. NẠP DANH MỤC CHUYÊN NGÀNH
INSERT INTO dbo.Major (MaNganh, TenNganh) VALUES  
('CNTT', N'Công nghệ thông tin'),  
('KTPM', N'Kỹ thuật phần mềm'),
('KHMT', N'Khoa học máy tính');
GO

-- 3. NẠP ĐẦY ĐỦ TÀI KHOẢN NGƯỜI DÙNG ĐỂ KHÔNG BỊ LỖI PHỤ THUỘC (Mật khẩu băm BCrypt của chuỗi "123")
INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status, FailedAttempts, LockoutEnd) VALUES  
(N'admin', N'admin@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 0, 1, 1, 0, NULL),
(N'giangvien1', N'nguyenvanan@teacher.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 2, 1, 1, 0, NULL),
(N'giangvien2', N'tranthibinh@teacher.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 2, 1, 1, 0, NULL),
(N'giangvien3', N'phamhoangcuong@teacher.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 2, 1, 1, 0, NULL),
(N'sinhvien1', N'30110158@student.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'sinhvien2', N'30110159@student.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'sinhvien3', N'30110201@student.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'giaovu1',  N'giaovu1@giaovu.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 3, 1, 1, 0, NULL),
(N'24110107', N'24110107@student.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'24110077', N'24110077@student.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL);
GO

-- 4. LIÊN KẾT HỒ SƠ GIẢNG VIÊN VẬT LÝ
DECLARE @Gv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien1');
DECLARE @Gv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien2');
DECLARE @Gv3Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien3');

INSERT INTO dbo.Teachers (UserId, MSGV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, AcademicRank, Status) VALUES
(@Gv1Uid, N'MSGV_001',  N'An', N'Nguyễn Văn','1980-05-12', N'Nam', '0912345678', N'gv1@gmail.com', N'Tiến sĩ', 1),
(@Gv2Uid, N'MSGV_002', N'Bình', N'Trần Thị', '1985-08-20', N'Nữ', '0987654321', N'gv2@gmail.com', N'Thạc sĩ', 1),
(@Gv3Uid, N'MSGV_003', N'Cường', N'Phạm Hoàng', '1978-11-02', N'Nam', '0966778899', N'gv3@gmail.com', N'Phó Giáo sư', 1);
GO

-- 5. NẠP DANH MỤC LỚP HÀNH CHÍNH
INSERT INTO dbo.Classroom (MaLop, TenLop, SiSo, GVCN, MaNganh) VALUES  
('LH_CNTT_K16A', N'Lớp sinh hoạt CNTT K16-A', 0, N'Nguyễn Văn An', 'CNTT'),
('LH_KTPM_K16A', N'Lớp sinh hoạt KTPM K16-A', 0, N'Trần Thị Bình', 'KTPM');
GO

-- 5B. LIÊN KẾT HỒ SƠ GIÁO VỤ VẬT LÝ
DECLARE @GvUId INT = (SELECT Id FROM dbo.Users WHERE Username = N'giaovu1');
INSERT INTO dbo.Staffs (UserId, MSNV, FirstName, LastName, Phone, Email, Department, Status) VALUES
(@GvUId, N'MSNV_GV01', N'Dung', N'Nguyễn Thị', '0933445566', N'giaovu1@giaovu.hcmute.edu.vn', N'Phòng Giáo vụ', 1);
GO

-- 6. LIÊN KẾT HỒ SƠ SINH VIÊN VẬT LÝ
DECLARE @Sv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien1');
DECLARE @Sv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien2');
DECLARE @Sv3Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien3');
DECLARE @Sv4Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'24110107');
DECLARE @Sv5Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'24110077');

INSERT INTO dbo.Students (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, MaLop, MaNganh) VALUES
(@Sv1Uid, N'30110158', N'Nguyễn Văn', N'Hùng', '2004-01-15', N'Nam', '0901112223', N'sv1@gmail.com', 'LH_CNTT_K16A', 'CNTT'),
(@Sv2Uid, N'30110159', N'Lê Thị', N'Mai', '2004-06-20', N'Nữ', '0904445556', N'sv2@gmail.com', 'LH_CNTT_K16A', 'CNTT'),
(@Sv3Uid, N'30110201', N'Phạm Minh', N'Quang', '2004-11-02', N'Nam', '0907778889', N'sv3@gmail.com', 'LH_KTPM_K16A', 'KTPM'),
(@Sv4Uid, N'24110107', N'Nguyễn Đăk', N'Lộc', '2006-01-01', N'Nam', '0911111111', N'locnguyen@gmail.com', 'LH_CNTT_K16A', 'CNTT'),
(@Sv5Uid, N'24110077', N'Trần Thiên', N'Ân', '2006-02-02', N'Nam', '0922222222', N'antran@gmail.com', 'LH_CNTT_K16A', 'CNTT');

-- Cập nhật sĩ số cơ sở dữ liệu sau khi thêm sinh viên
UPDATE dbo.Classroom SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE MaLop = 'LH_CNTT_K16A') WHERE MaLop = 'LH_CNTT_K16A';
UPDATE dbo.Classroom SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE MaLop = 'LH_KTPM_K16A') WHERE MaLop = 'LH_KTPM_K16A';
GO

-- 7. TẠO DANH MỤC MÔN HỌC KHUNG (Phủ nhiều học kỳ để test biểu đồ tuyến tính)
INSERT INTO dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota) VALUES  
('NMTH01', N'Nhập môn tin học', 2, 15, 1, N'2024-2025', N'Môn đại cương học kỳ 1 năm trước'),
('LTHDT2', N'Lập trình hướng đối tượng', 3, 15, 2, N'2024-2025', N'Môn cơ sở ngành học kỳ 2 năm trước'),
('CTDL01', N'Cấu trúc dữ liệu và giải thuật', 3, 15, 1, N'2025-2026', N'Môn học cốt lõi kì này'),
('CSDL02', N'Cơ sở dữ liệu', 3, 15, 1, N'2025-2026', N'Môn học nền tảng dữ liệu kì này'),
('HDH003', N'Hệ điều hành', 3, 15, 1, N'2025-2026', N'Môn học kiến trúc hệ thống'),
('ANM004', N'An toàn bảo mật thông tin', 3, 15, 2, N'2025-2026', N'Môn học nâng cao chuyên ngành kì tới');
GO

-- 8. PHÂN CÔNG GIẢNG VIÊN KHAI THÁC (Tài khoản đã tồn tại ở Bước 3 -> Chạy mượt mà, không bao giờ lỗi NULL)
DECLARE @Gv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien1');
DECLARE @Gv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien2');
DECLARE @Gv3Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien3');

INSERT INTO dbo.TeachingAssignment (HRID, MaMH) VALUES  
(@Gv1Uid, 'NMTH01'), (@Gv1Uid, 'CTDL01'),
(@Gv2Uid, 'LTHDT2'), (@Gv2Uid, 'CSDL02'),
(@Gv3Uid, 'HDH003'), (@Gv3Uid, 'ANM004');
GO

-- 9. MỞ CÁC LỚP HỌC PHẦN (Đã đồng bộ ThuHoc từ 2->7 và CaHoc từ 1->4)
INSERT INTO dbo.CourseSection (MaLopHP, MaMH, HocKy, NamHoc, MSGV, PhongHoc, MaxStudents, Status, ThuHoc, CaHoc) VALUES  
-- Học kỳ 1 (Năm học 2024-2025)
('LHP_NMTH_K24_N01', 'NMTH01', 1, N'2024-2025', N'MSGV_001', N'Phòng A.101', 40, 1, 2, 1),
-- Học kỳ 2 (Năm học 2024-2025)
('LHP_LTHDT_K24_N01', 'LTHDT2', 2, N'2024-2025', N'MSGV_002', N'Phòng B.302', 40, 1, 4, 2),
-- Học kỳ 1 HIỆN TẠI (Năm học 2025-2026) -> Phủ kín lịch từ Thứ 2 tới Thứ 5 để test TKB
('LHP_CTDL_N01', 'CTDL01', 1, N'2025-2026', N'MSGV_001', N'Phòng A.202', 40, 1, 2, 1), -- Thứ 2, Ca 1
('LHP_CSDL_N01', 'CSDL02', 1, N'2025-2026', N'MSGV_002', N'Phòng C.301', 50, 1, 3, 2), -- Thứ 3, Ca 2
('LHP_HDH_N01',  'HDH003', 1, N'2025-2026', N'MSGV_003', N'Phòng E.401', 45, 1, 5, 3), -- Thứ 5, Ca 3
-- Lớp mở dự phòng cho Học kỳ 2 sắp tới
('LHP_ANM_N01',  'ANM004', 2, N'2025-2026', N'MSGV_003', N'Phòng D.102', 40, 1, 6, 4);  -- Thứ 6, Ca 4
GO

-- 10. ĐĂNG KÝ MÔN HỌC (DKMH)
INSERT INTO dbo.DKMH (MSSV, MaLopHP) VALUES  
(N'30110158', 'LHP_NMTH_K24_N01'),
(N'30110158', 'LHP_LTHDT_K24_N01'),
(N'30110158', 'LHP_CTDL_N01'),  
(N'30110158', 'LHP_CSDL_N01'),
(N'30110158', 'LHP_HDH_N01'),

(N'30110159', 'LHP_NMTH_K24_N01'),
(N'30110159', 'LHP_CTDL_N01'),
(N'30110159', 'LHP_CSDL_N01'),

(N'24110107', 'LHP_CTDL_N01'),  
(N'24110107', 'LHP_CSDL_N01'),
(N'24110107', 'LHP_HDH_N01'),

(N'24110077', 'LHP_CTDL_N01'),  
(N'24110077', 'LHP_CSDL_N01'),
(N'24110077', 'LHP_HDH_N01');
GO

-- 11. NHẬP ĐIỂM KIỂM THỬ (Biến động tăng dần qua từng kỳ để vẽ đồ thị Line đẹp xuất sắc)
INSERT INTO dbo.Score (MSSV, MaLopHP, DiemQT, DiemCK, DiemTK, Mota) VALUES  
(N'30110158', 'LHP_NMTH_K24_N01', 7.0, 8.0, 7.50, N'Đạt'),
(N'30110158', 'LHP_LTHDT_K24_N01', 8.0, 8.5, 8.20, N'Tốt'),
(N'30110158', 'LHP_CTDL_N01', 8.5, 9.0, 8.85, N'Xuất sắc'),
(N'30110158', 'LHP_CSDL_N01', 6.5, 7.0, 6.80, N'Khá'),
(N'30110158', 'LHP_HDH_N01', 7.5, 8.0, 7.80, N'Khá'),

(N'30110159', 'LHP_NMTH_K24_N01', 6.0, 6.0, 6.00, N'Trung bình'),
(N'30110159', 'LHP_CTDL_N01', 5.5, 6.0, 5.85, N'Trung bình khá'),
(N'30110159', 'LHP_CSDL_N01', 8.0, 7.5, 7.70, N'Khá'),

(N'24110107', 'LHP_CTDL_N01',10, 10, 10, N'Xuất sắc'),
(N'24110107', 'LHP_CSDL_N01', 10,10, 10, N'Xuất sắc'),
(N'24110107', 'LHP_HDH_N01', 10,10, 10, N'Xuất sắc'),

(N'24110077', 'LHP_CTDL_N01', 10,10, 10, N'Xuất sắc'),
(N'24110077', 'LHP_CSDL_N01', 10,10, 10, N'Xuất sắc'),
(N'24110077', 'LHP_HDH_N01', 10,10, 10, N'Xuất sắc');
GO

-- 12. ĐƠN TỪ ĐỂ TEST TRẠNG THÁI UI
INSERT INTO dbo.Requests (MSSV, RequestType, RequestContent, Status, AdminComment) VALUES
(N'30110158', N'Phúc khảo', N'Em xin phúc khảo lại điểm thi cuối kỳ môn CTDL do thấy chấm sót câu 3.', N'Pending', NULL),
(N'30110158', N'Sửa thông tin', N'Cập nhật lại số điện thoại chính xác của em là 0901112223.', N'Approved', N'Đã cập nhật hệ thống'),
(N'30110159', N'Giấy xác nhận', N'Xin cấp giấy xác nhận sinh viên để bổ sung hồ sơ học bổng địa phương.', N'Approved', N'Đã ký và xuất bản điện tử'),
(N'30110201', N'Hoãn thi', N'Xin hoãn thi môn Cơ sở dữ liệu do lịch trùng lịch phẫu thuật.', N'Rejected', N'Lý do minh chứng chưa đầy đủ đóng mộc bệnh viện');
GO

PRINT N'======================================================================';
PRINT N'---> DỮ LIỆU ĐÃ ĐƯỢC ĐỔ CỰC KỲ ĐẦY ĐỦ VÀ SỬA LỖI THÀNH CÔNG 100%! <---';
PRINT N'======================================================================';
GO
