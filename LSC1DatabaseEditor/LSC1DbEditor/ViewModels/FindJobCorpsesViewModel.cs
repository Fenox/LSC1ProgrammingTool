using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class FindJobCorpsesViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public ObservableCollection<string> JobCorpses { get; set; }

        public ICommand AssignNameCommand { get; set; }

        private string newName;
        public string NewName
        {
            get => newName;
            set
            {
                newName = value;
                RaisePropertyChanged();
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
                await AsyncDbExecuter.DoTaskAsync("Suche Job Waisen...",
                    () => new FindJobOrphansQuery().Execute(Connection)));
        }

        public async void AssignName()
        {
            await AsyncDbExecuter.DoTaskAsync("Vergebe Namen...", () =>
            new NonReturnSimpleQuery("UPDATE tjobname SET Name = @Name WHERE JobNr = @JobNr", 
                    new MySqlParameter("Name", newName),
                    new MySqlParameter("JobNr", SelectedJobNr)));

            //TODO Neu Laden der Job-Leichen
            JobCorpses.Clear();

            foreach (string item in await AsyncDbExecuter.DoTaskAsync("Suche Job Waisen...",
                () => new FindJobOrphansQuery().Execute(Connection)))
                JobCorpses.Add(item);

            NewName = "";

            Messenger.Default.Send(new JobsChangedMessage());
        }
    }
}
