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
            JobCorpses = new ObservableCollection<string>(LSC1DatabaseFunctions.FindJobCorpses(LSC1UserSettings.Instance.DBSettings));

            AssignNameCommand = new RelayCommand(AssignName);
        }

        public void AssignName()
        {
            //Eintrag in JobName erstellen
            LSC1DatabaseConnector db = new LSC1DatabaseConnector(LSC1UserSettings.Instance.DBSettings);
            string insertQuery = "INSERT INTO `tjobname` VALUES('" + SelectedJobNr + "', '" + NewName + "')";
            db.ExecuteQuery(insertQuery);

            //TODO Neu Laden der Job-Leichen
            JobCorpses.Clear();

            foreach (var item in LSC1DatabaseFunctions.FindJobCorpses(LSC1UserSettings.Instance.DBSettings))
                JobCorpses.Add(item);

            NewName = "";

            Messenger.Default.Send(new JobsChangedMessage());
        }
    }
}
