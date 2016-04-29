using LSC1DatabaseLibrary.DatabaseModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSC1DatabaseLibrary
{
    public class LSC1DatabaseConnector
    {
        private MySqlConnection connection;

        private void Initialize()
        {
            string server = "127.0.0.1";
            //string server = "29.47.82.13";
            //string database = "lsc1";
            string database = "robot2";
            string uid = "root";
            //string password = "sql";
            string password = "";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";Connect Timeout=1";

            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
            connectionStringBuilder.Server = "127.0.0.1"; //"29.47.82.13"
            connectionStringBuilder.Database = "robot2"; //"lsc1"
            connectionStringBuilder.UserID = "root"; //"root"
            connectionStringBuilder.Password = ""; //"sql"

            connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                Initialize();
                connection.Open();

                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.ToString());
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                connection.Dispose();
            }
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
            Initialize();
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

        public DataTable GetJobDataTable(string query)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("JobNr");
            dt.Columns.Add("Step");
            dt.Columns.Add("Who");
            dt.Columns.Add("What");
            dt.Columns.Add("Name", typeof(List<string>));
            dt.Columns.Add("MoveParam");
            dt.Columns.Add("Frame");
            dt.Columns.Add("Tool");
            dt.Columns.Add("Interpol");
            dt.Columns.Add("WEM");
            dt.Columns.Add("LaserProgNr");

            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        dt.Rows.Add(new object[] { reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            new List<string> { reader.GetString(4), "Neuer Eintrag" },
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetString(9),
                            reader.GetString(10),
                        });
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }


                CloseConnection();
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
            if (OpenConnection())
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

                CloseConnection();
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
            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read() && items.Count < numResults)
                    {
                        if (reader.IsDBNull(0))
                            continue;

                        var newItem = new T();
                        for (int i = 0; i < newItem.NumElements; i++)
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

                CloseConnection();
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
            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                            continue;

                        var newItem = new T();
                        for (int i = 0; i < newItem.NumElements; i++)
                            newItem.Values[i] = reader.GetString(i);

                        items.Add(newItem);
                    }
                }
                catch (MySqlException ex)
                {
                    throw ex;
                }

                CloseConnection();
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
            if (OpenConnection())
            {
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

                CloseConnection();
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
                Insert(row.ColumnNames, row.Values, row.TableName);
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
