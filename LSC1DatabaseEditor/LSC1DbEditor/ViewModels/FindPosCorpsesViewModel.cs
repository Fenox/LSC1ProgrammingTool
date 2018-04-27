using GalaSoft.MvvmLight.Command;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class FindPosCorpsesViewModel
    {
        public ObservableCollection<string> ProcCorpsesList { get; set; }

        public ICommand DeleteCommand { get; set; }

        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();

        public FindPosCorpsesViewModel()
        {
            Initialize();
        }

        //TODO: async
        private async void Initialize()
        {
            ProcCorpsesList = new ObservableCollection<string>(
                await AsyncDbExecuter.DoTaskAsync("Lade pos Waisen...",
                    () => new FindPosOrphansQuery().Execute(Connection)));

            DeleteCommand = new RelayCommand<object>(DeletePosCorpses);
        }

        //TODO test (Problem: schlecht zu testen in viewmodel?)
        private async void DeletePosCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);
            await AsyncDbExecuter.DoTaskAsync("Lösche pos Waisen...", () =>
            {
                foreach (object item in selectedItemsList)
                {
                    //TODO: make to one simple query with IN keyword. + sanitize
                    string deletePosQuery = "DELETE FROM `tpos` WHERE Name = '" + item + "'";
                    new NonReturnSimpleQuery(deletePosQuery).Execute(Connection);
                }
            });

            ProcCorpsesList.Clear();

            foreach (string item in await AsyncDbExecuter.DoTaskAsync("Lade pos Waisen...",
                                                () => new FindPosOrphansQuery().Execute(Connection)))
                ProcCorpsesList.Add(item);
        }
    }
}
