using System;

namespace ClassProject.DataAccess.Entities
{
    public class DKMH
    {
        private string _mssv;
        private string _maLopHP;

        public string MSSV
        {
            get => _mssv;
            set => _mssv = value?.Trim();
        }

        public string MaLopHP
        {
            get => _maLopHP;
            set => _maLopHP = value?.Trim().ToUpper();
        }

        public DateTime RegistrationDate { get; set; }

        // Các thuộc tính hỗ trợ hiển thị nhanh trên GridView (Không lưu xuống bảng DKMH)
        public string TenMH { get; set; }
        public int SoTC { get; set; }

        public DKMH()
        {
            this.RegistrationDate = DateTime.Now;
        }

        public DKMH(string mssv, string maLopHP)
        {
            this.MSSV = mssv;
            this.MaLopHP = maLopHP;
            this.RegistrationDate = DateTime.Now;
        }
    }
}