using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    public class FindProcCorpsesViewModel : ViewModelBase
    {
        public ObservableCollection<string> ProcCorpsesList { get; set; }

        public ICommand DeleteCommand { get; set; }

        public FindProcCorpsesViewModel()
        {
            ProcCorpsesList = new ObservableCollection<string>(LSC1DatabaseFunctions.FindProcCorpses());

            DeleteCommand = new RelayCommand<object>(DeleteProcCorpses);
        }

        void DeleteProcCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();

            foreach (var item in selectedItemsList)
            {
                string deleteProcLaserQuery = "DELETE FROM `tproclaserdata` WHERE Name = '" + item + "'";
                string deleteProcRobotQuery = "DELETE FROM `tprocrobot` WHERE Name = '" + item + "'";

                db.ExecuteQuery(deleteProcLaserQuery);
                db.ExecuteQuery(deleteProcRobotQuery);
            }

            ProcCorpsesList.Clear();

            foreach (var item in LSC1DatabaseFunctions.FindProcCorpses())
                ProcCorpsesList.Add(item);
        }
    }
}
