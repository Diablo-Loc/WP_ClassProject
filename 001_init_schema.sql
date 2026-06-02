-- 1. Khởi tạo Database một cách an toàn
IF DB_ID(N'LoginDB') IS NULL
BEGIN
    CREATE DATABASE LoginDB;
END
GO

USE LoginDB;
GO

-- ============================================================================
-- PHẦN THIẾT KẾ HỆ THỐNG BẢNG (LOGIC CŨ VÀ MỚI ĐƯỢC GIỮ NGUYÊN)
-- ============================================================================

-- Tạo bảng Roles (Quyền truy cập hệ thống)
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT NOT NULL PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL UNIQUE
    );
END
GO

-- Tạo bảng Users (Tài khoản đăng nhập)
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        RoleId INT NOT NULL,
        Valid INT DEFAULT 0,              
        FailedAttempts INT DEFAULT 0,     
        LockoutEnd DATETIME NULL,         
        Created_At DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END
GO

-- Tạo bảng Students (Thông tin chi tiết sinh viên)
IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NULL,
        MSSV NVARCHAR(30) NOT NULL UNIQUE,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        DateOfBirth DATETIME NULL,
        Gender NVARCHAR(10) NULL,
        Phone NVARCHAR(15) NULL,
        Address NVARCHAR(200) NULL,
        Hometown NVARCHAR(100) NULL,
        Email NVARCHAR(100) NULL,
        Picture VARBINARY(MAX) NULL,
        Created_At DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Students_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

-- Tạo bảng Course (Thông tin môn học - ĐÃ THÊM CỘT NAMHOC)
IF OBJECT_ID(N'dbo.Course', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Course (
        MaMH CHAR(10) NOT NULL PRIMARY KEY,
        TenMH NVARCHAR(100) NOT NULL,
        SoTC INT NULL,
        Tuan INT NULL,
        Hky INT NULL,
        NamHoc NVARCHAR(20) NULL, 
        Mota NVARCHAR(500) NULL
    );
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Course') AND name = N'NamHoc')
    BEGIN
        ALTER TABLE dbo.Course ADD NamHoc NVARCHAR(20) NULL;
    END
END
GO

-- Tạo bảng DKMH (Bảng trung gian Đăng ký môn học)
IF OBJECT_ID(N'dbo.DKMH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DKMH (
        MSSV NVARCHAR(30) NOT NULL,
        MaMH CHAR(10) NOT NULL,
        RegistrationDate DATETIME DEFAULT GETDATE(),
        
        PRIMARY KEY (MSSV, MaMH),
        CONSTRAINT FK_DKMH_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE,
        CONSTRAINT FK_DKMH_Course FOREIGN KEY (MaMH) REFERENCES dbo.Course(MaMH) ON DELETE CASCADE
    );
END
GO

-- Tạo bảng Score (Bảng quản lý điểm số chi tiết)
IF OBJECT_ID(N'dbo.Score', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Score (
        MSSV NVARCHAR(30) NOT NULL,
        MaMH CHAR(10) NOT NULL,
        DiemQT DECIMAL(4,2) NULL,
        DiemCK DECIMAL(4,2) NULL,
        DiemTK DECIMAL(4,2) NULL,
        Mota NVARCHAR(200) NULL,
        
        PRIMARY KEY (MSSV, MaMH),
        CONSTRAINT FK_Score_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE, 
        CONSTRAINT FK_Score_Course FOREIGN KEY (MaMH) REFERENCES dbo.Course(MaMH) ON DELETE CASCADE
    );
END
GO

-- ⭐ [THÊM MỚI TẠI ĐÂY] - Tạo bảng Requests phục vụ Câu 4
IF OBJECT_ID(N'dbo.Requests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Requests
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MSSV NVARCHAR(30) NOT NULL,
        RequestContent NVARCHAR(500) NOT NULL,
        Status NVARCHAR(20) DEFAULT N'Pending', 
        AdminComment NVARCHAR(200) NULL,       
        Created_At DATETIME DEFAULT GETDATE(),
        Updated_At DATETIME NULL,

        CONSTRAINT FK_Requests_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE
    );
END
GO


-- ============================================================================
-- PHẦN DỮ LIỆU GỐC BẮT BUỘC VÀ DỮ LIỆU TEST (SEED DATA)
-- ============================================================================

-- Khởi tạo danh sách Roles mặc định cố định
MERGE dbo.Roles AS target
USING (VALUES
    (0, N'Admin'),
    (1, N'Student'),
    (2, N'Giảng viên')
) AS src(Id, RoleName)
ON target.Id = src.Id
WHEN NOT MATCHED THEN
    INSERT (Id, RoleName) VALUES (src.Id, src.RoleName);
GO

-- Khởi tạo tài khoản Quản trị tối cao (Admin)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin' OR Email = N'admin@gmail.com')
BEGIN
    EXEC('INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid)
          VALUES (N''admin'', N''admin@gmail.com'', N''$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS'', 0, 1)');
END
ELSE
BEGIN
    EXEC('UPDATE dbo.Users
          SET Password = N''$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS'',
              Valid = 1 
          WHERE Username = N''admin''');
END
GO

--Tự động tạo tài khoản Sinh viên mẫu để test luồng câu 4
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'sinhvien1')
BEGIN
    INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid)
    VALUES (N'sinhvien1', N'sv1@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 1, 1);

    DECLARE @NewUserId INT = SCOPE_IDENTITY();

    INSERT INTO dbo.Students (UserId, MSSV, FirstName, LastName, Gender, Email)
    VALUES (@NewUserId, N'22110158', N'Nguyễn Văn', N'Hùng', N'Nam', N'sv1@gmail.com');
END
GO

--Tự động tạo các Request chờ duyệt mẫu cho Sinh viên trên
IF EXISTS (SELECT 1 FROM dbo.Students WHERE MSSV = N'22110158') 
   AND NOT EXISTS (SELECT 1 FROM dbo.Requests WHERE MSSV = N'22110158')
BEGIN
    INSERT INTO dbo.Requests (MSSV, RequestContent, Status) VALUES
    (N'22110158', N'Em xin phúc khảo lại điểm thi cuối kỳ môn Cấu trúc dữ liệu và giải thuật (DASA230179E).', N'Pending'),
    (N'22110158', N'Cập nhật lại số điện thoại cá nhân bị nhập sai khi đăng ký nhập học.', N'Pending');
END
GO

-- In ra màn hình trạng thái để cả nhóm biết cấu trúc đã chạy thành công
SELECT * FROM dbo.Roles ORDER BY Id;
SELECT * FROM dbo.Users WHERE Username = 'admin';
SELECT * FROM dbo.Requests ORDER BY Created_At DESC;
SELECT N'Hệ thống CSDL chuẩn tổng hợp (đã tích hợp Câu 4) đã sẵn sàng!' AS [Trạng thái];
GO