using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using ClassProject.Business.Services;

namespace ClassProject.DataAccess.Db
{
    public class My_DB
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        public SqlConnection GetConnection([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var conn = new SqlConnection(connectionString);
            string connectionId = Guid.NewGuid().ToString().Substring(0, 8);

            // Đăng ký mở kết nối với hệ thống AI giám sát
            ConnectionMonitor.RegisterOpen(connectionId, sourceFilePath, sourceLineNumber);

            // Lắng nghe trạng thái thực tế khi connection gọi lệnh Close/Dispose
            conn.StateChange += (sender, e) =>
            {
                if (e.CurrentState == System.Data.ConnectionState.Closed)
                {
                    ConnectionMonitor.RegisterClose(connectionId);
                }
            };

            return conn;
        }
    }
}