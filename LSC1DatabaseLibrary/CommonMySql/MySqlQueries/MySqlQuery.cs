using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public abstract class MySqlQuery<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="MySqlException"></exception>
        /// <returns></returns>
        public TItem Execute(MySqlConnection connection)
        {
            OpenConnection(connection);
            TItem result = ProtectedExecution(connection);
            CloseConnection(connection);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="MySqlException"></exception>
        protected abstract TItem ProtectedExecution(MySqlConnection connection);

        public void OpenConnection(MySqlConnection connection)
        {
            connection.Close();
            connection.Open();
        }

        public void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
        }
    }
}
