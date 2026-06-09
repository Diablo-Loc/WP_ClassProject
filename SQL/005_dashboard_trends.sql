USE LoginDB;
GO

-- =====================================================================
-- THỦ TỤC LẤY XU HƯỚNG NHẬP HỌC (BIỂU ĐỒ ĐƯỜNG) THEO THỜI GIAN THỰC
-- =====================================================================
CREATE OR ALTER PROC proc_GetEnrollmentTrendStatistics
AS
BEGIN
    SET NOCOUNT ON;

    -- Nhóm dữ liệu sinh viên đăng ký học theo Tháng/Năm từ bảng Students
    SELECT 
        FORMAT(Created_At, 'MM/yyyy') AS MonthYear, 
        COUNT(Id) AS Total
    FROM dbo.Students
    GROUP BY FORMAT(Created_At, 'MM/yyyy'), YEAR(Created_At), MONTH(Created_At)
    ORDER BY YEAR(Created_At), MONTH(Created_At);
END;
GO
USE LoginDB;
GO

-- THỦ TỤC LẤY XU HƯỚNG TÀI KHOẢN/SINH VIÊN MỚI THEO THÁNG
CREATE OR ALTER PROC proc_GetEnrollmentTrendStatistics
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

USE LoginDB;
GO

CREATE OR ALTER PROC proc_GetDashboardSummaryCards
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Đếm tổng số sinh viên thực tế trong hệ thống
    DECLARE @TotalStudents INT;
    SELECT @TotalStudents = COUNT(*) FROM dbo.Students;

    -- 2. Đếm số sinh viên mới nhập học (giả sử tính trong tháng hiện tại)
    DECLARE @NewAdmissions INT;
    SELECT @NewAdmissions = COUNT(*) 
    FROM dbo.Students 
    WHERE MONTH(Created_At) = MONTH(GETDATE()) AND YEAR(Created_At) = YEAR(GETDATE());

    -- 3. Tính tỷ lệ qua môn trung bình (DiemTK >= 5.0) trên toàn hệ thống bảng điểm
    DECLARE @PassRate DECIMAL(5,1) = 0.0;
    IF EXISTS (SELECT 1 FROM dbo.Score)
    BEGIN
        SELECT @PassRate = ROUND((CAST(SUM(CASE WHEN DiemTK >= 5.0 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*)) * 100, 1)
        FROM dbo.Score;
    END

    -- Trả về một dòng kết quả duy nhất chứa toàn bộ các con số thực tế
    SELECT 
        ISNULL(@TotalStudents, 0) AS TotalStudents,
        ISNULL(@NewAdmissions, 0) AS NewAdmissions,
        ISNULL(@PassRate, 0.0) AS PassRate;
END;
GO

USE LoginDB;
GO

-- 1. Bổ sung môn học mẫu nếu chưa có
IF NOT EXISTS (SELECT 1 FROM dbo.Course WHERE MaMH = 'MH001')
BEGIN
    INSERT INTO dbo.Course (MaMH, TenMH, SoTC, Tuan, Hky, NamHoc, Mota)
    VALUES ('MH001', N'Lập trình Windows', 3, 15, 1, N'2025-2026', N'Môn học thực hành C#');
END

-- 2. Bổ sung điểm số mẫu cho sinh viên thực tế (MSSV: 22110158)
-- Nếu bạn có MSSV khác, hãy thay thế '22110158' thành MSSV thật bạn đã add nhé!
IF EXISTS (SELECT 1 FROM dbo.Students WHERE MSSV = N'22110158')
   AND NOT EXISTS (SELECT 1 FROM dbo.Score WHERE MSSV = N'22110158' AND MaMH = 'MH001')
BEGIN
    INSERT INTO dbo.Score (MSSV, MaMH, DiemQT, DiemCK, DiemTK, Mota)
    VALUES (N'22110158', 'MH001', 8.0, 9.0, 8.7, N'Điểm tổng kết kỳ 1');
END

-- 3. Bổ sung thêm điểm cho một vài MSSV khác của bạn tại đây để biểu đồ tròn chia múi đẹp hơn
-- INSERT INTO dbo.Score (MSSV, MaMH, DiemTK) VALUES (N'MSSV_CỦA_BẠN', 'MH001', 6.5);
-- INSERT INTO dbo.Score (MSSV, MaMH, DiemTK) VALUES (N'MSSV_CỦA_BẠN_2', 'MH001', 4.0);
GO