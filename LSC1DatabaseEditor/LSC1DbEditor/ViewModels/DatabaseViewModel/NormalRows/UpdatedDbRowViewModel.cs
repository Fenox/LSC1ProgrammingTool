using System.Collections.Specialized;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows
{
    public class UpdatedDbRowViewModel : DbRow
    {
        public LSC1DatabaseConnectionSettings ConnectionSettings { get; set; }
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();

        public UpdatedDbRowViewModel()
        {
            Values.CollectionChanged += Values_CollectionChanged;
        }

        //TODO: sanitize sql + logging
        private async void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Replace) return;

            string updateQuery = "UPDATE `" + TableName + "` SET ";

            var i = 0;
            foreach (string columnName in ColumnNames)
            {
                updateQuery += columnName + " = '" + Values[i] + " ";
            }

            //TODO: Catch exception.
            await AsyncDbExecuter.DoTaskAsync("Aktualisiere Wert in Datenbank...", () => 
                new NonReturnSimpleQuery(updateQuery).Execute(Connection));
        }
    }
}
