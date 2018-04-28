using MySql.Data.MySqlClient;

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
                MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder
                {
                    Server = Server, //"29.47.82.13"
                    Database = Database, //"lsc1"
                    UserID = Uid, //"root"
                    Password = Password //"sql"
                };

                return connStringBuilder.ConnectionString;
            }
        }
    }
}
