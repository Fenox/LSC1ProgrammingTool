using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class NonReturnSimpleQuery : IMySqlQuery<object>
    {
        public NonReturnSimpleQuery(string query)
        {
            Query = query;
        }

        public string Query { get; set; }


        public object Execute(MySqlConnection connection)
        {
            MySqlCommand cmd = new MySqlCommand(Query, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
            }

            return null;
        }
    }
}
