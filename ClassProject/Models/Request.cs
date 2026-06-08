using System;
using System.Collections.Generic;
using System.Text;

namespace ClassProject.Models
{
    internal class Request
    {
        public int Id { get; set; }
        public string MSSV { get; set; }
        public string RequestContent { get; set; }
        public string Status { get; set; } // Pending, Approved, Declined
        public string AdminComment { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
