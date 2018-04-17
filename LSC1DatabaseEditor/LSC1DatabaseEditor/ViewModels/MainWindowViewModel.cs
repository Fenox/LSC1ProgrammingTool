using ExtensionsAndCodeSnippets.RandomTools;
using ExtensionsAndCodeSnippets.SystemData.Classes;
using ExtensionsAndCodeSnippets.SystemData.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.DatabaseEditor.ViewModels;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1Library;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LSC1DatabaseEditor.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static Logger logger = LogManager.GetLogger("Usage");
        public LSC1EditorMenuVM MenuVM { get; set; } = new LSC1EditorMenuVM();

        ObservableCollection<LSC1TablePropertiesViewModelBase> tables;
        public ObservableCollection<LSC1TablePropertiesViewModelBase> Tables
        {
            get
            {
                return tables;
            }
            set
            {
                tables = value;
                RaisePropertyChanged("Tables");
            }
        }

        private LSC1TablePropertiesViewModelBase selectedTable;
        public LSC1TablePropertiesViewModelBase SelectedTable
        {
            get { return selectedTable; }
            set
            {
                if (selectedTable == value) return;

                selectedTable = value;
                CopyToEndCommand.RaiseCanExecuteChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                SelectedTableChanged();

                RaisePropertyChanged("SelectedTable");
                RaisePropertyChanged("NameFilterPossible");
            }
        }

        public ObservableCollection<DbJobNameRow> Jobs { get; set; }

        public ObservableCollection<string> Messages { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return selectedJob; }
            set
            {
                if (selectedJob == value) return;

                selectedJob = value;
                CopyToEndCommand.RaiseCanExecuteChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                SelectedJobChanged();

                RaisePropertyChanged("SelectedJob");
            }
        }

        string selectedNameFilter;
        public string SelectedNameFilter
        {
            get { return selectedNameFilter; }
            set
            {
                selectedNameFilter = value;
                ReloadGridViewData();
                RaisePropertyChanged("SelectedNameFilter");
            }
        }

        private bool jobFilterEnabled = true;
        public bool JobFilterEnabled
        {
            get { return jobFilterEnabled; }
            set
            {
                jobFilterEnabled = value;
                RaisePropertyChanged("JobFilterEnabled");
                ReloadGridViewData();
            }
        }

        private bool nameFilterEnabled = true;
        public bool NameFilterEnabled
        {
            get { return nameFilterEnabled; }
            set
            {
                nameFilterEnabled = value;
                RaisePropertyChanged("NameFilterEnabled");
                CopyToEndCommand.RaiseCanExecuteChanged();
                CopyToNextRowCommand.RaiseCanExecuteChanged();
                ReloadGridViewData();
            }
        }

        public bool NameFilterPossible
        {
            get  { return SelectedTable != null && SelectedTable.HasNameColumn; }
        }

        List<object> selectedItems = new List<object>();

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


            Messages = new ObservableCollection<string>();

            Messenger.Default.Send(new ConnectionChangedMessage());
        }         

        private void LoadData()
        {
            if (TryConnectToDatabase())
            {
                OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings);
                Jobs = new ObservableCollection<DbJobNameRow>(LSC1DatabaseFacade.GetJobs());

                Tables = new ObservableCollection<LSC1TablePropertiesViewModelBase>
                {
                    new FrameTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new JobDataTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new JobNameTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new MoveParamTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new PosTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ProcLaserTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ProcPlcTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ProcPulseTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ProcRobotTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ProcTurnTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new TableTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new ToolTableViewModel(LSC1UserSettings.Instance.DBSettings),
                    new TWTTableViewModel(LSC1UserSettings.Instance.DBSettings)
                };

                if (Jobs.Count > 0)
                    SelectedJob = Jobs[0];

                SelectedTable = Tables.First((t) => t.Table == TablesEnum.tframe);

                CheckAllMessages();
            }
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

                logger.Error(e, "Faild to connect to database");
                return false;
            }
        }

        #region Messenger
        public void JobsChanged(JobsChangedMessage msg)
        {
            Jobs.Clear();

            foreach (var item in LSC1DatabaseFacade.GetJobs())
                Jobs.Add(item);

            if (msg.AddedJob != null)
            {
                var theJob = Jobs.First(j => j.Name.Equals(msg.AddedJob));
                SelectedJob = theJob;
            }
        }
        #endregion Messenger

        #region Commands

        void CheckAllMessages()
        {
            Messages.Clear();
            var jobCorpses = LSC1DatabaseFacade.FindJobCorpses();
            var posCorpses = LSC1DatabaseFacade.FindPosCorpses();
            var procCorpses = LSC1DatabaseFacade.FindProcCorpses();

            if (jobCorpses.Count() > 0)
                Messages.Add("Job Leichen entdeckt! Bitte beseitigen!");

            if (posCorpses.Count() > 0)
                Messages.Add("pos Leichen entdeckt! Bitte beseitigen!");

            if (procCorpses.Count() > 0)
                Messages.Add("proc Leichen entdeckt! Bitte beseitigen!");

            if (jobCorpses.Count() == 0 && posCorpses.Count() == 0 && procCorpses.Count() == 0)
                Messages.Add("No Messages");

            logger.Info("Used: Check for all corpses");
        }

        void CopyToNextRow(DataGrid selectedItemsContentElement)
        {
            int indexOfFirst = selectedItemsContentElement.SelectedIndex;
            int indexOfLast = indexOfFirst + selectedItemsContentElement.SelectedItems.Count;

            CopyAndInsertAt(selectedItemsContentElement, indexOfLast + 1);

            logger.Info("Used CopyToNextRow");
        }

        void CopyAndInsertAt(DataGrid selectedItemsContentElement, int index)
        {
            MyDataRowCollection newRowsCopy = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in (IList)selectedItemsContentElement.SelectedItems)
            {
                var newRow = SelectedTable.DataTable.NewRow();
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

                //Update all items with higher step  
                string getHigherStepsQuery = "SELECT Step FROM `tjobdata` WHERE Step > '" + highestStepInSelection + "' AND JobNr = '" + SelectedJob.JobNr + "'";
                var higherSteps = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, getHigherStepsQuery)
                    .Select(item => int.Parse(item.Step)).ToList();
                
                higherSteps.Sort();
                higherSteps.Reverse();

                foreach (var item in higherSteps)
                {
                    string setStepHigherQuery = "UPDATE `tjobdata` SET Step = Step + " + numSelectedItems + " WHERE Step = '" + item + "' AND JobNr = '" + SelectedJob.JobNr +"'";                    
                    LSC1DatabaseFacade.SimpleQuery(setStepHigherQuery);
                }
            }

            if (SelectedTable.Table == TablesEnum.tprocpulse
                || SelectedTable.Table == TablesEnum.tprocplc
                || SelectedTable.Table == TablesEnum.tprocrobot
                || SelectedTable.Table == TablesEnum.tprocturn
                || SelectedTable.Table == TablesEnum.tproclaserdata)
            {
                newRowsCopy.ChangeRowElementAddIndex(1, highestStepInSelection + 1, "");

                //Update all items with higher step  
                string getHigherStepsQuery = "SELECT Step FROM `" + SelectedTable.Table + "` WHERE Step > '" + highestStepInSelection + "' AND Name = '" + SelectedNameFilter + "'";
                var higherSteps = LSC1DatabaseFacade.Read<DbJobDataRow>(LSC1UserSettings.Instance.DBSettings, getHigherStepsQuery)
                    .Select(item => int.Parse(item.Step)).ToList();

                higherSteps.Sort();
                higherSteps.Reverse();

                foreach (var item in higherSteps)
                {
                    string setStepHigherQuery = "UPDATE `" + SelectedTable.Table + "` SET Step = Step + '" + numSelectedItems + "' WHERE Step = '" + item + "' AND Name = '" + SelectedNameFilter + "'";
                    LSC1DatabaseFacade.SimpleQuery(setStepHigherQuery);
                }
            }

            if(SelectedTable.Table == TablesEnum.tmoveparam
                || SelectedTable.Table == TablesEnum.tframe
                || SelectedTable.Table == TablesEnum.tjobname)
            {
                foreach (var newRow in newRowsCopy)
                {
                    newRow[0] += RandomSnippets.RandomString(2);
                }
            }

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                foreach (var item in newRowsCopy)
                {
                    LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, item, SelectedTable.Table.ToString());
                    //TODO SelectedTable.DataTable.Rows.Add(item);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
                logger.Error("Error in Copy to end");
            }

            logger.Info("Used: Copy and Insert at");
            ReloadGridViewData();
        }

        void CopyToEnd(DataGrid selectedItemsContentElement)
        {
            MyDataRowCollection coll = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in (IList)selectedItemsContentElement.SelectedItems)
            {
                var newRow = SelectedTable.DataTable.NewRow();
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

                
                int highestStep = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, highestStepQuery)
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
                int highestStep = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, highestStepQuery)
                  .Select(val => int.Parse(val.Values[0]))
                  .First();

                coll.ChangeRowElementAddIndex(1, highestStep + 1, "");
            }

            //Bei Auswahl von ttool
            if (SelectedTable.Table == TablesEnum.ttool)
                coll.ChangeRowElementAddIndex(0, "newTool" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von twt
            if (SelectedTable.Table == TablesEnum.twt)
            {
                string highestNumQuery = "SELECT MAX(CAST(WtId AS SIGNED)) FROM `twt`";
                int highestId = LSC1DatabaseFacade.Read<DbRow>(LSC1UserSettings.Instance.DBSettings, highestNumQuery)
                  .Select(val => int.Parse(val.Values[0]))
                  .First();

                coll.ChangeRowElementAddIndex(1, highestId + 1, "");
            }

            //Neue Reihen zum dataGrid hinzufügen
            foreach (var row in coll)
                SelectedTable.DataTable.Rows.Add(row);

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                //TODO: insertMulti funktion
                foreach (var item in coll)
                {
                    LSC1DatabaseFacade.Insert(LSC1UserSettings.Instance.DBSettings, item, SelectedTable.Table.ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
                logger.Error("Error in Copy to end");
            }

            logger.Info("Used: Copy to end");
        }
        #endregion Commands


        void SelectedJobChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;


            logger.Info("Changed Selected Job to: {0}", SelectedJob.JobNr);
            ReloadGridViewData();
            UpdateNameFilter();
        }

        void SelectedTableChanged()
        {
            if (SelectedTable == null || SelectedJob == null)
                return;

            //Wird an die View gesendet, um dort das Tabellen-Layout zu ändern.
            Messenger.Default.Send(new TableSelectionChangedMessage(SelectedTable));
            UpdateNameFilter();

            logger.Info("Changed table to {0} (Job {1}", SelectedTable.DataGridName, selectedJob.Name);
        }

        void ReloadGridViewData()
        {
            //Updaten des GridView

            string query = SQLStringGenerator.GetData(JobFilterEnabled ? SelectedJob.JobNr : null, SelectedTable.Table, NameFilterEnabled ? SelectedNameFilter : null);

            if (query != string.Empty)
            {
                try
                {
                    var data = LSC1DatabaseFacade.GetTable(query);
                    SelectedTable.DataTable = data;
                    RaisePropertyChanged("SelectedTable");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehlermeldung: " + ex.Message, "Fehler beim laden des Jobs.");
                    logger.Error(ex, "Error while reloading grid (job {0}, table {1})", SelectedJob, SelectedTable.DataGridName);
                }
            }
        }

        /// <summary>
        /// Lädt die Name Filter für die Ausgewählte Tabelle
        /// </summary>
        void UpdateNameFilter()
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

            logger.Info("Updated Name Filters (job {0}, table {1})", SelectedJob, SelectedTable.DataGridName);
        }
    }

   
}
