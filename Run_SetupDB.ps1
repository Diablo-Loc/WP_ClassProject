Set-Location $PSScriptRoot

# Bạn có thể để mặc định là tên máy của bạn để tiện test nhanh
$DefaultServer = ".\SQLEXPRESS" 

Write-Host "=== HE THONG KHOI TAO DATABASE ===" -ForegroundColor Cyan
Write-Host "Thu muc: $PSScriptRoot" -ForegroundColor Gray
Write-Host ""

# ---------------------------------------------------------------------------------
# PHƯƠNG ÁN DỰ PHÒNG 1: TỰ ĐỘNG CÀI SQLCMD NẾU MÁY KHÁC BỊ THIẾU
# ---------------------------------------------------------------------------------
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    Write-Host "⚠️ CANH BAO: May này chua cai dat cong cu 'sqlcmd' cua Microsoft!" -ForegroundColor Yellow
    Write-Host "🤖 Dang tu dong tai va cai dat 'sqlcmd' qua winget ngam..." -ForegroundColor Cyan
    
    # Lệnh tải ngầm chính chủ từ Microsoft Store/Winget
    winget install --id Microsoft.go-sqlcmd --silent --accept-source-agreements --accept-package-agreements
    
    # Nghỉ 3 giây để Windows load lại môi trường biến hệ thống
    Start-Sleep -Seconds 3
    
    # Kiểm tra lại sau khi cài
    if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
        Write-Host "❌ LOI: Khong the tu dong cai 'sqlcmd'. Bạn hay mo CMD va go lenh sau:" -ForegroundColor Red
        Write-Host "   winget install Microsoft.go-sqlcmd" -ForegroundColor White
        pause
        Exit
    } else {
        Write-Host "✅ TU DONG SUA LOI THANH CONG! Da cai xong sqlcmd. Tiep tuc..." -ForegroundColor Green
    }
}

# ---------------------------------------------------------------------------------
# TIẾP TỤC TIẾN TRÌNH NHẬP TÊN SERVER
# ---------------------------------------------------------------------------------
Write-Host "Nhap ten SQL Server cua ban va bam Enter." -ForegroundColor Yellow
Write-Host "Khong biet thi mo SQLSERVER len --> COPY NGUYEN ||SERVER NAME||--> PASTE -->ENTER!"-ForegroundColor Green
Write-Host "Mac dinh la: $DefaultServer (Bam Enter neu dung luon): " -NoNewline -ForegroundColor Yellow
$UserInput = Read-Host

if ([string]::IsNullOrWhiteSpace($UserInput)) {
    $ServerName = $DefaultServer
} else {
    $ServerName = $UserInput
}

$DatabaseName = "LoginDB"    

$SqlFiles = @(
    "SQL\01_init_tables.sql",
    "SQL\02_stored_procedures.sql",
    "SQL\03_seed_data.sql",
    "SQL\04_DatabaseTriggers.sql"
)

Write-Host ""
Write-Host "Dang ket noi den Server: $ServerName ..." -ForegroundColor Cyan

$Status = $true
foreach ($file in $SqlFiles) {
    if (Test-Path $file) {
        Write-Host "Dang chay file: $file ..." -ForegroundColor Yellow
        
        sqlcmd -S "$ServerName" -d "$DatabaseName" -i "$file" -b
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ LOI TAI FILE: $file" -ForegroundColor Red
            # ---------------------------------------------------------------------
            # PHƯƠNG ÁN DỰ PHÒNG 2: GỢI Ý HƯỚNG DẪN KHI BỊ LỖI TRÙNG LOGIC DB
            # ---------------------------------------------------------------------
            Write-Host "👉 GOI Y: Neu la loi trung lap bang (Object already named), hay vao SSMS xoa DB '$DatabaseName' di roi chay lai!" -ForegroundColor DarkCyan
            $Status = $false
            break
        }
    } else {
        Write-Host "CANH BAO: Khong tim thay file $file" -ForegroundColor DarkYellow
    }
}

Write-Host ""
if ($Status) {
    Write-Host "XONG: Database da duoc nap thanh cong!" -ForegroundColor Green
} else {
    Write-Host "THAT BAI: Vui long kiem tra dong loi hoac huong dan o tren!" -ForegroundColor Red
}

pause