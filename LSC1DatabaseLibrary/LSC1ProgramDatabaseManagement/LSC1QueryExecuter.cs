using LSC1DatabaseLibrary.CommonMySql;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement
{
    public class LSC1MySqlQueryExecuter : MySqlQueryExecuter
    {       

        public LSC1MySqlQueryExecuter(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
        }

        public LSC1MySqlQueryExecuter(MySqlConnection connection)
        {
            this.connection = connection;
        }

        protected override void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        protected override void OpenConnection()
        {
            try
            {
                if(connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
