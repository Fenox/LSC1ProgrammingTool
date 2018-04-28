using System.Collections.Generic;
using System.Data;
using System.Linq;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.TypedDataTables
{
    public class UpdatedDataTableViewModel<T> where T : UpdatedDbRowViewModel, new()
    {
        private DataTable database;
        public DataTable Database
        {
            get => database;
            set
            {
                database = value;

                foreach (object row in database.Rows)
                {
                    T newRow = NewRow();
                    newRow.Row = (DataRow)row;
                    Rows.Add(newRow);
                }
                
                database.RowChanged += Database_RowChanged;
                database.RowDeleted += Database_RowDeleted;
            }
        }


        private readonly string ConnectionString;
        public TablesEnum TableName { get; set; }

        public List<T> Rows { get; set; } = new List<T>();

        public UpdatedDataTableViewModel(string connectionString, TablesEnum tableName)
        {
            TableName = tableName;
            this.ConnectionString = connectionString;
        }

        private void Database_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            T row = Rows.First(r => r.Row == e.Row);

            int changedColumnIndex = -1;
            for (var i = 0; i < e.Row.ItemArray.Length; i++)
            {
                if (e.Row[i] != e.Row[i, DataRowVersion.Original])
                    changedColumnIndex = i;
            }

            row.ValuesChanged(changedColumnIndex);
        }

        private void Database_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            T row = Rows.First(r => r.Row == e.Row);
            row.Delete();
            Rows.Remove(row);
        }


        public void Add(T row)
        {
            Database.Rows.Add(row.Row);
            row.Insert();
            Rows.Add(row);
        }

        public void Remove(T row)
        {
            Database.Rows.Remove(row.Row);
        }

        public T NewRow()
        {
            T newRow = UpdatedDbRowViewModel.RowCreator<T>(ConnectionString);
            newRow.Row = Database.NewRow();
            newRow.TableName = TableName;
            return newRow;
        }
    }

    public class UpdatedDbRowViewModel
    {
        public static T RowCreator<T>(string conSettings) where T : UpdatedDbRowViewModel, new()
        {
            T newRow = new T
            {
                ConnectionString = conSettings
            };
            return newRow;
        }

        public DataRow Row { get; set; }
        public string ConnectionString { get; set; }
        public MySqlConnection Connection => new MySqlConnection(ConnectionString);
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        public TablesEnum TableName { get; set; }

        public async void ValuesChanged(int changedColumnIndex)
        {
            if (changedColumnIndex < 0)
                return;

            //TODO: move sql query to a new query class
            DataColumn col = Row.Table.Columns[changedColumnIndex];
            string updateQuery = "UPDATE `" + TableName + "` SET `" + col.ColumnName + "` = '" + Row[col] + "' WHERE ";
            var parameters = new MySqlParameter[Row.Table.Columns.Count];            

            for (var i = 0; i < Row.Table.Columns.Count; i++)
            {
                string columnNamei = Row.Table.Columns[i].ColumnName;
                updateQuery += "`" + columnNamei + "` = @Value" + i;
                parameters[i] = new MySqlParameter("Value" + i, Row[i, DataRowVersion.Original]);

                if (i != Row.Table.Columns.Count - 1)
                    updateQuery += " AND ";
            }

            await AsyncDbExecuter.DoTaskAsync("Updating Value...", () =>
                new NonReturnSimpleQuery(updateQuery,
                        parameters)
                    .Execute(Connection));
        }

        public async void Insert()
        {
            new InsertQuery(Row, TableName.ToString()).Execute(Connection);

            var columnsList = new List<string>();
            foreach (DataColumn item in Row.Table.Columns)
            {
                columnsList.Add(item.ColumnName);
            }

            var values = Row.ItemArray.Select(item => item.ToString()).ToList();

            await AsyncDbExecuter.DoTaskAsync("Füge neue Zeile ein...", () => 
                new InsertQuery(TableName.ToString(), columnsList, values).Execute(Connection));
        }

        public async void Delete()
        {
            string deleteQuery = CreateDeleteQueryText();

            //TODO: write test
            //TODO: catch exception
            await AsyncDbExecuter.DoTaskAsync("Lösche Zeile...", () =>
                new NonReturnSimpleQuery(deleteQuery).Execute(Connection));

            //Updaten der Offline-Datenbank
            OfflineDatabase.UpdateTable(ConnectionString, TableName);
        }

        private string CreateDeleteQueryText()
        {
            //TODO: sanitize
            string deleteQuery = "DELETE FROM `" + TableName + "` WHERE ";
            for (var i = 0; i < Row.Table.Columns.Count; i++)
            {
                string columnNamei = Row.Table.Columns[i].ColumnName;
                deleteQuery += "`" + columnNamei + "` = '" + Row[i, DataRowVersion.Original] + "'";

                if (i != Row.Table.Columns.Count - 1)
                    deleteQuery += " AND ";
            }

            return deleteQuery;
        }
    }
}
