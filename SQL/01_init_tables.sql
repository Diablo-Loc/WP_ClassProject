-- ============================================================================
-- FILE 01: KHỞI TẠO CẤU TRÚC BẢNG, RÀNG BUỘC VÀ KHÔNG GIAN LƯU TRỮ (INDEX/VIEW)
-- ============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

USE master;
GO

IF DB_ID(N'LoginDB') IS NOT NULL
BEGIN
    ALTER DATABASE LoginDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LoginDB;
END
GO

CREATE DATABASE LoginDB;
GO
USE LoginDB;
GO

-- ============================================================================
-- 1. CÁC BẢNG DANH MỤC ĐỘC LẬP
-- ============================================================================

CREATE TABLE dbo.Roles
(
    Id INT NOT NULL PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE dbo.Major (
    MaNganh CHAR(10) NOT NULL PRIMARY KEY,
    TenNganh NVARCHAR(100) NOT NULL,
    Created_At DATETIME DEFAULT GETDATE(), -- Tích hợp quản lý vòng đời
    Updated_At DATETIME NULL
);
GO

CREATE TABLE dbo.Classroom
(
    MaLop VARCHAR(20) PRIMARY KEY,
    TenLop NVARCHAR(100) NOT NULL UNIQUE, -- Đảm bảo không trùng tên lớp hành chính
    SiSo INT DEFAULT 0 CONSTRAINT CK_Classroom_SiSo CHECK (SiSo >= 0), -- Chống sĩ số âm
    GVCN NVARCHAR(100),
    MaNganh CHAR(10) NULL,
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,
    CONSTRAINT FK_Classroom_Major FOREIGN KEY (MaNganh) REFERENCES dbo.Major(MaNganh) ON DELETE SET NULL
);
GO

CREATE TABLE dbo.Course (
    MaMH CHAR(10) NOT NULL PRIMARY KEY,
    TenMH NVARCHAR(100) NOT NULL,
    SoTC INT NULL CONSTRAINT CK_Course_SoTC CHECK (SoTC > 0), -- Số tín chỉ phải lớn hơn 0
    Tuan INT NULL,
    Hky INT NULL,
    NamHoc NVARCHAR(20) NULL, 
    Mota NVARCHAR(500) NULL,
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL
);
GO

-- ============================================================================
-- 2. PHÂN HỆ TÀI KHOẢN VÀ HỒ SƠ VẬT LÝ
-- ============================================================================

CREATE TABLE dbo.Users
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL, 
    RoleId INT NOT NULL,
    Valid INT DEFAULT 0,              
    Status INT DEFAULT 0,             
    FailedAttempts INT DEFAULT 0,     
    LockoutEnd DATETIME NULL,         
    LastLogin DATETIME NULL, 
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
);
GO

