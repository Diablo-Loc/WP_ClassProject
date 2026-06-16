-- ============================================================================
-- FILE 02: ĐỒNG BỘ TOÀN BỘ CÁC THỦ TỤC LƯU TRỮ (STORED PROCEDURES)
-- ============================================================================
USE LoginDB;
GO

-- 1. THỦ TỤC LẤY DANH SÁCH PHÂN CÔNG GIẢNG DẠY (Đã sửa để lấy Tên Giảng Viên thay vì Username)
CREATE OR ALTER PROC dbo.proc_GetTeachingAssignments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ta.ID,                       
        ta.HRID,                     
        (t.FirstName + ' ' + t.LastName) AS HRName, -- Lấy tên thật từ hồ sơ Teachers
        ta.MaMH,                     
        c.TenMH,
        (SELECT COUNT(*) FROM dbo.TeachingAssignment sub WHERE sub.HRID = ta.HRID) AS TotalAssigned                      
    FROM dbo.TeachingAssignment ta
    INNER JOIN dbo.Users u ON ta.HRID = u.Id
    LEFT JOIN dbo.Teachers t ON u.Id = t.UserId -- Liên kết sang hồ sơ giáo viên
    INNER JOIN dbo.Course c ON ta.MaMH = c.MaMH
    ORDER BY ta.ID DESC;             
END;
GO

-- 2. THỦ TỤC BÁO CÁO PHÂN CÔNG THEO BỘ LỌC COMBOBOX
CREATE OR ALTER PROC dbo.proc_GetTeachingAssignments_Report
    @HRID INT = NULL,       
    @MaMH CHAR(10) = NULL     
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ta.ID,                       
        ta.HRID,                     
        (t.FirstName + ' ' + t.LastName) AS HRName,                      
        ta.MaMH,                     
        c.TenMH                      
    FROM dbo.TeachingAssignment ta
    INNER JOIN dbo.Users u ON ta.HRID = u.Id
    LEFT JOIN dbo.Teachers t ON u.Id = t.UserId
    INNER JOIN dbo.Course c ON ta.MaMH = c.MaMH
    WHERE 
        (@HRID IS NULL OR ta.HRID = @HRID)
        AND (@MaMH IS NULL OR ta.MaMH = @MaMH)
    ORDER BY ta.ID DESC;             
END;
GO

-- 3. THỦ TỤC LẤY XU HƯỚNG NHẬP HỌC TRONG NĂM (VẼ BIỂU ĐỒ ĐƯỜNG - LINE CHART)
CREATE OR ALTER PROC dbo.proc_GetEnrollmentTrendStatistics
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FORMAT(Created_At, 'MM/yyyy') AS MonthYear, 
        COUNT(Id) AS Total
    FROM dbo.Students
    GROUP BY FORMAT(Created_At, 'MM/yyyy'), YEAR(Created_At), MONTH(Created_At)
    ORDER BY YEAR(Created_At), MONTH(Created_At);
END;
GO

