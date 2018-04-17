using LSC1DatabaseEditor;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.DatabaseModel
{
    public class UpdatedDbRowViewModel : DbRow
    {
        public LSC1DatabaseConnectionSettings ConnectionSettings { get; set; }

        public UpdatedDbRowViewModel()
        {
            Values.CollectionChanged += Values_CollectionChanged;
        }

        private void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                string updateQuery = "UPDATE `" + TableName + "` SET ";

                int i = 0;
                foreach (var columnName in ColumnNames)
                {
                    updateQuery += columnName + " = '" + Values[i] + " ";
                }

                //TODO: Catch exception.
                LSC1DatabaseFacade.SimpleQuery(updateQuery);
            }
        }
    }
}
