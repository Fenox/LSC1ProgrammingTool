using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LSC1DatabaseEditor.LSC1Database.Queries;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1ProgramDatabaseManagement;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    public class FindProcCorpsesViewModel : ViewModelBase
    {
        public ObservableCollection<string> ProcCorpsesList { get; set; }

        public ICommand DeleteCommand { get; set; }

        public FindProcCorpsesViewModel()
        {
            ProcCorpsesList = new ObservableCollection<string>(LSC1DatabaseFacade.FindProcCorpses());
            DeleteCommand = new RelayCommand<object>(DeleteProcCorpses);
        }

        void DeleteProcCorpses(object selectedItems)
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

            foreach (var item in LSC1DatabaseFacade.FindProcCorpses())
                ProcCorpsesList.Add(item);
        }
    }
}
