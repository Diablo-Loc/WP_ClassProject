using System;
using System.Collections.Generic;
using System.Text;

namespace ClassProject.Models
{
    internal class CourseRegistration
    {
        public string Mssv { get; set; }        // Khớp NVARCHAR(30)
        public string CourseId { get; set; }    // Khớp VARCHAR(50)
        public DateTime RegistrationDate { get; set; }
        public double? Score { get; set; }      // FLOAT NULL trong SQL tương đương double? trong C#

        public CourseRegistration() { }

        public CourseRegistration(string mssv, string courseId)
        {
            this.Mssv = mssv;
            this.CourseId = courseId;
            this.RegistrationDate = DateTime.Now;
            this.Score = null;
        }
    }
}
