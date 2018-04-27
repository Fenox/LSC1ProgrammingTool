using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LSC1DatabaseLibrary.CommonMySql.MySqlQueries
{
    public class InsertQuery : MySqlQuery<object>
    {
        private readonly string tableName;
        private readonly IEnumerable<string> columnNames;
        private readonly IEnumerable<string> values;

        public InsertQuery(string tableName, IEnumerable<string> columnNames, IEnumerable<string> values)
        {
            this.tableName = tableName;
            this.columnNames = columnNames;
            this.values = values;
        }

        public InsertQuery(DbRow row)
        {
            tableName = row.TableName;
            columnNames = row.ColumnNames;
            values = row.Values;
        }

        public InsertQuery(DataRow row, string tableName)
        {
            this.tableName = tableName;
            columnNames = row.Table.Columns.Cast<DataColumn>().Select(col => col.ColumnName);
            values = row.ItemArray.ToList().Select(item => item.ToString());
        }

        protected override object ProtectedExecution(MySqlConnection connection)
        {
            MySqlCommand cmd = Create();
            cmd.Connection = connection;
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

        public MySqlCommand Create()
        {
            string insertString = "INSERT INTO " + tableName + " (";

            insertString = columnNames.Aggregate(insertString, (current, item) => current + ("`" + item + "`, "));

            //Remove last space and comma
            insertString = insertString.Remove(insertString.Length - 2);

            insertString += ") VALUES (";

            var valueCount = 0;
            foreach (string unused in values)
            {
                insertString += "@Value" + valueCount + ", ";
                valueCount++;
            }

            //Entferne lestzes Leerzeichen und Komma.
            insertString = insertString.Remove(insertString.Length - 2);

            insertString += ")";

            var cmd = new MySqlCommand(insertString);
            
            for (var i = 0; i < values.Count(); i++)
                cmd.Parameters.AddWithValue("Value" + i, values.ToList()[i]);

            return cmd;
        }
    }
}
