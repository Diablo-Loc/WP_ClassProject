-- =====================================================================
-- 1. THỦ TỤC LẤY DỮ LIỆU XẾP LOẠI ĐỂ VẼ PIE CHART (ĐÃ FIX LỖI GOM NHÓM)
-- =====================================================================
CREATE OR ALTER PROC proc_GetAcademicRankingStatistics
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

-- =====================================================================
-- 2. THỦ TỤC LẤY TOP 10 SINH VIÊN XUẤT SẮC NHẤT (RANKING & GPA TABLE)
-- =====================================================================
CREATE OR ALTER PROC proc_GetTopStudentsRanking
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
    GROUP BY s.MSSV, s.FirstName, s.LastName
    ORDER BY GPA DESC;
END;
GO