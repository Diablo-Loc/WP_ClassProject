namespace ClassProject.Models
{
    public class Score
    {
        public string MSSV { get; set; }
        public string MaLopHP { get; set; }

        // Dùng decimal? để khớp chính xác DECIMAL(4,2) và cho phép NULL khi chưa có điểm
        public decimal? DiemQT { get; set; }
        public decimal? DiemCK { get; set; }
        public decimal? DiemTK { get; set; }

        public string Mota { get; set; }

        // Thuộc tính hỗ trợ hiển thị trên Form bảng điểm của sinh viên
        public string TenMH { get; set; }
        public int SoTC { get; set; }

        public Score() { }
    }
}