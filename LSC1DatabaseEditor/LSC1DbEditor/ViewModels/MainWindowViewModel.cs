using ExtensionsAndCodeSnippets.RandomTools;
using ExtensionsAndCodeSnippets.SystemData.Classes;
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
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncDbExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly Logger Logger = LogManager.GetLogger("Usage");
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);
        private static readonly LSC1DbFunctionCollection Functions =
            new LSC1DbFunctionCollection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public LSC1EditorMenuVM MenuVM { get; set; } = new LSC1EditorMenuVM();

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
                CopyToEndCommand.RaiseCanExecuteChanged();
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
                CopyToEndCommand.RaiseCanExecuteChanged();
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
                CopyToEndCommand.RaiseCanExecuteChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                ReloadGridViewData();
            }
        }

        public bool NameFilterPossible => SelectedTable != null && SelectedTable.HasNameColumn;

        private List<object> selectedItems = new List<object>();

        //Commands     
        //Bearbeitung Commands
        public RelayCommand<DataGrid> CopyToEndCommand { get; set; }
        public RelayCommand<DataGrid> CopyToNextRowCommand { get; set; }
        public RelayCommand CheckMessages { get; set; }

        public MainWindowViewModel()
        {
            //Bearbeiten Commands
            CopyToEndCommand = new RelayCommand<DataGrid>(CopyToEnd, (d) => SelectedTable != null && (
                                        SelectedTable.Table == TablesEnum.tjobdata && JobFilterEnabled && selectedItems.Count > 0
                                        || NameFilterEnabled && NameFilterPossible && selectedItems.Count > 0));

            CopyToNextRowCommand = new RelayCommand<DataGrid>(CopyToNextRow, (d) => (SelectedTable != null && selectedItems != null) && (
                                        SelectedTable.Table == TablesEnum.tjobdata && JobFilterEnabled && selectedItems.Count > 0
                                        || NameFilterEnabled && NameFilterPossible && selectedItems.Count > 0));

            CheckMessages = new RelayCommand(CheckAllMessages);

            Messenger.Default.Register<JobsChangedMessage>(this, JobsChanged);
            Messenger.Default.Register<SelectionChangedMessage>(this, (list) =>
            {
                selectedItems = list.SelectedObjects;
                CopyToEndCommand.RaiseCanExecuteChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
            });
            Messenger.Default.Register<ConnectionChangedMessage>(this, (msg) => LoadData());
            Messenger.Default.Register<StartedTaskMessage>(this, (msg) => CurrentTaskText = msg.TaskName);
            Messenger.Default.Register<EndedTaskMessage>(this, (msg) => CurrentTaskText = "Bereit");

            Messages = new ObservableCollection<string>();

            Messenger.Default.Send(new ConnectionChangedMessage());
        }         


        private async void LoadData()
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

        private static bool TryConnectToDatabase()
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

            if (!jobOrphans.Any() && !posOrphans.Any() && !procRobotOrphans.Any())
                Messages.Add("No Messages");

        }

        private async void CopyToNextRow(DataGrid selectedItemsContentElement)
        {
            int indexOfFirst = selectedItemsContentElement.SelectedIndex;
            int indexOfLast = indexOfFirst + selectedItemsContentElement.SelectedItems.Count;

            await CopyAndInsertAt(selectedItemsContentElement);

            Logger.Info("Used CopyToNextRow");
        }

        private async Task CopyAndInsertAt(MultiSelector selectedItemsContentElement)
        {
            var newRowsCopy = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in selectedItemsContentElement.SelectedItems)
            {
                DataRow newRow = SelectedTable.DataTable.NewRow();
                row.Row.CopyValuesTo(newRow);
                newRowsCopy.Add(newRow);
            }

            int numSelectedItems = selectedItemsContentElement.SelectedItems.Count;
            int highestStepInSelection = int.Parse(newRowsCopy.Last().ItemArray[1].ToString());

            //TODO darf nur möglich sein, wenn:
            //1. Der Job Filter aktiv ist
            //Bei Auswahl von tjobdata
            if (SelectedTable.Table == TablesEnum.tjobdata)
            {
                newRowsCopy.ChangeRowElementAddIndex(1, highestStepInSelection + 1, "");

                await AsyncDbExecuter.DoTaskAsync("Erhöhe Step-Nummern...", task: () =>
                    new IncreaseJobDataStepQuery(SelectedJob.JobNr, highestStepInSelection - 1,
                         numSelectedItems.ToString())
                     .Execute(Connection));
            }

            if (SelectedTable.Table == TablesEnum.tprocpulse
                || SelectedTable.Table == TablesEnum.tprocplc
                || SelectedTable.Table == TablesEnum.tprocrobot
                || SelectedTable.Table == TablesEnum.tprocturn
                || SelectedTable.Table == TablesEnum.tproclaserdata)
            {
                newRowsCopy.ChangeRowElementAddIndex(1, highestStepInSelection + 1, "");

                await AsyncDbExecuter.DoTaskAsync("Erhöhe Step-Nummern...", () =>
                    new IncreasProcStepQuery(numSelectedItems.ToString(), highestStepInSelection - 1, SelectedTable.Table.ToString(),
                        SelectedNameFilter)
                        .Execute(Connection));
            }

            if(SelectedTable.Table == TablesEnum.tmoveparam
                || SelectedTable.Table == TablesEnum.tframe
                || SelectedTable.Table == TablesEnum.tjobname)
            {
                foreach (DataRow newRow in newRowsCopy)
                {
                    newRow[0] += RandomSnippets.RandomString(2);
                }
            }

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                foreach (DataRow item in newRowsCopy)
                {
                    new InsertQuery(item, SelectedTable.Table.ToString()).Execute(Connection);
                    //TODO SelectedTable.DataTable.Rows.Add(item);
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
                Logger.Error(e, "Error in Copy to end");
            }

            Logger.Info("Used: Copy and Insert At");
            ReloadGridViewData();
        }

        private void CopyToEnd(DataGrid selectedItemsContentElement)
        {
            var coll = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in selectedItemsContentElement.SelectedItems)
            {
                DataRow newRow = SelectedTable.DataTable.NewRow();
                row.Row.CopyValuesTo(newRow);
                coll.Add(newRow);
            }

            //Bei Auswahl von tframe
            if (SelectedTable.Table == TablesEnum.tframe)//Hinzufügen eines Random Strings an den Framename (Primärschlüssel)
            {
                coll.ChangeRowElementAddIndex(0, "f" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");
            }

            //Bei Auswahl von tjobdata
            if (SelectedTable.Table == TablesEnum.tjobdata)
            {
                //Höchste Step Nummer herausfinden
                string highestStepQuery = "SELECT MAX(CAST(Step AS SIGNED)) FROM `tjobdata` WHERE JobNr = '" + SelectedJob.JobNr + "'";

                
                int highestStep = new ReadRowsQuery<DbRow>(highestStepQuery).Execute(Connection)
                    .Select(val => int.Parse(val.Values[0]))
                    .First();

                coll.ChangeRowElementAddIndex(1, highestStep + 1, "");
            }

            //jobname
            if (SelectedTable.Table == TablesEnum.tjobname)
                coll.ChangeRowElementAddIndex(0, "newJob" + SelectedJob.Name + RandomSnippets.RandomString(5) + "Nr");

            //moveParam
            if (SelectedTable.Table == TablesEnum.tmoveparam)
                coll.ChangeRowElementAddIndex(0, "newMove" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von tpos
            if (SelectedTable.Table == TablesEnum.tpos)
                coll.ChangeRowElementAddIndex(0, "p" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von ...
            if (SelectedTable.Table == TablesEnum.tprocpulse
                || SelectedTable.Table == TablesEnum.tprocplc
                || SelectedTable.Table == TablesEnum.tprocrobot
                || SelectedTable.Table == TablesEnum.tprocturn
                || SelectedTable.Table == TablesEnum.tproclaserdata)
            {
                string highestStepQuery = SQLStringGenerator.HighestStep(SelectedTable.Table, SelectedNameFilter);
                int highestStep = new ReadRowsQuery<DbRow>(highestStepQuery).Execute(Connection)
                  .Select(val => int.Parse(val.Values[0]))
                  .First();

                coll.ChangeRowElementAddIndex(1, highestStep + 1, "");
            }

            //Bei Auswahl von ttool
            if (SelectedTable.Table == TablesEnum.ttool)
                coll.ChangeRowElementAddIndex(0, "newTool" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von twt
            const string highestNumQuery = "SELECT MAX(CAST(WtId AS SIGNED)) FROM `twt`";
            if (SelectedTable.Table == TablesEnum.twt)
            {
                int highestId = new ReadRowsQuery<DbRow>(highestNumQuery).Execute(Connection)
                  .Select(val => int.Parse(val.Values[0]))
                  .First();

                coll.ChangeRowElementAddIndex(1, highestId + 1, "");
            }

            //Neue Reihen zum dataGrid hinzufügen
            foreach (DataRow row in coll)
                SelectedTable.DataTable.Rows.Add(row);

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                //TODO: insertMulti funktion
                foreach (DataRow item in coll)
                    new InsertQuery(item, SelectedTable.Table.ToString()).Execute(Connection);
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
                Logger.Error("Error in Copy to end");
            }

            Logger.Info("Used: Copy to end");
        }
        #endregion Commands

        private async void OnSelectedJobChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;
            
            Logger.Info("Changed Selected Job to: {0}", SelectedJob.JobNr);
            ReloadGridViewData();
            await UpdateNameFilter();
        }

        private async void SelectedTableChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;

            //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
            Messenger.Default.Send(new TableSelectionChangedMessage(SelectedTable));
            Task updateNameFilterTask = UpdateNameFilter();
            if(updateNameFilterTask != null)
                await updateNameFilterTask;

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
        private async Task UpdateNameFilter()
        {
            //update Filter
            Task updateTask = SelectedTable.UpdateNameFilter(SelectedJob.JobNr);
            if (updateTask != null)
                await updateTask;

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
