using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_AI.Hospital.database
{
    class Database
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["WPF_AI.Hospital.Properties.Settings.HospitalConnectionString"].ConnectionString;

        public static SqlConnection sqlConnection = new SqlConnection(connectionString);

        public static void OpenConnectionWithDatabase()
        {
            sqlConnection.Open();
        }

        public static void CloseConnectionWithDatabase()
        {
            sqlConnection.Close();
        }
    }
}
