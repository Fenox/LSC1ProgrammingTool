using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement
{
    public class LSC1DatabaseConnectionSettings
    {
        public string Database { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Uid { get; set; }

        public string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder();
                connStringBuilder.Server = Server; //"29.47.82.13"
                connStringBuilder.Database = Database; //"lsc1"
                connStringBuilder.UserID = Uid; //"root"
                connStringBuilder.Password = Password; //"sql"

                return connStringBuilder.ConnectionString;
            }
        }
    }
}
