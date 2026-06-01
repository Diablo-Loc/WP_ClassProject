using ClassProject.Presentation.Forms.Course;

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
            //Application.Run(new LoginForm());
            //Application.Run(new RegisterCourseForm());
            Application.Run(new ManageScoreForm());
            //Application.Run(new RegisterCourseForm());
        }
    }
}