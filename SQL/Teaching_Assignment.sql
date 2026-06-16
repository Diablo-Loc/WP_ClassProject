-- =====================================================================
-- 1. TẠO BẢNG PHÂN CÔNG GIẢNG DẠY (TEACHING ASSIGNMENT)
-- =====================================================================
IF OBJECT_ID(N'dbo.TeachingAssignment', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TeachingAssignment
    (
        ID INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính tự tăng
        HRID INT NOT NULL,                -- Liên kết trực tiếp tới Id của tài khoản Giảng viên (Users.Id)
        MaMH CHAR(10) NOT NULL,           -- Khóa ngoại liên kết tới Course.MaMH (CHAR(10) khớp bảng Course)
        
        -- Tạo khóa ngoại ràng buộc dữ liệu toàn vẹn hệ thống
        CONSTRAINT FK_Assignment_Users FOREIGN KEY (HRID) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Assignment_Course FOREIGN KEY (MaMH) REFERENCES dbo.Course(MaMH) ON DELETE CASCADE,
        
        -- CHỐNG TRÙNG LẶP: Không cho phép phân công một môn học cho cùng một giảng viên nhiều lần
        CONSTRAINT UC_HR_MonHoc UNIQUE (HRID, MaMH)
    );
END;
GO

-- =====================================================================
-- 2. THỦ TỤC LẤY DANH SÁCH PHÂN CÔNG (HIỂN THỊ LÊN GRIDVIEW)
-- =====================================================================
CREATE OR ALTER PROC proc_GetTeachingAssignments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ta.ID,                       -- Dùng ID ẩn trên Grid để xử lý hành động Xóa/Hủy phân công
        ta.HRID,                     -- Mã định danh tài khoản Giảng viên
        u.Username AS HRName,        -- Hiển thị Tên tài khoản hoặc Tên Giảng viên
        ta.MaMH,                     -- Mã môn học
        c.TenMH                      -- Tên môn học trực quan
    FROM dbo.TeachingAssignment ta
    INNER JOIN dbo.Users u ON ta.HRID = u.Id
    INNER JOIN dbo.Course c ON ta.MaMH = c.MaMH
    ORDER BY ta.ID DESC;             -- Đẩy các bản ghi phân công mới nhất lên trên đầu bảng
END;
GO

CREATE OR ALTER PROC proc_GetTeachingAssignments_Report
    @HRID INT = NULL,       
    @MaMH CHAR(10) = NULL     
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ta.ID,                       
        ta.HRID,                     
        u.Username AS HRName,        
        ta.MaMH,                     
        c.TenMH                      
    FROM dbo.TeachingAssignment ta
    INNER JOIN dbo.Users u ON ta.HRID = u.Id
    INNER JOIN dbo.Course c ON ta.MaMH = c.MaMH
    WHERE 
        (@HRID IS NULL OR ta.HRID = @HRID)
        AND (@MaMH IS NULL OR ta.MaMH = @MaMH)
    ORDER BY ta.ID DESC;             
END;
GO