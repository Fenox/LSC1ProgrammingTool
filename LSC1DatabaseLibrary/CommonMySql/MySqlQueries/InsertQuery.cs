using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    //TODO: test
    public class InsertQuery : IMySqlQuery<object>
    {
        private string tableName;
        private IEnumerable<string> columnNames;
        private IEnumerable<string> values;

        public InsertQuery(string tableName, IEnumerable<string> columnNames, IEnumerable<string> values)
        {
            this.tableName = tableName;
            this.columnNames = columnNames;
            this.values = values;
        }

        public InsertQuery(DbRow row)
        {
            this.tableName = row.TableName;
            this.columnNames = row.ColumnNames;
            this.values = row.Values;
        }

        public object Execute(MySqlConnection connection)
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

            MySqlCommand cmd = new MySqlCommand(insertString, connection);
            try
            {
                cmd.ExecuteNonQuery();
                return null;
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
