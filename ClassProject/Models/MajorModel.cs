using System;

namespace ClassProject.DataAccess.Models
{
    public class MajorModel
    {
        public string MaNganh { get; set; }
        public string TenNganh { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public MajorModel() { }

        public MajorModel(string maNganh, string tenNganh)
        {
            this.MaNganh = maNganh;
            this.TenNganh = tenNganh;
        }
    }
}