USE LoginDB;
GO
-- Trigger 01: Tự động tính Điểm tổng kết (Quá trình 30% + Cuối kỳ 70%)
CREATE OR ALTER TRIGGER trg_CalculateFinalScore
ON dbo.Score
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Score
    SET DiemTK = ROUND((inserted.DiemQT * 0.3) + (inserted.DiemCK * 0.7), 2)
    FROM dbo.Score
    INNER JOIN inserted ON dbo.Score.MSSV = inserted.MSSV AND dbo.Score.MaLopHP = inserted.MaLopHP
    WHERE inserted.DiemQT IS NOT NULL AND inserted.DiemCK IS NOT NULL;
END;
GO

-- Trigger 02: Tự động tính toán lại Sĩ số của lớp hành chính khi có biến động sinh viên
CREATE OR ALTER TRIGGER trg_UpdateClassroomSiSo
ON dbo.Students
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Xử lý khi thêm mới hoặc chuyển lớp sang lớp mới
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        UPDATE dbo.Classroom
        SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE Students.MaLop = Classroom.MaLop)
        WHERE MaLop IN (SELECT DISTINCT MaLop FROM inserted WHERE MaLop IS NOT NULL);
    END

    -- Xử lý khi xóa sinh viên hoặc chuyển đi khỏi lớp cũ
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        UPDATE dbo.Classroom
        SET SiSo = (SELECT COUNT(*) FROM dbo.Students WHERE Students.MaLop = Classroom.MaLop)
        WHERE MaLop IN (SELECT DISTINCT MaLop FROM deleted WHERE MaLop IS NOT NULL);
    END
END;
GO