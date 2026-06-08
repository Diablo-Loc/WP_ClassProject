IF OBJECT_ID('dbo.Classroom', 'U') IS NOT NULL
    DROP TABLE dbo.Classroom;
GO

CREATE TABLE Classroom
(
    MaLop VARCHAR(20) PRIMARY KEY,
    TenLop NVARCHAR(100) NOT NULL,
    SiSo INT,
    GVCN NVARCHAR(100)
);