using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Controller;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    public class FindJobCorpsesViewModel : ViewModelBase
    {
        private static LSC1InconsistencyHandler inconsistencyFinder = new LSC1InconsistencyHandler(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        private static LSC1AsyncTaskExecuter executer = new LSC1AsyncTaskExecuter();

        public ObservableCollection<string> JobCorpses { get; set; }

        public ICommand AssignNameCommand { get; set; }

        private string newName;
        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                RaisePropertyChanged("NewName");
            }
        }

        public string SelectedJobNr { get; set; }

        public FindJobCorpsesViewModel()
        {
            Initialize();

            AssignNameCommand = new RelayCommand(AssignName);
        }

        private async void Initialize()
        {
            JobCorpses = new ObservableCollection<string>(
                await executer.DoTaskAsync("Finde Job Waisen" , inconsistencyFinder.FindJobOrphansAsync()));
        }

        public async void AssignName()
        {
            LSC1DatabaseFacade.AssignNameToJob(SelectedJobNr, NewName);

            //TODO Neu Laden der Job-Leichen
            JobCorpses.Clear();

            foreach (var item in await executer.DoTaskAsync("Finde Job Waisen", inconsistencyFinder.FindJobOrphansAsync()))
                JobCorpses.Add(item);

            NewName = "";

            Messenger.Default.Send(new JobsChangedMessage());
        }
    }
}
