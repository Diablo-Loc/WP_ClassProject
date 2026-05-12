using System.Configuration;
using Microsoft.Data.SqlClient;

namespace ClassProject.DataAccess.Db
{
    public class My_DB
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}