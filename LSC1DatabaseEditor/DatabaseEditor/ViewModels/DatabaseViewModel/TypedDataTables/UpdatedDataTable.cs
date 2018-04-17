using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.TypedDataTables
{
    public class UpdatedDataTableViewModel<T> where T : UpdatedDbRowViewModel, new()
    {
        private DataTable database;
        public DataTable Database
        {
            get
            {
                return database;
            }
            set
            {
                database = value;

                foreach (var row in database.Rows)
                {
                    var newRow = NewRow();
                    newRow.Row = (DataRow)row;
                    Rows.Add(newRow);
                }
                
                database.RowChanged += Database_RowChanged;
                database.RowDeleted += Database_RowDeleted;
            }
        }
        

        LSC1DatabaseConnectionSettings settings;
        public TablesEnum TableName { get; set; }

        public List<T> Rows { get; set; } = new List<T>();

        public UpdatedDataTableViewModel(LSC1DatabaseConnectionSettings settings, TablesEnum tableName)
        {
            this.TableName = tableName;
            this.settings = settings;
        }

        private void Database_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            var row = Rows.First(r => r.Row == e.Row);

            int changedColumnIndex = -1;
            for (int i = 0; i < e.Row.ItemArray.Length; i++)
            {
                if (e.Row[i] != e.Row[i, DataRowVersion.Original])
                    changedColumnIndex = i;
            }

            row.ValuesChanged(e.Row, changedColumnIndex);
        }

        private void Database_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            var row = Rows.First(r => r.Row == e.Row);
            row.Delete();
            Rows.Remove(row);
        }


        protected Type GetRowType()
        {
            return typeof(T);
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
            T newRow = UpdatedDbRowViewModel.RowCreator<T>(settings);
            newRow.Row = Database.NewRow();
            newRow.TableName = TableName;
            return newRow;
        }
    }

    public class UpdatedDbRowViewModel
    {
        public static T RowCreator<T>(LSC1DatabaseConnectionSettings conSettings) where T : UpdatedDbRowViewModel, new()
        {
            T newRow = new T
            {
                ConSettings = conSettings
            };
            return newRow;
        }

        public DataRow Row { get; set; }
        public LSC1DatabaseConnectionSettings ConSettings { get; set; }
        public TablesEnum TableName { get; set; }

        public UpdatedDbRowViewModel() {  }

        public void ValuesChanged(DataRow row, int changedColumnIndex)
        {
            if (changedColumnIndex < 0)
                return;

            var col = row.Table.Columns[changedColumnIndex];
            string updateQuery = "UPDATE `" + TableName + "` SET `" + col.ColumnName + "` = '" + row[col] + "' WHERE ";
                        
            for (int i = 0; i < Row.Table.Columns.Count; i++)
            {
                string columnNamei = Row.Table.Columns[i].ColumnName;
                updateQuery += "`" + columnNamei + "` = '" + Row[i, DataRowVersion.Original] + "'";

                if (i != Row.Table.Columns.Count - 1)
                    updateQuery += " AND ";
            }

            LSC1DatabaseFacade.SimpleQuery(updateQuery);
        }

        public void Insert()
        {
            LSC1DatabaseFacade.Insert(ConSettings, Row, TableName.ToString());

            var columnsList = new List<string>();
            foreach (DataColumn item in Row.Table.Columns)
            {
                columnsList.Add(item.ColumnName);
            }

            var values = new List<string>();
            foreach (var item in Row.ItemArray)
            {
                values.Add(item.ToString());
            }

            try
            {
                LSC1DatabaseFacade.Insert(ConSettings, TableName.ToString(), columnsList, values);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete()
        {
            string deleteQuery = CreateDeleteQueryText();

            //TODO: write test
            //TODO: catch exception
            LSC1DatabaseFacade.SimpleQuery(deleteQuery);

            //Updaten der Offline-Datenbank
            OfflineDatabase.UpdateTable(ConSettings, TableName);
        }

        private string CreateDeleteQueryText()
        {
            string deleteQuery = "DELETE FROM `" + TableName + "` WHERE ";
            for (int i = 0; i < Row.Table.Columns.Count; i++)
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
