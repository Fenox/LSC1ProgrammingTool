using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class ReadRowsQuery<TItem> : MySqlQuery<IEnumerable<TItem>> 
        where TItem: DbRow, new()
    {
        private readonly string query;

        private readonly MySqlParameter[] parameters;

        public ReadRowsQuery(string query, params MySqlParameter[] parameters)
        {
            this.query = query;
            this.parameters = parameters;
        }

        private MySqlCommand Create()
        {
            MySqlCommand cmd = new MySqlCommand(query);
            foreach (MySqlParameter param in parameters)
                cmd.Parameters.Add(param);

            return cmd;
        }

        /// <returns></returns>
        protected override IEnumerable<TItem> ProtectedExecution(MySqlConnection connection)
        {
            List<TItem> queryResults = new List<TItem>();
            MySqlCommand cmd = Create();

            cmd.Connection = connection;
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;

                    var resultItem = new TItem();
                    if (resultItem.Values.Count < reader.FieldCount)
                        resultItem.Values = new ObservableCollection<string>(new string[reader.FieldCount].ToList()); //TODO should not be observable collection (thats View)
                    for (int i = 0; i < reader.FieldCount; i++) 
                        resultItem.Values[i] = reader.GetString(i);

                    queryResults.Add(resultItem);
                }
            }
            finally
            {
                cmd.Dispose();
            }

            return queryResults;
        }
    }


  
}
