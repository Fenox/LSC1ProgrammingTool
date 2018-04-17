
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.CommonMySql
{
    /// <summary>
    /// Collection of common mysql queries.
    /// </summary>
    public class CommonMySqlQueries
    {
        private MySqlConnection connection;

        public CommonMySqlQueries(MySqlConnection connection)
        {
            this.connection = connection;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public DataTable GetTable(string query)
        {
            DataTable dt = new DataTable();
            
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            try
            {
                adapter.Fill(dt);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                adapter.Dispose();
                builder.Dispose();
                connection.Dispose();
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="MySqlException"> </exception>
        public void ExecuteQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="numResults"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public List<T> ReadRows<T>(string query, int numResults) where T : DbRow, new()
        {
            List<T> items = new List<T>();
               MySqlCommand cmd = new MySqlCommand(query, connection);

                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read() && items.Count < numResults)
                    {
                        if (reader.IsDBNull(0))
                            continue;

                        var newItem = new T();
                        for (int i = 0; i < reader.FieldCount; i++)
                            newItem.Values[i] = reader.GetString(i);

                        items.Add(newItem);
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                }

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public List<T> ReadRows<T>(string query) where T : DbRow, new()
        {
            List<T> items = new List<T>();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                            continue;

                        var newItem = new T();
                        for (int i = 0; i < reader.FieldCount; i++)
                            newItem.Values[i] = reader.GetString(i);

                        items.Add(newItem);
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }


            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public List<string> ReadSingleColumnQuery(string query)
        {
            List<string> items = new List<string>();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        items.Add(reader.GetString(0));
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                }


            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="MySqlException"></exception>
        public List<int> ReadSingleIntColumnQuery(string query)
        {
            List<int> items = new List<int>();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        items.Add(reader.GetInt32(0));
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                }

            return items;
        }

        #region Insert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tableName"></param>
        /// <exception cref="MySqlException"></exception>
        public void Insert(DataRow row, string tableName)
        {
            var columnsList = new List<string>();
            foreach (DataColumn item in row.Table.Columns)
            {
                columnsList.Add(item.ColumnName);
            }

            var values = new List<string>();
            foreach (var item in row.ItemArray)
            {
                values.Add(item.ToString());
            }

            try
            {
                Insert(columnsList, values, tableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <exception cref="MySqlException"></exception>
        public void Insert(DbRow row)
        {
            try
            {
                Insert(row.ColumnNames, row.Values.ToList(), row.TableName);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="values"></param>
        /// <param name="tableName"></param>
        /// <exception cref="MySqlException"></exception>
        public void Insert(List<string> columnNames, List<string> values, string tableName)
        {
            string insertString = "INSERT INTO " + tableName + " (";

            foreach (var item in columnNames)
            {
                insertString += "`" + item + "`, ";
            }

            //Entferne lestzes Leerzeichen und Komma.
            insertString = insertString.Remove(insertString.Length - 2);

            insertString += ") VALUES (";

            foreach (var item in values)
            {
                insertString += "'" + item + "', ";
            }

            //Entferne lestzes Leerzeichen und Komma.
            insertString = insertString.Remove(insertString.Length - 2);

            insertString += ")";

            try
            {
                ExecuteQuery(insertString);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
        #endregion Insert
    }
}
