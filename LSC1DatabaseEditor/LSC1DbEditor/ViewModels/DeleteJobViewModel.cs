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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LSC1DatabaseEditor.LSC1DbEditor.ViewModels
{
    public class DeleteJobViewModel : ViewModelBase
    {
        private static readonly LSC1AsyncDBTaskExecuter AsyncExecuter = new LSC1AsyncDBTaskExecuter();
        private static readonly MySqlConnection Connection = new MySqlConnection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        private static readonly LSC1DbFunctionCollection Functions =
            new LSC1DbFunctionCollection(LSC1UserSettings.Instance.DBSettings.ConnectionString);

        public ObservableCollection<TreeViewItem> TreeItems { get; set; } = new ObservableCollection<TreeViewItem>();

        private List<DbJobNameRow> jobs;

        public List<DbJobNameRow> Jobs
        {
            get => jobs;
            set
            {
                jobs = value;
                RaisePropertyChanged();
            }
        }

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
            set
            {
                selectedJob = value;
                JobChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<Window> DeleteCommand { get; set; }

        public DeleteJobViewModel()
        {
            Initialize();
            DeleteCommand = new RelayCommand<Window>(DeleteJob, (wnd) => SelectedJob != null);
        }

        private async void Initialize()
        {
            Jobs = await AsyncExecuter.DoTaskAsync("Load Jobs", () => new GetJobsQuery().Execute(Connection).ToList());
        }

        private async void JobChanged()
        {
            TreeItems.Clear();

            //TODO: make frame handling correct (consider both base frames)
            //Laden des BaseFrames
            string baseFrameQuery = "SELECT FrameT1 FROM `twt` WHERE `WtId` = '" + SelectedJob.JobNr + "'";
            string baseFrame = await AsyncExecuter.DoTaskAsync("Lade alle BaseFrames", () =>
                                    new ReadRowsQuery<DbRow>(baseFrameQuery)
                                        .Execute(Connection)
                                        .Select(val => val.Values[0])
                                        .FirstOrDefault());

            //Berechnen der Anzahl der Vorkommen des BaseFrames in anderen Jobs
            string numOfBaseFrameOccurencesQuery = "SELECT * FROM `twt` WHERE FrameT1 ='" + baseFrame + "'";
            var baseFrameOccurences = await AsyncExecuter.DoTaskAsync("Lade BaseFrame", () =>
                new ReadRowsQuery<DbTwtRow>(numOfBaseFrameOccurencesQuery).Execute(Connection).ToList());
            TreeItems.Add(CreateBaseFrameTreeViewItems(baseFrame, baseFrameOccurences));

            //Laden von tprocdata
            TreeItems.Add(CreateTreeViewItem("tproc", await AsyncExecuter.DoTaskAsync("Lade tprocdata",
                 () => new GetNamesOfJobQuery(SelectedJob.JobNr, "proc").Execute(Connection).ToList())));
            //Laden aller tpos
            TreeItems.Add(CreateTreeViewItem("tpos", await AsyncExecuter.DoTaskAsync("Lade tpos", 
                () => new GetNamesOfJobQuery(SelectedJob.JobNr, "pos").Execute(Connection).ToList())));
            
            TreeItems.Add(CreateTreeViewItem("tturn", await AsyncExecuter.DoTaskAsync("Lade tturn...", 
                () => new GetNamesOfJobQuery(SelectedJob.JobNr, "turn").Execute(Connection).ToList())));

            TreeItems.Add(CreateTreeViewItem("tpulse", await AsyncExecuter.DoTaskAsync("Lade tpulse...",
                () => new GetNamesOfJobQuery(SelectedJob.JobNr, "pulse").Execute(Connection).ToList())));

            //Laden aller frames
            string framesQuery = "SELECT * FROM `tjobdata` WHERE `JobNr` = '" + SelectedJob.JobNr + "' GROUP BY `Frame`";
            var frameList = await AsyncExecuter.DoTaskAsync("Lade alle Frames", () =>
                new ReadRowsQuery<DbJobDataRow>(framesQuery).Execute(Connection).ToList());
            TreeViewItem item3 = CreateFrameTreeViewItems(frameList);
            TreeItems.Add(item3);

            Messenger.Default.Send(new TreeViewBuiltMessage());
        }

        private static TreeViewItem CreateBaseFrameTreeViewItems(string baseFrame, List<DbTwtRow> baseFrameOccurences)
        {
            var item0 = new TreeViewItem
            {
                Text = "base frame"
            };
            var occurenceIds = new List<TextItem>();
            foreach (DbTwtRow item in baseFrameOccurences)
            {
                occurenceIds.Add(new TextItem()
                {
                    Text = item.WtId
                });
            }
            if (baseFrameOccurences.Count > 0)
            {
                item0.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = baseFrame,
                    Checked = baseFrameOccurences.Count() == 1,
                    SubItems = new ObservableCollection<TextItem>(occurenceIds)
                });
            }
            return item0;
        }

        private static TreeViewItem CreateFrameTreeViewItems(IEnumerable<DbJobDataRow> frameList)
        {
            var item3 = new TreeViewItem
            {
                Text = "tframe"
            };
            foreach (DbJobDataRow item in frameList)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithFrame = Functions.FindJobsWithFrame(item.Frame);

                foreach (DbJobNameRow job in jobsWithFrame)
                {
                    subSubItems.Add(new TextItem()
                    {
                        Text = job.Name,
                    });
                }

                item3.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item.Frame,
                    SubItems = subSubItems,
                    Checked = jobsWithFrame.Count <= 1
                });
            }

            return item3;
        }

        private static TreeViewItem CreateTreeViewItem(string name, List<string> jobDataNames)
        {
            var item1 = new TreeViewItem
            {
                Text = name
            };
            foreach (string item in jobDataNames)
            {
                var subSubItems = new ObservableCollection<TextItem>();
                var jobsWithProc = Functions.FindJobsThatUseName(item);

                foreach (DbJobNameRow job in jobsWithProc)
                {
                    subSubItems.Add(new TextItem()
                    {
                        Text = job.Name,
                    });
                }

                item1.SubItems.Add(new CheckableItemWithSub()
                {
                    Text = item,
                    SubItems = subSubItems,
                    Checked = jobsWithProc.Count <= 1
                });
            }

            return item1;
        }

        public async void DeleteJob(Window wnd)
        {
            await AsyncExecuter.DoTaskAsync("Lösche Job" ,() =>
            {
                if (TreeItems.Count > 0)
                    new DeleteJobQuery(SelectedJob.Name).Execute(Connection);
            });

            Messenger.Default.Send(new JobsChangedMessage());

            OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings.ConnectionString);
            MessageBox.Show("Löschen war erfolgreich!");
            wnd.Close();
        }
    }
}
