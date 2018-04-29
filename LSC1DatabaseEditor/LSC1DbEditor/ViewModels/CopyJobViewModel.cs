using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1Database;
using LSC1DatabaseEditor.LSC1Database.Queries.Job;
using LSC1DatabaseEditor.LSC1DbEditor.Controller;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DataStructures;
using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.CommonMySql.MySqlQueries;
using MySql.Data.MySqlClient;
using NLog;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class CopyJobViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly Logger Logger = LogManager.GetLogger("Usage");
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public List<DbJobNameRow> Jobs { get; set; }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
            set
            {
                selectedJob = value;
                SelectedJobChanged();
                RaisePropertyChanged();
                CopyJobCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> CopyJobCommand { get; set; }

        private string newJobName;
        public string NewJobName
        {
            get => newJobName;
            set
            {
                newJobName = value;
                RaisePropertyChanged();
                CopyJobCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<TreeViewItem> TreeItems { get; set; } = new ObservableCollection<TreeViewItem>();

        public CopyJobViewModel()
        {
            CopyJobCommand = new RelayCommand<Window>(CopyJob, (wnd) => SelectedJob != null && NewJobName != null && NewJobName.Length > 0);
            //TODO: Make async
            Jobs = new ReadRowsQuery<DbJobNameRow>("SELECT * FROM `tjobname`").Execute(Connection).ToList();

            Messenger.Default.Register<TextChangedMessage>(this, (m) =>
            {
                NewJobName = m.NewText;
                CopyJobCommand.RaiseCanExecuteChanged();
            });
        }

        private void SelectedJobChanged()
        {
            //TODO: make async + log
            TreeItems.Clear();

            //Laden der twt Frames
            string twtQueryFrame1 = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";
            var frame1 = new ReadRowsQuery<DbRow>(twtQueryFrame1).Execute(Connection)
                .Select(val => val.Values[0]).ToList();

            //Nur wenn ein Eintrag zum kopieren besteht.
            var itemnNeg1 = new TreeViewItem
            {
                Text = "twt Frame"
            };
            itemnNeg1.SubItems.Add(new CheckableItemWithSub()
            {
                Text = frame1.Any() ? frame1.First() : "Neuer Eintrag wird erstellt",
                Checked = true
            });

            TreeItems.Add(itemnNeg1);

            //Laden der Frames
            string tframesQuery = "SELECT Frame FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP By `Frame`";
            var frameDataList = new ReadRowsQuery<DbRow>(tframesQuery).Execute(Connection)
                .Select(val => val.Values[0]);
            TreeItems.Add(CreateTreeViewItem("tframe", frameDataList));


            //Laden von tprocdata
            string tprocdataQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'proc' GROUP By `Name`";
            var procDataList = new ReadRowsQuery<DbJobDataRow>(tprocdataQuery).Execute(Connection)
                .Select(val => val.Name);
            TreeItems.Add(CreateTreeViewItem("tproc", procDataList));

            //Laden aller tpos
            string tposQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' AND `What` = 'pos' GROUP By `Name`";
            var posDataList = new ReadRowsQuery<DbJobDataRow>(tposQuery).Execute(Connection)
                .Select(val => val.Name);
            TreeItems.Add(CreateTreeViewItem("tpos", posDataList));
        }

        private static TreeViewItem CreateTreeViewItem(string treeViewName, IEnumerable<string> itemNames)
        {
            var item2 = new TreeViewItem { Text = treeViewName };
            foreach (var item in itemNames)
            {
                item2.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item,
                    Checked = true
                });
            }

            return item2;
        }

        public void CopyJob(Window wnd)
        {
            var keepFrame = new Dictionary<string, bool>();
            foreach (var item in TreeItems[1].SubItems)
                keepFrame.Add(item.Text, !item.Checked);

            var keepProc = new Dictionary<string, bool>();
            foreach (var item in TreeItems[2].SubItems)
                keepProc.Add(item.Text, !item.Checked);

            var keepPos = new Dictionary<string, bool>();
            foreach (var item in TreeItems[3].SubItems)
                keepPos.Add(item.Text, !item.Checked);


            //TODO: Async + Log
            new CopyJobQuery(NewJobName, SelectedJob.Name, keepPos, keepProc, keepFrame,
                !TreeItems[0].SubItems[0].Checked).Execute(Connection);

            Messenger.Default.Send(new JobsChangedMessage() { AddedJob = NewJobName });

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            MessageBox.Show("Kopieren war erfolgreich!");
            Logger.Info("Copied Job {0} as new Job {1}", SelectedJob.Name, NewJobName);
            wnd.Close();
        }
    }
}
