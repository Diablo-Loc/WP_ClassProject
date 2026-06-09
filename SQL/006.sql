USE LoginDB;
GO

-- 1. Xóa bảng cũ nếu tồn tại để reset sạch dữ liệu cấu trúc
IF OBJECT_ID('dbo.Assign', 'U') IS NOT NULL DROP TABLE dbo.Assign;
IF OBJECT_ID('dbo.HR', 'U') IS NOT NULL DROP TABLE dbo.HR;
GO

-- 2. Tạo bảng Quản lý nhân sự/Giảng viên
CREATE TABLE HR (
    MSGV NVARCHAR(20) PRIMARY KEY,
    Fname NVARCHAR(50),
    Lname NVARCHAR(50),
    Username VARCHAR(50),
    Pass VARCHAR(100),
    Email VARCHAR(100),
    Pic IMAGE,
    VALID BIT DEFAULT 1
);
GO

-- 3. Tạo bảng Phân công (Sử dụng kiểu NVARCHAR tương thích tự do)
CREATE TABLE Assign (
    ID_HR NVARCHAR(20),
    MaMH NVARCHAR(50), 
    PRIMARY KEY (ID_HR, MaMH)
);
GO

-- 4. Chèn một ít dữ liệu mẫu để Form có cái hiển thị chạy thử ngay lập tức
INSERT INTO dbo.HR (MSGV, Fname, Lname, Username, Pass) 
VALUES (N'GV001', N'Nguyễn Văn', N'A', 'gv001', '123'),
       (N'GV002', N'Trần Thị', N'B', 'gv002', '123');
GO

USE LoginDB;
GO

CREATE OR ALTER PROC proc_GetTeachingAssignments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ta.ID,                       -- Dùng ID ẩn trên Grid để xóa
        ta.HRID,                     -- Mã định danh tài khoản Giảng viên
        u.Username AS HRName,        -- Tên tài khoản Giảng viên
        ta.MaMH,                     -- Mã môn học
        c.TenMH,                      -- Tên môn học
        -- BÀI TẬP TỰ LÀM: Đếm số môn mỗi HR đang phụ trách ngay trong SQL
        (SELECT COUNT(*) FROM dbo.TeachingAssignment sub WHERE sub.HRID = ta.HRID) AS TotalAssigned
    FROM dbo.TeachingAssignment ta
    INNER JOIN dbo.Users u ON ta.HRID = u.Id
    INNER JOIN dbo.Course c ON ta.MaMH = c.MaMH
    ORDER BY ta.ID DESC;
END;
GO