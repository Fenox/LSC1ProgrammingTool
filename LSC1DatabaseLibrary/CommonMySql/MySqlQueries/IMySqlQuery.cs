using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public interface IMySqlQuery<TReturn>
    {
        /// <summary>
        /// Executes the query in Query and therefore uses the given connection.
        /// </summary>
        /// <param name="connection">Connection to connect to the database.</param>
        /// <returns>Result</returns>
        /// <exception cref="MySqlException">If something goes wrong with connecting, executung the query</exception>
        TReturn Execute(MySqlConnection connection);
    }

}
