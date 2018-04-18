using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    //TODO: handle proc robot and proc laser
    public class FindProcCorpsesViewModel : ViewModelBase
    {
        private static LSC1InconsistencyHandler inconsistencies = 
            new LSC1InconsistencyHandler(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        public ObservableCollection<string> ProcCorpsesList { get; set; }

        public ICommand DeleteCommand { get; set; }

        public FindProcCorpsesViewModel()
        {
            Initialize();
            DeleteCommand = new RelayCommand<object>(DeleteProcCorpses);
        }
        async void Initialize()
        {
            ProcCorpsesList = new ObservableCollection<string>(
                 await inconsistencies.FindProcLaserOrphansAsync());
        }

        async void DeleteProcCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);

            foreach (var item in selectedItemsList)
            {
                string deleteProcLaserQuery = "DELETE FROM `tproclaserdata` WHERE Name = '" + item + "'";
                string deleteProcRobotQuery = "DELETE FROM `tprocrobot` WHERE Name = '" + item + "'";

                LSC1DatabaseFacade.SimpleQuery(deleteProcLaserQuery);
                LSC1DatabaseFacade.SimpleQuery(deleteProcRobotQuery);
            }

            ProcCorpsesList.Clear();

            foreach (var item in await inconsistencies.FindProcLaserOrphansAsync())
                ProcCorpsesList.Add(item);
        }
    }
}