CREATE TABLE dbo.Groups (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    UserID INT NOT NULL,
    CONSTRAINT FK_Groups_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Contact (
    ContactID INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(100) NOT NULL,             
    Fname NVARCHAR(50) NOT NULL,             
    Lname NVARCHAR(50) NULL,              
    Dob DATETIME NULL,
    Gender NVARCHAR(10) NULL,
    Group_ID INT NULL,
    Phone NVARCHAR(15) NULL,
    Address NVARCHAR(200) NULL,
    Email NVARCHAR(100) NULL,
    Pic IMAGE NULL,
    UserID INT NOT NULL,
    CONSTRAINT FK_Contact_Groups FOREIGN KEY (Group_ID) REFERENCES dbo.Groups(ID) ON DELETE SET NULL,
    -- Đổi thành NO ACTION ở đây để SQL Server không báo lỗi trùng lặp đường dẫn xóa
    CONSTRAINT FK_Contact_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(Id) ON DELETE NO ACTION
);
GO


CREATE TABLE dbo.Teachers
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NULL,                             -- Liên kết sang tài khoản đăng nhập hệ thống (Users)
    MSGV NVARCHAR(30) NOT NULL UNIQUE,           -- Mã số giảng viên làm định danh nghiệp vụ
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATETIME NULL,
    Gender NVARCHAR(10) NULL,
    Phone NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    AcademicRank NVARCHAR(50) NULL,              -- Học hàm/Học vị (Thạc sĩ, Tiến sĩ...)
    Status INT DEFAULT 1,                        -- 1: Đang công tác, 0: Đã nghỉ việc / Đình chỉ
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,

    CONSTRAINT FK_Teachers_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE SET NULL,
    CONSTRAINT UQ_Teachers_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.Staffs
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NULL,                                 -- Liên kết tài khoản đăng nhập
    MSNV NVARCHAR(30) NOT NULL UNIQUE,               -- Mã số nhân viên (Mã Giáo vụ)
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL CONSTRAINT UQ_Staffs_Email UNIQUE,
    Department NVARCHAR(100) DEFAULT N'Phòng Giáo vụ',
    Status INT DEFAULT 1,                            -- 1: Đang làm việc, 0: Đã nghỉ việc
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,

    CONSTRAINT FK_Staffs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE SET NULL
);
GO

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
    Email NVARCHAR(100) NULL CONSTRAINT UQ_Students_Email UNIQUE,
    Picture VARBINARY(MAX) NULL, 
    MaLop VARCHAR(20) NULL,                      -- Đã sửa thành VARCHAR(20) để đồng bộ hoàn toàn với bảng Classroom
    MaNganh CHAR(10) NULL,          
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,

    CONSTRAINT FK_Students_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Students_Classroom FOREIGN KEY (MaLop) REFERENCES dbo.Classroom(MaLop) ON DELETE SET NULL,
    CONSTRAINT FK_Students_Major FOREIGN KEY (MaNganh) REFERENCES dbo.Major(MaNganh) ON DELETE SET NULL
);
GO


-- ============================================================================
-- 3. PHÂN HỆ QUẢN LÝ ĐÀO TẠO THEO TÍN CHỈ (LỚP HỌC PHẦN)
-- ============================================================================

CREATE TABLE dbo.CourseSection (
    MaLopHP VARCHAR(30) NOT NULL PRIMARY KEY, 
    MaMH CHAR(10) NOT NULL,                                                                                                                                                                     
    HocKy INT NOT NULL,                                                                                                                                                                         
    NamHoc NVARCHAR(20) NOT NULL,                               
    MSGV NVARCHAR(30) NULL,                      -- Đã sửa từ NVARCHAR sang liên kết mã số giảng viên (MSGV) làm việc thực tế
    PhongHoc NVARCHAR(50) NULL,
    MaxStudents INT DEFAULT 50 CONSTRAINT CK_CourseSection_MaxStudents CHECK (MaxStudents > 0),   
    Status INT DEFAULT 1,         
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,     
    CONSTRAINT FK_CourseSection_Course FOREIGN KEY (MaMH) REFERENCES dbo.Course(MaMH) ON DELETE CASCADE,
    CONSTRAINT FK_CourseSection_Teachers FOREIGN KEY (MSGV) REFERENCES dbo.Teachers(MSGV) ON DELETE SET NULL
);
GO

CREATE TABLE dbo.TeachingAssignment
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    HRID INT NOT NULL,                
    MaMH CHAR(10) NOT NULL,           
    
    CONSTRAINT FK_Assignment_Users FOREIGN KEY (HRID) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Assignment_Course FOREIGN KEY (MaMH) REFERENCES dbo.Course(MaMH) ON DELETE CASCADE,
    CONSTRAINT UC_HR_MonHoc UNIQUE (HRID, MaMH)
);
GO

