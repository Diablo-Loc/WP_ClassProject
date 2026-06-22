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
        c.TenMH,
        -- Bổ sung dòng này để đồng bộ dữ liệu đếm số môn sang C#
        (SELECT COUNT(*) FROM dbo.TeachingAssignment sub WHERE sub.HRID = ta.HRID) AS TotalAssigned                      
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
    @MSSV NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @MSSV IS NOT NULL
    BEGIN
        -- BIỂU ĐỒ ĐƯỜNG CÁ NHÂN: Lấy điểm trung bình học kỳ theo dòng thời gian
        SELECT 
            (NamHoc + N' - HK' + CAST(HocKy AS NVARCHAR(2))) AS MonthYear,
            ROUND(AVG(DiemTK), 2) AS Total
        FROM dbo.vw_StudentTranscript
        WHERE MSSV = @MSSV
        GROUP BY NamHoc, HocKy
        ORDER BY NamHoc ASC, HocKy ASC;
    END
    ELSE
    BEGIN
        -- GIỮ NGUYÊN CODE CŨ CHO GIÁO VỤ
        SELECT FORMAT(Created_At, 'MM/yyyy') AS MonthYear, COUNT(Id) AS Total
        FROM dbo.Students
        GROUP BY FORMAT(Created_At, 'MM/yyyy'), YEAR(Created_At), MONTH(Created_At)
        ORDER BY YEAR(Created_At), MONTH(Created_At);
    END
END;
GO

-- 4. THỦ TỤC TÍNH TOÁN CÁC THẺ TRẠNG THÁI TRÊN DASHBOARD CHÍNH (Tối ưu logic tính điểm)
CREATE OR ALTER PROC dbo.proc_GetDashboardSummaryCards
    @MSSV NVARCHAR(30) = NULL -- Nếu truyền MSSV => Dashboard Sinh viên; Nếu NULL => Dashboard Giáo vụ
