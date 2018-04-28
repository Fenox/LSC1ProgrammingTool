using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    //TODO: handle proc robot and proc laser
    //TODO: Logging
    public class FindProcCorpsesViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        public ObservableCollection<string> ProcCorpsesList { get; set; }
        public ICommand DeleteCommand { get; set; }


        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public FindProcCorpsesViewModel()
        {
            Initialize();
            DeleteCommand = new RelayCommand<object>(DeleteProcCorpses);
        }

        private async void Initialize()
        {
            ProcCorpsesList = new ObservableCollection<string>(
                await AsyncDbExecuter.DoTaskAsync("Suche Proc Robot Waisen...",
                    () => new FindProcRobotOrphansQuery().Execute(Connection)));
        }

        private async void DeleteProcCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);

            await AsyncDbExecuter.DoTaskAsync("Lösche Proc Robot Waisen", () =>
            {
                foreach (object item in selectedItemsList)
                {
                    string deleteProcLaserQuery = "DELETE FROM `tproclaserdata` WHERE Name = '" + item + "'";
                    string deleteProcRobotQuery = "DELETE FROM `tprocrobot` WHERE Name = '" + item + "'";

                    new NonReturnSimpleQuery(deleteProcLaserQuery).Execute(Connection);
                    new NonReturnSimpleQuery(deleteProcRobotQuery).Execute(Connection);
                }
            });

            ProcCorpsesList.Clear();

            foreach (string item in await AsyncDbExecuter.DoTaskAsync("Suche Proc Robot Waisen...",
                () => new FindProcRobotOrphansQuery().Execute(Connection)))
                ProcCorpsesList.Add(item);
        }
    }
}
