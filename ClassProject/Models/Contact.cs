using System;

namespace ClassProject.Models
{
    public class Contact
    {
        public int ContactID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}