AS
BEGIN
    SET NOCOUNT ON;

    IF @MSSV IS NOT NULL
    BEGIN
        -- ====================================================================
        -- NGHIỆP VỤ: DASHBOARD CHO TÀI KHOẢN SINH VIÊN
        -- ====================================================================
        
        -- 1. Tính tổng số tín chỉ tích lũy (Điểm TK >= 4.0 hoặc 5.0 tùy quy chế)
        DECLARE @TinChiTichLuy INT;
        SELECT @TinChiTichLuy = ISNULL(SUM(SoTC), 0) 
        FROM dbo.vw_StudentTranscript 
        WHERE MSSV = @MSSV AND DiemTK >= 4.0;

        -- 2. Tính điểm GPA trung bình hệ 10
        DECLARE @GPATrungBinh DECIMAL(3,2);
        SELECT @GPATrungBinh = CASE WHEN SUM(SoTC) = 0 THEN 0 ELSE ROUND(SUM(DiemTK * SoTC) / SUM(SoTC), 2) END
        FROM dbo.vw_StudentTranscript 
        WHERE MSSV = @MSSV;

        -- 3. Số môn học trong học kỳ hiện tại (Ví dụ học kỳ gần nhất)
        DECLARE @MonHocKyNay INT;
        SELECT @MonHocKyNay = COUNT(*) FROM dbo.DKMH WHERE MSSV = @MSSV;

        -- 4. Tính tạm học phí công nợ (Số TC * 400.000đ)
        DECLARE @HocPhiCongNo DECIMAL(12,2);
        SELECT @HocPhiCongNo = ISNULL(SUM(c.SoTC), 0) * 400000 
        FROM dbo.DKMH dk
        INNER JOIN dbo.CourseSection cs ON dk.MaLopHP = cs.MaLopHP
        INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
        WHERE dk.MSSV = @MSSV;

        -- Trả về dữ liệu gói gọn cho C# của Sinh viên
        SELECT 
            @TinChiTichLuy AS TotalStudents, -- Giữ nguyên tên cột Aliases để không phải sửa tầng Repo C#
            @MonHocKyNay AS NewAdmissions,
            @GPATrungBinh AS PassRate,
            ISNULL(@HocPhiCongNo, 0) AS CustomHocPhi; 
    END
    ELSE
    BEGIN
        -- [TÍNH TOÁN DỮ LIỆU CHUẨN XÁC THỰC TẾ CHO GIÁO VỤ / ADMIN]
        DECLARE @TotalStudents INT = (SELECT COUNT(*) FROM dbo.Students);
        DECLARE @NewAdmissions INT = (SELECT COUNT(*) FROM dbo.Students WHERE MONTH(Created_At) = MONTH(GETDATE()) AND YEAR(Created_At) = YEAR(GETDATE()));
        
        DECLARE @PassRate DECIMAL(5,1) = 0.0;
        IF EXISTS (SELECT 1 FROM dbo.Score WHERE DiemTK IS NOT NULL)
        BEGIN
            SELECT @PassRate = ROUND((CAST(SUM(CASE WHEN DiemTK >= 5.0 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(*)) * 100, 1) 
            FROM dbo.Score WHERE DiemTK IS NOT NULL;
        END

        -- Đếm số lượng đơn thư thực tế dựa theo chuỗi văn bản lưu trong bảng Requests
        DECLARE @ResolvedRequests INT = (SELECT COUNT(*) FROM dbo.Requests WHERE Status = N'Approved' OR Status = N'Đã Giải Quyết'); 
        DECLARE @PendingRequests INT = (SELECT COUNT(*) FROM dbo.Requests WHERE Status = N'Pending' OR Status = N'Đang Xử Lý');
        DECLARE @RejectedRequests INT = (SELECT COUNT(*) FROM dbo.Requests WHERE Status = N'Rejected' OR Status = N'Denied' OR Status = N'Đã Từ Chối');
        
        DECLARE @TotalRequests INT = @ResolvedRequests + @PendingRequests + @RejectedRequests;
        
        DECLARE @CalculatedRate DECIMAL(5,1) = 0.0;
        IF @TotalRequests > 0
        BEGIN
            -- Tỷ lệ xử lý = Đơn đã giải quyết / Tổng số đơn
            SET @CalculatedRate = ROUND((CAST(@ResolvedRequests AS FLOAT) / @TotalRequests) * 100, 1);
        END

        SELECT 
            @TotalStudents AS TotalStudents, 
            @NewAdmissions AS NewAdmissions, 
            @PassRate AS PassRate, 
            0 AS CustomHocPhi,
            @CalculatedRate AS AttendanceRate, -- Đẩy tỷ lệ phần trăm thực tế lên Card
            @ResolvedRequests AS ResolvedCount, -- Đẩy số lượng đơn giải quyết xuống Chart
            @PendingRequests AS PendingCount,   -- Đẩy số lượng đơn đang xử lý xuống Chart
            @RejectedRequests AS RejectedCount; -- Đẩy số lượng đơn từ chối xuống Chart
    END
END;
GO

-- 5. THỦ TỤC LẤY DỮ LIỆU XẾP LOẠI HỌC LỰC CỦA SINH VIÊN (VẼ BIỂU ĐỒ TRÒN - PIE CHART)
CREATE OR ALTER PROC dbo.proc_GetAcademicRankingStatistics
    @MSSV NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @MSSV IS NOT NULL
    BEGIN
        -- BIỂU ĐỒ TRÒN CÁ NHÂN: Phân loại số lượng môn học theo mức điểm của Sinh viên
        SELECT 
            CASE 
                WHEN DiemTK >= 8.5 THEN N'Điểm A (Giỏi/Xuất sắc)'
                WHEN DiemTK >= 7.0 AND DiemTK < 8.5 THEN N'Điểm B (Khá)'
                WHEN DiemTK >= 5.5 AND DiemTK < 7.0 THEN N'Điểm C (Trung bình)'
                ELSE N'Điểm D/F (Yếu/Kém)'
            END AS RankingGroup,
            COUNT(*) AS StudentCount
        FROM dbo.Score
        WHERE MSSV = @MSSV AND DiemTK IS NOT NULL
        GROUP BY 
            CASE 
                WHEN DiemTK >= 8.5 THEN N'Điểm A (Giỏi/Xuất sắc)'
                WHEN DiemTK >= 7.0 AND DiemTK < 8.5 THEN N'Điểm B (Khá)'
                WHEN DiemTK >= 5.5 AND DiemTK < 7.0 THEN N'Điểm C (Trung bình)'
                ELSE N'Điểm D/F (Yếu/Kém)'
            END;
    END
    ELSE
    BEGIN
        -- GIỮ NGUYÊN CODE CŨ CHO GIÁO VỤ (Thống kê toàn trường)
        SELECT DiemSinhVien.XepLoai AS RankingGroup, COUNT(*) AS StudentCount
        FROM (
            SELECT MSSV,
                CASE 
                    WHEN AVG(DiemTK) >= 9.0 AND AVG(DiemTK) <= 10.0 THEN N'Xuất sắc'
                    WHEN AVG(DiemTK) >= 8.0 AND AVG(DiemTK) < 9.0 THEN N'Giỏi'
                    WHEN AVG(DiemTK) >= 6.5 AND AVG(DiemTK) < 8.0 THEN N'Khá'
                    WHEN AVG(DiemTK) >= 5.0 AND AVG(DiemTK) < 6.5 THEN N'Trung bình'
                    ELSE N'Yếu/Kém'
                END AS XepLoai
            FROM dbo.Score WHERE DiemTK IS NOT NULL GROUP BY MSSV
        ) AS DiemSinhVien
        GROUP BY DiemSinhVien.XepLoai;
    END
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

--8. Tự động tạo user và pass cho staff
CREATE OR ALTER PROCEDURE dbo.sp_CreateStaffAccount
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255), -- Mật khẩu băm BCrypt truyền từ C#
    @MSNV NVARCHAR(30),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Department NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Sử dụng TRANSACTION để đảm bảo an toàn dữ liệu (thêm cả 2 bảng hoặc không bảng nào cả)
    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Kiểm tra trùng lặp dữ liệu trước khi chèn
        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
        BEGIN
            RAISERROR(N'Tên đăng nhập (Username) này đã tồn tại trong hệ thống!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email) OR EXISTS (SELECT 1 FROM dbo.Staffs WHERE Email = @Email)
        BEGIN
            RAISERROR(N'Địa chỉ Email này đã được đăng ký!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Staffs WHERE MSNV = @MSNV)
        BEGIN
            RAISERROR(N'Mã số nhân viên (MSNV) này đã tồn tại!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Chèn vào bảng Users (RoleId = 3 đại diện cho Giáo Vụ)
        DECLARE @NewUserId INT;
        
        INSERT INTO dbo.Users (Username, Email, Password, RoleId, Valid, Status)
        VALUES (@Username, @Email, @PasswordHash, 3, 1, 1);
        
        -- Lấy Id vừa tự động sinh của bảng Users
        SET @NewUserId = SCOPE_IDENTITY();

        -- 3. Chèn vào bảng Staffs liên kết hồ sơ vật lý
        INSERT INTO dbo.Staffs (UserId, MSNV, FirstName, LastName, Phone, Email, Department, Status)
        VALUES (@NewUserId, @MSNV, @FirstName, @LastName, @Phone, @Email, @Department, 1);

        -- Nếu mọi thứ mượt mà, xác nhận lưu vào CSDL
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

--9.
CREATE or ALTER PROCEDURE dbo.sp_GetStaffAccountSecurityInfo
    @StaffId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Truy vấn lấy thẳng dữ liệu từ Id (Khóa chính), không sợ lỗi khoảng trắng MSNV
    SELECT 
        s.MSNV,
        u.Username, 
        u.Password, 
        s.Email,
        (s.LastName + ' ' + s.FirstName) AS FullName
    FROM dbo.Staffs s 
    INNER JOIN dbo.Users u ON s.UserId = u.Id 
    WHERE s.Id = @StaffId;
END;
GO

--8. tạo pass mới mỗi khi nhấn xem
CREATE or ALTER PROCEDURE dbo.sp_ResetStaffPassword
    @StaffId INT,
    @NewPasswordHash NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Cập nhật trực tiếp vào bảng Users thông qua UserId lấy từ bảng Staffs
    UPDATE dbo.Users
    SET Password = @NewPasswordHash
    WHERE Id = (SELECT UserId FROM dbo.Staffs WHERE Id = @StaffId);
END;
GO

-- STORE PROCEDURE: LẤY TOÀN BỘ DỮ LIỆU ĐỒ LÊN DASHBOARD GIẢNG VIÊN
CREATE OR ALTER PROCEDURE dbo.sp_GetTeacherDashboardData
    @TeacherId NVARCHAR(30) -- Truyền vào MSGV từ UserSession.TeacherId
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy thông tin học kỳ và năm học mới nhất mà giảng viên đang dạy để làm ngữ cảnh
    DECLARE @CurrentYear NVARCHAR(20), @CurrentSemester INT;
    
    SELECT TOP 1 @CurrentYear = NamHoc, @CurrentSemester = HocKy 
    FROM dbo.CourseSection 
    WHERE MSGV = @TeacherId 
    ORDER BY Created_At DESC;

    -- Nếu chưa có dữ liệu lớp học, gán mặc định tạm thời
    IF @CurrentYear IS NULL 
    BEGIN
        SET @CurrentYear = N'2025-2026';
        SET @CurrentSemester = 1;
    END

    -- ------------------------------------------------------------------------
    -- KẾT QUẢ 1: DỮ LIỆU CHO 4 THẺ CHỈ SỐ (SUMMARY CARDS)
    -- ------------------------------------------------------------------------
    SELECT 
        -- 1. Tổng số lớp học phần phụ trách trong học kỳ hiện tại
        (SELECT COUNT(*) FROM dbo.CourseSection WHERE MSGV = @TeacherId AND NamHoc = @CurrentYear AND HocKy = @CurrentSemester) AS TotalClasses,
        
        -- 2. Tổng số sinh viên đăng ký học các lớp của giảng viên này
        (SELECT COUNT(DISTINCT d.MSSV) 
         FROM dbo.DKMH d 
         INNER JOIN dbo.CourseSection cs ON d.MaLopHP = cs.MaLopHP 
         WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester) AS TotalStudents,
         
        -- 3. Điểm tổng kết trung bình của học sinh (Hệ 10)
        ISNULL((SELECT AVG(s.DiemTK) 
         FROM dbo.Score s
         INNER JOIN dbo.CourseSection cs ON s.MaLopHP = cs.MaLopHP
         WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester), 0.0) AS AvgScore,
         
        -- 4. Số lượng đầu điểm (Quá trình hoặc Cuối kỳ) chưa được nhập (bị NULL)
        (SELECT COUNT(*) 
         FROM dbo.Score s
         INNER JOIN dbo.CourseSection cs ON s.MaLopHP = cs.MaLopHP
         WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester 
           AND (s.DiemQT IS NULL OR s.DiemCK IS NULL)) AS PendingScores;

    -- ------------------------------------------------------------------------
    -- KẾT QUẢ 2: BIỂU ĐỒ CỘT - SỐ LƯỢNG SINH VIÊN MỖI LỚP HỌC PHẦN
    -- ------------------------------------------------------------------------
    SELECT 
        cs.MaLopHP, 
        c.TenMH,
        COUNT(d.MSSV) AS StudentCount
    FROM dbo.CourseSection cs
    INNER JOIN dbo.Course c ON cs.MaMH = c.MaMH
    LEFT JOIN dbo.DKMH d ON cs.MaLopHP = d.MaLopHP
    WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester
    GROUP BY cs.MaLopHP, c.TenMH;

    -- ------------------------------------------------------------------------
    -- KẾT QUẢ 3: BIỂU ĐỒ TRÒN - TỶ LỆ ĐẠT / TRƯỢT MÔN (PASS/FAIL)
    -- ------------------------------------------------------------------------
    SELECT 
        CASE 
            WHEN s.DiemTK IS NULL THEN N'Chưa có điểm'
            WHEN s.DiemTK >= 5.0 THEN N'Đạt (>= 5.0)'
            ELSE N'Trượt (< 5.0)'
        END AS StatusGroup,
        COUNT(*) AS Quantity
    FROM dbo.Score s
    INNER JOIN dbo.CourseSection cs ON s.MaLopHP = cs.MaLopHP
    WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester
    GROUP BY 
        CASE 
            WHEN s.DiemTK IS NULL THEN N'Chưa có điểm'
            WHEN s.DiemTK >= 5.0 THEN N'Đạt (>= 5.0)'
            ELSE N'Trượt (< 5.0)'
        END;

    -- ------------------------------------------------------------------------
    -- KẾT QUẢ 4: DANH SÁCH SINH VIÊN CÓ NGUY CƠ TRƯỢT (ĐIỂM QUÁ TRÌNH THẤP)
    -- ------------------------------------------------------------------------
    SELECT TOP 10
        s.MSSV,
        st.FirstName + ' ' + st.LastName AS FullName,
        s.MaLopHP,
        s.DiemQT,
        N'Điểm quá trình thấp' AS WarningReason
    FROM dbo.Score s
    INNER JOIN dbo.Students st ON s.MSSV = st.MSSV
    INNER JOIN dbo.CourseSection cs ON s.MaLopHP = cs.MaLopHP
    WHERE cs.MSGV = @TeacherId AND cs.NamHoc = @CurrentYear AND cs.HocKy = @CurrentSemester
      AND s.DiemQT < 4.0 -- Ngưỡng cảnh báo nguy cơ
    ORDER BY s.DiemQT ASC;
END;
GO

CREATE PROCEDURE dbo.proc_GetClassroomsByTeacher
    @TeacherUsername NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy cấu trúc danh sách lớp tương tự như hàm GetAllClassrooms của bạn 
    -- nhưng thêm điều kiện ép lọc theo cột GVCN
    SELECT c.MaLop, c.TenLop, c.SiSo, c.GVCN, c.MaNganh, m.TenNganh, 'Active' AS Status
    FROM dbo.Classroom c
    LEFT JOIN dbo.Major m ON c.MaNganh = m.MaNganh
    WHERE c.GVCN = @TeacherUsername
    ORDER BY c.MaLop ASC;
END;
GO