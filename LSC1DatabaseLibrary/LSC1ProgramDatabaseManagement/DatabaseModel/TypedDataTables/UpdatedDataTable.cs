using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement.DatabaseModel.TypedDataTables
{
    public class UpdatedDataTable<T> where T : MyUpdatedDbRow, new()
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

        public UpdatedDataTable(LSC1DatabaseConnectionSettings settings, TablesEnum tableName)
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
            T newRow = MyUpdatedDbRow.RowCreator<T>(settings);
            newRow.Row = Database.NewRow();
            newRow.TableName = TableName;
            return newRow;
        }
    }

    public class MyUpdatedDbRow
    {
        public static T RowCreator<T>(LSC1DatabaseConnectionSettings conSettings) where T : MyUpdatedDbRow, new()
        {
            T newRow = new T();
            newRow.conSettings = conSettings;
            return newRow;
        }

        public DataRow Row { get; set; }
        public LSC1DatabaseConnectionSettings conSettings { get; set; }
        public TablesEnum TableName { get; set; }

        public MyUpdatedDbRow() {  }

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
            
            LSC1DatabaseConnector con = new LSC1DatabaseConnector(conSettings);
            con.ExecuteQuery(updateQuery);
        }

        public void Insert()
        {
            LSC1DatabaseConnector con = new LSC1DatabaseConnector(conSettings);
            con.Insert(Row, TableName.ToString());
        }

        public void Delete()
        {
            string deleteQuery = "DELETE FROM `" + TableName + "` WHERE ";
            for (int i = 0; i < Row.Table.Columns.Count; i++)
            {
                string columnNamei = Row.Table.Columns[i].ColumnName;
                deleteQuery += "`" + columnNamei + "` = '" + Row[i, DataRowVersion.Original] + "'";

                if (i != Row.Table.Columns.Count - 1)
                    deleteQuery += " AND ";
            }

            LSC1DatabaseConnector con = new LSC1DatabaseConnector(conSettings);
            con.ExecuteQuery(deleteQuery);
            
            //Updaten der Offline-Datenbank
            OfflineDatabase.UpdateTable(conSettings, TableName);
        }
    }
}
