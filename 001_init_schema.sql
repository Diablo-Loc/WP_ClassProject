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

-- Command: tạo bảng Roles nếu chưa có (Id cố định 0/1/2, KHÔNG identity)
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
        Created_At DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Users_Roles
            FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END
GO
-- Command: tạo bảng Students nếu chưa có (FK email -> Users.Email)
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

/* Command: seed Roles theo Id cố định
   - MERGE để chạy lại nhiều lần không bị insert trùng, không xóa dữ liệu cũ
*/
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

/* Command: seed admin (tùy chọn)
   - IF NOT EXISTS: chỉ thêm nếu chưa có, không overwrite password hay xóa user khác
*/
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin' OR Email = N'admin@gmail.com')
BEGIN
    INSERT INTO dbo.Users (Username, Email, Password, RoleId)
    VALUES (N'admin', N'admin@gmail.com', N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS', 0);
END
GO

UPDATE dbo.Users
SET Password = N'$2a$12$cWrDKpQg5HtG7nixf4Wu1OTveL5mWu8h5.1tIrA43Ssc4JCPWX8GS'
WHERE Username = N'admin';
GO

-- Command: kiểm tra nhanh dữ liệu
SELECT * FROM dbo.Roles ORDER BY Id;
SELECT TOP 50 * FROM dbo.Users ORDER BY Id DESC;
SELECT TOP 50 * FROM dbo.Students ORDER BY Id DESC;
GO

--DROP TABLE Students;
--DROP TABLE Users;
--DROP TABLE Roles;