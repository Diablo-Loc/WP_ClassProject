using ClassProject.Presentation.Forms.Admin;
using ClassProject.Presentation.Forms.Course;
using ClassProject.Presentation.Forms.Main;
using ClassProject.Presentation.Forms.Students;

namespace ClassProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
            //Application.Run(new RegisterCourseForm());
            //Application.Run(new ManageScoreForm());
            //Application.Run(new RegisterCourseForm());
            //Application.Run(new ClassroomForm());
            //Application.Run(new StudentRequestForm());
            //Application.Run(new AdminApprovalForm());
            //Application.Run(new StatisticsForm());

        }
    }
}