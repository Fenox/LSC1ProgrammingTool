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
    public class FindPosCorpsesViewModel
    {
        public ObservableCollection<string> ProcCorpsesList { get; set; }

        public ICommand DeleteCommand { get; set; }

        public FindPosCorpsesViewModel()
        {
            ProcCorpsesList = new ObservableCollection<string>(LSC1DatabaseFunctions.FindPosCorpses(LSC1UserSettings.Instance.DBSettings));

            DeleteCommand = new RelayCommand<object>(DeletePosCorpses);
        }

        void DeletePosCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);

            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);

            foreach (var item in selectedItemsList)
            {
                string deletePosQuery = "DELETE FROM `tpos` WHERE Name = '" + item + "'";

                db.ExecuteQuery(deletePosQuery);
            }

            ProcCorpsesList.Clear();

            foreach (var item in LSC1DatabaseFunctions.FindPosCorpses(LSC1UserSettings.Instance.DBSettings))
                ProcCorpsesList.Add(item);
        }
    }
}
