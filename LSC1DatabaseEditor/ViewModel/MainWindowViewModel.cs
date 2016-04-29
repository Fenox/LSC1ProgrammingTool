using ExtensionsAndCodeSnippets.RandomTools;
using ExtensionsAndCodeSnippets.SystemData.Classes;
using ExtensionsAndCodeSnippets.SystemData.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Model;
using LSC1DatabaseEditor.Views;
using LSC1DatabaseLibrary;
using LSC1DatabaseLibrary.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LSC1DatabaseEditor.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<TablesEnum> Tables { get; set; }

        private TablesEnum selectedTable;
        public TablesEnum SelectedTable
        {
            get { return selectedTable; }
            set
            {
                if (selectedTable == value) return;

                selectedTable = value;
                RaisePropertyChanged("SelectedTable");
                RaisePropertyChanged("NameFilterPossible");
                CopyToEndCommand.RaiseCanExecuteChanged();
                SelectedTableChanged();
            }
        }

        public ObservableCollection<DbJobNameRow> Jobs { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return selectedJob; }
            set
            {
                if (selectedJob == value) return;

                selectedJob = value;
                RaisePropertyChanged("SelectedJob");

                CreateFrameCommand.RaiseCanExecuteChanged();
                CreatePosCommand.RaiseCanExecuteChanged();
                CreateProcCommand.RaiseCanExecuteChanged();
                CreateProcPlcCommand.RaiseCanExecuteChanged();
                CreateMoveparamCommand.RaiseCanExecuteChanged();

                CopyToEndCommand.RaiseCanExecuteChanged();
                SelectedJobChanged();
            }
        }

        private DataTable tableData;
        public DataTable TableData
        {
            get { return tableData; }
            set
            {
                if (tableData == value)
                    return;

                tableData = value;
                RaisePropertyChanged("TableData");
            }
        }

        public ObservableCollection<string> NameFilterItems { get; set; } = new ObservableCollection<string>();

        string selectedFilter;
        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                selectedFilter = value;
                RaisePropertyChanged("SelectedFilter");

                SelectedFilterChanged();
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
                UpdateGridView();
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
                UpdateGridView();
            }
        }

        public bool NameFilterPossible
        {
            get
            {
                return SelectedTable == TablesEnum.tframe
                            || SelectedTable == TablesEnum.tjobdata
                            || SelectedTable == TablesEnum.tjobname
                            || SelectedTable == TablesEnum.tmoveparam
                            || SelectedTable == TablesEnum.tpos
                            || SelectedTable == TablesEnum.tprocplc
                            || SelectedTable == TablesEnum.tprocrobot
                            || SelectedTable == TablesEnum.tprocpulse
                            || SelectedTable == TablesEnum.tproclaserdata
                            || SelectedTable == TablesEnum.ttool
                            || SelectedTable == TablesEnum.twt;
            }

        }

        //Commands
        //Job Button Commands
        public ICommand CopyJobCommand { get; set; }
        public ICommand DeleteJobCommand { get; set; }

        //Menu Datei Commands
        public ICommand CloseWindowCommand { get; set; }

        //Menu Leichen Commands
        public ICommand FindJobCorpsesCommand { get; set; }
        public ICommand FindProcCorpsesCommand { get; set; }
        public ICommand FindPosCorpsesCommand { get; set; }

        //Menu Erstellen Commands
        public RelayCommand CreateFrameCommand { get; set; }
        public RelayCommand CreatePosCommand { get; set; }
        public RelayCommand CreateProcCommand { get; set; }
        public RelayCommand CreateProcPlcCommand { get; set; }
        public RelayCommand CreateMoveparamCommand { get; set; }

        //Bearbeitung Commands
        public RelayCommand<DataGrid> CopyToEndCommand { get; set; }
        public RelayCommand<DataGrid> CopyToNextRowCommand { get; set; }

        public MainWindowViewModel()
        {
            Jobs = new ObservableCollection<DbJobNameRow>(LSC1DatabaseFunctions.GetJobs());

            Tables = new ObservableCollection<TablesEnum>();
            Tables.Add(TablesEnum.tframe);
            Tables.Add(TablesEnum.tjobdata);
            Tables.Add(TablesEnum.tjobname);
            Tables.Add(TablesEnum.tmoveparam);
            Tables.Add(TablesEnum.tpos);
            Tables.Add(TablesEnum.tproclaserdata);
            Tables.Add(TablesEnum.tprocplc);
            Tables.Add(TablesEnum.tprocpulse);
            Tables.Add(TablesEnum.tprocrobot);
            Tables.Add(TablesEnum.tprocturn);
            Tables.Add(TablesEnum.ttable);
            Tables.Add(TablesEnum.ttool);
            Tables.Add(TablesEnum.twt);

            //Commands
            //Job Button Commands
            CopyJobCommand = new RelayCommand(OpenCopyJobWindow);
            DeleteJobCommand = new RelayCommand(OpenDeleteJobWindow);

            //Menu Datei Commands
            CloseWindowCommand = new RelayCommand<Window>((wnd) => wnd.Close());

            //Menu Corpses Commands
            FindJobCorpsesCommand = new RelayCommand(OpenFindJobCorpses);
            FindProcCorpsesCommand = new RelayCommand(OpenFindProcCorpses);
            FindPosCorpsesCommand = new RelayCommand(OpenFindPosCorpses);

            //Menu Erstellen Commands
            CreateFrameCommand = new RelayCommand(CreateNewFrame, () => SelectedJob != null);
            CreatePosCommand = new RelayCommand(CreateNewPos, () => SelectedJob != null);
            CreateProcCommand = new RelayCommand(CreateNewProc, () => SelectedJob != null);
            CreateProcPlcCommand = new RelayCommand(CreateNewProcPlc, () => SelectedJob != null);
            CreateMoveparamCommand = new RelayCommand(CreateNewMoveparam, () => SelectedJob != null);

            //Bearbeiten Commands
            CopyToEndCommand = new RelayCommand<DataGrid>(CopyToEnd, (d) => NameFilterEnabled && NameFilterPossible);
            CopyToNextRowCommand = new RelayCommand<DataGrid>(CopyToNextRow, (d) => NameFilterEnabled && NameFilterPossible);

            Messenger.Default.Register<JobsChangedMessage>(this, JobsChanged);

            if (Jobs.Count > 0)
                SelectedJob = Jobs[0];

            SelectedTable = TablesEnum.tframe;
        }

        private void TableData_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            string deleteQuery = "DELETE FROM `" + SelectedTable + "` WHERE ";

            //Konvertieren in Liste von Strings
            var values = new List<string>();
            foreach (var item in e.Row.ItemArray)
                values.Add(item.ToString());

            int i = 0;
            foreach (var item in values)
            {
                deleteQuery += "`" + e.Row.Table.Columns[i] + "` = '" + item + "' AND ";
                i++;
            }

            deleteQuery = deleteQuery.Remove(deleteQuery.Length - 5);

            LSC1DatabaseConnector db = new LSC1DatabaseConnector();
            db.ExecuteQuery(deleteQuery);

            //Updaten der OfflineDatenbank
            OfflineDatabase.UpdateTable(SelectedTable);
        }

        #region Messenger
        public void JobsChanged(JobsChangedMessage msg)
        {
            Jobs.Clear();

            foreach (var item in LSC1DatabaseFunctions.GetJobs())
                Jobs.Add(item);

            if (msg.AddedJob != null)
            {
                var theJob = Jobs.First(j => j.Name.Equals(msg.AddedJob));
                SelectedJob = theJob;
            }
        }
        #endregion Messenger

        #region Commands
        public void OpenFindJobCorpses()
        {
            var windows = new FindJobCorpsesWindow();
            windows.Show();
        }

        public void OpenFindProcCorpses()
        {
            var windows = new FindProcCorpsesWindow();
            windows.Show();
        }


        public void OpenFindPosCorpses()
        {
            var windows = new FindPosCorpsesWindow();
            windows.Show();
        }

        public void OpenCopyJobWindow()
        {
            var window = new CopyJobWindow(); //HACK neues fenster sollte folgendermaßen erstellt werden http://stackoverflow.com/questions/25845689/opening-new-window-in-mvvm-wpf
            window.Show();
        }

        public void OpenDeleteJobWindow()
        {
            var window = new DeleteJobWindow();
            window.Show();
        }

        void CopyToNextRow(DataGrid selectedItemsContentElement)
        {
            int indexOfFirst = selectedItemsContentElement.SelectedIndex;
            int indexOfLast = indexOfFirst + selectedItemsContentElement.SelectedItems.Count;

            CopyAndInsertAt(selectedItemsContentElement, indexOfLast + 1);
        }

        void CopyAndInsertAt(DataGrid selectedItemsContentElement, int index)
        {
            MyDataRowCollection newRowsCopy = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in (IList)selectedItemsContentElement.SelectedItems)
            {
                var newRow = TableData.NewRow();
                row.Row.CopyValuesTo(newRow);
                newRowsCopy.Add(newRow);
            }

            int highestStepInSelection = int.Parse(newRowsCopy.Last().ItemArray[1].ToString());
            int numOFFollowingEntries = (TableData.Rows.Count) - highestStepInSelection;
            //TODO nur möglich, bei Elementen mit zählbarem Primärschlüssel bzw. Zeile wie Step

            //TODO darf nur möglich sein, wenn:
            //1. Die Tabelle nach Step sortiert ist
            //2. Der Job Filter aktiv ist
            //Bei Auswahl von tjobdata
            if (SelectedTable == TablesEnum.tjobdata)
            {
                //Set Steps in new Rows
                newRowsCopy.ChangeRowElementAddIndex(1, highestStepInSelection + 1, "");

                //Update all items with higher step
                //Alle nachfolgenden weit nach hinten setzten um Kollisionen mit Primärschlüsseln zu umgehen
                string setStepsHigherQuery = "UPDATE `tjobdata` SET Step = Step + " + (numOFFollowingEntries + newRowsCopy.Count) + " WHERE Step > '" + (highestStepInSelection) + "' AND JobNr = '" + SelectedJob.JobNr + "'";
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.ExecuteQuery(setStepsHigherQuery);

                //Zurücksetzten der Indizes
                string setStepsBackQuery = "UPDATE `tjobdata` SET Step = Step - " + (numOFFollowingEntries) + " WHERE Step > '" + (highestStepInSelection) + "' AND JobNr = '" + SelectedJob.JobNr + "'";
                db.ExecuteQuery(setStepsBackQuery);
            }

            //TODO funktioniert nur wenn Name Filter aktiv ist!
            //Bei Auswahl von ...
            if (SelectedTable == TablesEnum.tprocpulse
                || SelectedTable == TablesEnum.tprocplc
                || SelectedTable == TablesEnum.tprocrobot
                || SelectedTable == TablesEnum.tprocturn
                || SelectedTable == TablesEnum.tproclaserdata)
            {
                newRowsCopy.ChangeRowElementAddIndex(1, highestStepInSelection + 1, "");
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();

                //Erhöhen aller nachfolgender Steps um die Anzahl der zu kopierenden Steps
                var higherStepNumsQuery = "SELECT Step FROM `" + SelectedTable + "` WHERE STEP > " + highestStepInSelection + " AND Name = '" + SelectedFilter + "'";

                //Parse from List of string to int
                List<int> intNums = new List<int>();
                db.ReadSingleColumnQuery(higherStepNumsQuery).ForEach(el => intNums.Add(int.Parse(el)));
                intNums.Sort();
                intNums.Reverse();

                foreach (var step in intNums)
                {
                    string updateStepsQuery = "UPDATE `" + SelectedTable + "` SET Step = '" + (step + newRowsCopy.Count) + "' WHERE Name = '" + SelectedFilter + "' AND Step = '" + step + "'";
                    db.ExecuteQuery(updateStepsQuery);
                }
            }

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                foreach (var item in newRowsCopy)
                {
                    db.Insert(item, SelectedTable.ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
            }


            UpdateGridView();
        }

        void CopyToEnd(DataGrid selectedItemsContentElement)
        {
            MyDataRowCollection coll = new MyDataRowCollection();

            //Kopieren der ausgewählten Reihen
            foreach (DataRowView row in (IList)selectedItemsContentElement.SelectedItems)
            {
                var newRow = TableData.NewRow();
                row.Row.CopyValuesTo(newRow);
                coll.Add(newRow);
            }

            //Bei Auswahl von tframe
            if (SelectedTable == TablesEnum.tframe)//Hinzufügen eines Random Strings an den Framename (Primärschlüssel)
            {
                coll.ChangeRowElementAddIndex(0, "f" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");
            }

            //Bei Auswahl von tjobdata
            if (SelectedTable == TablesEnum.tjobdata)
            {
                //Höchste Step Nummer herausfinden
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                string highestStepQuery = "SELECT MAX(CAST(Step AS SIGNED)) FROM `tjobdata` WHERE JobNr = '" + SelectedJob.JobNr + "'";
                int highestStep = int.Parse(db.ReadSingleColumnQuery(highestStepQuery)[0]);

                coll.ChangeRowElementAddIndex(1, highestStep + 1, "");
            }

            //jobname
            if (SelectedTable == TablesEnum.tjobname)
                coll.ChangeRowElementAddIndex(0, "newJob" + SelectedJob.Name + RandomSnippets.RandomString(5) + "Nr");

            //moveParam
            if (SelectedTable == TablesEnum.tmoveparam)
                coll.ChangeRowElementAddIndex(0, "newMove" + SelectedJob.Name + RandomSnippets.RandomString(5) + "Nr");

            //Bei Auswahl von tpos
            if (SelectedTable == TablesEnum.tpos)
                coll.ChangeRowElementAddIndex(0, "p" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von ...
            if (SelectedTable == TablesEnum.tprocpulse
                || SelectedTable == TablesEnum.tprocplc
                || SelectedTable == TablesEnum.tprocrobot
                || SelectedTable == TablesEnum.tprocturn
                || SelectedTable == TablesEnum.tproclaserdata)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                string highestStepQuery = SQLStringGenerator.HighestStep(SelectedTable, SelectedFilter);
                int highestStep = int.Parse(db.ReadSingleColumnQuery(highestStepQuery)[0]);

                coll.ChangeRowElementAddIndex(1, highestStep + 1, "");
            }

            //Bei Auswahl von ttool
            if (SelectedTable == TablesEnum.ttool)
                coll.ChangeRowElementAddIndex(0, "newTool" + SelectedJob.Name + RandomSnippets.RandomString(3) + "Nr");

            //Bei Auswahl von twt
            if (SelectedTable == TablesEnum.twt)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                string highestNumQuery = "SELECT MAX(CAST(WtId AS SIGNED)) FROM `twt`";
                int highestId = int.Parse(db.ReadSingleColumnQuery(highestNumQuery)[0]);

                coll.ChangeRowElementAddIndex(1, highestId + 1, "");
            }

            //Neue Reihen zum dataGrid hinzufügen
            foreach (var row in coll)
                TableData.Rows.Add(row);

            //Schreiben der veränderten Werte in die Datenbank!!
            try
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                foreach (var item in coll)
                {
                    db.Insert(item, SelectedTable.ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler, womöglich wurde ein Primärschlüssel doppelt eingefügt. Einfach nochmal probieren...");
            }
        }

        //Menu Erstellen Commands
        void CreateNewFrame()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Frame";
            dataContext.LabelText = "Geben sie den Namen des neuen Frames ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.Insert(new DbFrameRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllFrameNames();
            }
        }

        void CreateNewPos()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Pos Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Pos Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.Insert(new DbPosRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllPosNames();
            }
        }

        void CreateNewProc()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer Proc Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen Proc Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.Insert(new DbProcRobotRow() { Name = dataContext.TextBoxText });
                db.Insert(new DbProcLaserDataRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllProcNames();
            }
        }

        void CreateNewProcPlc()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer ProcPLC Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen ProcPLC Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.Insert(new DbProcPlcRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllProcPLCNames();
            }
        }

        void CreateNewMoveparam()
        {
            MyTextMessageBox form = new MyTextMessageBox();
            TextMessageBoxViewModel dataContext = form.DataContext as TextMessageBoxViewModel;
            dataContext.Title = "Neuer MoveParam Eintrag";
            dataContext.LabelText = "Geben sie den Namen des neuen MoveParam Eintrages ein";

            bool? result = form.ShowDialog();
            if (result.HasValue && result.Value && dataContext.TextBoxText.Length > 0)
            {
                LSC1DatabaseConnector db = new LSC1DatabaseConnector();
                db.Insert(new DbMoveParamRow() { Name = dataContext.TextBoxText });
                OfflineDatabase.UpdateAllMoveParamNames();
            }
        }
        #endregion Commands


        void SelectedJobChanged()
        {
            if (SelectedTable == TablesEnum.NOTABLE || SelectedJob == null)
                return;

            UpdateGridView();
            UpdateFilter();
        }

        void SelectedTableChanged()
        {
            if (SelectedTable == TablesEnum.NOTABLE || SelectedJob == null)
                return;
            
            UpdateGridView();
            UpdateFilter();
        }

        void SelectedFilterChanged()
        {
            UpdateGridView();
        }

        void UpdateGridView()
        {
            //Updaten des GridView
            LSC1DatabaseConnector db = new LSC1DatabaseConnector();

            string query = SQLStringGenerator.GetData(JobFilterEnabled ? SelectedJob.JobNr : null, SelectedTable, NameFilterEnabled ? SelectedFilter : null);

            if (query != string.Empty)
            {
                var data = db.GetTable(query);

                TableData = data;

                TableData.RowDeleting += TableData_RowDeleting;
            }
            else
            {
                TableData = null;
            }
        }

        /// <summary>
        /// Lädt die Name Filter für die Ausgewählte Tabelle
        /// </summary>
        void UpdateFilter()
        {
            //update Filter
            var newNameFilterItemsNotDistinct = new List<string>();

            if (SelectedTable == TablesEnum.tproclaserdata)
            {
                string getLaserProcsOfJob = "SELECT * FROM tproclaserdata" +
                            " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + SelectedJob.JobNr + ")";

                LSC1DatabaseConnector db = new LSC1DatabaseConnector();

                var procLaserOfJob = db.ReadRows<DbProcLaserDataRow>(getLaserProcsOfJob);

                foreach (var item in procLaserOfJob)
                    newNameFilterItemsNotDistinct.Add(item.Name);
            }
            else if (SelectedTable == TablesEnum.tprocrobot)
            {
                string getLaserProcsOfJobQuery = "SELECT * FROM `tprocrobot`" +
                   " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + SelectedJob.JobNr + ")";

                LSC1DatabaseConnector db = new LSC1DatabaseConnector();

                var procLaserOfJob = db.ReadRows<DbProcRobotRow>(getLaserProcsOfJobQuery);

                foreach (var item in procLaserOfJob)
                    newNameFilterItemsNotDistinct.Add(item.Name);
            }
            else if (SelectedTable == TablesEnum.tprocplc)
            {
                string getProcplcOfJobQuery = "SELECT * FROM `tprocplc`" +
                   " WHERE Name IN (SELECT DISTINCT Name FROM tjobdata WHERE JobNr = " + SelectedJob.JobNr + ")";

                LSC1DatabaseConnector db = new LSC1DatabaseConnector();

                var procLaserOfJob = db.ReadRows<DbProcPlcRow>(getProcplcOfJobQuery);

                foreach (var item in procLaserOfJob)
                    newNameFilterItemsNotDistinct.Add(item.Name);
            }


            var distinctStrings = newNameFilterItemsNotDistinct.Distinct();

            NameFilterItems.Clear();

            foreach (var item in distinctStrings)
                NameFilterItems.Add(item);

            if (NameFilterItems.Count > 0
                && (SelectedTable == TablesEnum.tprocrobot
                    || SelectedTable == TablesEnum.tproclaserdata
                    || SelectedTable == TablesEnum.tprocplc))
            {
                SelectedFilter = NameFilterItems[0];
            }
        }
    }
}
