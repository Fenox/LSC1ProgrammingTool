using GalaSoft.MvvmLight.Command;
using LSC1DatabaseEditor.LSC1Database;
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
            Initialize();
        }

        async void Initialize()
        {
            var finder = new LSC1InconsistencyHandler(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            ProcCorpsesList = new ObservableCollection<string>(await finder.FindPosOrphansAsync());

            DeleteCommand = new RelayCommand<object>(DeletePosCorpses);
        }

        //TODO test
        async void DeletePosCorpses(object selectedItems)
        {
            var selectedItemsList = ((System.Collections.IList)selectedItems);
            
            foreach (var item in selectedItemsList)
            {
                //TODO: make to one simple query with IN keyword.
                string deletePosQuery = "DELETE FROM `tpos` WHERE Name = '" + item + "'";
                LSC1DatabaseFacade.SimpleQuery(deletePosQuery);
            }

            ProcCorpsesList.Clear();

            foreach (var item in await new LSC1InconsistencyHandler(LSC1UserSettings.Instance.DBSettings.ConnectionString).FindPosOrphansAsync())
                ProcCorpsesList.Add(item);
        }
    }
}
