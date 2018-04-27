using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class CountQuery : MySqlQuery<int>
    {
        private readonly string query;
        private readonly MySqlParameter[] parameters;

        public CountQuery(string query, params MySqlParameter[] parameters)
        {
            this.parameters = parameters;
            this.query = query;
        }

        private MySqlCommand Create()
        {
            var cmd = new MySqlCommand(query);
            foreach (MySqlParameter param in parameters)
                cmd.Parameters.Add(param);

            return cmd;
        }

        protected override int ProtectedExecution(MySqlConnection connection)
        {
            MySqlCommand cmd = Create();
            cmd.Connection = connection;

            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return 0;

                return !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
            }
        }
    }
}
