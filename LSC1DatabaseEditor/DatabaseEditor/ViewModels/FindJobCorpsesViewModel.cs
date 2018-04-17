using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
            JobCorpses = new ObservableCollection<string>(LSC1DatabaseFacade.FindJobCorpses());

            AssignNameCommand = new RelayCommand(AssignName);
        }

        public void AssignName()
        {
            LSC1DatabaseFacade.AssignNameToJob(SelectedJobNr, NewName);

            //TODO Neu Laden der Job-Leichen
            JobCorpses.Clear();

            foreach (var item in LSC1DatabaseFacade.FindJobCorpses())
                JobCorpses.Add(item);

            NewName = "";

            Messenger.Default.Send(new JobsChangedMessage());
        }
    }
}
