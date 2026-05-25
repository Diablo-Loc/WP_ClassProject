/* =========================================================
   INIT / UPDATE SCHEMA (SAFE - no DROP DATABASE)
   RoleId convention:
   - 0 = Admin
   - 1 = Student
   - 2 = Giảng viên (Lecturer)
   ========================================================= */

-- Command: tạo DB nếu chưa có
IF DB_ID(N'LoginDB') IS NULL
BEGIN
    CREATE DATABASE LoginDB;
END
GO

-- Command: chuyển sang DB cần dùng
USE LoginDB;
GO

-- Command: tạo bảng Roles nếu chưa có
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT NOT NULL PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL UNIQUE
    );
END
GO

-- Command: tạo bảng Users nếu chưa có
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        RoleId INT NOT NULL,
        
        -- CÁC TRƯỜNG MỚI THÊM CHO LOGIC THỰC TẾ
        Valid INT DEFAULT 0,              
        FailedAttempts INT DEFAULT 0,     
        LockoutEnd DATETIME NULL,         
        
        Created_At DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Users_Roles
            FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END
GO

-- CHẠY ĐOẠN NÀY ĐỂ BỔ SUNG CỘT NẾU BẢNG ĐÃ TỒN TẠI
IF COL_LENGTH('dbo.Users', 'Valid') IS NULL
    ALTER TABLE dbo.Users ADD Valid INT DEFAULT 0;
IF COL_LENGTH('dbo.Users', 'FailedAttempts') IS NULL
    ALTER TABLE dbo.Users ADD FailedAttempts INT DEFAULT 0;
IF COL_LENGTH('dbo.Users', 'LockoutEnd') IS NULL
    ALTER TABLE dbo.Users ADD LockoutEnd DATETIME NULL;
GO

-- Command: tạo bảng Students nếu chưa có
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

        CONSTRAINT FK_Students_Users_UserId
            FOREIGN KEY (UserId)
            REFERENCES dbo.Users(Id)
    );
END
GO

/* Command: seed Roles theo Id cố định */
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

/* Command: seed admin (Fix lỗi Invalid Column Name bằng EXEC) */
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin' OR Email = N'admin@gmail.com')
BEGIN
    EXEC('INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid)
          VALUES (N''admin'', N''admin@gmail.com'', N''$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS'', 0, 1)');
END
GO

EXEC('UPDATE dbo.Users
      SET Password = N''$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS'',
          Valid = 1 
      WHERE Username = N''admin''');
GO

-- =========================================================
-- Tạo bảng Đăng ký môn học 
-- =========================================================
IF OBJECT_ID(N'dbo.CourseRegistration', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CourseRegistration (
        Mssv NVARCHAR(30) NOT NULL,
        CourseId VARCHAR(50) NOT NULL,
        RegistrationDate DATETIME DEFAULT GETDATE(),
        Score FLOAT NULL, 
        
        PRIMARY KEY (Mssv, CourseId),
        CONSTRAINT FK_CourseRegistration_Students 
            FOREIGN KEY (Mssv) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE
    );
END
GO

-- Command: kiểm tra nhanh dữ liệu toàn bộ hệ thống
SELECT * FROM dbo.Roles ORDER BY Id;
SELECT TOP 50 * FROM dbo.Users ORDER BY Id DESC;
SELECT TOP 50 * FROM dbo.Students ORDER BY Id DESC;
SELECT * FROM dbo.CourseRegistration;
GO