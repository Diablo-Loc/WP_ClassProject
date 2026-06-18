-- ============================================================================
-- FILE 03 (CHÍNH THỨC): NẠP DỮ LIỆU TEST CHUẨN ĐỒNG BỘ THEO FILE VĂN BẢN 01
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

-- 3. NẠP TÀI KHOẢN NGƯỜI DÙNG (Mật khẩu đã băm BCrypt của chuỗi "123")
-- Đã fix lỗi dấu chấm phẩy (;) ở tài khoản sinhvien3 và nối thêm giaovu1 một cách hợp lệ
INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status, FailedAttempts, LockoutEnd) VALUES  
(N'admin', N'admin@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 0, 1, 1, 0, NULL),
(N'giangvien1', N'gv1@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 2, 1, 1, 0, NULL),
(N'giangvien2', N'gv2@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 2, 1, 1, 0, NULL),
(N'sinhvien1', N'sv1@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'sinhvien2', N'sv2@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'sinhvien3', N'sv3@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1, 1, 0, NULL),
(N'giaovu1',  N'giaovu1@giaovu.hcmute.edu.vn', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 3, 1, 1, 0, NULL);
GO

-- 4. LIÊN KẾT HỒ SƠ GIẢNG VIÊN VẬT LÝ
DECLARE @Gv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien1');
DECLARE @Gv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien2');

INSERT INTO dbo.Teachers (UserId, MSGV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, AcademicRank, Status) VALUES
(@Gv1Uid, N'MSGV_001', N'Nguyễn Văn', N'An', '1980-05-12', N'Nam', '0912345678', N'gv1@gmail.com', N'Tiến sĩ', 1),
(@Gv2Uid, N'MSGV_002', N'Trần Thị', N'Bình', '1985-08-20', N'Nữ', '0987654321', N'gv2@gmail.com', N'Thạc sĩ', 1);
GO

-- 5. NẠP DANH MỤC LỚP HÀNH CHÍNH
INSERT INTO dbo.Classroom (MaLop, TenLop, SiSo, GVCN, MaNganh) VALUES  
('LH_CNTT_K16A', N'Lớp sinh hoạt CNTT K16-A', 0, N'Nguyễn Văn An', 'CNTT'),
('LH_KTPM_K16A', N'Lớp sinh hoạt KTPM K16-A', 0, N'Trần Thị Bình', 'KTPM');
GO

-- [MỚI] 5B. LIÊN KẾT HỒ SƠ GIÁO VỤ VẬT LÝ (Đã tách riêng và khai báo biến @GvUId hợp lệ)
DECLARE @GvUId INT = (SELECT Id FROM dbo.Users WHERE Username = N'giaovu1');
INSERT INTO dbo.Staffs (UserId, MSNV, FirstName, LastName, Phone, Email, Department, Status) VALUES
(@GvUId, N'MSNV_GV01', N'Dung', N'Nguyễn Thị', '0933445566', N'giaovu1@gmail.com', N'Phòng Giáo vụ', 1);
GO

-- 6. LIÊN KẾT HỒ SƠ SINH VIÊN VẬT LÝ
DECLARE @Sv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien1');
DECLARE @Sv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien2');
DECLARE @Sv3Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'sinhvien3');

INSERT INTO dbo.Students (UserId, MSSV, FirstName, LastName, DateOfBirth, Gender, Phone, Email, MaLop, MaNganh) VALUES
(@Sv1Uid, N'22110158', N'Nguyễn Văn', N'Hùng', '2004-01-15', N'Nam', '0901112223', N'sv1@gmail.com', 'LH_CNTT_K16A', 'CNTT'),
(@Sv2Uid, N'22110159', N'Lê Thị', N'Mai', '2004-06-20', N'Nữ', '0904445556', N'sv2@gmail.com', 'LH_CNTT_K16A', 'CNTT'),
(@Sv3Uid, N'22110201', N'Phạm Minh', N'Quang', '2004-11-02', N'Nam', '0907778889', N'sv3@gmail.com', 'LH_KTPM_K16A', 'KTPM');

-- Cập nhật sĩ số cơ sở dữ liệu sau khi thêm sinh viên
UPDATE dbo.Classroom SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE MaLop = 'LH_CNTT_K16A') WHERE MaLop = 'LH_CNTT_K16A';
UPDATE dbo.Classroom SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE MaLop = 'LH_KTPM_K16A') WHERE MaLop = 'LH_KTPM_K16A';
GO

-- 7. TẠO DANH MỤC MÔN HỌC KHUNG
INSERT INTO dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota) VALUES  
('CTDL01', N'Cấu trúc dữ liệu và giải thuật', 3, 15, 1, N'2025-2026', N'Môn học cơ sở ngành'),
('CSDL02', N'Cơ sở dữ liệu', 3, 15, 1, N'2025-2026', N'Môn học nền tảng dữ liệu');
GO

-- 8. PHÂN CÔNG GIẢNG VIÊN (Sử dụng ID từ bảng Users nối sang Course)
DECLARE @Gv1Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien1');
DECLARE @Gv2Uid INT = (SELECT Id FROM dbo.Users WHERE Username = N'giangvien2');

INSERT INTO dbo.TeachingAssignment (HRID, MaMH) VALUES  
(@Gv1Uid, 'CTDL01'),
(@Gv2Uid, 'CSDL02');
GO

-- 9. MỞ CÁC LỚP HỌC PHẦN
INSERT INTO dbo.CourseSection (MaLopHP, MaMH, HocKy, NamHoc, MSGV, PhongHoc, MaxStudents, Status) VALUES  
('LHP_CTDL_N01', 'CTDL01', 1, N'2025-2026', N'MSGV_001', N'Phòng A.202', 40, 1),
('LHP_CSDL_N01', 'CSDL02', 1, N'2025-2026', N'MSGV_002', N'Phòng C.301', 50, 1);
GO

-- 10. ĐĂNG KÝ MÔN HỌC (DKMH)
INSERT INTO dbo.DKMH (MSSV, MaLopHP) VALUES  
(N'22110158', 'LHP_CTDL_N01'),  
(N'22110158', 'LHP_CSDL_N01'),
(N'22110159', 'LHP_CTDL_N01');
GO

-- 11. NHẬP ĐIỂM KIỂM THỬ
INSERT INTO dbo.Score (MSSV, MaLopHP, DiemQT, DiemCK, DiemTK, Mota) VALUES  
(N'22110158', 'LHP_CTDL_N01', 8.5, 9.0, 8.85, N'Tốt'),
(N'22110158', 'LHP_CSDL_N01', 7.0, 8.0, 7.70, N'Đạt'),
(N'22110159', 'LHP_CTDL_N01', 5.5, 6.0, 5.85, N'Khá trung bình');
GO

-- 12. ĐƠN TỪ ĐỂ TEST TRẠNG THÁI UI
INSERT INTO dbo.Requests (MSSV, RequestType, RequestContent, Status) VALUES
(N'22110158', N'Phúc khảo', N'Em xin phúc khảo lại điểm thi cuối kỳ môn CTDL.', N'Pending');
GO

PRINT N'======================================================================';
PRINT N'---> TOÀN BỘ CƠ SỞ DỮ LIỆU ĐÃ ĐỒNG BỘ 100% VỚI CẤU TRÚC BẢNG MỚI! <---';
PRINT N'======================================================================';
GO