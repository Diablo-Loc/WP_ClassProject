@echo off
title He Thong Khoi Tao Database - Auto Fix Error
cls

echo ==================================================
echo   DANG KICH HOAT POWERSHELL (CHE DO AN TOAN)...
echo ==================================================

:: BƯỚC DỰ PHÒNG 1: Ép chạy đúng thư mục hiện tại để tránh lỗi quyền hệ thống
cd /d "%~dp0"

:: BƯỚC DỰ PHÒNG 2: Kiểm tra xem file Run_SetupDB.ps1 có bị mất hay đổi tên không
if not exist "Run_SetupDB.ps1" (
    color 0C
    echo [LOI NGUYEN NHAN]: Khong tim thay file 'Run_SetupDB.ps1' trong thu muc nay!
    echo [GIAI PHAP]: Hay chac chan ban dat file .bat nay cung cho voi file .ps1.
    goto KETTHUC
)

:: BƯỚC DỰ PHÒNG 3: Chạy PowerShell với cấu hình Bypass mạnh nhất (Bỏ qua Execution Policy và Profile rác)
powershell -NoProfile -ExecutionPolicy Bypass -File "Run_SetupDB.ps1"

:: BƯỚC DỰ PHÒNG 4: Nếu PowerShell bị lỗi hệ thống (ví dụ bị Windows khóa hoàn toàn)
if %ERRORLEVEL% NEQ 0 (
    if %ERRORLEVEL% EQU 9009 (
        color 0C
        echo [LOI NGUYEN NHAN]: Lenh 'powershell' khong hop le hoac bi chan tren may nay!
        echo [GIAI PHAP]: Ban hay nhap chuot phai vao file Run_SetupDB.ps1 -^> Chon 'Run with PowerShell'.
    )
)

:KETTHUC
echo.
echo --------------------------------------------------
echo Tien trinh ket thuc. Cua so duoc giu lai de kiem tra.
pause