-- 4. THỦ TỤC TÍNH TOÁN CÁC THẺ TRẠNG THÁI TRÊN DASHBOARD CHÍNH (Tối ưu logic tính điểm)
CREATE OR ALTER PROC dbo.proc_GetDashboardSummaryCards
AS
BEGIN
    SET NOCOUNT ON;

    -- Đếm tổng số sinh viên thực tế trong hệ thống
    DECLARE @TotalStudents INT;
    SELECT @TotalStudents = COUNT(*) FROM dbo.Students;

    -- Đếm số sinh viên mới nhập học trong tháng hiện tại
    DECLARE @NewAdmissions INT;
    SELECT @NewAdmissions = COUNT(*) 
    FROM dbo.Students 
    WHERE MONTH(Created_At) = MONTH(GETDATE()) AND YEAR(Created_At) = YEAR(GETDATE());

    -- Tính tỷ lệ qua môn trung bình (Chỉ tính khi các đầu điểm thành phần đã nhập đủ)
    DECLARE @PassRate DECIMAL(5,1) = 0.0;
    IF EXISTS (SELECT 1 FROM dbo.Score WHERE DiemQT IS NOT NULL AND DiemCK IS NOT NULL)
    BEGIN
        SELECT @PassRate = ROUND((CAST(SUM(CASE WHEN DiemTK >= 5.0 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*)) * 100, 1)
        FROM dbo.Score
        WHERE DiemQT IS NOT NULL AND DiemCK IS NOT NULL;
    END

    -- Trả về một dòng dữ liệu duy nhất chứa toàn bộ chỉ số thực
    SELECT 
        ISNULL(@TotalStudents, 0) AS TotalStudents,
        ISNULL(@NewAdmissions, 0) AS NewAdmissions,
        ISNULL(@PassRate, 0.0) AS PassRate;
END;
GO

-- 5. THỦ TỤC LẤY DỮ LIỆU XẾP LOẠI HỌC LỰC CỦA SINH VIÊN (VẼ BIỂU ĐỒ TRÒN - PIE CHART)
CREATE OR ALTER PROC dbo.proc_GetAcademicRankingStatistics
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        DiemSinhVien.XepLoai AS RankingGroup,
        COUNT(*) AS StudentCount
    FROM  
    (
        SELECT 
            MSSV,
            CASE 
                WHEN AVG(DiemTK) >= 9.0 AND AVG(DiemTK) <= 10.0 THEN N'Xuất sắc'
                WHEN AVG(DiemTK) >= 8.0 AND AVG(DiemTK) < 9.0 THEN N'Giỏi'
                WHEN AVG(DiemTK) >= 6.5 AND AVG(DiemTK) < 8.0 THEN N'Khá'
                WHEN AVG(DiemTK) >= 5.0 AND AVG(DiemTK) < 6.5 THEN N'Trung bình'
                ELSE N'Yếu/Kém'
            END AS XepLoai
        FROM dbo.Score
        WHERE DiemTK IS NOT NULL
        GROUP BY MSSV
    ) AS DiemSinhVien
    GROUP BY DiemSinhVien.XepLoai
    ORDER BY 
        CASE DiemSinhVien.XepLoai
            WHEN N'Xuất sắc' THEN 1
            WHEN N'Giỏi' THEN 2
            WHEN N'Khá' THEN 3
            WHEN N'Trung bình' THEN 4
            ELSE 5
        END;
END;
GO

-- 6. THỦ TỤC TRUY VẤN BẢNG THÀNH TÍCH TOP 10 SINH VIÊN ĐỨNG ĐẦU TRƯỜNG
CREATE OR ALTER PROC dbo.proc_GetTopStudentsRanking
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 10
        s.MSSV,
        (s.FirstName + ' ' + s.LastName) AS FullName,
        ROUND(AVG(sc.DiemTK), 2) AS GPA,
        CASE 
            WHEN AVG(sc.DiemTK) >= 9.0 THEN N'Xuất sắc'
            WHEN AVG(sc.DiemTK) >= 8.0 THEN N'Giỏi'
            WHEN AVG(sc.DiemTK) >= 6.5 THEN N'Khá'
            WHEN AVG(sc.DiemTK) >= 5.0 THEN N'Trung bình'
            ELSE N'Yếu'
        END AS Classification
    FROM dbo.Students s
    INNER JOIN dbo.Score sc ON s.MSSV = sc.MSSV
    WHERE sc.DiemTK IS NOT NULL
    GROUP BY s.MSSV, s.FirstName, s.LastName
    ORDER BY GPA DESC;
END;
GO

-- ============================================================================
-- 7. BỔ SUNG: CÁC THỦ TỤC LƯU TRỮ CHO PHÂN HỆ CLASSROOM (DẸP BỎ INLINE SQL)
-- ============================================================================

-- Proc 7.1: Lấy danh sách toàn bộ lớp học kèm tên ngành (Đổ ra DataGridView)
CREATE OR ALTER PROC dbo.proc_GetAllClassrooms
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.MaLop, 
        c.TenLop, 
        c.SiSo, 
        c.GVCN, 
        c.MaNganh, 
        m.TenNganh
    FROM dbo.Classroom c
    LEFT JOIN dbo.Major m ON c.MaNganh = m.MaNganh
    ORDER BY c.Created_At DESC;
END;
GO

-- Proc 7.2: Lấy danh sách Giảng viên hoạt động để làm GVCN (Đổ vào ComboBox)
CREATE OR ALTER PROC dbo.proc_GetActiveTeachers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        u.Username
    FROM dbo.Teachers t
    INNER JOIN dbo.Users u ON t.UserId = u.Id
    WHERE t.Status = 1;
END;
GO

-- Proc 7.3: Lấy danh sách Ngành học (Đổ vào ComboBox)
CREATE OR ALTER PROC dbo.proc_GetAllMajors
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNganh, TenNganh FROM dbo.Major ORDER BY TenNganh ASC;
END;
GO