CREATE TABLE dbo.DKMH (
    MSSV NVARCHAR(30) NOT NULL,
    MaLopHP VARCHAR(30) NOT NULL,
    RegistrationDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (MSSV, MaLopHP),
    CONSTRAINT FK_DKMH_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE,
    CONSTRAINT FK_DKMH_CourseSection FOREIGN KEY (MaLopHP) REFERENCES dbo.CourseSection(MaLopHP) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Score (
    MSSV NVARCHAR(30) NOT NULL,
    MaLopHP VARCHAR(30) NOT NULL,
    DiemQT DECIMAL(4,2) NULL,
    DiemCK DECIMAL(4,2) NULL,
    DiemTK DECIMAL(4,2) NULL,
    Mota NVARCHAR(200) NULL,
    PRIMARY KEY (MSSV, MaLopHP),
    CONSTRAINT FK_Score_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE, 
    CONSTRAINT FK_Score_CourseSection FOREIGN KEY (MaLopHP) REFERENCES dbo.CourseSection(MaLopHP) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Requests
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    MSSV NVARCHAR(30) NOT NULL,
    RequestType NVARCHAR(50) NULL, 
    RequestContent NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) DEFAULT N'Pending', 
    AdminComment NVARCHAR(200) NULL,       
    Created_At DATETIME DEFAULT GETDATE(),
    Updated_At DATETIME NULL,
    CONSTRAINT FK_Requests_Students FOREIGN KEY (MSSV) REFERENCES dbo.Students(MSSV) ON DELETE CASCADE
);
GO


-- ============================================================================
-- 4. HIỆU NĂNG TRUY VẤN (INDEXES) & TRÌNH XEM (VIEWS)
-- ============================================================================

CREATE INDEX IX_Students_UserId ON dbo.Students(UserId);     
CREATE INDEX IX_Users_Username ON dbo.Users(Username);        
CREATE INDEX IX_Score_MSSV ON dbo.Score(MSSV);               
CREATE INDEX IX_Requests_MSSV ON dbo.Requests(MSSV);         
CREATE INDEX IX_DKMH_MaLopHP ON dbo.DKMH(MaLopHP);   
CREATE NONCLUSTERED INDEX IX_Contact_User_Group ON dbo.Contact(UserID, Group_ID) INCLUDE (Name, Phone, Email);
CREATE NONCLUSTERED INDEX IX_Groups_User ON dbo.Groups(UserID) INCLUDE (Name);
CREATE NONCLUSTERED INDEX IX_Teachers_UserId ON dbo.Teachers(UserId);
CREATE NONCLUSTERED INDEX IX_Teachers_MSGV ON dbo.Teachers(MSGV);
CREATE UNIQUE NONCLUSTERED INDEX UX_Teachers_Phone 
ON dbo.Teachers(Phone) WHERE Phone IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Major_TenNganh ON dbo.Major(TenNganh);
GO

CREATE OR ALTER VIEW dbo.vw_StudentTranscript
AS
SELECT
    s.MSSV,
    s.FirstName + ' ' + s.LastName AS StudentName,
    cs.MaLopHP,
    c.MaMH,
    c.TenMH,
    c.SoTC,
    sc.DiemQT,
    sc.DiemCK,
    sc.DiemTK,
    cs.NamHoc,
    cs.HocKy
FROM dbo.Score sc
INNER JOIN dbo.CourseSection cs ON sc.MaLopHP = cs.MaLopHP
INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
INNER JOIN dbo.Students s ON sc.MSSV = s.MSSV;
GO

CREATE OR ALTER VIEW dbo.vw_StudentRegistrationDetail
AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY dk.RegistrationDate DESC) AS STT,
    dk.MSSV,
    dk.MaLopHP,
    c.TenMH,
    c.SoTC,
    (t.LastName + ' ' + t.FirstName) AS TenGiangVien, -- Ghép họ tên giảng viên từ bảng Teachers
    cs.PhongHoc,
    dk.RegistrationDate
FROM dbo.DKMH dk
INNER JOIN dbo.CourseSection cs ON dk.MaLopHP = cs.MaLopHP
INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
LEFT JOIN dbo.Teachers t ON cs.MSGV = t.MSGV; -- Liên kết an toàn sang bảng giáo viên bằng MSGV
GO