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

-- NÂNG CẤP: Bảng quản lý Nhóm/Phòng ban dạng Cây Phân Cấp (Hierarchy)
CREATE TABLE dbo.Groups (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    GroupCode VARCHAR(50) NULL,          -- Mã phòng ban (CNTT, HCNS, TO-AI,...)
    ParentID INT NULL,                  -- Đệ quy: NULL nếu là cấp cao nhất (Khối/Phòng gốc)
    IsSystemData BIT DEFAULT 0,         -- 1: Phòng ban của Trường, 0: Nhóm cá nhân tự tạo
    UserID INT NOT NULL,                -- User sở hữu hoặc tạo lập nhóm này
    CONSTRAINT FK_Groups_Parent FOREIGN KEY (ParentID) REFERENCES dbo.Groups(ID),
    CONSTRAINT FK_Groups_Users FOREIGN KEY (UserID) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);
GO

-- CHUẨN HÓA: Bảng Contact cá nhân (Không còn cột Group_ID trực tiếp)
CREATE TABLE dbo.Contact (
    ContactID INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(100) NOT NULL,             
    Fname NVARCHAR(50) NOT NULL,             
    Lname NVARCHAR(50) NULL,              
    Dob DATETIME NULL,
    Gender NVARCHAR(10) NULL,
    Phone NVARCHAR(15) NULL,
    Address NVARCHAR(200) NULL,
    Email NVARCHAR(100) NULL,
    Pic IMAGE NULL,
    UserID INT NOT NULL,
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

CREATE TABLE dbo.MemberGroupMappings
(
    UniqueID VARCHAR(50) NOT NULL,      -- Định danh chuỗi: 'TEACHER_1', 'CONTACT_12'
    GroupID INT NOT NULL,
    IsPrimary BIT DEFAULT 1,            -- 1: Phòng ban chính (Dùng để hiển thị mặc định), 0: Kiêm nhiệm/Nhóm phụ
    AssignedAt DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT PK_MemberGroupMappings PRIMARY KEY (UniqueID, GroupID),
    CONSTRAINT FK_Mappings_Groups FOREIGN KEY (GroupID) REFERENCES dbo.Groups(ID) ON DELETE CASCADE
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
    MSGV NVARCHAR(30) NULL,                      -- Liên kết mã số giảng viên (MSGV) làm việc thực tế
    PhongHoc NVARCHAR(50) NULL,
    MaxStudents INT DEFAULT 50 CONSTRAINT CK_CourseSection_MaxStudents CHECK (MaxStudents > 0),   
    Status INT DEFAULT 1,          
    
    ThuHoc INT NOT NULL DEFAULT 2 CONSTRAINT CK_CourseSection_Thu CHECK (ThuHoc BETWEEN 2 AND 7), -- 2: Thứ 2 -> 7: Thứ 7 (Mặc định xếp vào Thứ 2 để không lỗi Form cũ)
    CaHoc INT NOT NULL DEFAULT 1 CONSTRAINT CK_CourseSection_Ca CHECK (CaHoc BETWEEN 1 AND 4),    -- 1: Ca 1 -> 4: Ca 4 (Mặc định xếp vào Ca 1)

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

CREATE TABLE UserLoginLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    LoginTime DATETIME NOT NULL DEFAULT GETDATE(),
    IsSuccess BIT NOT NULL,                 -- 1: Thành công, 0: Thất bại
    LoginMethod NVARCHAR(20) NOT NULL,       -- 'PASSWORD' hoặc 'FACE_ID'
    IPAddress NVARCHAR(45) NULL,            -- Lưu được cả IPv4 và IPv6
    UserAgent NVARCHAR(500) NULL,           -- Lưu thông tin thiết bị/HĐH (Windows 11, WinForms App...)
    FailureReason NVARCHAR(250) NULL        -- Lý do sai: 'Wrong Password', 'Face Not Match', 'Account Locked'
);


-- ============================================================================
-- 4. HIỆU NĂNG TRUY VẤN (INDEXES) & TRÌNH XEM (VIEWS)
-- ============================================================================

CREATE INDEX IX_Students_UserId ON dbo.Students(UserId);     
CREATE INDEX IX_Users_Username ON dbo.Users(Username);        
CREATE INDEX IX_Score_MSSV ON dbo.Score(MSSV);               
CREATE INDEX IX_Requests_MSSV ON dbo.Requests(MSSV);         
CREATE INDEX IX_DKMH_MaLopHP ON dbo.DKMH(MaLopHP);   
CREATE NONCLUSTERED INDEX IX_Contact_User ON dbo.Contact(UserID) INCLUDE (Name, Phone, Email);
CREATE NONCLUSTERED INDEX IX_Mappings_GroupID ON dbo.MemberGroupMappings(GroupID) INCLUDE (UniqueID, IsPrimary);
CREATE NONCLUSTERED INDEX IX_Groups_User ON dbo.Groups(UserID) INCLUDE (Name);
CREATE NONCLUSTERED INDEX IX_Teachers_UserId ON dbo.Teachers(UserId);
CREATE NONCLUSTERED INDEX IX_Teachers_MSGV ON dbo.Teachers(MSGV);
CREATE UNIQUE NONCLUSTERED INDEX UX_Teachers_Phone 
ON dbo.Teachers(Phone) WHERE Phone IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Major_TenNganh ON dbo.Major(TenNganh);
CREATE NONCLUSTERED INDEX IX_UserLoginLogs_Username_LoginTime 
ON UserLoginLogs (Username, LoginTime);
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

CREATE OR ALTER VIEW dbo.vw_StudentDailySchedule
AS
SELECT 
    dk.MSSV,
    cs.MaLopHP,
    c.TenMH,
    cs.PhongHoc,
    cs.ThuHoc,
    cs.CaHoc,
    CASE cs.CaHoc
        WHEN 1 THEN N'07:30 - 09:50'
        WHEN 2 THEN N'10:00 - 12:20'
        WHEN 3 THEN N'13:00 - 15:20'
        WHEN 4 THEN N'15:30 - 17:50'
        ELSE N'Chưa xếp ca'
    END AS ThoiGian
FROM dbo.DKMH dk
INNER JOIN dbo.CourseSection cs ON dk.MaLopHP = cs.MaLopHP
INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
WHERE cs.Status = 1;
GO