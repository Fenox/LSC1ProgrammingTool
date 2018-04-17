using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql
{
    /// <summary>
    /// Responsible for executing a mysql query and returning the results.
    /// Also responsible for opeing and closing the connection.
    /// </summary>
    public abstract class MySqlQueryExecuter
    {
        protected MySqlConnection connection;

        /// <summary>
        /// Executes the given query and handles opening and closing of the connection.
        /// </summary>
        /// <typeparam name="TReturn">Return value</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>Result.</returns>
        /// <exception cref="MySqlException">Throws a mysql exception if opeing, closing, executing
        /// goes wrong.</exception>
        public TReturn Execute<TReturn>(IMySqlQuery<TReturn> query)
        {
            try
            {
                OpenConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var result = query.Execute(connection);

            try
            {
                CloseConnection();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }

            return result;
        }

        //TODO: test
        /// <summary>
        /// Executes the given query and handles opening and closing of the connection.
        /// </summary>
        /// <typeparam name="TReturn">Return value</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns>Result.</returns>
        /// <exception cref="MySqlException">Throws a mysql exception if opeing, closing, executing
        /// goes wrong.</exception>
        public MySqlQueryExecuter MultiExecute<TReturnQuery>(IMySqlQuery<TReturnQuery> query)
        {
            try
            {
                OpenConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var result = query.Execute(connection);

            try
            {
                CloseConnection();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }

            return this;
        }

        protected abstract void OpenConnection();
        protected abstract void CloseConnection();
    }
}
