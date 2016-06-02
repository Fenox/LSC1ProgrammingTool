using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class DbRow
    {
        public virtual string TableName { get; set; }

        public int NumElements
        {
            get { return ColumnNames.Count; }
            set { }
        }

        List<string> columnNames = new List<string>();
        public List<string> ColumnNames
        {
            get { return columnNames; }
            set { columnNames = value; }
        }

        public ObservableCollection<string> Values { get; set; } = new ObservableCollection<string>();
        
    }

    public class UpdatedDbRow : DbRow
    {
        public LSC1DatabaseConnectionSettings connectionSettings { get; set; }

        public UpdatedDbRow()
        {
            Values.CollectionChanged += Values_CollectionChanged;
        }

        private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                string updateQuery = "UPDATE `" + TableName + "` SET ";

                int i = 0;
                foreach (var columnName in ColumnNames)
                {
                    updateQuery += columnName + " = '" + Values[i] + " ";
                }

                LSC1DatabaseConnector con = new LSC1DatabaseConnector(connectionSettings);
                con.ExecuteQuery(updateQuery);
            }
        }
    }
}
