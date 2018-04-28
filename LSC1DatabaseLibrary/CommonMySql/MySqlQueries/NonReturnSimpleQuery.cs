using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class NonReturnSimpleQuery : MySqlQuery<object>
    {
        public NonReturnSimpleQuery(string query, params MySqlParameter[] parameters)
        {
            this.Query = query;
            this.parameters = parameters;
        }

        private readonly MySqlParameter[] parameters;
        public string Query;

        private MySqlCommand Create()
        {
            var cmd = new MySqlCommand(Query);
            foreach (MySqlParameter param in parameters)
                cmd.Parameters.Add(param);

            return cmd;
        }

        protected override object ProtectedExecution(MySqlConnection connection)
        {
            MySqlCommand cmd = Create();
            cmd.Connection = connection;
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
            }

            return new object();
        }
    }
}
