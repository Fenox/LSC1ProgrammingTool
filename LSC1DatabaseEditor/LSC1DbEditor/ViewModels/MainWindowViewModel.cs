using ExtensionsAndCodeSnippets.RandomTools;
using ExtensionsAndCodeSnippets.SystemData.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1Database.Queries.Inconsistencies;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        private readonly Logger Logger;
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);


        private ObservableCollection<LSC1TablePropertiesViewModelBase> tables;
        public ObservableCollection<LSC1TablePropertiesViewModelBase> Tables
        {
            get => tables;
            set
            {
                tables = value;
                RaisePropertyChanged();
            }
        }

        private LSC1TablePropertiesViewModelBase selectedTable;
        public LSC1TablePropertiesViewModelBase SelectedTable
        {
            get => selectedTable;
            set
            {
                if (selectedTable == value) return;

                selectedTable = value;
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                SelectedTableChanged();

                RaisePropertyChanged();
                RaisePropertyChanged($"NameFilterPossible");
            }
        }

        private ObservableCollection<DbJobNameRow> jobs;
        public ObservableCollection<DbJobNameRow> Jobs
        {
            get => jobs;
            set
            {
                jobs = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> Messages { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
            set
            {
                if (selectedJob == value) return;

                selectedJob = value;
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
                OnSelectedJobChanged();
            }
        }

        private string selectedNameFilter;
        public string SelectedNameFilter
        {
            get => selectedNameFilter;
            set
            {
                selectedNameFilter = value;
                ReloadGridViewData();
                RaisePropertyChanged();
            }
        }

        private bool jobFilterEnabled = true;
        public bool JobFilterEnabled
        {
            get => jobFilterEnabled;
            set
            {
                jobFilterEnabled = value;
                RaisePropertyChanged();
                ReloadGridViewData();
            }
        }

        private string currentTaskText = "Bereit";
        public string CurrentTaskText
        {
            get => currentTaskText;
            set
            {
                currentTaskText = value;
                RaisePropertyChanged();
            }
        }

        private bool nameFilterEnabled = true;
        public bool NameFilterEnabled
        {
            get => nameFilterEnabled;
            set
            {
                nameFilterEnabled = value;
                RaisePropertyChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                ReloadGridViewData();
            }
        }

        public DataGridSelectionUnit SelectionUnit => rowSelection ? DataGridSelectionUnit.FullRow : DataGridSelectionUnit.Cell;
        public DataGridSelectionMode SelectionMode => rowSelection ? DataGridSelectionMode.Single : DataGridSelectionMode.Extended;    

        private bool rowSelection = false;
        public bool RowSelection
        {
            get { return rowSelection; }
            set
            {
                rowSelection = value;
                RaisePropertyChanged("SelectionUnit");
                RaisePropertyChanged("SelectionMode");
            }
        }

        public bool NameFilterPossible => SelectedTable != null && SelectedTable.HasNameColumn;

        private List<object> selectedItems = new List<object>();

        //Commands     
        //Bearbeitung Commands
        public RelayCommand<DataGrid> CopyToNextRowCommand { get; set; }
        public RelayCommand CheckMessages { get; set; }

        public MainWindowViewModel(Logger usageLogger)
        {
            Logger = usageLogger;
            //Bearbeiten Commands
            CopyToNextRowCommand = new RelayCommand<DataGrid>(CopyToNextRow, (d) => (SelectedTable != null && selectedItems != null) && (
                                        SelectedTable.Table == TablesEnum.tjobdata && JobFilterEnabled && selectedItems.Count > 0
                                        || NameFilterEnabled && NameFilterPossible && selectedItems.Count > 0));

            CheckMessages = new RelayCommand(CheckAllMessages);

            Messenger.Default.Register<JobsChangedMessage>(this, JobsChanged);
            Messenger.Default.Register<SelectionChangedMessage>(this, (list) =>
            {
                selectedItems = list.SelectedObjects;
                CopyToNextRowCommand.RaiseCanExecuteChanged();
            });
            Messenger.Default.Register<ConnectionChangedMessage>(this, (msg) => LoadData());
            Messenger.Default.Register<StartedTaskMessage>(this, (msg) => UpdateTaskText(msg.TaskName));
            Messenger.Default.Register<EndedTaskMessage>(this, (msg) => UpdateTaskText("Bereit"));

            Messages = new ObservableCollection<string>();

            Messenger.Default.Send(new ConnectionChangedMessage());
        }

        private void UpdateTaskText(string text)
        {
            CurrentTaskText = text;
        }

        public async void LoadData()
        {
            var taskExecuter = new LSC1AsyncDBTaskExecuter();
            if (!await taskExecuter.DoTaskAsync("Versuche Verbindungsaufbau...", TryConnectToDatabase)) return;


            await taskExecuter.DoTaskAsync("Aktualisiere Datenbank...",
                () => OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings.ConnectionString));
            Jobs = await taskExecuter.DoTaskAsync("Aktualisiere Jobs...",
                () => new ObservableCollection<DbJobNameRow>(new GetJobsQuery().Execute(Connection).ToList()));

            Tables = new ObservableCollection<LSC1TablePropertiesViewModelBase>
            {
                new FrameTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new JobNameTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new MoveParamTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new PosTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcPlcTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcPulseTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ProcTurnTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new TableTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new ToolTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString),
                new TWTTableViewModel(LSC1UserSettings.Instance.DBSettings.ConnectionString)
            };

            if (Jobs.Count > 0)
                SelectedJob = Jobs[0];

            SelectedTable = Tables.First((t) => t.Table == TablesEnum.tframe);

            CheckAllMessages();
        }

        private bool TryConnectToDatabase()
        {
            try
            {
                var connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.ErrorCode)
                {
                    case 0:
                        MessageBox.Show("Keine Verbindung zur Datenbank möglich");
                        break;
                    case 1045:
                        MessageBox.Show("Ungeültiger Nutzername/Passwort");
                        break;
                    default:
                        MessageBox.Show("Unbekannter Fehler beim Verbinden mit der Datenbank.");
                        break;
                }

                Logger.Error(e, "Faild to connect to database");
                return false;
            }
        }

        #region Messenger
        public async void JobsChanged(JobsChangedMessage msg)
        {
            Jobs.Clear();

            foreach (DbJobNameRow item in await AsyncDbExecuter.DoTaskAsync("Lade Job Namen...",
                                        () => new GetJobsQuery().Execute(Connection).ToList()))
                Jobs.Add(item);

            if (msg.AddedJob == null) return;

            DbJobNameRow theJob = Jobs.First(j => j.Name.Equals(msg.AddedJob));
            SelectedJob = theJob;
        }
        #endregion Messenger

        #region Commands

        private async void CheckAllMessages()
        {
            Logger.Info("Used: Check for all orphans");

            Messages.Clear();

            var jobOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche Job Waisen...",() => new FindJobOrphansQuery().Execute(Connection))).ToList();
            var posOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche Positions Waisen...", () => new FindPosOrphansQuery().Execute(Connection))).ToList();
            var procRobotOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche ProcRobot Waisen...", () => new FindProcRobotOrphansQuery().Execute(Connection))).ToList();
            var procLaserOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche ProcLaser Waisen...", () => new FindProcLaserOrphansQuery().Execute(Connection))).ToList();
            var procFrameOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche Frame Waisen...", () => new FindFrameOrphans().Execute(Connection))).ToList();
            var procTurnOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche proc turn Waisen...",
                () => new FindProcTurnOrphans().Execute(Connection))).ToList();
            var procPulseOrphans = (await AsyncDbExecuter.DoTaskAsync("Suche proc pulse Waisen...",
                () => new FindPulseOrphans().Execute(Connection))).ToList();

            if (jobOrphans.Any())
                Messages.Add("Job Leichen entdeckt! Bitte beseitigen!");
            if (posOrphans.Any())
                Messages.Add("pos Leichen entdeckt! Bitte beseitigen!");
            if (procRobotOrphans.Any())
                Messages.Add("proc robot Leichen entdeckt! Bitte beseitigen!");
            if (procLaserOrphans.Any())
                Messages.Add("proc laser Leichen entdeckt! Bitte beseitigen!");
            if (procFrameOrphans.Any())
                Messages.Add("proc frame Leichen entdeckt! Bitte beseitigen!");
            if (procTurnOrphans.Any())
                Messages.Add("proc turn Leichen entdeckt! Bitte beseitigen!");
            if (procPulseOrphans.Any())
                Messages.Add("proc pulse Leichen entdeckt! Bitte beseitigen!");

            if (!jobOrphans.Any() && 
                !posOrphans.Any() && 
                !procRobotOrphans.Any() &&
                !procLaserOrphans.Any() &&
                !procFrameOrphans.Any() &&
                !procTurnOrphans.Any() &&
                !procPulseOrphans.Any())
                Messages.Add("No Messages");

        }

        private void CopyToNextRow(DataGrid selectedItemsContentElement)
        {
            //CopyAndInsertAt(selectedItemsContentElement);

            CopyToNext(((DataRowView)selectedItemsContentElement.SelectedItems[0]).Row);

            Logger.Info("Used CopyToNextRow");
        }

        private async void CopyToNext(DataRow row)
        {

            if (SelectedTable.Table == TablesEnum.tjobdata)
            {
                var step = int.Parse(row.ItemArray[1].ToString());
                await AsyncDbExecuter.DoTaskAsync("Erhöhe Step-Nummern...", task: () =>
                    new IncreaseJobDataStepQuery(SelectedJob.JobNr, step, "1")
                     .Execute(Connection));
            }

            if (SelectedTable.Table == TablesEnum.tprocpulse
               || SelectedTable.Table == TablesEnum.tprocplc
               || SelectedTable.Table == TablesEnum.tprocrobot
               || SelectedTable.Table == TablesEnum.tprocturn
               || SelectedTable.Table == TablesEnum.tproclaserdata)
            {
                var step = int.Parse(row.ItemArray[1].ToString());
                await AsyncDbExecuter.DoTaskAsync("Erhöhe Step-Nummern...", () =>
                    new IncreasProcLaserDataStepQuery("1", step, SelectedTable.Table.ToString(),
                        SelectedNameFilter)
                        .Execute(Connection));
            }
            
            //Create copy with higher step.
            var copy = SelectedTable.DataTable.NewRow();
            row.CopyValuesTo(copy);

            if(SelectedTable.Table == TablesEnum.tprocpulse
               || SelectedTable.Table == TablesEnum.tprocplc
               || SelectedTable.Table == TablesEnum.tprocrobot
               || SelectedTable.Table == TablesEnum.tprocturn
               || SelectedTable.Table == TablesEnum.tproclaserdata
               || SelectedTable.Table == TablesEnum.tjobdata)
            copy.SetField(1, int.Parse(row.ItemArray[1].ToString()) + 1);


            if (SelectedTable.Table == TablesEnum.tmoveparam
               || SelectedTable.Table == TablesEnum.tframe
               || SelectedTable.Table == TablesEnum.tjobname
               || SelectedTable.Table == TablesEnum.tpos)
            {
                copy[0] += RandomSnippets.RandomString(2);
            }

            new InsertQuery(copy, SelectedTable.Table.ToString()).Execute(Connection);

            Logger.Info("Used: Copy and Insert");
            ReloadGridViewData();
        }
        #endregion Commands

        private void OnSelectedJobChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;
            
            Logger.Info("Changed Selected Job to: {0}", SelectedJob.JobNr);
            ReloadGridViewData();
            UpdateNameFilter();
        }

        private void SelectedTableChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;

            //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
            Messenger.Default.Send(new TableSelectionChangedMessage(SelectedTable));
            UpdateNameFilter();

            Logger.Info("Changed table to {0} (Job {1}", SelectedTable.DataGridName, selectedJob.Name);
        }

        private async void ReloadGridViewData()
        {
            //Updaten des GridView
            string query = SQLStringGenerator.GetData(JobFilterEnabled ? SelectedJob.JobNr : null, SelectedTable.Table, NameFilterEnabled ? SelectedNameFilter : null);

            if (query == string.Empty) return;

            try
            {
                DataTable data = await AsyncDbExecuter.DoTaskAsync("Lade Tabelle: " + SelectedTable.DataGridName, 
                    () => new GetTableQuery(query).Execute(Connection));
                SelectedTable.DataTable = data;
                RaisePropertyChanged($"SelectedTable");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehlermeldung: " + ex.Message, "Fehler beim laden des Jobs.");
                Logger.Error(ex, "Error while reloading grid (job {0}, table {1})", SelectedJob, SelectedTable.DataGridName);
            }
        }

        /// <summary>
        /// Lädt die Name Filter für die Ausgewählte Tabelle
        /// </summary>
        private void UpdateNameFilter()
        {
            //update Filter
            SelectedTable.UpdateNameFilter(SelectedJob.JobNr);
            
            if (SelectedTable.NameFilterItems.Count > 0
                && (SelectedTable.UsesNameFilter))
            {
                SelectedNameFilter = SelectedTable.NameFilterItems[0];
            }
            else
            {
                SelectedNameFilter = null;
            }

            Logger.Info("Updated Name Filters (job {0}, table {1})", SelectedJob, SelectedTable.DataGridName);
        }
    }

   
}
